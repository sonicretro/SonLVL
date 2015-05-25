using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SonLVL.API.SCDPC
{
	public class Layout : LayoutFormatSeparate
	{
		private void ReadLayoutInternal(byte[] rawdata, ref byte[,] layout)
		{
			layout = new byte[MaxSize.Width, MaxSize.Height];
			int c = 0;
			for (int lr = 0; lr < MaxSize.Height; lr++)
				for (int lc = 0; lc < MaxSize.Width; lc++)
					layout[lc, lr] = rawdata[c++];
		}

		public override void ReadFG(byte[] rawdata, LayoutData layout)
		{
			ReadLayoutInternal(rawdata, ref layout.FGLayout);
		}

		public override void ReadBG(byte[] rawdata, LayoutData layout)
		{
			ReadLayoutInternal(rawdata, ref layout.BGLayout);
		}

		private void WriteLayoutInternal(byte[,] layout, out byte[] rawdata)
		{
			rawdata = new byte[MaxSize.Width * MaxSize.Height];
			int c = 0;
			for (int lr = 0; lr < MaxSize.Height; lr++)
				for (int lc = 0; lc < MaxSize.Width; lc++)
					rawdata[c++] = layout[lc, lr];
		}

		public override void WriteFG(LayoutData layout, out byte[] rawdata)
		{
			WriteLayoutInternal(layout.FGLayout, out rawdata);
		}

		public override void WriteBG(LayoutData layout, out byte[] rawdata)
		{
			WriteLayoutInternal(layout.BGLayout, out rawdata);
		}

		public override System.Drawing.Size MaxSize { get { return new System.Drawing.Size(64, 8); } }
	}
}
