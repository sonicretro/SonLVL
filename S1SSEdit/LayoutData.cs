using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S1SSEdit
{
	public class LayoutData
	{
		public byte[,] Layout { get; private set; }
		public Position StartPosition { get; private set; }

		public LayoutData()
		{
			Layout = new byte[0x40, 0x40];
		}

		public LayoutData(byte[] layoutdata)
			: this()
		{
			int i = 0;
			for (int y = 0; y < 0x40; y++)
				for (int x = 0; x < 0x40; x++)
					Layout[x, y] = layoutdata[i++];
		}

		public LayoutData(byte[] layoutdata, byte[] startposdata)
			: this(layoutdata)
		{
			StartPosition = new Position(startposdata);
		}

		public byte[] GetBytes()
		{
			byte[] result = new byte[0x40 * 0x40];
			int i = 0;
			for (int y = 0; y < 0x40; y++)
				for (int x = 0; x < 0x40; x++)
					result[i++] = Layout[x, y];
			return result;
		}

		public LayoutData Clone()
		{
			LayoutData result = new LayoutData();
			result.Layout = (byte[,])Layout.Clone();
			if (StartPosition != null)
				result.StartPosition = new Position(StartPosition.X, StartPosition.Y);
			return result;
		}
	}
}
