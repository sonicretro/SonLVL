using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace SonicRetro.SonLVL.API.XMLDef
{
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
        [XmlAttribute]
        public bool Debug { get; set; }
        public ImageList Images { get; set; }
        public SubtypeList Subtypes { get; set; }
        public DataList Data { get; set; }
        public PropertyList Properties { get; set; }
        public EnumList Enums { get; set; }
        public Display Display { get; set; }

        public static ObjDef Load(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObjDef));
            XmlTextReader xtr = new XmlTextReader(filename);
            ObjDef result = (ObjDef)xs.Deserialize(xtr);
            xtr.Close();
            return result;
        }

        public void Save(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObjDef));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);
            XmlTextWriter xtr = new XmlTextWriter(sw);
            xs.Serialize(xtr, this);
            xtr.Close();
            sw.Close();
        }
    }

    public class ArtFile
    {
        [XmlAttribute]
        public string filename { get; set; }
        [XmlAttribute]
        public Compression.CompressionType compression { get; set; }
        [XmlAttribute]
        public int offset { get; set; }
        [XmlIgnore]
        public bool offsetSpecified { get; set; }
    }

    public class MapFileBin
    {
        [XmlAttribute]
        public string filename { get; set; }
        [XmlAttribute]
        public string dplcfile { get; set; }
        [XmlAttribute]
        public int frame { get; set; }
        [XmlAttribute]
        public int startpal { get; set; }
        [XmlAttribute]
        public EngineVersion version { get; set; }
        [XmlAttribute]
        public EngineVersion dplcver { get; set; }
    }

    public class MapFileAsm
    {
        [XmlAttribute]
        public string filename { get; set; }
        [XmlAttribute]
        public string dplcfile { get; set; }
        [XmlAttribute]
        public string label { get; set; }
        [XmlAttribute]
        public string dplclabel { get; set; }
        [XmlAttribute]
        public int frame { get; set; }
        [XmlIgnore]
        public bool frameSpecified { get; set; }
        [XmlAttribute]
        public int startpal { get; set; }
        [XmlAttribute]
        public EngineVersion version { get; set; }
        [XmlAttribute]
        public EngineVersion dplcver { get; set; }
    }

    public class ImageList
    {
        [XmlElement("ImageFromMappings", typeof(ImageFromMappings))]
        [XmlElement("ImageFromBitmap", typeof(ImageFromBitmap))]
        [XmlElement("ImageFromSprite", typeof(ImageFromSprite))]
        public object[] Items { get; set; }
    }

    public class ImageFromMappings
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlElement("ArtFile")]
        public ArtFile[] ArtFiles { get; set; }
        [XmlElement("MapFileBin", typeof(MapFileBin))]
        [XmlElement("MapFileAsm", typeof(MapFileAsm))]
        public object mappings { get; set; }
        public XmlPoint offset { get; set; }
    }

    public class ImageFromBitmap
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string filename { get; set; }
        public XmlPoint offset { get; set; }
    }

    public class XmlPoint
    {
        [XmlAttribute]
        public int X { get; set; }
        [XmlAttribute]
        public int Y { get; set; }

        public System.Drawing.Point ToPoint() { return new System.Drawing.Point(X, Y); }
    }

    public class ImageFromSprite
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public int frame { get; set; }
        public XmlPoint offset { get; set; }
    }

    public class SubtypeList
    {
        [XmlElement("Subtype")]
        public Subtype[] Items { get; set; }
    }

    public class Subtype
    {
        [XmlIgnore]
        public byte subtype { get; set; }
        [XmlAttribute]
        public string id { get { return subtype.ToString("X2"); } set { subtype = byte.Parse(value, System.Globalization.NumberStyles.HexNumber); } }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string image { get; set; }
    }

    public class DataList
    {
        [XmlElement("DataArray", typeof(DataArray))]
        [XmlElement("DataFile", typeof(DataFile))]
        public object[] Items { get; set; }
    }

    public class DataArray
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlIgnore]
        public string[] Data { get; set; }
        [XmlText]
        public string _Text_
        {
            get
            {
                return string.Join(", ", Data);
            }
            set
            {
                string[] data = value.Split(',');
                for (int i = 0; i < data.Length; i++)
                    data[i] = data[i].Trim();
                Data = data;
            }
        }
    }

    public class DataFile
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string filename { get; set; }
    }

    public class PropertyList
    {
        [XmlElement("CustomProperty", typeof(CustomProperty))]
        [XmlElement("BitsProperty", typeof(BitsProperty))]
        public object[] Items { get; set; }
    }

    public class CustomProperty
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        public string get { get; set; }
        public string set { get; set; }
        [XmlAttribute]
        public bool @override { get; set; }
        [XmlAttribute]
        public string description { get; set; }
    }

    public class BitsProperty
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public int startbit { get; set; }
        [XmlAttribute]
        public int length { get; set; }
        [XmlAttribute]
        public string description { get; set; }
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
        public string SpriteRoutine { get; set; }
        public string BoundsRoutine { get; set; }
        [XmlElement("DisplayOption")]
        public DisplayOption[] DisplayOptions { get; set; }
    }

    public class DisplayOption
    {
        [XmlElement("Condition")]
        public Condition[] Conditions { get; set; }
        [XmlElement("ImageRef")]
        public ImageRef[] Images { get; set; }
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

    public class ImageRef
    {
        [XmlAttribute]
        public string image { get; set; }
        public XmlPoint Offset { get; set; }
        [XmlAttribute]
        public bool xflip { get; set; }
        [XmlIgnore]
        public bool xflipSpecified { get; set; }
        [XmlAttribute]
        public bool yflip { get; set; }
        [XmlIgnore]
        public bool yflipSpecified { get; set; }
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
}