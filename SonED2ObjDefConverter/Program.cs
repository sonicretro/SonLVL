using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using SonicRetro.SonLVL.API;
using SonicRetro.SonLVL.API.XMLDef;

namespace SonED2ObjDefConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			string filename;
			if (args.Length > 0)
			{
				filename = args[0];
				Console.WriteLine("File: " + filename);
			}
			else
			{
				Console.Write("File: ");
				filename = Console.ReadLine();
			}
			Environment.CurrentDirectory = Path.GetDirectoryName(Path.Combine(Environment.CurrentDirectory, filename));
			string pcxfmt = Path.GetFileNameWithoutExtension(filename) + "{0}.pcx";
			string outdir = Path.Combine(Environment.CurrentDirectory, "Converted");
			if (!Directory.Exists(outdir)) Directory.CreateDirectory(outdir);
			List<string> data = new List<string>();
			foreach (string line in File.ReadAllLines(filename))
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
			for (int set = 0; set < numspritesets; set++)
			{
				int numsprites = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
				for (int sprite = 0; sprite < numsprites; sprite++)
				{
					Bitmap pcxfile;
					if (!pcxfiles.ContainsKey(data[linenum]))
						pcxfiles.Add(data[linenum], LoadPCXFile(string.Format(pcxfmt, data[linenum])));
					pcxfile = pcxfiles[data[linenum++]];
					pcxfile.Clone(new Rectangle(int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo)), pcxfile.PixelFormat).Save(Path.Combine(spritedir, "sprite" + set.ToString(NumberFormatInfo.InvariantInfo) + "-" + sprite.ToString(NumberFormatInfo.InvariantInfo) + ".png"));
				}
			}
			int numobjects = int.Parse(data[linenum++], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
			Dictionary<string, Dictionary<string, ObjectData>> objectinis = new Dictionary<string, Dictionary<string, ObjectData>>();
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
					ObjDef xmldata = new ObjDef() { Language = "cs", Namespace = "SonED2." + zonename, Image = "Image1" };
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
								string sprname = "sprite" + sprset.ToString(NumberFormatInfo.InvariantInfo) + "-" + sprnum.ToString(NumberFormatInfo.InvariantInfo) + ".png";
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
						xmldata.Images = new ImageList() { Items = new SonicRetro.SonLVL.API.XMLDef.Image[] { new ImageFromBitmap() { id = "Image1", filename = "Sprites/sprite0-1.png" } } };
						xmldata.Display = new Display() { DisplayOptions = new DisplayOption[] { new DisplayOption() { Images = new ImageRef[] { new ImageRef() { image = "Image1", Offset = new XmlPoint() { X = 8, Y = 7 } } } } } };
					}
					objectinis[zoneid][objid.ToString("X2")] = objdata;
					xmldata.Save(Path.Combine(Path.Combine(outdir, zonename), id + ".xml"));
				}
			}
			foreach (KeyValuePair<string, Dictionary<string, ObjectData>> item in objectinis)
				IniSerializer.Serialize(item.Value, Path.Combine(outdir, item.Key == "FF" ? "obj.ini" : "obj" + item.Key + ".ini"));
		}

		public static Bitmap LoadPCXFile(string filename)
		{
			Color[] palette;
			return BitmapBits.ReadPCX(filename, out palette).ToBitmap(palette);
		}
	}
}
