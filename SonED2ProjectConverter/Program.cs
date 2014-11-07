using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SonicRetro.SonLVL.API;
using System.Globalization;
using System.Drawing;
using SonicRetro.SonLVL.API.XMLDef;

namespace SonED2ProjectConverter
{
    class Program
    {
        static void Main(string[] args)
        {
			try
			{
				string folder;
				if (args.Length > 0)
				{
					folder = args[0];
					Console.WriteLine("Folder: " + folder);
				}
				else
				{
					Console.Write("Folder: ");
					folder = Console.ReadLine();
				}
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, folder);
				string outdir = Path.Combine(Environment.CurrentDirectory, "Converted");
				if (!Directory.Exists(outdir)) Directory.CreateDirectory(outdir);
				GameInfo output = new GameInfo() { Levels = new Dictionary<string, LevelInfo>() };
				foreach (string filename in Directory.GetFiles(Environment.CurrentDirectory, "*.sep"))
				{
					List<string> data = new List<string>();
					foreach (string line in File.ReadAllLines(filename))
					{
						string line2 = line.Split(';')[0].Trim();
						if (line2.Length == 0 | !line2.Contains(":")) continue;
						data.Add(line2.Substring(line2.IndexOf(':') + 1).Trim());
					}
					int linenum = 0;
					int gamenum;
					if (int.TryParse(data[linenum], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out gamenum)) // Old format
					{
						LevelInfo info = new LevelInfo();
						switch (gamenum)
						{
							case 1: // Sonic 1
								output.EngineVersion = info.EngineVersion = EngineVersion.S1;
								break;
							case 2: // Sonic 2
								output.EngineVersion = info.EngineVersion = EngineVersion.S2;
								break;
						}
						linenum++;
						string zoneid = data[linenum++];
						linenum++;
						string objdef = data[linenum++];
						ReadObjectDefinition(objdef);
						if (File.Exists(Path.Combine(outdir, Path.GetFileName(objdef) + zoneid + ".ini")))
							info.ObjectList = new[] { Path.GetFileName(objdef) + ".ini", Path.GetFileName(objdef) + zoneid + ".ini" };
						else
							info.ObjectList = new[] { Path.GetFileName(objdef) + ".ini" };
						info.Tiles = new[] { new SonicRetro.SonLVL.API.FileInfo(data[linenum++]) };
						info.Blocks = new[] { new SonicRetro.SonLVL.API.FileInfo(data[linenum++]) };
						info.Chunks = new[] { new SonicRetro.SonLVL.API.FileInfo(data[linenum++]) };
						switch (gamenum)
						{
							case 1: // Sonic 1
								info.FGLayout = data[linenum++];
								info.BGLayout = data[linenum++];
								break;
							case 2: // Sonic 2
								info.Layout = data[linenum++];
								break;
						}
						info.Objects = data[linenum++];
						if (gamenum == 2)
						{
							info.Rings = data[linenum++];
							if (!data[linenum].Equals("NONE", StringComparison.OrdinalIgnoreCase))
								info.Bumpers = data[linenum];
							linenum++;
						}
						int numpal = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						string pal1 = string.Empty;
						string pal2 = null;
						for (int i = 0; i < numpal; i++)
						{
							int palstart = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
							string pallen = data[linenum++];
							string palfile = data[linenum++];
							if (palstart >= 64)
							{
								if (pal2 == null)
									pal2 = "Secondary";
								pal2 += "|" + palfile + ":0:" + (palstart - 64) + ":" + pallen;
							}
							else
							{
								if (pal1 != string.Empty)
									pal1 += "|";
								pal1 += palfile + ":0:" + palstart + ":" + pallen;
							}
						}
						info.Palette = new PaletteList(pal1);
						if (pal2 != null)
							info.ExtraPalettes = new[] { new NamedPaletteList(pal2) };
						info.Angles = data[linenum++];
						info.CollisionArray1 = data[linenum++];
						info.CollisionArray2 = data[linenum++];
						switch (gamenum)
						{
							case 1: // Sonic 1
								info.CollisionIndex = data[linenum++];
								break;
							case 2: // Sonic 2
								info.CollisionIndex1 = data[linenum++];
								info.CollisionIndex2 = data[linenum++];
								break;
						}
						output.Levels.Add(Path.GetFileNameWithoutExtension(filename), info);
					}
					else if (data[linenum++] == "Level") // New format
					{
						LevelInfo info = new LevelInfo();
						string zoneid = data[linenum++];
						linenum++;
						string objdef = data[linenum++];
						int numsprites = ReadObjectDefinition(objdef);
						if (File.Exists(Path.Combine(outdir, Path.GetFileName(objdef) + zoneid + ".ini")))
							info.ObjectList = new[] { Path.GetFileName(objdef) + ".ini", Path.GetFileName(objdef) + zoneid + ".ini" };
						else
							info.ObjectList = new[] { Path.GetFileName(objdef) + ".ini" };
						linenum++;
						info.TileCompression = compressionTypes[int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo)];
						int cnt = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						List<SonicRetro.SonLVL.API.FileInfo> filelist = new List<SonicRetro.SonLVL.API.FileInfo>(cnt);
						for (int i = 0; i < cnt; i++)
							filelist.Add(new SonicRetro.SonLVL.API.FileInfo(data[linenum++]));
						info.Tiles = filelist.ToArray();
						info.BlockCompression = compressionTypes[int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo)];
						cnt = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						filelist = new List<SonicRetro.SonLVL.API.FileInfo>(cnt);
						for (int i = 0; i < cnt; i++)
						{
							linenum++;
							filelist.Add(new SonicRetro.SonLVL.API.FileInfo(data[linenum++]));
						}
						info.Blocks = filelist.ToArray();
						switch (int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo))
						{
							case 128:
								info.ChunkFormat = EngineVersion.S2;
								break;
							case 256:
								info.ChunkFormat = EngineVersion.S1;
								break;
						}
						info.ChunkCompression = compressionTypes[int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo)];
						cnt = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						filelist = new List<SonicRetro.SonLVL.API.FileInfo>(cnt);
						for (int i = 0; i < cnt; i++)
						{
							linenum++;
							filelist.Add(new SonicRetro.SonLVL.API.FileInfo(data[linenum++]));
						}
						info.Chunks = filelist.ToArray();
						switch (int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo))
						{
							case 0:
								info.LayoutFormat = EngineVersion.S1;
								break;
							case 1:
								info.LayoutFormat = EngineVersion.S2;
								break;
							case 2:
								info.LayoutFormat = EngineVersion.S3K;
								break;
						}
						info.LayoutCompression = compressionTypes[int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo)];
						switch (info.LayoutFormat)
						{
							case EngineVersion.S1:
								info.FGLayout = data[linenum++];
								linenum++;
								info.BGLayout = data[linenum++];
								break;
							case EngineVersion.S2:
							case EngineVersion.S3K:
								linenum++;
								info.Layout = data[linenum++];
								break;
						}
						switch (int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo))
						{
							case 1:
								info.ObjectFormat = EngineVersion.S1;
								info.Objects = data[linenum++];
								break;
							case 2:
								info.ObjectFormat = EngineVersion.SCD;
								switch (data[linenum++])
								{
									case "A":
										info.TimeZone = SonicRetro.SonLVL.API.TimeZone.Present;
										break;
									case "B":
										info.TimeZone = SonicRetro.SonLVL.API.TimeZone.Past;
										break;
									case "C":
									case "D":
										info.TimeZone = SonicRetro.SonLVL.API.TimeZone.Future;
										break;
								}
								info.Objects = data[linenum++];
								break;
							case 3:
								info.ObjectFormat = EngineVersion.S2;
								info.Objects = data[linenum++];
								break;
							case 4:
								info.ObjectFormat = EngineVersion.S3K;
								info.Objects = data[linenum++];
								break;
						}
						switch (int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo))
						{
							case 1:
								info.RingFormat = EngineVersion.S2;
								info.Rings = data[linenum++];
								break;
							case 2:
								info.RingFormat = EngineVersion.S3K;
								info.Rings = data[linenum++];
								break;
						}
						if (int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo) == 1)
							info.Bumpers = data[linenum++];
						cnt = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						List<StartPositionInfo> startlist = new List<StartPositionInfo>(cnt);
						for (int i = 0; i < cnt; i++)
							startlist.Add(new StartPositionInfo(data[linenum++] + ":Sprite" + (numsprites - cnt + i) + ":Start Position " + i));
						info.StartPositions = startlist.ToArray();
						int numpal = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						string pal1 = string.Empty;
						string pal2 = null;
						for (int i = 0; i < numpal; i++)
						{
							int palstart = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
							string pallen = data[linenum++];
							string palfile = data[linenum++];
							if (palstart >= 64)
							{
								if (pal2 == null)
									pal2 = "Secondary";
								pal2 += "|" + palfile + ":0:" + (palstart - 64) + ":" + pallen;
							}
							else
							{
								if (pal1 != string.Empty)
									pal1 += "|";
								pal1 += palfile + ":0:" + palstart + ":" + pallen;
							}
						}
						info.Palette = new PaletteList(pal1);
						if (pal2 != null)
							info.ExtraPalettes = new[] { new NamedPaletteList(pal2) };
						linenum++;
						info.CollisionArray1 = data[linenum++];
						info.CollisionArray2 = data[linenum++];
						info.Angles = data[linenum++];
						switch (int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo))
						{
							case 0:
								info.CollisionIndexFormat = EngineVersion.S1;
								break;
							case 1:
								info.CollisionIndexFormat = EngineVersion.S2;
								break;
							case 2:
								info.CollisionIndexFormat = EngineVersion.S3K;
								info.CollisionIndexSize = 2;
								break;
							case 3:
								info.CollisionIndexFormat = EngineVersion.S3K;
								info.CollisionIndexSize = 1;
								break;
						}
						info.CollisionIndexCompression = compressionTypes[int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo)];
						switch (info.CollisionIndexFormat)
						{
							case EngineVersion.S1:
							case EngineVersion.S3K:
								info.CollisionIndex = data[linenum++];
								break;
							case EngineVersion.S2:
								info.CollisionIndex1 = data[linenum++];
								info.CollisionIndex2 = data[linenum++];
								break;
						}
						switch (data[linenum++])
						{
							case "Sonic 1":
								output.EngineVersion = info.EngineVersion = EngineVersion.S1;
								break;
							case "Sonic CD":
								output.EngineVersion = info.EngineVersion = EngineVersion.SCD;
								break;
							case "Sonic 2":
								output.EngineVersion = info.EngineVersion = EngineVersion.S2;
								break;
							case "Sonic 3":
								output.EngineVersion = info.EngineVersion = EngineVersion.S3K;
								break;
						}
						output.Levels.Add(Path.GetFileNameWithoutExtension(filename), info);
					}
					else
						Console.WriteLine(Path.GetFileName(filename) + " not a level project, skipping.");
				}
				if (output.EngineVersion == EngineVersion.Invalid)
					output.EngineVersion = EngineVersion.S1;
				output.Save(Path.Combine(outdir, "SonLVL.ini"));
			}
			catch (Exception ex)
			{
				File.WriteAllText("SonED2ProjectConverter.log", ex.ToString());
				throw;
			}
        }

        static readonly Dictionary<int, CompressionType> compressionTypes = new Dictionary<int, CompressionType>()
        {
            { 0, CompressionType.Uncompressed },
            { 1, CompressionType.Kosinski },
            { 2, CompressionType.Enigma },
            { 3, CompressionType.Nemesis },
            { 5, CompressionType.KosinskiM }
        };

        static int ReadObjectDefinition(string filename)
        {
            int result = 0;
            string outdir = Path.Combine(Environment.CurrentDirectory, "Converted");
            string pcxfmt = filename + "{0}.pcx";
            List<string> data = new List<string>();
            foreach (string line in File.ReadAllLines(filename + ".lst"))
            {
                string line2 = line.Split(';')[0].Trim();
                if (line2.Length == 0 | !line2.Contains(":")) continue;
                data.Add(line2.Substring(line2.IndexOf(':') + 1).Trim());
            }
            int linenum = 0;
            int numspritesets = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            Dictionary<string, Bitmap> pcxfiles = new Dictionary<string, Bitmap>();
            string spritedir = Path.Combine(outdir, "Sprites");
            if (!Directory.Exists(spritedir)) Directory.CreateDirectory(spritedir);
            Dictionary<string, Dictionary<string, ObjectData>> objectinis = new Dictionary<string, Dictionary<string, ObjectData>>()
                {
                    { "FF", new Dictionary<string, ObjectData>() }
                };
            for (int set = 0; set < numspritesets; set++)
            {
                int numsprites = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                if (set == 0) result = numsprites;
                for (int sprite = 0; sprite < numsprites; sprite++)
                {
                    Bitmap pcxfile;
                    if (!pcxfiles.ContainsKey(data[linenum]))
                        pcxfiles.Add(data[linenum], LoadPCXFile(string.Format(pcxfmt, data[linenum])));
                    pcxfile = pcxfiles[data[linenum++]];
                    Rectangle rect = new Rectangle(int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
                    pcxfile.Clone(rect, pcxfile.PixelFormat).Save(Path.Combine(spritedir, Path.GetFileName(filename) + "sprite" + set.ToString(NumberFormatInfo.InvariantInfo) + "-" + sprite.ToString(NumberFormatInfo.InvariantInfo) + ".png"));
                    if (set == 0)
                    {
                        ObjectData obj = new ObjectData();
                        obj.Image = Path.Combine("Sprites", Path.GetFileName(filename) + "sprite" + set.ToString(NumberFormatInfo.InvariantInfo) + "-" + sprite.ToString(NumberFormatInfo.InvariantInfo) + ".png");
                        obj.Offset = new Size(rect.Width / 2, rect.Height / 2);
                        objectinis["FF"]["Sprite" + sprite] = obj;
                    }
                }
            }
            int numobjects = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            for (int obj = 0; obj < numobjects; obj++)
            {
                byte objid = byte.Parse(data[linenum++], NumberStyles.HexNumber);
                int numzones = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                for (int zone = 0; zone < numzones; zone++)
                {
                    string zoneid = data[linenum++];
                    if (!objectinis.ContainsKey(zoneid))
                        objectinis.Add(zoneid, new Dictionary<string, ObjectData>());
                    string zonename = zoneid == "FF" ? "Common" : zoneid;
                    if (!Directory.Exists(Path.Combine(outdir, zonename)))
                        Directory.CreateDirectory(Path.Combine(outdir, zonename));
                    string description = data[linenum++];
                    string id = description.MakeIdentifier();
                    ObjectData objdata = new ObjectData();
                    ObjDef xmldata = new ObjDef() { Language = "cs", Namespace = "SonED2.Zone" + zonename, Image = "Image1" };
                    objdata.XMLFile = zonename + "/" + id + ".xml";
                    xmldata.Name = description;
                    xmldata.TypeName = id;
                    int numbitfields = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                    if (numbitfields > 0)
                    {
                        xmldata.Properties = new PropertyList() { Items = new Property[numbitfields] };
                        xmldata.Enums = new EnumList() { Items = new SonicRetro.SonLVL.API.XMLDef.Enum[numbitfields] };
                        for (int property = 0; property < numbitfields; property++)
                        {
                            string bits = data[linenum++];
                            int startbit = -1;
                            int endbit = -1;
                            for (int i = 7; i >= 0; i--)
                            {
                                if (bits[i] == '1')
                                {
                                    if (startbit == -1)
                                        startbit = 7 - i;
                                    endbit = 7 - i;
                                }
                                else if (startbit != -1)
                                    break;
                            }
                            int length = endbit - startbit + 1;
                            int numenum = (int)Math.Pow(2, length);
                            xmldata.Properties.Items[property] = new BitsProperty() { name = "Property" + (property + 1).ToString(NumberFormatInfo.InvariantInfo), startbit = startbit, length = length, type = "Enum" + (property + 1).ToString(NumberFormatInfo.InvariantInfo) };
                            xmldata.Enums.Items[property] = new SonicRetro.SonLVL.API.XMLDef.Enum() { name = "Enum" + (property + 1).ToString(NumberFormatInfo.InvariantInfo), Items = new EnumMember[numenum] };
                            for (int i = 0; i < numenum; i++)
                            {
                                string enumnamebase = data[linenum++];
                                string enumname = enumnamebase;
                                int j = 2;
                                for (int k = 0; k < i; k++)
                                    if (xmldata.Enums.Items[property].Items[k].name == enumname)
                                        enumname = enumnamebase + (j++).ToString(NumberFormatInfo.InvariantInfo);
                                xmldata.Enums.Items[property].Items[i] = new EnumMember() { name = enumname, value = i };
                            }
                        }
                    }
                    xmldata.DefaultSubtype = data[linenum++];
                    linenum += 2; // skip DefaultFlag and DefaultDir
                    xmldata.RememberState = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo) == 1;
                    int numdependents = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                    int[] dependents = new int[numdependents];
                    for (int i = 0; i < numdependents; i++)
                        dependents[i] = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                    int numtypes = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                    if (numtypes > 0)
                    {
                        xmldata.Display = new Display() { DisplayOptions = new DisplayOption[numtypes] };
                        List<string> images = new List<string>();
                        for (int type = 0; type < numtypes; type++)
                        {
                            xmldata.Display.DisplayOptions[type] = new DisplayOption();
                            if (numdependents > 0)
                            {
                                xmldata.Display.DisplayOptions[type].Conditions = new Condition[numdependents];
                                for (int dependent = 0; dependent < numdependents; dependent++)
                                    xmldata.Display.DisplayOptions[type].Conditions[dependent] = new Condition() { property = ((BitsProperty)xmldata.Properties.Items[dependents[dependent]]).name, value = xmldata.Enums.Items[dependents[dependent]].Items[int.Parse(data[linenum++], NumberStyles.HexNumber)].name };
                            }
                            linenum += 4; // skip XMin, YMin, XMax, YMax
                            int numsprites = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            xmldata.Display.DisplayOptions[type].Images = new ImageRef[numsprites];
                            for (int sprite = 0; sprite < numsprites; sprite++)
                            {
                                int sprnum = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                int sprset = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                int dir = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                                string sprname = Path.GetFileName(filename) + "sprite" + sprset.ToString(NumberFormatInfo.InvariantInfo) + "-" + sprnum.ToString(NumberFormatInfo.InvariantInfo) + ".png";
                                if (!images.Contains(sprname))
                                    images.Add(sprname);
                                xmldata.Display.DisplayOptions[type].Images[sprite] = new ImageRef() { image = "Image" + (images.IndexOf(sprname) + 1).ToString(NumberFormatInfo.InvariantInfo), Offset = new XmlPoint() { X = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), Y = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo) }, xflip = (dir & 1) == 1 ? FlipType.ReverseFlip : FlipType.NormalFlip, yflip = (dir & 2) == 2 ? FlipType.ReverseFlip : FlipType.NormalFlip };
                            }
                        }
                        xmldata.Images = new ImageList() { Items = new SonicRetro.SonLVL.API.XMLDef.Image[images.Count] };
                        for (int i = 0; i < images.Count; i++)
                            xmldata.Images.Items[i] = new ImageFromBitmap() { id = "Image" + (i + 1).ToString(NumberFormatInfo.InvariantInfo), filename = "Sprites/" + images[i] };
                    }
                    else
                    {
                        xmldata.Images = new ImageList() { Items = new SonicRetro.SonLVL.API.XMLDef.Image[] { new ImageFromBitmap() { id = "Image1", filename = "Sprites/" + Path.GetFileName(filename) + "sprite0-1.png" } } };
                        xmldata.Display = new Display() { DisplayOptions = new DisplayOption[] { new DisplayOption() { Images = new ImageRef[] { new ImageRef() { image = "Image1", Offset = new XmlPoint() { X = 8, Y = 7 } } } } } };
                    }
                    objectinis[zoneid][objid.ToString("X2")] = objdata;
                    xmldata.Save(Path.Combine(Path.Combine(outdir, zonename), id + ".xml"));
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, ObjectData>> item in objectinis)
                IniFile.Serialize(item.Value, Path.Combine(outdir, item.Key == "FF" ? Path.GetFileName(filename) + ".ini" : Path.GetFileName(filename) + item.Key + ".ini"));
            return result;
        }

        static Bitmap LoadPCXFile(string filename)
        {
            Color[] palette;
            return BitmapBits.ReadPCX(filename, out palette).ToBitmap(palette);
        }
    }
}
