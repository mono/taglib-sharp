
using System;
using TagLib;

public class StripImageData
{
	private static byte[] image_data = new byte[] {
		0xFF, 0xDA, 0x00, 0x0C, 0x03, 0x01, 0x00,
		0x02, 0x11, 0x03, 0x11, 0x00, 0x3F, 0x00,
		0x8C, 0x80, 0x07, 0xFF, 0xD9
	};

	public static void Main (string [] args)
	{
		if (args.Length != 1) {
			Console.Out.WriteLine ("usage: mono StripImageData.exe [jpegfile]");
			return;
		}

		ImageFile file = new ImageFile (args [0]);

		file.Mode = File.AccessMode.Write;
		long sos = file.RFind (new byte [] {0xFF, 0xDA});

		if (sos == -1) {
			Console.Out.WriteLine ("doesn't look like an jpeg file");
			return;
		}

		file.RemoveBlock (sos, file.Length - sos);
		file.Seek (sos);
		file.WriteBlock (image_data);
		file.Mode = File.AccessMode.Closed;
	}


	private class ImageFile : File {

		// Hacky implementation to make use of some methods defined in TagLib.File

		public ImageFile (string path)
		: base (new File.LocalFileAbstraction (path)) {}

		public override Tag GetTag (TagLib.TagTypes type, bool create)
		{
			throw new System.NotImplementedException ();
		}

		public override Properties Properties {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public override void RemoveTags (TagLib.TagTypes types)
		{
			throw new System.NotImplementedException ();
		}

		public override void Save ()
		{
			throw new System.NotImplementedException ();
		}

		public override Tag Tag {
			get {
				throw new System.NotImplementedException ();
			}
		}
	}
}