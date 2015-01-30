using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace SonicRetro.SonLVL.API
{
    public struct SonLVLColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color RGBColor
        {
            get
            {
                return Color.FromArgb(R, G, B);
            }
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }

        public ushort MDColor
        {
            get
            {
                if (LevelData.Level != null && LevelData.Level.PaletteFormat == EngineVersion.SKC)
                    return (ushort)(((int)Math.Round(R / (double)0x11) & 0xF) | (((int)Math.Round(G / (double)0x11) & 0xF) << 4) | (((int)Math.Round(B / (double)0x11) & 0xF) << 8));
                else
                    return (ushort)((RGBToMD(R) << 1) | (RGBToMD(G) << 5) | (RGBToMD(B) << 9));
            }
            set
            {
                if (LevelData.Level != null && LevelData.Level.PaletteFormat == EngineVersion.SKC)
                {
                    R = (byte)((value & 0xF) * 0x11);
                    G = (byte)(((value >> 4) & 0xF) * 0x11);
                    B = (byte)(((value >> 8) & 0xF) * 0x11);
                }
                else
                {
                    R = MDColorTable[(value >> 1) & 7];
                    G = MDColorTable[(value >> 5) & 7];
                    B = MDColorTable[(value >> 9) & 7];
                }
            }
        }

        public SonLVLColor(byte red, byte green, byte blue)
            : this()
        {
            R = red;
            G = green;
            B = blue;
        }

        public SonLVLColor(Color color)
            : this()
        {
            RGBColor = color;
        }

        public SonLVLColor(ushort mdcolor)
            : this()
        {
            MDColor = mdcolor;
        }

        private static readonly byte[] MDColorTable = { 0, 0x24, 0x49, 0x6D, 0x92, 0xB6, 0xDB, 0xFF };

        private static int RGBToMD(byte RGB)
        {
            if (Array.IndexOf(MDColorTable, RGB) != -1)
                return Array.IndexOf(MDColorTable, RGB);
            int result = 0;
            int distance = 256;
            foreach (byte b in MDColorTable)
            {
                int temp = Math.Abs(RGB - b);
                if (temp < distance)
                {
                    distance = temp;
                    result = Array.IndexOf(MDColorTable, b);
                }
            }
            return result;
        }

        public static SonLVLColor[] Load(byte[] file, int address, int length, EngineVersion game)
        {
            SonLVLColor[] palfile = new SonLVLColor[length];
            if (game != EngineVersion.SCDPC)
                for (int pi = 0; pi < length; pi++)
                    palfile[pi] = new SonLVLColor(ByteConverter.ToUInt16(file, address + (pi * 2)));
            else
                for (int pi = 0; pi < length; pi++)
                    palfile[pi] = new SonLVLColor(file[address + (pi * 4)], file[address + (pi * 4) + 1], file[address + (pi * 4) + 2]);
            return palfile;
        }
    }

    public class PatternIndex
    {
        public bool Priority { get; set; }
        private byte _pal;
        public byte Palette
        {
            get
            {
                return _pal;
            }
            set
            {
                _pal = (byte)(value & 0x3);
            }
        }
        public bool XFlip { get; set; }
        public bool YFlip { get; set; }
        private ushort _ind;

        [Browsable(false)]
        public ushort Tile
        {
            get
            {
                return _ind;
            }
            set
            {
                _ind = (ushort)(value & 0x7FF);
            }
        }

        [DisplayName("Tile")]
        [Editor(typeof(TileEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string _Tile
        {
            get
            {
                return _ind.ToString("X4");
            }
            set
            {
                _ind = (ushort)(ushort.Parse(value, System.Globalization.NumberStyles.HexNumber) & 0x7FF);
            }
        }

        public static int Size { get { return 2; } }

        public PatternIndex() { }

        public PatternIndex(byte[] file, int address)
        {
            ushort val = ByteConverter.ToUInt16(file, address);
            Priority = (val & 0x8000) == 0x8000;
            Palette = (byte)((val >> 13) & 0x3);
            YFlip = (val & 0x1000) == 0x1000;
            XFlip = (val & 0x800) == 0x800;
            _ind = (ushort)(val & 0x7FF);
        }

        public byte[] GetBytes()
        {
            ushort val = _ind;
            if (XFlip) val |= 0x800;
            if (YFlip) val |= 0x1000;
            val |= (ushort)(Palette << 13);
            if (Priority) val |= 0x8000;
            return ByteConverter.GetBytes(val);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PatternIndex)) return false;
            PatternIndex other = (PatternIndex)obj;
            if (Priority != other.Priority) return false;
            if (Palette != other.Palette) return false;
            if (XFlip != other.XFlip) return false;
            if (YFlip != other.YFlip) return false;
            if (Tile != other.Tile) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(PatternIndex a, PatternIndex b)
        {
			if (object.ReferenceEquals(a, null))
				return object.ReferenceEquals(b, null);
			return a.Equals(b);
        }

        public static bool operator !=(PatternIndex a, PatternIndex b)
        {
			if (object.ReferenceEquals(a, null))
				return !object.ReferenceEquals(b, null);
            return !a.Equals(b);
        }
    }

    public class Block
    {
        public PatternIndex[,] Tiles { get; set; }

        public static int Size { get { return PatternIndex.Size * 4; } }

        public Block()
        {
            Tiles = new PatternIndex[2, 2];
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                    Tiles[x, y] = new PatternIndex();
        }

        public Block(byte[] file, int address)
        {
            Tiles = new PatternIndex[2, 2];
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                    Tiles[x, y] = new PatternIndex(file, address + ((x + (y * 2)) * PatternIndex.Size));
        }

        public byte[] GetBytes()
        {
            List<byte> val = new List<byte>();
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                    val.AddRange(Tiles[x, y].GetBytes());
            return val.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Block)) return false;
            Block other = (Block)obj;
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                    if (Tiles[x, y] != other.Tiles[x, y]) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum Solidity : byte
    {
        NotSolid = 0,
        TopSolid = 1,
        LRBSolid = 2,
        AllSolid = 3
    }

    public abstract class ChunkBlock
    {
        protected byte _so1;
        public Solidity Solid1
        {
            get
            {
                return (Solidity)_so1;
            }
            set
            {
                _so1 = (byte)(value & Solidity.AllSolid);
            }
        }

        public bool XFlip { get; set; }
        public bool YFlip { get; set; }
        protected ushort _ind;
        [Browsable(false)]
        public ushort Block
        {
            get
            {
                return _ind;
            }
            set
            {
                _ind = (ushort)(value & 0x3FF);
            }
        }

        [DisplayName("Block")]
        [Editor(typeof(BlockEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string _Block
        {
            get
            {
                return _ind.ToString("X4");
            }
            set
            {
                _ind = (ushort)(ushort.Parse(value, System.Globalization.NumberStyles.HexNumber) & 0x3FF);
            }
        }

        public static int Size { get { return 2; } }

        public abstract byte[] GetBytes();

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkBlock)) return false;
            ChunkBlock other = (ChunkBlock)obj;
            if (Solid1 != other.Solid1) return false;
            if (XFlip != other.XFlip) return false;
            if (YFlip != other.YFlip) return false;
            if (Block != other.Block) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class S2ChunkBlock : ChunkBlock
    {
        private byte _so2;
        public Solidity Solid2
        {
            get
            {
                return (Solidity)_so2;
            }
            set
            {
                _so2 = (byte)(value & Solidity.AllSolid);
            }
        }

        public S2ChunkBlock() { }

        public S2ChunkBlock(byte[] file, int address)
        {
            ushort val = ByteConverter.ToUInt16(file, address);
            _so2 = (byte)((val >> 14) & 0x3);
            _so1 = (byte)((val >> 12) & 0x3);
            YFlip = (val & 0x800) == 0x800;
            XFlip = (val & 0x400) == 0x400;
            _ind = (ushort)(val & 0x3FF);
        }

        public override byte[] GetBytes()
        {
            ushort val = _ind;
            if (XFlip) val |= 0x400;
            if (YFlip) val |= 0x800;
            val |= (ushort)(_so1 << 12);
            val |= (ushort)(_so2 << 14);
            return ByteConverter.GetBytes(val);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is S2ChunkBlock)) return false;
            if (!base.Equals(obj)) return false;
            S2ChunkBlock other = (S2ChunkBlock)obj;
            if (Solid2 != other.Solid2) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class S1ChunkBlock : ChunkBlock
    {
        public S1ChunkBlock() { }

        public S1ChunkBlock(byte[] file, int address)
        {
            ushort val = ByteConverter.ToUInt16(file, address);
            _so1 = (byte)((val >> 13) & 0x3);
            YFlip = (val & 0x1000) == 0x1000;
            XFlip = (val & 0x800) == 0x800;
            _ind = (ushort)(val & 0x3FF);
        }

        public override byte[] GetBytes()
        {
            ushort val = _ind;
            if (XFlip) val |= 0x800;
            if (YFlip) val |= 0x1000;
            val |= (ushort)(_so1 << 13);
            return ByteConverter.GetBytes(val);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is S1ChunkBlock)) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Chunk
    {
        public ChunkBlock[,] Blocks { get; set; }

        private int size;

        public static int Size { get { return ChunkBlock.Size * (int)Math.Pow(LevelData.chunksz / 16, 2); } }

        public Chunk()
        {
            size = LevelData.chunksz / 16;
            Blocks = new ChunkBlock[size, size];
            switch (LevelData.Level.ChunkFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                            Blocks[x, y] = new S1ChunkBlock();
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                            Blocks[x, y] = new S2ChunkBlock();
                    break;
            }
        }

        public Chunk(byte[] file, int address)
        {
            size = LevelData.chunksz / 16;
            Blocks = new ChunkBlock[size, size];
            switch (LevelData.Level.ChunkFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.SCD:
                case EngineVersion.SCDPC:
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                            Blocks[x, y] = new S1ChunkBlock(file, address + ((x + (y * size)) * ChunkBlock.Size));
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                            Blocks[x, y] = new S2ChunkBlock(file, address + ((x + (y * size)) * ChunkBlock.Size));
                    break;
            }
        }

        public byte[] GetBytes()
        {
            List<byte> val = new List<byte>();
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    val.AddRange(Blocks[x, y].GetBytes());
            return val.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Chunk)) return false;
            Chunk other = (Chunk)obj;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    if (Blocks[x, y] != other.Blocks[x, y]) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [TypeConverter(typeof(PositionConverter))]
    [Serializable]
    public class Position
    {
        [NonSerialized]
        private Entry ent;
        private ushort x, y;
        [Browsable(false)]
        public ushort X { get { if (ent != null) x = ent.X; return x; } set { x = value; if (ent != null) ent.X = value; } }
        [Browsable(false)]
        public ushort Y { get { if (ent != null) y = ent.Y; return y; } set { y = value; if (ent != null) ent.Y = value; } }

        [DisplayName("X")]
        [Description("The horizontal component of the position.")]
        public string XHex { get { return X.ToString("X4"); } set { X = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber); } }
        [DisplayName("Y")]
        [Description("The vertical component of the position.")]
        public string YHex { get { return Y.ToString("X4"); } set { Y = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber); } }

        public Position() { }

        public Position(Entry item)
        {
            ent = item;
            x = item.X;
            y = item.Y;
        }

        public Position(string data)
        {
            string[] a = data.Split(',');
            X = ushort.Parse(a[0], System.Globalization.NumberStyles.HexNumber);
            Y = ushort.Parse(a[1], System.Globalization.NumberStyles.HexNumber);
        }

        public Position(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return X.ToString("X4") + ", " + Y.ToString("X4");
        }

        public ushort[] ToArray()
        {
            ushort[] result = new ushort[2];
            result[0] = X;
            result[1] = Y;
            return result;
        }

        public ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        return;
                    case 1:
                        Y = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    public class PositionConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Position))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Position)
                return ((Position)value).ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return new Position((string)value);
            return base.ConvertFrom(context, culture, value);
        }
    }

    [Serializable]
    public abstract class Entry : IComparable<Entry>
    {
        [Browsable(false)]
        public ushort X { get; set; }
        [NonSerialized]
        protected Position pos;
        [Category("Standard")]
        [Description("The location of the item within the level.")]
        public Position Position
        {
            get
            {
                return pos;
            }
            set
            {
                X = value.X;
                Y = value.Y;
                pos = new Position(this);
            }
        }
        [Browsable(false)]
        public ushort Y { get; set; }

        [Category("Standard")]
        [Description("The hexadecimal representation of the item.")]
        public string Data
        {
            get
            {
                byte[] value = GetBytes();
                List<string> stuff = new List<string>();
                for (int i = 0; i < value.Length; i += 2)
                    stuff.Add(ByteConverter.ToUInt16(value, i).ToString("X4"));
                return string.Join(" ", stuff.ToArray());
            }
            set
            {
                string data = string.Empty;
                foreach (char item in value)
                    if (!char.IsWhiteSpace(item))
                        data += item;
                byte[] bytes = GetBytes();
                data = data.PadRight(bytes.Length * 2, '0');
                data = data.Substring(0, bytes.Length * 2);
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = byte.Parse(data.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                FromBytes(bytes);
            }
        }

        public abstract byte[] GetBytes();

        public abstract void FromBytes(byte[] bytes);

        [Browsable(false)]
        public Sprite Sprite { get; protected set; }

        public abstract void UpdateSprite();

        [ReadOnly(true)]
        [ParenthesizePropertyName(true)]
        [Category("Meta")]
        [Description("The name of the item.")]
        public abstract string Name { get; }

        public void ResetPos() { pos = new Position(this); }

        int IComparable<Entry>.CompareTo(Entry other)
        {
            int c = X.CompareTo(other.X);
            if (c == 0) c = Y.CompareTo(other.Y);
            return c;
        }
    }

    [Serializable]
    public abstract class ObjectEntry : Entry, IComparable<ObjectEntry>, ICustomTypeDescriptor
    {
        [DefaultValue(false)]
        [Description("Flips the object vertically.")]
        [DisplayName("Y Flip")]
        public virtual bool YFlip { get; set; }
        [DefaultValue(false)]
        [Description("Flips the object horizontally.")]
        [DisplayName("X Flip")]
        public virtual bool XFlip { get; set; }
        [Browsable(false)]
        public virtual byte ID { get; set; }
        [DefaultValue("00")]
        [Description("The ID number of the object.")]
        [DisplayName("ID")]
        [Editor(typeof(IDEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public virtual string _ID
        {
            get
            {
                return ID.ToString("X2");
            }
            set
            {
                ID = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }
        [Browsable(false)]
        public virtual byte SubType { get; set; }
        [DefaultValue("00")]
        [Description("The subtype of the object.")]
        [DisplayName("Subtype")]
        [Editor(typeof(SubTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public virtual string _SubType
        {
            get
            {
                return SubType.ToString("X2");
            }
            set
            {
                SubType = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }

        protected bool isLoaded = false;

        int IComparable<ObjectEntry>.CompareTo(ObjectEntry other)
        {
            int c = X.CompareTo(other.X);
            if (c == 0) c = Y.CompareTo(other.Y);
            return c;
        }

        public override void UpdateSprite()
        {
            Sprite = LevelData.GetObjectDefinition(ID).GetSprite(this);
        }

        public override string Name
        {
            get
            {
                string ret = LevelData.unkobj.Name;
                if (LevelData.ObjTypes.ContainsKey(ID))
                    ret = LevelData.ObjTypes[ID].Name;
                return ret;
            }
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection result = TypeDescriptor.GetProperties(this, attributes, true);

            ObjectDefinition objdef = LevelData.GetObjectDefinition(ID);
            if (objdef.CustomProperties == null || objdef.CustomProperties.Length == 0) return result;
            List<PropertyDescriptor> props = new List<PropertyDescriptor>(result.Count);
            foreach (PropertyDescriptor item in result)
                props.Add(item);

            foreach (PropertySpec property in objdef.CustomProperties)
            {
                List<Attribute> attrs = new List<Attribute>();

                // Additionally, append the custom attributes associated with the
                // PropertySpec, if any.
                if (property.Attributes != null)
                    attrs.AddRange(property.Attributes);

                // Create a new property descriptor for the property item, and add
                // it to the list.
                PropertySpecDescriptor pd = new PropertySpecDescriptor(property,
                    property.Name, attrs.ToArray());
                props.Add(pd);
            }

            return new PropertyDescriptorCollection(props.ToArray(), true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }

    [DefaultProperty("ID")]
    [Serializable]
    public class S2ObjectEntry : ObjectEntry
    {
        [DefaultValue(false)]
        [Description("If true, the object will stay destroyed after it leaves the screen.")]
        [DisplayName("Remember State")]
        public virtual bool RememberState { get; set; }

        [DefaultValue(false)]
        [DisplayName("Long Distance")]
        public bool LongDistance { get; set; }

        public static int Size { get { return 6; } }

        public S2ObjectEntry() { pos = new Position(this); isLoaded = true; }

        public S2ObjectEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
            isLoaded = true;
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(X));
            ushort val = (ushort)(Y & 0xFFF);
            if (LongDistance) val |= 0x1000;
            if (XFlip) val |= 0x2000;
            if (YFlip) val |= 0x4000;
            if (RememberState) val |= 0x8000;
            ret.AddRange(ByteConverter.GetBytes(val));
            ret.Add(ID);
            ret.Add(SubType);
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            X = ByteConverter.ToUInt16(bytes, 0);
            ushort val = ByteConverter.ToUInt16(bytes, 2);
            RememberState = (val & 0x8000) == 0x8000;
            YFlip = (val & 0x4000) == 0x4000;
            XFlip = (val & 0x2000) == 0x2000;
            LongDistance = (val & 0x1000) == 0x1000;
            Y = (ushort)(val & 0xFFF);
            ID = bytes[4];
            SubType = bytes[5];
        }
    }

    [DefaultProperty("ID")]
    [Serializable]
    public class S2NAObjectEntry : S2ObjectEntry
    {
        public S2NAObjectEntry() { pos = new Position(this); isLoaded = true; }

        public S2NAObjectEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
            isLoaded = true;
        }

        public override byte ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = (byte)(value & 0x7F);
            }
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(X));
            ushort val = (ushort)(Y & 0xFFF);
            if (LongDistance) val |= 0x2000;
            if (XFlip) val |= 0x4000;
            if (YFlip) val |= 0x8000;
            ret.AddRange(ByteConverter.GetBytes(val));
            ret.Add((byte)(ID | (RememberState ? 0x80 : 0)));
            ret.Add(SubType);
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            X = ByteConverter.ToUInt16(bytes, 0);
            ushort val = ByteConverter.ToUInt16(bytes, 2);
            YFlip = (val & 0x8000) == 0x8000;
            XFlip = (val & 0x4000) == 0x4000;
            LongDistance = (val & 0x2000) == 0x2000;
            Y = (ushort)(val & 0xFFF);
            ID = bytes[4];
            RememberState = (bytes[4] & 0x80) == 0x80;
            SubType = bytes[5];
        }
    }

    [DefaultProperty("ID")]
    [Serializable]
    public class S1ObjectEntry : ObjectEntry
    {
        [DefaultValue(false)]
        [Description("If true, the object will stay destroyed after it leaves the screen.")]
        [DisplayName("Remember State")]
        public virtual bool RememberState { get; set; }

        public override string _ID
        {
            get
            {
                return ID.ToString("X2");
            }
            set
            {
                ID = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }

        public override byte ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = (byte)(value & 0x7F);
            }
        }

        public static int Size { get { return 6; } }

        public S1ObjectEntry() { pos = new Position(this); isLoaded = true; }

        public S1ObjectEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
            isLoaded = true;
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(X));
            ushort val = (ushort)(Y & 0xFFF);
            if (XFlip) val |= 0x4000;
            if (YFlip) val |= 0x8000;
            ret.AddRange(ByteConverter.GetBytes(val));
            ret.Add((byte)(ID | (RememberState ? 0x80 : 0)));
            ret.Add(SubType);
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            X = ByteConverter.ToUInt16(bytes, 0);
            ushort val = ByteConverter.ToUInt16(bytes, 2);
            YFlip = (val & 0x8000) == 0x8000;
            XFlip = (val & 0x4000) == 0x4000;
            Y = (ushort)(val & 0xFFF);
            ID = bytes[4];
            RememberState = (bytes[4] & 0x80) == 0x80;
            SubType = bytes[5];
        }
    }

    [DefaultProperty("ID")]
    [Serializable]
    public class S3KObjectEntry : ObjectEntry
    {
        [DefaultValue(false)]
        [Description("If true, the object will be loaded when it is in horizontal range of the screen, regardless of its Y position.")]
        [DisplayName("Make object manager ignore Y position")]
        public virtual bool SomeFlag { get; set; }

        public static int Size { get { return 6; } }

        public S3KObjectEntry() { pos = new Position(this); isLoaded = true; }

        public S3KObjectEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
            isLoaded = true;
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(X));
            ushort val = (ushort)(Y & 0xFFF);
            if (XFlip) val |= 0x2000;
            if (YFlip) val |= 0x4000;
            if (SomeFlag) val |= 0x8000;
            ret.AddRange(ByteConverter.GetBytes(val));
            ret.Add(ID);
            ret.Add(SubType);
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            X = ByteConverter.ToUInt16(bytes, 0);
            ushort val = ByteConverter.ToUInt16(bytes, 2);
            SomeFlag = (val & 0x8000) == 0x8000;
            YFlip = (val & 0x4000) == 0x4000;
            XFlip = (val & 0x2000) == 0x2000;
            Y = (ushort)(val & 0xFFF);
            ID = bytes[4];
            SubType = bytes[5];
        }
    }

    [DefaultProperty("ID")]
    [Serializable]
    public class SCDObjectEntry : ObjectEntry
    {
        [DefaultValue(false)]
        [Description("If true, the object will stay destroyed after it leaves the screen.")]
        [DisplayName("Remember State")]
        public virtual bool RememberState { get; set; }

        [Description("If true, the object should be loaded in the Present time zone.")]
        [DisplayName("Show in Present")]
        public virtual bool ShowPresent { get; set; }
        [Description("If true, the object should be loaded in the Past time zone.")]
        [DisplayName("Show in Past")]
        public virtual bool ShowPast { get; set; }
        [Description("If true, the object should be loaded in the Future time zone.")]
        [DisplayName("Show in Future")]
        public virtual bool ShowFuture { get; set; }

        public override string _ID
        {
            get
            {
                return ID.ToString("X2");
            }
            set
            {
                ID = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }

        public override byte ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = (byte)(value & 0x7F);
            }
        }

        public static int Size { get { return 8; } }

        public SCDObjectEntry() { pos = new Position(this); isLoaded = true; }

        public SCDObjectEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
            isLoaded = true;
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(X));
            ushort val = (ushort)(Y & 0xFFF);
            if (XFlip) val |= 0x4000;
            if (YFlip) val |= 0x8000;
            ret.AddRange(ByteConverter.GetBytes(val));
            ret.Add((byte)(ID | (RememberState ? 0x80 : 0)));
            ret.Add(SubType);
            byte b = 0;
            if (ShowPresent) b |= 0x40;
            if (ShowPast) b |= 0x80;
            if (ShowFuture) b |= 0x20;
            ret.Add(b);
            ret.Add(0);
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            X = ByteConverter.ToUInt16(bytes, 0);
            ushort val = ByteConverter.ToUInt16(bytes, 2);
            YFlip = (val & 0x8000) == 0x8000;
            XFlip = (val & 0x4000) == 0x4000;
            Y = (ushort)(val & 0xFFF);
            ID = bytes[4];
            RememberState = (bytes[4] & 0x80) == 0x80;
            SubType = bytes[5];
            ShowPresent = (bytes[6] & 0x40) == 0x40;
            ShowPast = (bytes[6] & 0x80) == 0x80;
            ShowFuture = (bytes[6] & 0x20) == 0x20;
        }
    }

	[DefaultProperty("ID")]
	[Serializable]
	public class ChaotixObjectEntry : ObjectEntry
	{
		public override byte SubType
		{
			get
			{
				return (byte)fullSubType;
			}
			set
			{
				fullSubType = value;
			}
		}

		private ushort fullSubType;
		public ushort FullSubType
		{
			get { return fullSubType; }
			set { fullSubType = (ushort)(value & 0x1FFF); }
		}

		[DefaultValue("00")]
		[Description("The subtype of the object.")]
		[DisplayName("Subtype")]
		[Editor(typeof(SubTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public override string _SubType
		{
			get { return FullSubType.ToString("X4"); }
			set { FullSubType = byte.Parse(value, System.Globalization.NumberStyles.HexNumber); }
		}
		[DefaultValue(false)]
		[Description("If true, the object will be loaded when it is in horizontal range of the screen, regardless of its Y position.")]
		[DisplayName("Make object manager ignore Y position")]
		public virtual bool SomeFlag { get; set; }

		public static int Size { get { return 8; } }

		public ChaotixObjectEntry() { pos = new Position(this); isLoaded = true; }

		/* Format: 
		8 bytes per entry:
			x_pos (word)
			y_pos (word)
			4*ID (word)
			secondary subtype (byte), highest 3 bits are No-Y-Check, Y-Flip, X-Flip
			subtype (byte)
		*/

		public ChaotixObjectEntry(byte[] file, int address)
		{
			byte[] bytes = new byte[Size];
			Array.Copy(file, address, bytes, 0, Size);
			FromBytes(bytes);
			pos = new Position(this);
			isLoaded = true;
		}

		public override byte[] GetBytes()
		{
			List<byte> ret = new List<byte>();
			ret.AddRange(ByteConverter.GetBytes(X));
			ret.AddRange(ByteConverter.GetBytes(Y));
			ret.AddRange(ByteConverter.GetBytes(ID << 2));
			ushort val = fullSubType;
			if (XFlip) val |= 0x2000;
			if (YFlip) val |= 0x4000;
			if (SomeFlag) val |= 0x8000;
			ret.AddRange(ByteConverter.GetBytes(val));
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			Y = ByteConverter.ToUInt16(bytes, 2);
			ID = (byte)(ByteConverter.ToUInt16(bytes, 4) >> 2);

			ushort val = ByteConverter.ToUInt16(bytes, 6);

			SomeFlag = (val & 0x8000) == 0x8000;
			YFlip = (val & 0x4000) == 0x4000;
			XFlip = (val & 0x2000) == 0x2000;
			fullSubType = (ushort)(val & 0x1FFF);
		}
	}
	
	[Serializable]
    public abstract class RingEntry : Entry, IComparable<RingEntry>
    {
        int IComparable<RingEntry>.CompareTo(RingEntry other)
        {
            int c = X.CompareTo(other.X);
            if (c == 0) c = Y.CompareTo(other.Y);
            return c;
        }

        public override string Name
        {
            get { return "Ring"; }
        }
    }

    public enum Direction
    {
        Horizontal,
        Vertical
    }

    [DefaultProperty("ID")]
    [Serializable]
    public class CNZBumperEntry : Entry, IComparable<CNZBumperEntry>
    {
        [Browsable(false)]
        public ushort ID { get; set; }

        [DefaultValue("0000")]
        [Description("The type of bumper.")]
        [DisplayName("ID")]
        public string _ID
        {
            get
            {
                return ID.ToString("X4");
            }
            set
            {
                ID = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }

        public static int Size { get { return 6; } }

        public CNZBumperEntry() { pos = new Position(this); }

        public CNZBumperEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(ID));
            ret.AddRange(ByteConverter.GetBytes(X));
            ret.AddRange(ByteConverter.GetBytes(Y));
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            ID = ByteConverter.ToUInt16(bytes, 0);
            X = ByteConverter.ToUInt16(bytes, 2);
            Y = ByteConverter.ToUInt16(bytes, 4);
        }

        int IComparable<CNZBumperEntry>.CompareTo(CNZBumperEntry other)
        {
            int c = X.CompareTo(other.X);
            if (c == 0) c = Y.CompareTo(other.Y);
            return c;
        }

        public override void UpdateSprite()
        {
            Sprite = LevelData.unkobj.GetSprite(new S2ObjectEntry() { X = X, Y = Y });
        }

        public override string Name
        {
            get { return "Bumper"; }
        }
    }

    public class StartPositionEntry : Entry
    {
        public static int Size { get { return 4; } }

        public StartPositionEntry() { pos = new Position(this); }

        public StartPositionEntry(byte[] file, int address)
        {
            byte[] bytes = new byte[Size];
            Array.Copy(file, address, bytes, 0, Size);
            FromBytes(bytes);
            pos = new Position(this);
        }

        public override byte[] GetBytes()
        {
            List<byte> ret = new List<byte>();
            ret.AddRange(ByteConverter.GetBytes(X));
            ret.AddRange(ByteConverter.GetBytes(Y));
            return ret.ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            X = ByteConverter.ToUInt16(bytes, 0);
            Y = ByteConverter.ToUInt16(bytes, 2);
        }

        public override void UpdateSprite()
        {
            Sprite = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(this)].GetSprite(this);
        }

        public override string Name
        {
            get { return LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(this)].Name; }
        }
    }

    public class MappingsTile
    {
        public short Y { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public PatternIndex Tile { get; set; }
        public PatternIndex Tile2 { get; set; }
        public short X { get; set; }

        public static int Size(EngineVersion version) { switch (version) { case EngineVersion.S1: return 5; case EngineVersion.S2: case EngineVersion.S2NA: return 8; default: return 6; } }

        public MappingsTile(short xpos, short ypos, byte width, byte height, ushort tile, bool xflip, bool yflip, byte pal, bool pri)
        {
            X = xpos;
            Y = ypos;
            Width = width;
            Height = height;
            Tile = new PatternIndex() { Tile = tile, XFlip = xflip, YFlip = yflip, Palette = pal, Priority = pri };
            Tile2 = new PatternIndex() { Tile = (ushort)(tile >> 1), XFlip = xflip, YFlip = yflip, Palette = pal, Priority = pri };
        }

        public MappingsTile(short xpos, short ypos, byte width, byte height, ushort tile, bool xflip, bool yflip, byte pal, bool pri, ushort tile2, bool xflip2, bool yflip2, byte pal2, bool pri2)
            : this(xpos, ypos, width, height, tile, xflip, yflip, pal, pri)
        {
            Tile2 = new PatternIndex() { Tile = tile2, XFlip = xflip2, YFlip = yflip2, Palette = pal2, Priority = pri2 };
        }

        public MappingsTile(byte[] file, int address, EngineVersion version)
        {
            Y = unchecked((sbyte)file[address]);
            Width = (byte)(((file[address + 1] & 0xC) >> 2) + 1);
            Height = (byte)((file[address + 1] & 0x3) + 1);
            Tile = new PatternIndex(file, address + 2);
            Tile2 = new PatternIndex(file, address + 2);
            Tile2.Tile = (ushort)(Tile2.Tile >> 1);
            switch (version)
            {
                case EngineVersion.S1:
                    X = unchecked((sbyte)file[address + 4]);
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    Tile2 = new PatternIndex(file, address + 4);
                    X = ByteConverter.ToInt16(file, address + 6);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    X = ByteConverter.ToInt16(file, address + 4);
                    break;
            }
        }

        public byte[] GetBytes(EngineVersion version)
        {
            List<byte> result = new List<byte>(Size(version));
            result.Add(unchecked((byte)((sbyte)Y)));
            result.Add((byte)((((Width - 1) & 3) << 2) | ((Height - 1) & 3)));
            result.AddRange(Tile.GetBytes());
            switch (version)
            {
                case EngineVersion.S1:
                    result.Add(unchecked((byte)((sbyte)X)));
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    result.AddRange(Tile2.GetBytes());
                    goto case EngineVersion.S3K;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    result.AddRange(ByteConverter.GetBytes(X));
                    break;
            }
            return result.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MappingsTile)) return false;
            MappingsTile other = (MappingsTile)obj;
            if (X != other.X) return false;
            if (Y != other.Y) return false;
            if (Width != other.Width) return false;
            if (Height != other.Height) return false;
            if (Tile != other.Tile) return false;
            if (Tile2 != other.Tile2) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MappingsFrame
    {
        public string Name { get; set; }

        public List<MappingsTile> Tiles { get; set; }
        public MappingsTile this[int index] { get { return Tiles[index]; } set { Tiles[index] = value; } }

        public int TileCount { get { return Tiles.Count; } }

        public int Size(EngineVersion version) { return (TileCount * MappingsTile.Size(version)) + (version == EngineVersion.S1 ? 1 : 2); }

        public static NamedList<MappingsFrame> LoadASM(string file, EngineVersion version)
        {
            Dictionary<string, int> labels = new Dictionary<string, int>();
            byte[] bin = LevelData.ASMToBin(file, version, out labels);
            return new NamedList<MappingsFrame>(labels.GetKeyOrDefault(0, Path.GetFileNameWithoutExtension(file).MakeIdentifier()), Load(bin, version, labels));
        }

        public static List<MappingsFrame> Load(byte[] file, EngineVersion version)
        {
            return Load(file, version, null);
        }

        public static List<MappingsFrame> Load(byte[] file, EngineVersion version, Dictionary<string, int> labels)
        {
            int[] addresses = LevelData.GetOffsetList(file);
            List<MappingsFrame> result = new List<MappingsFrame>();
            foreach (int item in addresses)
            {
                string name = "map_" + item.ToString("X4");
                if (labels != null && labels.ContainsValue(item))
                    foreach (KeyValuePair<string, int> label in labels)
                        if (label.Value == item)
                            name = label.Key;
                result.Add(new MappingsFrame(file, item, version, name));
            }
            return result;
        }

        public MappingsFrame(byte[] file, int address, EngineVersion version, string name)
        {
            Name = name;
            int tileCount;
            if (version == EngineVersion.S1)
                tileCount = file[address];
            else
                tileCount = ByteConverter.ToUInt16(file, address);
            Tiles = new List<MappingsTile>(tileCount);
            for (int i = 0; i < tileCount; i++)
                Tiles.Add(new MappingsTile(file, (i * MappingsTile.Size(version)) + address + (version == EngineVersion.S1 ? 1 : 2), version));
        }

        public static void ToASM(string file, NamedList<MappingsFrame> frames, EngineVersion version, bool macros)
        {
            ToASM(file, frames.Name, frames, version, macros);
        }

        public static void ToASM(string file, string name, List<MappingsFrame> frames, EngineVersion version, bool macros)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                if (macros)
                {
                    writer.WriteLine(name + ":\tmappingsTable");
                    foreach (MappingsFrame frame in frames)
                        writer.WriteLine("\tmappingsTableEntry.w\t" + frame.Name);
                    writer.WriteLine();
                    List<string> writtenFrames = new List<string>();
                    unchecked
                    {
                        foreach (MappingsFrame frame in frames)
                        {
                            if (writtenFrames.Contains(frame.Name)) continue;
                            writtenFrames.Add(frame.Name);
                            writer.WriteLine(frame.Name + ":\tspriteHeader");
                            for (int i = 0; i < frame.TileCount; i++)
                            {
                                MappingsTile tile = frame[i];
                                bool s2p = tile.Tile2.Equals(tile.Tile.Tile);
                                writer.Write("\tspritePiece" + (s2p ? "2P" : string.Empty) + "\t");
                                writer.Write(tile.X.ToHex68k());
                                writer.Write(", " + tile.Y.ToHex68k());
                                writer.Write(", " + tile.Width.ToHex68k());
                                writer.Write(", " + tile.Height.ToHex68k());
                                writer.Write(", " + tile.Tile.Tile.ToHex68k());
                                writer.Write(", " + (tile.Tile.XFlip ? "1" : "0"));
                                writer.Write(", " + (tile.Tile.YFlip ? "1" : "0"));
                                writer.Write(", " + tile.Tile.Palette.ToHex68k());
                                writer.Write(", " + (tile.Tile.Priority ? "1" : "0"));
                                if (s2p)
                                {
                                    writer.Write(", " + tile.Tile2.Tile.ToHex68k());
                                    writer.Write(", " + (tile.Tile2.XFlip ? "1" : "0"));
                                    writer.Write(", " + (tile.Tile2.YFlip ? "1" : "0"));
                                    writer.Write(", " + tile.Tile2.Palette.ToHex68k());
                                    writer.Write(", " + (tile.Tile2.Priority ? "1" : "0"));
                                }
                                writer.WriteLine();
                            }
                            writer.WriteLine(frame.Name + "_End");
                            writer.WriteLine();
                        }
                    }
                }
                else
                {
                    List<string> writtenFrames = new List<string>();
                    writer.WriteLine(name + ":");
                    foreach (MappingsFrame frame in frames)
                        writer.WriteLine("\tdc.w\t" + frame.Name + "-" + name);
                    writer.WriteLine();
                    unchecked
                    {
                        foreach (MappingsFrame frame in frames)
                        {
                            if (writtenFrames.Contains(frame.Name)) continue;
                            writtenFrames.Add(frame.Name);
                            writer.WriteLine(frame.Name + ":\tdc." + (version == EngineVersion.S1 ? "b " + ((byte)frame.TileCount).ToHex68k() : "w " + ((ushort)frame.TileCount).ToHex68k()));
                            for (int i = 0; i < frame.TileCount; i++)
                            {
                                byte[] data = frame[i].GetBytes(version);
                                writer.Write("\tdc.");
                                switch (version)
                                {
                                    case EngineVersion.S1:
                                        writer.Write("b " + string.Join(", ", Array.ConvertAll(data, (a) => a.ToHex68k())));
                                        break;
                                    case EngineVersion.S2:
                                    case EngineVersion.S2NA:
                                        writer.Write("w " + ByteConverter.ToUInt16(data, 0).ToHex68k());
                                        for (int j = 1; j < 4; j++)
                                            writer.Write(", " + ByteConverter.ToUInt16(data, j * 2).ToHex68k());
                                        break;
                                    case EngineVersion.S3K:
                                        writer.Write("w " + ByteConverter.ToUInt16(data, 0).ToHex68k());
                                        for (int j = 1; j < 3; j++)
                                            writer.Write(", " + ByteConverter.ToUInt16(data, j * 2).ToHex68k());
                                        break;
                                }
                                writer.WriteLine();
                            }
                            writer.WriteLine();
                        }
                    }
                }
                writer.WriteLine("\teven");
                writer.Close();
            }

        }

        public static byte[] GetBytes(MappingsFrame[] maps, EngineVersion version)
        {
            int off = maps.Length * 2;
            List<short> offs = new List<short>(maps.Length);
            List<byte> mapbytes = new List<byte>();
            for (int i = 0; i < maps.Length; i++)
                if (i == 0 & maps[i].TileCount == 0)
                    offs.Add(0);
                else
                {
                    bool found = false;
                    for (int j = 0; j < i; j++)
                        if (maps[i].Equals(maps[j]))
                        {
                            found = true;
                            offs.Add(offs[j]);
                            break;
                        }
                    if (found) continue;
                    offs.Add((short)off);
                    mapbytes.AddRange(maps[i].GetBytes(version));
                    off = maps.Length * 2 + mapbytes.Count;
                }
            List<byte> result = new List<byte>(maps.Length * 2 + mapbytes.Count);
            foreach (short item in offs)
                result.AddRange(ByteConverter.GetBytes(item));
            result.AddRange(mapbytes);
            return result.ToArray();
        }

        public byte[] GetBytes(EngineVersion version)
        {
            List<byte> result = new List<byte>(Size(version));
            if (version == EngineVersion.S1)
                result.Add((byte)TileCount);
            else
                result.AddRange(ByteConverter.GetBytes((ushort)TileCount));
            foreach (MappingsTile tile in Tiles)
                result.AddRange(tile.GetBytes(version));
            return result.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MappingsFrame)) return false;
            MappingsFrame other = (MappingsFrame)obj;
            if (TileCount != other.TileCount) return false;
            for (int i = 0; i < TileCount; i++)
                if (this[i] != other[i]) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DPLCEntry
    {
        public byte TileCount { get; set; }
        public ushort TileNum { get; set; }

        public static int Size { get { return 2; } }

        public DPLCEntry(byte tiles, ushort offset)
        {
            TileCount = tiles;
            TileNum = offset;
        }

        public DPLCEntry(byte[] file, int address, EngineVersion version)
        {
            switch (version)
            {
                case EngineVersion.S1:
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    TileNum = ByteConverter.ToUInt16(file, address);
                    TileCount = (byte)((TileNum >> 12) + 1);
                    TileNum &= 0xFFF;
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    TileNum = ByteConverter.ToUInt16(file, address);
                    TileCount = (byte)((TileNum & 0xF) + 1);
                    TileNum = (ushort)(TileNum >> 4);
                    break;
            }
        }

        public byte[] GetBytes(EngineVersion version)
        {
            switch (version)
            {
                case EngineVersion.S1:
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    return ByteConverter.GetBytes((ushort)((((TileCount - 1) & 0xF) << 12) | (TileNum & 0xFFF)));
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    return ByteConverter.GetBytes((ushort)(((TileNum & 0xFFF) << 4) | ((TileCount - 1) & 0xF)));
            }
            throw new ArgumentOutOfRangeException("version");
        }
    }

    public class DPLCFrame
    {
        public string Name { get; set; }

        public List<DPLCEntry> Tiles { get; set; }
        public DPLCEntry this[int index] { get { return Tiles[index]; } set { Tiles[index] = value; } }

        public int Count { get { return Tiles.Count; } }

        public int Size(EngineVersion version) { return (Count * DPLCEntry.Size) + (version == EngineVersion.S1 ? 1 : 2); }

        public static NamedList<DPLCFrame> LoadASM(string file, EngineVersion version)
        {
            Dictionary<string, int> labels = new Dictionary<string, int>();
            byte[] bin = LevelData.ASMToBin(file, version, out labels);
            return new NamedList<DPLCFrame>(labels.GetKeyOrDefault(0, Path.GetFileNameWithoutExtension(file).MakeIdentifier()), Load(bin, version, labels));
        }

        public static DPLCFrame[] Load(byte[] file, EngineVersion version)
        {
            return Load(file, version, null);
        }

        public static DPLCFrame[] Load(byte[] file, EngineVersion version, Dictionary<string, int> labels)
        {
            int[] addresses = LevelData.GetOffsetList(file);
            List<DPLCFrame> result = new List<DPLCFrame>();
            foreach (int item in addresses)
            {
                string name = "dplc_" + item.ToString("X4");
                if (labels != null && labels.ContainsValue(item))
                    foreach (KeyValuePair<string, int> label in labels)
                        if (label.Value == item)
                            name = label.Key;
                result.Add(new DPLCFrame(file, item, version, name));
            }
            return result.ToArray();
        }

        public DPLCFrame(byte[] file, int address, EngineVersion version, string name)
        {
            try
            {
                Name = name;
                int tileCount = 0;
                switch (version)
                {
                    case EngineVersion.S1:
                        tileCount = file[address];
                        break;
                    case EngineVersion.S2NA:
                    case EngineVersion.S2:
                        tileCount = ByteConverter.ToUInt16(file, address);
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        tileCount = ByteConverter.ToUInt16(file, address) + 1;
                        break;
                }
                Tiles = new List<DPLCEntry>(tileCount);
                for (int i = 0; i < tileCount; i++)
                    Tiles.Add(new DPLCEntry(file, (i * DPLCEntry.Size) + address + (version == EngineVersion.S1 ? 1 : 2), version));
            }
            catch { }
        }

        public static void ToASM(string file, NamedList<DPLCFrame> frames, EngineVersion version, bool macros, bool s3kp)
        {
            ToASM(file, frames.Name, frames, version, macros, s3kp);
        }

        public static void ToASM(string file, string name, List<DPLCFrame> frames, EngineVersion version, bool macros, bool s3kp)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                if (macros)
                {
                    writer.WriteLine(name + ":\tmappingsTable");
                    foreach (DPLCFrame frame in frames)
                        writer.WriteLine("\tmappingsTableEntry.w\t" + frame.Name);
                    writer.WriteLine();
                    List<string> writtenFrames = new List<string>();
                    string dplcHeader = s3kp ? "s3kPlayerDplcHeader" : "dplcHeader";
                    string dplcEntry = s3kp ? "s3kPlayerDplcEntry" : "dplcEntry";
                    unchecked
                    {
                        foreach (DPLCFrame frame in frames)
                        {
                            if (writtenFrames.Contains(frame.Name)) continue;
                            writtenFrames.Add(frame.Name);
                            writer.WriteLine(frame.Name + ":\t" + dplcHeader);
                            for (int i = 0; i < frame.Count; i++)
                            {
                                DPLCEntry tile = frame[i];
                                writer.Write("\t" + dplcEntry + "\t");
                                writer.Write(tile.TileCount.ToHex68k());
                                writer.Write(", " + tile.TileNum.ToHex68k());
                                writer.WriteLine();
                            }
                            writer.WriteLine(frame.Name + "_End");
                            writer.WriteLine();
                        }
                    }
                }
                else
                {
                    List<string> writtenFrames = new List<string>();
                    writer.WriteLine(name + ":");
                    foreach (DPLCFrame frame in frames)
                        writer.WriteLine("\tdc.w\t" + frame.Name + "-" + name);
                    writer.WriteLine();
                    unchecked
                    {
                        foreach (DPLCFrame frame in frames)
                        {
                            if (writtenFrames.Contains(frame.Name)) continue;
                            writtenFrames.Add(frame.Name);
                            writer.Write(frame.Name + ":\tdc.");
                            switch (version)
                            {
                                case EngineVersion.S1:
                                    writer.WriteLine("b " + ((byte)frame.Count).ToHex68k());
                                    break;
                                case EngineVersion.S2:
                                    writer.WriteLine("w " + ((ushort)frame.Count).ToHex68k());
                                    break;
                                case EngineVersion.S3K:
                                    writer.WriteLine("w " + ((ushort)(frame.Count - 1)).ToHex68k());
                                    break;
                            }
                            for (int i = 0; i < frame.Count; i++)
                            {
                                byte[] data = frame[i].GetBytes(version);
                                writer.Write("\tdc.");
                                switch (version)
                                {
                                    case EngineVersion.S1:
                                        writer.Write("b " + data[0].ToHex68k() + ", " + data[1].ToHex68k());
                                        break;
                                    case EngineVersion.S2:
                                    case EngineVersion.S3K:
                                        writer.Write("w " + ByteConverter.ToUInt16(data, 0).ToHex68k());
                                        break;
                                }
                                writer.WriteLine();
                            }
                            writer.WriteLine();
                        }
                    }
                }
                writer.WriteLine("\teven");
                writer.Close();
            }
        }

        public static byte[] GetBytes(DPLCFrame[] dplcs, EngineVersion version)
        {
            int off = dplcs.Length * 2;
            List<short> offs = new List<short>(dplcs.Length);
            List<byte> mapbytes = new List<byte>();
            for (int i = 0; i < dplcs.Length; i++)
                if (i == 0 & dplcs[i].Count == 0 & version != EngineVersion.S3K & version != EngineVersion.SKC)
                    offs.Add(0);
                else
                {
                    bool found = false;
                    for (int j = 0; j < i; j++)
                        if (dplcs[i].Equals(dplcs[j]))
                        {
                            found = true;
                            offs.Add(offs[j]);
                            break;
                        }
                    if (found) continue;
                    offs.Add((short)off);
                    mapbytes.AddRange(dplcs[i].GetBytes(version));
                    off = dplcs.Length * 2 + mapbytes.Count;
                }
            List<byte> result = new List<byte>(dplcs.Length * 2 + mapbytes.Count);
            foreach (short item in offs)
                result.AddRange(ByteConverter.GetBytes(item));
            result.AddRange(mapbytes);
            return result.ToArray();
        }

        public byte[] GetBytes(EngineVersion version)
        {
            List<byte> result = new List<byte>(Size(version));
            switch (version)
            {
                case EngineVersion.S1:
                    result.Add((byte)Count);
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    result.AddRange(ByteConverter.GetBytes((ushort)Count));
                    break;
                case EngineVersion.S3K:
                    result.AddRange(ByteConverter.GetBytes((ushort)(Count - 1)));
                    break;
            }
            foreach (DPLCEntry tile in Tiles)
                result.AddRange(tile.GetBytes(version));
            return result.ToArray();
        }
    }

    public class Animation
    {
        public string Name { get; set; }

        public byte Speed { get; set; }

        public List<byte> Frames { get; set; }
        public byte this[int index] { get { return Frames[index]; } set { Frames[index] = value; } }

        public byte EndType { get; set; }

        public byte? ExtraParam { get; set; }

        public int Count { get { return Frames.Count; } }

        public int Size { get { return Count + 2 + (ExtraParam.HasValue ? 1 : 0); } }

        public static NamedList<Animation> LoadASM(string file) { return LoadASM(file, EngineVersion.S2); }

        public static NamedList<Animation> LoadASM(string file, EngineVersion version)
        {
            Dictionary<string, int> labels = new Dictionary<string, int>();
            byte[] bin = LevelData.ASMToBin(file, version, out labels);
            return new NamedList<Animation>(labels.GetKeyOrDefault(0, Path.GetFileNameWithoutExtension(file).MakeIdentifier()), Load(bin, labels));
        }

        public static List<Animation> Load(byte[] file)
        {
            return Load(file, null);
        }

        public static List<Animation> Load(byte[] file, Dictionary<string, int> labels)
        {
            int[] addresses = LevelData.GetOffsetList(file);
            List<Animation> result = new List<Animation>();
            foreach (int item in addresses)
            {
                string name = "ani_" + item.ToString("X4");
                if (labels != null && labels.ContainsValue(item))
                    foreach (KeyValuePair<string, int> label in labels)
                        if (label.Value == item)
                            name = label.Key;
                result.Add(new Animation(file, item, name));
            }
            return result;
        }

        public Animation(byte[] file, int address, string name)
        {
            Name = name;
            Frames = new List<byte>();
            Speed = file[address++];
            while (address < file.Length && file[address] < 0xF0)
                Frames.Add(file[address++]);
            if (address < file.Length)
                EndType = file[address++];
            switch (EndType)
            {
                case 0xFE:
                    ExtraParam = file[address++];
                    break;
                case 0xFD:
                    ExtraParam = file[address++];
                    break;
            }
        }

        public static void ToASM(string file, NamedList<Animation> anims, bool macros)
        {
            ToASM(file, anims.Name, anims, macros);
        }

        public static void ToASM(string file, string name, List<Animation> anims, bool macros)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                if (macros)
                {
                    writer.WriteLine(name + ":\tmappingsTable");
                    foreach (Animation anim in anims)
                        writer.WriteLine("\tmappingsTableEntry.w\t" + anim.Name);
                }
                else
                {
                    writer.WriteLine(name + ":");
                    foreach (Animation anim in anims)
                        writer.WriteLine("\tdc.w\t" + anim.Name + "-" + name);
                    writer.WriteLine();
                }
                List<string> writtenFrames = new List<string>();
                unchecked
                {
                    foreach (Animation anim in anims)
                    {
                        if (writtenFrames.Contains(anim.Name)) continue;
                        writtenFrames.Add(anim.Name);
                        writer.Write(anim.Name + ":\tdc.b ");
                        writer.WriteLine(string.Join(", ", Array.ConvertAll(anim.GetBytes(), (a) => a.ToHex68k())));
                        writer.WriteLine();
                    }
                }
                writer.WriteLine("\teven");
                writer.Close();
            }
        }

        public static byte[] GetBytes(Animation[] anims)
        {
            int off = anims.Length * 2;
            List<short> offs = new List<short>(anims.Length);
            List<byte> mapbytes = new List<byte>();
            for (int i = 0; i < anims.Length; i++)
            {
                bool found = false;
                for (int j = 0; j < i; j++)
                    if (anims[i].Equals(anims[j]))
                    {
                        found = true;
                        offs.Add(offs[j]);
                        break;
                    }
                if (found) continue;
                offs.Add((short)off);
                mapbytes.AddRange(anims[i].GetBytes());
                off = anims.Length * 2 + mapbytes.Count;
            }
            List<byte> result = new List<byte>(anims.Length * 2 + mapbytes.Count);
            foreach (short item in offs)
                result.AddRange(ByteConverter.GetBytes(item));
            result.AddRange(mapbytes);
            return result.ToArray();
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>(Size);
            result.Add(Speed);
            result.AddRange(Frames);
            result.Add(EndType);
            if (ExtraParam.HasValue)
                result.Add(ExtraParam.Value);
            return result.ToArray();
        }
    }

    [Serializable]
    public struct Sprite
    {
        public Point Offset;
        public BitmapBits Image;
        public int X { get { return Offset.X; } set { Offset.X = value; } }
        public int Y { get { return Offset.Y; } set { Offset.Y = value; } }
        public int Width { get { return Image.Width; } }
        public int Height { get { return Image.Height; } }
        public Size Size { get { return Image.Size; } }
        public int Left { get { return X; } }
        public int Top { get { return Y; } }
        public int Right { get { return X + Width; } }
        public int Bottom { get { return Y + Height; } }
        public Rectangle Bounds { get { return new Rectangle(Offset, Size); } }

        public Sprite(BitmapBits spr, Point off)
        {
            Image = spr;
            Offset = off;
        }

        public Sprite(Sprite sprite)
        {
            Image = new BitmapBits(sprite.Image);
            Offset = sprite.Offset;
        }

        public Sprite(params Sprite[] sprites)
            : this(new List<Sprite>(sprites))
        {
        }

        public Sprite(IEnumerable<Sprite> sprites)
        {
            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;
            foreach (Sprite spr in sprites)
                if (spr.Image != null)
                {
                    left = Math.Min(spr.Left, left);
                    right = Math.Max(spr.Right, right);
                    top = Math.Min(spr.Top, top);
                    bottom = Math.Max(spr.Bottom, bottom);
                }
            Offset = new Point(left, top);
            Image = new BitmapBits(right - left, bottom - top);
            foreach (Sprite spr in sprites)
                if (spr.Image != null)
                    Image.DrawBitmapComposited(spr.Image, new Point(spr.X - left, spr.Y - top));
        }
    }
}