namespace SonicRetro.SonLVL.API
{
	public static class ObjectHelper
	{
		public static byte[] OpenArtFile(string file, CompressionType comp) { return LevelData.ReadFile(file, comp); }

		public static Sprite MapToBmp(byte[] artfile, byte[] mapfile, int frame, int startpal, bool priority = false, EngineVersion version = EngineVersion.Invalid)
		{
			if (version == EngineVersion.Invalid)
				version = LevelData.Game.MappingsVersion;
			return LevelData.MapFrameToBmp(artfile, MappingsFrame.Load(mapfile, version)[frame], startpal, priority);
		}

		public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, int frame, int startpal, bool priority = false, EngineVersion version = EngineVersion.Invalid)
		{
			if (version == EngineVersion.Invalid)
				version = LevelData.Game.MappingsVersion;
			return MapToBmp(artfile, LevelData.ASMToBin(mapfileloc, version), frame, startpal, priority, version);
		}

		public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, string label, int startpal, bool priority = false, EngineVersion version = EngineVersion.Invalid)
		{
			if (version == EngineVersion.Invalid)
				version = LevelData.Game.MappingsVersion;
			byte[] mapfile = LevelData.ASMToBin(mapfileloc, label, version);
			return LevelData.MapFrameToBmp(artfile, new MappingsFrame(mapfile, 0, version, string.Empty), startpal, false);
		}

		public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, byte[] dplc, int frame, int startpal, bool priority = false, EngineVersion version = EngineVersion.Invalid)
		{
			return MapDPLCToBmp(artfile, mapfile, version, dplc, version, frame, startpal, priority);
		}

		public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, EngineVersion mapversion, byte[] dplc, EngineVersion dplcversion, int frame, int startpal, bool priority = false)
		{
			if (mapversion == EngineVersion.Invalid)
				mapversion = LevelData.Game.MappingsVersion;
			if (dplcversion == EngineVersion.Invalid)
				dplcversion = LevelData.Game.DPLCVersion;
			return LevelData.MapFrameToBmp(artfile, MappingsFrame.Load(mapfile, mapversion)[frame], DPLCFrame.Load(dplc, dplcversion)[frame], startpal, priority);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string label, string dplcloc, string dplclabel, int startpal, bool priority = false, EngineVersion version = EngineVersion.Invalid)
		{
			return MapASMDPLCToBmp(artfile, mapfileloc, label, version, dplcloc, dplclabel, version, startpal, priority);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string label, EngineVersion mapversion, string dplcloc, string dplclabel, EngineVersion dplcversion, int startpal, bool priority = false)
		{
			if (mapversion == EngineVersion.Invalid)
				mapversion = LevelData.Game.MappingsVersion;
			if (dplcversion == EngineVersion.Invalid)
				dplcversion = LevelData.Game.DPLCVersion;
			byte[] mapfile = LevelData.ASMToBin(mapfileloc, label, mapversion);
			byte[] dplcfile = LevelData.ASMToBin(dplcloc, dplclabel, dplcversion);
			return LevelData.MapFrameToBmp(artfile, new MappingsFrame(mapfile, 0, mapversion, string.Empty), new DPLCFrame(dplcfile, 0, dplcversion, string.Empty), startpal, priority);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string dplcloc, int frame, int startpal, bool priority = false, EngineVersion version = EngineVersion.Invalid)
		{
			return MapASMDPLCToBmp(artfile, mapfileloc, version, dplcloc, version, frame, startpal, priority);
		}

		public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, EngineVersion mapversion, string dplcloc, EngineVersion dplcversion, int frame, int startpal, bool priority = false)
		{
			if (mapversion == EngineVersion.Invalid)
				mapversion = LevelData.Game.MappingsVersion;
			if (dplcversion == EngineVersion.Invalid)
				dplcversion = LevelData.Game.DPLCVersion;
			return MapDPLCToBmp(artfile, LevelData.ASMToBin(mapfileloc, mapversion), mapversion, LevelData.ASMToBin(dplcloc, dplcversion), dplcversion, frame, startpal, priority);
		}

		public static Sprite UnknownObject { get { return new Sprite(LevelData.UnknownSprite); } }

		public static Sprite GetSprite(int index) { return LevelData.Sprites[index]; }

		public static byte[] LevelArt { get { return LevelData.TileArray; } }
	}
}
