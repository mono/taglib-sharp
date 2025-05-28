using System;

namespace ParsePhoto;

public class Program
{
	public static void Main (string [] args)
	{
		if(args.Length == 0) {
			Console.Error.WriteLine("USAGE: mono ParsePhoto.exe PATH [...]");
			return;
		}

		foreach (string path in args)
			ParsePhoto (path);
	}

	static void ParsePhoto (string path)
	{
		TagLib.File file;
		try {
			file = TagLib.File.Create(path);
		} catch (TagLib.UnsupportedFormatException) {
			Console.WriteLine ("UNSUPPORTED FILE: " + path);
			Console.WriteLine ();
			Console.WriteLine ("---------------------------------------");
			Console.WriteLine ();
			return;
		}

		var image = file as TagLib.Image.File;
		if (file == null) {
			Console.WriteLine ($"NOT AN IMAGE FILE: {path}");
			Console.WriteLine ();
			Console.WriteLine ("---------------------------------------");
			Console.WriteLine ();
			return;
		}

		Console.WriteLine ();
		Console.WriteLine (path);
		Console.WriteLine ();

		Console.WriteLine($"Tags in object  : {image.TagTypes}");
		Console.WriteLine ();

		Console.WriteLine($"Comment         : {image.ImageTag.Comment}");
		Console.Write("Keywords        : ");
		foreach (var keyword in image.ImageTag.Keywords) {
			Console.Write ($"{keyword} ");
		}

		Console.WriteLine ();
		Console.WriteLine($"Rating          : {image.ImageTag.Rating}");
		Console.WriteLine($"DateTime        : {image.ImageTag.DateTime}");
		Console.WriteLine($"Orientation     : {image.ImageTag.Orientation}");
		Console.WriteLine($"Software        : {image.ImageTag.Software}");
		Console.WriteLine($"ExposureTime    : {image.ImageTag.ExposureTime}");
		Console.WriteLine($"FNumber         : {image.ImageTag.FNumber}");
		Console.WriteLine($"ISOSpeedRatings : {image.ImageTag.ISOSpeedRatings}");
		Console.WriteLine($"FocalLength     : {image.ImageTag.FocalLength}");
		Console.WriteLine($"FocalLength35mm : {image.ImageTag.FocalLengthIn35mmFilm}");
		Console.WriteLine($"Make            : {image.ImageTag.Make}");
		Console.WriteLine($"Model           : {image.ImageTag.Model}");

		if (image.Properties != null) {
			Console.WriteLine($"Width           : {image.Properties.PhotoWidth}");
			Console.WriteLine($"Height          : {image.Properties.PhotoHeight}");
			Console.WriteLine($"Type            : {image.Properties.Description}");
		}

		Console.WriteLine ();
		Console.WriteLine($"Writable?       : {image.Writeable}");
		Console.WriteLine($"Corrupt?        : {image.PossiblyCorrupt}");

		if (image.PossiblyCorrupt) {
			foreach (string reason in image.CorruptionReasons) {
				Console.WriteLine ($"    * {reason}");
			}
		}

		Console.WriteLine ("---------------------------------------");
	}
}
