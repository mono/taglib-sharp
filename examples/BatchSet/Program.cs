using System;
using System.Collections.Generic;
using TagLib;

namespace BatchSet;

public class Program
{
	enum Mode {
		Tag,
		Value,
		File
	}

	public static void Main(string [] args)
	{
		if(args.Length < 3) {
			Console.Error.WriteLine ("USAGE: BatchSet.exe -tag value [-tag2 value ...] File1 [File2 ...]");
			return;
		}

		Mode mode = Mode.Tag;
		var files = new List<string> ();
		var tags  = new Dictionary<string,string> ();

		string tag = null;

		foreach (string str in args) {
			if (mode == Mode.Tag) {
				if (str [0] == '-') {
					if (str == "--") {
						mode = Mode.File;
					} else {
						tag = str.Substring (1);
						mode = Mode.Value;
					}

					continue;
				}
				mode = Mode.File;
			}

			if (mode == Mode.Value) {
				if (!tags.ContainsKey (tag))
					tags.Add (tag, str);
				mode = Mode.Tag;
				continue;
			}

			if (mode == Mode.File)
				files.Add (str);
		}

		foreach (string filename in files) {
			using var file = TagLib.File.Create (filename);
			if (file == null)
				continue;

			Console.WriteLine ($"Updating Tags For: {filename}");

			foreach (string key in tags.Keys) {
				string value = tags [key];
				try {
					switch (key) {
					case "id3version":
						byte number = byte.Parse (value);
						if (number == 1) {
							file.RemoveTags (TagTypes.Id3v2);
						} else {
							if (file.GetTag (TagTypes.Id3v2, true) is TagLib.Id3v2.Tag v2)
								v2.Version = number;
						}
						break;
					case "album":
						file.Tag.Album = value;
						break;
					case "artists":
						file.Tag.AlbumArtists = value.Split ([';']);
						break;
					case "comment":
						file.Tag.Comment = value;
						break;
					case "lyrics":
						file.Tag.Lyrics = value;
						break;
					case "composers":
						file.Tag.Composers = value.Split ([';']);
						break;
					case "disc":
						file.Tag.Disc = uint.Parse (value);
						break;
					case "disccount":
						file.Tag.DiscCount = uint.Parse (value);
						break;
					case "genres":
						file.Tag.Genres = value.Split ([';']);
						break;
					case "performers":
						file.Tag.Performers = value.Split ([';']);
						break;
					case "title":
						file.Tag.Title = value;
						break;
					case "track":
						file.Tag.Track = uint.Parse (value);
						break;
					case "trackcount":
						file.Tag.TrackCount = uint.Parse (value);
						break;
					case "year":
						file.Tag.Year = uint.Parse (value);
						break;
					case "pictures":
						var pics = new List<Picture> ();
						if (!string.IsNullOrEmpty (value))
							foreach (string path in value.Split ([';'])) {
								pics.Add (new Picture (path));
							}
						file.Tag.Pictures = pics.ToArray ();
						break;
					}
				} catch (Exception e) {
					Console.WriteLine ($"Error setting tag {key}:");
					Console.WriteLine (e);
				}
			}

			file.Save();
		}
	}
}
