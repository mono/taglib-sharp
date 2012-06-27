#if MSCLR
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace tests.fixtures.TagLib.Tests.Images.Validators {
	class Pixbuf : IDisposable {
		private byte[] m_pixData;

		public Pixbuf(byte[] pixData) { this.m_pixData = pixData; }
		
		internal byte[] SaveToBuffer(string imgtype) {
			using (var sRead = new MemoryStream(m_pixData))
			using (var img = Image.FromStream(sRead))
			using (var sWrite = new MemoryStream()) {
				img.Save(sWrite, ParseFmt(imgtype));
				return sWrite.ToArray();
			}
				
			
		}

		static ImageFormat ParseFmt(string fmt) {
			switch (fmt.ToLowerInvariant()) {
				case "png": return ImageFormat.Png;
				case "jpg":
				case "jpeg": return ImageFormat.Jpeg;
				case "tiff": return ImageFormat.Tiff;
				default: throw new NotImplementedException();
			}
		}

		public void Dispose() { }
	}
}
#endif