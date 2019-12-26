using System;
using System.Collections.Generic;

namespace SonicRetro.SonLVL.API.S3K
{
	public class Layout : LayoutFormatCombined
	{
		public override void ReadLayout(byte[] rawdata, LayoutData layout)
		{
			int fgw = ByteConverter.ToUInt16(rawdata, 0);
			int bgw = ByteConverter.ToUInt16(rawdata, 2);
			int fgh = Math.Min((int)ByteConverter.ToUInt16(rawdata, 4), MaxSize.Height);
			int bgh = Math.Min((int)ByteConverter.ToUInt16(rawdata, 6), MaxSize.Height);
			layout.FGLayout = new byte[fgw, fgh];
			layout.BGLayout = new byte[bgw, bgh];
			for (int la = 0; la < Math.Max(fgh, bgh) * 4; la += 4)
			{
				ushort lfp = ByteConverter.ToUInt16(rawdata, 8 + la);
				if (lfp != 0)
					for (int laf = 0; laf < fgw; laf++)
						layout.FGLayout[laf, la / 4] = rawdata[lfp - StartAddress + laf];
				ushort lbp = ByteConverter.ToUInt16(rawdata, 8 + la + 2);
				if (lbp != 0)
					for (int lab = 0; lab < bgw; lab++)
						layout.BGLayout[lab, la / 4] = rawdata[lbp - StartAddress + lab];
			}
		}

		public override void WriteLayout(LayoutData layout, out byte[] rawdata)
		{
			List<byte> tmp = new List<byte>();
			ushort fgw = (ushort)layout.FGLayout.GetLength(0);
			ushort bgw = (ushort)layout.BGLayout.GetLength(0);
			ushort fgh = (ushort)layout.FGLayout.GetLength(1);
			ushort bgh = (ushort)layout.BGLayout.GetLength(1);
			tmp.AddRange(ByteConverter.GetBytes(fgw));
			tmp.AddRange(ByteConverter.GetBytes(bgw));
			tmp.AddRange(ByteConverter.GetBytes(fgh));
			tmp.AddRange(ByteConverter.GetBytes(bgh));
			for (int la = 0; la < MaxSize.Height; la++)
			{
				if (la < fgh)
					tmp.AddRange(ByteConverter.GetBytes((ushort)(StartAddress + 0x88 + (la * fgw))));
				else
					tmp.AddRange(new byte[2]);
				if (la < bgh)
					tmp.AddRange(ByteConverter.GetBytes((ushort)(StartAddress + 0x88 + (fgh * fgw) + (la * bgw))));
				else
					tmp.AddRange(new byte[2]);
			}
			for (int y = 0; y < fgh; y++)
				for (int x = 0; x < fgw; x++)
					tmp.Add(layout.FGLayout[x, y]);
			for (int y = 0; y < bgh; y++)
				for (int x = 0; x < bgw; x++)
					tmp.Add(layout.BGLayout[x, y]);
			rawdata = tmp.ToArray();
		}

		public override bool IsResizable { get { return true; } }

		public override System.Drawing.Size MaxSize { get { return new System.Drawing.Size(200, 32); } }

		public override System.Drawing.Size DefaultSize { get { return new System.Drawing.Size(128, 16); } }

		public virtual ushort StartAddress { get { return 0x8000; } }
	}
}
