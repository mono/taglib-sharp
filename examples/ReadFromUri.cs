using System;
using System.IO;
using TagLib;

public class ReadFromUri
{
	public static void Write (string name, object value)
	{
		Console.WriteLine ($"{name,20}: {value ?? ""}");
	}

	public static void Write (string name, string[] values)
	{
		Console.WriteLine ($"{name,20}: {(values == null ? "" : string.Join ("\n            ", values))}");
	}

	public static void Main (string[] args)
	{
		if (args.Length == 0) {
			Console.Error.WriteLine ("USAGE: mono ReadFromUri.exe PATH [...]");
			return;
		}

		var start = DateTime.Now;
		int songs_read = 0;
		try {
			foreach (string path in args) {
				string uri = path;
				Console.WriteLine (uri);
				TagLib.File file;

				try {
					var file_info = new FileInfo (uri);
					uri = new Uri (file_info.FullName).ToString ();
				} catch {
				}

				try {
					file = TagLib.File.Create (new FileAbstraction (uri));
				} catch (TagLib.UnsupportedFormatException) {
					Console.WriteLine ("UNSUPPORTED FILE: " + uri);
					Console.WriteLine (string.Empty);
					Console.WriteLine ("---------------------------------------");
					Console.WriteLine (string.Empty);
					continue;
				}

				Console.WriteLine ("Tags on disk:   " + file.TagTypesOnDisk);
				Console.WriteLine ("Tags in object: " + file.TagTypes);
				Console.WriteLine (string.Empty);

				Write ("Grouping", file.Tag.Grouping);
				Write ("Title", file.Tag.Title);
				Write ("TitleSort", file.Tag.TitleSort);
				Write ("Album Artists", file.Tag.AlbumArtists);
				Write ("Album Artists Sort", file.Tag.AlbumArtistsSort);
				Write ("Performers", file.Tag.Performers);
				Write ("Performers Sort", file.Tag.PerformersSort);
				Write ("Composers", file.Tag.Composers);
				Write ("Composers Sort", file.Tag.ComposersSort);
				Write ("Conductor", file.Tag.Conductor);
				Write ("Album", file.Tag.Album);
				Write ("Album Sort", file.Tag.AlbumSort);
				Write ("Comment", file.Tag.Comment);
				Write ("Copyright", file.Tag.Copyright);
				Write ("Genres", file.Tag.Genres);
				Write ("BPM", file.Tag.BeatsPerMinute);
				Write ("Year", file.Tag.Year);
				Write ("Track", file.Tag.Track);
				Write ("TrackCount", file.Tag.TrackCount);
				Write ("Disc", file.Tag.Disc);
				Write ("DiscCount", file.Tag.DiscCount);

				Console.WriteLine ("Lyrics:\n" + file.Tag.Lyrics + "\n");

				Console.WriteLine ("Media Types:     " + file.Properties.MediaTypes + "\n");

				foreach (TagLib.ICodec codec in file.Properties.Codecs) {
					TagLib.IAudioCodec acodec = codec as TagLib.IAudioCodec;
					TagLib.IVideoCodec vcodec = codec as TagLib.IVideoCodec;

					if (acodec != null && (acodec.MediaTypes & TagLib.MediaTypes.Audio) != TagLib.MediaTypes.None) {
						Console.WriteLine ("Audio Properties : " + acodec.Description);
						Console.WriteLine ("Bitrate:    " + acodec.AudioBitrate);
						Console.WriteLine ("SampleRate: " + acodec.AudioSampleRate);
						Console.WriteLine ("Channels:   " + acodec.AudioChannels + "\n");
					}

					if (vcodec != null && (vcodec.MediaTypes & TagLib.MediaTypes.Video) != TagLib.MediaTypes.None) {
						Console.WriteLine ("Video Properties : " + vcodec.Description);
						Console.WriteLine ("Width:      " + vcodec.VideoWidth);
						Console.WriteLine ("Height:     " + vcodec.VideoHeight + "\n");
					}
				}

				if (file.Properties.MediaTypes != TagLib.MediaTypes.None)
					Console.WriteLine ("Length:     " + file.Properties.Duration + "\n");

				IPicture[] pictures = file.Tag.Pictures;

				Console.WriteLine ("Embedded Pictures: " + pictures.Length);

				foreach (IPicture picture in pictures) {
					Console.WriteLine (picture.Description);
					Console.WriteLine ("   MimeType: " + picture.MimeType);
					Console.WriteLine ("   Size:     " + picture.Data.Count);
					Console.WriteLine ("   Type:     " + picture.Type);
				}

				Console.WriteLine (string.Empty);
				Console.WriteLine ("---------------------------------------");
				Console.WriteLine (string.Empty);

				songs_read++;
			}
		} finally {
		}

		DateTime end = DateTime.Now;

		Console.WriteLine ("Total running time:    " + (end - start));
		Console.WriteLine ("Total files read:      " + songs_read);

		if (songs_read > 0) {
			Console.WriteLine ("Average time per file: " + new TimeSpan ((end - start).Ticks / songs_read));
		}
	}
}

public class FileAbstraction : TagLib.File.IFileAbstraction
{
	public FileAbstraction (string file)
	{
		Name = file;
	}

	public string Name { get; }

	public Stream ReadStream => new FileStream (Name, FileMode.Open);

	public Stream WriteStream => new FileStream (Name, FileMode.Open);

	public void CloseStream (Stream stream)
	{
		stream.Close ();
	}
}
