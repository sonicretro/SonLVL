using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace SonicRetro.SonLVL.API
{
	/// <summary>
	/// Represents a level layout format.
	/// </summary>
	public abstract class LayoutFormat
	{
		/// <summary>
		/// The default compression used for layout files.
		/// </summary>
		public virtual CompressionType DefaultCompression { get { return CompressionType.Uncompressed; } }
		public virtual bool HasLoopFlag { get { return false; } }
		public virtual bool IsResizable { get { return false; } }
		/// <summary>
		/// The maximum (or only) size of a level layout.
		/// </summary>
		public abstract Size MaxSize { get; }
		/// <summary>
		/// The default size of a level layout.
		/// </summary>
		public virtual Size DefaultSize { get { return MaxSize; } }
		/// <summary>
		/// If true, the layout is stored in a single file for both FG and BG data.
		/// If false, the FG and BG data are stored in separate files.
		/// </summary>
		public abstract bool IsCombinedLayout { get; }
	}

	/// <summary>
	/// Represents a level layout format that stores FG and BG data in a single file.
	/// </summary>
	public abstract class LayoutFormatCombined : LayoutFormat
	{
		public abstract void ReadLayout(byte[] rawdata, LayoutData layout);

		public void ReadLayout(string filename, CompressionType compression, LayoutData layout)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			LevelData.Log("Loading layout from file \"" + filename + "\", using compression " + compression + "...");
			ReadLayout(Compression.Decompress(filename, compression), layout);
		}

		public void ReadLayout(string filename, LayoutData layout) { ReadLayout(filename, DefaultCompression, layout); }

		public void TryReadLayout(string filename, CompressionType compression, LayoutData layout)
		{
			if (File.Exists(filename))
				ReadLayout(filename, compression, layout);
			else
			{
				LevelData.Log("Layout file \"" + filename + "\" not found.");
				layout.FGLayout = new byte[DefaultSize.Width, DefaultSize.Height];
				layout.BGLayout = new byte[DefaultSize.Width, DefaultSize.Height];
				if (HasLoopFlag)
				{
					layout.FGLoop = new bool[DefaultSize.Width, DefaultSize.Height];
					layout.BGLoop = new bool[DefaultSize.Width, DefaultSize.Height];
				}
			}
		}

		public void TryReadLayout(string filename, LayoutData layout) { TryReadLayout(filename, DefaultCompression, layout); }

		public abstract void WriteLayout(LayoutData layout, out byte[] rawdata);

		public void WriteLayout(LayoutData layout, CompressionType compression, string filename)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			WriteLayout(layout, out byte[] data);
			Compression.Compress(data, filename, compression);
		}

		public void WriteLayout(LayoutData layout, string filename) { WriteLayout(layout, DefaultCompression, filename); }

		public override bool IsCombinedLayout { get { return true; } }
	}

	/// <summary>
	/// Represents a level layout format that stores FG and BG data in separate files.
	/// </summary>
	public abstract class LayoutFormatSeparate : LayoutFormat
	{
		public abstract void ReadFG(byte[] rawdata, LayoutData layout);

		public abstract void ReadBG(byte[] rawdata, LayoutData layout);

		public void ReadFG(string filename, CompressionType compression, LayoutData layout)
		{
			if (compression == CompressionType.Invalid) compression = DefaultFGCompression;
			LevelData.Log("Loading FG layout from file \"" + filename + "\", using compression " + compression + "...");
			ReadFG(Compression.Decompress(filename, compression), layout);
		}

		public void ReadBG(string filename, CompressionType compression, LayoutData layout)
		{
			if (compression == CompressionType.Invalid) compression = DefaultBGCompression;
			LevelData.Log("Loading BG layout from file \"" + filename + "\", using compression " + compression + "...");
			ReadBG(Compression.Decompress(filename, compression), layout);
		}

		public void ReadFG(string filename, LayoutData layout) { ReadFG(filename, DefaultFGCompression, layout); }

		public void ReadBG(string filename, LayoutData layout) { ReadBG(filename, DefaultBGCompression, layout); }

		public void TryReadFG(string filename, CompressionType compression, LayoutData layout)
		{
			if (File.Exists(filename))
				ReadFG(filename, compression, layout);
			else
			{
				LevelData.Log("FG layout file \"" + filename + "\" not found.");
				layout.FGLayout = new byte[DefaultSize.Width, DefaultSize.Height];
				if (HasLoopFlag)
					layout.FGLoop = new bool[DefaultSize.Width, DefaultSize.Height];
			}
		}

		public void TryReadBG(string filename, CompressionType compression, LayoutData layout)
		{
			if (File.Exists(filename))
				ReadBG(filename, compression, layout);
			else
			{
				LevelData.Log("BG layout file \"" + filename + "\" not found.");
				layout.BGLayout = new byte[DefaultSize.Width, DefaultSize.Height];
				if (HasLoopFlag)
					layout.BGLoop = new bool[DefaultSize.Width, DefaultSize.Height];
			}
		}

		public void TryReadFG(string filename, LayoutData layout) { TryReadFG(filename, DefaultFGCompression, layout); }

		public void TryReadBG(string filename, LayoutData layout) { TryReadBG(filename, DefaultBGCompression, layout); }

		public void ReadLayout(byte[] rawfg, byte[] rawbg, LayoutData layout)
		{
			ReadFG(rawfg, layout);
			ReadBG(rawbg, layout);
		}

		public void ReadLayout(string fgfilename, string bgfilename, CompressionType fgcompression, CompressionType bgcompression, LayoutData layout)
		{
			ReadFG(fgfilename, fgcompression, layout);
			ReadBG(bgfilename, bgcompression, layout);
		}

		public void ReadLayout(string fgfilename, string bgfilename, CompressionType compression, LayoutData layout)
		{
			ReadFG(fgfilename, compression, layout);
			ReadBG(bgfilename, compression, layout);
		}

		public void ReadLayout(string fgfilename, string bgfilename, LayoutData layout)
		{
			ReadFG(fgfilename, layout);
			ReadBG(bgfilename, layout);
		}

		public void TryReadLayout(string fgfilename, string bgfilename, CompressionType fgcompression, CompressionType bgcompression, LayoutData layout)
		{
			TryReadFG(fgfilename, fgcompression, layout);
			TryReadBG(bgfilename, bgcompression, layout);
		}

		public void TryReadLayout(string fgfilename, string bgfilename, CompressionType compression, LayoutData layout)
		{
			TryReadFG(fgfilename, compression, layout);
			TryReadBG(bgfilename, compression, layout);
		}

		public void TryReadLayout(string fgfilename, string bgfilename, LayoutData layout)
		{
			TryReadFG(fgfilename, layout);
			TryReadBG(bgfilename, layout);
		}

		public abstract void WriteFG(LayoutData layout, out byte[] rawdata);

		public abstract void WriteBG(LayoutData layout, out byte[] rawdata);

		public void WriteFG(LayoutData layout, CompressionType compression, string filename)
		{
			if (compression == CompressionType.Invalid) compression = DefaultFGCompression;
			WriteFG(layout, out byte[] data);
			Compression.Compress(data, filename, compression);
		}

		public void WriteBG(LayoutData layout, CompressionType compression, string filename)
		{
			if (compression == CompressionType.Invalid) compression = DefaultBGCompression;
			WriteBG(layout, out byte[] data);
			Compression.Compress(data, filename, compression);
		}

		public void WriteFG(LayoutData layout, string filename) { WriteFG(layout, DefaultFGCompression, filename); }

		public void WriteBG(LayoutData layout, string filename) { WriteBG(layout, DefaultBGCompression, filename); }

		public void WriteLayout(LayoutData layout, out byte[] rawfg, out byte[] rawbg)
		{
			WriteFG(layout, out rawfg);
			WriteBG(layout, out rawbg);
		}

		public void WriteLayout(LayoutData layout, CompressionType fgcompression, CompressionType bgcompression, string fgfilename, string bgfilename)
		{
			WriteFG(layout, fgcompression, fgfilename);
			WriteBG(layout, bgcompression, bgfilename);
		}

		public void WriteLayout(LayoutData layout, CompressionType compression, string fgfilename, string bgfilename)
		{
			WriteFG(layout, compression, fgfilename);
			WriteBG(layout, compression, bgfilename);
		}

		public void WriteLayout(LayoutData layout, string fgfilename, string bgfilename)
		{
			WriteFG(layout, fgfilename);
			WriteBG(layout, bgfilename);
		}

		public override bool IsCombinedLayout { get { return false; } }

		public virtual CompressionType DefaultFGCompression { get { return DefaultCompression; } }

		public virtual CompressionType DefaultBGCompression { get { return DefaultCompression; } }
	}
}
