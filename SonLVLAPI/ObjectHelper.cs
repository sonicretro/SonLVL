using System.Drawing;

namespace SonicRetro.SonLVL.API
{
	public static class ObjectHelper
	{
		public static byte[] OpenArtFile(string file, CompressionType comp) { return LevelData.ReadFile(file, comp); }

		public static Sprite MapToBmp(byte[] artfile, byte[] mapfile, int frame, int startpal)
		{
			return MapToBmp(artfile, mapfile, frame, startpal, EngineVersion.Invalid);
		}

		public static Sprite MapToBmp(byte[] artfile, byte[] mapfile, int frame, int startpal, EngineVersion version)
		{
			if (version == EngineVersion.Invalid)
				version = LevelData.Game.MappingsVersion;
			return new Sprite(LevelData.MapFrameToBmp(artfile, MappingsFrame.Load(mapfile, version)[frame], startpal));
		}

		public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, int frame, int startpal)
		{
			return MapASMToBmp(artfile, mapfileloc, frame, startpal, EngineVersion.Invalid);
		}

		public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, int frame, int startpal, EngineVersion version)
		{
			if (version == EngineVersion.Invalid)
				version = LevelData.Game.MappingsVersion;
			return MapToBmp(artfile, LevelData.ASMToBin(mapfileloc, version), frame, startpal, version);
		}

		public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, string label, int startpal)
		{
			return MapASMToBmp(artfile, mapfileloc, label, startpal, EngineVersion.Invalid);
		}

		public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, string label, int startpal, EngineVersion version)
		{
			if (version == EngineVersion.Invalid)
				version = LevelData.Game.MappingsVersion;
			byte[] mapfile = LevelData.ASMToBin(mapfileloc, label, version);
			return new Sprite(LevelData.MapFrameToBmp(artfile, new MappingsFrame(mapfile, 0, version, string.Empty), startpal));
		}

		public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, byte[] dplc, int frame, int startpal)
		{
			return MapDPLCToBmp(artfile, mapfile, dplc, frame, startpal, EngineVersion.Invalid);
		}

		public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, byte[] dplc, int frame, int startpal, EngineVersion version)
		{
			return MapDPLCToBmp(artfile, mapfile, version, dplc, version, frame, startpal);
		}

		public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, EngineVersion mapversion, byte[] dplc, EngineVersion dplcversion, int frame, int startpal)
		{
			if (mapversion == EngineVersion.Invalid)
				mapversion = LevelData.Game.MappingsVersion;
			if (dplcversion == EngineVersion.Invalid)
				dplcversion = LevelData.Game.DPLCVersion;
			return new Sprite(LevelData.MapFrameToBmp(artfile, MappingsFrame.Load(mapfile, mapversion)[frame], DPLCFrame.Load(dplc, dplcversion)[frame], startpal));
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string label, string dplcloc, string dplclabel, int startpal)
		{
			return MapASMDPLCToBmp(artfile, mapfileloc, label, EngineVersion.Invalid, dplcloc, dplclabel, EngineVersion.Invalid, startpal);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string label, EngineVersion mapversion, string dplcloc, string dplclabel, EngineVersion dplcversion, int startpal)
		{
			if (mapversion == EngineVersion.Invalid)
				mapversion = LevelData.Game.MappingsVersion;
			if (dplcversion == EngineVersion.Invalid)
				dplcversion = LevelData.Game.DPLCVersion;
			byte[] mapfile = LevelData.ASMToBin(mapfileloc, label, mapversion);
			byte[] dplcfile = LevelData.ASMToBin(dplcloc, dplclabel, dplcversion);
			return new Sprite(LevelData.MapFrameToBmp(artfile, new MappingsFrame(mapfile, 0, mapversion, string.Empty), new DPLCFrame(dplcfile, 0, dplcversion, string.Empty), startpal));
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string dplcloc, int frame, int startpal)
		{
			return MapASMDPLCToBmp(artfile, mapfileloc, dplcloc, frame, startpal, EngineVersion.Invalid);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string dplcloc, int frame, int startpal, EngineVersion version)
		{
			return MapASMDPLCToBmp(artfile, mapfileloc, version, dplcloc, version, frame, startpal);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, EngineVersion mapversion, string dplcloc, EngineVersion dplcversion, int frame, int startpal)
		{
			if (mapversion == EngineVersion.Invalid)
				mapversion = LevelData.Game.MappingsVersion;
			if (dplcversion == EngineVersion.Invalid)
				dplcversion = LevelData.Game.DPLCVersion;
			return MapDPLCToBmp(artfile, LevelData.ASMToBin(mapfileloc, mapversion), mapversion, LevelData.ASMToBin(dplcloc, dplcversion), dplcversion, frame, startpal);
		}

		public static Sprite UnknownObject { get { return new Sprite(LevelData.UnknownSprite); } }

		public static Sprite GetSprite(int index) { return LevelData.Sprites[index]; }

		public static byte[] LevelArt { get { return LevelData.TileArray; } }

		public static int ShiftLeft(int value, int num) { return value << num; }

		public static int ShiftRight(int value, int num) { return value >> num; }

		public static byte SetSubtypeMask(byte subtype, byte value, int mask) { return (byte)((subtype & ~mask) | (value & mask)); }

		public static bool Not(bool value) { return !value; }
	}
}
