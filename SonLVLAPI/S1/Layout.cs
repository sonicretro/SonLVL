namespace SonicRetro.SonLVL.API.S1
{
	public class Layout : LayoutFormatSeparate
	{
		private void ReadLayoutInternal(byte[] rawdata, ref byte[,] layout, ref bool[,] loop)
		{
			int w = rawdata[0] + 1;
			int h = rawdata[1] + 1;
			layout = new byte[w, h];
			loop = new bool[w, h];
			for (int lr = 0; lr < h; lr++)
				for (int lc = 0; lc < w; lc++)
				{
					if ((lr * w) + lc + 2 >= rawdata.Length) break;
					layout[lc, lr] = (byte)(rawdata[(lr * w) + lc + 2] & 0x7F);
					loop[lc, lr] = (rawdata[(lr * w) + lc + 2] & 0x80) == 0x80;
				}
		}

		public override void ReadFG(byte[] rawdata, LayoutData layout)
		{
			ReadLayoutInternal(rawdata, ref layout.FGLayout, ref layout.FGLoop);
		}

		public override void ReadBG(byte[] rawdata, LayoutData layout)
		{
			ReadLayoutInternal(rawdata, ref layout.BGLayout, ref layout.BGLoop);
		}

		private void WriteLayoutInternal(byte[,] layout, bool[,] loop, out byte[] rawdata)
		{
			int w = layout.GetLength(0);
			int h = layout.GetLength(1);
			rawdata = new byte[w * h + 2];
			rawdata[0] = (byte)(w - 1);
			rawdata[1] = (byte)(h - 1);
			int c = 2;
			for (int lr = 0; lr < h; lr++)
				for (int lc = 0; lc < w; lc++)
					rawdata[c++] = (byte)(layout[lc, lr] | (loop[lc, lr] ? 0x80 : 0));
		}

		public override void WriteFG(LayoutData layout, out byte[] rawdata)
		{
			WriteLayoutInternal(layout.FGLayout, layout.FGLoop, out rawdata);
		}

		public override void WriteBG(LayoutData layout, out byte[] rawdata)
		{
			WriteLayoutInternal(layout.BGLayout, layout.BGLoop, out rawdata);
		}

		public override bool HasLoopFlag { get { return true; } }

		public override bool IsResizable { get { return true; } }

		public override System.Drawing.Size MaxSize { get { return new System.Drawing.Size(64, 8); } }
	}
}
