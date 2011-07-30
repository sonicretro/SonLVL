using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace SonicRetro.SonLVL
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            LevelData.MainForm = this;
            InitializeComponent();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log(e.Exception.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
            System.IO.File.WriteAllLines("SonLVL.log", LogFile.ToArray());
            if (MessageBox.Show("Unhandled Exception " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", "SonLVL Fatal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.No)
                Close();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
            System.IO.File.WriteAllLines("SonLVL.log", LogFile.ToArray());
            MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SonLVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        ImageAttributes imageTransparency = new ImageAttributes();
        Dictionary<string, Dictionary<string, string>> ini;
        Bitmap LevelBmp;
        Graphics LevelGfx;
        Graphics PanelGfx;
        string levelPath;
        string level;
        internal bool loaded;
        Point camera;
        EditingModes EditingMode;
        internal byte SelectedTile;
        internal List<Entry> SelectedItems;
        internal ToolWindow EditControls;
        ObjectList ObjectSelect;
        TileForm TileEditor;
        Stack<UndoAction> UndoList;
        Stack<UndoAction> RedoList;
        internal LogWindow LogWindow;
        internal List<string> LogFile = new List<string>();
        Dictionary<string, ToolStripMenuItem> levelMenuItems;

        internal void Log(params string[] lines)
        {
            LogFile.AddRange(lines);
            if (LogWindow != null)
                LogWindow.Invoke(new MethodInvoker(LogWindow.UpdateLines));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            System.Drawing.Imaging.ColorMatrix x = new System.Drawing.Imaging.ColorMatrix();
            x.Matrix33 = 0.75f;
            imageTransparency.SetColorMatrix(x, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            if (System.Diagnostics.Debugger.IsAttached)
                logToolStripMenuItem_Click(sender, e);
            if (Program.args.Length > 0)
            {
                PanelGfx = panel1.CreateGraphics();
                PanelGfx.SetOptions();
                Log("Opening INI file \"" + Program.args[0] + "\"...");
                ini = IniFile.Load(Program.args[0]);
                Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(Program.args[0]);
                changeLevelToolStripMenuItem.DropDownItems.Clear();
                levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
                foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
                {
                    if (!string.IsNullOrEmpty(item.Key))
                    {
                        string[] itempath = item.Key.Split('\\');
                        ToolStripMenuItem parent = changeLevelToolStripMenuItem;
                        for (int i = 0; i < itempath.Length - 1; i++)
                        {
                            string curpath = string.Empty;
                            if (i - 1 >= 0)
                                curpath = string.Join("\\", itempath, 0, i - 1);
                            if (!string.IsNullOrEmpty(curpath))
                                parent = levelMenuItems[curpath];
                            curpath += itempath[i];
                            if (!levelMenuItems.ContainsKey(curpath))
                            {
                                ToolStripMenuItem it = new ToolStripMenuItem(itempath[i].Replace("&", "&&")) { Tag = curpath };
                                levelMenuItems.Add(curpath, it);
                                parent.DropDownItems.Add(it);
                                parent = it;
                            }
                            else
                                parent = levelMenuItems[curpath];
                        }
                        ToolStripMenuItem ts = new ToolStripMenuItem(itempath[itempath.Length - 1], null, new EventHandler(LevelToolStripMenuItem_Clicked)) { Tag = item.Key };
                        levelMenuItems.Add(item.Key, ts);
                        parent.DropDownItems.Add(ts);
                    }
                }
                try
                {
                    LevelData.EngineVersion = (EngineVersion)Enum.Parse(typeof(EngineVersion), ini[string.Empty].GetValueOrDefault("version", "S2"));
                }
                catch
                {
                    LevelData.EngineVersion = EngineVersion.Invalid;
                }
                LevelData.littleendian = false;
                switch (LevelData.EngineVersion)
                {
                    case EngineVersion.S1:
                        LevelData.chunksz = 256;
                        Icon = Properties.Resources.gogglemon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                        break;
                    case EngineVersion.SCDPC:
                        LevelData.chunksz = 256;
                        Icon = Properties.Resources.gogglemon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                        LevelData.littleendian = true;
                        break;
                    case EngineVersion.S2:
                        LevelData.chunksz = 128;
                        Icon = Properties.Resources.telemon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                        break;
                    case EngineVersion.S3K:
                        LevelData.chunksz = 128;
                        Icon = Properties.Resources.watermon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg3K.Copy();
                        break;
                    case EngineVersion.SKC:
                        LevelData.chunksz = 128;
                        Icon = Properties.Resources.lightningmon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg3K.Copy();
                        LevelData.littleendian = true;
                        break;
                    default:
                        throw new NotImplementedException("Game type " + LevelData.EngineVersion.ToString() + " is not supported!");
                }
                Text = LevelData.EngineVersion.ToString() + "LVL";
                Log("Game type is " + LevelData.EngineVersion.ToString() + ".");
                buildAndRunToolStripMenuItem.Enabled = ini[string.Empty].ContainsKey("buildscr") & ini[string.Empty].ContainsKey("runcmd");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loaded)
            {
                switch (MessageBox.Show(this, "Do you want to save?", LevelData.EngineVersion.ToString() + "LVL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:                        
                        return;
                }
            }
            OpenFileDialog a = new OpenFileDialog()
            {
                DefaultExt = "ini",
                Filter = "INI Files|*.ini|All Files|*.*"
            };
            if (a.ShowDialog(this) == DialogResult.OK)
            {
                loaded = false;
                PanelGfx = panel1.CreateGraphics();
                PanelGfx.SetOptions();
                Log("Opening INI file \"" + a.FileName + "\"...");
                ini = IniFile.Load(a.FileName);
                Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(a.FileName);
                changeLevelToolStripMenuItem.DropDownItems.Clear();
                levelMenuItems = new Dictionary<string,ToolStripMenuItem>();
                foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
                {
                    if (!string.IsNullOrEmpty(item.Key))
                    {
                        string[] itempath = item.Key.Split('\\');
                        ToolStripMenuItem parent = changeLevelToolStripMenuItem;
                        for (int i = 0; i < itempath.Length - 1; i++)
                        {
                            string curpath = string.Empty;
                            if (i - 1 >= 0)
                                curpath = string.Join("\\", itempath, 0, i - 1);
                            if (!string.IsNullOrEmpty(curpath))
                                parent = levelMenuItems[curpath];
                            curpath += itempath[i];
                            if (!levelMenuItems.ContainsKey(curpath))
                            {
                                ToolStripMenuItem it = new ToolStripMenuItem(itempath[i].Replace("&", "&&")) { Tag = curpath };
                                levelMenuItems.Add(curpath, it);
                                parent.DropDownItems.Add(it);
                                parent = it;
                            }
                            else
                                parent = levelMenuItems[curpath];
                        }
                        ToolStripMenuItem ts = new ToolStripMenuItem(itempath[itempath.Length - 1], null, new EventHandler(LevelToolStripMenuItem_Clicked)) { Tag = item.Key };
                        levelMenuItems.Add(item.Key, ts);
                        parent.DropDownItems.Add(ts);
                    }
                }
                try
                {
                    LevelData.EngineVersion = (EngineVersion)Enum.Parse(typeof(EngineVersion), ini[string.Empty].GetValueOrDefault("version", "S2"));
                }
                catch
                {
                    LevelData.EngineVersion = EngineVersion.Invalid;
                }
                LevelData.littleendian = false;
                switch (LevelData.EngineVersion)
                {
                    case EngineVersion.S1:
                        LevelData.chunksz = 256;
                        Icon = Properties.Resources.gogglemon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                        break;
                    case EngineVersion.SCDPC:
                        LevelData.chunksz = 256;
                        Icon = Properties.Resources.gogglemon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                        LevelData.littleendian = true;
                        break;
                    case EngineVersion.S2:
                        LevelData.chunksz = 128;
                        Icon = Properties.Resources.telemon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                        break;
                    case EngineVersion.S3K:
                        LevelData.chunksz = 128;
                        Icon = Properties.Resources.watermon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg3K.Copy();
                        break;
                    case EngineVersion.SKC:
                        LevelData.chunksz = 128;
                        Icon = Properties.Resources.lightningmon;
                        LevelData.UnknownImg = Properties.Resources.UnknownImg3K.Copy();
                        LevelData.littleendian = true;
                        break;
                    default:
                        throw new NotImplementedException("Game type " + LevelData.EngineVersion.ToString() + " is not supported!");
                }
                Text = LevelData.EngineVersion.ToString() + "LVL";
                Log("Game type is " + LevelData.EngineVersion.ToString() + ".");
                buildAndRunToolStripMenuItem.Enabled = ini[string.Empty].ContainsKey("buildscr") & ini[string.Empty].ContainsKey("runcmd");
            }
        }

        private void LevelToolStripMenuItem_Clicked(object sender, EventArgs e)
        {
            if (loaded)
            {
                fileToolStripMenuItem.DropDown.Hide();
                switch (MessageBox.Show(this, "Do you want to save?", LevelData.EngineVersion.ToString() + "LVL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            loaded = false;
            foreach (KeyValuePair<string, ToolStripMenuItem> item in levelMenuItems)
            {
                item.Value.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            level = ((ToolStripMenuItem)sender).Text;
            levelPath = (string)((ToolStripMenuItem)sender).Tag;
            Enabled = false;
            Text = LevelData.EngineVersion.ToString() + "LVL - Loading " + level + "...";
            Log("Loading " + level + "...");
#if !DEBUG
            backgroundWorker1.RunWorkerAsync();
#else
            backgroundWorker1_DoWork(null, null);
            backgroundWorker1_RunWorkerCompleted(null, null);
#endif
        }

        bool initerror = false;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
#if !DEBUG
            try
            {
#endif
                camera = new Point();
                SelectedTile = 0;
                UndoList = new Stack<UndoAction>();
                RedoList = new Stack<UndoAction>();
                Dictionary<string, string> gr = ini[levelPath];
                LevelData.TileFmt = gr.ContainsKey("tile8fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["tile8fmt"]) : LevelData.EngineVersion;
                LevelData.BlockFmt = gr.ContainsKey("block16fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["block16fmt"]) : LevelData.EngineVersion;
                LevelData.ChunkFmt = gr.ContainsKey("chunkfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["chunkfmt"]) : LevelData.EngineVersion;
                LevelData.LayoutFmt = gr.ContainsKey("layoutfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["layoutfmt"]) : LevelData.EngineVersion;
                LevelData.PaletteFmt = gr.ContainsKey("palettefmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["palettefmt"]) : LevelData.EngineVersion;
                LevelData.ObjectFmt = gr.ContainsKey("objectsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["objectsfmt"]) : LevelData.EngineVersion;
                LevelData.RingFmt = gr.ContainsKey("ringsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["ringsfmt"]) : LevelData.EngineVersion;
                switch (LevelData.ChunkFmt)
                {
                    case EngineVersion.S1:
                    case EngineVersion.SCDPC:
                        LevelData.chunksz = 256;
                        break;
                    case EngineVersion.S2:
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        LevelData.chunksz = 128;
                        break;
                }
                string defcmp;
                string[] tilelist = gr["tile8"].Split('|');
                byte[] tmp = null;
                List<byte> data = new List<byte>();
                LevelData.Tiles = new MultiFileIndexer<byte>();
                if (LevelData.TileFmt != EngineVersion.SCDPC)
                {
                    switch (LevelData.TileFmt)
                    {
                        case EngineVersion.S1:
                            defcmp = "Nemesis";
                            break;
                        case EngineVersion.S2:
                            defcmp = "Kosinski";
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            defcmp = "KosinskiM";
                            break;
                        default:
                            defcmp = string.Empty;
                            break;
                    }
                    foreach (string tileent in tilelist)
                    {
                        tmp = null;
                        string[] tileentsp = tileent.Split(':');
                        if (File.Exists(tileentsp[0]))
                        {
                            Log("Loading 8x8 tiles from file \"" + tileentsp[0] + "\", using compression " + gr.GetValueOrDefault("tile8cmp", defcmp) + "...");
                            tmp = Compression.Decompress(tileentsp[0], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("tile8cmp", defcmp)));
                            int off = -1;
                            if (tileentsp.Length > 1)
                            {
                                string offstr = tileentsp[1];
                                if (offstr.StartsWith("0x"))
                                    off = int.Parse(offstr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                                else
                                    off = int.Parse(offstr, System.Globalization.NumberStyles.Integer);
                            }
                            LevelData.Tiles.AddFile(new List<byte>(tmp), off);
                        }
                        else
                            Log("8x8 tile file \"" + tileentsp[0] + "\" not found.");
                    }
                    LevelData.TilesArray = LevelData.Tiles.ToArray();
                }
                else
                {
                    if (File.Exists(gr["tile8"]))
                    {
                        Log("Loading 8x8 tiles from file \"" + gr["tile8"] + "\", using compression SZDD...");
                        tmp = Compression.Decompress(gr["tile8"], Compression.CompressionType.SZDD);
                        int tcnt = BitConverter.ToInt32(tmp, 8);
                        int sta = BitConverter.ToInt32(tmp, 0xC);
                        byte[] tiles = new byte[tcnt * 32];
                        Array.Copy(tmp, sta, tiles, 0, tiles.Length);
                        LevelData.Tiles.AddFile(new List<byte>(tiles), -1);
                    }
                    else
                        Log("8x8 tile file \"" + gr["tile8"] + "\" not found.");
                    if (LevelData.Tiles.Count == 0)
                        LevelData.Tiles.AddFile(new List<byte>(new byte[32]), -1);
                    LevelData.TilesArray = LevelData.Tiles.ToArray();
                }
                LevelData.Blocks = new MultiFileIndexer<Block>();
                switch (LevelData.BlockFmt)
                {
                    case EngineVersion.S1:
                        defcmp = "Enigma";
                        break;
                    case EngineVersion.S2:
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        defcmp = "Kosinski";
                        break;
                    case EngineVersion.SCDPC:
                        defcmp = "Uncompressed";
                        break;
                    default:
                        defcmp = string.Empty;
                        break;
                }
                tilelist = gr["block16"].Split('|');
                foreach (string tileent in tilelist)
                {
                    string[] tileentsp = tileent.Split(':');
                    if (File.Exists(tileentsp[0]))
                    {
                        Log("Loading 16x16 blocks from file \"" + tileentsp[0] + "\", using compression " + gr.GetValueOrDefault("block16cmp", defcmp) + "...");
                        tmp = Compression.Decompress(tileentsp[0], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("block16cmp", defcmp)));
                        List<Block> tmpblk = new List<Block>();
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = false;
                        for (int ba = 0; ba < tmp.Length; ba += Block.Size)
                        {
                            tmpblk.Add(new Block(tmp, ba));
                        }
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = true;
                        int off = -1;
                        if (tileentsp.Length > 1)
                        {
                            string offstr = tileentsp[1];
                            if (offstr.StartsWith("0x"))
                                off = int.Parse(offstr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                            else
                                off = int.Parse(offstr, System.Globalization.NumberStyles.Integer);
                        }
                        LevelData.Blocks.AddFile(tmpblk, off == -1 ? off : off / Block.Size);
                    }
                    else
                        Log("16x16 chunk file \"" + tileentsp[0] + "\" not found.");
                }
                if (LevelData.Blocks.Count == 0)
                    LevelData.Blocks.AddFile(new List<Block>() { new Block() }, -1);
                LevelData.Chunks = new MultiFileIndexer<Chunk>();
                switch (LevelData.ChunkFmt)
                {
                    case EngineVersion.S1:
                    case EngineVersion.S2:
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        defcmp = "Kosinski";
                        break;
                    case EngineVersion.SCDPC:
                        defcmp = "Uncompressed";
                        break;
                    default:
                        defcmp = string.Empty;
                        break;
                }
                tilelist = gr["chunk" + LevelData.chunksz].Split('|');
                data = new List<byte>();
                int fileind = 0;
                foreach (string tileent in tilelist)
                {
                    string[] tileentsp = tileent.Split(':');
                    if (File.Exists(tileentsp[0]))
                    {
                        Log("Loading " + LevelData.chunksz + "x" + LevelData.chunksz + " chunks from file \"" + tileentsp[0] + "\", using compression " + gr.GetValueOrDefault("chunk" + LevelData.chunksz + "cmp", defcmp) + "...");
                        tmp = Compression.Decompress(tileentsp[0], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("chunk" + LevelData.chunksz + "cmp", defcmp)));
                        List<Chunk> tmpchnk = new List<Chunk>();
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = false;
                        if (fileind == 0)
                        {
                            switch (LevelData.EngineVersion)
                            {
                                case EngineVersion.S1:
                                case EngineVersion.SCD:
                                case EngineVersion.SCDPC:
                                    tmpchnk.Add(new Chunk());
                                    break;
                            }
                        }
                        for (int ba = 0; ba < tmp.Length; ba += Chunk.Size)
                            tmpchnk.Add(new Chunk(tmp, ba));
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = true;
                        int off = -1;
                        if (tileentsp.Length > 1)
                        {
                            string offstr = tileentsp[1];
                            if (offstr.StartsWith("0x"))
                                off = int.Parse(offstr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                            else
                                off = int.Parse(offstr, System.Globalization.NumberStyles.Integer);
                        }
                        LevelData.Chunks.AddFile(tmpchnk, off == -1 ? off : off / Chunk.Size);
                        fileind++;
                    }
                    else
                        Log(LevelData.chunksz + "x" + LevelData.chunksz + " chunk file \"" + tileentsp[0] + "\" not found.");
                }
                if (LevelData.Chunks.Count == 0)
                    LevelData.Chunks.AddFile(new List<Chunk>() { new Chunk() }, -1);
                int fgw, fgh, bgw, bgh;
                LevelData.FGLoop = null;
                LevelData.BGLoop = null;
                switch (LevelData.LayoutFmt)
                {
                    case EngineVersion.S1:
                        int s1xmax = int.Parse(ini[string.Empty]["levelwidthmax"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        int s1ymax = int.Parse(ini[string.Empty]["levelheightmax"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        if (File.Exists(gr["fglayout"]))
                        {
                            Log("Loading FG layout from file \"" + gr["fglayout"] + "\", using compression " + gr.GetValueOrDefault("fglayoutcmp", "Uncompressed") + "...");
                            tmp = Compression.Decompress(gr["fglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("fglayoutcmp", "Uncompressed")));
                            fgw = tmp[0] + 1;
                            fgh = tmp[1] + 1;
                            LevelData.FGLayout = new byte[fgw, fgh];
                            LevelData.FGLoop = new bool[fgw, fgh];
                            for (int lr = 0; lr < fgh; lr++)
                                for (int lc = 0; lc < fgw; lc++)
                                {
                                    if ((lr * fgw) + lc + 2 >= tmp.Length) break;
                                    LevelData.FGLayout[lc, lr] = (byte)(tmp[(lr * fgw) + lc + 2] & 0x7F);
                                    LevelData.FGLoop[lc, lr] = (tmp[(lr * fgw) + lc + 2] & 0x80) == 0x80;
                                }
                        }
                        else
                        {
                            Log("FG layout file \"" + gr["fglayout"] + "\" not found.");
                            LevelData.FGLayout = new byte[s1xmax, s1ymax];
                            LevelData.FGLoop = new bool[s1xmax, s1ymax];
                        }
                        if (File.Exists(gr["bglayout"]))
                        {
                            Log("Loading BG layout from file \"" + gr["bglayout"] + "\", using compression " + gr.GetValueOrDefault("bglayoutcmp", "Uncompressed") + "...");
                            tmp = Compression.Decompress(gr["bglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("bglayoutcmp", "Uncompressed")));
                            bgw = tmp[0] + 1;
                            bgh = tmp[1] + 1;
                            LevelData.BGLayout = new byte[bgw, bgh];
                            LevelData.BGLoop = new bool[bgw, bgh];
                            for (int lr = 0; lr < bgh; lr++)
                                for (int lc = 0; lc < bgw; lc++)
                                {
                                    LevelData.BGLayout[lc, lr] = (byte)(tmp[(lr * bgw) + lc + 2] & 0x7F);
                                    LevelData.BGLoop[lc, lr] = (tmp[(lr * bgw) + lc + 2] & 0x80) == 0x80;
                                }
                        }
                        else
                        {
                            Log("BG layout file \"" + gr["bglayout"] + "\" not found.");
                            LevelData.BGLayout = new byte[s1xmax, s1ymax];
                            LevelData.BGLoop = new bool[s1xmax, s1ymax];
                        }
                        break;
                    case EngineVersion.S2:
                        LevelData.FGLayout = new byte[128, 16];
                        LevelData.BGLayout = new byte[128, 16];
                        if (File.Exists(gr["layout"]))
                        {
                            Log("Loading layout from file \"" + gr["layout"] + "\", using compression " + gr.GetValueOrDefault("layoutcmp", "Kosinski") + "...");
                            tmp = Compression.Decompress(gr["layout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", "Kosinski")));
                            for (int la = 0; la < tmp.Length; la += 256)
                            {
                                for (int laf = 0; laf < 128; laf++)
                                    LevelData.FGLayout[laf, la / 256] = tmp[la + laf];
                                for (int lab = 0; lab < 128; lab++)
                                    LevelData.BGLayout[lab, la / 256] = tmp[la + lab + 128];
                            }
                        }
                        else
                            Log("Layout file \"" + gr["layout"] + "\" not found.");
                        break;
                    case EngineVersion.S3K:
                        if (File.Exists(gr["layout"]))
                        {
                            Log("Loading layout from file \"" + gr["layout"] + "\", using compression " + gr.GetValueOrDefault("layoutcmp", "Uncompressed") + "...");
                            tmp = Compression.Decompress(gr["layout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", "Uncompressed")));
                            fgw = ByteConverter.ToUInt16(tmp, 0);
                            bgw = ByteConverter.ToUInt16(tmp, 2);
                            fgh = ByteConverter.ToUInt16(tmp, 4);
                            bgh = ByteConverter.ToUInt16(tmp, 6);
                            LevelData.FGLayout = new byte[fgw, fgh];
                            LevelData.BGLayout = new byte[bgw, bgh];
                            for (int la = 0; la < Math.Max(fgh, bgh) * 4; la += 4)
                            {
                                ushort lfp = ByteConverter.ToUInt16(tmp, 8 + la);
                                if (lfp != 0)
                                    for (int laf = 0; laf < fgw; laf++)
                                        LevelData.FGLayout[laf, la / 4] = tmp[lfp - 0x8000 + laf];
                                ushort lbp = ByteConverter.ToUInt16(tmp, 8 + la + 2);
                                if (lbp != 0)
                                    for (int lab = 0; lab < bgw; lab++)
                                        LevelData.BGLayout[lab, la / 4] = tmp[lbp - 0x8000 + lab];
                            }
                        }
                        else
                        {
                            Log("Layout file \"" + gr["layout"] + "\" not found.");
                            LevelData.FGLayout = new byte[128, 16];
                            LevelData.BGLayout = new byte[128, 16];
                        }
                        break;
                    case EngineVersion.SKC:
                        if (File.Exists(gr["layout"]))
                        {
                            Log("Loading layout from file \"" + gr["layout"] + "\", using compression " + gr.GetValueOrDefault("layoutcmp", "Uncompressed") + "...");
                            tmp = Compression.Decompress(gr["layout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", "Uncompressed")));
                            fgw = ByteConverter.ToUInt16(tmp, 0);
                            bgw = ByteConverter.ToUInt16(tmp, 2);
                            fgh = ByteConverter.ToUInt16(tmp, 4);
                            bgh = ByteConverter.ToUInt16(tmp, 6);
                            LevelData.FGLayout = new byte[fgw, fgh];
                            LevelData.BGLayout = new byte[bgw, bgh];
                            for (int la = 0; la < Math.Max(fgh, bgh) * 4; la += 4)
                            {
                                ushort lfp = ByteConverter.ToUInt16(tmp, 8 + la);
                                if (lfp != 0)
                                    for (int laf = 0; laf < fgw; laf++)
                                        LevelData.FGLayout[laf, la / 4] = tmp[(lfp - 0x8000 + laf) ^ 1];
                                ushort lbp = ByteConverter.ToUInt16(tmp, 8 + la + 2);
                                if (lbp != 0)
                                    for (int lab = 0; lab < bgw; lab++)
                                        LevelData.BGLayout[lab, la / 4] = tmp[(lbp - 0x8000 + lab) ^ 1];
                            }
                        }
                        else
                        {
                            Log("Layout file \"" + gr["layout"] + "\" not found.");
                            LevelData.FGLayout = new byte[128, 16];
                            LevelData.BGLayout = new byte[128, 16];
                        }
                        break;
                    case EngineVersion.SCDPC:
                        LevelData.FGLayout = new byte[64, 8];
                        LevelData.FGLoop = new bool[64, 8];
                        if (File.Exists(gr["fglayout"]))
                        {
                            Log("Loading FG layout from file \"" + gr["fglayout"] + "\", using compression " + gr.GetValueOrDefault("fglayoutcmp", "Uncompressed") + "...");
                            tmp = Compression.Decompress(gr["fglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("fglayoutcmp", "Uncompressed")));
                            for (int lr = 0; lr < 8; lr++)
                                for (int lc = 0; lc < 64; lc++)
                                {
                                    if ((lr * 64) + lc >= tmp.Length) break;
                                    LevelData.FGLayout[lc, lr] = (byte)(tmp[(lr * 64) + lc] & 0x7F);
                                    LevelData.FGLoop[lc, lr] = (tmp[(lr * 64) + lc] & 0x80) == 0x80;
                                }
                        }
                        else
                            Log("FG layout file \"" + gr["fglayout"] + "\" not found.");
                        LevelData.BGLayout = new byte[64, 8];
                        LevelData.BGLoop = new bool[64, 8];
                        if (File.Exists(gr["bglayout"]))
                        {
                            Log("Loading BG layout from file \"" + gr["bglayout"] + "\", using compression " + gr.GetValueOrDefault("bglayoutcmp", "Uncompressed") + "...");
                            tmp = Compression.Decompress(gr["bglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("bglayoutcmp", "Uncompressed")));
                            for (int lr = 0; lr < 8; lr++)
                                for (int lc = 0; lc < 64; lc++)
                                {
                                    LevelData.BGLayout[lc, lr] = (byte)(tmp[(lr * 64) + lc] & 0x7F);
                                    LevelData.BGLoop[lc, lr] = (tmp[(lr * 64) + lc] & 0x80) == 0x80;
                                }
                        }
                        else
                            Log("BG layout file \"" + gr["bglayout"] + "\" not found.");
                        break;
                }
                LevelData.PalName = new List<string>() { "Normal" };
                LevelData.Palette = new List<ushort[,]>() { new ushort[4, 16] };
                LevelData.PalNum = new List<byte[,]>() { new byte[4, 16] };
                LevelData.PalAddr = new List<int[,]>() { new int[4, 16] };
                byte palfilenum = 0;
                string[] palentstr;
                if (gr.ContainsKey("palette"))
                {
                    palentstr = gr["palette"].Split('|');
                    for (byte pn = 0; pn < palentstr.Length; pn++)
                    {
                        string[] palent = palentstr[pn].Split(':');
                        Log("Loading palette file \"" + palent[0] + "\"...", "Source: " + palent[1] + " Destination: " + palent[2] + " Length: " + palent[3]);
                        tmp = System.IO.File.ReadAllBytes(palent[0]);
                        ushort[] palfile;
                        if (LevelData.PaletteFmt != EngineVersion.SCDPC)
                        {
                            palfile = new ushort[tmp.Length / 2];
                            for (int pi = 0; pi < tmp.Length; pi += 2)
                                palfile[pi / 2] = ByteConverter.ToUInt16(tmp, pi);
                        }
                        else
                        {
                            palfile = new ushort[tmp.Length / 4];
                            for (int pi = 0; pi < tmp.Length; pi += 4)
                                palfile[pi / 4] = (ushort)((tmp[pi] >> 4) | (tmp[pi + 1] & 0xF0) | ((tmp[pi + 2] >> 4) << 8));
                        }
                        int src = int.Parse(palent[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        int dest = int.Parse(palent[2], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        for (int pa = 0; pa < int.Parse(palent[3], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture); pa++)
                        {
                            LevelData.Palette[0][(pa + dest) / 16, (pa + dest) % 16] = palfile[pa + src];
                            LevelData.PalNum[0][(pa + dest) / 16, (pa + dest) % 16] = palfilenum;
                            LevelData.PalAddr[0][(pa + dest) / 16, (pa + dest) % 16] = pa + src;
                        }
                        palfilenum++;
                    }
                }
                int palnum = 2;
                while (gr.ContainsKey("palette" + palnum))
                {
                    palentstr = gr["palette" + palnum].Split('|');
                    LevelData.PalName.Add(palentstr[0]);
                    LevelData.Palette.Add(new ushort[4, 16]);
                    LevelData.PalNum.Add(new byte[4, 16]);
                    LevelData.PalAddr.Add(new int[4, 16]);
                    for (byte pn = 1; pn < palentstr.Length; pn++)
                    {
                        string[] palent = palentstr[pn].Split(':');
                        Log("Loading palette file \"" + palent[0] + "\"...", "Source: " + palent[1] + " Destination: " + palent[2] + " Length: " + palent[3]);
                        tmp = System.IO.File.ReadAllBytes(palent[0]);
                        ushort[] palfile;
                        if (LevelData.PaletteFmt != EngineVersion.SCDPC)
                        {
                            palfile = new ushort[tmp.Length / 2];
                            for (int pi = 0; pi < tmp.Length; pi += 2)
                                palfile[pi / 2] = ByteConverter.ToUInt16(tmp, pi);
                        }
                        else
                        {
                            palfile = new ushort[tmp.Length / 4];
                            for (int pi = 0; pi < tmp.Length; pi += 4)
                                palfile[pi / 4] = (ushort)((tmp[pi] >> 4) | (tmp[pi + 1] & 0xF0) | ((tmp[pi + 2] >> 4) << 8));
                        }
                        int src = int.Parse(palent[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        int dest = int.Parse(palent[2], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        for (int pa = 0; pa < int.Parse(palent[3], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture); pa++)
                        {
                            LevelData.Palette[LevelData.Palette.Count - 1][(pa + dest) / 16, (pa + dest) % 16] = palfile[pa + src];
                            LevelData.PalNum[LevelData.Palette.Count - 1][(pa + dest) / 16, (pa + dest) % 16] = palfilenum;
                            LevelData.PalAddr[LevelData.Palette.Count - 1][(pa + dest) / 16, (pa + dest) % 16] = pa + src;
                        }
                        palfilenum++;
                    }
                    palnum++;
                }
                LevelData.CurPal = 0;
                Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
                LevelData.BmpPal = palbmp.Palette;
                for (int i = 0; i < 64; i++)
                    LevelData.BmpPal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, true);
                for (int i = 64; i < 256; i++)
                    LevelData.BmpPal.Entries[i] = Color.Black;
                palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
                LevelImgPalette = palbmp.Palette;
                for (int i = 0; i < 64; i++)
                    LevelImgPalette.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, true);
                for (int i = 64; i < 256; i++)
                    LevelImgPalette.Entries[i] = Color.Black;
                LevelImgPalette.Entries[0] = LevelData.PaletteToColor(2, 0, false);
                LevelImgPalette.Entries[64] = Color.White;
                LevelImgPalette.Entries[65] = Color.Yellow;
                LevelImgPalette.Entries[66] = Color.Black;
                LevelData.UnknownImg.Palette = LevelData.BmpPal;
                LevelData.Sprites = new List<SCDPCSprite>();
                if (gr.ContainsKey("sprites"))
                {
                    tmp = Compression.Decompress(gr["sprites"], Compression.CompressionType.SZDD);
                    int numspr = BitConverter.ToInt32(tmp, 8);
                    int taddr = BitConverter.ToInt32(tmp, 0xC);
                    for (int i = 0; i < numspr; i++)
                    {
                        ushort width = BitConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 4);
                        if ((width & 4) == 4)
                            width += 4;
                        ushort height = BitConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 6);
                        ushort startcol = BitConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 8);
                        BitmapBits bmp = new BitmapBits(width, height);
                        byte[] til = new byte[height * (width / 2)];
                        Array.Copy(tmp, taddr, til, 0, til.Length);
                        taddr += til.Length;
                        LevelData.LoadBitmap4BppIndexed(bmp, til, width / 2);
                        bmp.IncrementIndexes(startcol / 2);
                        LevelData.Sprites.Add(new SCDPCSprite(bmp, new Point(BitConverter.ToInt16(tmp, 0x10 + (i * 0xC) + 0), BitConverter.ToInt16(tmp, 0x10 + (i * 0xC) + 2))));
                    }
                }
                LevelData.ObjTypes = new Dictionary<byte, ObjectDefinition>();
                LevelData.filecache = new Dictionary<string, byte[]>();
                LevelData.unkobj = new DefaultObjectDefinition();
                LevelData.unkobj.Init(new Dictionary<string, string> { {"name", "Unknown"} });
                if (!System.IO.Directory.Exists("dllcache"))
                {
                    System.IO.DirectoryInfo dir = System.IO.Directory.CreateDirectory("dllcache");
                    dir.Attributes |= System.IO.FileAttributes.Hidden;
                }
                Dictionary<string, Dictionary<string,string>> objini = new Dictionary<string,Dictionary<string,string>>();
                LevelData.S2RingDef = new DefS2RingDef();
                LevelData.S2RingDef.Init(null);
                LevelData.S3KRingDef = new S3KRingDefinition();
                if (ini[string.Empty].ContainsKey("objlst"))
                {
                    LoadObjectDefinitions(ini[string.Empty]["objlst"]);
                    objini = IniFile.Load(ini[string.Empty]["objlst"]);
                }
                if (gr.ContainsKey("objlst")) LoadObjectDefinitions(gr["objlst"]);
                LevelData.Objects = new List<ObjectEntry>();
                if (gr.ContainsKey("objects"))
                {
                    if (File.Exists(gr["objects"]))
                    {
                        Log("Loading objects from file \"" + gr["objects"] + "\", using compression " + gr.GetValueOrDefault("objectscmp", "Uncompressed") + "...");
                        tmp = Compression.Decompress(gr["objects"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("objectscmp", "Uncompressed")));
                        switch (LevelData.ObjectFmt)
                        {
                            case EngineVersion.S1:
                                for (int oa = 0; oa < tmp.Length; oa += S1ObjectEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    ObjectEntry ent = new S1ObjectEntry(tmp, oa);
                                    LevelData.Objects.Add(ent);
                                    LevelData.ChangeObjectType(ent);
                                }
                                break;
                            case EngineVersion.S2:
                                for (int oa = 0; oa < tmp.Length; oa += S2ObjectEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    ObjectEntry ent = new S2ObjectEntry(tmp, oa);
                                    LevelData.Objects.Add(ent);
                                    LevelData.ChangeObjectType(ent);
                                }
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                for (int oa = 0; oa < tmp.Length; oa += S3KObjectEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    ObjectEntry ent = new S3KObjectEntry(tmp, oa);
                                    LevelData.Objects.Add(ent);
                                    LevelData.ChangeObjectType(ent);
                                }
                                break;
                            case EngineVersion.SCDPC:
                                for (int oa = 0; oa < tmp.Length; oa += SCDObjectEntry.Size)
                                {
                                    if (ByteConverter.ToUInt64(tmp, oa) == 0xFFFFFFFFFFFFFFFF) break;
                                    ObjectEntry ent = new SCDObjectEntry(tmp, oa);
                                    LevelData.Objects.Add(ent);
                                    LevelData.ChangeObjectType(ent);
                                }
                                break;
                        }
                    }
                    else
                        Log("Object file \"" + gr["objects"] + "\" not found.");
                }
                LevelData.Rings = new List<RingEntry>();
                if (gr.ContainsKey("rings"))
                {
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S2:
                            if (File.Exists(gr["rings"]))
                            {
                                Log("Loading rings from file \"" + gr["rings"] + "\", using compression " + gr.GetValueOrDefault("ringscmp", "Uncompressed") + "...");
                                tmp = Compression.Decompress(gr["rings"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("ringscmp", "Uncompressed")));
                                for (int oa = 0; oa < tmp.Length; oa += S2RingEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    LevelData.Rings.Add(new S2RingEntry(tmp, oa));
                                }
                            }
                            else
                                Log("Ring file \"" + gr["rings"] + "\" not found.");
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            if (File.Exists(gr["rings"]))
                            {
                                Log("Loading rings from file \"" + gr["rings"] + "\", using compression " + gr.GetValueOrDefault("ringscmp", "Uncompressed") + "...");
                                tmp = Compression.Decompress(gr["rings"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("ringscmp", "Uncompressed")));
                                for (int oa = 4; oa < tmp.Length; oa += S3KRingEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    LevelData.Rings.Add(new S3KRingEntry(tmp, oa));
                                }
                            }
                            else
                                Log("Ring file \"" + gr["rings"] + "\" not found.");
                            break;
                    }
                }
                if (gr.ContainsKey("bumpers"))
                {
                    LevelData.Bumpers = new List<CNZBumperEntry>();
                    if (File.Exists(gr["bumpers"]))
                    {
                        Log("Loading bumpers from file \"" + gr["bumpers"] + "\", using compression " + gr.GetValueOrDefault("bumperscmp", "Uncompressed") + "...");
                        tmp = Compression.Decompress(gr["bumpers"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("bumperscmp", "Uncompressed")));
                        for (int i = 0; i < tmp.Length; i += CNZBumperEntry.Size)
                        {
                            if (ByteConverter.ToUInt16(tmp, i + 2) == 0xFFFF) break;
                            LevelData.Bumpers.Add(new CNZBumperEntry(tmp, i));
                        }
                    }
                    else
                        Log("Bumper file \"" + gr["bumpers"] + "\" not found.");
                }
                else
                {
                    LevelData.Bumpers = null;
                }
                LevelData.StartPositions = new List<StartPositionEntry>();
                LevelData.StartPosDefs = new List<StartPositionDefinition>();
                if (gr.ContainsKey("startpos"))
                {
                    string[] stposs = gr["startpos"].Split('|');
                    foreach (string item in stposs)
                    {
                        string[] stpos = item.Split(':');
                        if (File.Exists(stpos[0]))
                        {
                            Log("Loading start position \"" + stpos[2] + "\" from file \"" + stpos[0] + "\"...");
                            LevelData.StartPositions.Add(new StartPositionEntry(System.IO.File.ReadAllBytes(stpos[0]), 0));
                            if (!string.IsNullOrEmpty(stpos[1]))
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(objini[stpos[1]], stpos[2]));
                            else
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(stpos[2]));
                        }
                        else
                        {
                            Log("Start position file \"" + stpos[0] + "\" not found.");
                            LevelData.StartPositions.Add(new StartPositionEntry());
                            if (!string.IsNullOrEmpty(stpos[1]))
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(objini[stpos[1]], stpos[2]));
                            else
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(stpos[2]));
                        }
                    }
                }
                if (gr.ContainsKey("levelsize"))
                {
                    tmp = System.IO.File.ReadAllBytes(gr["levelsize"]);
                    LevelData.LevelBounds = Rectangle.FromLTRB(ByteConverter.ToInt16(tmp, 0), ByteConverter.ToInt16(tmp, 4), ByteConverter.ToInt16(tmp, 2), ByteConverter.ToInt16(tmp, 6));
                    yWrapToolStripMenuItem.Enabled = true;
                    if (LevelData.LevelBounds.Value.Top == 0xFF00)
                        yWrapToolStripMenuItem.Checked = true;
                }
                else
                {
                    LevelData.LevelBounds = null;
                    yWrapToolStripMenuItem.Checked = false;
                    yWrapToolStripMenuItem.Enabled = false;
                }
                LevelData.ColInds1 = new List<byte>();
                LevelData.ColInds2 = new List<byte>();
                switch (LevelData.EngineVersion)
                {
                    case EngineVersion.S1:
                    case EngineVersion.SCD:
                    case EngineVersion.SCDPC:
                        if (gr.ContainsKey("colind") && File.Exists(gr["colind"]))
                            LevelData.ColInds1.AddRange(Compression.Decompress(gr["colind"], Compression.CompressionType.Uncompressed));
                        break;
                    case EngineVersion.S2:
                        if (gr.ContainsKey("colind1") && File.Exists(gr["colind1"]))
                            LevelData.ColInds1.AddRange(Compression.Decompress(gr["colind1"], Compression.CompressionType.Kosinski));
                        if (gr.ContainsKey("colind2") && File.Exists(gr["colind2"]))
                            LevelData.ColInds2.AddRange(Compression.Decompress(gr["colind2"], Compression.CompressionType.Kosinski));
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        if (gr.ContainsKey("colind"))
                        {
                            if (File.Exists(gr["colind"]))
                            {
                                tmp = Compression.Decompress(gr["colind"], Compression.CompressionType.Uncompressed);
                                int colindt = int.Parse(gr.GetValueOrDefault("colindsz", "1"));
                                switch (colindt)
                                {
                                    case 1:
                                        for (int i = 0; i < 0x300; i++)
                                            LevelData.ColInds1.Add(tmp[i]);
                                        for (int i = 0x300; i < 0x600; i++)
                                            LevelData.ColInds2.Add(tmp[i]);
                                        break;
                                    case 2:
                                        for (int i = 0; i < 0x600; i += 2)
                                            LevelData.ColInds1.Add((byte)ByteConverter.ToUInt16(tmp, i));
                                        for (int i = 0x600; i < 0xC00; i += 2)
                                            LevelData.ColInds2.Add((byte)ByteConverter.ToUInt16(tmp, i));
                                        break;
                                }
                            }
                            else
                            {
                                LevelData.ColInds1.AddRange(new byte[0x300]);
                                LevelData.ColInds2.AddRange(new byte[0x300]);
                            }
                        }
                        break;
                }
                if (LevelData.EngineVersion != EngineVersion.S3K && LevelData.EngineVersion != EngineVersion.SKC)
                {
                    if (LevelData.ColInds1.Count < LevelData.Blocks.Count)
                        LevelData.ColInds1.AddRange(new byte[LevelData.Blocks.Count - LevelData.ColInds1.Count]);
                    if (LevelData.ColInds2.Count < LevelData.Blocks.Count)
                        LevelData.ColInds2.AddRange(new byte[LevelData.Blocks.Count - LevelData.ColInds2.Count]);
                }
                LevelData.ColArr1 = new sbyte[256][];
                if (ini[string.Empty].ContainsKey("colarr1") && File.Exists(ini[string.Empty]["colarr1"]))
                    tmp = Compression.Decompress(ini[string.Empty]["colarr1"], Compression.CompressionType.Uncompressed);
                else
                    tmp = new byte[256 * 16];
                for (int i = 0; i < 256; i++)
                {
                    LevelData.ColArr1[i] = new sbyte[16];
                    for (int j = 0; j < 16; j++)
                        LevelData.ColArr1[i][j] = unchecked((sbyte)tmp[(i * 16) + j]);
                }
                if (ini[string.Empty].ContainsKey("angles") && File.Exists(ini[string.Empty]["angles"]))
                    LevelData.Angles = Compression.Decompress(ini[string.Empty]["angles"], Compression.CompressionType.Uncompressed);
                else
                    LevelData.Angles = new byte[256];
                LevelData.BlockBmps = new List<Bitmap[]>();
                LevelData.BlockBmpBits = new List<BitmapBits[]>();
                Log("Drawing block bitmaps...");
                for (int bi = 0; bi < LevelData.Blocks.Count; bi++)
                {
                    LevelData.BlockBmps.Add(new Bitmap[2]);
                    LevelData.BlockBmpBits.Add(new BitmapBits[2]);
                    LevelData.RedrawBlock(bi, false);
                }
                LevelData.ColBmps = new Bitmap[256];
                LevelData.ColBmpBits = new BitmapBits[256];
                for (int ci = 0; ci < 256; ci++)
                    LevelData.RedrawCol(ci, false);
                LevelData.ChunkBmps = new List<Bitmap[]>();
                LevelData.ChunkBmpBits = new List<BitmapBits[]>();
                LevelData.ChunkColBmps = new List<Bitmap[]>();
                LevelData.ChunkColBmpBits = new List<BitmapBits[]>();
                Log("Drawing chunk bitmaps...");
                for (int ci = 0; ci < LevelData.Chunks.Count; ci++)
                {
                    LevelData.ChunkBmps.Add(new Bitmap[2]);
                    LevelData.ChunkBmpBits.Add(new BitmapBits[2]);
                    LevelData.ChunkColBmps.Add(new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Add(new BitmapBits[2]);
                    LevelData.RedrawChunk(ci);
                }
                Log("Creating level bitmap...");
                LevelBmp = new Bitmap(panel1.Width, panel1.Height);
                LevelImg8bpp = new BitmapBits(panel1.Width, panel1.Height);
                LevelGfx = Graphics.FromImage(LevelBmp);
                LevelGfx.SetOptions();
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.GetType().Name + ": " + ex.Message + "\nLog file has been saved to " + System.IO.Path.Combine(Environment.CurrentDirectory, "SonLVL.log") + ".\nSend this to MainMemory on the Sonic Retro forums.",
                    LevelData.EngineVersion.ToString() + "LVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log(ex.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                System.IO.File.WriteAllLines("SonLVL.log", LogFile.ToArray());
                initerror = true;
            }
#endif
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (initerror)
            {
                Close();
                return;
            }
            Log("Load completed.");
            if (TileEditor == null)
                TileEditor = new TileForm();
            if (EditControls == null)
            {
                EditControls = new ToolWindow();
                EditControls.Show(this);
                EditControls.Location = new Point(Right, Top);
                EditControls.ChunkSelector.SelectedIndexChanged += new EventHandler(EditControls_listView1_SelectedIndexChanged);
                EditControls.propertyGrid1.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(EditControls_propertyGrid1_SelectedGridItemChanged);
                EditControls.propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(EditControls_propertyGrid1_PropertyValueChanged);
                Activate();
            }
            EditControls.ChunkSelector.Images = LevelData.ChunkBmps;
            TileEditor.ChunkSelector.Images = LevelData.ChunkBmps;
            EditControls.ChunkSelector.ImageSize = LevelData.chunksz;
            TileEditor.ChunkSelector.ImageSize = LevelData.chunksz;
            TileEditor.BlockSelector.Images = LevelData.BlockBmps;
            TileEditor.BlockSelector.ChangeSize();
            TileEditor.CollisionSelector.Images = new List<Bitmap>(LevelData.ColBmps);
            TileEditor.CollisionSelector.ChangeSize();
            EditControls.ChunkSelector.SelectedIndex = 0;
            EditControls.ChunkSelector.BackColor = LevelData.PaletteToColor(2, 0, false);
            TileEditor.ChunkSelector.SelectedIndex = 0;
            TileEditor.ChunkPicture.Size = new Size(LevelData.chunksz, LevelData.chunksz);
            TileEditor.BlockSelector.SelectedIndex = 0;
            TileEditor.TileSelector.Images.Clear();
            for (int i = 0; i < LevelData.TilesArray.Length / 0x20; i++)
                TileEditor.TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.TilesArray, i, 2));
            TileEditor.TileSelector.SelectedIndex = 0;
            TileEditor.TileSelector.ChangeSize();
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    TileEditor.BlockCollision2.Visible = false;
                    TileEditor.button2.Visible = false;
                    path2ToolStripMenuItem.Visible = false;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    TileEditor.BlockCollision2.Visible = true;
                    TileEditor.button2.Visible = true;
                    path2ToolStripMenuItem.Visible = true;
                    break;
            }
            if (ObjectSelect == null)
            {
                ObjectSelect = new ObjectList();
                ObjectSelect.listView1.SelectedIndexChanged += new EventHandler(ObjectSelect_listView1_SelectedIndexChanged);
                ObjectSelect.listView2.SelectedIndexChanged += new EventHandler(ObjectSelect_listView2_SelectedIndexChanged);
            }
            ObjectSelect.listView1.Items.Clear();
            ObjectSelect.imageList1.Images.Clear();
            foreach (KeyValuePair<byte, ObjectDefinition> item in LevelData.ObjTypes)
            {
                ObjectSelect.imageList1.Images.Add(item.Value.Image().ToBitmap(LevelData.BmpPal).Resize(ObjectSelect.imageList1.ImageSize));
                ObjectSelect.listView1.Items.Add(item.Value.Name(), ObjectSelect.imageList1.Images.Count - 1);
            }
            ObjectSelect.listView2.Items.Clear();
            ObjectSelect.imageList2.Images.Clear();
            Text = LevelData.EngineVersion.ToString() + "LVL - " + level;
            hScrollBar1.Minimum = 0;
            vScrollBar1.Minimum = 0;
            switch (EditingMode)
            {
                case EditingModes.Objects:
                case EditingModes.PlaneA:
                    hScrollBar1.Maximum = (LevelData.FGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.FGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    break;
                case EditingModes.PlaneB:
                    hScrollBar1.Maximum = (LevelData.BGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.BGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    break;
            }
            hScrollBar1.Value = hScrollBar1.Minimum;
            hScrollBar1.SmallChange = 16;
            hScrollBar1.LargeChange = 128;
            vScrollBar1.Value = vScrollBar1.Minimum;
            vScrollBar1.SmallChange = 16;
            vScrollBar1.LargeChange = 128;
            hScrollBar1.Enabled = true;
            vScrollBar1.Enabled = true;
            loaded = true;
            SelectedItems = new List<Entry>();
            undoCtrlZToolStripMenuItem.DropDownItems.Clear();
            redoCtrlYToolStripMenuItem.DropDownItems.Clear();
            saveToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            editorToolStripMenuItem.Enabled = true;
            exportToolStripMenuItem.Enabled = true;
            paletteToolStripMenuItem2.DropDownItems.Clear();
            foreach (string item in LevelData.PalName)
                paletteToolStripMenuItem2.DropDownItems.Add(new ToolStripMenuItem(item));
            ((ToolStripMenuItem)paletteToolStripMenuItem2.DropDownItems[0]).Checked = true;
            blendAlternatePaletteToolStripMenuItem.Enabled = LevelData.Palette.Count > 1;
            SelectedObjectChanged();
            Enabled = true;
            DrawLevel();
        }

        void ObjectSelect_listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            if (ObjectSelect.listView1.SelectedIndices.Count == 0) return;
            if (ObjectSelect.listView2.SelectedIndices.Count == 0) return;
            int sel = ObjectSelect.listView1.SelectedIndices[0];
            int i = 0;
            byte ID = 0;
            foreach (KeyValuePair<byte, ObjectDefinition> item in LevelData.ObjTypes)
            {
                if (i == sel)
                {
                    ID = item.Key;
                    break;
                }
                i++;
            }
            ObjectSelect.numericUpDown1.Value = ID;
            sel = ObjectSelect.listView2.SelectedIndices[0];
            i = 0;
            byte sub = 0;
            foreach (byte item in LevelData.ObjTypes[ID].Subtypes())
            {
                if (i == sel)
                {
                    sub = item;
                    break;
                }
                i++;
            }
            ObjectSelect.numericUpDown2.Value = sub;
        }

        void ObjectSelect_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            if (ObjectSelect.listView1.SelectedIndices.Count == 0) return;
            int sel = ObjectSelect.listView1.SelectedIndices[0];
            int i = 0;
            byte ID = 0;
            foreach (KeyValuePair<byte, ObjectDefinition> item in LevelData.ObjTypes)
            {
                if (i == sel)
                {
                    ID = item.Key;
                    break;
                }
                i++;
            }
            ObjectSelect.numericUpDown1.Value = ID;
            ObjectSelect.numericUpDown2.Value = 0;
            ObjectSelect.listView2.Items.Clear();
            ObjectSelect.imageList2.Images.Clear();
            foreach (byte item in LevelData.ObjTypes[ID].Subtypes())
            {
                ObjectSelect.imageList2.Images.Add(LevelData.ObjTypes[ID].Image(item).ToBitmap(LevelData.BmpPal).Resize(ObjectSelect.imageList2.ImageSize));
                ObjectSelect.listView2.Items.Add(LevelData.ObjTypes[ID].SubtypeName(item), ObjectSelect.imageList2.Images.Count - 1);
            }
        }

        List<object> oldvalues;
        void EditControls_propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            oldvalues = new List<object>();
            foreach (Entry item in SelectedItems)
            {
                oldvalues.Add(item.GetType().GetProperty(e.NewSelection.PropertyDescriptor.Name).GetValue(item, null));
            }
        }

        void EditControls_propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (SelectedItems.Count == 1 && SelectedItems[0] is ObjectEntry && (e.ChangedItem.Label == "ID" | e.ChangedItem.Label == "Subtype"))
            {
                ObjectEntry it = (ObjectEntry)SelectedItems[0];
                EditControls.ObjName.Text = LevelData.getFullObjectName(it.ID, it.SubType);
                EditControls.objPicture.Image = LevelData.getObjectDefinition(it.ID).Image(it.SubType).ToBitmap(LevelData.BmpPal);
            }
            AddUndo(new ObjectPropertyChangedUndoAction(new List<Entry>(SelectedItems), oldvalues, e.ChangedItem.PropertyDescriptor.Name, e.ChangedItem.PropertyDescriptor.DisplayName));
            oldvalues = new List<object>();
            foreach (Entry item in SelectedItems)
            {
                oldvalues.Add(item.GetType().GetProperty(e.ChangedItem.PropertyDescriptor.Name).GetValue(item, null));
            }
            DrawLevel();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Saving " + levelPath + "...");
            Dictionary<string, string> gr = ini[levelPath];
            string defcmp;
            string[] tilelist;
            int fileind = -1;
            if (LevelData.TileFmt != EngineVersion.SCDPC)
            {
                switch (LevelData.TileFmt)
                {
                    case EngineVersion.S1:
                        defcmp = "Nemesis";
                        break;
                    case EngineVersion.S2:
                        defcmp = "Kosinski";
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        defcmp = "KosinskiM";
                        break;
                    default:
                        defcmp = string.Empty;
                        break;
                }
                tilelist = gr["tile8"].Split('|');
                ReadOnlyCollection<ReadOnlyCollection<byte>> tilefiles = LevelData.Tiles.GetFiles();
                foreach (string tileent in tilelist)
                {
                    fileind++;
                    string[] tileentsp = tileent.Split(':');
                    Compression.Compress(new List<byte>(tilefiles[fileind]).ToArray(), tileentsp[0], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("block16cmp", defcmp)));
                }
            }
            List<byte> tmp;
            switch (LevelData.BlockFmt)
            {
                case EngineVersion.S1:
                    defcmp = "Enigma";
                    break;
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    defcmp = "Kosinski";
                    break;
                case EngineVersion.SCDPC:
                    defcmp = "Uncompressed";
                    break;
                default:
                    defcmp = string.Empty;
                    break;
            }
            tilelist = gr["block16"].Split('|');
            fileind = -1;
            ReadOnlyCollection<ReadOnlyCollection<Block>> blockfiles = LevelData.Blocks.GetFiles();
            foreach (string tileent in tilelist)
            {
                fileind++;
                string[] tileentsp = tileent.Split(':');
                tmp = new List<byte>();
                if (LevelData.EngineVersion == EngineVersion.SKC)
                    LevelData.littleendian = false;
                foreach (Block b in blockfiles[fileind])
                {
                    tmp.AddRange(b.GetBytes());
                }
                if (LevelData.EngineVersion == EngineVersion.SKC)
                    LevelData.littleendian = true;
                Compression.Compress(tmp.ToArray(), tileentsp[0], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("block16cmp", defcmp)));
            }
            tilelist = gr["chunk" + LevelData.chunksz].Split('|');
            fileind = -1;
            switch (LevelData.ChunkFmt)
            {
                case EngineVersion.S1:
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    defcmp = "Kosinski";
                    break;
                case EngineVersion.SCDPC:
                    defcmp = "Uncompressed";
                    break;
                default:
                    defcmp = string.Empty;
                    break;
            }
            ReadOnlyCollection<ReadOnlyCollection<Chunk>> chunkfiles = LevelData.Chunks.GetFiles();
            foreach (string tileent in tilelist)
            {
                fileind++;
                string[] tileentsp = tileent.Split(':');
                tmp = new List<byte>();
                if (LevelData.EngineVersion == EngineVersion.SKC)
                    LevelData.littleendian = false;
                foreach (Chunk c in chunkfiles[fileind])
                    tmp.AddRange(c.GetBytes());
                if (LevelData.EngineVersion == EngineVersion.SKC)
                    LevelData.littleendian = true;
                if (fileind == 0)
                {
                    switch (LevelData.EngineVersion)
                    {
                        case EngineVersion.S1:
                        case EngineVersion.SCD:
                        case EngineVersion.SCDPC:
                            tmp.RemoveRange(0, Chunk.Size);
                            break;
                    }
                }
                Compression.Compress(tmp.ToArray(), tileentsp[0], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("chunk" + LevelData.chunksz + "cmp", defcmp)));
            }
            switch (LevelData.LayoutFmt)
            {
                case EngineVersion.S1:
                    tmp = new List<byte>();
                    tmp.Add((byte)(LevelData.FGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(LevelData.FGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < LevelData.FGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < LevelData.FGLayout.GetLength(0); lc++)
                            tmp.Add((byte)(LevelData.FGLayout[lc, lr] | (LevelData.FGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), gr["fglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("fglayoutcmp", "Uncompressed")));
                    tmp = new List<byte>();
                    tmp.Add((byte)(LevelData.BGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(LevelData.BGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < LevelData.BGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < LevelData.BGLayout.GetLength(0); lc++)
                            tmp.Add((byte)(LevelData.BGLayout[lc, lr] | (LevelData.BGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), gr["bglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("bglayoutcmp", "Uncompressed")));
                    break;
                case EngineVersion.S2:
                    tmp = new List<byte>();
                    for (int la = 0; la < 16; la++)
                    {
                        for (int laf = 0; laf < 128; laf++)
                            tmp.Add(LevelData.FGLayout[laf, la]);
                        for (int lab = 0; lab < 128; lab++)
                            tmp.Add(LevelData.BGLayout[lab, la]);
                    }
                    Compression.Compress(tmp.ToArray(), gr["layout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", "Kosinski")));
                    break;
                case EngineVersion.S3K:
                    tmp = new List<byte>();
                    ushort fgw = (ushort)LevelData.FGLayout.GetLength(0);
                    ushort bgw = (ushort)LevelData.BGLayout.GetLength(0);
                    ushort fgh = (ushort)LevelData.FGLayout.GetLength(1);
                    ushort bgh = (ushort)LevelData.BGLayout.GetLength(1);
                    tmp.AddRange(ByteConverter.GetBytes(fgw));
                    tmp.AddRange(ByteConverter.GetBytes(bgw));
                    tmp.AddRange(ByteConverter.GetBytes(fgh));
                    tmp.AddRange(ByteConverter.GetBytes(bgh));
                    ushort ptr = 0x8088;
                    for (int la = 0; la < 32; la++)
                    {
                        if (la < fgh)
                        {
                            tmp.AddRange(ByteConverter.GetBytes(ptr));
                            ptr += fgw;
                        }
                        else
                            tmp.AddRange(new byte[2]);
                        if (la < bgh)
                        {
                            tmp.AddRange(ByteConverter.GetBytes(ptr));
                            ptr += bgw;
                        }
                        else
                            tmp.AddRange(new byte[2]);
                    }
                    for (int y = 0; y < Math.Max(fgh, bgh); y++)
                    {
                        if (y < fgh)
                            for (int x = 0; x < fgw; x++)
                                tmp.Add(LevelData.FGLayout[x, y]);
                        if (y < bgh)
                            for (int x = 0; x < bgw; x++)
                                tmp.Add(LevelData.BGLayout[x, y]);
                    }
                    Compression.Compress(tmp.ToArray(), gr["layout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", "Uncompressed")));
                    break;
                case EngineVersion.SKC:
                    tmp = new List<byte>();
                    fgw = (ushort)LevelData.FGLayout.GetLength(0);
                    bgw = (ushort)LevelData.BGLayout.GetLength(0);
                    fgh = (ushort)LevelData.FGLayout.GetLength(1);
                    bgh = (ushort)LevelData.BGLayout.GetLength(1);
                    tmp.AddRange(ByteConverter.GetBytes(fgw));
                    tmp.AddRange(ByteConverter.GetBytes(bgw));
                    tmp.AddRange(ByteConverter.GetBytes(fgh));
                    tmp.AddRange(ByteConverter.GetBytes(bgh));
                    ptr = 0x8088;
                    for (int la = 0; la < 32; la++)
                    {
                        if (la < fgh)
                        {
                            tmp.AddRange(ByteConverter.GetBytes(ptr));
                            ptr += fgw;
                        }
                        else
                            tmp.AddRange(new byte[2]);
                        if (la < bgh)
                        {
                            tmp.AddRange(ByteConverter.GetBytes(ptr));
                            ptr += bgw;
                        }
                        else
                            tmp.AddRange(new byte[2]);
                    }
                    List<byte> l = new List<byte>();
                    for (int y = 0; y < Math.Max(fgh, bgh); y++)
                    {
                        if (y < fgh)
                            for (int x = 0; x < fgw; x++)
                                l.Add(LevelData.FGLayout[x, y]);
                        if (y < bgh)
                            for (int x = 0; x < bgw; x++)
                                l.Add(LevelData.BGLayout[x, y]);
                    }
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add(l[i ^ 1]);
                    Compression.Compress(tmp.ToArray(), gr["layout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", "Uncompressed")));
                    break;
                case EngineVersion.SCDPC:
                    tmp = new List<byte>();
                    for (int lr = 0; lr < 8; lr++)
                        for (int lc = 0; lc < 64; lc++)
                            tmp.Add((byte)(LevelData.FGLayout[lc, lr] | (LevelData.FGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), gr["fglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("fglayoutcmp", "Uncompressed")));
                    tmp = new List<byte>();
                    for (int lr = 0; lr < 8; lr++)
                        for (int lc = 0; lc < 64; lc++)
                            tmp.Add((byte)(LevelData.BGLayout[lc, lr] | (LevelData.BGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), gr["bglayout"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("bglayoutcmp", "Uncompressed")));
                    break;
            }
            if (LevelData.EngineVersion != EngineVersion.SCDPC)
            {
                byte[] paltmp;
                List<ushort[]> palfiles = new List<ushort[]>();
                string[] palentstr;
                byte palfilenum = 0;
                if (gr.ContainsKey("palette"))
                {
                    palentstr = gr["palette"].Split('|');
                    for (byte pn = 0; pn < palentstr.Length; pn++)
                    {
                        string[] palent = palentstr[pn].Split(':');
                        paltmp = System.IO.File.ReadAllBytes(palent[0]);
                        ushort[] palfile = new ushort[paltmp.Length / 2];
                        for (int pi = 0; pi < paltmp.Length; pi += 2)
                            palfile[pi / 2] = ByteConverter.ToUInt16(paltmp, pi);
                        palfiles.Add(palfile);
                    }
                    for (int pl = 0; pl < 4; pl++)
                    {
                        for (int pi = 0; pi < 16; pi++)
                        {
                            palfiles[LevelData.PalNum[0][pl, pi]][LevelData.PalAddr[0][pl, pi]] = LevelData.Palette[0][pl, pi];
                        }
                    }
                    for (byte pn = 0; pn < palentstr.Length; pn++)
                    {
                        tmp = new List<byte>();
                        for (int pi = 0; pi < palfiles[pn].Length; pi++)
                            tmp.AddRange(ByteConverter.GetBytes(palfiles[pn][pi]));
                        System.IO.File.WriteAllBytes(palentstr[pn].Split(':')[0], tmp.ToArray());
                    }
                    palfilenum = (byte)palfiles.Count;
                }
                int palnum = 2;
                while (gr.ContainsKey("palette" + palnum))
                {
                    palentstr = gr["palette" + palnum].Split('|');
                    for (byte pn = 1; pn < palentstr.Length; pn++)
                    {
                        string[] palent = palentstr[pn].Split(':');
                        paltmp = System.IO.File.ReadAllBytes(palent[0]);
                        ushort[] palfile = new ushort[paltmp.Length / 2];
                        for (int pi = 0; pi < paltmp.Length; pi += 2)
                            palfile[pi / 2] = ByteConverter.ToUInt16(paltmp, pi);
                        palfiles.Add(palfile);
                    }
                    for (int pl = 0; pl < 4; pl++)
                    {
                        for (int pi = 0; pi < 16; pi++)
                        {
                            palfiles[LevelData.PalNum[palnum - 1][pl, pi]][LevelData.PalAddr[palnum - 1][pl, pi]] = LevelData.Palette[palnum - 1][pl, pi];
                        }
                    }
                    for (byte pn = 1; pn < palentstr.Length; pn++)
                    {
                        tmp = new List<byte>();
                        for (int pi = 0; pi < palfiles[pn - 1 + palfilenum].Length; pi++)
                            tmp.AddRange(ByteConverter.GetBytes(palfiles[pn - 1 + palfilenum][pi]));
                        System.IO.File.WriteAllBytes(palentstr[pn].Split(':')[0], tmp.ToArray());
                    }
                    palnum++;
                    palfilenum = (byte)palfiles.Count;
                }
            }
            if (gr.ContainsKey("objects"))
            {
                LevelData.Objects.Sort();
                tmp = new List<byte>();
                switch (LevelData.ObjectFmt)
                {
                    case EngineVersion.S1:
                        for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                        {
                            tmp.AddRange(((S1ObjectEntry)LevelData.Objects[oi]).GetBytes());
                        }
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        while (tmp.Count % S1ObjectEntry.Size > 0)
                        {
                            tmp.Add(0);
                        }
                        break;
                    case EngineVersion.S2:
                        for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                        {
                            tmp.AddRange(((S2ObjectEntry)LevelData.Objects[oi]).GetBytes());
                        }
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        while (tmp.Count % S2ObjectEntry.Size > 0)
                        {
                            tmp.Add(0);
                        }
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                        {
                            tmp.AddRange(((S3KObjectEntry)LevelData.Objects[oi]).GetBytes());
                        }
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        while (tmp.Count % S3KObjectEntry.Size > 0)
                        {
                            tmp.Add(0);
                        }
                        break;
                    case EngineVersion.SCDPC:
                        for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                        {
                            tmp.AddRange(((SCDObjectEntry)LevelData.Objects[oi]).GetBytes());
                        }
                        tmp.Add(0xFF);
                        while (tmp.Count % SCDObjectEntry.Size > 0)
                        {
                            tmp.Add(0xFF);
                        }
                        break;
                }
                Compression.Compress(tmp.ToArray(), gr["objects"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("objectscmp", "Uncompressed")));
            }
            if (gr.ContainsKey("rings"))
            {
                LevelData.Rings.Sort();
                switch (LevelData.RingFmt)
                {
                    case EngineVersion.S2:
                        tmp = new List<byte>();
                        for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                        {
                            tmp.AddRange(((S2RingEntry)LevelData.Rings[ri]).GetBytes());
                        }
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        Compression.Compress(tmp.ToArray(), gr["rings"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("ringscmp", "Uncompressed")));
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        tmp = new List<byte>();
                        tmp.AddRange(new byte[] { 0, 0, 0, 0 });
                        for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                        {
                            tmp.AddRange(((S3KRingEntry)LevelData.Rings[ri]).GetBytes());
                        }
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        Compression.Compress(tmp.ToArray(), gr["rings"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("ringscmp", "Uncompressed")));
                        break;
                }
            }
            if (LevelData.Bumpers != null)
            {
                LevelData.Bumpers.Sort();
                tmp = new List<byte>();
                foreach (CNZBumperEntry item in LevelData.Bumpers)
                {
                    tmp.AddRange(item.GetBytes());
                }
                tmp.AddRange(new byte[] { 0, 0, 0xFF, 0xFF, 0, 0 });
                Compression.Compress(tmp.ToArray(), gr["bumpers"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("bumperscmp", "Uncompressed")));
            }
            if (gr.ContainsKey("startpos"))
            {
                string[] stposs = gr["startpos"].Split('|');
                int i = 0;
                foreach (string item in stposs)
                {
                    string[] stpos = item.Split(':');
                    System.IO.File.WriteAllBytes(stpos[0], LevelData.StartPositions[i].GetBytes());
                    i++;
                }
            }
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    if (gr.ContainsKey("colind"))
                        Compression.Compress(LevelData.ColInds1.ToArray(), gr["colind"], Compression.CompressionType.Uncompressed);
                    break;
                case EngineVersion.S2:
                    if (gr.ContainsKey("colind1"))
                        Compression.Compress(LevelData.ColInds1.ToArray(),gr["colind1"], Compression.CompressionType.Kosinski);
                    if (gr.ContainsKey("colind2"))
                        Compression.Compress(LevelData.ColInds2.ToArray(), gr["colind2"], Compression.CompressionType.Kosinski);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    if (gr.ContainsKey("colind"))
                    {
                        tmp = new List<byte>();
                        int colindt = int.Parse(gr.GetValueOrDefault("colindsz", "1"));
                        switch (colindt)
                        {
                            case 1:
                                tmp.AddRange(LevelData.ColInds1);
                                tmp.AddRange(LevelData.ColInds2);
                                break;
                            case 2:
                                foreach (byte item in LevelData.ColInds1)
                                    tmp.AddRange(ByteConverter.GetBytes((ushort)item));
                                foreach (byte item in LevelData.ColInds2)
                                    tmp.AddRange(ByteConverter.GetBytes((ushort)item));
                                break;
                        }
                        Compression.Compress(tmp.ToArray(), gr["colind"], Compression.CompressionType.Uncompressed);
                    }
                    break;
            }
            if (ini[string.Empty].ContainsKey("colarr1"))
            {
                tmp = new List<byte>();
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 16; j++)
                        tmp.Add(unchecked((byte)LevelData.ColArr1[i][j]));
                Compression.Compress(tmp.ToArray(), ini[string.Empty]["colarr2"], Compression.CompressionType.Uncompressed);
            }
            if (ini[string.Empty].ContainsKey("colarr2"))
            {
                sbyte[][] rotcol = LevelData.GenerateRotatedCollision();
                tmp = new List<byte>();
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 16; j++)
                        tmp.Add(unchecked((byte)rotcol[i][j]));
                Compression.Compress(tmp.ToArray(), ini[string.Empty]["colarr2"], Compression.CompressionType.Uncompressed);
            }
            if (ini[string.Empty].ContainsKey("angles"))
                Compression.Compress(LevelData.Angles, ini[string.Empty]["angles"], Compression.CompressionType.Uncompressed);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        BitmapBits LevelImg8bpp;
        internal ColorPalette LevelImgPalette;
        internal void DrawLevel()
        {
            if (!loaded) return;
            LevelGfx.Clear(LevelData.PaletteToColor(2, 0, false));
            LevelImg8bpp.Clear();
            Point pnlcur = panel1.PointToClient(Cursor.Position);
            switch (EditingMode)
            {
                case EditingModes.Objects:
                case EditingModes.PlaneA:
                    for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min((camera.Y + (panel1.Height - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                    {
                        for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min((camera.X + (panel1.Width - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                        {
                            if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            if (objectsAboveHighPlaneToolStripMenuItem.Checked)
                            {
                                if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                {
                                    LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    if (path1ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    else if (path2ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                }
                            }
                        }
                    }
                    for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                    {
                        ObjectDefinition od = LevelData.getObjectDefinition(LevelData.Objects[oi].ID);
                        od.Draw(LevelImg8bpp, new Point(LevelData.Objects[oi].X - camera.X, LevelData.Objects[oi].Y - camera.Y), LevelData.Objects[oi].SubType, LevelData.Objects[oi].XFlip, LevelData.Objects[oi].YFlip, true);
                    }
                    for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                    {
                        switch (LevelData.RingFmt)
                        {
                            case EngineVersion.S2:
                                S2RingEntry re = (S2RingEntry)LevelData.Rings[ri];
                                LevelData.S2RingDef.Draw(LevelImg8bpp, new Point(re.X - camera.X, re.Y - camera.Y), re.Direction, re.Count, true);
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                LevelData.S3KRingDef.Draw(LevelImg8bpp, new Point(LevelData.Rings[ri].X - camera.X, LevelData.Rings[ri].Y - camera.Y), true);
                                break;
                        }
                    }
                    if (LevelData.Bumpers != null)
                        foreach (CNZBumperEntry item in LevelData.Bumpers)
                            LevelData.unkobj.Draw(LevelImg8bpp, new Point(item.X - camera.X, item.Y - camera.Y), 0, false, false, true);
                    foreach (StartPositionEntry item in LevelData.StartPositions)
                        LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Draw(LevelImg8bpp, new Point(item.X - camera.X, item.Y - camera.Y), true);
                    if (!objectsAboveHighPlaneToolStripMenuItem.Checked)
                    {
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min((camera.Y + (panel1.Height - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                        {
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min((camera.X + (panel1.Width - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                            {
                                if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                {
                                    LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    if (path1ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    else if (path2ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                }
                            }
                        }
                    }
                    LevelGfx.DrawImage(LevelImg8bpp.ToBitmap(LevelImgPalette), 0, 0, LevelImg8bpp.Width, LevelImg8bpp.Height);
                    foreach (Entry item in SelectedItems)
                    {
                        if (item is ObjectEntry)
                        {
                            ObjectEntry objitem = (ObjectEntry)item;
                            ObjectDefinition selobjd = LevelData.getObjectDefinition(objitem.ID);
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), selobjd.Bounds(new Point(objitem.X - camera.X, objitem.Y - camera.Y), objitem.SubType));
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, selobjd.Bounds(new Point(objitem.X - camera.X, objitem.Y - camera.Y), objitem.SubType));
                        }
                        else if (item is S2RingEntry)
                        {
                            S2RingEntry rngitem = (S2RingEntry)item;
                            Rectangle bnd = LevelData.S2RingDef.Bounds(new Point(rngitem.X - camera.X, rngitem.Y - camera.Y), rngitem.Direction, rngitem.Count);
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Yellow)), bnd);
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
                        }
                        else if (item is S3KRingEntry)
                        {
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), LevelData.S3KRingDef.Bounds(new Point(item.X - camera.X, item.Y - camera.Y)));
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, LevelData.S3KRingDef.Bounds(new Point(item.X - camera.X, item.Y - camera.Y)));
                        }
                        else if (item is CNZBumperEntry)
                        {
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), LevelData.unkobj.Bounds(new Point(item.X - camera.X, item.Y - camera.Y), 0));
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, LevelData.unkobj.Bounds(new Point(item.X - camera.X, item.Y - camera.Y), 0));
                        }
                        else if (item is StartPositionEntry)
                        {
                            StartPositionEntry strtitem = (StartPositionEntry)item;
                            Rectangle bnd = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(strtitem)].Bounds(new Point(strtitem.X - camera.X, strtitem.Y - camera.Y));
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), bnd);
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
                        }
                    }
                    if (LevelData.LayoutFmt == EngineVersion.S1)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min((camera.Y + (panel1.Height - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min((camera.X + (panel1.Width - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                                if (LevelData.FGLoop[x, y])
                                    LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = 3 }, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, LevelData.chunksz, LevelData.chunksz);
                    if (selecting)
                    {
                        Rectangle selbnds = Rectangle.FromLTRB(
                        Math.Min(selpoint.X, lastmouse.X) - camera.X,
                        Math.Min(selpoint.Y, lastmouse.Y) - camera.Y,
                        Math.Max(selpoint.X, lastmouse.X) - camera.X,
                        Math.Max(selpoint.Y, lastmouse.Y) - camera.Y);
                        LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, selbnds);
                    }
                    if (EditingMode == EditingModes.PlaneA)
                    {
                        LevelGfx.DrawImage(LevelData.ChunkBmps[SelectedTile][0],
                        new Rectangle((((pnlcur.X + camera.X) / LevelData.chunksz) * LevelData.chunksz) - camera.X, (((pnlcur.Y + camera.Y) / LevelData.chunksz) * LevelData.chunksz) - camera.Y, LevelData.chunksz, LevelData.chunksz),
                        0, 0, LevelData.chunksz, LevelData.chunksz,
                        GraphicsUnit.Pixel, imageTransparency);
                        LevelGfx.DrawImage(LevelData.ChunkBmps[SelectedTile][1],
                        new Rectangle((((pnlcur.X + camera.X) / LevelData.chunksz) * LevelData.chunksz) - camera.X, (((pnlcur.Y + camera.Y) / LevelData.chunksz) * LevelData.chunksz) - camera.Y, LevelData.chunksz, LevelData.chunksz),
                        0, 0, LevelData.chunksz, LevelData.chunksz,
                        GraphicsUnit.Pixel, imageTransparency);
                    }
                    if (LevelData.LevelBounds.HasValue)
                        LevelGfx.DrawRectangle(Pens.Magenta, LevelData.LevelBounds.Value.X - camera.X, LevelData.LevelBounds.Value.Y - camera.Y, LevelData.LevelBounds.Value.Width + 320, LevelData.LevelBounds.Value.Height + 224);
                    break;
                case EditingModes.PlaneB:
                    for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min((camera.Y + (panel1.Height - 1)) / LevelData.chunksz, LevelData.BGLayout.GetLength(1) - 1); y++)
                    {
                        for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min((camera.X + (panel1.Width - 1)) / LevelData.chunksz, LevelData.BGLayout.GetLength(0) - 1); x++)
                        {
                            if (LevelData.BGLayout[x, y] < LevelData.Chunks.Count)
                                LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            if (path1ToolStripMenuItem.Checked)
                                LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.BGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            else if (path2ToolStripMenuItem.Checked)
                                LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.BGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                        }
                    }
                    LevelGfx.DrawImage(LevelImg8bpp.ToBitmap(LevelImgPalette), 0, 0, LevelImg8bpp.Width, LevelImg8bpp.Height);
                    if (LevelData.LayoutFmt == EngineVersion.S1)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min((camera.Y + (panel1.Height - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min((camera.X + (panel1.Width - 1)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                                if (LevelData.BGLoop[x, y])
                                    LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = 3 }, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, LevelData.chunksz, LevelData.chunksz);
                    LevelGfx.DrawImage(LevelData.ChunkBmps[SelectedTile][0],
                    new Rectangle((((pnlcur.X + camera.X) / LevelData.chunksz) * LevelData.chunksz) - camera.X, (((pnlcur.Y + camera.Y) / LevelData.chunksz) * LevelData.chunksz) - camera.Y, LevelData.chunksz, LevelData.chunksz),
                    0, 0, LevelData.chunksz, LevelData.chunksz,
                    GraphicsUnit.Pixel, imageTransparency);
                    LevelGfx.DrawImage(LevelData.ChunkBmps[SelectedTile][1],
                    new Rectangle((((pnlcur.X + camera.X) / LevelData.chunksz) * LevelData.chunksz) - camera.X, (((pnlcur.Y + camera.Y) / LevelData.chunksz) * LevelData.chunksz) - camera.Y, LevelData.chunksz, LevelData.chunksz),
                    0, 0, LevelData.chunksz, LevelData.chunksz,
                    GraphicsUnit.Pixel, imageTransparency);
                    break;
                default:
                    break;
            }
            PanelGfx.DrawImage(LevelBmp, 0, 0, panel1.Width, panel1.Height);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            DrawLevel();
        }

        Rectangle prevbnds;
        FormWindowState prevstate;
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            long step = e.Shift ? LevelData.chunksz : 16;
            step = e.Control ? int.MaxValue : step;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!loaded) return;
                    vScrollBar1.Value = (int)Math.Max(camera.Y - step, vScrollBar1.Minimum);
                    break;
                case Keys.Down:
                    if (!loaded) return;
                    vScrollBar1.Value = (int)Math.Min(camera.Y + step, vScrollBar1.Maximum);
                    break;
                case Keys.Left:
                    if (!loaded) return;
                    hScrollBar1.Value = (int)Math.Max(camera.X - step, hScrollBar1.Minimum);
                    break;
                case Keys.Right:
                    if (!loaded) return;
                    hScrollBar1.Value = (int)Math.Min(camera.X + step, hScrollBar1.Maximum);
                    break;
                case Keys.Delete:
                    if (!loaded) return;
                    if (EditingMode == EditingModes.Objects & SelectedItems.Count > 0)
                        deleteToolStripMenuItem_Click(sender, EventArgs.Empty);
                    break;
                case Keys.A:
                    if (!loaded) return;
                    switch (EditingMode)
                    {
                        case EditingModes.Objects:
                            foreach (Entry item in SelectedItems)
                            {
                                if (item is S1ObjectEntry)
                                {
                                    S1ObjectEntry oi = item as S1ObjectEntry;
                                    oi.ID = (byte)(oi.ID == 0 ? 0x7F : oi.ID - 1);
                                }
                                else if (item is SCDObjectEntry)
                                {
                                    SCDObjectEntry oi = item as SCDObjectEntry;
                                    oi.ID = (byte)(oi.ID == 0 ? 0x7F : oi.ID - 1);
                                }
                                else if (item is ObjectEntry)
                                {
                                    ObjectEntry oi = item as ObjectEntry;
                                    oi.ID = (byte)(oi.ID == 0 ? 255 : oi.ID - 1);
                                }
                                else if (item is S2RingEntry)
                                {
                                    S2RingEntry ri = item as S2RingEntry;
                                    if (ri.Count == 1)
                                    {
                                        ri.Direction = (ri.Direction == Direction.Vertical ? Direction.Horizontal : Direction.Vertical);
                                        ri.Count = 8;
                                    }
                                    else
                                    {
                                        ri.Count -= 1;
                                    }
                                }
                                else if (item is CNZBumperEntry)
                                {
                                    CNZBumperEntry ci = item as CNZBumperEntry;
                                    ci.ID = (ushort)(ci.ID == 0 ? 65535 : ci.ID - 1);
                                }
                            }
                            break;
                        case EditingModes.PlaneA:
                        case EditingModes.PlaneB:
                            SelectedTile = (byte)(SelectedTile == 0 ? LevelData.Chunks.Count - 1 : SelectedTile - 1);
                            break;
                    }
                    DrawLevel();
                    break;
                case Keys.C:
                    if (!loaded) return;
                    if (e.Control)
                    {
                        if (EditingMode == EditingModes.Objects & SelectedItems.Count > 0)
                            copyToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    break;
                case Keys.L:
                    if (!loaded) return;
                    EditingMode = EditingModes.PlaneA;
                    loaded = false;
                    hScrollBar1.Maximum = (LevelData.FGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.FGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    loaded = true;
                    camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
                    DrawLevel();
                    break;
                case Keys.O:
                    if (e.Control)
                    {
                        openToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    else if (loaded)
                    {
                        EditingMode = EditingModes.Objects;
                        loaded = false;
                        hScrollBar1.Maximum = (LevelData.FGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                        vScrollBar1.Maximum = (LevelData.FGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                        loaded = true;
                        camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
                        DrawLevel();
                    }
                    break;
                case Keys.P:
                    if (!loaded) return;
                    EditingMode = EditingModes.PlaneB;
                    loaded = false;
                    hScrollBar1.Maximum = (LevelData.BGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.BGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    loaded = true;
                    camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
                    DrawLevel();
                    break;
                case Keys.R:
                    if (!loaded | !(e.Control & e.Alt)) return;
                    Random rand = new Random();
                    int w = LevelData.FGLayout.GetLength(0);
                    int h = LevelData.FGLayout.GetLength(1);
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            LevelData.FGLayout[x, y] = (byte)rand.Next(LevelData.Chunks.Count);
                    if (LevelData.FGLoop != null) Array.Clear(LevelData.FGLoop, 0, LevelData.FGLoop.Length);
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            LevelData.BGLayout[x, y] = (byte)rand.Next(LevelData.Chunks.Count);
                    if (LevelData.BGLoop != null) Array.Clear(LevelData.BGLoop, 0, LevelData.BGLoop.Length);
                    w *= LevelData.chunksz;
                    h *= LevelData.chunksz;
                    LevelData.Objects.Clear();
                    List<KeyValuePair<byte, ObjectDefinition>> objdefs = new List<KeyValuePair<byte, ObjectDefinition>>();
                    foreach (KeyValuePair<byte, ObjectDefinition> item in LevelData.ObjTypes)
                        objdefs.Add(item);
                    int o = rand.Next(256);
                    for (int i = 0; i < o; i++)
                    {
                        byte ID = objdefs[rand.Next(objdefs.Count)].Key;
                        byte sub = 0;
                        System.Collections.ObjectModel.ReadOnlyCollection<byte> subs = LevelData.ObjTypes[ID].Subtypes();
                        if (subs.Count > 0)
                            sub = subs[rand.Next(subs.Count)];
                        switch (LevelData.ObjectFmt)
                        {
                            case EngineVersion.S1:
                                S1ObjectEntry ent1 = (S1ObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(ent1);
                                ent1.SubType = sub;
                                ent1.X = (ushort)(rand.Next(w));
                                ent1.Y = (ushort)(rand.Next(h));
                                ent1.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                break;
                            case EngineVersion.S2:
                                S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(ent);
                                ent.SubType = sub;
                                ent.X = (ushort)(rand.Next(w));
                                ent.Y = (ushort)(rand.Next(h));
                                ent.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                S3KObjectEntry ent3 = (S3KObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(ent3);
                                ent3.SubType = sub;
                                ent3.X = (ushort)(rand.Next(w));
                                ent3.Y = (ushort)(rand.Next(h));
                                break;
                            case EngineVersion.SCDPC:
                                SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(entcd);
                                entcd.SubType = sub;
                                entcd.X = (ushort)(rand.Next(w));
                                entcd.Y = (ushort)(rand.Next(h));
                                entcd.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                break;
                        }
                    }
                    LevelData.Objects.Sort();
                    LevelData.Rings.Clear();
                    o = rand.Next(256);
                    for (int i = 0; i < o; i++)
                    {
                        switch (LevelData.RingFmt)
                        {
                            case EngineVersion.S2:
                                S2RingEntry ent = new S2RingEntry();
                                ent.X = (ushort)(rand.Next(w));
                                ent.Y = (ushort)(rand.Next(h));
                                ent.Count = (byte)rand.Next(1, 9);
                                ent.Direction = (Direction)rand.Next(2);
                                LevelData.Rings.Add(ent);
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                S3KRingEntry ent3 = new S3KRingEntry();
                                ent3.X = (ushort)(rand.Next(w));
                                ent3.Y = (ushort)(rand.Next(h));
                                LevelData.Rings.Add(ent3);
                                break;
                        }
                    }
                    LevelData.Rings.Sort();
                    DrawLevel();
                    break;
                case Keys.S:
                    if (!loaded) return;
                    if (e.Control)
                    {
                        saveToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    else
                    {
                        if (EditingMode == EditingModes.Objects)
                        {
                            foreach (Entry item in SelectedItems)
                            {
                                if (item is ObjectEntry)
                                {
                                    ObjectEntry oi = item as ObjectEntry;
                                    oi.SubType = (byte)(oi.SubType == 0 ? 255 : oi.SubType - 1);
                                }
                            }
                            DrawLevel();
                        }
                    }
                    break;
                case Keys.T:
                    objectsAboveHighPlaneToolStripMenuItem.Checked = !objectsAboveHighPlaneToolStripMenuItem.Checked;
                    DrawLevel();
                    break;
                case Keys.V:
                    if (!loaded) return;
                    if (e.Control & EditingMode == EditingModes.Objects)
                    {
                        menuLoc = new Point(panel1.Width / 2, panel1.Height / 2);
                        pasteToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    break;
                case Keys.X:
                    if (!loaded) return;
                    if (e.Control)
                    {
                        if (EditingMode == EditingModes.Objects & SelectedItems.Count > 0)
                            cutToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    else
                    {
                        if (EditingMode == EditingModes.Objects)
                        {
                            foreach (Entry item in SelectedItems)
                            {
                                if (item is ObjectEntry)
                                {
                                    ObjectEntry oi = item as ObjectEntry;
                                    oi.SubType = (byte)(oi.SubType == 255 ? 0 : oi.SubType + 1);
                                }
                            }
                            DrawLevel();
                        }
                    }
                    break;
                case Keys.Y:
                    if (!loaded) return;
                    if (e.Control && RedoList.Count > 0) DoRedo(1);
                    break;
                case Keys.Z:
                    if (!loaded) return;
                    if (e.Control && UndoList.Count > 0)
                        DoUndo(1);
                    else
                    {
                        switch (EditingMode)
                        {
                            case EditingModes.Objects:
                                foreach (Entry item in SelectedItems)
                                {
                                    if (item is S1ObjectEntry)
                                    {
                                        S1ObjectEntry oi = item as S1ObjectEntry;
                                        oi.ID = (byte)(oi.ID == 0x7F ? 0 : oi.ID + 1);
                                    }
                                    else if (item is SCDObjectEntry)
                                    {
                                        SCDObjectEntry oi = item as SCDObjectEntry;
                                        oi.ID = (byte)(oi.ID == 0x7F ? 0 : oi.ID + 1);
                                    }
                                    else if (item is ObjectEntry)
                                    {
                                        ObjectEntry oi = item as ObjectEntry;
                                        oi.ID = (byte)(oi.ID == 255 ? 0 : oi.ID + 1);
                                    }
                                    else if (item is S2RingEntry)
                                    {
                                        S2RingEntry ri = item as S2RingEntry;
                                        if (ri.Count == 8)
                                        {
                                            ri.Direction = (ri.Direction == Direction.Vertical ? Direction.Horizontal : Direction.Vertical);
                                            ri.Count = 1;
                                        }
                                        else
                                        {
                                            ri.Count += 1;
                                        }
                                    }
                                    else if (item is CNZBumperEntry)
                                    {
                                        CNZBumperEntry ci = item as CNZBumperEntry;
                                        ci.ID = (ushort)(ci.ID == 0 ? 65535 : ci.ID - 1);
                                    }
                                }
                                break;
                            case EditingModes.PlaneA:
                            case EditingModes.PlaneB:
                                SelectedTile = (byte)(SelectedTile == LevelData.Chunks.Count - 1 ? 0 : SelectedTile + 1);
                                break;
                        }
                        DrawLevel();
                    }
                    break;
                case Keys.Enter:
                    if (e.Alt)
                    {
                        if (!TopMost)
                        {
                            prevbnds = Bounds;
                            prevstate = WindowState;
                            TopMost = true;
                            WindowState = FormWindowState.Normal;
                            FormBorderStyle = FormBorderStyle.None;
                            Bounds = Screen.FromControl(this).Bounds;
                        }
                        else
                        {
                            TopMost = false;
                            WindowState = prevstate;
                            FormBorderStyle = FormBorderStyle.Sizable;
                            Bounds = prevbnds;
                        }
                    }
                    break;
                case Keys.F5:
                    menuStrip1.ShowHide();
                    break;
                case Keys.Oemplus:
                    using (ResizeLevelDialog dg = new ResizeLevelDialog())
                    {
                        bool canResize;
                        switch (LevelData.LayoutFmt)
                        {
                            case EngineVersion.S1:
                                canResize = true;
                                dg.levelWidth.Minimum = 1;
                                dg.levelWidth.Maximum = int.Parse(ini[string.Empty]["levelwidthmax"], System.Globalization.NumberStyles.Integer);
                                dg.levelHeight.Minimum = 1;
                                dg.levelHeight.Maximum = int.Parse(ini[string.Empty]["levelheightmax"], System.Globalization.NumberStyles.Integer);
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                canResize = true;
                                dg.levelWidth.Minimum = 1;
                                dg.levelWidth.Maximum = 62;
                                dg.levelHeight.Minimum = 1;
                                dg.levelHeight.Maximum = 32;
                                break;
                            default:
                                canResize = false;
                                break;
                        }
                        if (canResize)
                        {
                            switch (EditingMode)
                            {
                                case EditingModes.Objects:
                                case EditingModes.PlaneA:
                                    dg.levelWidth.Value = LevelData.FGLayout.GetLength(0);
                                    dg.levelHeight.Value = LevelData.FGLayout.GetLength(1);
                                    if (dg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                                    {
                                        byte[,] newFG = new byte[(int)dg.levelWidth.Value, (int)dg.levelHeight.Value];
                                        bool[,] newFGLoop = LevelData.LayoutFmt == EngineVersion.S1 ? new bool[(int)dg.levelWidth.Value, (int)dg.levelHeight.Value] : null;
                                        for (int y = 0; y < Math.Min(dg.levelHeight.Value, LevelData.FGLayout.GetLength(1)); y++)
                                        {
                                            for (int x = 0; x < Math.Min(dg.levelWidth.Value, LevelData.FGLayout.GetLength(0)); x++)
                                            {
                                                newFG[x, y] = LevelData.FGLayout[x, y];
                                                if (LevelData.LayoutFmt == EngineVersion.S1)
                                                    newFGLoop[x, y] = LevelData.FGLoop[x, y];
                                            }
                                        }
                                        LevelData.FGLayout = newFG;
                                        LevelData.FGLoop = newFGLoop;
                                        hScrollBar1.Maximum = (LevelData.FGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                                        vScrollBar1.Maximum = (LevelData.FGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                                        DrawLevel();
                                    }
                                    break;
                                case EditingModes.PlaneB:
                                    dg.levelWidth.Value = LevelData.BGLayout.GetLength(0);
                                    dg.levelHeight.Value = LevelData.BGLayout.GetLength(1);
                                    if (dg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                                    {
                                        byte[,] newBG = new byte[(int)dg.levelWidth.Value, (int)dg.levelHeight.Value];
                                        bool[,] newBGLoop = LevelData.LayoutFmt == EngineVersion.S1 ? new bool[(int)dg.levelWidth.Value, (int)dg.levelHeight.Value] : null;
                                        for (int y = 0; y < Math.Min(dg.levelHeight.Value, LevelData.FGLayout.GetLength(1)); y++)
                                        {
                                            for (int x = 0; x < Math.Min(dg.levelWidth.Value, LevelData.FGLayout.GetLength(0)); x++)
                                            {
                                                newBG[x, y] = LevelData.BGLayout[x, y];
                                                if (LevelData.LayoutFmt == EngineVersion.S1)
                                                    newBGLoop[x, y] = LevelData.BGLoop[x, y];
                                            }
                                        }
                                        LevelData.BGLayout = newBG;
                                        LevelData.BGLoop = newBGLoop;
                                        hScrollBar1.Maximum = (LevelData.BGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                                        vScrollBar1.Maximum = (LevelData.BGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                                        DrawLevel();
                                    }
                                    break;
                            }
                        }
                        else
                            MessageBox.Show("The current game does not allow you to resize levels!");
                    }
                    break;
            }
        }

        private void editorToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            EditingMode = (EditingModes)editorToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
            foreach (ToolStripMenuItem item in editorToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            loaded = false;
            hScrollBar1.Minimum = 0;
            vScrollBar1.Minimum = 0;
            switch (EditingMode)
            {
                case EditingModes.Objects:
                case EditingModes.PlaneA:
                    hScrollBar1.Maximum = (LevelData.FGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.FGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    break;
                case EditingModes.PlaneB:
                    hScrollBar1.Maximum = (LevelData.BGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.BGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    break;
            }
            loaded = true;
            camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            DrawLevel();
        }

        bool objdrag = false;
        bool selecting = false;
        Point selpoint;
        List<Point> locs = new List<Point>();
        List<byte> tiles = new List<byte>();
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            Point chunkpoint = new Point((e.X + camera.X) / LevelData.chunksz, (e.Y + camera.Y) / LevelData.chunksz);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (EditingMode)
                    {
                        case EditingModes.Objects:
                            if (e.Clicks == 2)
                            {
                                if (ModifierKeys != Keys.Shift)
                                {
                                    if (ModifierKeys != Keys.Control || LevelData.Bumpers == null)
                                    {
                                        if (ObjectSelect.ShowDialog(this) == DialogResult.OK)
                                        {
                                            byte ID = (byte)ObjectSelect.numericUpDown1.Value;
                                            byte sub = (byte)ObjectSelect.numericUpDown2.Value;
                                            switch (LevelData.ObjectFmt)
                                            {
                                                case EngineVersion.S2:
                                                    S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                                                    LevelData.Objects.Add(ent);
                                                    ent.SubType = sub;
                                                    ent.X = (ushort)(e.X + camera.X);
                                                    ent.Y = (ushort)(e.Y + camera.Y);
                                                    ent.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                                    SelectedItems.Clear();
                                                    SelectedItems.Add(ent);
                                                    SelectedObjectChanged();
                                                    AddUndo(new ObjectAddedUndoAction(ent));
                                                    break;
                                                case EngineVersion.S1:
                                                    S1ObjectEntry ent1 = (S1ObjectEntry)LevelData.CreateObject(ID);
                                                    LevelData.Objects.Add(ent1);
                                                    ent1.SubType = sub;
                                                    ent1.X = (ushort)(e.X + camera.X);
                                                    ent1.Y = (ushort)(e.Y + camera.Y);
                                                    ent1.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                                    SelectedItems.Clear();
                                                    SelectedItems.Add(ent1);
                                                    SelectedObjectChanged();
                                                    AddUndo(new ObjectAddedUndoAction(ent1));
                                                    break;
                                                case EngineVersion.S3K:
                                                case EngineVersion.SKC:
                                                    S3KObjectEntry ent3 = (S3KObjectEntry)LevelData.CreateObject(ID);
                                                    LevelData.Objects.Add(ent3);
                                                    ent3.SubType = sub;
                                                    ent3.X = (ushort)(e.X + camera.X);
                                                    ent3.Y = (ushort)(e.Y + camera.Y);
                                                    SelectedItems.Clear();
                                                    SelectedItems.Add(ent3);
                                                    SelectedObjectChanged();
                                                    AddUndo(new ObjectAddedUndoAction(ent3));
                                                    break;
                                                case EngineVersion.SCDPC:
                                                    SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                                                    LevelData.Objects.Add(entcd);
                                                    entcd.SubType = sub;
                                                    entcd.X = (ushort)(e.X + camera.X);
                                                    entcd.Y = (ushort)(e.Y + camera.Y);
                                                    entcd.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                                    SelectedItems.Clear();
                                                    SelectedItems.Add(entcd);
                                                    SelectedObjectChanged();
                                                    AddUndo(new ObjectAddedUndoAction(entcd));
                                                    break;
                                            }
                                            LevelData.Objects.Sort();
                                            DrawLevel();
                                        }
                                    }
                                    else
                                    {
                                        LevelData.Bumpers.Add(new CNZBumperEntry() { X = (ushort)(e.X + camera.X), Y = (ushort)(e.Y + camera.Y) });
                                        SelectedItems.Clear();
                                        SelectedItems.Add(LevelData.Bumpers[LevelData.Bumpers.Count - 1]);
                                        SelectedObjectChanged();
                                        AddUndo(new ObjectAddedUndoAction(LevelData.Bumpers[LevelData.Bumpers.Count - 1]));
                                        LevelData.Bumpers.Sort();
                                        DrawLevel();
                                    }
                                }
                                else if (LevelData.RingFmt == EngineVersion.S2)
                                {
                                    LevelData.Rings.Add(new S2RingEntry() { X = (ushort)(e.X + camera.X), Y = (ushort)(e.Y + camera.Y) });
                                    SelectedItems.Clear();
                                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                                    SelectedObjectChanged();
                                    AddUndo(new ObjectAddedUndoAction(LevelData.Rings[LevelData.Rings.Count - 1]));
                                    LevelData.Rings.Sort();
                                    DrawLevel();
                                }
                                else if (LevelData.RingFmt == EngineVersion.S3K | LevelData.RingFmt == EngineVersion.SKC)
                                {
                                    LevelData.Rings.Add(new S3KRingEntry() { X = (ushort)(e.X + camera.X), Y = (ushort)(e.Y + camera.Y) });
                                    SelectedItems.Clear();
                                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                                    SelectedObjectChanged();
                                    AddUndo(new ObjectAddedUndoAction(LevelData.Rings[LevelData.Rings.Count - 1]));
                                    LevelData.Rings.Sort();
                                    DrawLevel();
                                }
                            }
                            foreach (ObjectEntry item in LevelData.Objects)
                            {
                                ObjectDefinition dat = LevelData.getObjectDefinition(item.ID);
                                Rectangle bound = dat.Bounds(new Point(item.X, item.Y), item.SubType);
                                if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                {
                                    if (ModifierKeys == Keys.Control)
                                    {
                                        if (SelectedItems.Contains(item))
                                            SelectedItems.Remove(item);
                                        else
                                            SelectedItems.Add(item);
                                    }
                                    else if (!SelectedItems.Contains(item))
                                    {
                                        SelectedItems.Clear();
                                        SelectedItems.Add(item);
                                    }
                                    SelectedObjectChanged();
                                    objdrag = true;
                                    DrawLevel();
                                    break;
                                }
                            }
                            if (!objdrag)
                                foreach (RingEntry ritem in LevelData.Rings)
                                {
                                    if (ritem is S2RingEntry)
                                    {
                                        S2RingEntry item = ritem as S2RingEntry;
                                        Rectangle bound = LevelData.S2RingDef.Bounds(new Point(item.X, item.Y), item.Direction, item.Count);
                                        if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                        {
                                            if (ModifierKeys == Keys.Control)
                                            {
                                                if (SelectedItems.Contains(item))
                                                    SelectedItems.Remove(item);
                                                else
                                                    SelectedItems.Add(item);
                                            }
                                            else if (!SelectedItems.Contains(item))
                                            {
                                                SelectedItems.Clear();
                                                SelectedItems.Add(item);
                                            }
                                            SelectedObjectChanged();
                                            objdrag = true;
                                            DrawLevel();
                                            break;
                                        }
                                    }
                                    else if (ritem is S3KRingEntry)
                                    {
                                        S3KRingEntry item = ritem as S3KRingEntry;
                                        Rectangle bound = LevelData.S3KRingDef.Bounds(new Point(item.X, item.Y));
                                        if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                        {
                                            if (ModifierKeys == Keys.Control)
                                            {
                                                if (SelectedItems.Contains(item))
                                                    SelectedItems.Remove(item);
                                                else
                                                    SelectedItems.Add(item);
                                            }
                                            else if (!SelectedItems.Contains(item))
                                            {
                                                SelectedItems.Clear();
                                                SelectedItems.Add(item);
                                            }
                                            SelectedObjectChanged();
                                            objdrag = true;
                                            DrawLevel();
                                            break;
                                        }
                                    }
                                }
                            if (!objdrag && LevelData.Bumpers != null)
                                foreach (CNZBumperEntry item in LevelData.Bumpers)
                                {
                                    Rectangle bound = LevelData.unkobj.Bounds(new Point(item.X, item.Y), 0);
                                    if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                    {
                                        if (ModifierKeys == Keys.Control)
                                        {
                                            if (SelectedItems.Contains(item))
                                                SelectedItems.Remove(item);
                                            else
                                                SelectedItems.Add(item);
                                        }
                                        if (!SelectedItems.Contains(item))
                                        {
                                            SelectedItems.Clear();
                                            SelectedItems.Add(item);
                                        }
                                        SelectedObjectChanged();
                                        objdrag = true;
                                        DrawLevel();
                                        break;
                                    }
                                }
                            if (!objdrag)
                                foreach (StartPositionEntry item in LevelData.StartPositions)
                                {
                                    Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(new Point(item.X, item.Y));
                                    if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                    {
                                        if (ModifierKeys == Keys.Control)
                                        {
                                            if (SelectedItems.Contains(item))
                                                SelectedItems.Remove(item);
                                            else
                                                SelectedItems.Add(item);
                                        }
                                        if (!SelectedItems.Contains(item))
                                        {
                                            SelectedItems.Clear();
                                            SelectedItems.Add(item);
                                        }
                                        SelectedObjectChanged();
                                        objdrag = true;
                                        DrawLevel();
                                        break;
                                    }
                                }
                            if (!objdrag)
                            {
                                selecting = true;
                                selpoint = new Point(e.X + camera.X, e.Y + camera.Y);
                                SelectedItems.Clear();
                                SelectedObjectChanged();
                            }
                            else
                            {
                                locs = new List<Point>();
                                foreach (Entry item in SelectedItems)
                                {
                                    locs.Add(new Point(item.X, item.Y));
                                }
                            }
                            break;
                        case EditingModes.PlaneA:
                            if ((LevelData.LayoutFmt == EngineVersion.S1 | LevelData.LayoutFmt == EngineVersion.SCDPC) && e.Clicks >= 2)
                            {
                                LevelData.FGLoop[chunkpoint.X, chunkpoint.Y] = !LevelData.FGLoop[chunkpoint.X, chunkpoint.Y];
                            }
                            else
                                {
                                    locs = new List<Point>();
                                    tiles = new List<byte>();
                                    byte t = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                                    if (t != SelectedTile)
                                    {
                                        locs.Add(chunkpoint);
                                        tiles.Add(t);
                                        LevelData.FGLayout[chunkpoint.X, chunkpoint.Y] = SelectedTile;
                                        DrawLevel();
                                    }
                                }
                            break;
                        case EditingModes.PlaneB:
                                if ((LevelData.LayoutFmt == EngineVersion.S1 | LevelData.LayoutFmt == EngineVersion.SCDPC) && e.Clicks >= 2)
                                {
                                    LevelData.BGLoop[chunkpoint.X, chunkpoint.Y] = !LevelData.BGLoop[chunkpoint.X, chunkpoint.Y];
                                }
                                else
                                {
                                    locs = new List<Point>();
                                    tiles = new List<byte>();
                                    byte tb = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                                    if (tb != SelectedTile)
                                    {
                                        locs.Add(chunkpoint);
                                        tiles.Add(tb);
                                        LevelData.BGLayout[chunkpoint.X, chunkpoint.Y] = SelectedTile;
                                        DrawLevel();
                                    }
                                }
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    switch (EditingMode)
                    {
                        case EditingModes.Objects:
                            menuLoc = e.Location;
                            foreach (ObjectEntry item in LevelData.Objects)
                            {
                                ObjectDefinition dat = LevelData.getObjectDefinition(item.ID);
                                Rectangle bound = dat.Bounds(new Point(item.X , item.Y), item.SubType);
                                if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                {
                                    if (!SelectedItems.Contains(item))
                                    {
                                        SelectedItems.Clear();
                                        SelectedItems.Add(item);
                                    }
                                    SelectedObjectChanged();
                                    objdrag = true;
                                    DrawLevel();
                                    break;
                                }
                            }
                            if (!objdrag)
                                foreach (RingEntry ritem in LevelData.Rings)
                                {
                                    if (ritem is S2RingEntry)
                                    {
                                        S2RingEntry item = ritem as S2RingEntry;
                                        Rectangle bound = LevelData.S2RingDef.Bounds(new Point(item.X, item.Y), item.Direction, item.Count);
                                        if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                        {
                                            if (!SelectedItems.Contains(item))
                                            {
                                                SelectedItems.Clear();
                                                SelectedItems.Add(item);
                                            }
                                            SelectedObjectChanged();
                                            objdrag = true;
                                            DrawLevel();
                                            break;
                                        }
                                    }
                                    else if (ritem is S3KRingEntry)
                                    {
                                        S3KRingEntry item = ritem as S3KRingEntry;
                                        Rectangle bound = LevelData.S3KRingDef.Bounds(new Point(item.X, item.Y));
                                        if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                        {
                                            if (!SelectedItems.Contains(item))
                                            {
                                                SelectedItems.Clear();
                                                SelectedItems.Add(item);
                                            }
                                            SelectedObjectChanged();
                                            objdrag = true;
                                            DrawLevel();
                                            break;
                                        }
                                    }
                                }
                            if (!objdrag && LevelData.Bumpers != null)
                                foreach (CNZBumperEntry item in LevelData.Bumpers)
                                {
                                    Rectangle bound = LevelData.unkobj.Bounds(new Point(item.X, item.Y), 0);
                                        if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                        {
                                            if (!SelectedItems.Contains(item))
                                            {
                                                SelectedItems.Clear();
                                                SelectedItems.Add(item);
                                            }
                                            SelectedObjectChanged();
                                            objdrag = true;
                                            DrawLevel();
                                            break;
                                        }
                                }
                            if (!objdrag)
                                foreach (StartPositionEntry item in LevelData.StartPositions)
                                {
                                    Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(new Point(item.X, item.Y));
                                    if (bound.Contains(e.X + camera.X, e.Y + camera.Y))
                                    {
                                        if (!SelectedItems.Contains(item))
                                        {
                                            SelectedItems.Clear();
                                            SelectedItems.Add(item);
                                        }
                                        SelectedObjectChanged();
                                        objdrag = true;
                                        DrawLevel();
                                        break;
                                    }
                                }
                            objdrag = false;
                            if (SelectedItems.Count > 0)
                            {
                                cutToolStripMenuItem.Enabled = true;
                                copyToolStripMenuItem.Enabled = true;
                                deleteToolStripMenuItem.Enabled = true;
                            }
                            else
                            {
                                cutToolStripMenuItem.Enabled = false;
                                copyToolStripMenuItem.Enabled = false;
                                deleteToolStripMenuItem.Enabled = false;
                            }
                            pasteToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLObjectList");
                            contextMenuStrip1.Show(panel1, menuLoc);
                            break;
                        case EditingModes.PlaneA:
                            SelectedTile = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                            EditControls.ChunkSelector.SelectedIndex = SelectedTile;
                            DrawLevel();
                            break;
                        case EditingModes.PlaneB:
                            SelectedTile = LevelData.BGLayout[chunkpoint.X, chunkpoint.Y];
                            EditControls.ChunkSelector.SelectedIndex = SelectedTile;
                            DrawLevel();
                            break;
                    }
                    break;
            }
        }

        Point lastchunkpoint;
        Point lastmouse;
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            if (!panel1.Bounds.Contains(panel1.PointToClient(Cursor.Position))) return;
            Point mouse = new Point(e.X + camera.X, e.Y + camera.Y);
            Point chunkpoint = new Point(mouse.X / LevelData.chunksz, mouse.Y / LevelData.chunksz);
            bool redraw = false;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (EditingMode)
                    {
                        case EditingModes.Objects:
                            if (objdrag)
                            {
                                foreach (Entry item in SelectedItems)
                                {
                                    item.X = (ushort)(item.X + (mouse.X - lastmouse.X));
                                    item.Y = (ushort)(item.Y + (mouse.Y - lastmouse.Y));
                                }
                                redraw = true;
                            }
                            else if (selecting)
                            {
                                int selobjs = SelectedItems.Count;
                                SelectedItems.Clear();
                                Rectangle selbnds = Rectangle.FromLTRB(
                                Math.Min(selpoint.X, mouse.X),
                                Math.Min(selpoint.Y, mouse.Y),
                                Math.Max(selpoint.X, mouse.X),
                                Math.Max(selpoint.Y, mouse.Y));
                                foreach (ObjectEntry item in LevelData.Objects)
                                {
                                    ObjectDefinition dat = LevelData.getObjectDefinition(item.ID);
                                    Rectangle bound = dat.Bounds(new Point(item.X, item.Y), item.SubType);
                                    if (bound.IntersectsWith(selbnds))
                                        SelectedItems.Add(item);
                                }
                                foreach (RingEntry ritem in LevelData.Rings)
                                {
                                    if (ritem is S2RingEntry)
                                    {
                                        S2RingEntry item = ritem as S2RingEntry;
                                        Rectangle bound = LevelData.S2RingDef.Bounds(new Point(item.X, item.Y), item.Direction, item.Count);
                                        if (bound.IntersectsWith(selbnds))
                                            SelectedItems.Add(item);
                                    }
                                    else if (ritem is S3KRingEntry)
                                    {
                                        S3KRingEntry item = ritem as S3KRingEntry;
                                        Rectangle bound = LevelData.S3KRingDef.Bounds(new Point(item.X, item.Y));
                                        if (bound.IntersectsWith(selbnds))
                                            SelectedItems.Add(item);
                                    }
                                }
                                if (LevelData.Bumpers != null)
                                    foreach (CNZBumperEntry item in LevelData.Bumpers)
                                    {
                                        Rectangle bound = LevelData.unkobj.Bounds(new Point(item.X, item.Y), 0);
                                        if (bound.IntersectsWith(selbnds))
                                            SelectedItems.Add(item);
                                    }
                                foreach (StartPositionEntry item in LevelData.StartPositions)
                                {
                                    Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(new Point(item.X, item.Y));
                                    if (bound.IntersectsWith(selbnds))
                                        SelectedItems.Add(item);
                                }
                                if (selobjs != SelectedItems.Count) SelectedObjectChanged();
                                redraw = true;
                            }
                            break;
                        case EditingModes.PlaneA:
                            byte t = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                            if (t != SelectedTile)
                            {
                                locs.Add(chunkpoint);
                                tiles.Add(t);
                                LevelData.FGLayout[chunkpoint.X, chunkpoint.Y] = SelectedTile;
                            }
                            break;
                        case EditingModes.PlaneB:
                            byte tb = LevelData.BGLayout[chunkpoint.X, chunkpoint.Y];
                            if (tb != SelectedTile)
                            {
                                locs.Add(chunkpoint);
                                tiles.Add(tb);
                                LevelData.BGLayout[chunkpoint.X, chunkpoint.Y] = SelectedTile;
                            }
                            break;
                    }
                    break;
            }
            if (EditingMode == EditingModes.Objects)
            {
                Cursor cur = Cursors.Default;
                foreach (ObjectEntry item in LevelData.Objects)
                {
                    ObjectDefinition dat = LevelData.getObjectDefinition(item.ID);
                    Rectangle bound = dat.Bounds(new Point(item.X, item.Y), item.SubType);
                    if (bound.Contains(mouse))
                    {
                        cur = Cursors.SizeAll;
                        break;
                    }
                }
                foreach (RingEntry item in LevelData.Rings)
                {
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S2:
                            S2RingEntry rngitem = (S2RingEntry)item;
                            Rectangle bound = LevelData.S2RingDef.Bounds(new Point(rngitem.X, rngitem.Y), rngitem.Direction, rngitem.Count);
                            if (bound.Contains(mouse))
                            {
                                cur = Cursors.SizeAll;
                                break;
                            }
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            Rectangle bound3 = LevelData.S3KRingDef.Bounds(new Point(item.X, item.Y));
                            if (bound3.Contains(mouse))
                            {
                                cur = Cursors.SizeAll;
                                break;
                            }
                            break;
                    }
                }
                if (LevelData.Bumpers != null)
                    foreach (CNZBumperEntry item in LevelData.Bumpers)
                    {
                        Rectangle bound = LevelData.unkobj.Bounds(new Point(item.X, item.Y), 0);
                        if (bound.Contains(mouse))
                        {
                            cur = Cursors.SizeAll;
                            break;
                        }
                    }
                foreach (StartPositionEntry item in LevelData.StartPositions)
                {
                    Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(new Point(item.X, item.Y));
                    if (bound.Contains(mouse))
                    {
                        cur = Cursors.SizeAll;
                        break;
                    }
                }
                if (LevelData.LevelBounds.HasValue)
                {
                    Rectangle r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Left + 2, LevelData.LevelBounds.Value.Top - 2, LevelData.LevelBounds.Value.Right + 320 - 2, LevelData.LevelBounds.Value.Top + 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeNS;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Left - 2, LevelData.LevelBounds.Value.Top + 2, LevelData.LevelBounds.Value.Left + 2, LevelData.LevelBounds.Value.Bottom + 224 - 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeWE;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Left + 2, LevelData.LevelBounds.Value.Bottom + 224 - 2, LevelData.LevelBounds.Value.Right + 320 - 2, LevelData.LevelBounds.Value.Bottom + 224 + 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeNS;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Right + 320 - 2, LevelData.LevelBounds.Value.Top + 2, LevelData.LevelBounds.Value.Right + 320 + 2, LevelData.LevelBounds.Value.Bottom + 224 - 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeWE;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Left - 2, LevelData.LevelBounds.Value.Top - 2, LevelData.LevelBounds.Value.Left + 2, LevelData.LevelBounds.Value.Top + 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeNWSE;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Right + 320 - 2, LevelData.LevelBounds.Value.Top - 2, LevelData.LevelBounds.Value.Right + 320 + 2, LevelData.LevelBounds.Value.Top + 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeNESW;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Left - 2, LevelData.LevelBounds.Value.Bottom + 224 - 2, LevelData.LevelBounds.Value.Left + 2, LevelData.LevelBounds.Value.Bottom + 224 + 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeNESW;
                    r = Rectangle.FromLTRB(LevelData.LevelBounds.Value.Right + 320 - 2, LevelData.LevelBounds.Value.Bottom + 224 - 2, LevelData.LevelBounds.Value.Right + 320 + 2, LevelData.LevelBounds.Value.Bottom + 224 + 2);
                    if (r.Contains(mouse) & cur == Cursors.Default)
                        cur = Cursors.SizeNWSE;
                }
                Cursor = cur;
            }
            if ((EditingMode == EditingModes.PlaneA | EditingMode == EditingModes.PlaneB) && chunkpoint != lastchunkpoint) DrawLevel();
            if (redraw) DrawLevel();
            lastchunkpoint = chunkpoint;
            lastmouse = mouse;
        }

        private void EditControls_listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            if (EditControls.ChunkSelector.SelectedIndex == -1) return;
            SelectedTile = (byte)EditControls.ChunkSelector.SelectedIndex;
            if (EditingMode == EditingModes.PlaneA | EditingMode == EditingModes.PlaneB) DrawLevel();
        }

        private void blocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory };
            if (a.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < LevelData.Blocks.Count; i++)
                {
                    BitmapBits bmp = new BitmapBits(16, 16);
                    bmp.DrawBitmapComposited(LevelData.BlockBmpBits[i][0], new Point(0, 0));
                    bmp.DrawBitmapComposited(LevelData.BlockBmpBits[i][1], new Point(0, 0));
                    bmp.ToBitmap(LevelData.BmpPal).Save(System.IO.Path.Combine(a.SelectedPath, i + ".png"));
                }
            }
        }

        private void chunksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory };
            if (a.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < LevelData.Chunks.Count; i++)
                {
                    BitmapBits bmp = new BitmapBits(LevelData.chunksz, LevelData.chunksz);
                    bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[i][0], new Point(0, 0));
                    bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[i][1], new Point(0, 0));
                    bmp.ToBitmap(LevelData.BmpPal).Save(System.IO.Path.Combine(a.SelectedPath, i + ".png"));
                }
            }
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog()
            {
                DefaultExt = "png",
                Filter = "PNG Files|*.png",
                RestoreDirectory = true
            };
            if (a.ShowDialog() == DialogResult.OK)
            {
                int xend = 0;
                int yend = 0;
                for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                {
                    for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                    {
                        if (LevelData.FGLayout[x, y] > 0)
                        {
                            xend = Math.Max(xend, x);
                            yend = Math.Max(yend, y);
                        }
                    }
                }
                xend++;
                yend++;
                BitmapBits bmp = new BitmapBits(xend * LevelData.chunksz, yend * LevelData.chunksz);
                for (int y = 0; y < yend; y++)
                {
                    for (int x = 0; x < xend; x++)
                    {
                        bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                        if (objectsAboveHighPlaneToolStripMenuItem.Checked)
                            bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                    }
                }
                if (includeobjectsWithFGToolStripMenuItem.Checked)
                {
                    for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                    {
                        ObjectDefinition od = LevelData.getObjectDefinition(LevelData.Objects[oi].ID);
                        od.Draw(bmp, new Point(LevelData.Objects[oi].X, LevelData.Objects[oi].Y), LevelData.Objects[oi].SubType, LevelData.Objects[oi].XFlip, LevelData.Objects[oi].YFlip, !hideDebugObjectsToolStripMenuItem.Checked);
                    }
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S2:
                            for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                            {
                                S2RingEntry re = (S2RingEntry)LevelData.Rings[ri];
                                LevelData.S2RingDef.Draw(bmp, new Point(re.X, re.Y), re.Direction, re.Count, !hideDebugObjectsToolStripMenuItem.Checked);
                            }
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                            {
                                LevelData.S3KRingDef.Draw(bmp, new Point(LevelData.Rings[ri].X, LevelData.Rings[ri].Y), !hideDebugObjectsToolStripMenuItem.Checked);
                            }
                            break;
                    }
                    for (int si = 0; si < LevelData.StartPositions.Count; si++)
                        LevelData.StartPosDefs[si].Draw(bmp, new Point(LevelData.StartPositions[si].X, LevelData.StartPositions[si].Y), !hideDebugObjectsToolStripMenuItem.Checked);
                }
                if (!objectsAboveHighPlaneToolStripMenuItem.Checked)
                    for (int y = 0; y < yend; y++)
                    {
                        for (int x = 0; x < xend; x++)
                        {
                            bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                        }
                    }
                for (int i = 0; i < bmp.Bits.Length; i++)
                    if (bmp.Bits[i] == 0)
                        bmp.Bits[i] = 32;
                Bitmap res = bmp.ToBitmap();
                ColorPalette pal = res.Palette;
                for (int i = 0; i < 64; i++)
                    pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackFGBGToolStripMenuItem.Checked);
                for (int i = 64; i < 256; i++)
                    pal.Entries[i] = Color.Black;
                res.Palette = pal;
                res.Save(a.FileName);
            }
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog()
            {
                DefaultExt = "png",
                Filter = "PNG Files|*.png",
                RestoreDirectory = true
            };
            if (a.ShowDialog() == DialogResult.OK)
            {
                int xend = 0;
                int yend = 0;
                for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                {
                    for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                    {
                        if (LevelData.BGLayout[x, y] > 0)
                        {
                            xend = Math.Max(xend, x);
                            yend = Math.Max(yend, y);
                        }
                    }
                }
                xend++;
                yend++;
                BitmapBits bmp = new BitmapBits(xend * LevelData.chunksz, yend * LevelData.chunksz);
                for (int y = 0; y < yend; y++)
                {
                    for (int x = 0; x < xend; x++)
                    {
                        bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][0], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                        bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][1], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                    }
                }
                for (int i = 0; i < bmp.Bits.Length; i++)
                    if (bmp.Bits[i] == 0)
                        bmp.Bits[i] = 32;
                Bitmap res = bmp.ToBitmap();
                ColorPalette pal = res.Palette;
                for (int i = 0; i < 64; i++)
                    pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackFGBGToolStripMenuItem.Checked);
                for (int i = 64; i < 256; i++)
                    pal.Entries[i] = Color.Black;
                res.Palette = pal;
                res.Save(a.FileName);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((EditingMode == EditingModes.PlaneA | EditingMode == EditingModes.PlaneB) && locs.Count > 0) AddUndo(new LayoutEditUndoAction(EditingMode, locs, tiles));
            if (objdrag)
            {
                AddUndo(new ObjectMoveUndoAction(new List<Entry>(SelectedItems), locs));
                if (ModifierKeys == Keys.Shift)
                {
                    foreach (Entry item in SelectedItems)
                    {
                        item.X &= 0xFFF8;
                        item.Y &= 0xFFF8;
                    }
                }
                EditControls.propertyGrid1.SelectedObjects = SelectedItems.ToArray();
                LevelData.Objects.Sort();
                LevelData.Rings.Sort();
                if (LevelData.Bumpers != null) LevelData.Bumpers.Sort();
            }
            objdrag = false;
            selecting = false;
            DrawLevel();
        }

        private void SelectedObjectChanged()
        {
            if (SelectedItems.Count > 0)
            {
                if (SelectedItems.Count == 1)
                {
                    if (SelectedItems[0] is ObjectEntry)
                    {
                        ObjectEntry objitem = (ObjectEntry)SelectedItems[0];
                        EditControls.ObjName.Text = LevelData.getFullObjectName(objitem.ID, objitem.SubType);
                        EditControls.objPicture.Image = LevelData.getObjectDefinition(objitem.ID).Image(objitem.SubType).ToBitmap(LevelData.BmpPal);
                    }
                    else if (SelectedItems[0] is S2RingEntry)
                    {
                        EditControls.ObjName.Text = LevelData.S2RingDef.Name();
                        EditControls.objPicture.Image = LevelData.S2RingDef.Image().ToBitmap(LevelData.BmpPal);
                    }
                    else if (SelectedItems[0] is S3KRingEntry)
                    {
                        EditControls.ObjName.Text = "Ring";
                        EditControls.objPicture.Image = LevelData.S3KRingDef.Image().ToBitmap(LevelData.BmpPal);
                    }
                    else if (SelectedItems[0] is CNZBumperEntry)
                    {
                        EditControls.ObjName.Text = "Bumper";
                        EditControls.objPicture.Image = LevelData.UnknownImg;
                    }
                    else if (SelectedItems[0] is StartPositionEntry)
                    {
                        StartPositionDefinition def = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf((StartPositionEntry)SelectedItems[0])];
                        EditControls.ObjName.Text = def.Name();
                        EditControls.objPicture.Image = def.Image().ToBitmap(LevelData.BmpPal);
                    }
                }
                else
                {
                    EditControls.ObjName.Text = "Multiple objects";
                    EditControls.objPicture.Image = null;
                }
            }
            else
            {
                EditControls.ObjName.Text = "No objects selected";
                EditControls.objPicture.Image = null;
            }
            EditControls.propertyGrid1.SelectedObjects = SelectedItems.ToArray();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loaded)
            {
                switch (MessageBox.Show(this, "Do you want to save?", LevelData.EngineVersion.ToString() + "LVL", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void LoadObjectDefinitions(string file)
        {
            Log("Loading object definition file \"" + file + "\".");
            Dictionary<string, Dictionary<string, string>> ini = IniFile.Load(file);
            foreach (KeyValuePair<string, Dictionary<string, string>> group in ini)
            {
                byte ID;
                if (group.Key == "Ring")
                {
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S2:
                            string ty = group.Value["codetype"];
                            string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                            DateTime modDate = DateTime.MinValue;
                            if (System.IO.File.Exists(dllfile))
                                modDate = System.IO.File.GetLastWriteTime(dllfile);
                            string fp = group.Value["codefile"].Replace('/', System.IO.Path.DirectorySeparatorChar);
                            Log("Loading S2RingDefinition type " + ty + " from \"" + fp + "\"...");
                            if (modDate >= System.IO.File.GetLastWriteTime(fp))
                            {
                                Log("Loading type from cached assembly \"" + dllfile + "\"...");
                                LevelData.S2RingDef = (S2RingDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                            }
                            else
                            {
                                Log("Compiling code file...");
                                string ext = System.IO.Path.GetExtension(fp);
                                CodeDomProvider pr = null;
                                switch (ext.ToLowerInvariant())
                                {
                                    case ".cs":
                                        pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                        break;
                                    case ".vb":
                                        pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                        break;
                                }
                                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                                para.GenerateExecutable = false;
                                para.GenerateInMemory = false;
                                para.IncludeDebugInformation = true;
                                para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
                                CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
                                if (res.Errors.HasErrors)
                                {
                                    Log("Compile failed.", "Errors:");
                                    foreach (CompilerError item in res.Errors)
                                        Log(item.ToString());
                                    Log(string.Empty);
                                    LevelData.S2RingDef = new DefS2RingDef();
                                }
                                else
                                {
                                    Log("Compile succeeded.");
                                    LevelData.S2RingDef = (S2RingDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
                                }
                            }
                            LevelData.S2RingDef.Init(group.Value);
                            Log("S2 Ring Definition loaded.");
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            Log("Loading S3K Ring Definition...");
                            LevelData.S3KRingDef = new S3KRingDefinition(group.Value);
                            break;
                    }
                }
                else if (byte.TryParse(group.Key, System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo, out ID))
                {
                    if (LevelData.ObjTypes.ContainsKey(ID))
                        LevelData.ObjTypes.Remove(ID);
                    ObjectDefinition def = null;
                    if (group.Value.ContainsKey("codefile"))
                    {
                        string ty = group.Value["codetype"];
                        string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                        DateTime modDate = DateTime.MinValue;
                        if (System.IO.File.Exists(dllfile))
                            modDate = System.IO.File.GetLastWriteTime(dllfile);
                        string fp = group.Value["codefile"].Replace('/', System.IO.Path.DirectorySeparatorChar);
                        Log("Loading ObjectDefinition type " + ty + " from \"" + fp + "\"...");
                        if (modDate >= System.IO.File.GetLastWriteTime(fp))
                        {
                            Log("Loading type from cached assembly \"" + dllfile + "\"...");
                            def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                        }
                        else
                        {
                            Log("Compiling code file...");
                            string ext = System.IO.Path.GetExtension(fp);
                            CodeDomProvider pr = null;
                            switch (ext.ToLowerInvariant())
                            {
                                case ".cs":
                                    pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
                                case ".vb":
                                    pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
                            }
                            if (pr != null)
                            {
                                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                                para.GenerateExecutable = false;
                                para.GenerateInMemory = false;
                                para.IncludeDebugInformation = true;
                                para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
                                CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
                                if (res.Errors.HasErrors)
                                {
                                    Log("Compile failed.", "Errors:");
                                    foreach (CompilerError item in res.Errors)
                                        Log(item.ToString());
                                    Log(string.Empty);
                                    def = new DefaultObjectDefinition();
                                }
                                else
                                {
                                    Log("Compile succeeded.");
                                    def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
                                }
                            }
                            else
                                def = new DefaultObjectDefinition();
                        }
                    }
                    else
                        def = new DefaultObjectDefinition();
                    LevelData.ObjTypes.Add(ID, def);
                    def.Init(group.Value);
                }
            }
        }

        private void tilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            exportToolStripMenuItem.DropDown.Hide();
            FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory };
            if (a.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < LevelData.TilesArray.Length / 32; i++)
                {
                    LevelData.TileToBmp4bpp(LevelData.TilesArray, i, tilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)).Save(System.IO.Path.Combine(a.SelectedPath, i + ".png"));
                }
            }
        }

        private void objectsAboveHighPlaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            objectsAboveHighPlaneToolStripMenuItem.Checked = !objectsAboveHighPlaneToolStripMenuItem.Checked;
            DrawLevel();
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
            DrawLevel();
        }

        public void AddUndo(UndoAction action)
        {
            UndoList.Push(action);
            undoCtrlZToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(action.Name));
            if (UndoList.Count > 100)
            {
                UndoAction[] l = UndoList.ToArray();
                UndoList.Clear();
                for (int i = 99; i >= 0; i--)
                {
                    UndoList.Push(l[i]);
                }
                undoCtrlZToolStripMenuItem.DropDownItems.RemoveAt(100);
            }
            RedoList.Clear();
            redoCtrlYToolStripMenuItem.DropDownItems.Clear();
        }

        private void undoCtrlZToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DoUndo(undoCtrlZToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
        }

        private void redoCtrlYToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            DoRedo(redoCtrlYToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
        }

        private void DoUndo(int num)
        {
            for (int i = 0; i < num; i++)
            {
                undoCtrlZToolStripMenuItem.DropDownItems.RemoveAt(0);
                UndoAction act = UndoList.Pop();
                act.Undo();
                RedoList.Push(act);
                redoCtrlYToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
            }
            SelectedItems.Clear();
            SelectedObjectChanged();
            DrawLevel();
        }

        private void DoRedo(int num)
        {
            for (int i = 0; i < num; i++)
            {
                redoCtrlYToolStripMenuItem.DropDownItems.RemoveAt(0);
                UndoAction act = RedoList.Pop();
                act.Redo();
                UndoList.Push(act);
                undoCtrlZToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
            }
            SelectedItems.Clear();
            SelectedObjectChanged();
            DrawLevel();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;
            LevelBmp = new Bitmap(panel1.Width, panel1.Height);
            LevelImg8bpp = new BitmapBits(panel1.Width, panel1.Height);
            LevelGfx = Graphics.FromImage(LevelBmp);
            PanelGfx = panel1.CreateGraphics();
            loaded = false;
            switch (EditingMode)
            {
                case EditingModes.Objects:
                case EditingModes.PlaneA:
                    hScrollBar1.Maximum = (LevelData.FGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.FGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    break;
                case EditingModes.PlaneB:
                    hScrollBar1.Maximum = (LevelData.BGLayout.GetLength(0) * LevelData.chunksz) - panel1.Width;
                    vScrollBar1.Maximum = (LevelData.BGLayout.GetLength(1) * LevelData.chunksz) - panel1.Height;
                    break;
            }
            loaded = true;
            camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
            DrawLevel();
        }

        Point menuLoc;
        private void addObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjectSelect.ShowDialog(this) == DialogResult.OK)
            {
                byte ID = (byte)ObjectSelect.numericUpDown1.Value;
                byte sub = (byte)ObjectSelect.numericUpDown2.Value;
                switch (LevelData.ObjectFmt)
                {
                    case EngineVersion.S1:
                        S1ObjectEntry ent1 = (S1ObjectEntry)LevelData.CreateObject(ID);
                        LevelData.Objects.Add(ent1);
                        ent1.SubType = sub;
                        ent1.X = (ushort)(menuLoc.X + camera.X);
                        ent1.Y = (ushort)(menuLoc.Y + camera.Y);
                        ent1.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                        SelectedItems.Clear();
                        SelectedItems.Add(ent1);
                        SelectedObjectChanged();
                        AddUndo(new ObjectAddedUndoAction(ent1));
                        break;
                    case EngineVersion.S2:
                        S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                        LevelData.Objects.Add(ent);
                        ent.SubType = sub;
                        ent.X = (ushort)(menuLoc.X + camera.X);
                        ent.Y = (ushort)(menuLoc.Y + camera.Y);
                        ent.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                        SelectedItems.Clear();
                        SelectedItems.Add(ent);
                        SelectedObjectChanged();
                        AddUndo(new ObjectAddedUndoAction(ent));
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        S3KObjectEntry ent3 = (S3KObjectEntry)LevelData.CreateObject(ID);
                        LevelData.Objects.Add(ent3);
                        ent3.SubType = sub;
                        ent3.X = (ushort)(menuLoc.X + camera.X);
                        ent3.Y = (ushort)(menuLoc.Y + camera.Y);
                        SelectedItems.Clear();
                        SelectedItems.Add(ent3);
                        SelectedObjectChanged();
                        AddUndo(new ObjectAddedUndoAction(ent3));
                        break;
                    case EngineVersion.SCDPC:
                        SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                        LevelData.Objects.Add(entcd);
                        entcd.SubType = sub;
                        entcd.X = (ushort)(menuLoc.X + camera.X);
                        entcd.Y = (ushort)(menuLoc.Y + camera.Y);
                        entcd.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                        SelectedItems.Clear();
                        SelectedItems.Add(entcd);
                        SelectedObjectChanged();
                        AddUndo(new ObjectAddedUndoAction(entcd));
                        break;
                }
                LevelData.Objects.Sort();
                DrawLevel();
            }
        }

        private void addRingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (LevelData.RingFmt)
            {
                case EngineVersion.S1:
                    LevelData.Objects.Add(new S1ObjectEntry() { X = (ushort)(menuLoc.X + camera.X), Y = (ushort)(menuLoc.Y + camera.Y), ID = 0x25 });
                    SelectedItems.Clear();
                    SelectedItems.Add(LevelData.Objects[LevelData.Objects.Count - 1]);
                    SelectedObjectChanged();
                    AddUndo(new ObjectAddedUndoAction(LevelData.Objects[LevelData.Objects.Count - 1]));
                    LevelData.Objects.Sort();
                    break;
                case EngineVersion.S2:
                    LevelData.Rings.Add(new S2RingEntry() { X = (ushort)(menuLoc.X + camera.X), Y = (ushort)(menuLoc.Y + camera.Y) });
                    SelectedItems.Clear();
                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                    SelectedObjectChanged();
                    AddUndo(new ObjectAddedUndoAction(LevelData.Rings[LevelData.Rings.Count - 1]));
                    LevelData.Rings.Sort();
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    LevelData.Rings.Add(new S3KRingEntry() { X = (ushort)(menuLoc.X + camera.X), Y = (ushort)(menuLoc.Y + camera.Y) });
                    SelectedItems.Clear();
                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                    SelectedObjectChanged();
                    AddUndo(new ObjectAddedUndoAction(LevelData.Rings[LevelData.Rings.Count - 1]));
                    LevelData.Rings.Sort();
                    break;
            }
            DrawLevel();
        }

        private void addGroupOfObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ObjectSelect.ShowDialog(this) == DialogResult.OK)
            {
                byte ID = (byte)ObjectSelect.numericUpDown1.Value;
                byte sub = (byte)ObjectSelect.numericUpDown2.Value;
                using (AddGroupDialog dlg = new AddGroupDialog())
                {
                    dlg.Text = "Add Group of Objects";
                    dlg.XDist.Value = LevelData.getObjectDefinition(ID).Bounds(Point.Empty, sub).Width;
                    dlg.YDist.Value = LevelData.getObjectDefinition(ID).Bounds(Point.Empty, sub).Height;
                    if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        Point pt = new Point(menuLoc.X + camera.X, menuLoc.Y + camera.Y);
                        int xst = pt.X;
                        Size xsz = new Size((int)dlg.XDist.Value, 0);
                        Size ysz = new Size(0, (int)dlg.YDist.Value);
                        SelectedItems.Clear();
                        for (int y = 0; y < dlg.Columns.Value; y++)
                        {
                            for (int x = 0; x < dlg.Rows.Value; x++)
                            {
                                switch (LevelData.ObjectFmt)
                                {
                                    case EngineVersion.S1:
                                        S1ObjectEntry ent1 = (S1ObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(ent1);
                                        ent1.SubType = sub;
                                        ent1.X = (ushort)(pt.X);
                                        ent1.Y = (ushort)(pt.Y);
                                        ent1.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                        SelectedItems.Add(ent1);
                                        break;
                                    case EngineVersion.S2:
                                        S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(ent);
                                        ent.SubType = sub;
                                        ent.X = (ushort)(pt.X);
                                        ent.Y = (ushort)(pt.Y);
                                        ent.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                        SelectedItems.Add(ent);
                                        break;
                                    case EngineVersion.S3K:
                                    case EngineVersion.SKC:
                                        S3KObjectEntry ent3 = (S3KObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(ent3);
                                        ent3.SubType = sub;
                                        ent3.X = (ushort)(pt.X);
                                        ent3.Y = (ushort)(pt.Y);
                                        SelectedItems.Add(ent3);
                                        break;
                                    case EngineVersion.SCDPC:
                                        SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(entcd);
                                        entcd.SubType = sub;
                                        entcd.X = (ushort)(pt.X);
                                        entcd.Y = (ushort)(pt.Y);
                                        entcd.RememberState = LevelData.getObjectDefinition(ID).RememberState();
                                        SelectedItems.Add(entcd);
                                        break;
                                }
                                pt += xsz;
                            }
                            pt.X = xst;
                            pt += ysz;
                        }
                        SelectedObjectChanged();
                        LevelData.Objects.Sort();
                        DrawLevel();
                    }
                }
            }
        }

        private void addGroupOfRingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AddGroupDialog dlg = new AddGroupDialog())
            {
                dlg.Text = "Add Group of Rings";
                switch (LevelData.RingFmt)
                {
                    case EngineVersion.S2:
                        dlg.XDist.Value = LevelData.S2RingDef.Bounds(Point.Empty, Direction.Horizontal, 1).Width;
                        dlg.YDist.Value = LevelData.S2RingDef.Bounds(Point.Empty, Direction.Horizontal, 1).Height;
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        dlg.XDist.Value = LevelData.S3KRingDef.Bounds(Point.Empty).Width;
                        dlg.YDist.Value = LevelData.S3KRingDef.Bounds(Point.Empty).Height;
                        break;
                    default:
                        return;
                }
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    Point pt = new Point(menuLoc.X + camera.X, menuLoc.Y + camera.Y);
                    int xst = pt.X;
                    Size xsz = new Size((int)dlg.XDist.Value, 0);
                    Size ysz = new Size(0, (int)dlg.YDist.Value);
                    SelectedItems.Clear();
                    for (int y = 0; y < dlg.Columns.Value; y++)
                    {
                        for (int x = 0; x < dlg.Rows.Value; x++)
                        {
                            switch (LevelData.RingFmt)
                            {
                                case EngineVersion.S2:
                                    LevelData.Rings.Add(new S2RingEntry() { X = (ushort)(pt.X), Y = (ushort)(pt.Y) });
                                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                                    break;
                                case EngineVersion.S3K:
                                case EngineVersion.SKC:
                                    LevelData.Rings.Add(new S3KRingEntry() { X = (ushort)(pt.X), Y = (ushort)(pt.Y) });
                                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                                    break;
                            }
                            pt += xsz;
                        }
                        pt.X = xst;
                        pt += ysz;
                    }
                    SelectedObjectChanged();
                    LevelData.Rings.Sort();
                    DrawLevel();
                }
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Entry> selitems = new List<Entry>();
            foreach (Entry item in SelectedItems)
            {
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Remove((ObjectEntry)item);
                    selitems.Add(LevelData.GetBaseObjectType((ObjectEntry)item));
                }
                else if (item is RingEntry)
                {
                    LevelData.Rings.Remove((RingEntry)item);
                    selitems.Add(item);
                }
                else if (item is CNZBumperEntry)
                {
                    LevelData.Bumpers.Remove((CNZBumperEntry)item);
                    selitems.Add(item);
                }
            }
            if (selitems.Count == 0) return;
            Clipboard.SetData("SonLVLObjectList", selitems);
            AddUndo(new ObjectsDeletedUndoAction(selitems));
            SelectedItems.Clear();
            SelectedObjectChanged();
            DrawLevel();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Entry> selitems = new List<Entry>();
            foreach (Entry item in SelectedItems)
            {
                if (item is ObjectEntry)
                    selitems.Add(LevelData.GetBaseObjectType((ObjectEntry)item));
                else if (item is RingEntry)
                    selitems.Add(item);
                else if (item is CNZBumperEntry)
                    selitems.Add(item);
            }
            if (selitems.Count == 0) return;
            Clipboard.SetData("SonLVLObjectList", selitems);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Entry> objs = Clipboard.GetData("SonLVLObjectList") as List<Entry>;
            Point upleft = new Point(int.MaxValue, int.MaxValue);
            foreach (Entry item in objs)
            {
                upleft.X = Math.Min(upleft.X, item.X);
                upleft.Y = Math.Min(upleft.Y, item.Y);
            }
            Size off = new Size((menuLoc.X + camera.X) - upleft.X, (menuLoc.Y + camera.Y) - upleft.Y);
            foreach (Entry item in objs)
            {
                item.X += (ushort)off.Width;
                item.Y += (ushort)off.Height;
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Add((ObjectEntry)item);
                    LevelData.ChangeObjectType((ObjectEntry)item);
                }
                else if (item is RingEntry)
                    LevelData.Rings.Add((RingEntry)item);
                else if (item is CNZBumperEntry)
                    LevelData.Bumpers.Add((CNZBumperEntry)item);
            }
            AddUndo(new ObjectsPastedUndoAction(new List<Entry>(objs)));
            SelectedItems = new List<Entry>(objs);
            SelectedObjectChanged();
            LevelData.Objects.Sort();
            LevelData.Rings.Sort();
            if (LevelData.Bumpers != null) LevelData.Bumpers.Sort();
            DrawLevel();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Entry> selitems = new List<Entry>();
            foreach (Entry item in SelectedItems)
            {
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Remove((ObjectEntry)item);
                    selitems.Add(item);
                }
                else if (item is RingEntry)
                {
                    LevelData.Rings.Remove((RingEntry)item);
                    selitems.Add(item);
                }
                else if (item is CNZBumperEntry)
                {
                    LevelData.Bumpers.Remove((CNZBumperEntry)item);
                    selitems.Add(item);
                }
            }
            if (selitems.Count == 0) return;
            AddUndo(new ObjectsDeletedUndoAction(selitems));
            SelectedItems.Clear();
            SelectedObjectChanged();
            DrawLevel();
        }

        private void paletteToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            exportToolStripMenuItem.DropDown.Hide();
            SaveFileDialog a = new SaveFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png", RestoreDirectory = true };
            if (a.ShowDialog() == DialogResult.OK)
            {
                int line = paletteToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
                if (line < 4)
                {
                    Bitmap bmp = new Bitmap(16, 1, PixelFormat.Format4bppIndexed);
                    ColorPalette pal = bmp.Palette;
                    byte[] pix = new byte[8];
                    byte px = 1;
                    for (int i = 0; i < 16; i++)
                        pal.Entries[i] = LevelData.PaletteToColor(line, i, false);
                    for (int i = 0; i < 8; i++)
                    {
                        pix[i] = px;
                        px += 0x22;
                    }
                    bmp.Palette = pal;
                    BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, 16, 1), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
                    System.Runtime.InteropServices.Marshal.Copy(pix, 0, bmpd.Scan0, 8);
                    bmp.UnlockBits(bmpd);
                    bmp.Save(a.FileName);
                }
                else
                {
                    Bitmap bmp = new Bitmap(16, 4, PixelFormat.Format8bppIndexed);
                    ColorPalette pal = bmp.Palette;
                    for (int i = 0; i < 64; i++)
                        pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, false);
                    for (int i = 64; i < 256; i++)
                        pal.Entries[i] = Color.Black;
                    bmp.Palette = pal;
                    byte[] pix = new byte[64];
                    for (byte i = 0; i < 64; i++)
                        pix[i] = i;
                    BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, 16, 4), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    System.Runtime.InteropServices.Marshal.Copy(pix, 0, bmpd.Scan0, 64);
                    bmp.UnlockBits(bmpd);
                    bmp.Save(a.FileName);
                }
            }
        }

        private void tileEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TileEditor.Visible)
            {
                TileEditor.Hide();
                tileEditorToolStripMenuItem.Checked = false;
            }
            else
            {
                TileEditor.Show(this);
                tileEditorToolStripMenuItem.Checked = true;
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogWindow = new LogWindow();
            LogWindow.Show(this);
            logToolStripMenuItem.Enabled = false;
        }

        private void blendAlternatePaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AlternatePaletteDialog dlg = new AlternatePaletteDialog())
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    int underwater = LevelData.CurPal;
                    LevelData.CurPal = 0;
                    Color[,] pal = new Color[4, 16];
                    for (int l = 0; l < 4; l++)
                        for (int i = 0; i < 16; i++)
                        {
                            Color col = LevelData.PaletteToColor(l, i, false);
                            if (dlg.radioButton1.Checked)
                                pal[l, i] = col.Blend(dlg.BlendColor);
                            else if (dlg.radioButton2.Checked)
                                pal[l, i] = Color.FromArgb(Math.Min(col.R + dlg.BlendColor.R, 255), Math.Min(col.G + dlg.BlendColor.G, 255), Math.Min(col.B + dlg.BlendColor.B, 255));
                            else
                                pal[l, i] = Color.FromArgb(Math.Max(col.R - dlg.BlendColor.R, 0), Math.Max(col.G - dlg.BlendColor.G, 0), Math.Max(col.B - dlg.BlendColor.B, 0));
                        }
                    LevelData.CurPal = dlg.paletteIndex.SelectedIndex + 1;
                    for (int l = 0; l < 4; l++)
                        for (int i = 0; i < 16; i++)
                            LevelData.ColorToPalette(l, i, pal[l, i]);
                    LevelData.CurPal = underwater;
                    LevelData.PaletteChanged();
                }
            }
        }

        private void yWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rectangle r = LevelData.LevelBounds.Value;
            int b = r.Bottom;
            if (r.Y < 0)
                r.Y = 0;
            else
                r.Y = -0x100;
            r.Height = b - r.Y;
            LevelData.LevelBounds = r;
            DrawLevel();
        }

        private void collisionToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem item in collisionToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            DrawLevel();
        }

        private void paletteToolStripMenuItem2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem item in paletteToolStripMenuItem2.DropDownItems)
                item.Checked = false;
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            LevelData.CurPal = paletteToolStripMenuItem2.DropDownItems.IndexOf(e.ClickedItem);
            LevelData.PaletteChanged();
        }

        private void buildAndRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(System.IO.Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["buildscr"])) { WorkingDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["buildscr"])) }).WaitForExit();
            System.Diagnostics.Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["runcmd"]));
        }
    }

    internal enum EditingModes
    {
        Objects,
        PlaneA,
        PlaneB
    }

    public abstract class UndoAction
    {
        public string Name { get; protected set; }
        public abstract void Undo();
        public abstract void Redo();
    }

    internal class LayoutEditUndoAction : UndoAction
    {
        private List<Point> locations;
        private List<byte> oldtiles;
        private EditingModes mode;

        public LayoutEditUndoAction(EditingModes Mode, List<Point> Locations, List<byte> OldTiles)
        {
            Name = "Layout Edit" + " (" + Locations.Count + " chunk" + (Locations.Count > 1 ? "s" : "") + ")";
            mode = Mode;
            locations = Locations;
            oldtiles = OldTiles;
        }

        public override void Undo()
        {
            byte t;
            switch (mode)
            {
                case EditingModes.PlaneA:
                    for (int i = 0; i < locations.Count; i++)
                    {
                        t = LevelData.FGLayout[locations[i].X, locations[i].Y];
                        LevelData.FGLayout[locations[i].X, locations[i].Y] = oldtiles[i];
                        oldtiles[i] = t;
                    }
                    break;
                case EditingModes.PlaneB:
                    for (int i = 0; i < locations.Count; i++)
                    {
                        t = LevelData.FGLayout[locations[i].X, locations[i].Y];
                        LevelData.BGLayout[locations[i].X, locations[i].Y] = oldtiles[i];
                        oldtiles[i] = t;
                    }
                    break;
            }
        }

        public override void Redo()
        {
            Undo();
        }
    }

    internal class ObjectPropertyChangedUndoAction : UndoAction
    {
        private List<Entry> objects;
        private List<object> values;
        private string propname;

        public ObjectPropertyChangedUndoAction(List<Entry> Objects, List<object> Values, string PropertyName, string DisplayName)
        {
            Name = "Change of property " + DisplayName + " (" + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "") + ")";
            objects = Objects;
            values = Values;
            propname = PropertyName;
        }

        public override void Undo()
        {
            object val;
            System.Reflection.PropertyInfo prop;
            for (int i = 0; i < objects.Count; i++)
            {
                prop = objects[i].GetType().GetProperty(propname);
                val = prop.GetValue(objects[i], null);
                prop.SetValue(objects[i], values[i], null);
                values[i] = val;
            }
        }

        public override void Redo()
        {
            Undo();
        }
    }

    internal class ObjectMoveUndoAction : UndoAction
    {
        private List<Entry> objects;
        private List<Point> locations;
        
        public ObjectMoveUndoAction(List<Entry> Objects, List<Point> Locations)
        {
            Name = "Moved " + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "");
            objects = Objects;
            locations = Locations;
        }

        public override void Undo()
        {
            Point pt;
            for (int i = 0; i < objects.Count; i++)
            {
                pt = new Point(objects[i].X, objects[i].Y);
                objects[i].X = (ushort)locations[i].X;
                objects[i].Y = (ushort)locations[i].Y;
                locations[i] = pt;
            }
        }

        public override void Redo()
        {
            Undo();
        }
    }

    internal class ObjectAddedUndoAction : UndoAction
    {
        private Entry obj;

        public ObjectAddedUndoAction(Entry Obj)
        {
            Name = "Added object";
            obj = Obj;
        }

        public override void Redo()
        {
            if (obj is ObjectEntry)
            {
                LevelData.Objects.Add((ObjectEntry)obj);
            }
            else if (obj is RingEntry)
            {
                LevelData.Rings.Add((RingEntry)obj);
            }
            else if (obj is CNZBumperEntry)
            {
                LevelData.Bumpers.Add((CNZBumperEntry)obj);
            }
        }

        public override void Undo()
        {
            if (obj is ObjectEntry)
            {
                LevelData.Objects.Remove((ObjectEntry)obj);
            }
            else if (obj is RingEntry)
            {
                LevelData.Rings.Remove((RingEntry)obj);
            }
            else if (obj is CNZBumperEntry)
            {
                LevelData.Bumpers.Remove((CNZBumperEntry)obj);
            }
        }
    }

    internal class ObjectsDeletedUndoAction : UndoAction
    {
        private List<Entry> objs;

        public ObjectsDeletedUndoAction(List<Entry> Objects)
        {
            Name = "Deleted " + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "");
            objs = Objects;
        }

        public override void Undo()
        {
            foreach (Entry item in objs)
            {
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Add((ObjectEntry)item);
                }
                else if (item is RingEntry)
                {
                    LevelData.Rings.Add((RingEntry)item);
                }
                else if (item is CNZBumperEntry)
                {
                    LevelData.Bumpers.Add((CNZBumperEntry)item);
                }
            }
        }

        public override void Redo()
        {
            foreach (Entry item in objs)
            {
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Remove((ObjectEntry)item);
                }
                else if (item is RingEntry)
                {
                    LevelData.Rings.Remove((RingEntry)item);
                }
                else if (item is CNZBumperEntry)
                {
                    LevelData.Bumpers.Remove((CNZBumperEntry)item);
                }
            }
        }
    }


    internal class ObjectsPastedUndoAction : UndoAction
    {
        private List<Entry> objs;

        public ObjectsPastedUndoAction(List<Entry> Objects)
        {
            Name = "Pasted " + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "");
            objs = Objects;
        }

        public override void Redo()
        {
            foreach (Entry item in objs)
            {
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Add((ObjectEntry)item);
                }
                else if (item is RingEntry)
                {
                    LevelData.Rings.Add((RingEntry)item);
                }
                else if (item is CNZBumperEntry)
                {
                    LevelData.Bumpers.Add((CNZBumperEntry)item);
                }
            }
        }

        public override void Undo()
        {
            foreach (Entry item in objs)
            {
                if (item is ObjectEntry)
                {
                    LevelData.Objects.Remove((ObjectEntry)item);
                }
                else if (item is RingEntry)
                {
                    LevelData.Rings.Remove((RingEntry)item);
                }
                else if (item is CNZBumperEntry)
                {
                    LevelData.Bumpers.Remove((CNZBumperEntry)item);
                }
            }
        }
    }

    internal class ObjectIDChangedUndoAction : UndoAction
    {
        private ObjectEntry oldobj, newobj;

        public ObjectIDChangedUndoAction(ObjectEntry oldobj, ObjectEntry newobj)
        {
            Name = "Object ID changed";
            this.oldobj = oldobj;
            this.newobj = newobj;
        }

        public override void Undo()
        {
            LevelData.Objects[LevelData.Objects.IndexOf(newobj)] = oldobj;
            ObjectEntry o = oldobj;
            oldobj = newobj;
            newobj = o;
        }

        public override void Redo()
        {
            Undo();
        }
    }
}