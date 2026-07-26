using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SonicRetro.SonLVL.API;

namespace S1ObjectDefinitions.SBZ
{
	class ConveyorBelt : ObjectDefinition
	{
		private Sprite img;
		private PropertySpec[] properties = new PropertySpec[3];

		public override void Init(ObjectData data)
		{
			byte[] artfile = ObjectHelper.OpenArtFile("../artnem/Monitors.nem", CompressionType.Nemesis);
			img = ObjectHelper.MapASMToBmp(artfile, "../_maps/Invisible Barriers.asm", 0, 0);

			properties[0] = new PropertySpec("Direction", typeof(int), "Extended",
				"The direction this Conveyor Belt should push the player towards.", null, new Dictionary<string, int>
				{
					{ "Right", 0x00 },
					{ "Left",  0x80 }
				},
				(obj) => obj.SubType & 0x80,
				(obj, value) => obj.SubType = (byte)((obj.SubType & ~0x80) | (int)value));
			
			properties[1] = new PropertySpec("Speed", typeof(int), "Extended",
				"How fast this Conveyor Belt should push the player.", null,
				(obj) => (obj.SubType & 0x70) >> 4,
				(obj, value) => obj.SubType = (byte)((obj.SubType & ~0x70) | (Math.Min(7, Math.Max((int)value, 1)) << 4)));

			properties[2] = new PropertySpec("Width", typeof(int), "Extended",
				"How wide, in pixels, this Conveyor Belt is.", null, new Dictionary<string, int>
				{
					{ "112 Pixels", 1 },
					{ "256 Pixels", 0 }
				},
				(obj) => ((obj.SubType & 0x0f) == 0) ? 0 : 1,
				(obj, value) => obj.SubType = (byte)((obj.SubType & ~0x0f) | (int)value));
		}

		public override ReadOnlyCollection<byte> Subtypes
		{
			get { return new ReadOnlyCollection<byte>(new byte[] { 0x20, 0xA0, 0x21, 0xA1 }); }
		}

		public override string Name
		{
			get { return "Conveyor Belt"; }
		}

		public override bool RememberState
		{
			get { return false; }
		}

		public override byte DefaultSubtype
		{
			get { return 0x20; }
		}

		public override string SubtypeName(byte subtype)
		{
			string name = "Moving " + (((subtype & 0x80) == 0x80) ? "Right" : "Left");
			name += ((subtype & 0x0f) == 0) ? " (Large)" : " (Small)";
			return name;
		}

		public override Sprite Image
		{
			get { return img; }
		}

		public override bool Debug
		{
			get { return true; }
		}

		public override PropertySpec[] CustomProperties
		{
			get { return properties; }
		}

		public override Sprite SubtypeImage(byte subtype)
		{
			return img;
		}

		public override Sprite GetSprite(ObjectEntry obj)
		{
			return img;
		}
		
		public override Sprite GetDebugOverlay(ObjectEntry obj)
		{
			int width = ((obj.SubType & 0x0f) == 0) ? 256 : 112;
			BitmapBits bitmap = new BitmapBits(width + 1, 21);
			bitmap.DrawRectangle(LevelData.ColorWhite, 0, 0, width, 20);
			return new Sprite(bitmap, -(width / 2), -20);
		}
	}
}
