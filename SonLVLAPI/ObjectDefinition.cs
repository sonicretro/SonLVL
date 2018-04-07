using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SonicRetro.SonLVL.API
{
	public class ObjectData
	{
		[IniName("codefile")]
		public string CodeFile;
		[IniName("codetype")]
		public string CodeType;
		[IniName("xmlfile")]
		public string XMLFile;
		[IniName("name")]
		[DefaultValue("Unknown")]
		public string Name;
		[IniName("art")]
		[IniCollection(IniCollectionMode.SingleLine, Format = "|")]
		public FileInfo[] Art;
		[IniName("artcmp")]
		public CompressionType ArtCompression;
		[IniName("map")]
		public string MapFile;
		[IniName("mapcmp")]
		[DefaultValue(CompressionType.Uncompressed)]
		public CompressionType MapCompression;
		[IniName("mapasm")]
		public string MapFileAsm;
		[IniName("mapasmlbl")]
		public string MapAsmLabel;
		[IniName("mapver")]
		public EngineVersion MapVersion;
		[IniName("dplc")]
		public string DPLCFile;
		[IniName("dplccmp")]
		[DefaultValue(CompressionType.Uncompressed)]
		public CompressionType DPLCCompression;
		[IniName("dplcasm")]
		public string DPLCFileAsm;
		[IniName("dplcasmlbl")]
		public string DPLCAsmLabel;
		[IniName("dplcver")]
		public EngineVersion DPLCVersion;
		[IniName("frame")]
		public int Frame;
		[IniName("pal")]
		public int Palette;
		[IniName("image")]
		public string Image;
		[IniName("sprite")]
		[DefaultValue(-1)]
		public int Sprite;
		[IniName("offset")]
		public Size Offset;
		[IniName("rememberstate")]
		public bool RememberState;
		[IniName("defaultsubtype")]
		[TypeConverter(typeof(ByteHexConverter))]
		public byte DefaultSubtype;
		[IniName("debug")]
		public bool Debug;
		[IniName("subtypes")]
		[IniCollection(IniCollectionMode.SingleLine, Format = ",", ValueConverter = typeof(ByteHexConverter))]
		public byte[] Subtypes;
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, string> CustomProperties;

		public ObjectData()
		{
			Sprite = -1;
		}

		public void Init()
		{
			if (DPLCVersion == EngineVersion.Invalid)
				DPLCVersion = MapVersion;
		}
	}

	public abstract class ObjectDefinition
	{
		public abstract void Init(ObjectData data);
		public abstract ReadOnlyCollection<byte> Subtypes { get; }
		public abstract string SubtypeName(byte subtype);
		public abstract Sprite SubtypeImage(byte subtype);
		public abstract string Name { get; }
		public virtual bool RememberState { get { return false; } }
		public virtual byte DefaultSubtype { get { return 0; } }
		public abstract Sprite Image { get; }
		public abstract Sprite GetSprite(ObjectEntry obj);
#pragma warning disable CS0618 // Type or member is obsolete
		public virtual Rectangle GetBounds(ObjectEntry obj) { return GetBounds(obj, Point.Empty); }
#pragma warning restore CS0618 // Type or member is obsolete
		[Obsolete("The two-argument version of this function is obsolete. Please change your code to use the single-argument version instead.")]
		public virtual Rectangle GetBounds(ObjectEntry obj, Point camera) { return Rectangle.Empty; }
		public virtual Sprite? GetDebugOverlay(ObjectEntry obj) { return null; }
		public virtual bool Debug { get { return false; } }
		static readonly PropertySpec[] specs = new PropertySpec[0];
		public virtual PropertySpec[] CustomProperties => specs;
	}

	/// <summary>
	/// Represents a single property in a PropertySpec.
	/// </summary>
	public class PropertySpec
	{
		private Attribute[] attributes;
		private string category;
		private object defaultValue;
		private string description;
		private string name;
		private Type type;
		private Type typeConverter;
		private Dictionary<string, int> @enum;
		private Func<ObjectEntry, object> getMethod;
		private Action<ObjectEntry, object> setMethod;

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
			: this(name, Type.GetType(type), category, description, defaultValue, getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
		{
			this.name = name;
			this.type = type;
			this.category = category;
			this.description = description;
			this.defaultValue = defaultValue;
			this.attributes = null;
			this.getMethod = getMethod;
			this.setMethod = setMethod;
		}

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, string typeConverter, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
			: this(name, Type.GetType(type), category, description, defaultValue, Type.GetType(typeConverter), getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The fully qualified name of the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, string typeConverter, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod) :
			this(name, type, category, description, defaultValue, Type.GetType(typeConverter), getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, Type typeConverter, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod) :
			this(name, Type.GetType(type), category, description, defaultValue, typeConverter, getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="typeConverter">The Type that represents the type of the type
		/// converter for this property.  This type must derive from TypeConverter.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, Type typeConverter, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod) :
			this(name, type, category, description, defaultValue, getMethod, setMethod)
		{
			this.typeConverter = typeConverter;
		}

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">The fully qualified name of the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="enum">The enumeration used by the property.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, string type, string category, string description, object defaultValue, Dictionary<string, int> @enum, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod) :
			this(name, Type.GetType(type), category, description, defaultValue, @enum, getMethod, setMethod) { }

		/// <summary>
		/// Initializes a new instance of the PropertySpec class.
		/// </summary>
		/// <param name="name">The name of the property displayed in the property grid.</param>
		/// <param name="type">A Type that represents the type of the property.</param>
		/// <param name="category">The category under which the property is displayed in the
		/// property grid.</param>
		/// <param name="description">A string that is displayed in the help area of the
		/// property grid.</param>
		/// <param name="defaultValue">The default value of the property, or null if there is
		/// no default value.</param>
		/// <param name="enum">The enumeration used by the property.</param>
		/// <param name="getMethod">The method called to get the value of the property.</param>
		/// <param name="setMethod">The method called to set the value of the property.</param>
		public PropertySpec(string name, Type type, string category, string description, object defaultValue, Dictionary<string, int> @enum, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod) :
			this(name, type, category, description, defaultValue, typeof(StringEnumConverter), getMethod, setMethod)
		{
			this.@enum = @enum;
		}

		/// <summary>
		/// Gets or sets a collection of additional Attributes for this property.  This can
		/// be used to specify attributes beyond those supported intrinsically by the
		/// PropertySpec class, such as ReadOnly and Browsable.
		/// </summary>
		public Attribute[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		/// <summary>
		/// Gets or sets the category name of this property.
		/// </summary>
		public string Category
		{
			get { return category; }
			set { category = value; }
		}

		/// <summary>
		/// Gets or sets the default value of this property.
		/// </summary>
		public object DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}

		/// <summary>
		/// Gets or sets the help text description of this property.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Gets or sets the name of this property.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the type of this property.
		/// </summary>
		public Type Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Gets or sets the type converter
		/// type for this property.
		/// </summary>
		public Type ConverterType
		{
			get { return typeConverter; }
			set { typeConverter = value; }
		}

		public object GetValue(ObjectEntry item)
		{
			return getMethod(item);
		}

		public void SetValue(ObjectEntry item, object value)
		{
			setMethod(item, value);
		}

		public Dictionary<string, int> Enumeration
		{
			get { return @enum; }
			set { @enum = value; }
		}
	}

	public class StringEnumConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(int))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
			{
				Dictionary<string, int> values = ((PropertySpecDescriptor)context.PropertyDescriptor).Enumeration;
				if (values.ContainsKey((string)value))
					return values[(string)value];
				else
					return int.Parse((string)value, culture);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is int)
			{
				Dictionary<string, int> values = ((PropertySpecDescriptor)context.PropertyDescriptor).Enumeration;
				if (values.ContainsValue((int)value))
					return values.GetKey((int)value);
				else
					return ((int)value).ToString(culture);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(((PropertySpecDescriptor)context.PropertyDescriptor).Enumeration.Keys);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}

	internal class PropertySpecDescriptor : PropertyDescriptor
	{
		private PropertySpec item;

		public PropertySpecDescriptor(PropertySpec item, string name, Attribute[] attrs) :
			base(name, attrs)
		{
			this.item = item;
		}

		public override Type ComponentType
		{
			get { return item.GetType(); }
		}

		public override bool IsReadOnly
		{
			get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
		}

		public override Type PropertyType
		{
			get { return item.Type; }
		}

		public override bool CanResetValue(object component)
		{
			if (item.DefaultValue == null)
				return false;
			else
				return !this.GetValue(component).Equals(item.DefaultValue);
		}

		public override object GetValue(object component)
		{
			return item.GetValue((ObjectEntry)component);
		}

		public override void ResetValue(object component)
		{
			SetValue(component, item.DefaultValue);
		}

		public override void SetValue(object component, object value)
		{
			item.SetValue((ObjectEntry)component, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			object val = this.GetValue(component);

			if (item.DefaultValue == null && val == null)
				return false;
			else
				return !val.Equals(item.DefaultValue);
		}

		public override TypeConverter Converter
		{
			get
			{
				if (item.ConverterType != null)
					return (TypeConverter)Activator.CreateInstance(item.ConverterType);
				return base.Converter;
			}
		}

		public override string Category { get { return item.Category; } }

		public override string Description { get { return item.Description; } }

		public Dictionary<string, int> Enumeration { get { return item.Enumeration; } }
	}

	public class DefaultObjectDefinition : ObjectDefinition
	{
		private Sprite spr;
		private string name;
		private bool rememberstate;
		private byte defsub;
		private List<byte> subtypes = new List<byte>();
		bool debug = false;

		public override void Init(ObjectData data)
		{
			name = data.Name ?? "Unknown";
			try
			{
				if (data.Art != null && data.Art.Length > 0)
				{
					MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
					foreach (FileInfo file in data.Art)
						art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(file.Filename, data.ArtCompression == CompressionType.Invalid ? LevelData.Game.ObjectArtCompression : data.ArtCompression)), file.Offset);
					byte[] artfile = art.ToArray();
					if (data.MapFile != null)
					{
						if (data.DPLCFile != null)
							spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.MapVersion, LevelData.ReadFile(data.DPLCFile, data.DPLCCompression), data.DPLCVersion, data.Frame, data.Palette);
						else
							spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.MapVersion);
					}
					else if (data.MapFileAsm != null)
					{
						if (data.MapAsmLabel != null)
						{
							if (data.DPLCFileAsm != null)
								spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.DPLCVersion, data.Palette);
							else
								spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.MapVersion);
						}
						else
						{
							if (data.DPLCFileAsm != null)
								spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.DPLCVersion, data.Frame, data.Palette);
							else
								spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.MapVersion);
						}
					}
					else
						spr = ObjectHelper.UnknownObject;
					if (data.Offset != Size.Empty)
						spr.Offset = spr.Offset + data.Offset;
				}
				else if (data.Image != null)
				{
					BitmapBits img = new BitmapBits(data.Image);
					spr = new Sprite(img, new Point(data.Offset));
					debug = true;
				}
				else if (data.Sprite > -1)
					spr = ObjectHelper.GetSprite(data.Sprite);
				else
				{
					spr = ObjectHelper.UnknownObject;
					debug = true;
				}
			}
			catch (Exception ex)
			{
				LevelData.Log("Error loading object definition " + name + ":", ex.ToString());
				spr = ObjectHelper.UnknownObject;
				debug = true;
			}
			spr = spr.Trim();
			rememberstate = data.RememberState;
			defsub = data.DefaultSubtype;
			debug = debug | data.Debug;
			if (data.Subtypes != null)
				subtypes.AddRange(data.Subtypes);
		}

		public override ReadOnlyCollection<byte> Subtypes { get { return new ReadOnlyCollection<byte>(subtypes); } }

		public override string SubtypeName(byte subtype) { return string.Empty; }

		public override Sprite SubtypeImage(byte subtype) { return Image; }

		public override string Name { get { return name; } }

		public override bool RememberState { get { return rememberstate; } }

		public override byte DefaultSubtype { get { return defsub; } }

		public override Sprite Image { get { return spr; } }

		public override Sprite GetSprite(ObjectEntry obj)
		{
			Sprite sprite = new Sprite(spr);
			sprite.Flip(obj.XFlip, obj.YFlip);
			sprite.X += obj.X;
			sprite.Y += obj.Y;
			return sprite;
		}

		public override bool Debug { get { return debug; } }
	}

	public class XMLObjectDefinition : ObjectDefinition
	{
		class PropertyInfo
		{
			private Type type;
			private Dictionary<string, int> @enum;
			private Func<ObjectEntry, object> getMethod;
			private Action<ObjectEntry, object> setMethod;

			/// <summary>
			/// Initializes a new instance of the PropertyInfo class.
			/// </summary>
			/// <param name="type">The fully qualified name of the type of the property.</param>
			/// <param name="getMethod">The method called to get the value of the property.</param>
			/// <param name="setMethod">The method called to set the value of the property.</param>
			public PropertyInfo(string type, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
				: this(Type.GetType(type), getMethod, setMethod) { }

			/// <summary>
			/// Initializes a new instance of the PropertyInfo class.
			/// </summary>
			/// <param name="type">A Type that represents the type of the property.</param>
			/// <param name="getMethod">The method called to get the value of the property.</param>
			/// <param name="setMethod">The method called to set the value of the property.</param>
			public PropertyInfo(Type type, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
			{
				this.type = type;
				this.getMethod = getMethod;
				this.setMethod = setMethod;
			}

			/// <summary>
			/// Initializes a new instance of the PropertyInfo class.
			/// </summary>
			/// <param name="type">The fully qualified name of the type of the property.</param>
			/// <param name="enum">The enumeration used by the property.</param>
			/// <param name="getMethod">The method called to get the value of the property.</param>
			/// <param name="setMethod">The method called to set the value of the property.</param>
			public PropertyInfo(string type, Dictionary<string, int> @enum, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
				: this(Type.GetType(type), @enum, getMethod, setMethod) { }

			/// <summary>
			/// Initializes a new instance of the PropertyInfo class.
			/// </summary>
			/// <param name="type">A Type that represents the type of the property.</param>
			/// <param name="enum">The enumeration used by the property.</param>
			/// <param name="getMethod">The method called to get the value of the property.</param>
			/// <param name="setMethod">The method called to set the value of the property.</param>
			public PropertyInfo(Type type, Dictionary<string, int> @enum, Func<ObjectEntry, object> getMethod, Action<ObjectEntry, object> setMethod)
				: this(type, getMethod, setMethod)
			{
				this.@enum = @enum;
			}

			/// <summary>
			/// Gets or sets the type of this property.
			/// </summary>
			public Type Type
			{
				get { return type; }
				set { type = value; }
			}

			public object GetValue(ObjectEntry item)
			{
				return getMethod(item);
			}

			public void SetValue(ObjectEntry item, object value)
			{
				setMethod(item, value);
			}

			public Dictionary<string, int> Enumeration { get { return @enum; } }
		}

		XMLDef.ObjDef xmldef;
		Dictionary<string, Sprite> images = new Dictionary<string, Sprite>();
		Dictionary<string, XMLDef.ImageRef[]> imagesets = new Dictionary<string, XMLDef.ImageRef[]>();
		Sprite unkobj;
		PropertySpec[] customProperties = new PropertySpec[0];
		Dictionary<string, PropertyInfo> propertyInfo = new Dictionary<string, PropertyInfo>();
		Dictionary<string, Dictionary<string, int>> enums;

		public override void Init(ObjectData data)
		{
			xmldef = XMLDef.ObjDef.Load(data.XMLFile);
			if (xmldef.Images != null && xmldef.Images.Items != null)
				foreach (XMLDef.Image item in xmldef.Images.Items)
				{
					Sprite sprite = default(Sprite);
					switch (item)
					{
						case XMLDef.ImageFromBitmap bmpimg:
							sprite = new Sprite(new BitmapBits(bmpimg.filename), bmpimg.offset.ToPoint());
							break;
						case XMLDef.ImageFromMappings mapimg:
							MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
							foreach (XMLDef.ArtFile artfile in mapimg.ArtFiles)
								art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(artfile.filename,
									artfile.compression == CompressionType.Invalid ? LevelData.Game.ObjectArtCompression : artfile.compression)),
									artfile.offsetSpecified ? artfile.offset : -1);
							XMLDef.MapFile map = mapimg.MapFile;
							switch (map.type)
							{
								case XMLDef.MapFileType.Binary:
									if (string.IsNullOrEmpty(map.dplcfile))
										sprite = ObjectHelper.MapToBmp(art.ToArray(), File.ReadAllBytes(map.filename),
											map.frame, map.startpal, map.version);
									else
										sprite = ObjectHelper.MapDPLCToBmp(art.ToArray(), File.ReadAllBytes(map.filename),
											File.ReadAllBytes(map.dplcfile), map.frame, map.startpal, map.version);
									break;
								case XMLDef.MapFileType.ASM:
									if (string.IsNullOrEmpty(map.label))
									{
										if (string.IsNullOrEmpty(map.dplcfile))
											sprite = ObjectHelper.MapASMToBmp(art.ToArray(), map.filename,
												map.frame, map.startpal, map.version);
										else
											sprite = ObjectHelper.MapASMDPLCToBmp(art.ToArray(), map.filename, map.version,
												map.dplcfile, map.dplcver, map.frame, map.startpal);
									}
									else
									{
										if (string.IsNullOrEmpty(map.dplcfile))
											sprite = ObjectHelper.MapASMToBmp(art.ToArray(), map.filename,
												map.label, map.startpal, map.version);
										else
											sprite = ObjectHelper.MapASMDPLCToBmp(art.ToArray(), map.filename, map.label, map.version,
												map.dplcfile, map.dplclabel, map.dplcver, map.startpal);
									}
									break;
							}
							if (!mapimg.offset.IsEmpty)
								sprite.Offset = new Point(sprite.X + mapimg.offset.X, sprite.Y + mapimg.offset.Y);
							break;
						case XMLDef.ImageFromSprite sprimg:
							sprite = ObjectHelper.GetSprite(sprimg.frame);
							if (!sprimg.offset.IsEmpty)
								sprite.Offset = new Point(sprite.X + sprimg.offset.X, sprite.Y + sprimg.offset.Y);
							break;
					}
					images.Add(item.id, sprite.Trim());
				}
			if (xmldef.ImageSets != null && xmldef.ImageSets.Items != null)
				foreach (XMLDef.ImageSet set in xmldef.ImageSets.Items)
					imagesets[set.id] = set.Images;
			if (xmldef.Subtypes == null)
				xmldef.Subtypes = new XMLDef.SubtypeList();
			if (xmldef.Subtypes.Items == null)
				xmldef.Subtypes.Items = new XMLDef.Subtype[0];
			if (xmldef.Enums != null && xmldef.Enums.Items != null)
			{
				enums = new Dictionary<string, Dictionary<string, int>>(xmldef.Enums.Items.Length);
				foreach (XMLDef.Enum item in xmldef.Enums.Items)
				{
					Dictionary<string, int> members = new Dictionary<string, int>(item.Items.Length);
					int value = 0;
					foreach (XMLDef.EnumMember mem in item.Items)
					{
						if (mem.valueSpecified)
							value = mem.value;
						members.Add(mem.name, value++);
					}
					enums.Add(item.name, members);
				}
			}
			else
				enums = new Dictionary<string, Dictionary<string, int>>();
			if (xmldef.Properties != null && xmldef.Properties.Items != null)
			{
				List<PropertySpec> custprops = new List<PropertySpec>(xmldef.Properties.Items.Length);
				Dictionary<string, PropertyInfo> propinf = new Dictionary<string, PropertyInfo>(xmldef.Properties.Items.Length);
				foreach (XMLDef.BitsProperty property in xmldef.Properties.Items.OfType<XMLDef.BitsProperty>())
				{
					int mask = 0;
					int prop_startbit = property.startbit;
					for (int i = 0; i < property.length; i++)
						mask |= 1 << (property.startbit + i);
					Func<ObjectEntry, object> getMethod;
					Action<ObjectEntry, object> setMethod;
					if (enums.ContainsKey(property.type))
					{
						getMethod = (obj) => (obj.SubType & mask) >> prop_startbit;
						setMethod = (obj, val) => obj.SubType = (byte)((obj.SubType & ~mask) | (((int)val << prop_startbit) & mask));
						custprops.Add(new PropertySpec(property.displayname ?? property.name, typeof(int), "Extended", property.description, null, enums[property.type], getMethod, setMethod));
						propinf.Add(property.name, new PropertyInfo(typeof(int), enums[property.type], getMethod, setMethod));
					}
					else
					{
						Type type = Type.GetType(LevelData.ExpandTypeName(property.type));
						if (type != typeof(bool))
						{
							getMethod = (obj) => (obj.SubType & mask) >> prop_startbit;
							setMethod = (obj, val) => obj.SubType = (byte)((obj.SubType & ~mask) | (((int)val << prop_startbit) & mask));
						}
						else
						{
							getMethod = (obj) => ((obj.SubType & mask) >> prop_startbit) != 0;
							setMethod = (obj, val) => obj.SubType = (byte)((obj.SubType & ~mask) | (((bool)val ? 1 : 0) << prop_startbit));
						}
						custprops.Add(new PropertySpec(property.displayname ?? property.name, type, "Extended", property.description, null, getMethod, setMethod));
						propinf.Add(property.name, new PropertyInfo(type, getMethod, setMethod));
					}
				}
				if (xmldef.Properties.Items.Any(a => a is XMLDef.CustomProperty))
				{
					Type functype = null;
					string fulltypename = xmldef.Namespace + "." + xmldef.TypeName;
					string dllfile = Path.Combine("dllcache", fulltypename + ".dll");
					DateTime modDate = DateTime.MinValue;
					if (File.Exists(dllfile))
						modDate = File.GetLastWriteTime(dllfile);
					if (modDate >= File.GetLastWriteTime(data.XMLFile) && modDate > File.GetLastWriteTime(System.Windows.Forms.Application.ExecutablePath))
					{
						LevelData.Log("Loading type from cached assembly \"" + dllfile + "\"...");
						functype = System.Reflection.Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(fulltypename);
					}
					else
					{
						LevelData.Log("Building code file...");
						CodeDomProvider pr = null;
						switch (xmldef.Language.ToLowerInvariant())
						{
							case "cs":
								pr = new Microsoft.CSharp.CSharpCodeProvider();
								break;
							case "vb":
								pr = new Microsoft.VisualBasic.VBCodeProvider();
								break;
#if false
								case "js":
									pr = new Microsoft.JScript.JScriptCodeProvider();
									break;
#endif
						}
						List<CodeTypeMember> members = new List<CodeTypeMember>();
						CodeMemberMethod method = new CodeMemberMethod();
						Type basetype = LevelData.ObjectFormat.ObjectType;
						foreach (XMLDef.CustomProperty item in xmldef.Properties.Items.OfType<XMLDef.CustomProperty>())
						{
							method = new CodeMemberMethod()
							{
								Attributes = MemberAttributes.Public | MemberAttributes.Static,
								Name = "Get" + item.name
							};
							method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(ObjectEntry), "_obj"));
							method.ReturnType = new CodeTypeReference(typeof(object));
							method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("_obj"))));
							method.Statements.Add(new CodeSnippetStatement(((XMLDef.CustomProperty)item).get));
							members.Add(method);
							method = new CodeMemberMethod()
							{
								Attributes = MemberAttributes.Public | MemberAttributes.Static,
								Name = "Set" + item.name
							};
							method.Parameters.AddRange(new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(ObjectEntry), "_obj"), new CodeParameterDeclarationExpression(typeof(object), "_val") });
							method.ReturnType = new CodeTypeReference(typeof(void));
							method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("_obj"))));
							if (enums.ContainsKey(item.type))
								method.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "value", new CodeCastExpression(typeof(int), new CodeArgumentReferenceExpression("_val"))));
							else
								method.Statements.Add(new CodeVariableDeclarationStatement(LevelData.ExpandTypeName(item.type), "value", new CodeCastExpression(LevelData.ExpandTypeName(item.type), new CodeArgumentReferenceExpression("_val"))));
							method.Statements.Add(new CodeSnippetStatement(((XMLDef.CustomProperty)item).set));
							members.Add(method);
						}
						CodeTypeDeclaration ctd = new CodeTypeDeclaration(xmldef.TypeName)
						{
							Attributes = MemberAttributes.Public | MemberAttributes.Static,
							IsClass = true
						};
						ctd.Members.AddRange(members.ToArray());
						CodeNamespace cn = new CodeNamespace(xmldef.Namespace);
						cn.Types.Add(ctd);
						cn.Imports.Add(new CodeNamespaceImport(typeof(LevelData).Namespace));
						CodeCompileUnit ccu = new CodeCompileUnit();
						ccu.Namespaces.Add(cn);
						ccu.ReferencedAssemblies.AddRange(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
						LevelData.Log("Compiling code file...");
						if (pr != null)
						{
#if DEBUG
							using (StreamWriter sw = new StreamWriter(Path.Combine("dllcache", xmldef.Namespace + "." + xmldef.TypeName + "." + pr.FileExtension)))
								pr.GenerateCodeFromCompileUnit(ccu, sw, new CodeGeneratorOptions() { BlankLinesBetweenMembers = true, BracingStyle = "C", VerbatimOrder = true });
#endif
							CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location })
							{
								GenerateExecutable = false,
								GenerateInMemory = false,
								IncludeDebugInformation = true,
								OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
							};
							CompilerResults res = pr.CompileAssemblyFromDom(para, ccu);
							if (res.Errors.HasErrors)
							{
								LevelData.Log("Compile failed.", "Errors:");
								foreach (CompilerError item in res.Errors)
									LevelData.Log(item.ToString());
								LevelData.Log(string.Empty);
							}
							else
							{
								LevelData.Log("Compile succeeded.");
								functype = res.CompiledAssembly.GetType(fulltypename);
							}
						}
					}
					if (functype != null)
						foreach (XMLDef.CustomProperty property in xmldef.Properties.Items.OfType<XMLDef.CustomProperty>())
						{
							Func<ObjectEntry, object> getMethod = (Func<ObjectEntry, object>)Delegate.CreateDelegate(typeof(Func<ObjectEntry, object>), functype.GetMethod("Get" + property.name));
							Action<ObjectEntry, object> setMethod = (Action<ObjectEntry, object>)Delegate.CreateDelegate(typeof(Action<ObjectEntry, object>), functype.GetMethod("Set" + property.name));
							if (enums.ContainsKey(property.type))
							{
								custprops.Add(new PropertySpec(property.displayname ?? property.name, typeof(int), "Extended", property.description, null, enums[property.type], getMethod, setMethod));
								propinf.Add(property.name, new PropertyInfo(typeof(int), enums[property.type], getMethod, setMethod));
							}
							else
							{
								Type type = Type.GetType(LevelData.ExpandTypeName(property.type));
								custprops.Add(new PropertySpec(property.displayname ?? property.name, type, "Extended", property.description, null, getMethod, setMethod));
								propinf.Add(property.name, new PropertyInfo(type, getMethod, setMethod));
							}
						}
				}
				customProperties = custprops.ToArray();
				propertyInfo = propinf;
			}
			unkobj = ObjectHelper.UnknownObject;
		}

		private Sprite ReadImageSet(XMLDef.ImageRef[] refs)
		{
			List<Sprite> sprs = new List<Sprite>(refs.Length);
			foreach (XMLDef.ImageRef img in refs)
				sprs.Add(ReadImageRef(img));
			return new Sprite(sprs);
		}

		private Sprite ReadImageRef(XMLDef.ImageRef img)
		{
			bool xflip = false, yflip = false;
			switch (img.xflip)
			{
				case XMLDef.FlipType.ReverseFlip:
				case XMLDef.FlipType.AlwaysFlip:
					xflip = true;
					break;
			}
			switch (img.yflip)
			{
				case XMLDef.FlipType.ReverseFlip:
				case XMLDef.FlipType.AlwaysFlip:
					yflip = true;
					break;
			}
			int xoff = img.Offset.X;
			if (xflip)
				xoff = -xoff;
			int yoff = img.Offset.Y;
			if (yflip)
				yoff = -yoff;
			Sprite sp = new Sprite(images[img.image]);
			sp.Flip(xflip, yflip);
			sp.X += xoff;
			sp.Y += yoff;
			return sp;
		}

		private Sprite ReadImageRefList(XMLDef.ImageRefList list)
		{
			List<Sprite> sprs = new List<Sprite>();
			foreach (object item in list.Images)
				switch (item)
				{
					case XMLDef.ImageSetRef set:
						sprs.Add(ReadImageSet(imagesets[set.set]));
						break;
					case XMLDef.ImageRef img:
						sprs.Add(ReadImageRef(img));
						break;
				}
			return new Sprite(sprs);
		}

		public override ReadOnlyCollection<byte> Subtypes
		{
			get { return new ReadOnlyCollection<byte>(Array.ConvertAll(xmldef.Subtypes.Items, (a) => a.subtype)); }
		}

		public override string SubtypeName(byte subtype)
		{
			foreach (XMLDef.Subtype item in xmldef.Subtypes.Items)
				if (item.subtype == subtype)
					return item.name;
			return string.Empty;
		}

		public override Sprite SubtypeImage(byte subtype)
		{
			foreach (XMLDef.Subtype item in xmldef.Subtypes.Items)
				if (item.subtype == subtype)
					if (item.Images != null)
						return ReadImageRefList(item);
					else if (item.image != null)
						return images[item.image];
					else
						return unkobj;
			return unkobj;
		}

		public override string Name
		{
			get { return xmldef.Name; }
		}

		public override Sprite Image
		{
			get
			{
				if (xmldef.DefaultImage != null && xmldef.DefaultImage.Images != null)
					return ReadImageRefList(xmldef.DefaultImage);
				else if (xmldef.Image != null)
					return images[xmldef.Image];
				else
					return SubtypeImage(DefaultSubtype);
			}
		}

		public override Sprite GetSprite(ObjectEntry obj)
		{
			if (xmldef.Display != null && xmldef.Display.DisplayOptions != null)
			{
				foreach (XMLDef.DisplayOption option in xmldef.Display.DisplayOptions)
				{
					if (!CheckConditions(obj, option))
						continue;
					if (option.Images != null)
						return ReadImageRefList(option, obj);
				}
			}
			else if (xmldef.Subtypes != null && xmldef.Subtypes.Items != null)
			{
				foreach (XMLDef.Subtype item in xmldef.Subtypes.Items)
				{
					if (obj.SubType == item.subtype)
						if (item.Images != null)
							return ReadImageRefList(item, obj);
						else
						{
							Sprite spr = new Sprite(images[item.image]);
							spr.Flip(obj.XFlip, obj.YFlip);
							spr.X += obj.X;
							spr.Y += obj.Y;
							return spr;
						}
				}
			}
			Sprite sprite;
			if (xmldef.DefaultImage != null && xmldef.DefaultImage.Images != null)
				return ReadImageRefList(xmldef.DefaultImage, obj);
			else if (xmldef.Image != null)
				sprite = new Sprite(images[xmldef.Image]);
			else
				sprite = new Sprite(unkobj);
			sprite.Flip(obj.XFlip, obj.YFlip);
			sprite.X += obj.X;
			sprite.Y += obj.Y;
			return sprite;
		}

		private Sprite ReadImageSet(XMLDef.ImageRef[] refs, ObjectEntry obj)
		{
			List<Sprite> sprs = new List<Sprite>(refs.Length);
			foreach (XMLDef.ImageRef img in refs)
				sprs.Add(ReadImageRef(img, obj));
			return new Sprite(sprs);
		}

		private Sprite ReadImageRef(XMLDef.ImageRef img, ObjectEntry obj)
		{
			bool xflip = false, yflip = false;
			switch (img.xflip)
			{
				case XMLDef.FlipType.NormalFlip:
					xflip = obj.XFlip;
					break;
				case XMLDef.FlipType.ReverseFlip:
					xflip = !obj.XFlip;
					break;
				case XMLDef.FlipType.AlwaysFlip:
					xflip = true;
					break;
			}
			switch (img.yflip)
			{
				case XMLDef.FlipType.NormalFlip:
					yflip = obj.YFlip;
					break;
				case XMLDef.FlipType.ReverseFlip:
					yflip = !obj.YFlip;
					break;
				case XMLDef.FlipType.AlwaysFlip:
					yflip = true;
					break;
			}
			int xoff = img.Offset.X;
			if (xflip)
				xoff = -xoff;
			int yoff = img.Offset.Y;
			if (yflip)
				yoff = -yoff;
			Sprite sp = new Sprite(images[img.image]);
			sp.Flip(xflip, yflip);
			sp.X += xoff + obj.X;
			sp.Y += yoff + obj.Y;
			return sp;
		}

		private Sprite ReadImageRefList(XMLDef.ImageRefList list, ObjectEntry obj)
		{
			List<Sprite> sprs = new List<Sprite>();
			foreach (object item in list.Images)
				switch (item)
				{
					case XMLDef.ImageSetRef set:
						sprs.Add(ReadImageSet(imagesets[set.set], obj));
						break;
					case XMLDef.ImageRef img:
						sprs.Add(ReadImageRef(img, obj));
						break;
				}
			return new Sprite(sprs);
		}

		private bool CheckConditions(ObjectEntry obj, XMLDef.DisplayOption option)
		{
			if (option.Conditions != null)
				foreach (XMLDef.Condition cond in option.Conditions)
				{
					if (propertyInfo.ContainsKey(cond.property))
					{
						PropertyInfo prop = propertyInfo[cond.property];
						object value = prop.GetValue(obj);
						if (prop.Enumeration != null)
						{
							if ((int)value != prop.Enumeration[cond.value])
								return false;
						}
						else
						{
							if (!object.Equals(value, prop.Type.InvokeMember("Parse", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, null, new[] { cond.value })))
								return false;
						}
					}
					else
					{
						System.Reflection.PropertyInfo prop = LevelData.ObjectFormat.ObjectType.GetProperty(cond.property);
						object value = prop.GetValue(obj, null);
						if (!object.Equals(value, prop.PropertyType.InvokeMember("Parse", System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, null, new[] { cond.value })))
							return false;
					}
				}
			return true;
		}

		public override bool Debug
		{
			get { return xmldef.Debug; }
		}

		public override bool RememberState
		{
			get { return xmldef.RememberState; }
		}

		public override byte DefaultSubtype
		{
			get { return xmldef.DefaultSubtypeValue; }
		}

		public override PropertySpec[] CustomProperties
		{
			get { return customProperties; }
		}
	}

	public class StartPositionDefinition
	{
		private Sprite spr;
		private string name;
		bool debug = false;

		public StartPositionDefinition(string name)
		{
			this.name = name;
			spr = ObjectHelper.UnknownObject;
		}

		public StartPositionDefinition(ObjectData data, string name)
			: this(name)
		{
			try
			{
				if (data.Art != null)
				{
					MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
					foreach (FileInfo file in data.Art)
						art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(file.Filename, data.ArtCompression)), file.Offset);
					byte[] artfile = art.ToArray();
					if (data.MapFile != null)
					{
						if (data.DPLCFile != null)
							spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.MapVersion, LevelData.ReadFile(data.DPLCFile, data.DPLCCompression), data.DPLCVersion == EngineVersion.Invalid & LevelData.Game.DPLCVersion == EngineVersion.S3K ? EngineVersion.S2 : data.DPLCVersion, data.Frame, data.Palette);
						else
							spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.MapVersion);
					}
					else if (data.MapFileAsm != null)
					{
						if (data.MapAsmLabel != null)
						{
							if (data.DPLCFileAsm != null)
								spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.DPLCVersion == EngineVersion.Invalid & LevelData.Game.DPLCVersion == EngineVersion.S3K ? EngineVersion.S2 : data.DPLCVersion, data.Palette);
							else
								spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.MapVersion);
						}
						else
						{
							if (data.DPLCFileAsm != null)
								spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.DPLCVersion == EngineVersion.Invalid & LevelData.Game.DPLCVersion == EngineVersion.S3K ? EngineVersion.S2 : data.DPLCVersion, data.Frame, data.Palette);
							else
								spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.MapVersion);
						}
					}
					else
						spr = ObjectHelper.UnknownObject;
					if (data.Offset != Size.Empty)
						spr.Offset = spr.Offset + data.Offset;
				}
				else if (data.Image != null)
				{
					BitmapBits img = new BitmapBits(data.Image);
					spr = new Sprite(img, new Point(data.Offset));
				}
				else if (data.Sprite > -1)
					spr = ObjectHelper.GetSprite(data.Sprite);
				else
					spr = ObjectHelper.UnknownObject;
			}
			catch (Exception ex)
			{
				LevelData.Log("Error loading start position definition " + this.name + ":", ex.ToString());
				spr = ObjectHelper.UnknownObject;
				debug = true;
			}
			spr = spr.Trim();
		}

		public string Name { get { return name; } }

		public Sprite Image { get { return spr; } }

		public Rectangle GetBounds(StartPositionEntry st)
		{
			return new Rectangle(st.X + spr.X, st.Y + spr.Y, spr.Width, spr.Height);
		}

		public Sprite GetSprite(StartPositionEntry st)
		{
			return new Sprite(spr.Image, new Point(st.X + spr.X, st.Y + spr.Y));
		}

		public bool Debug { get { return debug; } }
	}
}
