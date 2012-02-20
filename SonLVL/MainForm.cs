using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            LevelData.MainForm = this;
            InitializeComponent();
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Log(e.Exception.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
            System.IO.File.WriteAllLines("SonLVL.log", LogFile.ToArray());
            using (ErrorDialog ed = new ErrorDialog("Unhandled Exception " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", true))
            {
                if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                    Close();
            }
        }

        ImageAttributes imageTransparency = new ImageAttributes();
        Dictionary<string, Dictionary<string, string>> ini;
        Bitmap LevelBmp;
        Graphics LevelGfx, Panel1Gfx, Panel2Gfx, Panel3Gfx;
        string levelPath;
        string level;
        string levelName;
        internal bool loaded;
        internal byte SelectedChunk;
        internal List<Entry> SelectedItems;
        ObjectList ObjectSelect;
        Stack<UndoAction> UndoList;
        Stack<UndoAction> RedoList;
        internal LogWindow LogWindow;
        internal List<string> LogFile = new List<string>();
        Dictionary<string, ToolStripMenuItem> levelMenuItems;
        List<BitmapBits> HUDFont;
        string HUDChars = "0123456789.-:/\\ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        internal void Log(params string[] lines)
        {
            LogFile.AddRange(lines);
            if (LogWindow != null)
                LogWindow.Invoke(new MethodInvoker(LogWindow.UpdateLines));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("SonLVL Updater.exe"))
            {
                Dictionary<string, Dictionary<string, string>> myini, updini;
                if (File.Exists("Updater.ini"))
                    myini = IniFile.Load("Updater.ini");
                else
                    myini = new Dictionary<string, Dictionary<string, string>>() { { string.Empty, new Dictionary<string, string>() } };
#if !DEBUG
                try
                {
#endif
                    using (System.Net.WebClient cli = new System.Net.WebClient())
                    {
                        string updatefile = Path.GetTempFileName();
                        cli.DownloadFile("http://x-hax.cultnet.net/MainMemory/SonLVL/update.ini", updatefile);
                        updini = IniFile.Load(updatefile);
                        File.Delete(updatefile);
                    }
                    List<string> updates = new List<string>();
                    foreach (KeyValuePair<string, Dictionary<string, string>> item in updini)
                    {
                        if (string.IsNullOrEmpty(item.Key)) continue;
                        if (myini[string.Empty].ContainsKey(item.Key))
                            if (int.Parse(myini[string.Empty][item.Key], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo) < int.Parse(item.Value["revision"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo))
                                updates.Add(item.Key);
                    }
                    if (updates.Count > 0)
                    {
                        List<string> message = new List<string>();
                        message.Add("The following components have updates available:");
                        foreach (string item in updates)
                            message.Add('\t' + item);
                        message.Add(string.Empty);
                        message.Add("Do you want to run the updater?");
                        if (MessageBox.Show(this, string.Join(Environment.NewLine, message.ToArray()), "SonLVL Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("SonLVL Updater.exe");
                            Close();
                            return;
                        }
                    }
#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "SonLVL Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
#endif
            }
            System.Drawing.Imaging.ColorMatrix x = new System.Drawing.Imaging.ColorMatrix();
            x.Matrix33 = 0.75f;
            imageTransparency.SetColorMatrix(x, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            HUDFont = new List<BitmapBits>();
            string HUDpath = Path.Combine(Application.StartupPath, "HUD");
            HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, "spc.png"))));
            for (int i = 0; i <= 9; i++)
                HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, i + ".png"))));
            HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, "..png"))));
            HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, "-.png"))));
            HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, "col.png"))));
            HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, "slsh.png"))));
            HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, "bslsh.png"))));
            for (int i = 0x41; i <= 0x5A; i++)
                HUDFont.Add(new BitmapBits(new Bitmap(Path.Combine(HUDpath, (char)i + ".png"))));
            hUDToolStripMenuItem.Checked = Properties.Settings.Default.ShowHUD;
            if (System.Diagnostics.Debugger.IsAttached)
                logToolStripMenuItem_Click(sender, e);
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Emulator))
            {
                if (File.Exists(Properties.Settings.Default.Emulator))
                    setupEmulatorToolStripMenuItem.Checked = true;
                else
                    Properties.Settings.Default.Emulator = null;
            }
            if (Properties.Settings.Default.MRUList == null)
                Properties.Settings.Default.MRUList = new System.Collections.Specialized.StringCollection();
            System.Collections.Specialized.StringCollection mru = new System.Collections.Specialized.StringCollection();
            foreach (string item in Properties.Settings.Default.MRUList)
            {
                if (File.Exists(item))
                {
                    mru.Add(item);
                    recentProjectsToolStripMenuItem.DropDownItems.Add(item);
                }
            }
            Properties.Settings.Default.MRUList = mru;
            if (Program.args.Length > 0)
                LoadINI(Program.args[0]);
        }

        private void LoadINI(string filename)
        {
            Panel1Gfx = panel1.CreateGraphics();
            Panel1Gfx.SetOptions();
            Panel2Gfx = panel2.CreateGraphics();
            Panel2Gfx.SetOptions();
            Panel3Gfx = panel3.CreateGraphics();
            Panel3Gfx.SetOptions();
            Log("Opening INI file \"" + filename + "\"...");
            ini = IniFile.Load(filename);
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(filename);
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
            timeZoneToolStripMenuItem.Visible = false;
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S1:
                    LevelData.chunksz = 256;
                    Icon = Properties.Resources.gogglemon;
                    LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                    break;
                case EngineVersion.SCDPC:
                    LevelData.chunksz = 256;
                    Icon = Properties.Resources.clockmon;
                    LevelData.UnknownImg = Properties.Resources.UnknownImg.Copy();
                    timeZoneToolStripMenuItem.Visible = true;
                    LevelData.littleendian = true;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
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
            buildAndRunToolStripMenuItem.Enabled = ini[string.Empty].ContainsKey("buildscr") & (ini[string.Empty].ContainsKey("romfile") | ini[string.Empty].ContainsKey("runcmd"));
            if (Properties.Settings.Default.MRUList.Contains(filename))
            {
                recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Properties.Settings.Default.MRUList.IndexOf(filename));
                Properties.Settings.Default.MRUList.Remove(filename);
            }
            Properties.Settings.Default.MRUList.Insert(0, filename);
            recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
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
            using (OpenFileDialog a = new OpenFileDialog()
            {
                DefaultExt = "ini",
                Filter = "INI Files|*.ini|All Files|*.*"
            })
                if (a.ShowDialog(this) == DialogResult.OK)
                {
                    loaded = false;
                    LoadINI(a.FileName);
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
            if (ini[levelPath].ContainsKey("displayname"))
                levelName = ini[levelPath]["displayname"];
            else
                levelName = level;
            Enabled = false;
            Text = LevelData.EngineVersion.ToString() + "LVL - Loading " + levelName + "...";
            Log("Loading " + levelName + "...");
#if !DEBUG
            backgroundWorker1.RunWorkerAsync();
#else
            backgroundWorker1_DoWork(null, null);
            backgroundWorker1_RunWorkerCompleted(null, null);
#endif
        }

        Exception initerror = null;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
#if !DEBUG
            try
            {
#endif
                SelectedChunk = 0;
                UndoList = new Stack<UndoAction>();
                RedoList = new Stack<UndoAction>();
                Dictionary<string, string> egr = ini[string.Empty];
                Dictionary<string, string> gr = ini[levelPath];
                LevelData.TimeZone = gr.ContainsKey("timezone") ? (TimeZone)Enum.Parse(typeof(TimeZone), gr["timezone"]) : TimeZone.None;
                LevelData.TileFmt = gr.ContainsKey("tile8fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["tile8fmt"]) : egr.ContainsKey("tile8fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["tile8fmt"]) : LevelData.EngineVersion;
                LevelData.BlockFmt = gr.ContainsKey("block16fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["block16fmt"]) : egr.ContainsKey("block16fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["block16fmt"]) : LevelData.EngineVersion;
                LevelData.ChunkFmt = gr.ContainsKey("chunkfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["chunkfmt"]) : egr.ContainsKey("chunkfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["chunkfmt"]) : LevelData.EngineVersion;
                LevelData.LayoutFmt = gr.ContainsKey("layoutfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["layoutfmt"]) : egr.ContainsKey("layoutfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["layoutfmt"]) : LevelData.EngineVersion;
                LevelData.PaletteFmt = gr.ContainsKey("palettefmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["palettefmt"]) : egr.ContainsKey("palettefmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["palettefmt"]) : LevelData.EngineVersion;
                LevelData.ObjectFmt = gr.ContainsKey("objectsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["objectsfmt"]) : egr.ContainsKey("objectsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["objectsfmt"]) : LevelData.EngineVersion;
                LevelData.RingFmt = gr.ContainsKey("ringsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["ringsfmt"]) : egr.ContainsKey("ringsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), egr["ringsfmt"]) : LevelData.EngineVersion;
                switch (LevelData.ChunkFmt)
                {
                    case EngineVersion.S1:
                    case EngineVersion.SCDPC:
                        LevelData.chunksz = 256;
                        break;
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        LevelData.chunksz = 128;
                        break;
                }
                string defcmp;
                string[] tilelist = gr["tile8"].Split('|');
                byte[] tmp = null;
                List<byte> data = new List<byte>();
                LevelData.Tiles = new MultiFileIndexer<byte[]>();
                if (LevelData.TileFmt != EngineVersion.SCDPC)
                {
                    switch (LevelData.TileFmt)
                    {
                        case EngineVersion.S1:
                        case EngineVersion.S2NA:
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
                            defcmp = "Uncompressed";
                            break;
                    }
                    LevelData.TileCmp = (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("tile8cmp", egr.GetValueOrDefault("tile8cmp", defcmp)));
                    foreach (string tileent in tilelist)
                    {
                        tmp = null;
                        string[] tileentsp = tileent.Split(':');
                        int off = -1;
                        if (tileentsp.Length > 1)
                        {
                            string offstr = tileentsp[1];
                            if (offstr.StartsWith("0x"))
                                off = int.Parse(offstr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                            else
                                off = int.Parse(offstr, System.Globalization.NumberStyles.Integer);
                        }
                        if (File.Exists(tileentsp[0]))
                        {
                            Log("Loading 8x8 tiles from file \"" + tileentsp[0] + "\", using compression " + LevelData.TileCmp.ToString() + "...");
                            tmp = Compression.Decompress(tileentsp[0], LevelData.TileCmp);
                            List<byte[]> tiles = new List<byte[]>();
                            for (int i = 0; i < tmp.Length; i += 32)
                            {
                                byte[] tile = new byte[32];
                                Array.Copy(tmp, i, tile, 0, 32);
                                tiles.Add(tile);
                            }
                            LevelData.Tiles.AddFile(tiles, off == -1 ? off : off / 32);
                        }
                        else
                        {
                            Log("8x8 tile file \"" + tileentsp[0] + "\" not found.");
                            LevelData.Tiles.AddFile(new List<byte[]>() { new byte[32] }, off == -1 ? off : off / 32);
                        }
                    }
                }
                else
                {
                    LevelData.TileCmp = Compression.CompressionType.SZDD;
                    if (File.Exists(gr["tile8"]))
                    {
                        Log("Loading 8x8 tiles from file \"" + gr["tile8"] + "\", using compression SZDD...");
                        tmp = Compression.Decompress(gr["tile8"], Compression.CompressionType.SZDD);
                        int sta = ByteConverter.ToInt32(tmp, 0xC);
                        int numt = ByteConverter.ToInt32(tmp, 8);
                        List<byte[]> tiles = new List<byte[]>();
                        for (int i = 0; i < numt; i++)
                        {
                            byte[] tile = new byte[32];
                            Array.Copy(tmp, sta, tile, 0, 32);
                            tiles.Add(tile);
                            sta += 32;
                        }
                        LevelData.Tiles.AddFile(tiles, -1);
                    }
                    else
                    {
                        Log("8x8 tile file \"" + gr["tile8"] + "\" not found.");
                        LevelData.Tiles.AddFile(new List<byte[]>() { new byte[32] }, -1);
                    }
                }
                LevelData.UpdateTileArray();
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
                    case EngineVersion.S2NA:
                        defcmp = "Uncompressed";
                        break;
                    default:
                        defcmp = "Uncompressed";
                        break;
                }
                LevelData.BlockCmp = (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("block16cmp", egr.GetValueOrDefault("block16cmp", defcmp)));
                tilelist = gr["block16"].Split('|');
                foreach (string tileent in tilelist)
                {
                    string[] tileentsp = tileent.Split(':');
                    int off = -1;
                    if (tileentsp.Length > 1)
                    {
                        string offstr = tileentsp[1];
                        if (offstr.StartsWith("0x"))
                            off = int.Parse(offstr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        else
                            off = int.Parse(offstr, System.Globalization.NumberStyles.Integer);
                    }
                    if (File.Exists(tileentsp[0]))
                    {
                        Log("Loading 16x16 blocks from file \"" + tileentsp[0] + "\", using compression " + LevelData.BlockCmp.ToString() + "...");
                        tmp = Compression.Decompress(tileentsp[0], LevelData.BlockCmp);
                        List<Block> tmpblk = new List<Block>();
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = false;
                        for (int ba = 0; ba < tmp.Length; ba += Block.Size)
                        {
                            tmpblk.Add(new Block(tmp, ba));
                        }
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = true;
                        LevelData.Blocks.AddFile(tmpblk, off == -1 ? off : off / Block.Size);
                    }
                    else
                    {
                        Log("16x16 block file \"" + tileentsp[0] + "\" not found.");
                        LevelData.Blocks.AddFile(new List<Block>() { new Block() }, off == -1 ? off : off / Block.Size);
                    }
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
                    case EngineVersion.S2NA:
                        defcmp = "Uncompressed";
                        break;
                    default:
                        defcmp = string.Empty;
                        break;
                }
                LevelData.ChunkCmp = (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("chunk" + LevelData.chunksz + "cmp", egr.GetValueOrDefault("chunk" + LevelData.chunksz + "cmp", defcmp)));
                tilelist = gr["chunk" + LevelData.chunksz].Split('|');
                data = new List<byte>();
                int fileind = 0;
                foreach (string tileent in tilelist)
                {
                    string[] tileentsp = tileent.Split(':');
                    int off = -1;
                    if (tileentsp.Length > 1)
                    {
                        string offstr = tileentsp[1];
                        if (offstr.StartsWith("0x"))
                            off = int.Parse(offstr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        else
                            off = int.Parse(offstr, System.Globalization.NumberStyles.Integer);
                    }
                    if (File.Exists(tileentsp[0]))
                    {
                        Log("Loading " + LevelData.chunksz + "x" + LevelData.chunksz + " chunks from file \"" + tileentsp[0] + "\", using compression " + LevelData.ChunkCmp.ToString() + "...");
                        tmp = Compression.Decompress(tileentsp[0], LevelData.ChunkCmp);
                        List<Chunk> tmpchnk = new List<Chunk>();
                        if (fileind == 0)
                        {
                            switch (LevelData.ChunkFmt)
                            {
                                case EngineVersion.S1:
                                case EngineVersion.SCD:
                                case EngineVersion.SCDPC:
                                    tmpchnk.Add(new Chunk());
                                    break;
                            }
                        }
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = false;
                        for (int ba = 0; ba < tmp.Length; ba += Chunk.Size)
                            tmpchnk.Add(new Chunk(tmp, ba));
                        if (LevelData.EngineVersion == EngineVersion.SKC)
                            LevelData.littleendian = true;
                        LevelData.Chunks.AddFile(tmpchnk, off == -1 ? off : off / Chunk.Size);
                        fileind++;
                    }
                    else
                    {
                        Log(LevelData.chunksz + "x" + LevelData.chunksz + " chunk file \"" + tileentsp[0] + "\" not found.");
                        LevelData.Chunks.AddFile(new List<Chunk>() { new Chunk() }, off == -1 ? off : off / Chunk.Size);
                    }
                }
                if (LevelData.Chunks.Count == 0)
                    LevelData.Chunks.AddFile(new List<Chunk>() { new Chunk() }, -1);
                int fgw, fgh, bgw, bgh;
                LevelData.FGLoop = null;
                LevelData.BGLoop = null;
                switch (LevelData.LayoutFmt)
                {
                    case EngineVersion.S1:
                    case EngineVersion.S2NA:
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                    case EngineVersion.SCDPC:
                        defcmp = "Uncompressed";
                        break;
                    case EngineVersion.S2:
                        defcmp = "Kosinski";
                        break;
                    default:
                        defcmp = "Uncompressed";
                        break;
                }
                LevelData.LayoutCmp = (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("layoutcmp", egr.GetValueOrDefault("layoutcmp", defcmp)));
                switch (LevelData.LayoutFmt)
                {
                    case EngineVersion.S1:
                        int s1xmax = int.Parse(ini[string.Empty]["levelwidthmax"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        int s1ymax = int.Parse(ini[string.Empty]["levelheightmax"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        if (File.Exists(gr["fglayout"]))
                        {
                            Log("Loading FG layout from file \"" + gr["fglayout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["fglayout"], LevelData.LayoutCmp);
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
                            Log("Loading BG layout from file \"" + gr["bglayout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["bglayout"], LevelData.LayoutCmp);
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
                    case EngineVersion.S2NA:
                        s1xmax = int.Parse(ini[string.Empty]["levelwidthmax"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        s1ymax = int.Parse(ini[string.Empty]["levelheightmax"], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
                        if (File.Exists(gr["fglayout"]))
                        {
                            Log("Loading FG layout from file \"" + gr["fglayout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["fglayout"], LevelData.LayoutCmp);
                            fgw = tmp[0] + 1;
                            fgh = tmp[1] + 1;
                            LevelData.FGLayout = new byte[fgw, fgh];
                            for (int lr = 0; lr < fgh; lr++)
                                for (int lc = 0; lc < fgw; lc++)
                                {
                                    if ((lr * fgw) + lc + 2 >= tmp.Length) break;
                                    LevelData.FGLayout[lc, lr] = tmp[(lr * fgw) + lc + 2];
                                }
                        }
                        else
                        {
                            Log("FG layout file \"" + gr["fglayout"] + "\" not found.");
                            LevelData.FGLayout = new byte[s1xmax, s1ymax];
                        }
                        if (File.Exists(gr["bglayout"]))
                        {
                            Log("Loading BG layout from file \"" + gr["bglayout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["bglayout"], LevelData.LayoutCmp);
                            bgw = tmp[0] + 1;
                            bgh = tmp[1] + 1;
                            LevelData.BGLayout = new byte[bgw, bgh];
                            for (int lr = 0; lr < bgh; lr++)
                                for (int lc = 0; lc < bgw; lc++)
                                {
                                    LevelData.BGLayout[lc, lr] = tmp[(lr * bgw) + lc + 2];
                                }
                        }
                        else
                        {
                            Log("BG layout file \"" + gr["bglayout"] + "\" not found.");
                            LevelData.BGLayout = new byte[s1xmax, s1ymax];
                        }
                        break;
                    case EngineVersion.S2:
                        LevelData.FGLayout = new byte[128, 16];
                        LevelData.BGLayout = new byte[128, 16];
                        if (File.Exists(gr["layout"]))
                        {
                            Log("Loading layout from file \"" + gr["layout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["layout"], LevelData.LayoutCmp);
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
                            Log("Loading layout from file \"" + gr["layout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["layout"], LevelData.LayoutCmp);
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
                            Log("Loading layout from file \"" + gr["layout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["layout"], LevelData.LayoutCmp);
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
                        if (File.Exists(gr["fglayout"]))
                        {
                            Log("Loading FG layout from file \"" + gr["fglayout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["fglayout"], LevelData.LayoutCmp);
                            for (int lr = 0; lr < 8; lr++)
                                for (int lc = 0; lc < 64; lc++)
                                {
                                    if ((lr * 64) + lc >= tmp.Length) break;
                                    LevelData.FGLayout[lc, lr] = tmp[(lr * 64) + lc];
                                }
                        }
                        else
                            Log("FG layout file \"" + gr["fglayout"] + "\" not found.");
                        LevelData.BGLayout = new byte[64, 8];
                        if (File.Exists(gr["bglayout"]))
                        {
                            Log("Loading BG layout from file \"" + gr["bglayout"] + "\", using compression " + LevelData.LayoutCmp.ToString() + "...");
                            tmp = Compression.Decompress(gr["bglayout"], LevelData.LayoutCmp);
                            for (int lr = 0; lr < 8; lr++)
                                for (int lc = 0; lc < 64; lc++)
                                {
                                    LevelData.BGLayout[lc, lr] = tmp[(lr * 64) + lc];
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
                LevelImgPalette.Entries[67] = Properties.Settings.Default.GridColor;
                LevelData.UnknownImg.Palette = LevelData.BmpPal;
                curpal = new Color[16];
                for (int i = 0; i < 16; i++)
                    curpal[i] = LevelData.PaletteToColor(0, i, false);
                LevelData.Sprites = new List<Sprite>();
                if (gr.ContainsKey("sprites"))
                {
                    tmp = Compression.Decompress(gr["sprites"], Compression.CompressionType.SZDD);
                    int numspr = ByteConverter.ToInt32(tmp, 8);
                    int taddr = ByteConverter.ToInt32(tmp, 0xC);
                    for (int i = 0; i < numspr; i++)
                    {
                        ushort width = ByteConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 4);
                        if ((width & 4) == 4)
                            width += 4;
                        ushort height = ByteConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 6);
                        ushort startcol = (ushort)(ByteConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 8) - 0x10);
                        BitmapBits bmp = new BitmapBits(width, height);
                        byte[] til = new byte[height * (width / 2)];
                        Array.Copy(tmp, taddr, til, 0, til.Length);
                        taddr += til.Length;
                        LevelData.LoadBitmap4BppIndexed(bmp, til, width / 2);
                        bmp.IncrementIndexes(startcol);
                        LevelData.Sprites.Add(new Sprite(bmp, new Point(ByteConverter.ToInt16(tmp, 0x10 + (i * 0xC) + 0), ByteConverter.ToInt16(tmp, 0x10 + (i * 0xC) + 2))));
                    }
                }
                LevelData.ObjTypes = new Dictionary<byte, ObjectDefinition>();
                LevelData.filecache = new Dictionary<string, byte[]>();
                LevelData.unkobj = new DefaultObjectDefinition();
                LevelData.unkobj.Init(new Dictionary<string, string> { { "name", "Unknown" } });
                if (!System.IO.Directory.Exists("dllcache"))
                {
                    System.IO.DirectoryInfo dir = System.IO.Directory.CreateDirectory("dllcache");
                    dir.Attributes |= System.IO.FileAttributes.Hidden;
                }
                Dictionary<string, Dictionary<string, string>> objini = new Dictionary<string, Dictionary<string, string>>();
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
                                    LevelData.Objects[LevelData.Objects.Count - 1].UpdateSprite();
                                }
                                break;
                            case EngineVersion.S2:
                            case EngineVersion.S2NA:
                                for (int oa = 0; oa < tmp.Length; oa += S2ObjectEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    ObjectEntry ent = new S2ObjectEntry(tmp, oa);
                                    LevelData.Objects.Add(ent);
                                    LevelData.ChangeObjectType(ent);
                                    LevelData.Objects[LevelData.Objects.Count - 1].UpdateSprite();
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
                                    LevelData.Objects[LevelData.Objects.Count - 1].UpdateSprite();
                                }
                                break;
                            case EngineVersion.SCDPC:
                                for (int oa = 0; oa < tmp.Length; oa += SCDObjectEntry.Size)
                                {
                                    if (ByteConverter.ToUInt64(tmp, oa) == 0xFFFFFFFFFFFFFFFF) break;
                                    ObjectEntry ent = new SCDObjectEntry(tmp, oa);
                                    LevelData.Objects.Add(ent);
                                    LevelData.ChangeObjectType(ent);
                                    LevelData.Objects[LevelData.Objects.Count - 1].UpdateSprite();
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
                        case EngineVersion.S2NA:
                            if (File.Exists(gr["rings"]))
                            {
                                Log("Loading rings from file \"" + gr["rings"] + "\", using compression " + gr.GetValueOrDefault("ringscmp", "Uncompressed") + "...");
                                tmp = Compression.Decompress(gr["rings"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("ringscmp", "Uncompressed")));
                                for (int oa = 0; oa < tmp.Length; oa += S2RingEntry.Size)
                                {
                                    if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                    S2RingEntry ent = new S2RingEntry(tmp, oa);
                                    LevelData.Rings.Add(ent);
                                    ent.UpdateSprite();
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
                                    S3KRingEntry ent = new S3KRingEntry(tmp, oa);
                                    LevelData.Rings.Add(ent);
                                    ent.UpdateSprite();
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
                            CNZBumperEntry ent = new CNZBumperEntry(tmp, i);
                            LevelData.Bumpers.Add(ent);
                            ent.UpdateSprite();
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
                            StartPositionEntry ent = new StartPositionEntry(System.IO.File.ReadAllBytes(stpos[0]), 0);
                            LevelData.StartPositions.Add(ent);
                            if (!string.IsNullOrEmpty(stpos[1]))
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(objini[stpos[1]], stpos[2]));
                            else
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(stpos[2]));
                            ent.UpdateSprite();
                        }
                        else
                        {
                            Log("Start position file \"" + stpos[0] + "\" not found.");
                            StartPositionEntry ent = new StartPositionEntry();
                            LevelData.StartPositions.Add(ent);
                            if (!string.IsNullOrEmpty(stpos[1]))
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(objini[stpos[1]], stpos[2]));
                            else
                                LevelData.StartPosDefs.Add(new StartPositionDefinition(stpos[2]));
                            ent.UpdateSprite();
                        }
                    }
                }
                LevelData.ColInds1 = new List<byte>();
                LevelData.ColInds2 = new List<byte>();
                switch (LevelData.EngineVersion)
                {
                    case EngineVersion.S1:
                    case EngineVersion.S2NA:
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                    case EngineVersion.SCDPC:
                        defcmp = "Uncompressed";
                        break;
                    case EngineVersion.S2:
                        defcmp = "Kosinski";
                        break;
                    default:
                        defcmp = "Uncompressed";
                        break;
                }
                LevelData.ColIndCmp = (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), gr.GetValueOrDefault("colindcmp", egr.GetValueOrDefault("colindcmp", defcmp)));
                switch (LevelData.ChunkFmt)
                {
                    case EngineVersion.S1:
                    case EngineVersion.SCD:
                    case EngineVersion.SCDPC:
                        if (gr.ContainsKey("colind") && File.Exists(gr["colind"]))
                            LevelData.ColInds1.AddRange(Compression.Decompress(gr["colind"], LevelData.ColIndCmp));
                        LevelData.ColInds2 = LevelData.ColInds1;
                        break;
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                        if (gr.ContainsKey("colind1") && File.Exists(gr["colind1"]))
                            LevelData.ColInds1.AddRange(Compression.Decompress(gr["colind1"], LevelData.ColIndCmp));
                        if (gr.ContainsKey("colind2"))
                        {
                            if (File.Exists(gr["colind2"]))
                                LevelData.ColInds2.AddRange(Compression.Decompress(gr["colind2"], LevelData.ColIndCmp));
                        }
                        else
                            LevelData.ColInds2 = LevelData.ColInds1;
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        if (gr.ContainsKey("colind"))
                        {
                            if (File.Exists(gr["colind"]))
                            {
                                tmp = Compression.Decompress(gr["colind"], LevelData.ColIndCmp);
                                int colindt = int.Parse(gr.GetValueOrDefault("colindsz", "1"));
                                switch (colindt)
                                {
                                    case 1:
                                        for (int i = 0; i < 0x600; i += 2)
                                        {
                                            LevelData.ColInds1.Add(tmp[i]);
                                            LevelData.ColInds2.Add(tmp[i + 1]);
                                        }
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
                if (File.Exists(gr.GetValueOrDefault("colarr1", ini[string.Empty].GetValueOrDefault("colarr1", string.Empty))))
                    tmp = Compression.Decompress(gr.GetValueOrDefault("colarr1", ini[string.Empty].GetValueOrDefault("colarr1", null)), Compression.CompressionType.Uncompressed);
                else
                    tmp = new byte[256 * 16];
                for (int i = 0; i < 256; i++)
                {
                    LevelData.ColArr1[i] = new sbyte[16];
                    for (int j = 0; j < 16; j++)
                        LevelData.ColArr1[i][j] = unchecked((sbyte)tmp[(i * 16) + j]);
                }
                if (File.Exists(gr.GetValueOrDefault("angles", ini[string.Empty].GetValueOrDefault("angles", string.Empty))))
                    LevelData.Angles = Compression.Decompress(gr.GetValueOrDefault("angles", ini[string.Empty].GetValueOrDefault("angles", string.Empty)), Compression.CompressionType.Uncompressed);
                else
                    LevelData.Angles = new byte[256];
                LevelData.BlockBmps = new List<Bitmap[]>();
                LevelData.BlockBmpBits = new List<BitmapBits[]>();
                LevelData.CompBlockBmps = new List<Bitmap>();
                LevelData.CompBlockBmpBits = new List<BitmapBits>();
                Log("Drawing block bitmaps...");
                for (int bi = 0; bi < LevelData.Blocks.Count; bi++)
                {
                    LevelData.BlockBmps.Add(new Bitmap[2]);
                    LevelData.BlockBmpBits.Add(new BitmapBits[2]);
                    LevelData.CompBlockBmps.Add(null);
                    LevelData.CompBlockBmpBits.Add(null);
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
                LevelData.CompChunkBmps = new List<Bitmap>();
                LevelData.CompChunkBmpBits = new List<BitmapBits>();
                Log("Drawing chunk bitmaps...");
                for (int ci = 0; ci < LevelData.Chunks.Count; ci++)
                {
                    LevelData.ChunkBmps.Add(new Bitmap[2]);
                    LevelData.ChunkBmpBits.Add(new BitmapBits[2]);
                    LevelData.ChunkColBmps.Add(new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Add(new BitmapBits[2]);
                    LevelData.CompChunkBmps.Add(null);
                    LevelData.CompChunkBmpBits.Add(null);
                    LevelData.RedrawChunk(ci);
                }
#if !DEBUG
            }
            catch (Exception ex) { initerror = ex; }
#endif
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (initerror != null)
            {
                Log(initerror.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                System.IO.File.WriteAllLines("SonLVL.log", LogFile.ToArray());
                using (ErrorDialog ed = new ErrorDialog(initerror.GetType().Name + ": " + initerror.Message + "\nLog file has been saved to " + System.IO.Path.Combine(Environment.CurrentDirectory, "SonLVL.log") + ".\nSend this to MainMemory on the Sonic Retro forums.", true))
                    if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel) Close();
                Enabled = true;
                return;
            }
            Log("Load completed.");
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    LevelImg8bpp = new BitmapBits((int)(panel1.Width / ZoomLevel), (int)(panel1.Height / ZoomLevel));
                    break;
                case 1:
                    LevelImg8bpp = new BitmapBits((int)(panel2.Width / ZoomLevel), (int)(panel2.Height / ZoomLevel));
                    break;
                case 2:
                    LevelImg8bpp = new BitmapBits((int)(panel3.Width / ZoomLevel), (int)(panel3.Height / ZoomLevel));
                    break;
                default:
                    LevelImg8bpp = new BitmapBits(1, 1);
                    break;
            }
            ChunkSelector.Images = LevelData.CompChunkBmps;
            ChunkSelector.ImageSize = LevelData.chunksz;
            BlockSelector.Images = LevelData.CompBlockBmps;
            BlockSelector.ChangeSize();
            CollisionSelector.Images = new List<Bitmap>(LevelData.ColBmps);
            CollisionSelector.ChangeSize();
            ChunkSelector.SelectedIndex = 0;
            ChunkPicture.Size = new Size(LevelData.chunksz, LevelData.chunksz);
            BlockSelector.SelectedIndex = 0;
            TileSelector.Images.Clear();
            for (int i = 0; i < LevelData.Tiles.Count; i++)
                TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, 2));
            TileSelector.SelectedIndex = 0;
            TileSelector.ChangeSize();
            switch (LevelData.ChunkFmt)
            {
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    BlockCollision2.Visible = false;
                    button2.Visible = false;
                    path2ToolStripMenuItem.Visible = false;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    BlockCollision2.Visible = true;
                    button2.Visible = true;
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
            Text = LevelData.EngineVersion.ToString() + "LVL - " + levelName;
            hScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel1.Width, 0);
            vScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel1.Height, 0);
            hScrollBar1.Value = 0;
            hScrollBar1.SmallChange = 16;
            hScrollBar1.LargeChange = 128;
            vScrollBar1.Value = 0;
            vScrollBar1.SmallChange = 16;
            vScrollBar1.LargeChange = 128;
            hScrollBar1.Enabled = true;
            vScrollBar1.Enabled = true;
            hScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel2.Width, 0);
            vScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel2.Height, 0);
            hScrollBar2.Value = 0;
            hScrollBar2.SmallChange = 16;
            hScrollBar2.LargeChange = 128;
            vScrollBar2.Value = 0;
            vScrollBar2.SmallChange = 16;
            vScrollBar2.LargeChange = 128;
            hScrollBar2.Enabled = true;
            vScrollBar2.Enabled = true;
            hScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel3.Width, 0);
            vScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel3.Height, 0);
            hScrollBar3.Value = 0;
            hScrollBar3.SmallChange = 16;
            hScrollBar3.LargeChange = 128;
            vScrollBar3.Value = 0;
            vScrollBar3.SmallChange = 16;
            vScrollBar3.LargeChange = 128;
            hScrollBar3.Enabled = true;
            vScrollBar3.Enabled = true;
            loaded = true;
            SelectedItems = new List<Entry>();
            undoCtrlZToolStripMenuItem.DropDownItems.Clear();
            redoCtrlYToolStripMenuItem.DropDownItems.Clear();
            saveToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            exportToolStripMenuItem.Enabled = true;
            paletteToolStripMenuItem2.DropDownItems.Clear();
            foreach (string item in LevelData.PalName)
                paletteToolStripMenuItem2.DropDownItems.Add(new ToolStripMenuItem(item));
            ((ToolStripMenuItem)paletteToolStripMenuItem2.DropDownItems[0]).Checked = true;
            blendAlternatePaletteToolStripMenuItem.Enabled = LevelData.Palette.Count > 1;
            timeZoneToolStripMenuItem.Visible = LevelData.TimeZone != TimeZone.None;
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
        void ObjectProperties_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.PropertyDescriptor == null) return;
            oldvalues = new List<object>();
            foreach (Entry item in SelectedItems)
                oldvalues.Add(item.GetType().GetProperty(e.NewSelection.PropertyDescriptor.Name).GetValue(item, null));
        }

        void ObjectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            AddUndo(new ObjectPropertyChangedUndoAction(new List<Entry>(SelectedItems), oldvalues, e.ChangedItem.PropertyDescriptor.Name, e.ChangedItem.PropertyDescriptor.DisplayName));
            oldvalues = new List<object>();
            foreach (Entry item in SelectedItems)
            {
                oldvalues.Add(item.GetType().GetProperty(e.ChangedItem.PropertyDescriptor.Name).GetValue(item, null));
                item.UpdateSprite();
            }
            DrawLevel();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Saving " + levelPath + "...");
            Dictionary<string, string> gr = ini[levelPath];
            string[] tilelist;
            int fileind = -1;
            List<byte> tmp;
            ReadOnlyCollection<ReadOnlyCollection<byte[]>> tilefiles = LevelData.Tiles.GetFiles();
            if (LevelData.TileFmt != EngineVersion.SCDPC)
            {
                tilelist = gr["tile8"].Split('|');
                foreach (string tileent in tilelist)
                {
                    fileind++;
                    string[] tileentsp = tileent.Split(':');
                    tmp = new List<byte>();
                    foreach (byte[] item in tilefiles[fileind])
                    {
                        tmp.AddRange(item);
                    }
                    Compression.Compress(tmp.ToArray(), tileentsp[0], LevelData.TileCmp);
                }
            }
            else
            {
                List<ushort>[] tilepals = new List<ushort>[4];
                for (int i = 0; i < 4; i++)
                    tilepals[i] = new List<ushort>();
                foreach (Block blk in LevelData.Blocks)
                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 2; x++)
                            if (!tilepals[blk.tiles[x, y].Palette].Contains(blk.tiles[x, y].Tile))
                                tilepals[blk.tiles[x, y].Palette].Add(blk.tiles[x, y].Tile);
                foreach (Block blk in LevelData.Blocks)
                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 2; x++)
                        {
                            byte pal = blk.tiles[x, y].Palette;
                            int c = 0;
                            for (int i = pal - 1; i >= 0; i--)
                                c += tilepals[i].Count;
                            blk.tiles[x, y].Tile = (ushort)(tilepals[pal].IndexOf(blk.tiles[x, y].Tile) + c);
                        }
                List<byte[]> tiles = new List<byte[]>();
                for (int p = 0; p < 4; p++)
                    foreach (ushort item in tilepals[p])
                        if (LevelData.Tiles[item] != null)
                            tiles.Add(LevelData.Tiles[item]);
                        else
                            tiles.Add(new byte[32]);
                LevelData.Tiles.Clear();
                LevelData.Tiles.AddFile(tiles, -1);
                LevelData.UpdateTileArray();
                tmp = new List<byte>();
                tmp.Add(0x53);
                tmp.Add(0x43);
                tmp.Add(0x52);
                tmp.Add(0x4C);
                tmp.AddRange(ByteConverter.GetBytes(0x18 + (LevelData.Tiles.Count * 4) + (LevelData.Tiles.Count * 32)));
                tmp.AddRange(ByteConverter.GetBytes(LevelData.Tiles.Count));
                tmp.AddRange(ByteConverter.GetBytes(0x18 + (LevelData.Tiles.Count * 4)));
                for (int i = 0; i < 4; i++)
                    tmp.AddRange(ByteConverter.GetBytes((ushort)tilepals[i].Count));
                for (int i = 0; i < LevelData.Tiles.Count; i++)
                {
                    tmp.AddRange(ByteConverter.GetBytes((ushort)8));
                    tmp.AddRange(ByteConverter.GetBytes((ushort)8));
                }
                tmp.AddRange(LevelData.TileArray);
                Compression.Compress(tmp.ToArray(), gr["tile8"], Compression.CompressionType.SZDD);
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
                Compression.Compress(tmp.ToArray(), tileentsp[0], LevelData.BlockCmp);
            }
            tilelist = gr["chunk" + LevelData.chunksz].Split('|');
            fileind = -1;
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
                    switch (LevelData.ChunkFmt)
                    {
                        case EngineVersion.S1:
                        case EngineVersion.SCD:
                        case EngineVersion.SCDPC:
                            tmp.RemoveRange(0, Chunk.Size);
                            break;
                    }
                }
                Compression.Compress(tmp.ToArray(), tileentsp[0], LevelData.ChunkCmp);
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
                    Compression.Compress(tmp.ToArray(), gr["fglayout"], LevelData.LayoutCmp);
                    tmp = new List<byte>();
                    tmp.Add((byte)(LevelData.BGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(LevelData.BGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < LevelData.BGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < LevelData.BGLayout.GetLength(0); lc++)
                            tmp.Add((byte)(LevelData.BGLayout[lc, lr] | (LevelData.BGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), gr["bglayout"], LevelData.LayoutCmp);
                    break;
                case EngineVersion.S2NA:
                    tmp = new List<byte>();
                    tmp.Add((byte)(LevelData.FGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(LevelData.FGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < LevelData.FGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < LevelData.FGLayout.GetLength(0); lc++)
                            tmp.Add(LevelData.FGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), gr["fglayout"], LevelData.LayoutCmp);
                    tmp = new List<byte>();
                    tmp.Add((byte)(LevelData.BGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(LevelData.BGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < LevelData.BGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < LevelData.BGLayout.GetLength(0); lc++)
                            tmp.Add(LevelData.BGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), gr["bglayout"], LevelData.LayoutCmp);
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
                    Compression.Compress(tmp.ToArray(), gr["layout"], LevelData.LayoutCmp);
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
                    for (int la = 0; la < 32; la++)
                    {
                        if (la < fgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (la * fgw))));
                        else
                            tmp.AddRange(new byte[2]);
                        if (la < bgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (fgh * fgw) + (la * bgw))));
                        else
                            tmp.AddRange(new byte[2]);
                    }
                    for (int y = 0; y < fgh; y++)
                        for (int x = 0; x < fgw; x++)
                            tmp.Add(LevelData.FGLayout[x, y]);
                    for (int y = 0; y < bgh; y++)
                        for (int x = 0; x < bgw; x++)
                            tmp.Add(LevelData.BGLayout[x, y]);
                    Compression.Compress(tmp.ToArray(), gr["layout"], LevelData.LayoutCmp);
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
                    for (int la = 0; la < 32; la++)
                    {
                        if (la < fgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (la * fgw))));
                        else
                            tmp.AddRange(new byte[2]);
                        if (la < bgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (fgh * fgw) + (la * bgw))));
                        else
                            tmp.AddRange(new byte[2]);
                    }
                    List<byte> l = new List<byte>();
                    for (int y = 0; y < fgh; y++)
                        for (int x = 0; x < fgw; x++)
                            l.Add(LevelData.FGLayout[x, y]);
                    for (int y = 0; y < bgh; y++)
                        for (int x = 0; x < bgw; x++)
                            l.Add(LevelData.BGLayout[x, y]);
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add(l[i ^ 1]);
                    Compression.Compress(tmp.ToArray(), gr["layout"], LevelData.LayoutCmp);
                    break;
                case EngineVersion.SCDPC:
                    tmp = new List<byte>();
                    for (int lr = 0; lr < 8; lr++)
                        for (int lc = 0; lc < 64; lc++)
                            tmp.Add(LevelData.FGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), gr["fglayout"], LevelData.LayoutCmp);
                    tmp = new List<byte>();
                    for (int lr = 0; lr < 8; lr++)
                        for (int lc = 0; lc < 64; lc++)
                            tmp.Add(LevelData.BGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), gr["bglayout"], LevelData.LayoutCmp);
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
            else
            {
                List<byte[]> palfiles = new List<byte[]>();
                string[] palentstr;
                byte palfilenum = 0;
                if (gr.ContainsKey("palette"))
                {
                    palentstr = gr["palette"].Split('|');
                    for (byte pn = 0; pn < palentstr.Length; pn++)
                    {
                        string[] palent = palentstr[pn].Split(':');
                        palfiles.Add(System.IO.File.ReadAllBytes(palent[0]));
                    }
                    for (int pl = 0; pl < 4; pl++)
                    {
                        for (int pi = 0; pi < 16; pi++)
                        {
                            palfiles[LevelData.PalNum[0][pl, pi]][LevelData.PalAddr[0][pl, pi] * 4] = (byte)((LevelData.Palette[0][pl, pi] & 0xF) * 0x11);
                            palfiles[LevelData.PalNum[0][pl, pi]][LevelData.PalAddr[0][pl, pi] * 4 + 1] = (byte)(((LevelData.Palette[0][pl, pi] & 0xF0) >> 4) * 0x11);
                            palfiles[LevelData.PalNum[0][pl, pi]][LevelData.PalAddr[0][pl, pi] * 4 + 2] = (byte)(((LevelData.Palette[0][pl, pi] & 0xF00) >> 8) * 0x11);
                        }
                    }
                    for (byte pn = 0; pn < palentstr.Length; pn++)
                        System.IO.File.WriteAllBytes(palentstr[pn].Split(':')[0], palfiles[pn]);
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
                    case EngineVersion.S2NA:
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
                    case EngineVersion.S2NA:
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
            switch (LevelData.ChunkFmt)
            {
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    if (gr.ContainsKey("colind"))
                        Compression.Compress(LevelData.ColInds1.ToArray(), gr["colind"], LevelData.ColIndCmp);
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    if (gr.ContainsKey("colind1"))
                        Compression.Compress(LevelData.ColInds1.ToArray(),gr["colind1"], LevelData.ColIndCmp);
                    if (gr.ContainsKey("colind2"))
                        Compression.Compress(LevelData.ColInds2.ToArray(), gr["colind2"], LevelData.ColIndCmp);
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
                                for (int i = 0; i < 0x300; i++)
                                {
                                    tmp.Add(LevelData.ColInds1[i]);
                                    tmp.Add(LevelData.ColInds2[i]);
                                }
                                break;
                            case 2:
                                foreach (byte item in LevelData.ColInds1)
                                    tmp.AddRange(ByteConverter.GetBytes((ushort)item));
                                foreach (byte item in LevelData.ColInds2)
                                    tmp.AddRange(ByteConverter.GetBytes((ushort)item));
                                break;
                        }
                        Compression.Compress(tmp.ToArray(), gr["colind"], LevelData.ColIndCmp);
                    }
                    break;
            }
            if (gr.GetValueOrDefault("colarr1", ini[string.Empty].GetValueOrDefault("colarr1", null)) != null)
            {
                tmp = new List<byte>();
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 16; j++)
                        tmp.Add(unchecked((byte)LevelData.ColArr1[i][j]));
                Compression.Compress(tmp.ToArray(), gr.GetValueOrDefault("colarr1", ini[string.Empty].GetValueOrDefault("colarr1", null)), Compression.CompressionType.Uncompressed);
            }
            if (gr.GetValueOrDefault("colarr2", ini[string.Empty].GetValueOrDefault("colarr2", null)) != null)
            {
                sbyte[][] rotcol = LevelData.GenerateRotatedCollision();
                tmp = new List<byte>();
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 16; j++)
                        tmp.Add(unchecked((byte)rotcol[i][j]));
                Compression.Compress(tmp.ToArray(), gr.GetValueOrDefault("colarr2", ini[string.Empty].GetValueOrDefault("colarr2", null)), Compression.CompressionType.Uncompressed);
            }
            if (gr.GetValueOrDefault("angles", ini[string.Empty].GetValueOrDefault("angles", null)) != null)
                Compression.Compress(LevelData.Angles, gr.GetValueOrDefault("angles", ini[string.Empty].GetValueOrDefault("angles", null)), Compression.CompressionType.Uncompressed);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        BitmapBits LevelImg8bpp;
        internal ColorPalette LevelImgPalette;
        double ZoomLevel = 1;
        internal void DrawLevel()
        {
            if (!loaded) return;
            LevelImg8bpp.Clear();
            Point pnlcur;
            Point camera;
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    pnlcur = panel1.PointToClient(Cursor.Position);
                    camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
                    for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel1.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                    {
                        for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel1.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                        {
                            if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count & lowToolStripMenuItem.Checked)
                                LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            if (objectsAboveHighPlaneToolStripMenuItem.Checked)
                            {
                                if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                {
                                    if (highToolStripMenuItem.Checked)
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
                        ObjectEntry oe = LevelData.Objects[oi];
                        Sprite spr = oe.Sprite;
                        Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                        if (ObjectVisible(oe))
                            LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                    }
                    for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                    {
                        switch (LevelData.RingFmt)
                        {
                            case EngineVersion.S2:
                            case EngineVersion.S2NA:
                                S2RingEntry re = (S2RingEntry)LevelData.Rings[ri];
                                Sprite spr = re.Sprite;
                                Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                                LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                S3KRingEntry re3 = (S3KRingEntry)LevelData.Rings[ri];
                                spr = re3.Sprite;
                                pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                                LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                                break;
                        }
                    }
                    if (LevelData.Bumpers != null)
                        foreach (CNZBumperEntry item in LevelData.Bumpers)
                        {
                            Sprite spr = item.Sprite;
                            Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                            LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                        }
                    foreach (StartPositionEntry item in LevelData.StartPositions)
                    {
                        Sprite spr = item.Sprite;
                        Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                        LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                    }
                    if (!objectsAboveHighPlaneToolStripMenuItem.Checked)
                    {
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel1.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                        {
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel1.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                            {
                                if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                {
                                    if (highToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    if (path1ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    else if (path2ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                }
                            }
                        }
                    }
                    if (enableGridToolStripMenuItem.Checked)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel1.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel1.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                            {
                                LevelImg8bpp.DrawLine(67, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, x * LevelData.chunksz - camera.X + LevelData.chunksz - 1, y * LevelData.chunksz - camera.Y);
                                LevelImg8bpp.DrawLine(67, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y + LevelData.chunksz - 1);
                            }
                    Rectangle hudbnd = Rectangle.Empty;
                    int ringcnt = 0;
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S1:
                            foreach (ObjectEntry item in LevelData.Objects)
                                if (item.ID == 0x25)
                                    ringcnt += Math.Min(6, item.SubType & 7) + 1;
                            break;
                        case EngineVersion.S2:
                        case EngineVersion.S2NA:
                            foreach (RingEntry item in LevelData.Rings)
                                ringcnt += ((S2RingEntry)item).Count;
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            ringcnt = LevelData.Rings.Count;
                            break;
                    }
                    if (hUDToolStripMenuItem.Checked)
                        DrawHUDStr(8, 8,
                            "Screen Pos: " + camera.X.ToString("X4") + ' ' + camera.Y.ToString("X4") + '\n' +
                            "Level Size: " + (LevelData.FGLayout.GetLength(0) * LevelData.chunksz).ToString("X4") + ' ' + (LevelData.FGLayout.GetLength(1) * LevelData.chunksz).ToString("X4") + '\n' +
                            "Objects: " + LevelData.Objects.Count + '\n' +
                            "Rings: " + ringcnt
                            , out hudbnd);
                    LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).Clone(new Rectangle(0, 0, LevelImg8bpp.Width, LevelImg8bpp.Height), PixelFormat.Format32bppArgb);
                    LevelGfx = Graphics.FromImage(LevelBmp);
                    LevelGfx.SetOptions();
                    foreach (Entry item in SelectedItems)
                    {
                        if (item is ObjectEntry)
                        {
                            ObjectEntry objitem = (ObjectEntry)item;
                            Rectangle objbnd = LevelData.GetObjectDefinition(objitem.ID).Bounds(objitem, camera);
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), objbnd);
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, objbnd);
                        }
                        else if (item is S2RingEntry)
                        {
                            S2RingEntry rngitem = (S2RingEntry)item;
                            Rectangle bnd = LevelData.S2RingDef.Bounds(rngitem, camera);
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Yellow)), bnd);
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
                        }
                        else if (item is S3KRingEntry)
                        {
                            S3KRingEntry rngitem = (S3KRingEntry)item;
                            Rectangle bnd = LevelData.S3KRingDef.Bounds(rngitem, camera);
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Yellow)), bnd);
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
                        }
                        else if (item is CNZBumperEntry)
                        {
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), LevelData.unkobj.Bounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, new Point(camera.X, camera.Y)));
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, LevelData.unkobj.Bounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, new Point(camera.X, camera.Y)));
                        }
                        else if (item is StartPositionEntry)
                        {
                            StartPositionEntry strtitem = (StartPositionEntry)item;
                            Rectangle bnd = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(strtitem)].Bounds(strtitem, camera);
                            LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), bnd);
                            LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
                        }
                    }
                    if (LevelData.LayoutFmt == EngineVersion.S1)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel1.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel1.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                                if (LevelData.FGLoop[x, y])
                                    LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = 3 }, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, LevelData.chunksz, LevelData.chunksz);
                    if (selecting)
                    {
                        Rectangle selbnds = Rectangle.FromLTRB(
                        Math.Min(selpoint.X, lastmouse.X) - camera.X,
                        Math.Min(selpoint.Y, lastmouse.Y) - camera.Y,
                        Math.Max(selpoint.X, lastmouse.X) - camera.X,
                        Math.Max(selpoint.Y, lastmouse.Y) - camera.Y);
                        LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), selbnds);
                        LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, selbnds);
                    }
                    Panel1Gfx.DrawImage(LevelBmp, 0, 0, panel1.Width, panel1.Height);
                    break;
                case 1:
                    pnlcur = panel2.PointToClient(Cursor.Position);
                    camera = new Point(hScrollBar2.Value, vScrollBar2.Value);
                    for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel2.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                    {
                        for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel2.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                        {
                            if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count & lowToolStripMenuItem.Checked)
                                LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            if (objectsAboveHighPlaneToolStripMenuItem.Checked)
                            {
                                if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                {
                                    if (highToolStripMenuItem.Checked)
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
                        ObjectEntry oe = LevelData.Objects[oi];
                        Sprite spr = oe.Sprite;
                        Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                        if (ObjectVisible(oe))
                            LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                    }
                    for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                    {
                        switch (LevelData.RingFmt)
                        {
                            case EngineVersion.S2:
                            case EngineVersion.S2NA:
                                S2RingEntry re = (S2RingEntry)LevelData.Rings[ri];
                                Sprite spr = re.Sprite;
                                Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                                LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                S3KRingEntry re3 = (S3KRingEntry)LevelData.Rings[ri];
                                spr = re3.Sprite;
                                pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                                LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                                break;
                        }
                    }
                    if (LevelData.Bumpers != null)
                        foreach (CNZBumperEntry item in LevelData.Bumpers)
                        {
                            Sprite spr = item.Sprite;
                            Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                            LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                        }
                    foreach (StartPositionEntry item in LevelData.StartPositions)
                    {
                        Sprite spr = item.Sprite;
                        Point pt = new Point(spr.Offset.X - camera.X, spr.Offset.Y - camera.Y);
                        LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                    }
                    if (!objectsAboveHighPlaneToolStripMenuItem.Checked)
                    {
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel2.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                        {
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel2.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                            {
                                if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                                {
                                    if (highToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    if (path1ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                    else if (path2ToolStripMenuItem.Checked)
                                        LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                }
                            }
                        }
                    }
                    if (enableGridToolStripMenuItem.Checked)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel2.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel2.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                            {
                                LevelImg8bpp.DrawLine(67, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, x * LevelData.chunksz - camera.X + LevelData.chunksz - 1, y * LevelData.chunksz - camera.Y);
                                LevelImg8bpp.DrawLine(67, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y + LevelData.chunksz - 1);
                            }
                    ringcnt = 0;
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S1:
                            foreach (ObjectEntry item in LevelData.Objects)
                                if (item.ID == 0x25)
                                    ringcnt += Math.Min(6, item.SubType & 7) + 1;
                            break;
                        case EngineVersion.S2:
                        case EngineVersion.S2NA:
                            foreach (RingEntry item in LevelData.Rings)
                                ringcnt += ((S2RingEntry)item).Count;
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            ringcnt = LevelData.Rings.Count;
                            break;
                    }
                    if (hUDToolStripMenuItem.Checked)
                    DrawHUDStr(8, 8,
                        "Screen Pos: " + camera.X.ToString("X4") + ' ' + camera.Y.ToString("X4") + '\n' +
                        "Level Size: " + (LevelData.FGLayout.GetLength(0) * LevelData.chunksz).ToString("X4") + ' ' + (LevelData.FGLayout.GetLength(1) * LevelData.chunksz).ToString("X4") + '\n' +
                        "Objects: " + LevelData.Objects.Count + '\n' +
                        "Rings: " + ringcnt + '\n' +
                        "Chunk: " + SelectedChunk.ToString("X2")
                        , out hudbnd);
                    LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).Clone(new Rectangle(0, 0, LevelImg8bpp.Width, LevelImg8bpp.Height), PixelFormat.Format32bppArgb);
                    LevelGfx = Graphics.FromImage(LevelBmp);
                    LevelGfx.SetOptions();
                    if (LevelData.LayoutFmt == EngineVersion.S1)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel2.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel2.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                                if (LevelData.FGLoop[x, y])
                                    LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = 3 }, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, LevelData.chunksz, LevelData.chunksz);
                        LevelGfx.DrawImage(LevelData.CompChunkBmps[SelectedChunk],
                        new Rectangle((((pnlcur.X + camera.X) / LevelData.chunksz) * LevelData.chunksz) - camera.X, (((pnlcur.Y + camera.Y) / LevelData.chunksz) * LevelData.chunksz) - camera.Y, LevelData.chunksz, LevelData.chunksz),
                        0, 0, LevelData.chunksz, LevelData.chunksz,
                        GraphicsUnit.Pixel, imageTransparency);
                    Panel2Gfx.DrawImage(LevelBmp, 0, 0, panel2.Width, panel2.Height);
                    break;
                case 2:
                    pnlcur = panel3.PointToClient(Cursor.Position);
                    camera = new Point(hScrollBar3.Value, vScrollBar3.Value);
                    for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel3.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.BGLayout.GetLength(1) - 1); y++)
                    {
                        for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel3.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.BGLayout.GetLength(0) - 1); x++)
                        {
                            if (LevelData.BGLayout[x, y] < LevelData.Chunks.Count)
                            {
                                if (lowToolStripMenuItem.Checked)
                                    LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                if (highToolStripMenuItem.Checked)
                                    LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                if (path1ToolStripMenuItem.Checked)
                                    LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.BGLayout[x, y]][0], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                                else if (path2ToolStripMenuItem.Checked)
                                    LevelImg8bpp.DrawBitmapComposited(LevelData.ChunkColBmpBits[LevelData.BGLayout[x, y]][1], new Point(x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y));
                            }
                        }
                    }
                    if (enableGridToolStripMenuItem.Checked)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel2.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel2.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.FGLayout.GetLength(0) - 1); x++)
                            {
                                LevelImg8bpp.DrawLine(67, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, x * LevelData.chunksz - camera.X + LevelData.chunksz - 1, y * LevelData.chunksz - camera.Y);
                                LevelImg8bpp.DrawLine(67, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y + LevelData.chunksz - 1);
                            }
                    if (hUDToolStripMenuItem.Checked)
                    DrawHUDStr(8, 8,
                        "Screen Pos: " + camera.X.ToString("X4") + ' ' + camera.Y.ToString("X4") + '\n' +
                        "Level Size: " + (LevelData.BGLayout.GetLength(0) * LevelData.chunksz).ToString("X4") + (LevelData.BGLayout.GetLength(1) * LevelData.chunksz).ToString("X4") + '\n' +
                        "Chunk: " + SelectedChunk.ToString("X2")
                        , out hudbnd);
                    LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).Clone(new Rectangle(0, 0, LevelImg8bpp.Width, LevelImg8bpp.Height), PixelFormat.Format32bppArgb);
                    LevelGfx = Graphics.FromImage(LevelBmp);
                    LevelGfx.SetOptions();
                    if (LevelData.LayoutFmt == EngineVersion.S1)
                        for (int y = Math.Max(camera.Y / LevelData.chunksz, 0); y <= Math.Min(((camera.Y + (panel3.Height - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.BGLayout.GetLength(1) - 1); y++)
                            for (int x = Math.Max(camera.X / LevelData.chunksz, 0); x <= Math.Min(((camera.X + (panel3.Width - 1) / ZoomLevel)) / LevelData.chunksz, LevelData.BGLayout.GetLength(0) - 1); x++)
                                if (LevelData.BGLoop[x, y])
                                    LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = 3 }, x * LevelData.chunksz - camera.X, y * LevelData.chunksz - camera.Y, LevelData.chunksz, LevelData.chunksz);
                    LevelGfx.DrawImage(LevelData.CompChunkBmps[SelectedChunk],
                    new Rectangle((((pnlcur.X + camera.X) / LevelData.chunksz) * LevelData.chunksz) - camera.X, (((pnlcur.Y + camera.Y) / LevelData.chunksz) * LevelData.chunksz) - camera.Y, LevelData.chunksz, LevelData.chunksz),
                    0, 0, LevelData.chunksz, LevelData.chunksz,
                    GraphicsUnit.Pixel, imageTransparency);
                    Panel3Gfx.DrawImage(LevelBmp, 0, 0, panel3.Width, panel3.Height);
                    break;
            }
        }

        public void DrawHUDStr(int X, int Y, string str, out Rectangle bounds)
        {
            BitmapBits curimg;
            int curX = X;
            int curY = Y;
            bounds = new Rectangle();
            bounds.X = X;
            bounds.Y = Y;
            int maxX = X;
            foreach (string line in str.Split(new char[] {'\n'}, StringSplitOptions.None))
            {
                int maxY = 0;
                foreach (char ch in line)
                {
                    curimg = HUDFont[HUDChars.IndexOf(char.ToUpper(ch)) + 1];
                    LevelImg8bpp.DrawBitmapComposited(curimg, new Point(curX, curY));
                    curX += curimg.Width;
                    maxX = Math.Max(maxX, curX);
                    maxY = Math.Max(maxY, curimg.Height);
                }
                curY += maxY;
                curX = X;
            }
            bounds.Height = curY - Y;
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            DrawLevel();
        }

        Rectangle prevbnds;
        FormWindowState prevstate;
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.O:
                    if (e.Control)
                        openToolStripMenuItem_Click(sender, EventArgs.Empty);
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
                                ent1.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                ent1.UpdateSprite();
                                break;
                            case EngineVersion.S2:
                            case EngineVersion.S2NA:
                                S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(ent);
                                ent.SubType = sub;
                                ent.X = (ushort)(rand.Next(w));
                                ent.Y = (ushort)(rand.Next(h));
                                ent.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                ent.UpdateSprite();
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                S3KObjectEntry ent3 = (S3KObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(ent3);
                                ent3.SubType = sub;
                                ent3.X = (ushort)(rand.Next(w));
                                ent3.Y = (ushort)(rand.Next(h));
                                ent3.UpdateSprite();
                                break;
                            case EngineVersion.SCDPC:
                                SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                                LevelData.Objects.Add(entcd);
                                entcd.SubType = sub;
                                entcd.X = (ushort)(rand.Next(w));
                                entcd.Y = (ushort)(rand.Next(h));
                                entcd.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                entcd.UpdateSprite();
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
                            case EngineVersion.S2NA:
                                S2RingEntry ent = new S2RingEntry();
                                ent.X = (ushort)(rand.Next(w));
                                ent.Y = (ushort)(rand.Next(h));
                                ent.Count = (byte)rand.Next(1, 9);
                                ent.Direction = (Direction)rand.Next(2);
                                ent.UpdateSprite();
                                LevelData.Rings.Add(ent);
                                break;
                            case EngineVersion.S3K:
                            case EngineVersion.SKC:
                                S3KRingEntry ent3 = new S3KRingEntry();
                                ent3.X = (ushort)(rand.Next(w));
                                ent3.Y = (ushort)(rand.Next(h));
                                ent3.UpdateSprite();
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
                        saveToolStripMenuItem_Click(sender, EventArgs.Empty);
                    break;
                case Keys.Y:
                    if (!loaded) return;
                    if (e.Control && RedoList.Count > 0) DoRedo(1);
                    break;
                case Keys.Z:
                    if (!loaded) return;
                    if (e.Control && UndoList.Count > 0)
                        DoUndo(1);
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
            }
        }

        private void panel1_KeyDown(object sender, KeyEventArgs e)
        {
            long step = e.Shift ? LevelData.chunksz : 16;
            step = e.Control ? int.MaxValue : step;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!loaded) return;
                    vScrollBar1.Value = (int)Math.Max(vScrollBar1.Value - step, vScrollBar1.Minimum);
                    break;
                case Keys.Down:
                    if (!loaded) return;
                    vScrollBar1.Value = (int)Math.Min(vScrollBar1.Value + step, vScrollBar1.Maximum - LevelData.chunksz + 1);
                    break;
                case Keys.Left:
                    if (!loaded) return;
                    hScrollBar1.Value = (int)Math.Max(hScrollBar1.Value - step, hScrollBar1.Minimum);
                    break;
                case Keys.Right:
                    if (!loaded) return;
                    hScrollBar1.Value = (int)Math.Min(hScrollBar1.Value + step, hScrollBar1.Maximum - LevelData.chunksz + 1);
                    break;
                case Keys.Delete:
                    if (!loaded) return;
                    if (SelectedItems.Count > 0)
                        deleteToolStripMenuItem_Click(sender, EventArgs.Empty);
                    break;
                case Keys.A:
                    if (!loaded) return;
                    for (int i = 0; i < SelectedItems.Count; i++)
                    {
                        if (SelectedItems[i] is S1ObjectEntry)
                        {
                            S1ObjectEntry oi = SelectedItems[i] as S1ObjectEntry;
                            oi.ID = (byte)(oi.ID == 0 ? 0x7F : oi.ID - 1);
                            oi.UpdateSprite();
                        }
                        else if (SelectedItems[i] is SCDObjectEntry)
                        {
                            SCDObjectEntry oi = SelectedItems[i] as SCDObjectEntry;
                            oi.ID = (byte)(oi.ID == 0 ? 0x7F : oi.ID - 1);
                            oi.UpdateSprite();
                        }
                        else if (SelectedItems[i] is ObjectEntry)
                        {
                            ObjectEntry oi = SelectedItems[i] as ObjectEntry;
                            oi.ID = (byte)(oi.ID == 0 ? 255 : oi.ID - 1);
                            oi.UpdateSprite();
                        }
                        else if (SelectedItems[i] is S2RingEntry)
                        {
                            S2RingEntry ri = SelectedItems[i] as S2RingEntry;
                            if (ri.Count == 1)
                            {
                                ri.Direction = (ri.Direction == Direction.Vertical ? Direction.Horizontal : Direction.Vertical);
                                ri.Count = 8;
                            }
                            else
                            {
                                ri.Count -= 1;
                            }
                            ri.UpdateSprite();
                        }
                        else if (SelectedItems[i] is CNZBumperEntry)
                        {
                            CNZBumperEntry ci = SelectedItems[i] as CNZBumperEntry;
                            ci.ID = (ushort)(ci.ID == 0 ? 65535 : ci.ID - 1);
                            ci.UpdateSprite();
                        }
                    }
                    DrawLevel();
                    break;
                case Keys.C:
                    if (!loaded) return;
                    if (e.Control)
                        if (SelectedItems.Count > 0)
                            copyToolStripMenuItem_Click(sender, EventArgs.Empty);
                    break;
                case Keys.S:
                    if (!loaded) return;
                    if (!e.Control)
                    {
                        foreach (Entry item in SelectedItems)
                        {
                            if (item is ObjectEntry)
                            {
                                ObjectEntry oi = item as ObjectEntry;
                                oi.SubType = (byte)(oi.SubType == 0 ? 255 : oi.SubType - 1);
                                oi.UpdateSprite();
                            }
                        }
                        DrawLevel();
                    }
                    break;
                case Keys.T:
                    objectsAboveHighPlaneToolStripMenuItem.Checked = !objectsAboveHighPlaneToolStripMenuItem.Checked;
                    DrawLevel();
                    break;
                case Keys.V:
                    if (!loaded) return;
                    if (e.Control)
                    {
                        menuLoc = new Point(panel1.Width / 2, panel1.Height / 2);
                        pasteToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    break;
                case Keys.X:
                    if (!loaded) return;
                    if (e.Control)
                    {
                        if (SelectedItems.Count > 0)
                            cutToolStripMenuItem_Click(sender, EventArgs.Empty);
                    }
                    else
                    {
                        foreach (Entry item in SelectedItems)
                        {
                            if (item is ObjectEntry)
                            {
                                ObjectEntry oi = item as ObjectEntry;
                                oi.SubType = (byte)(oi.SubType == 255 ? 0 : oi.SubType + 1);
                                oi.UpdateSprite();
                            }
                        }
                        DrawLevel();
                    }
                    break;
                case Keys.Z:
                    if (!loaded) return;
                    if (!e.Control)
                    {
                        for (int i = 0; i < SelectedItems.Count; i++)
                        {
                            if (SelectedItems[i] is S1ObjectEntry)
                            {
                                S1ObjectEntry oi = SelectedItems[i] as S1ObjectEntry;
                                oi.ID = (byte)(oi.ID == 0x7F ? 0 : oi.ID + 1);
                                oi.UpdateSprite();
                            }
                            else if (SelectedItems[i] is SCDObjectEntry)
                            {
                                SCDObjectEntry oi = SelectedItems[i] as SCDObjectEntry;
                                oi.ID = (byte)(oi.ID == 0x7F ? 0 : oi.ID + 1);
                                oi.UpdateSprite();
                            }
                            else if (SelectedItems[i] is ObjectEntry)
                            {
                                ObjectEntry oi = SelectedItems[i] as ObjectEntry;
                                oi.ID = (byte)(oi.ID == 255 ? 0 : oi.ID + 1);
                                oi.UpdateSprite();
                            }
                            else if (SelectedItems[i] is S2RingEntry)
                            {
                                S2RingEntry ri = SelectedItems[i] as S2RingEntry;
                                if (ri.Count == 8)
                                {
                                    ri.Direction = (ri.Direction == Direction.Vertical ? Direction.Horizontal : Direction.Vertical);
                                    ri.Count = 1;
                                }
                                else
                                {
                                    ri.Count += 1;
                                }
                                ri.UpdateSprite();
                            }
                            else if (SelectedItems[i] is CNZBumperEntry)
                            {
                                CNZBumperEntry ci = SelectedItems[i] as CNZBumperEntry;
                                ci.ID = (ushort)(ci.ID == 0 ? 65535 : ci.ID - 1);
                                ci.UpdateSprite();
                            }
                        }
                        DrawLevel();
                    }
                    break;
                case Keys.Oemplus:
                    using (ResizeLevelDialog dg = new ResizeLevelDialog(true))
                    {
                        bool canResize;
                        switch (LevelData.LayoutFmt)
                        {
                            case EngineVersion.S1:
                            case EngineVersion.S2NA:
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
                                dg.levelWidth.Maximum = 200;
                                dg.levelHeight.Minimum = 1;
                                dg.levelHeight.Maximum = 32;
                                break;
                            default:
                                canResize = false;
                                break;
                        }
                        if (canResize)
                        {
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
                                loaded = false;
                                hScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel1.Width, 0);
                                vScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel1.Height, 0);
                                hScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel2.Width, 0);
                                vScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel2.Height, 0);
                                hScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel3.Width, 0);
                                vScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel3.Height, 0);
                                loaded = true;
                                DrawLevel();
                            }
                        }
                        else
                            MessageBox.Show("The current game does not allow you to resize levels!");
                    }
                    break;
            }
        }

        private void panel2_KeyDown(object sender, KeyEventArgs e)
        {
            long step = e.Shift ? LevelData.chunksz : 16;
            step = e.Control ? int.MaxValue : step;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!loaded) return;
                    vScrollBar2.Value = (int)Math.Max(vScrollBar2.Value - step, vScrollBar2.Minimum);
                    break;
                case Keys.Down:
                    if (!loaded) return;
                    vScrollBar2.Value = (int)Math.Min(vScrollBar2.Value + step, vScrollBar2.Maximum - LevelData.chunksz + 1);
                    break;
                case Keys.Left:
                    if (!loaded) return;
                    hScrollBar2.Value = (int)Math.Max(hScrollBar2.Value - step, hScrollBar2.Minimum);
                    break;
                case Keys.Right:
                    if (!loaded) return;
                    hScrollBar2.Value = (int)Math.Min(hScrollBar2.Value + step, hScrollBar2.Maximum - LevelData.chunksz + 1);
                    break;
                case Keys.A:
                    if (!loaded) return;
                    SelectedChunk = (byte)(SelectedChunk == 0 ? LevelData.Chunks.Count - 1 : SelectedChunk - 1);
                    DrawLevel();
                    break;
                case Keys.T:
                    objectsAboveHighPlaneToolStripMenuItem.Checked = !objectsAboveHighPlaneToolStripMenuItem.Checked;
                    DrawLevel();
                    break;
                case Keys.Z:
                    if (!loaded) return;
                    if (!e.Control)
                    {
                        SelectedChunk = (byte)(SelectedChunk == LevelData.Chunks.Count - 1 ? 0 : SelectedChunk + 1);
                        DrawLevel();
                    }
                    break;
                case Keys.Oemplus:
                    using (ResizeLevelDialog dg = new ResizeLevelDialog(true))
                    {
                        bool canResize;
                        switch (LevelData.LayoutFmt)
                        {
                            case EngineVersion.S1:
                            case EngineVersion.S2NA:
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
                                dg.levelWidth.Maximum = 200;
                                dg.levelHeight.Minimum = 1;
                                dg.levelHeight.Maximum = 32;
                                break;
                            default:
                                canResize = false;
                                break;
                        }
                        if (canResize)
                        {
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
                                loaded = false;
                                hScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel1.Width, 0);
                                vScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel1.Height, 0);
                                hScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel2.Width, 0);
                                vScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel2.Height, 0);
                                hScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel3.Width, 0);
                                vScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel3.Height, 0);
                                loaded = true;
                                DrawLevel();
                            }
                            else
                                MessageBox.Show("The current game does not allow you to resize levels!");
                        }
                    }
                    break;
            }
        }

        private void panel3_KeyDown(object sender, KeyEventArgs e)
        {
            long step = e.Shift ? LevelData.chunksz : 16;
            step = e.Control ? int.MaxValue : step;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!loaded) return;
                    vScrollBar3.Value = (int)Math.Max(vScrollBar3.Value - step, vScrollBar3.Minimum);
                    break;
                case Keys.Down:
                    if (!loaded) return;
                    vScrollBar3.Value = (int)Math.Min(vScrollBar3.Value + step, vScrollBar3.Maximum - LevelData.chunksz + 1);
                    break;
                case Keys.Left:
                    if (!loaded) return;
                    hScrollBar3.Value = (int)Math.Max(hScrollBar3.Value - step, hScrollBar3.Minimum);
                    break;
                case Keys.Right:
                    if (!loaded) return;
                    hScrollBar3.Value = (int)Math.Min(hScrollBar3.Value + step, hScrollBar3.Maximum - LevelData.chunksz + 1);
                    break;
                case Keys.A:
                    if (!loaded) return;
                    SelectedChunk = (byte)(SelectedChunk == 0 ? LevelData.Chunks.Count - 1 : SelectedChunk - 1);
                    DrawLevel();
                    break;
                case Keys.Z:
                    if (!loaded) return;
                    if (!e.Control)
                    {
                        SelectedChunk = (byte)(SelectedChunk == LevelData.Chunks.Count - 1 ? 0 : SelectedChunk + 1);
                        DrawLevel();
                    }
                    break;
                case Keys.Oemplus:
                    using (ResizeLevelDialog dg = new ResizeLevelDialog(false))
                    {
                        bool canResize;
                        switch (LevelData.LayoutFmt)
                        {
                            case EngineVersion.S1:
                            case EngineVersion.S2NA:
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
                                dg.levelWidth.Maximum = 200;
                                dg.levelHeight.Minimum = 1;
                                dg.levelHeight.Maximum = 32;
                                break;
                            default:
                                canResize = false;
                                break;
                        }
                        if (canResize)
                        {
                            dg.levelWidth.Value = LevelData.BGLayout.GetLength(0);
                            dg.levelHeight.Value = LevelData.BGLayout.GetLength(1);
                            if (dg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                            {
                                byte[,] newBG = new byte[(int)dg.levelWidth.Value, (int)dg.levelHeight.Value];
                                bool[,] newBGLoop = LevelData.LayoutFmt == EngineVersion.S1 ? new bool[(int)dg.levelWidth.Value, (int)dg.levelHeight.Value] : null;
                                for (int y = 0; y < Math.Min(dg.levelHeight.Value, LevelData.BGLayout.GetLength(1)); y++)
                                {
                                    for (int x = 0; x < Math.Min(dg.levelWidth.Value, LevelData.BGLayout.GetLength(0)); x++)
                                    {
                                        newBG[x, y] = LevelData.BGLayout[x, y];
                                        if (LevelData.LayoutFmt == EngineVersion.S1)
                                            newBGLoop[x, y] = LevelData.BGLoop[x, y];
                                    }
                                }
                                LevelData.BGLayout = newBG;
                                LevelData.BGLoop = newBGLoop;
                                loaded = false;
                                hScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel1.Width, 0);
                                vScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel1.Height, 0);
                                hScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel2.Width, 0);
                                vScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel2.Height, 0);
                                hScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel3.Width, 0);
                                vScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel3.Height, 0);
                                loaded = true;
                                DrawLevel();
                            }
                            else
                                MessageBox.Show("The current game does not allow you to resize levels!");
                        }
                    }
                    break;
            }
        }

        bool objdrag = false;
        bool selecting = false;
        Point selpoint;
        List<Point> locs = new List<Point>();
        List<byte> tiles = new List<byte>();
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            int curx = (int)(e.X / ZoomLevel) + hScrollBar1.Value;
            int cury = (int)(e.Y / ZoomLevel) + vScrollBar1.Value;
            switch (e.Button)
            {
                case MouseButtons.Left:
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
                                        case EngineVersion.S2NA:
                                            S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                                            LevelData.Objects.Add(ent);
                                            ent.SubType = sub;
                                            ent.X = (ushort)(curx);
                                            ent.Y = (ushort)(e.Y /  ZoomLevel + vScrollBar1.Value);
                                            ent.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                            ent.UpdateSprite();
                                            SelectedItems.Clear();
                                            SelectedItems.Add(ent);
                                            SelectedObjectChanged();
                                            AddUndo(new ObjectAddedUndoAction(ent));
                                            break;
                                        case EngineVersion.S1:
                                            S1ObjectEntry ent1 = (S1ObjectEntry)LevelData.CreateObject(ID);
                                            LevelData.Objects.Add(ent1);
                                            ent1.SubType = sub;
                                            ent1.X = (ushort)(curx);
                                            ent1.Y = (ushort)(cury);
                                            ent1.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                            ent1.UpdateSprite();
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
                                            ent3.X = (ushort)(curx);
                                            ent3.Y = (ushort)(cury);
                                            ent3.UpdateSprite();
                                            SelectedItems.Clear();
                                            SelectedItems.Add(ent3);
                                            SelectedObjectChanged();
                                            AddUndo(new ObjectAddedUndoAction(ent3));
                                            break;
                                        case EngineVersion.SCD:
                                        case EngineVersion.SCDPC:
                                            SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                                            LevelData.Objects.Add(entcd);
                                            entcd.SubType = sub;
                                            entcd.X = (ushort)(curx);
                                            entcd.Y = (ushort)(cury);
                                            entcd.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                            switch (LevelData.TimeZone)
                                            {
                                                case TimeZone.Past:
                                                    entcd.ShowPast = true;
                                                    break;
                                                case TimeZone.Present:
                                                    entcd.ShowPresent = true;
                                                    break;
                                                case TimeZone.Future:
                                                    entcd.ShowFuture = true;
                                                    break;
                                            }
                                            entcd.UpdateSprite();
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
                                LevelData.Bumpers.Add(new CNZBumperEntry() { X = (ushort)(curx), Y = (ushort)(cury) });
                                LevelData.Bumpers[LevelData.Bumpers.Count - 1].UpdateSprite();
                                SelectedItems.Clear();
                                SelectedItems.Add(LevelData.Bumpers[LevelData.Bumpers.Count - 1]);
                                SelectedObjectChanged();
                                AddUndo(new ObjectAddedUndoAction(LevelData.Bumpers[LevelData.Bumpers.Count - 1]));
                                LevelData.Bumpers.Sort();
                                DrawLevel();
                            }
                        }
                        else if (LevelData.RingFmt == EngineVersion.S2 | LevelData.RingFmt == EngineVersion.S2NA)
                        {
                            LevelData.Rings.Add(new S2RingEntry() { X = (ushort)(curx), Y = (ushort)(cury) });
                            LevelData.Rings[LevelData.Rings.Count - 1].UpdateSprite();
                            SelectedItems.Clear();
                            SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                            SelectedObjectChanged();
                            AddUndo(new ObjectAddedUndoAction(LevelData.Rings[LevelData.Rings.Count - 1]));
                            LevelData.Rings.Sort();
                            DrawLevel();
                        }
                        else if (LevelData.RingFmt == EngineVersion.S3K | LevelData.RingFmt == EngineVersion.SKC)
                        {
                            LevelData.Rings.Add(new S3KRingEntry() { X = (ushort)(curx), Y = (ushort)(cury) });
                            LevelData.Rings[LevelData.Rings.Count - 1].UpdateSprite();
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
                        ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
                        Rectangle bound = dat.Bounds(item, Point.Empty);
                        if (ObjectVisible(item) && bound.Contains(curx, cury))
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
                                Rectangle bound = LevelData.S2RingDef.Bounds(item, Point.Empty);
                                if (bound.Contains(curx, cury))
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
                                Rectangle bound = LevelData.S3KRingDef.Bounds(item, Point.Empty);
                                if (bound.Contains(curx, cury))
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
                            Rectangle bound = LevelData.unkobj.Bounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
                            if (bound.Contains(curx, cury))
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
                            Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(item, Point.Empty);
                            if (bound.Contains(curx, cury))
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
                        selpoint = new Point(curx, cury);
                        SelectedItems.Clear();
                        SelectedObjectChanged();
                    }
                    else
                    {
                        locs = new List<Point>();
                        foreach (Entry item in SelectedItems)
                            locs.Add(new Point(item.X, item.Y));
                    }
                    break;
                case MouseButtons.Right:
                    menuLoc = e.Location;
                    foreach (ObjectEntry item in LevelData.Objects)
                    {
                        ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
                        Rectangle bound = dat.Bounds(item, Point.Empty);
                        if (ObjectVisible(item) && bound.Contains(curx, cury))
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
                                Rectangle bound = LevelData.S2RingDef.Bounds(item, Point.Empty);
                                if (bound.Contains(curx, cury))
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
                                Rectangle bound = LevelData.S3KRingDef.Bounds(item, Point.Empty);
                                if (bound.Contains(curx, cury))
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
                            Rectangle bound = LevelData.unkobj.Bounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
                            if (bound.Contains(curx, cury))
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
                            Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(item, Point.Empty);
                            if (bound.Contains(curx, cury))
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
            }
        }

        Point lastchunkpoint;
        Point lastmouse;
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            Rectangle bnd = panel1.Bounds;
            bnd.Offset(-panel1.Location.X, -panel1.Location.Y);
            if (!bnd.Contains(panel1.PointToClient(Cursor.Position))) return;
            Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar1.Value, (int)(e.Y / ZoomLevel) + vScrollBar1.Value);
            bool redraw = false;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (objdrag)
                    {
                        foreach (Entry item in SelectedItems)
                        {
                            item.X = (ushort)(item.X + (mouse.X - lastmouse.X));
                            item.Y = (ushort)(item.Y + (mouse.Y - lastmouse.Y));
                            item.UpdateSprite();
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
                            ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
                            Rectangle bound = dat.Bounds(item, Point.Empty);
                            if (ObjectVisible(item) && bound.IntersectsWith(selbnds))
                                SelectedItems.Add(item);
                        }
                        foreach (RingEntry ritem in LevelData.Rings)
                        {
                            if (ritem is S2RingEntry)
                            {
                                S2RingEntry item = ritem as S2RingEntry;
                                Rectangle bound = LevelData.S2RingDef.Bounds(item, Point.Empty);
                                if (bound.IntersectsWith(selbnds))
                                    SelectedItems.Add(item);
                            }
                            else if (ritem is S3KRingEntry)
                            {
                                S3KRingEntry item = ritem as S3KRingEntry;
                                Rectangle bound = LevelData.S3KRingDef.Bounds(item, Point.Empty);
                                if (bound.IntersectsWith(selbnds))
                                    SelectedItems.Add(item);
                            }
                        }
                        if (LevelData.Bumpers != null)
                            foreach (CNZBumperEntry item in LevelData.Bumpers)
                            {
                                Rectangle bound = LevelData.unkobj.Bounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
                                if (bound.IntersectsWith(selbnds))
                                    SelectedItems.Add(item);
                            }
                        foreach (StartPositionEntry item in LevelData.StartPositions)
                        {
                            Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(item, Point.Empty);
                            if (bound.IntersectsWith(selbnds))
                                SelectedItems.Add(item);
                        }
                        if (selobjs != SelectedItems.Count) SelectedObjectChanged();
                        redraw = true;
                    }
                    break;
            }
            Cursor cur = Cursors.Default;
            foreach (ObjectEntry item in LevelData.Objects)
            {
                ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
                Rectangle bound = dat.Bounds(item, Point.Empty);
                if (ObjectVisible(item) && bound.Contains(mouse))
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
                    case EngineVersion.S2NA:
                        Rectangle bound = LevelData.S2RingDef.Bounds((S2RingEntry)item, Point.Empty);
                        if (bound.Contains(mouse))
                        {
                            cur = Cursors.SizeAll;
                            break;
                        }
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        bound = LevelData.S3KRingDef.Bounds((S3KRingEntry)item, Point.Empty);
                        if (bound.Contains(mouse))
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
                    Rectangle bound = LevelData.unkobj.Bounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
                    if (bound.Contains(mouse))
                    {
                        cur = Cursors.SizeAll;
                        break;
                    }
                }
            foreach (StartPositionEntry item in LevelData.StartPositions)
            {
                Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].Bounds(item, Point.Empty);
                if (bound.Contains(mouse))
                {
                    cur = Cursors.SizeAll;
                    break;
                }
            }
            panel1.Cursor = cur;
            if (redraw) DrawLevel();
            lastmouse = mouse;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            Point chunkpoint = new Point(((int)(e.X / ZoomLevel) + hScrollBar2.Value) / LevelData.chunksz, ((int)(e.Y / ZoomLevel) + vScrollBar2.Value) / LevelData.chunksz);
            if (chunkpoint.X >= LevelData.FGLayout.GetLength(0) | chunkpoint.Y >= LevelData.FGLayout.GetLength(1)) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (LevelData.LayoutFmt == EngineVersion.S1 && e.Clicks >= 2)
                        LevelData.FGLoop[chunkpoint.X, chunkpoint.Y] = !LevelData.FGLoop[chunkpoint.X, chunkpoint.Y];
                    else
                    {
                        locs = new List<Point>();
                        tiles = new List<byte>();
                        byte t = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                        if (t != SelectedChunk)
                        {
                            locs.Add(chunkpoint);
                            tiles.Add(t);
                            LevelData.FGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
                            DrawLevel();
                        }
                    }
                    break;
                case MouseButtons.Right:
                    SelectedChunk = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    DrawLevel();
                    break;
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            Rectangle bnd = panel2.Bounds;
            bnd.Offset(-panel2.Location.X, -panel2.Location.Y);
            if (!bnd.Contains(panel2.PointToClient(Cursor.Position))) return;
            Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar2.Value, (int)(e.Y / ZoomLevel) + vScrollBar2.Value);
            Point chunkpoint = new Point(mouse.X / LevelData.chunksz, mouse.Y / LevelData.chunksz);
            if (chunkpoint.X >= LevelData.FGLayout.GetLength(0) | chunkpoint.Y >= LevelData.FGLayout.GetLength(1)) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    byte t = LevelData.FGLayout[chunkpoint.X, chunkpoint.Y];
                    if (t != SelectedChunk)
                    {
                        locs.Add(chunkpoint);
                        tiles.Add(t);
                        LevelData.FGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
                    }
                    break;
            }
            if (chunkpoint != lastchunkpoint) DrawLevel();
            lastchunkpoint = chunkpoint;
            lastmouse = mouse;
        }

        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            Point chunkpoint = new Point(((int)(e.X / ZoomLevel) + hScrollBar3.Value) / LevelData.chunksz, ((int)(e.Y / ZoomLevel) + vScrollBar3.Value) / LevelData.chunksz);
            if (chunkpoint.X >= LevelData.BGLayout.GetLength(0) | chunkpoint.Y >= LevelData.BGLayout.GetLength(1)) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (LevelData.LayoutFmt == EngineVersion.S1 && e.Clicks >= 2)
                        LevelData.BGLoop[chunkpoint.X, chunkpoint.Y] = !LevelData.BGLoop[chunkpoint.X, chunkpoint.Y];
                    else
                    {
                        locs = new List<Point>();
                        tiles = new List<byte>();
                        byte tb = LevelData.BGLayout[chunkpoint.X, chunkpoint.Y];
                        if (tb != SelectedChunk)
                        {
                            locs.Add(chunkpoint);
                            tiles.Add(tb);
                            LevelData.BGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
                            DrawLevel();
                        }
                    }
                    break;
                case MouseButtons.Right:
                    SelectedChunk = LevelData.BGLayout[chunkpoint.X, chunkpoint.Y];
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    DrawLevel();
                    break;
            }
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            Rectangle bnd = panel3.Bounds;
            bnd.Offset(-panel3.Location.X, -panel3.Location.Y);
            if (!bnd.Contains(panel3.PointToClient(Cursor.Position))) return;
            Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar3.Value, (int)(e.Y / ZoomLevel) + vScrollBar3.Value);
            Point chunkpoint = new Point(mouse.X / LevelData.chunksz, mouse.Y / LevelData.chunksz);
            if (chunkpoint.X >= LevelData.BGLayout.GetLength(0) | chunkpoint.Y >= LevelData.BGLayout.GetLength(1)) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    byte tb = LevelData.BGLayout[chunkpoint.X, chunkpoint.Y];
                    if (tb != SelectedChunk)
                    {
                        locs.Add(chunkpoint);
                        tiles.Add(tb);
                        LevelData.BGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
                    }
                    break;
            }
            if (chunkpoint != lastchunkpoint) DrawLevel();
            lastchunkpoint = chunkpoint;
            lastmouse = mouse;
        }

        private void ChunkSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            if (ChunkSelector.SelectedIndex == -1) return;
            SelectedChunk = (byte)ChunkSelector.SelectedIndex;
            SelectedChunkBlock = new Point();
            ChunkBlockPropertyGrid.SelectedObject = LevelData.Chunks[SelectedChunk].blocks[0, 0];
            ChunkPicture.Invalidate();
            ChunkID.Text = SelectedChunk.ToString("X2");
            ChunkCount.Text = LevelData.Chunks.Count.ToString("X") + " / 100";
            DrawLevel();
        }

        private void blocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory };
            if (a.ShowDialog() == DialogResult.OK)
            {
                Color[] palette = new Color[256];
                for (int i = 0; i < 64; i++)
                    palette[i] = LevelData.PaletteToColor(i / 16, i % 16, false);
                for (int i = 64; i < 256; i++)
                    palette[i] = Color.Black;
                palette[0] = LevelData.PaletteToColor(2, 0, false);
                for (int i = 0; i < LevelData.Blocks.Count; i++)
                    LevelData.CompBlockBmpBits[i].ToBitmap(palette).Save(System.IO.Path.Combine(a.SelectedPath, i + ".png"));
            }
        }

        private void chunksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory };
            if (a.ShowDialog() == DialogResult.OK)
            {
                Color[] palette = new Color[256];
                for (int i = 0; i < 64; i++)
                    palette[i] = LevelData.PaletteToColor(i / 16, i % 16, false);
                for (int i = 64; i < 256; i++)
                    palette[i] = Color.Black;
                palette[0] = LevelData.PaletteToColor(2, 0, false);
                for (int i = 0; i < LevelData.Chunks.Count; i++)
                    LevelData.CompChunkBmpBits[i].ToBitmap(palette).Save(System.IO.Path.Combine(a.SelectedPath, i + ".png"));
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
                    for (int x = 0; x < xend; x++)
                        if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                        {
                            if (lowToolStripMenuItem.Checked)
                                bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][0], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                            if (objectsAboveHighPlaneToolStripMenuItem.Checked)
                            {
                                if (highToolStripMenuItem.Checked)
                                    bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                            }
                        }
                if (includeobjectsWithFGToolStripMenuItem.Checked)
                {
                    for (int oi = 0; oi < LevelData.Objects.Count; oi++)
                    {
                        ObjectDefinition od = LevelData.GetObjectDefinition(LevelData.Objects[oi].ID);
                        if (ObjectVisible(LevelData.Objects[oi]))
                        {
                            bool draw = true;
                            if (hideDebugObjectsToolStripMenuItem.Checked)
                                draw = !od.Debug;
                            if (draw)
                            {
                                Sprite spr = LevelData.Objects[oi].Sprite;
                                bmp.DrawBitmapComposited(spr.Image, spr.Offset);
                            }
                        }
                    }
                    switch (LevelData.RingFmt)
                    {
                        case EngineVersion.S2:
                        case EngineVersion.S2NA:
                            for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                            {
                                S2RingEntry re = (S2RingEntry)LevelData.Rings[ri];
                                bool draw = true;
                                if (hideDebugObjectsToolStripMenuItem.Checked)
                                    draw = !LevelData.S2RingDef.Debug;
                                if (draw)
                                {
                                    Sprite spr = re.Sprite;
                                    bmp.DrawBitmapComposited(spr.Image, spr.Offset);
                                }
                            }
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            for (int ri = 0; ri < LevelData.Rings.Count; ri++)
                            {
                                S3KRingEntry re = (S3KRingEntry)LevelData.Rings[ri];
                                bool draw = true;
                                if (hideDebugObjectsToolStripMenuItem.Checked)
                                    draw = !LevelData.S3KRingDef.Debug;
                                if (draw)
                                {
                                    Sprite spr = re.Sprite;
                                    bmp.DrawBitmapComposited(spr.Image, spr.Offset);
                                }
                            }
                            break;
                    }
                    for (int si = 0; si < LevelData.StartPositions.Count; si++)
                    {
                        bool draw = true;
                        if (hideDebugObjectsToolStripMenuItem.Checked)
                            draw = !LevelData.StartPosDefs[si].Debug;
                        if (draw)
                        {
                            Sprite spr = LevelData.StartPositions[si].Sprite;
                            bmp.DrawBitmapComposited(spr.Image, spr.Offset);
                        }
                    }
                }
                if (!objectsAboveHighPlaneToolStripMenuItem.Checked)
                    for (int y = 0; y < yend; y++)
                        for (int x = 0; x < xend; x++)
                            if (LevelData.FGLayout[x, y] < LevelData.Chunks.Count)
                            {
                                if (highToolStripMenuItem.Checked)
                                    bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.FGLayout[x, y]][1], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
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
                pal.Entries[0] = LevelData.PaletteToColor(2, 0, transparentBackFGBGToolStripMenuItem.Checked);
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
                    for (int x = 0; x < xend; x++)
                        if (LevelData.BGLayout[x, y] < LevelData.Chunks.Count)
                        {
                            if (lowToolStripMenuItem.Checked)
                                bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][0], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
                            if (highToolStripMenuItem.Checked)
                                bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[LevelData.BGLayout[x, y]][1], new Point(x * LevelData.chunksz, y * LevelData.chunksz));
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
                pal.Entries[0] = LevelData.PaletteToColor(2, 0, transparentBackFGBGToolStripMenuItem.Checked);
                res.Palette = pal;
                res.Save(a.FileName);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (objdrag)
            {
                if (ModifierKeys == Keys.Shift)
                {
                    foreach (Entry item in SelectedItems)
                    {
                        item.X = (ushort)(Math.Round(item.X / 8.0, MidpointRounding.AwayFromZero) * 8);
                        item.Y = (ushort)(Math.Round(item.Y / 8.0, MidpointRounding.AwayFromZero) * 8);
                        item.UpdateSprite();
                    }
                }
                bool moved = false;
                for (int i = 0; i < SelectedItems.Count; i++)
                    if (SelectedItems[i].X != locs[i].X | SelectedItems[i].Y != locs[i].Y)
                        moved = true;
                if (moved)
                    AddUndo(new ObjectMoveUndoAction(new List<Entry>(SelectedItems), locs));
                ObjectProperties.SelectedObjects = SelectedItems.ToArray();
                LevelData.Objects.Sort();
                LevelData.Rings.Sort();
                if (LevelData.Bumpers != null) LevelData.Bumpers.Sort();
            }
            objdrag = false;
            selecting = false;
            DrawLevel();
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            if (locs.Count > 0) AddUndo(new LayoutEditUndoAction(1, locs, tiles));
            DrawLevel();
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            if (locs.Count > 0) AddUndo(new LayoutEditUndoAction(2, locs, tiles));
            DrawLevel();
        }

        private void SelectedObjectChanged()
        {
            ObjectProperties.SelectedObjects = SelectedItems.ToArray();
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
            Properties.Settings.Default.ShowHUD = hUDToolStripMenuItem.Checked;
            Properties.Settings.Default.ShowGrid = enableGridToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
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
                        case EngineVersion.S2NA:
                            string ty = group.Value["codetype"];
                            string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                            DateTime modDate = DateTime.MinValue;
                            if (System.IO.File.Exists(dllfile))
                                modDate = System.IO.File.GetLastWriteTime(dllfile);
                            string fp = group.Value["codefile"].Replace('/', System.IO.Path.DirectorySeparatorChar);
                            Log("Loading S2RingDefinition type " + ty + " from \"" + fp + "\"...");
                            if (modDate >= File.GetLastWriteTime(fp) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
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
#if false
                                    case ".js":
                                        pr = new Microsoft.JScript.JScriptCodeProvider();
                                        break;
#endif
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
                        if (modDate >= File.GetLastWriteTime(fp) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
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
#if false
                                case ".js":
                                    pr = new Microsoft.JScript.JScriptCodeProvider();
                                    break;
#endif
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
                    else if (group.Value.ContainsKey("xmlfile"))
                    {
                        XMLDef.ObjDef xdef = XMLDef.ObjDef.Load(group.Value["xmlfile"]);
                        string ty = xdef.Namespace + "." + xdef.TypeName;
                        string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                        DateTime modDate = DateTime.MinValue;
                        if (System.IO.File.Exists(dllfile))
                            modDate = System.IO.File.GetLastWriteTime(dllfile);
                        Log("Loading ObjectDefinition type " + ty + " from \"" + group.Value["xmlfile"] + "\"...");
                        if (modDate >= File.GetLastWriteTime(group.Value["xmlfile"]) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
                        {
                            Log("Loading type from cached assembly \"" + dllfile + "\"...");
                            def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                        }
                        else
                        {
                            Log("Building code file...");
                            Type basetype;
                            switch (LevelData.ObjectFmt)
                            {
                                case EngineVersion.S1:
                                    basetype = typeof(S1ObjectEntry);
                                    break;
                                case EngineVersion.S2:
                                case EngineVersion.S2NA:
                                    basetype = typeof(S2ObjectEntry);
                                    break;
                                case EngineVersion.S3K:
                                case EngineVersion.SKC:
                                    basetype = typeof(S3KObjectEntry);
                                    break;
                                case EngineVersion.SCD:
                                case EngineVersion.SCDPC:
                                    basetype = typeof(SCDObjectEntry);
                                    break;
                                default:
                                    basetype = typeof(ObjectEntry);
                                    break;
                            }
                            CodeTypeReferenceExpression objhelprefex = new CodeTypeReferenceExpression(typeof(ObjectHelper));
                            CodeThisReferenceExpression thisref = new CodeThisReferenceExpression();
                            List<CodeTypeMember> members = new List<CodeTypeMember>();
                            CodeMemberMethod method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Init";
                            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Dictionary<string, string>), "data"));
                            method.ReturnType = new CodeTypeReference(typeof(void));
                            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(MultiFileIndexer<byte>), "artfiles", new CodePrimitiveExpression(null)));
                            members.Add(new CodeMemberField(typeof(Sprite), "unkimg") { Attributes = MemberAttributes.Private });
                            method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, "unkimg"), new CodePropertyReferenceExpression(objhelprefex, "UnknownObject")));
                            if (xdef.Images != null & xdef.Images.Items != null)
                            {
                                foreach (object item in xdef.Images.Items)
                                {
                                    if (item is XMLDef.ImageFromBitmap)
                                    {
                                        XMLDef.ImageFromBitmap img = (XMLDef.ImageFromBitmap)item;
                                        members.Add(new CodeMemberField(typeof(Sprite), img.id) { Attributes = MemberAttributes.Private });
                                        ;
                                        Point pnt = Point.Empty;
                                        if (img.offset != null)
                                            pnt = img.offset.ToPoint();
                                        method.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("off"), new CodeObjectCreateExpression(typeof(Point), new CodePrimitiveExpression(pnt.X), new CodePrimitiveExpression(pnt.Y))));
                                        method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeObjectCreateExpression(typeof(Sprite),
                                            new CodeObjectCreateExpression(typeof(BitmapBits), new CodeObjectCreateExpression(typeof(Bitmap), new CodePrimitiveExpression(img.filename))),
                                            new CodeObjectCreateExpression(typeof(Point), new CodePrimitiveExpression(pnt.X), new CodePrimitiveExpression(pnt.Y)))));
                                    }
                                    else if (item is XMLDef.ImageFromMappings)
                                    {
                                        XMLDef.ImageFromMappings img = (XMLDef.ImageFromMappings)item;
                                        members.Add(new CodeMemberField(typeof(Sprite), img.id) { Attributes = MemberAttributes.Private });
                                        method.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("artfiles"), new CodeObjectCreateExpression(typeof(MultiFileIndexer<byte>))));
                                        foreach (XMLDef.ArtFile artfile in img.ArtFiles)
                                        {
                                            method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"),
                                                "AddFile", new CodeObjectCreateExpression(typeof(List<byte>), new CodeMethodInvokeExpression(objhelprefex, "OpenArtFile",
                                                    new CodePrimitiveExpression(artfile.filename),
                                                    new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Compression.CompressionType)), artfile.compression.ToString()))),
                                                    new CodePrimitiveExpression(artfile.offsetSpecified ? artfile.offset : -1)));
                                        }
                                        if (img.mappings is XMLDef.MapFileBin)
                                        {
                                            XMLDef.MapFileBin map = (XMLDef.MapFileBin)img.mappings;
                                            if (string.IsNullOrEmpty(map.dplcfile))
                                                method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapToBmp",
                                                    new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(File)), "ReadAllBytes", new CodePrimitiveExpression(map.filename)),
                                                    new CodePrimitiveExpression(map.frame), new CodePrimitiveExpression(map.startpal))));
                                            else
                                                method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapDPLCToBmp",
                                                    new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(File)), "ReadAllBytes", new CodePrimitiveExpression(map.filename)),
                                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(File)), "ReadAllBytes", new CodePrimitiveExpression(map.dplcfile)),
                                                    new CodePrimitiveExpression(map.dplcver == EngineVersion.Invalid ? LevelData.EngineVersion : map.dplcver),
                                                    new CodePrimitiveExpression(map.frame), new CodePrimitiveExpression(map.startpal))));
                                        }
                                        else if (img.mappings is XMLDef.MapFileAsm)
                                        {
                                            XMLDef.MapFileAsm map = (XMLDef.MapFileAsm)img.mappings;
                                            if (map.frameSpecified)
                                            {
                                                if (string.IsNullOrEmpty(map.dplcfile))
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.frame),
                                                        new CodePrimitiveExpression(map.startpal))));
                                                else
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapDPLCASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.dplcfile),
                                                        new CodePrimitiveExpression(map.dplcver == EngineVersion.Invalid ? LevelData.EngineVersion : map.dplcver),
                                                        new CodePrimitiveExpression(map.frame), new CodePrimitiveExpression(map.startpal))));
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(map.dplcfile))
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.label),
                                                        new CodePrimitiveExpression(map.startpal))));
                                                else
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapDPLCASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.label),
                                                        new CodePrimitiveExpression(map.dplcfile), new CodePrimitiveExpression(map.dplclabel),
                                                        new CodePrimitiveExpression(map.dplcver == EngineVersion.Invalid ? LevelData.EngineVersion : map.dplcver),
                                                        new CodePrimitiveExpression(map.startpal))));
                                            }
                                        }
                                        if (img.offset != null)
                                        {
                                            Point pnt = img.offset.ToPoint();
                                            method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "X"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.X)), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Y"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.Y)))));
                                        }
                                    }
                                    else if (item is XMLDef.ImageFromSprite)
                                    {
                                        XMLDef.ImageFromSprite img = (XMLDef.ImageFromSprite)item;
                                        members.Add(new CodeMemberField(typeof(Sprite), img.id) { Attributes = MemberAttributes.Private });
                                        method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "GetSprite", new CodePrimitiveExpression(img.frame))));
                                        if (img.offset != null)
                                        {
                                            Point pnt = img.offset.ToPoint();
                                            method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "X"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.X)), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Y"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.Y)))));
                                        }
                                    }
                                }
                            }
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Name";
                            method.ReturnType = new CodeTypeReference(typeof(string));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(xdef.Name)));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Subtypes";
                            method.ReturnType = new CodeTypeReference(typeof(ReadOnlyCollection<byte>));
                            List<CodeExpression> subtypeexprs = new List<CodeExpression>();
                            if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    subtypeexprs.Add(new CodePrimitiveExpression(item.subtype));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(ReadOnlyCollection<byte>), new CodeArrayCreateExpression(typeof(byte), subtypeexprs.ToArray()))));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "SubtypeName";
                            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte), "subtype"));
                            method.ReturnType = new CodeTypeReference(typeof(string));
                            if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("subtype"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)), new CodeMethodReturnStatement(new CodePrimitiveExpression(item.name))));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(string.Empty)));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Image";
                            method.ReturnType = new CodeTypeReference(typeof(BitmapBits));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, string.IsNullOrEmpty(xdef.Image) ? "unkimg" : xdef.Image), "Image")));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Image";
                            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte), "subtype"));
                            method.ReturnType = new CodeTypeReference(typeof(BitmapBits));
                            if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("subtype"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)), new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, string.IsNullOrEmpty(item.image) ? "unkimg" : item.image), "Image"))));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(thisref, "Image")));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "GetSprite";
                            method.Parameters.AddRange(new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(ObjectEntry), "obj") });
                            method.ReturnType = new CodeTypeReference(typeof(Sprite));
                            if (xdef.Display != null && !string.IsNullOrEmpty(xdef.Display.SpriteRoutine))
                                method.Statements.Add(new CodeSnippetStatement(xdef.Display.SpriteRoutine));
                            else if (xdef.Display != null && xdef.Display.DisplayOptions != null && xdef.Display.DisplayOptions.Length > 0)
                            {
                                Dictionary<string, string> props = new Dictionary<string, string>();
                                if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                                    foreach (object item in xdef.Properties.Items)
                                        if (item is XMLDef.BitsProperty)
                                        {
                                            XMLDef.BitsProperty bp = (XMLDef.BitsProperty)item;
                                            props.Add(bp.name, bp.type);
                                        }
                                        else
                                        {
                                            XMLDef.CustomProperty cp = (XMLDef.CustomProperty)item;
                                            props.Add(cp.name, cp.type);
                                        }
                                if (props.Count > 0)
                                    method.Statements.Add(new CodeVariableDeclarationStatement(xdef.TypeName + basetype.Name, "obj2", new CodeCastExpression(xdef.TypeName + basetype.Name, new CodeArgumentReferenceExpression("obj"))));
                                else
                                    method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                List<string> enums = new List<string>();
                                if (xdef.Enums != null && xdef.Enums.Items != null && xdef.Enums.Items.Length > 0)
                                    foreach (XMLDef.Enum item in xdef.Enums.Items)
                                        enums.Add(item.name);
                                foreach (XMLDef.DisplayOption opt in xdef.Display.DisplayOptions)
                                {
                                    CodeExpression condlist = null;
                                    if (opt.Conditions != null && opt.Conditions.Length > 0)
                                    {
                                        foreach (XMLDef.Condition item in opt.Conditions)
                                        {
                                            CodeBinaryOperatorExpression cond = new CodeBinaryOperatorExpression(null, CodeBinaryOperatorType.IdentityEquality, null);
                                            cond.Left = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), item.property);
                                            if (props.ContainsKey(item.property))
                                            {
                                                if (enums.Contains(props[item.property]))
                                                    cond.Right = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(props[item.property]), item.value);
                                                else
                                                    switch (props[item.property])
                                                    {
                                                        case "bool":
                                                            cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                            break;
                                                        case "byte":
                                                            cond.Right = new CodePrimitiveExpression(byte.Parse(item.value));
                                                            break;
                                                        case "int":
                                                            cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                                            break;
                                                        default:
                                                            cond.Right = new CodePrimitiveExpression(item.value);
                                                            break;
                                                    }
                                            }
                                            else
                                            {
                                                Type t = basetype.GetProperty(item.property).PropertyType;
                                                if (t == typeof(bool))
                                                    cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                if (t == typeof(byte) || t == typeof(ushort))
                                                    cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                            }
                                            if (condlist == null)
                                                condlist = cond;
                                            else
                                                condlist = new CodeBinaryOperatorExpression(condlist, CodeBinaryOperatorType.BooleanAnd, cond);
                                        }
                                    }
                                    else
                                        condlist = new CodePrimitiveExpression(true);
                                    CodeConditionStatement ifstatement = new CodeConditionStatement(condlist);
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "bits"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "xoff"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "yoff"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(List<Sprite>), "sprs", new CodeObjectCreateExpression(typeof(List<Sprite>))));
                                    if (opt.Images != null)
                                        foreach (XMLDef.ImageRef img in opt.Images)
                                        {
                                            int xoff = img.Offset != null ? img.Offset.X : 0;
                                            int yoff = img.Offset != null ? img.Offset.Y : 0;
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodePrimitiveExpression(img.xflip ? -xoff : xoff)));
                                            if (!img.xflipSpecified)
                                                ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("xoff")))));
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodePrimitiveExpression(img.yflip ? -yoff : yoff)));
                                            if (!img.yflipSpecified)
                                                ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("yoff")))));
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("bits"), new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"))));
                                            ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePrimitiveExpression(img.xflip), new CodePrimitiveExpression(img.yflip)));
                                            if (!img.xflipSpecified)
                                                if (!img.yflipSpecified)
                                                    ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip")));
                                                else
                                                    ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePrimitiveExpression(false)));
                                            else if (!img.yflipSpecified)
                                                ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePrimitiveExpression(false), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip")));
                                            ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sprs"), "Add", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("bits"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "X"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("xoff")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Y"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("yoff"))))));
                                        }
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "spr", new CodeObjectCreateExpression(typeof(Sprite), new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sprs"), "ToArray"))));
                                    ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Y")))));
                                    ifstatement.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("spr")));
                                    method.Statements.Add(ifstatement);
                                }
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "unkbits", new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"))));
                                method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("unkbits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"))));
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "unkspr", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("unkbits"), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"))));
                                method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Y")))));
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("unkspr")));
                            }
                            else
                            {
                                method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                {
                                    foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    {
                                        CodeConditionStatement ifstatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "SubType"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)));
                                        ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "bits", new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Image"))));
                                        ifstatement.TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"))));
                                        ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "spr", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("bits"), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Offset"))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Y")))));
                                        ifstatement.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("spr")));
                                        method.Statements.Add(ifstatement);
                                    }
                                }
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "unkbits", new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"))));
                                method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("unkbits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"))));
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "unkspr", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("unkbits"), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"))));
                                method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Y")))));
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("unkspr")));
                            }
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Bounds";
                            method.Parameters.AddRange(new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(ObjectEntry), "obj"), new CodeParameterDeclarationExpression(typeof(Point), "camera") });
                            method.ReturnType = new CodeTypeReference(typeof(Rectangle));
                            if (xdef.Display != null && !string.IsNullOrEmpty(xdef.Display.BoundsRoutine))
                                method.Statements.Add(new CodeSnippetStatement(xdef.Display.BoundsRoutine));
                            else if (xdef.Display != null && xdef.Display.DisplayOptions != null && xdef.Display.DisplayOptions.Length > 0)
                            {
                                Dictionary<string, string> props = new Dictionary<string, string>();
                                if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                                    foreach (object item in xdef.Properties.Items)
                                        if (item is XMLDef.BitsProperty)
                                        {
                                            XMLDef.BitsProperty bp = (XMLDef.BitsProperty)item;
                                            props.Add(bp.name, bp.type);
                                        }
                                        else
                                        {
                                            XMLDef.CustomProperty cp = (XMLDef.CustomProperty)item;
                                            props.Add(cp.name, cp.type);
                                        }
                                if (props.Count > 0)
                                    method.Statements.Add(new CodeVariableDeclarationStatement(xdef.TypeName + basetype.Name, "obj2", new CodeCastExpression(xdef.TypeName + basetype.Name, new CodeArgumentReferenceExpression("obj"))));
                                else
                                    method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                List<string> enums = new List<string>();
                                if (xdef.Enums != null && xdef.Enums.Items != null && xdef.Enums.Items.Length > 0)
                                    foreach (XMLDef.Enum item in xdef.Enums.Items)
                                        enums.Add(item.name);
                                foreach (XMLDef.DisplayOption opt in xdef.Display.DisplayOptions)
                                {
                                    CodeExpression condlist = null;
                                    if (opt.Conditions != null && opt.Conditions.Length > 0)
                                    {
                                        foreach (XMLDef.Condition item in opt.Conditions)
                                        {
                                            CodeBinaryOperatorExpression cond = new CodeBinaryOperatorExpression(null, CodeBinaryOperatorType.IdentityEquality, null);
                                            cond.Left = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), item.property);
                                            if (props.ContainsKey(item.property))
                                            {
                                                if (enums.Contains(props[item.property]))
                                                    cond.Right = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(props[item.property]), item.value);
                                                else
                                                    switch (props[item.property])
                                                    {
                                                        case "bool":
                                                            cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                            break;
                                                        case "byte":
                                                            cond.Right = new CodePrimitiveExpression(byte.Parse(item.value));
                                                            break;
                                                        case "int":
                                                            cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                                            break;
                                                        default:
                                                            cond.Right = new CodePrimitiveExpression(item.value);
                                                            break;
                                                    }
                                            }
                                            else
                                            {
                                                Type t = basetype.GetProperty(item.property).PropertyType;
                                                if (t == typeof(bool))
                                                    cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                if (t == typeof(byte) || t == typeof(ushort))
                                                    cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                            }
                                            if (condlist == null)
                                                condlist = cond;
                                            else
                                                condlist = new CodeBinaryOperatorExpression(condlist, CodeBinaryOperatorType.BooleanAnd, cond);
                                        }
                                    }
                                    else
                                        condlist = new CodePrimitiveExpression(true);
                                    CodeConditionStatement ifstatement = new CodeConditionStatement(condlist, new CodeVariableDeclarationStatement(typeof(Rectangle), "rect", new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Rectangle)), "Empty")));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "xoff"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "yoff"));
                                    bool first = true;
                                    foreach (XMLDef.ImageRef img in opt.Images)
                                    {
                                        int xoff = img.Offset != null ? img.Offset.X : 0;
                                        int yoff = img.Offset != null ? img.Offset.Y : 0;
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodePrimitiveExpression(img.xflip ? -xoff : xoff)));
                                        if (!img.xflipSpecified)
                                            ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("xoff")))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("xoff"), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X"))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodePrimitiveExpression(img.yflip ? -yoff : yoff)));
                                        if (!img.yflipSpecified)
                                            ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("yoff")))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("yoff"), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y"))));
                                        if (first)
                                        {
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rect"), new CodeObjectCreateExpression(typeof(Rectangle),
                                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "X"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("xoff"))),
                                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "Y"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("yoff"))),
                                            new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Width"),
                                            new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Height"))));
                                            first = false;
                                        }
                                        else
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rect"), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Rectangle)), "Union", new CodeVariableReferenceExpression("rect"), new CodeObjectCreateExpression(typeof(Rectangle),
                                                new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "X"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("xoff"))),
                                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "Y"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("yoff"))),
                                                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Width"),
                                                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Height")))));
                                    }
                                    ifstatement.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("rect")));
                                    method.Statements.Add(ifstatement);
                                }
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(Rectangle), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "X")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X")), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "Y")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y")), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Width"), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Height"))));
                            }
                            else
                            {
                                method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                    foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                        method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "SubType"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)), new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(Rectangle), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Offset"), "X")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X")), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Offset"), "Y")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y")), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Image"), "Width"), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Image"), "Height")))));
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(Rectangle), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "X")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X")), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "Y")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y")), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Width"), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Height"))));
                            }
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "RememberState";
                            method.ReturnType = new CodeTypeReference(typeof(bool));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(xdef.RememberState)));
                            members.Add(method);
                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            prop.Name = "Debug";
                            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(xdef.Debug)));
                            prop.HasGet = true;
                            prop.HasSet = false;
                            prop.Type = new CodeTypeReference(typeof(bool));
                            members.Add(prop);
                            if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                            {
                                prop = new CodeMemberProperty();
                                prop.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                                prop.Name = "ObjectType";
                                prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeTypeOfExpression(xdef.TypeName + basetype.Name)));
                                prop.HasGet = true;
                                prop.HasSet = false;
                                prop.Type = new CodeTypeReference(typeof(Type));
                                members.Add(prop);
                            }
                            CodeTypeDeclaration ctd = new CodeTypeDeclaration(xdef.TypeName);
                            ctd.BaseTypes.Add(typeof(ObjectDefinition));
                            ctd.IsClass = true;
                            ctd.Members.AddRange(members.ToArray());
                            CodeNamespace cn = new CodeNamespace(xdef.Namespace);
                            cn.Imports.Add(new CodeNamespaceImport("System.Drawing"));
                            cn.Types.Add(ctd);
                            if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                            {
                                members = new List<CodeTypeMember>();
                                CodeConstructor ctor = new CodeConstructor();
                                ctor.Attributes = MemberAttributes.Public;
                                ctor.BaseConstructorArgs.Add(new CodeSnippetExpression(string.Empty));
                                members.Add(ctor);
                                ctor = new CodeConstructor();
                                ctor.Attributes = MemberAttributes.Public;
                                ctor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("file"));
                                ctor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("address"));
                                ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte[]), "file"));
                                ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "address"));
                                members.Add(ctor);
                                foreach (object item in xdef.Properties.Items)
                                {
                                    if (item is XMLDef.BitsProperty)
                                    {
                                        XMLDef.BitsProperty bp = (XMLDef.BitsProperty)item;
                                        int mask = 0;
                                        for (int i = 0; i < bp.length; i++)
                                            mask += (int)Math.Pow(2, bp.startbit + i);
                                        prop = new CodeMemberProperty();
                                        prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CategoryAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("Extended"))));
                                        if (!string.IsNullOrEmpty(bp.description))
                                            prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DescriptionAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(bp.description))));
                                        prop.Attributes = MemberAttributes.Public;
                                        prop.Name = bp.name;
                                        if (ExpandTypeName(bp.type) != typeof(bool).FullName)
                                        {
                                            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(bp.type, new CodeMethodInvokeExpression(objhelprefex, "ShiftRight", new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(thisref, "SubType"), CodeBinaryOperatorType.BitwiseAnd, new CodePrimitiveExpression(mask)), new CodePrimitiveExpression(bp.startbit)))));
                                            prop.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(thisref, "SubType"), new CodeMethodInvokeExpression(objhelprefex, "SetSubtypeMask", new CodePropertyReferenceExpression(thisref, "SubType"), new CodeCastExpression(typeof(byte), new CodeMethodInvokeExpression(objhelprefex, "ShiftLeft", new CodeCastExpression(typeof(byte), new CodePropertySetValueReferenceExpression()), new CodePrimitiveExpression(bp.startbit))), new CodePrimitiveExpression(mask))));
                                        }
                                        else
                                        {
                                            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(objhelprefex, "ShiftRight", new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(thisref, "SubType"), CodeBinaryOperatorType.BitwiseAnd, new CodePrimitiveExpression(mask)), new CodePrimitiveExpression(bp.startbit)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0))));
                                            prop.SetStatements.Add(new CodeConditionStatement(new CodePropertySetValueReferenceExpression(),
                                                new CodeStatement[] { new CodeAssignStatement(new CodePropertyReferenceExpression(thisref, "SubType"), new CodeMethodInvokeExpression(objhelprefex, "SetSubtypeMask", new CodePropertyReferenceExpression(thisref, "SubType"), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(mask)), new CodePrimitiveExpression(mask))) },
                                                new CodeStatement[] { new CodeAssignStatement(new CodePropertyReferenceExpression(thisref, "SubType"), new CodeMethodInvokeExpression(objhelprefex, "SetSubtypeMask", new CodePropertyReferenceExpression(thisref, "SubType"), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(0)), new CodePrimitiveExpression(mask))) }));
                                        }
                                        prop.HasGet = true;
                                        prop.HasSet = true;
                                        prop.Type = new CodeTypeReference(ExpandTypeName(bp.type));
                                        members.Add(prop);
                                    }
                                    else
                                    {
                                        XMLDef.CustomProperty cp = (XMLDef.CustomProperty)item;
                                        prop = new CodeMemberProperty();
                                        prop.Attributes = MemberAttributes.Public;
                                        if (cp.@override) prop.Attributes |= MemberAttributes.Override;
                                        prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CategoryAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("Extended"))));
                                        if (!string.IsNullOrEmpty(cp.description))
                                            prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DescriptionAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(cp.description))));
                                        prop.Name = cp.name;
                                        prop.GetStatements.Add(new CodeSnippetStatement(cp.get));
                                        prop.HasGet = true;
                                        prop.HasSet = true;
                                        prop.SetStatements.Add(new CodeSnippetStatement(cp.set));
                                        prop.Type = new CodeTypeReference(ExpandTypeName(cp.type));
                                        members.Add(prop);
                                    }
                                }
                                ctd = new CodeTypeDeclaration(xdef.TypeName + basetype.Name);
                                ctd.Attributes = MemberAttributes.Public;
                                ctd.BaseTypes.Add(basetype);
                                ctd.IsClass = true;
                                ctd.Members.AddRange(members.ToArray());
                                cn.Types.Add(ctd);
                            }
                            if (xdef.Enums != null && xdef.Enums.Items != null)
                            {
                                foreach (XMLDef.Enum item in xdef.Enums.Items)
                                {
                                    ctd = new CodeTypeDeclaration(item.name);
                                    ctd.Attributes = MemberAttributes.Public;
                                    ctd.BaseTypes.Add(typeof(int));
                                    ctd.IsEnum = true;
                                    foreach (XMLDef.EnumMember mem in item.Items)
                                    {
                                        CodeMemberField mf = new CodeMemberField(typeof(int), mem.name);
                                        if (mem.valueSpecified)
                                            mf.InitExpression = new CodePrimitiveExpression(mem.value);
                                        ctd.Members.Add(mf);
                                    }
                                    cn.Types.Add(ctd);
                                }
                            }
                            CodeCompileUnit ccu = new CodeCompileUnit();
                            ccu.Namespaces.Add(cn);
                            ccu.ReferencedAssemblies.AddRange(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                            Log("Compiling code file...");
                            CodeDomProvider pr = null;
                            switch (xdef.Language.ToLowerInvariant())
                            {
                                case "cs":
                                    pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
                                case "vb":
                                    pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
#if false
                                case "js":
                                    pr = new Microsoft.JScript.JScriptCodeProvider();
                                    break;
#endif
                            }
                            if (pr != null)
                            {
#if DEBUG
                                StreamWriter sw = new StreamWriter(xdef.Namespace + "." + xdef.TypeName + "." + pr.FileExtension);
                                pr.GenerateCodeFromCompileUnit(ccu, sw, new CodeGeneratorOptions() { BlankLinesBetweenMembers = true, BracingStyle = "C", VerbatimOrder = true });
                                sw.Close();
#endif
                                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                                para.GenerateExecutable = false;
                                para.GenerateInMemory = false;
                                para.IncludeDebugInformation = true;
                                para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
                                CompilerResults res = pr.CompileAssemblyFromDom(para, ccu);
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

        private string ExpandTypeName(string type)
        {
            switch (type)
            {
                case "bool":
                    return typeof(bool).FullName;
                case "byte":
                    return typeof(byte).FullName;
                case "char":
                    return typeof(char).FullName;
                case "decimal":
                    return typeof(decimal).FullName;
                case "double":
                    return typeof(double).FullName;
                case "float":
                    return typeof(float).FullName;
                case "int":
                    return typeof(int).FullName;
                case "long":
                    return typeof(long).FullName;
                case "object":
                    return typeof(object).FullName;
                case "sbyte":
                    return typeof(sbyte).FullName;
                case "short":
                    return typeof(short).FullName;
                case "string":
                    return typeof(string).FullName;
                case "uint":
                    return typeof(uint).FullName;
                case "ulong":
                    return typeof(ulong).FullName;
                case "ushort":
                    return typeof(ushort).FullName;
                default:
                    return type;
            }
        }

        private void tilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            exportToolStripMenuItem.DropDown.Hide();
            FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory };
            if (a.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < LevelData.Tiles.Count; i++)
                {
                    LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, tilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)).Save(System.IO.Path.Combine(a.SelectedPath, i + ".png"));
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
            loaded = false;
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    hScrollBar2.Value = Math.Min(hScrollBar1.Value, hScrollBar2.Maximum);
                    vScrollBar2.Value = Math.Min(vScrollBar1.Value, vScrollBar2.Maximum);
                    break;
                case 1:
                    hScrollBar1.Value = Math.Min(hScrollBar2.Value, hScrollBar1.Maximum);
                    vScrollBar1.Value = Math.Min(vScrollBar2.Value, vScrollBar1.Maximum);
                    break;
            }
            loaded = true;
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

        private void panel_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    LevelImg8bpp = new BitmapBits((int)(panel1.Width / ZoomLevel), (int)(panel1.Height / ZoomLevel));
                    break;
                case 1:
                    LevelImg8bpp = new BitmapBits((int)(panel2.Width / ZoomLevel), (int)(panel2.Height / ZoomLevel));
                    break;
                case 2:
                    LevelImg8bpp = new BitmapBits((int)(panel3.Width / ZoomLevel), (int)(panel3.Height / ZoomLevel));
                    break;
                default:
                    LevelImg8bpp = new BitmapBits(1, 1);
                    break;
            }
            Panel1Gfx = panel1.CreateGraphics();
            Panel1Gfx.SetOptions();
            Panel2Gfx = panel2.CreateGraphics();
            Panel2Gfx.SetOptions();
            Panel3Gfx = panel3.CreateGraphics();
            Panel3Gfx.SetOptions();
            loaded = false;
            hScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel1.Width, 0);
            vScrollBar1.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel1.Height, 0);
            hScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel2.Width, 0);
            vScrollBar2.Maximum = Math.Max(((LevelData.FGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel2.Height, 0);
            hScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(0) + 1) * LevelData.chunksz) - panel3.Width, 0);
            vScrollBar3.Maximum = Math.Max(((LevelData.BGLayout.GetLength(1) + 1) * LevelData.chunksz) - panel3.Height, 0);
            loaded = true;
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
                        ent1.X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value);
                        ent1.Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value);
                        ent1.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                        ent1.UpdateSprite();
                        SelectedItems.Clear();
                        SelectedItems.Add(ent1);
                        SelectedObjectChanged();
                        AddUndo(new ObjectAddedUndoAction(ent1));
                        break;
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                        S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                        LevelData.Objects.Add(ent);
                        ent.SubType = sub;
                        ent.X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value);
                        ent.Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value);
                        ent.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                        ent.UpdateSprite();
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
                        ent3.X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value);
                        ent3.Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value);
                        ent3.UpdateSprite();
                        SelectedItems.Clear();
                        SelectedItems.Add(ent3);
                        SelectedObjectChanged();
                        AddUndo(new ObjectAddedUndoAction(ent3));
                        break;
                    case EngineVersion.SCDPC:
                        SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                        LevelData.Objects.Add(entcd);
                        entcd.SubType = sub;
                        entcd.X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value);
                        entcd.Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value);
                        entcd.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                        switch (LevelData.TimeZone)
                        {
                            case TimeZone.Present:
                                entcd.ShowPresent = true;
                                break;
                            case TimeZone.Past:
                                entcd.ShowPast = true;
                                break;
                            case TimeZone.Future:
                                entcd.ShowFuture = true;
                                break;
                        }
                        entcd.UpdateSprite();
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
                    LevelData.Objects.Add(new S1ObjectEntry() { X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value), Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value), ID = 0x25 });
                    LevelData.Objects[LevelData.Objects.Count - 1].UpdateSprite();
                    SelectedItems.Clear();
                    SelectedItems.Add(LevelData.Objects[LevelData.Objects.Count - 1]);
                    SelectedObjectChanged();
                    AddUndo(new ObjectAddedUndoAction(LevelData.Objects[LevelData.Objects.Count - 1]));
                    LevelData.Objects.Sort();
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    LevelData.Rings.Add(new S2RingEntry() { X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value), Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value) });
                    LevelData.Rings[LevelData.Rings.Count - 1].UpdateSprite();
                    SelectedItems.Clear();
                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                    SelectedObjectChanged();
                    AddUndo(new ObjectAddedUndoAction(LevelData.Rings[LevelData.Rings.Count - 1]));
                    LevelData.Rings.Sort();
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    LevelData.Rings.Add(new S3KRingEntry() { X = (ushort)((menuLoc.X * ZoomLevel) + hScrollBar1.Value), Y = (ushort)((menuLoc.Y * ZoomLevel) + vScrollBar1.Value) });
                    LevelData.Rings[LevelData.Rings.Count - 1].UpdateSprite();
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
                    dlg.XDist.Value = LevelData.GetObjectDefinition(ID).Bounds(new S2ObjectEntry() { SubType = sub }, Point.Empty).Width;
                    dlg.YDist.Value = LevelData.GetObjectDefinition(ID).Bounds(new S2ObjectEntry() { SubType = sub }, Point.Empty).Height;
                    if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        Point pt = new Point((int)(menuLoc.X * ZoomLevel) + hScrollBar1.Value, (int)(menuLoc.Y * ZoomLevel) + vScrollBar1.Value);
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
                                        ent1.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                        ent1.UpdateSprite();
                                        SelectedItems.Add(ent1);
                                        break;
                                    case EngineVersion.S2:
                                    case EngineVersion.S2NA:
                                        S2ObjectEntry ent = (S2ObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(ent);
                                        ent.SubType = sub;
                                        ent.X = (ushort)(pt.X);
                                        ent.Y = (ushort)(pt.Y);
                                        ent.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                        ent.UpdateSprite();
                                        SelectedItems.Add(ent);
                                        break;
                                    case EngineVersion.S3K:
                                    case EngineVersion.SKC:
                                        S3KObjectEntry ent3 = (S3KObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(ent3);
                                        ent3.SubType = sub;
                                        ent3.X = (ushort)(pt.X);
                                        ent3.Y = (ushort)(pt.Y);
                                        ent3.UpdateSprite();
                                        SelectedItems.Add(ent3);
                                        break;
                                    case EngineVersion.SCDPC:
                                        SCDObjectEntry entcd = (SCDObjectEntry)LevelData.CreateObject(ID);
                                        LevelData.Objects.Add(entcd);
                                        entcd.SubType = sub;
                                        entcd.X = (ushort)(pt.X);
                                        entcd.Y = (ushort)(pt.Y);
                                        entcd.RememberState = LevelData.GetObjectDefinition(ID).RememberState();
                                        switch (LevelData.TimeZone)
                                        {
                                            case TimeZone.Present:
                                                entcd.ShowPresent = true;
                                                break;
                                            case TimeZone.Past:
                                                entcd.ShowPast = true;
                                                break;
                                            case TimeZone.Future:
                                                entcd.ShowFuture = true;
                                                break;
                                        }
                                        entcd.UpdateSprite();
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
                    case EngineVersion.S2NA:
                        dlg.XDist.Value = LevelData.S2RingDef.Bounds(new S2RingEntry(), Point.Empty).Width;
                        dlg.YDist.Value = LevelData.S2RingDef.Bounds(new S2RingEntry(), Point.Empty).Height;
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        dlg.XDist.Value = LevelData.S3KRingDef.Bounds(new S3KRingEntry(), Point.Empty).Width;
                        dlg.YDist.Value = LevelData.S3KRingDef.Bounds(new S3KRingEntry(), Point.Empty).Height;
                        break;
                    default:
                        return;
                }
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    Point pt = new Point((int)(menuLoc.X * ZoomLevel) + hScrollBar1.Value, (int)(menuLoc.Y * ZoomLevel) + vScrollBar1.Value);
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
                                case EngineVersion.S2NA:
                                    LevelData.Rings.Add(new S2RingEntry() { X = (ushort)(pt.X), Y = (ushort)(pt.Y) });
                                    LevelData.Rings[LevelData.Rings.Count - 1].UpdateSprite();
                                    SelectedItems.Add(LevelData.Rings[LevelData.Rings.Count - 1]);
                                    break;
                                case EngineVersion.S3K:
                                case EngineVersion.SKC:
                                    LevelData.Rings.Add(new S3KRingEntry() { X = (ushort)(pt.X), Y = (ushort)(pt.Y) });
                                    LevelData.Rings[LevelData.Rings.Count - 1].UpdateSprite();
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
            Size off = new Size((menuLoc.X + hScrollBar1.Value) - upleft.X, (menuLoc.Y + vScrollBar1.Value) - upleft.Y);
            SelectedItems = new List<Entry>(objs);
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
                item.UpdateSprite();
            }
            AddUndo(new ObjectsPastedUndoAction(new List<Entry>(SelectedItems)));
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
            string romfile = ini[string.Empty].GetValueOrDefault("romfile", null);
            if (romfile == null)
                romfile = ini[string.Empty].GetValueOrDefault("runcmd", null);
            if (bool.Parse(ini[string.Empty].GetValueOrDefault("useemu", "true")))
                if (!string.IsNullOrEmpty(Properties.Settings.Default.Emulator))
                    System.Diagnostics.Process.Start(Properties.Settings.Default.Emulator, '"' + System.IO.Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["romfile"]) + '"');
                else
                    MessageBox.Show("You must set up an emulator before you can run the ROM, use File -> Setup Emulator.");
            else
                System.Diagnostics.Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["romfile"]));
        }

        private void currentOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentOnlyToolStripMenuItem.Checked = true;
            allToolStripMenuItem.Checked = false;
            DrawLevel();
        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allToolStripMenuItem.Checked = true;
            currentOnlyToolStripMenuItem.Checked = false;
            DrawLevel();
        }

        private bool ObjectVisible(ObjectEntry obj)
        {
            if (allToolStripMenuItem.Checked)
                return true;
            if (obj is SCDObjectEntry)
            {
                SCDObjectEntry scdobj = (SCDObjectEntry)obj;
                switch (LevelData.TimeZone)
                {
                    case TimeZone.Past:
                        return scdobj.ShowPast;
                    case TimeZone.Present:
                        return scdobj.ShowPresent;
                    case TimeZone.Future:
                        return scdobj.ShowFuture;
                    default:
                        return true;
                }
            }
            return true;
        }

        private void lowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawLevel();
        }

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawLevel();
        }

        private void setupEmulatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog opn = new OpenFileDialog() { DefaultExt = "exe", Filter = "EXE Files|*.exe|All Files|*.*", RestoreDirectory = true })
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.Emulator))
                {
                    opn.FileName = Path.GetFileName(Properties.Settings.Default.Emulator);
                    opn.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.Emulator);
                }
                if (opn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.Emulator = opn.FileName;
                    setupEmulatorToolStripMenuItem.Checked = true;
                }
            }
        }

        private void panel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    LevelImg8bpp = new BitmapBits((int)(panel1.Width / ZoomLevel), (int)(panel1.Height / ZoomLevel));
                    panel1.Focus();
                    break;
                case 1:
                    LevelImg8bpp = new BitmapBits((int)(panel2.Width / ZoomLevel), (int)(panel2.Height / ZoomLevel));
                    splitContainer2.Panel2.Controls.Add(ChunkSelector);
                    panel2.Focus();
                    break;
                case 2:
                    LevelImg8bpp = new BitmapBits((int)(panel3.Width / ZoomLevel), (int)(panel3.Height / ZoomLevel));
                    splitContainer3.Panel2.Controls.Add(ChunkSelector);
                    panel3.Focus();
                    break;
                case 3:
                    panel10.Controls.Add(ChunkSelector);
                    break;
            }
            DrawLevel();
        }

        private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            loaded = false;
            LoadINI(Properties.Settings.Default.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
        }

        public int SelectedBlock, SelectedTile;
        public Point SelectedChunkBlock, SelectedBlockTile, SelectedColor;

        private void ChunkPicture_MouseClick(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            SelectedChunkBlock = new Point(e.X / 16, e.Y / 16);
            ChunkBlockPropertyGrid.SelectedObject = LevelData.Chunks[SelectedChunk].blocks[e.X / 16, e.Y / 16];
            ChunkPicture.Invalidate();
        }

        private void ChunkBlockPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            LevelData.RedrawChunk(SelectedChunk);
            DrawLevel();
            ChunkPicture.Invalidate();
        }

        private void ChunkPicture_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;
            e.Graphics.Clear(LevelData.PaletteToColor(2, 0, false));
            if (lowToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ChunkBmps[SelectedChunk][0], 0, 0, LevelData.chunksz, LevelData.chunksz);
            if (highToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ChunkBmps[SelectedChunk][1], 0, 0, LevelData.chunksz, LevelData.chunksz);
            if (path1ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ChunkColBmps[SelectedChunk][0], 0, 0, LevelData.chunksz, LevelData.chunksz);
            if (path2ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ChunkColBmps[SelectedChunk][1], 0, 0, LevelData.chunksz, LevelData.chunksz);
            e.Graphics.DrawRectangle(Pens.White, SelectedChunkBlock.X * 16 - 1, SelectedChunkBlock.Y * 16 - 1, 18, 18);
        }

        private void BlockPicture_MouseClick(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            SelectedBlockTile = new Point(e.X / 32, e.Y / 32);
            BlockTilePropertyGrid.SelectedObject = LevelData.Blocks[SelectedBlock].tiles[e.X / 32, e.Y / 32];
            BlockPicture.Invalidate();
        }

        private void BlockTilePropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            LevelData.RedrawBlock(SelectedBlock, true);
            DrawLevel();
            BlockPicture.Invalidate();
        }

        private void BlockSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BlockSelector.SelectedIndex > -1)
            {
                SelectedBlock = BlockSelector.SelectedIndex;
                SelectedBlockTile = new Point();
                BlockTilePropertyGrid.SelectedObject = LevelData.Blocks[SelectedBlock].tiles[0, 0];
                BlockCollision1.Value = LevelData.ColInds1[SelectedBlock];
                BlockCollision2.Value = LevelData.ColInds2[SelectedBlock];
                BlockID.Text = SelectedBlock.ToString("X3");
                int blockmax = 0x400;
                switch (LevelData.EngineVersion)
                {
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                        blockmax = 0x340;
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        blockmax = 0x300;
                        break;
                }
                BlockCount.Text = LevelData.Blocks.Count.ToString("X") + " / " + blockmax.ToString("X");
                BlockPicture.Invalidate();
            }
        }

        private void BlockPicture_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.Clear(LevelData.PaletteToColor(2, 0, false));
            if (lowToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.BlockBmpBits[SelectedBlock][0].Scale(4).ToBitmap(LevelData.BmpPal), 0, 0, 64, 64);
            if (highToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.BlockBmpBits[SelectedBlock][1].Scale(4).ToBitmap(LevelData.BmpPal), 0, 0, 64, 64);
            if (path1ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ColBmps[LevelData.ColInds1[SelectedBlock]], 0, 0, 64, 64);
            if (path2ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ColBmps[LevelData.ColInds2[SelectedBlock]], 0, 0, 64, 64);
            e.Graphics.DrawRectangle(Pens.White, SelectedBlockTile.X * 32, SelectedBlockTile.Y * 32, 31, 31);
        }

        private void PalettePanel_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;
            e.Graphics.Clear(Color.Black);
            for (int y = 0; y <= 3; y++)
                for (int x = 0; x <= 15; x++)
                {
                    e.Graphics.FillRectangle(new SolidBrush(LevelData.PaletteToColor(y, x, false)), x * 32, y * 32, 32, 32);
                    e.Graphics.DrawRectangle(Pens.White, x * 32, y * 32, 31, 31);
                }
            e.Graphics.DrawRectangle(new Pen(Color.Yellow, 2), SelectedColor.X * 32, SelectedColor.Y * 32, 32, 32);
        }

        int[] cols;
        private void PalettePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            int line = e.Y / 32;
            int index = e.X / 32;
            ColorDialog a = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                FullOpen = true,
                SolidColorOnly = true,
                Color = LevelData.PaletteToColor(line, index, false)
            };
            if (cols != null)
                a.CustomColors = cols;
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LevelData.ColorToPalette(line, index, a.Color);
                PalettePanel.Invalidate();
                LevelData.PaletteChanged();
                ChunkSelector.Invalidate();
                ChunkPicture.Invalidate();
                BlockSelector.Invalidate();
                BlockPicture.Invalidate();
                TileSelector.Invalidate();
                TilePicture.Invalidate();
            }
            cols = a.CustomColors;
        }

        private void BlockCollision1_ValueChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            LevelData.ColInds1[SelectedBlock] = (byte)BlockCollision1.Value;
            if (Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                BlockCollision2.Value = BlockCollision1.Value;
        }

        private void BlockCollision2_ValueChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            LevelData.ColInds2[SelectedBlock] = (byte)BlockCollision2.Value;
            if (Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                BlockCollision1.Value = BlockCollision2.Value;
        }

        private Color[] curpal;
        private void PalettePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            bool newpal = e.Y / 32 != SelectedColor.Y;
            SelectedColor = new Point(e.X / 32, e.Y / 32);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip2.Show(PalettePanel, e.Location);
            }
            PalettePanel.Invalidate();
            if (newpal)
            {
                curpal = new Color[16];
                for (int i = 0; i < 16; i++)
                    curpal[i] = LevelData.PaletteToColor(SelectedColor.Y, i, false);
            }
            TilePicture.Invalidate();
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog a = new OpenFileDialog())
            {
                a.DefaultExt = "bin";
                a.Filter = "MD Palettes|*.bin|Image Files|*.bmp;*.png;*.jpg;*.gif";
                a.RestoreDirectory = true;
                if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    int l = SelectedColor.Y;
                    int x = SelectedColor.X;
                    switch (System.IO.Path.GetExtension(a.FileName))
                    {
                        case ".bin":
                            byte[] file = System.IO.File.ReadAllBytes(a.FileName);
                            for (int i = 0; i < file.Length; i += 2)
                            {
                                LevelData.Palette[LevelData.CurPal][l, x] = BitConverter.ToUInt16(file, i);
                                x++;
                                if (x == 16)
                                {
                                    x = 0;
                                    l++;
                                    if (l == 4)
                                        break;
                                }
                            }
                            break;
                        case ".bmp":
                        case ".png":
                        case ".jpg":
                        case ".gif":
                            Bitmap bmp = new Bitmap(a.FileName);
                            if ((bmp.PixelFormat & System.Drawing.Imaging.PixelFormat.Indexed) == System.Drawing.Imaging.PixelFormat.Indexed)
                            {
                                Color[] pal = bmp.Palette.Entries;
                                for (int i = 0; i < pal.Length; i++)
                                {
                                    LevelData.ColorToPalette(l, x, pal[i]);
                                    x++;
                                    if (x == 16)
                                    {
                                        x = 0;
                                        l++;
                                        if (l == 4)
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            LevelData.PaletteChanged();
            ChunkSelector.Invalidate();
            ChunkPicture.Invalidate();
            BlockSelector.Invalidate();
            BlockPicture.Invalidate();
            TileSelector.Invalidate();
            TilePicture.Invalidate();
        }

        private BitmapBits tile;
        private void TileSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TileSelector.SelectedIndex > -1)
            {
                SelectedTile = TileSelector.SelectedIndex;
                tile = BitmapBits.FromTile(LevelData.Tiles[SelectedTile], 0);
                TileID.Text = SelectedTile.ToString("X3");
                TileCount.Text = LevelData.Tiles.Count.ToString("X") + " / 800";
                TilePicture.Invalidate();
            }
        }

        private void TilePicture_Paint(object sender, PaintEventArgs e)
        {
            if (TileSelector.SelectedIndex == -1) return;
            e.Graphics.SetOptions();
            e.Graphics.DrawImage(tile.Scale(16).ToBitmap(curpal), 0, 0, 128, 128);
        }

        private void TilePicture_MouseDown(object sender, MouseEventArgs e)
        {
            if (TileSelector.SelectedIndex == -1) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                tile.Bits[((e.Y / 16) * 8) + (e.X / 16)] = (byte)SelectedColor.X;
                TilePicture.Invalidate();
            }
        }

        private void TilePicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (TileSelector.SelectedIndex == -1) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left && new Rectangle(Point.Empty, TilePicture.Size).Contains(e.Location))
            {
                tile.Bits[((e.Y / 16) * 8) + (e.X / 16)] = (byte)SelectedColor.X;
                TilePicture.Invalidate();
            }
        }

        private void TilePicture_MouseUp(object sender, MouseEventArgs e)
        {
            if (TileSelector.SelectedIndex == -1) return;
            LevelData.Tiles[SelectedTile] = tile.ToTile();
            LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
            for (int i = 0; i < LevelData.Blocks.Count; i++)
            {
                bool dr = false;
                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 2; x++)
                        if (LevelData.Blocks[i].tiles[x, y].Tile == SelectedTile)
                            dr = true;
                if (dr)
                    LevelData.RedrawBlock(i, true);
            }
            TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2);
        }

        private void ChunkSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            if (tabControl1.SelectedIndex == 3 & e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                pasteBeforeToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLChunk") & LevelData.Chunks.Count < 256;
                pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
                importToolStripMenuItem.Enabled = LevelData.Chunks.Count < 256;
                drawToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertAfterToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertBeforeToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                tileContextMenuStrip.Show(ChunkSelector, e.Location);
            }
        }

        private void BlockSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int blockmax = 0x400;
                switch (LevelData.EngineVersion)
                {
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                        blockmax = 0x340;
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        blockmax = 0x300;
                        break;
                }
                pasteBeforeToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLBlock") & LevelData.Blocks.Count < blockmax;
                pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
                importToolStripMenuItem.Enabled = LevelData.Blocks.Count < blockmax;
                drawToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertAfterToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertBeforeToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                tileContextMenuStrip.Show(BlockSelector, e.Location);
            }
        }

        private void TileSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                pasteBeforeToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLTile") & LevelData.Tiles.Count < 0x800;
                pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
                importToolStripMenuItem.Enabled = LevelData.Tiles.Count < 0x800;
                drawToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertAfterToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertBeforeToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                tileContextMenuStrip.Show(TileSelector, e.Location);
            }
        }

        private void cutTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    Clipboard.SetData("SonLVLChunk", LevelData.Chunks[SelectedChunk].GetBytes());
                    LevelData.Chunks.RemoveAt(SelectedChunk);
                    LevelData.ChunkBmpBits.RemoveAt(SelectedChunk);
                    LevelData.ChunkBmps.RemoveAt(SelectedChunk);
                    LevelData.ChunkColBmpBits.RemoveAt(SelectedChunk);
                    LevelData.ChunkColBmps.RemoveAt(SelectedChunk);
                    LevelData.CompChunkBmps.RemoveAt(SelectedChunk);
                    LevelData.CompChunkBmpBits.RemoveAt(SelectedChunk);
                    SelectedChunk = (byte)Math.Min(SelectedChunk, LevelData.Chunks.Count - 1);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] > SelectedChunk)
                                LevelData.FGLayout[x, y]--;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] > SelectedChunk)
                                LevelData.BGLayout[x, y]--;
                    ChunkSelector.SelectedIndex = Math.Min(ChunkSelector.SelectedIndex, LevelData.Chunks.Count - 1);
                    break;
                case 4: // Blocks
                    Clipboard.SetData("SonLVLBlock", LevelData.Blocks[SelectedBlock].GetBytes());
                    LevelData.Blocks.RemoveAt(SelectedBlock);
                    LevelData.BlockBmps.RemoveAt(SelectedBlock);
                    LevelData.BlockBmpBits.RemoveAt(SelectedBlock);
                    LevelData.CompBlockBmps.RemoveAt(SelectedBlock);
                    LevelData.CompBlockBmpBits.RemoveAt(SelectedBlock);
                    LevelData.ColInds1.RemoveAt(SelectedBlock);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                            LevelData.ColInds2.RemoveAt(SelectedBlock);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block == SelectedBlock)
                                    dr = true;
                                else if (LevelData.Chunks[i].blocks[x, y].Block > SelectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block--;
                        if (dr)
                            LevelData.RedrawChunk(i);
                    }
                    BlockSelector.SelectedIndex = Math.Min(BlockSelector.SelectedIndex, LevelData.Blocks.Count - 1);
                    break;
                case 5: // Tiles
                    Clipboard.SetData("SonLVLTile", LevelData.Tiles[SelectedTile]);
                    LevelData.Tiles.RemoveAt(SelectedTile);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.RemoveAt(SelectedTile);
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile == SelectedTile)
                                    dr = true;
                                else if (LevelData.Blocks[i].tiles[x, y].Tile > SelectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile--;
                        if (dr)
                            LevelData.RedrawBlock(i, true);
                    }
                    TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
                    break;
            }
        }

        private void copyTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    Clipboard.SetData("SonLVLChunk", LevelData.Chunks[SelectedChunk].GetBytes());
                    break;
                case 4: // Blocks
                    Clipboard.SetData("SonLVLBlock", LevelData.Blocks[SelectedBlock].GetBytes());
                    break;
                case 5: // Tiles
                    Clipboard.SetData("SonLVLTile", LevelData.Tiles[SelectedTile]);
                    break;
            }
        }

        private void pasteBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    LevelData.Chunks.InsertBefore(SelectedChunk, new Chunk((byte[])Clipboard.GetData("SonLVLChunk"), 0));
                    LevelData.ChunkBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.CompChunkBmps.Insert(SelectedChunk, null);
                    LevelData.CompChunkBmpBits.Insert(SelectedChunk, null);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= SelectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= SelectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(SelectedChunk);
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    break;
                case 4: // Blocks
                    LevelData.Blocks.InsertBefore(SelectedBlock, new Block((byte[])Clipboard.GetData("SonLVLBlock"), 0));
                    LevelData.BlockBmps.Insert(SelectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(SelectedBlock, new BitmapBits[2]);
                    LevelData.CompBlockBmps.Insert(SelectedBlock, null);
                    LevelData.CompBlockBmpBits.Insert(SelectedBlock, null);
                    LevelData.ColInds1.Insert(SelectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                            LevelData.ColInds2.Insert(SelectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= SelectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(SelectedBlock, false);
                    BlockSelector.SelectedIndex = SelectedBlock;
                    break;
                case 5: // Tiles
                    byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
                    LevelData.Tiles.InsertBefore(SelectedTile, t);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(SelectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= SelectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = SelectedTile;
                    break;
            }
        }

        private void pasteAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    LevelData.Chunks.InsertAfter(SelectedChunk, new Chunk((byte[])Clipboard.GetData("SonLVLChunk"), 0));
                    SelectedChunk++;
                    LevelData.ChunkBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.CompChunkBmps.Insert(SelectedChunk, null);
                    LevelData.CompChunkBmpBits.Insert(SelectedChunk, null);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= SelectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= SelectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(SelectedChunk);
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    break;
                case 4: // Blocks
                    LevelData.Blocks.InsertAfter(SelectedBlock, new Block((byte[])Clipboard.GetData("SonLVLBlock"), 0));
                    SelectedBlock++;
                    LevelData.BlockBmps.Insert(SelectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(SelectedBlock, new BitmapBits[2]);
                    LevelData.CompBlockBmps.Insert(SelectedBlock, null);
                    LevelData.CompBlockBmpBits.Insert(SelectedBlock, null);
                    LevelData.ColInds1.Insert(SelectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                            LevelData.ColInds2.Insert(SelectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= SelectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(SelectedBlock, false);
                    BlockSelector.SelectedIndex = SelectedBlock;
                    break;
                case 5: // Tiles
                    byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
                    LevelData.Tiles.InsertAfter(SelectedTile, t);
                    SelectedTile++;
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(SelectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= SelectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = SelectedTile;
                    break;
            }
        }

        private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    LevelData.Chunks.InsertBefore(SelectedChunk, new Chunk());
                    LevelData.ChunkBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.CompChunkBmps.Insert(SelectedChunk, null);
                    LevelData.CompChunkBmpBits.Insert(SelectedChunk, null);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= SelectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= SelectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(SelectedChunk);
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    break;
                case 4: // Blocks
                    LevelData.Blocks.InsertBefore(SelectedBlock, new Block());
                    LevelData.BlockBmps.Insert(SelectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(SelectedBlock, new BitmapBits[2]);
                    LevelData.CompBlockBmps.Insert(SelectedBlock, null);
                    LevelData.CompBlockBmpBits.Insert(SelectedBlock, null);
                    LevelData.ColInds1.Insert(SelectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                            LevelData.ColInds2.Insert(SelectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= SelectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(SelectedBlock, false);
                    BlockSelector.SelectedIndex = SelectedBlock;
                    break;
                case 5: // Tiles
                    LevelData.Tiles.InsertBefore(SelectedTile, new byte[32]);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(SelectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= SelectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = SelectedTile;
                    break;
            }
        }

        private void insertAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks

                    LevelData.Chunks.InsertAfter(SelectedChunk, new Chunk());
                    SelectedChunk++;
                    LevelData.ChunkBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(SelectedChunk, new Bitmap[2]);
                    LevelData.CompChunkBmps.Insert(SelectedChunk, null);
                    LevelData.CompChunkBmpBits.Insert(SelectedChunk, null);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= SelectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= SelectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(SelectedChunk);
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    break;
                case 4: // Blocks
                    LevelData.Blocks.InsertAfter(SelectedBlock, new Block());
                    SelectedBlock++;
                    LevelData.BlockBmps.Insert(SelectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(SelectedBlock, new BitmapBits[2]);
                    LevelData.CompBlockBmps.Insert(SelectedBlock, null);
                    LevelData.CompBlockBmpBits.Insert(SelectedBlock, null);
                    LevelData.ColInds1.Insert(SelectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                            LevelData.ColInds2.Insert(SelectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= SelectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(SelectedBlock, false);
                    BlockSelector.SelectedIndex = SelectedBlock;
                    break;
                case 5: // Tiles
                    LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
                    SelectedTile++;
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(SelectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= SelectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = SelectedTile;
                    break;
            }
        }

        private void deleteTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    LevelData.Chunks.RemoveAt(SelectedChunk);
                    LevelData.ChunkBmpBits.RemoveAt(SelectedChunk);
                    LevelData.ChunkBmps.RemoveAt(SelectedChunk);
                    LevelData.ChunkColBmpBits.RemoveAt(SelectedChunk);
                    LevelData.ChunkColBmps.RemoveAt(SelectedChunk);
                    LevelData.CompChunkBmps.RemoveAt(SelectedChunk);
                    LevelData.CompChunkBmpBits.RemoveAt(SelectedChunk);
                    SelectedChunk = (byte)Math.Min(SelectedChunk, LevelData.Chunks.Count - 1);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] > SelectedChunk)
                                LevelData.FGLayout[x, y]--;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] > SelectedChunk)
                                LevelData.BGLayout[x, y]--;
                    ChunkSelector.SelectedIndex = Math.Min(ChunkSelector.SelectedIndex, LevelData.Chunks.Count - 1);
                    break;
                case 4: // Blocks
                    LevelData.Blocks.RemoveAt(SelectedBlock);
                    LevelData.BlockBmps.RemoveAt(SelectedBlock);
                    LevelData.BlockBmpBits.RemoveAt(SelectedBlock);
                    LevelData.CompBlockBmps.RemoveAt(SelectedBlock);
                    LevelData.CompBlockBmpBits.RemoveAt(SelectedBlock);
                    LevelData.ColInds1.RemoveAt(SelectedBlock);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                            LevelData.ColInds2.RemoveAt(SelectedBlock);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block == SelectedBlock)
                                    dr = true;
                                else if (LevelData.Chunks[i].blocks[x, y].Block > SelectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block--;
                        if (dr)
                            LevelData.RedrawChunk(i);
                    }
                    BlockSelector.SelectedIndex = Math.Min(BlockSelector.SelectedIndex, LevelData.Blocks.Count - 1);
                    break;
                case 5: // Tiles
                    LevelData.Tiles.RemoveAt(SelectedTile);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.RemoveAt(SelectedTile);
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile == SelectedTile)
                                    dr = true;
                                else if (LevelData.Blocks[i].tiles[x, y].Tile > SelectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile--;
                        if (dr)
                            LevelData.RedrawBlock(i, true);
                    }
                    TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
                    break;
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog opendlg = new OpenFileDialog())
            {
                opendlg.DefaultExt = "png";
                opendlg.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif";
                opendlg.RestoreDirectory = true;
                if (opendlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    ImportImage(new Bitmap(opendlg.FileName));
            }
        }

        private void ImportImage(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            int pal = 0;
            bool match = false;
            List<BitmapBits> tiles = new List<BitmapBits>();
            List<Block> blocks = new List<Block>();
            List<Chunk> chunks = new List<Chunk>();
            byte[] tile;
            int curtilecnt = LevelData.Tiles.Count;
            int curblkcnt = LevelData.Blocks.Count;
            switch (tabControl1.SelectedIndex)
            {
                case 3: // Chunks
                    for (int cy = 0; cy < h / LevelData.chunksz; cy++)
                        for (int cx = 0; cx < w / LevelData.chunksz; cx++)
                        {
                            Chunk cnk = new Chunk();
                            for (int by = 0; by < LevelData.chunksz / 16; by++)
                                for (int bx = 0; bx < LevelData.chunksz / 16; bx++)
                                {
                                    Block blk = new Block();
                                    for (int y = 0; y < 2; y++)
                                        for (int x = 0; x < 2; x++)
                                        {
                                            tile = LevelData.BmpToTile(bmp.Clone(new Rectangle((cx * 16) + (bx * 16) + (x * 8), (cy * 16) + (by * 16) + (y * 8), 8, 8), bmp.PixelFormat), out pal);
                                            blk.tiles[x, y].Palette = (byte)pal;
                                            BitmapBits bits = BitmapBits.FromTile(tile, 0);
                                            match = false;
                                            for (int i = 0; i < tiles.Count; i++)
                                            {
                                                if (tiles[i].Equals(bits))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    break;
                                                }
                                                BitmapBits flip = new BitmapBits(bits);
                                                flip.Flip(true, false);
                                                if (tiles[i].Equals(flip))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    blk.tiles[x, y].XFlip = true;
                                                    break;
                                                }
                                                flip = new BitmapBits(bits);
                                                flip.Flip(false, true);
                                                if (tiles[i].Equals(flip))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    blk.tiles[x, y].YFlip = true;
                                                    break;
                                                }
                                                flip = new BitmapBits(bits);
                                                flip.Flip(true, true);
                                                if (tiles[i].Equals(flip))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    blk.tiles[x, y].XFlip = true;
                                                    blk.tiles[x, y].YFlip = true;
                                                    break;
                                                }
                                            }
                                            if (match) continue;
                                            tiles.Add(bits);
                                            LevelData.Tiles.Add(tile);
                                            SelectedTile = LevelData.Tiles.Count - 1;
                                            blk.tiles[x, y].Tile = (ushort)SelectedTile;
                                            LevelData.UpdateTileArray();
                                            TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                                        }
                                    match = false;
                                    for (int i = 0; i < blocks.Count; i++)
                                    {
                                        if (blk.Equals(blocks[i]))
                                        {
                                            match = true;
                                            cnk.blocks[bx, by].Block = (ushort)i;
                                            break;
                                        }
                                    }
                                    if (match) continue;
                                    blocks.Add(blk);
                                    LevelData.Blocks.Add(blk);
                                    LevelData.ColInds1.Add(0);
                                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                                        if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                                            LevelData.ColInds2.Add(0);
                                    SelectedBlock = LevelData.Blocks.Count - 1;
                                    LevelData.BlockBmps.Add(new Bitmap[2]);
                                    LevelData.BlockBmpBits.Add(new BitmapBits[2]);
                                    LevelData.CompBlockBmps.Add(null);
                                    LevelData.CompBlockBmpBits.Add(null);
                                    LevelData.RedrawBlock(SelectedBlock, false);
                                    cnk.blocks[bx, by].Block = (ushort)SelectedBlock;
                                }
                            match = false;
                            for (int i = 0; i < chunks.Count; i++)
                            {
                                if (cnk.Equals(chunks[i]))
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (match) continue;
                            chunks.Add(cnk);
                            LevelData.Chunks.Add(cnk);
                            SelectedChunk = (byte)(LevelData.Chunks.Count - 1);
                            LevelData.ChunkBmpBits.Add(new BitmapBits[2]);
                            LevelData.ChunkBmps.Add(new Bitmap[2]);
                            LevelData.ChunkColBmpBits.Add(new BitmapBits[2]);
                            LevelData.ChunkColBmps.Add(new Bitmap[2]);
                            LevelData.CompChunkBmps.Add(null);
                            LevelData.CompChunkBmpBits.Add(null);
                            LevelData.RedrawChunk(SelectedChunk);
                        }
                    TileSelector.SelectedIndex = SelectedTile;
                    BlockSelector.SelectedIndex = SelectedBlock;
                    ChunkSelector.SelectedIndex = SelectedChunk;
                    break;
                case 4: // Blocks
                    for (int by = 0; by < h / 16; by++)
                        for (int bx = 0; bx < w / 16; bx++)
                        {
                            Block blk = new Block();
                            for (int y = 0; y < 2; y++)
                                for (int x = 0; x < 2; x++)
                                {
                                    tile = LevelData.BmpToTile(bmp.Clone(new Rectangle((bx * 16) + (x * 8), (by * 16) + (y * 8), 8, 8), bmp.PixelFormat), out pal);
                                    blk.tiles[x, y].Palette = (byte)pal;
                                    BitmapBits bits = BitmapBits.FromTile(tile, 0);
                                    match = false;
                                    for (int i = 0; i < tiles.Count; i++)
                                    {
                                        if (tiles[i].Equals(bits))
                                        {
                                            match = true;
                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                            break;
                                        }
                                        BitmapBits flip = new BitmapBits(bits);
                                        flip.Flip(true, false);
                                        if (tiles[i].Equals(flip))
                                        {
                                            match = true;
                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                            blk.tiles[x, y].XFlip = true;
                                            break;
                                        }
                                        flip = new BitmapBits(bits);
                                        flip.Flip(false, true);
                                        if (tiles[i].Equals(flip))
                                        {
                                            match = true;
                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                            blk.tiles[x, y].YFlip = true;
                                            break;
                                        }
                                        flip = new BitmapBits(bits);
                                        flip.Flip(true, true);
                                        if (tiles[i].Equals(flip))
                                        {
                                            match = true;
                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                            blk.tiles[x, y].XFlip = true;
                                            blk.tiles[x, y].YFlip = true;
                                            break;
                                        }
                                    }
                                    if (match) continue;
                                    tiles.Add(bits);
                                    LevelData.Tiles.Add(tile);
                                    SelectedTile = LevelData.Tiles.Count - 1;
                                    blk.tiles[x, y].Tile = (ushort)SelectedTile;
                                    LevelData.UpdateTileArray();
                                    TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                                }
                            match = false;
                            for (int i = 0; i < blocks.Count; i++)
                            {
                                if (blk.Equals(blocks[i]))
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (match) continue;
                            blocks.Add(blk);
                            LevelData.Blocks.Add(blk);
                            LevelData.ColInds1.Add(0);
                            if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S2NA || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                                if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
                                    LevelData.ColInds2.Add(0);
                            SelectedBlock = LevelData.Blocks.Count - 1;
                            LevelData.BlockBmps.Add(new Bitmap[2]);
                            LevelData.BlockBmpBits.Add(new BitmapBits[2]);
                            LevelData.CompBlockBmps.Add(null);
                            LevelData.CompBlockBmpBits.Add(null);
                            LevelData.RedrawBlock(SelectedBlock, false);
                        }
                    TileSelector.SelectedIndex = SelectedTile;
                    BlockSelector.SelectedIndex = SelectedBlock;
                    break;
                case 5: // Tiles
                    for (int y = 0; y < h / 8; y++)
                        for (int x = 0; x < w / 8; x++)
                        {
                            tile = LevelData.BmpToTile(bmp.Clone(new Rectangle(x * 8, y * 8, 8, 8), bmp.PixelFormat), out pal);
                            BitmapBits bits = BitmapBits.FromTile(tile, 0);
                            match = false;
                            for (int i = 0; i < tiles.Count; i++)
                            {
                                if (tiles[i].Equals(bits))
                                {
                                    match = true;
                                    break;
                                }
                                BitmapBits flip = new BitmapBits(bits);
                                flip.Flip(true, false);
                                if (tiles[i].Equals(flip))
                                {
                                    match = true;
                                    break;
                                }
                                flip = new BitmapBits(bits);
                                flip.Flip(false, true);
                                if (tiles[i].Equals(flip))
                                {
                                    match = true;
                                    break;
                                }
                                flip = new BitmapBits(bits);
                                flip.Flip(true, true);
                                if (tiles[i].Equals(flip))
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (match) continue;
                            tiles.Add(bits);
                            LevelData.Tiles.Add(tile);
                            SelectedTile = LevelData.Tiles.Count - 1;
                            LevelData.UpdateTileArray();
                            TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2));
                        }
                    TileSelector.SelectedIndex = SelectedTile;
                    break;
            }
            bmp.Dispose();
        }

        private int SelectedCol;
        private void CollisionSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CollisionSelector.SelectedIndex > -1)
            {
                SelectedCol = CollisionSelector.SelectedIndex;
                ColAngle.Value = LevelData.Angles[SelectedCol];
                ColID.Text = SelectedCol.ToString("X2");
                ColPicture.Invalidate();
            }
        }

        private void ColPicture_Paint(object sender, PaintEventArgs e)
        {
            if (CollisionSelector.SelectedIndex == -1) return;
            e.Graphics.SetOptions();
            e.Graphics.DrawImage(LevelData.ColBmpBits[SelectedCol].Scale(4).ToBitmap(Color.Black, Color.White), 0, 0, 64, 64);
        }

        private void ColPicture_MouseDown(object sender, MouseEventArgs e)
        {
            if (CollisionSelector.SelectedIndex == -1) return;
            int x = e.X / 4;
            int y = e.Y / 4;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    LevelData.ColArr1[SelectedCol][x] = (sbyte)(16 - y);
                    break;
                case MouseButtons.Right:
                    if (y == 16)
                        LevelData.ColArr1[SelectedCol][x] = 0;
                    else
                        LevelData.ColArr1[SelectedCol][x] = (sbyte)(-y - 1);
                    break;
            }
            LevelData.RedrawCol(SelectedCol, false);
            ColPicture.Invalidate();
            CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
        }

        private void ColPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (CollisionSelector.SelectedIndex == -1) return;
            int x = e.X / 4;
            if (x < 0 | x > 15) return;
            int y = e.Y / 4;
            if (y < 0 | y > 16) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    LevelData.ColArr1[SelectedCol][x] = (sbyte)(16 - y);
                    LevelData.RedrawCol(SelectedCol, false);
                    ColPicture.Invalidate();
                    CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
                    break;
                case MouseButtons.Right:
                    if (y == 16)
                        LevelData.ColArr1[SelectedCol][x] = 0;
                    else
                        LevelData.ColArr1[SelectedCol][x] = (sbyte)(-y - 1);
                    LevelData.RedrawCol(SelectedCol, false);
                    ColPicture.Invalidate();
                    CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
                    break;
            }
        }

        private void ColPicture_MouseUp(object sender, KeyEventArgs e)
        {
            if (CollisionSelector.SelectedIndex == -1) return;
            LevelData.RedrawCol(SelectedCol, true);
            ColPicture.Invalidate();
            CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
        }

        private void ColAngle_ValueChanged(object sender, EventArgs e)
        {
            if (CollisionSelector.SelectedIndex == -1) return;
            LevelData.Angles[SelectedCol] = (byte)ColAngle.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!loaded) return;
            CollisionSelector sel = new CollisionSelector();
            sel.ShowDialog(this);
            BlockCollision1.Value = sel.Selection;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!loaded) return;
            CollisionSelector sel = new CollisionSelector();
            sel.ShowDialog(this);
            BlockCollision2.Value = sel.Selection;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (TileSelector.SelectedIndex == -1) return;
            tile.Rotate(3);
            LevelData.Tiles[SelectedTile] = tile.ToTile();
            LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
            for (int i = 0; i < LevelData.Blocks.Count; i++)
            {
                bool dr = false;
                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 2; x++)
                        if (LevelData.Blocks[i].tiles[x, y].Tile == SelectedTile)
                            dr = true;
                if (dr)
                    LevelData.RedrawBlock(i, true);
            }
            TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, 2);
            TilePicture.Invalidate();
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (DrawTileDialog dlg = new DrawTileDialog())
            {
                switch (tabControl1.SelectedIndex)
                {
                    case 3: // Chunks
                        dlg.tile = new BitmapBits(LevelData.chunksz, LevelData.chunksz);
                        break;
                    case 4: // Blocks
                        dlg.tile = new BitmapBits(16, 16);
                        break;
                    case 5: // Tiles
                        dlg.tile = new BitmapBits(8, 8);
                        break;
                }
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    ImportImage(dlg.tile.ToBitmap(LevelData.BmpPal));
            }
        }

        private void TileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControl1.SelectedIndex > 2)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        if (e.Control)
                            copyTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
                        break;
                    case Keys.Delete:
                        deleteTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
                        break;
                    case Keys.Insert:
                        switch (tabControl1.SelectedIndex)
                        {
                            case 3: // Chunks
                                if (LevelData.Chunks.Count < 0x100)
                                    insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
                                break;
                            case 4: // Blocks
                                int blockmax = 0x400;
                                switch (LevelData.EngineVersion)
                                {
                                    case EngineVersion.S2:
                                    case EngineVersion.S2NA:
                                        blockmax = 0x340;
                                        break;
                                    case EngineVersion.S3K:
                                    case EngineVersion.SKC:
                                        blockmax = 0x300;
                                        break;
                                }
                                if (LevelData.Blocks.Count < blockmax)
                                    insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
                                break;
                            case 5: // Tiles
                                if (LevelData.Tiles.Count < 0x800)
                                    insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
                                break;
                        }
                        break;
                    case Keys.V:
                        if (e.Control)
                            switch (tabControl1.SelectedIndex)
                            {
                                case 3: // Chunks
                                    if (Clipboard.GetDataObject().GetDataPresent("SonLVLChunk") & LevelData.Chunks.Count < 0x100)
                                        pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
                                    break;
                                case 4: // Blocks
                                    int blockmax = 0x400;
                                    switch (LevelData.EngineVersion)
                                    {
                                        case EngineVersion.S2:
                                        case EngineVersion.S2NA:
                                            blockmax = 0x340;
                                            break;
                                        case EngineVersion.S3K:
                                        case EngineVersion.SKC:
                                            blockmax = 0x300;
                                            break;
                                    }
                                    if (Clipboard.GetDataObject().GetDataPresent("SonLVLBlock") & LevelData.Blocks.Count < blockmax)
                                        pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
                                    break;
                                case 5: // Tiles
                                    if (Clipboard.GetDataObject().GetDataPresent("SonLVLTile") & LevelData.Tiles.Count < 0x800)
                                        pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
                                    break;
                            }
                        break;
                    case Keys.X:
                        if (e.Control)
                            cutTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
                        break;
                }
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog a = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                FullOpen = true,
                SolidColorOnly = true,
                Color = Properties.Settings.Default.GridColor
            };
            if (cols != null)
                a.CustomColors = cols;
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.GridColor = a.Color;
                LevelImgPalette.Entries[67] = a.Color;
                DrawLevel();
            }
            cols = a.CustomColors;
            a.Dispose();
        }

        private void viewReadmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "readme.txt"));
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BugReportDialog err = new BugReportDialog())
                err.ShowDialog();
        }

        private void zoomToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem item in zoomToolStripMenuItem.DropDownItems)
                item.Checked = false;
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            switch (zoomToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem))
            {
                case 0: // 1/2x
                    ZoomLevel = 0.5;
                    break;
                default:
                    ZoomLevel = zoomToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
                    break;
            }
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    LevelImg8bpp = new BitmapBits((int)(panel1.Width / ZoomLevel), (int)(panel1.Height / ZoomLevel));
                    break;
                case 1:
                    LevelImg8bpp = new BitmapBits((int)(panel2.Width / ZoomLevel), (int)(panel2.Height / ZoomLevel));
                    break;
                case 2:
                    LevelImg8bpp = new BitmapBits((int)(panel3.Width / ZoomLevel), (int)(panel3.Height / ZoomLevel));
                    break;
                default:
                    LevelImg8bpp = new BitmapBits(1, 1);
                    break;
            }
            DrawLevel();
        }

        private void selectAllObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Entry[] items = new Entry[LevelData.Objects.Count];
            for (int i = 0; i < items.Length; i++)
                items[i] = LevelData.Objects[i];
            SelectedItems = new List<Entry>(items);
            SelectedObjectChanged();
        }

        private void selectAllRingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Entry> items = null;
            switch (LevelData.RingFmt)
            {
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    items = new List<Entry>(LevelData.Rings.Count);
                    for (int i = 0; i < items.Count; i++)
                        items.Add(LevelData.Rings[i]);
                    break;
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    items = new List<Entry>();
                    foreach (ObjectEntry item in LevelData.Objects)
                        if (item.ID == 0x25)
                            items.Add(item);
                    break;
            }
            SelectedItems = new List<Entry>(items);
            SelectedObjectChanged();
        }
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
        private int mode;

        public LayoutEditUndoAction(int Mode, List<Point> Locations, List<byte> OldTiles)
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
                case 1:
                    for (int i = 0; i < locations.Count; i++)
                    {
                        t = LevelData.FGLayout[locations[i].X, locations[i].Y];
                        LevelData.FGLayout[locations[i].X, locations[i].Y] = oldtiles[i];
                        oldtiles[i] = t;
                    }
                    break;
                case 2:
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