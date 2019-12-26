namespace SonicRetro.SonLVL.API.S2
{
	public class Layout : LayoutFormatCombined
	{
		public override void ReadLayout(byte[] rawdata, LayoutData layout)
		{
			layout.FGLayout = new byte[DefaultSize.Width, DefaultSize.Height];
			layout.BGLayout = new byte[DefaultSize.Width, DefaultSize.Height];
			int c = 0;
			for (int lr = 0; lr < DefaultSize.Height; lr++)
			{
				for (int lc = 0; lc < DefaultSize.Width; lc++)
					layout.FGLayout[lc, lr] = rawdata[c++];
				for (int lc = 0; lc < DefaultSize.Width; lc++)
					layout.BGLayout[lc, lr] = rawdata[c++];
			}
		}

		public override void WriteLayout(LayoutData layout, out byte[] rawdata)
		{
			rawdata = new byte[(DefaultSize.Width * DefaultSize.Height) * 2];
			int c = 0;
			for (int lr = 0; lr < DefaultSize.Height; lr++)
			{
				for (int lc = 0; lc < DefaultSize.Width; lc++)
					rawdata[c++] = layout.FGLayout[lc, lr];
				for (int lc = 0; lc < DefaultSize.Width; lc++)
					rawdata[c++] = layout.BGLayout[lc, lr];
			}
		}

		public override CompressionType DefaultCompression { get { return CompressionType.Kosinski; } }

		public override System.Drawing.Size MaxSize { get { return new System.Drawing.Size(128, 16); } }
	}
}
