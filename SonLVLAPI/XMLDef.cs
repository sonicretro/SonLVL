using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace SonicRetro.SonLVL.API.XMLDef
{
	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class ObjDef
	{
		[XmlAttribute]
		public string Namespace { get; set; }
		[XmlAttribute]
		public string TypeName { get; set; }
		[XmlAttribute]
		public string Language { get; set; }
		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public string Image { get; set; }
		[XmlAttribute]
		public bool RememberState { get; set; }
		[XmlIgnore]
		public bool RememberStateSpecified { get { return !RememberState; } set { } }
		[XmlIgnore]
		public byte DefaultSubtypeValue { get; set; }
		[XmlAttribute]
		public string DefaultSubtype { get { return DefaultSubtypeValue.ToString("X2"); } set { DefaultSubtypeValue = byte.Parse(value, NumberStyles.HexNumber); } }
		[XmlIgnore]
		public bool DefaultSubtypeSpecified { get { return DefaultSubtypeValue != 0; } set { } }
		[XmlAttribute]
		public bool Debug { get; set; }
		[XmlIgnore]
		public bool DebugSpecified { get { return !Debug; } set { } }
		public ImageList Images { get; set; }
		public ImageSetList ImageSets { get; set; }
		public ImageRefList DefaultImage { get; set; }
		public SubtypeList Subtypes { get; set; }
		public PropertyList Properties { get; set; }
		public EnumList Enums { get; set; }
		public Display Display { get; set; }

		static readonly XmlSerializer xs = new XmlSerializer(typeof(ObjDef));

		public static ObjDef Load(string filename)
		{
			using (XmlTextReader xtr = new XmlTextReader(filename))
				return (ObjDef)xs.Deserialize(xtr);
		}

		public void Save(string filename)
		{
			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
			using (XmlTextWriter xtr = new XmlTextWriter(sw) { Formatting = Formatting.Indented })
				xs.Serialize(xtr, this);
		}
	}

	public class ImageList
	{
		[XmlElement("ImageFromMappings", typeof(ImageFromMappings))]
		[XmlElement("ImageFromBitmap", typeof(ImageFromBitmap))]
		[XmlElement("ImageFromSprite", typeof(ImageFromSprite))]
		public Image[] Items { get; set; }
	}

	public abstract class Image
	{
		[XmlAttribute]
		public string id { get; set; }
		[XmlAttribute]
		public bool priority { get; set; }
		public XmlPoint offset { get; set; }
		[XmlIgnore]
		public bool offsetSpecified { get { return !offset.IsEmpty; } set { } }
	}

	public struct XmlPoint
	{
		[XmlAttribute]
		public int X { get; set; }
		[XmlIgnore]
		public bool XSpecified { get { return X != 0; } set { } }
		[XmlAttribute]
		public int Y { get; set; }
		[XmlIgnore]
		public bool YSpecified { get { return Y != 0; } set { } }

		public XmlPoint(int x, int y)
			: this()
		{
			X = x;
			Y = y;
		}

		public bool IsEmpty { get { return X == 0 & Y == 0; } }

		public System.Drawing.Point ToPoint() { return new System.Drawing.Point(X, Y); }
	}

	public class ImageFromMappings : Image
	{
		[XmlElement("ArtFile")]
		public ArtFile[] ArtFiles { get; set; }
		public MapFile MapFile { get; set; }
	}

	public class ArtFile
	{
		[XmlAttribute]
		public string filename { get; set; }
		[XmlAttribute]
		public CompressionType compression { get; set; }
		[XmlIgnore]
		public bool compressionSpecified { get { return compression != CompressionType.Invalid; } set { } }
		[XmlAttribute]
		public int offset { get; set; }
		[XmlIgnore]
		public bool offsetSpecified { get; set; }
	}

	public class MapFile
	{
		[XmlAttribute]
		public MapFileType type { get; set; }
		[XmlAttribute]
		public string filename { get; set; }
		[XmlAttribute]
		public string label { get; set; }
		[XmlIgnore]
		public bool labelSpecified { get { return !string.IsNullOrEmpty(label); } set { } }
		[XmlAttribute]
		public string dplcfile { get; set; }
		[XmlIgnore]
		public bool dplcfileSpecified { get { return !string.IsNullOrEmpty(dplcfile); } set { } }
		[XmlAttribute]
		public string dplclabel { get; set; }
		[XmlIgnore]
		public bool dplclabelSpecified { get { return !string.IsNullOrEmpty(dplclabel); } set { } }
		[XmlAttribute]
		public int frame { get; set; }
		[XmlIgnore]
		public bool frameSpecified { get { return string.IsNullOrEmpty(label); } set { } }
		[XmlAttribute]
		public int startpal { get; set; }
		[XmlAttribute]
		public EngineVersion version { get; set; }
		[XmlIgnore]
		public bool versionSpecified { get { return version != EngineVersion.Invalid; } set { } }
		[XmlAttribute]
		public EngineVersion dplcver { get; set; }
		[XmlIgnore]
		public bool dplcverSpecified { get { return dplcver != EngineVersion.Invalid; } set { } }
	}

	public enum MapFileType
	{
		Binary,
		ASM
	}

	public class ImageFromBitmap : Image
	{
		[XmlAttribute]
		public string filename { get; set; }
	}

	public class ImageFromSprite : Image
	{
		[XmlAttribute]
		public int frame { get; set; }
	}

	public class ImageSetList
	{
		[XmlElement("ImageSet")]
		public ImageSet[] Items { get; set; }
	}

	public class ImageSet
	{
		[XmlAttribute]
		public string id { get; set; }
		[XmlElement("ImageRef")]
		public ImageRef[] Images { get; set; }
	}

	public class ImageRef
	{
		[XmlAttribute]
		public string image { get; set; }
		public XmlPoint Offset { get; set; }
		[XmlIgnore]
		public bool OffsetSpecified { get { return !Offset.IsEmpty; } set { } }
		[XmlAttribute]
		public FlipType xflip { get; set; }
		[XmlIgnore]
		public bool xflipSpecified { get { return xflip != FlipType.NormalFlip; } set { } }
		[XmlAttribute]
		public FlipType yflip { get; set; }
		[XmlIgnore]
		public bool yflipSpecified { get { return yflip != FlipType.NormalFlip; } set { } }
	}

	public enum FlipType
	{
		NormalFlip,
		ReverseFlip,
		NeverFlip,
		AlwaysFlip
	}

	public class SubtypeList
	{
		[XmlElement("Subtype")]
		public Subtype[] Items { get; set; }
	}

	public class Subtype : ImageRefList
	{
		[XmlIgnore]
		public byte subtype { get; set; }
		[XmlAttribute]
		public string id { get { return subtype.ToString("X2"); } set { subtype = byte.Parse(value, NumberStyles.HexNumber); } }
		[XmlAttribute]
		public string name { get; set; }
		[XmlAttribute]
		public string image { get; set; }
	}

	public class PropertyList
	{
		[XmlElement("CustomProperty", typeof(CustomProperty))]
		[XmlElement("BitsProperty", typeof(BitsProperty))]
		public Property[] Items { get; set; }
	}

	public abstract class Property
	{
		[XmlAttribute]
		public string name { get; set; }
		[XmlAttribute]
		public string displayname { get; set; }
		[XmlIgnore]
		public bool displaynameSpecified { get { return !string.IsNullOrEmpty(displayname); } set { } }
		[XmlAttribute]
		public string type { get; set; }
		[XmlAttribute]
		public string description { get; set; }
		[XmlIgnore]
		public bool descriptionSpecified { get { return !string.IsNullOrEmpty(description); } set { } }
	}

	public class CustomProperty : Property
	{
		public string get { get; set; }
		public string set { get; set; }
	}

	public class BitsProperty : Property
	{
		[XmlAttribute]
		public int startbit { get; set; }
		[XmlAttribute]
		public int length { get; set; }
	}

	public class EnumList
	{
		[XmlElement("Enum")]
		public Enum[] Items { get; set; }
	}

	public class Enum
	{
		[XmlAttribute]
		public string name { get; set; }
		[XmlElement("EnumMember")]
		public EnumMember[] Items { get; set; }
	}

	public class EnumMember
	{
		[XmlAttribute]
		public string name { get; set; }
		[XmlAttribute]
		public int value { get; set; }
		[XmlIgnore]
		public bool valueSpecified { get; set; }
	}

	public class Display
	{
		[XmlElement("DisplayOption")]
		public DisplayOption[] DisplayOptions { get; set; }
	}

	public class DisplayOption : ImageRefList
	{
		[XmlElement("Condition")]
		public Condition[] Conditions { get; set; }
		[XmlElement("Line")]
		public Line[] Lines { get; set; }
	}

	public class Condition
	{
		[XmlAttribute]
		public string property { get; set; }
		[XmlAttribute]
		public string value { get; set; }
	}

	public class ImageSetRef
	{
		[XmlAttribute]
		public string set { get; set; }
	}

	public class Line
	{
		[XmlAttribute]
		public byte color { get; set; }
		[XmlAttribute]
		public int x1 { get; set; }
		[XmlAttribute]
		public int y1 { get; set; }
		[XmlAttribute]
		public int x2 { get; set; }
		[XmlAttribute]
		public int y2 { get; set; }
	}

	public class ImageRefList
	{
		[XmlElement("ImageSetRef", typeof(ImageSetRef))]
		[XmlElement("ImageRef", typeof(ImageRef))]
		public object[] Images { get; set; }
	}


	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class StartPosDef
	{
		[XmlElement("ImageFromMappings", typeof(ImageFromMappings))]
		[XmlElement("ImageFromBitmap", typeof(ImageFromBitmap))]
		[XmlElement("ImageFromSprite", typeof(ImageFromSprite))]
		public Image[] Images { get; set; }

		static readonly XmlSerializer xs = new XmlSerializer(typeof(StartPosDef));

		public static StartPosDef Load(string filename)
		{
			using (XmlTextReader xtr = new XmlTextReader(filename))
				return (StartPosDef)xs.Deserialize(xtr);
		}

		public void Save(string filename)
		{
			using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
			using (XmlTextWriter xtr = new XmlTextWriter(sw) { Formatting = Formatting.Indented })
				xs.Serialize(xtr, this);
		}
	}
}
