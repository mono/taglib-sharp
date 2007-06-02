using System;
using TagLib;
using Gnome.Vfs;

public class ReadFromUri
{
    public static void Main(string [] args)
    {
        if(args.Length == 0) {
            Console.Error.WriteLine("USAGE: mono ReadFromUri.exe PATH [...]");
            return;
        }
        
        Gnome.Vfs.Vfs.Initialize();
        
        DateTime start = DateTime.Now;
        int songs_read = 0;
        try {
            foreach (string path in args)
            {
                string uri = path;
                Console.WriteLine (uri);
                TagLib.File file = null;
                
                try {
                    System.IO.FileInfo file_info = new System.IO.FileInfo(uri);
                    uri = Gnome.Vfs.Uri.GetUriFromLocalPath (file_info.FullName);
                } catch {
                }
                
                try
                {
                    file = TagLib.File.Create(new VfsFileAbstraction (uri));
                }
                catch (TagLib.UnsupportedFormatException)
                {
                    Console.WriteLine ("UNSUPPORTED FILE: " + uri);
                    Console.WriteLine (String.Empty);
                    Console.WriteLine ("---------------------------------------");
                    Console.WriteLine (String.Empty);
                    continue;
                }
                
                Console.WriteLine("Tags on disk:   " +  file.TagTypesOnDisk);
                Console.WriteLine("Tags in object: " +  file.TagTypes);
                Console.WriteLine (String.Empty);
               
                Console.WriteLine("Grouping:   " +  file.Tag.Grouping);
                Console.WriteLine("Title:      " +  file.Tag.Title);
                Console.WriteLine("Artists:    " + (file.Tag.AlbumArtists == null ? String.Empty : System.String.Join ("\n            ", file.Tag.AlbumArtists)));
                Console.WriteLine("Performers: " + (file.Tag.Performers   == null ? String.Empty : System.String.Join ("\n            ", file.Tag.Performers)));
                Console.WriteLine("Composers:  " + (file.Tag.Composers    == null ? String.Empty : System.String.Join ("\n            ", file.Tag.Composers)));
                Console.WriteLine("Conductor:  " +  file.Tag.Conductor);
                Console.WriteLine("Album:      " +  file.Tag.Album);
                Console.WriteLine("Comment:    " +  file.Tag.Comment);
                Console.WriteLine("Copyright:  " +  file.Tag.Copyright);
                Console.WriteLine("Genres:     " + (file.Tag.Genres       == null ? String.Empty : System.String.Join ("\n            ", file.Tag.Genres)));
                Console.WriteLine("BPM:        " +  file.Tag.BeatsPerMinute);
                Console.WriteLine("Year:       " +  file.Tag.Year);
                Console.WriteLine("Track:      " +  file.Tag.Track);
                Console.WriteLine("TrackCount: " +  file.Tag.TrackCount);
                Console.WriteLine("Disc:       " +  file.Tag.Disc);
                Console.WriteLine("DiscCount:  " +  file.Tag.DiscCount);
                Console.WriteLine("Lyrics:\n"    +  file.Tag.Lyrics + "\n");
                
                Console.WriteLine("Media Types: " + file.Properties.MediaTypes + "\n");
                
                foreach (TagLib.ICodec codec in file.Properties.Codecs)
                {
                    TagLib.IAudioCodec acodec = codec as TagLib.IAudioCodec;
                    TagLib.IVideoCodec vcodec = codec as TagLib.IVideoCodec;
                
                    if (acodec != null && (acodec.MediaTypes & TagLib.MediaTypes.Audio) != TagLib.MediaTypes.None)
                    {
                        Console.WriteLine("Audio Properties : " + acodec.Description);
                        Console.WriteLine("Bitrate:    " + acodec.AudioBitrate);
                        Console.WriteLine("SampleRate: " + acodec.AudioSampleRate);
                        Console.WriteLine("Channels:   " + acodec.AudioChannels + "\n");
                    }
                
                    if (vcodec != null && (vcodec.MediaTypes & TagLib.MediaTypes.Video) != TagLib.MediaTypes.None)
                    {
                        Console.WriteLine("Video Properties : " + vcodec.Description);
                        Console.WriteLine("Width:      " + vcodec.VideoWidth);
                        Console.WriteLine("Height:     " + vcodec.VideoHeight + "\n");
                    }
                }
                
                if (file.Properties.MediaTypes != TagLib.MediaTypes.None)
                    Console.WriteLine("Length:     " + file.Properties.Duration + "\n");
                
                IPicture [] pictures = file.Tag.Pictures;
                
                Console.WriteLine("Embedded Pictures: " + pictures.Length);
                
                foreach(IPicture picture in pictures) {
                    Console.WriteLine(picture.Description);
                    Console.WriteLine("   MimeType: " + picture.MimeType);
                    Console.WriteLine("   Size:     " + picture.Data.Count);
                    Console.WriteLine("   Type:     " + picture.Type);
                }
                
                Console.WriteLine (String.Empty);
                Console.WriteLine ("---------------------------------------");
                Console.WriteLine (String.Empty);
                
                songs_read ++;
            }
        } finally {
           Gnome.Vfs.Vfs.Shutdown();
        }
        
        DateTime end = DateTime.Now;
        
        Console.WriteLine ("Total running time:    " + (end - start));
        Console.WriteLine ("Total files read:      " + songs_read);
        Console.WriteLine ("Average time per file: " + new TimeSpan ((end - start).Ticks / songs_read));
    }
}

public class VfsFileAbstraction : TagLib.File.IFileAbstraction
{
    private string name;

    public VfsFileAbstraction(string file)
    {
        name = file;
    }

    public string Name {
        get { return name; }
    }

    public System.IO.Stream ReadStream {
        get { return new VfsStream(Name, System.IO.FileMode.Open); }
    }

    public System.IO.Stream WriteStream {
        get { return new VfsStream(Name, System.IO.FileMode.Open); }
    }
    
    public void CloseStream (System.IO.Stream stream)
    {
       stream.Close ();
    }
}
