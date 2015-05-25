using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using SonicRetro.SonLVL.API;

#region .NET Disclaimer/Info
//===============================================================================
//
// gOODiDEA, uland.com
//===============================================================================
//
// $Header :		$  
// $Author :		$
// $Date   :		$
// $Revision:		$
// $History:		$  
//  
//===============================================================================
#endregion 

#region Java
/**
 * Class AnimatedGifEncoder - Encodes a GIF file consisting of one or
 * more frames.
 * <pre>
 * Example:
 *    AnimatedGifEncoder e = new AnimatedGifEncoder();
 *    e.start(outputFileName);
 *    e.setDelay(1000);   // 1 frame per sec
 *    e.addFrame(image1);
 *    e.addFrame(image2);
 *    e.finish();
 * </pre>
 * No copyright asserted on the source code of this class.  May be used
 * for any purpose, however, refer to the Unisys LZW patent for restrictions
 * on use of the associated LZWEncoder class.  Please forward any corrections
 * to kweiner@fmsware.com.
 *
 * @author Kevin Weiner, FM Software
 * @version 1.03 November 2003
 *
 */
#endregion

namespace SonAni
{
	public class AnimatedGifEncoder
	{
		protected int width; // image size
		protected int height;
		protected int transIndex; // transparent index in color table
		protected int repeat = -1; // no repeat
		protected int delay = 0; // frame delay (hundredths)
		protected bool started = false; // ready to output frames
		//	protected BinaryWriter bw;
		protected FileStream fs;

		protected BitmapBits image; // current frame
		protected byte[] indexedPixels; // converted frame indexed to palette
		protected int colorDepth = 8; // number of bit planes
		protected byte[] colorTab; // RGB palette
		protected bool[] usedEntry = new bool[256]; // active palette entries
		protected int palSize = 7; // color table size (bits-1)
		protected int dispose = -1; // disposal code (-1 = use default)
		protected bool closeStream = false; // close stream when finished
		protected bool firstFrame = true;
		protected bool sizeSet = false; // if false, get size from first frame

		/**
		 * Sets the delay time between each frame, or changes it
		 * for subsequent frames (applies to last frame added).
		 *
		 * @param ms int delay time in milliseconds
		 */
		public void SetDelay(double ms)
		{
			delay = (int)Math.Truncate(ms);
		}

		/**
		 * Sets the GIF frame disposal code for the last added frame
		 * and any subsequent frames.  Default is 0 if no transparent
		 * color has been set, otherwise 2.
		 * @param code int disposal code.
		 */
		public void SetDispose(int code)
		{
			if (code >= 0)
			{
				dispose = code;
			}
		}

		/**
		 * Sets the number of times the set of GIF frames
		 * should be played.  Default is 1; 0 means play
		 * indefinitely.  Must be invoked before the first
		 * image is added.
		 *
		 * @param iter int number of iterations.
		 * @return
		 */
		public void SetRepeat(int iter)
		{
			if (iter >= 0)
			{
				repeat = iter;
			}
		}

		public void SetPalette(Color[] palette)
		{
			colorTab = new byte[256 * 3];
			for (int i = 0; i < palette.Length; i++)
			{
				colorTab[i * 3] = palette[i].R;
				colorTab[(i * 3) + 1] = palette[i].G;
				colorTab[(i * 3) + 2] = palette[i].B;
			}
		}

		/**
		 * Adds next GIF frame.  The frame is not written immediately, but is
		 * actually deferred until the next frame is received so that timing
		 * data can be inserted.  Invoking <code>finish()</code> flushes all
		 * frames.  If <code>setSize</code> was not invoked, the size of the
		 * first image is used for all subsequent frames.
		 *
		 * @param im BufferedImage containing frame to write.
		 * @return true if successful.
		 */
		public bool AddFrame(BitmapBits im)
		{
			if ((im == null) || !started)
			{
				return false;
			}
			bool ok = true;
			try
			{
				if (!sizeSet)
				{
					// use first frame's size
					SetSize(im.Width, im.Height);
				}
				image = im;
				GetImagePixels(); // convert to correct format if necessary
				if (firstFrame)
				{
					WriteLSD(); // logical screen descriptior
					WritePalette(); // global color table
					if (repeat >= 0)
					{
						// use NS app extension to indicate reps
						WriteNetscapeExt();
					}
				}
				WriteGraphicCtrlExt(); // write graphic control extension
				WriteImageDesc(); // image descriptor
				/*if (!firstFrame)
				{
					WritePalette(); // local color table
				}*/
				WritePixels(); // encode and write pixel data
				firstFrame = false;
			}
			catch (IOException)
			{
				ok = false;
			}

			return ok;
		}

		/**
		 * Flushes any pending data and closes output file.
		 * If writing to an OutputStream, the stream is not
		 * closed.
		 */
		public bool Finish()
		{
			if (!started) return false;
			bool ok = true;
			started = false;
			try
			{
				fs.WriteByte(0x3b); // gif trailer
				fs.Flush();
				if (closeStream)
				{
					fs.Close();
				}
			}
			catch (IOException)
			{
				ok = false;
			}

			// reset for subsequent use
			transIndex = 0;
			fs = null;
			image = null;
			indexedPixels = null;
			colorTab = null;
			closeStream = false;
			firstFrame = true;

			return ok;
		}

		/**
		 * Sets the GIF frame size.  The default size is the
		 * size of the first frame added if this method is
		 * not invoked.
		 *
		 * @param w int frame width.
		 * @param h int frame width.
		 */
		public void SetSize(int w, int h)
		{
			if (started && !firstFrame) return;
			width = w;
			height = h;
			if (width < 1) width = 320;
			if (height < 1) height = 240;
			sizeSet = true;
		}

		/**
		 * Initiates GIF file creation on the given stream.  The stream
		 * is not closed automatically.
		 *
		 * @param os OutputStream on which GIF images are written.
		 * @return false if initial write failed.
		 */
		public bool Start(FileStream os)
		{
			if (os == null) return false;
			bool ok = true;
			closeStream = false;
			fs = os;
			try
			{
				WriteString("GIF89a"); // header
			}
			catch (IOException)
			{
				ok = false;
			}
			return started = ok;
		}

		/**
		 * Initiates writing of a GIF file with the specified name.
		 *
		 * @param file String containing output file name.
		 * @return false if open or initial write failed.
		 */
		public bool Start(String file)
		{
			bool ok = true;
			try
			{
				//			bw = new BinaryWriter( new FileStream( file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None ) );
				fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				ok = Start(fs);
				closeStream = true;
			}
			catch (IOException)
			{
				ok = false;
			}
			return started = ok;
		}

		/**
		 * Extracts image pixels into byte array "pixels"
		 */
		protected void GetImagePixels()
		{
			int w = image.Width;
			int h = image.Height;
			//		int type = image.GetType().;
			if ((w != width)
				|| (h != height)
				)
			{
				// create new image with right size/format
				BitmapBits bmp = new BitmapBits(width, height);
				bmp.DrawBitmapComposited(image, Point.Empty);
				image = bmp;
			}
			indexedPixels = image.Bits;
		}

		/**
		 * Writes Graphic Control Extension
		 */
		protected void WriteGraphicCtrlExt()
		{
			fs.WriteByte(0x21); // extension introducer
			fs.WriteByte(0xf9); // GCE label
			fs.WriteByte(4); // data block size
			int transp, disp;
				transp = 1;
				disp = 2; // force clear if using transparent color
			if (dispose >= 0)
			{
				disp = dispose & 7; // user override
			}
			disp <<= 2;

			// packed fields
			fs.WriteByte(Convert.ToByte(0 | // 1:3 reserved
				disp | // 4:6 disposal
				0 | // 7   user input - 0 = none
				transp)); // 8   transparency flag

			WriteShort(delay); // delay x 1/100 sec
			fs.WriteByte(Convert.ToByte(transIndex)); // transparent color index
			fs.WriteByte(0); // block terminator
		}

		/**
		 * Writes Image Descriptor
		 */
		protected void WriteImageDesc()
		{
			fs.WriteByte(0x2c); // image separator
			WriteShort(0); // image position x,y = 0,0
			WriteShort(0);
			WriteShort(width); // image size
			WriteShort(height);
			// packed fields
				fs.WriteByte(0);
		}

		/**
		 * Writes Logical Screen Descriptor
		 */
		protected void WriteLSD()
		{
			// logical screen size
			WriteShort(width);
			WriteShort(height);
			// packed fields
			fs.WriteByte(Convert.ToByte(0x80 | // 1   : global color table flag = 1 (gct used)
				0x70 | // 2-4 : color resolution = 7
				0x00 | // 5   : gct sort flag = 0
				palSize)); // 6-8 : gct size

			fs.WriteByte(0); // background color index
			fs.WriteByte(0); // pixel aspect ratio - assume 1:1
		}

		/**
		 * Writes Netscape application extension to define
		 * repeat count.
		 */
		protected void WriteNetscapeExt()
		{
			fs.WriteByte(0x21); // extension introducer
			fs.WriteByte(0xff); // app extension label
			fs.WriteByte(11); // block size
			WriteString("NETSCAPE" + "2.0"); // app id + auth code
			fs.WriteByte(3); // sub-block size
			fs.WriteByte(1); // loop sub-block id
			WriteShort(repeat); // loop count (extra iterations, 0=repeat forever)
			fs.WriteByte(0); // block terminator
		}

		/**
		 * Writes color table
		 */
		protected void WritePalette()
		{
			fs.Write(colorTab, 0, colorTab.Length);
			int n = (3 * 256) - colorTab.Length;
			for (int i = 0; i < n; i++)
			{
				fs.WriteByte(0);
			}
		}

		/**
		 * Encodes and writes pixel data
		 */
		protected void WritePixels()
		{
			LZWEncoder encoder =
				new LZWEncoder(width, height, indexedPixels, colorDepth);
			encoder.Encode(fs);
		}

		/**
		 *    Write 16-bit value to output stream, LSB first
		 */
		protected void WriteShort(int value)
		{
			fs.WriteByte(Convert.ToByte(value & 0xff));
			fs.WriteByte(Convert.ToByte((value >> 8) & 0xff));
		}

		/**
		 * Writes string to output stream
		 */
		protected void WriteString(String s)
		{
			char[] chars = s.ToCharArray();
			for (int i = 0; i < chars.Length; i++)
			{
				fs.WriteByte((byte)chars[i]);
			}
		}
	}

}
