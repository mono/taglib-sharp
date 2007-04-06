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
        
        TagLib.File.SetFileAbstractionCreator(new TagLib.File.FileAbstractionCreator(
            VfsFileAbstraction.CreateFile));
        
        DateTime start = DateTime.Now;
        int songs_read = 0;
        try {
            foreach (string path in args)
            {
                string uri = path;
                Console.WriteLine (uri);
                
                try {
                    System.IO.FileInfo file_info = new System.IO.FileInfo(uri);
                    uri = Gnome.Vfs.Uri.GetUriFromLocalPath (file_info.FullName);
                } catch {
                }
                
                TagLib.File file = TagLib.File.Create(uri);
                
                Console.WriteLine("Title:      " +  file.Tag.Title);
                Console.WriteLine("Artists:    " + (file.Tag.AlbumArtists == null ? String.Empty : System.String.Join ("\n            ", file.Tag.AlbumArtists)));
                Console.WriteLine("Performers: " + (file.Tag.Performers   == null ? String.Empty : System.String.Join ("\n            ", file.Tag.Performers)));
                Console.WriteLine("Composers:  " + (file.Tag.Composers    == null ? String.Empty : System.String.Join ("\n            ", file.Tag.Composers)));
                Console.WriteLine("Album:      " +  file.Tag.Album);
                Console.WriteLine("Comment:    " +  file.Tag.Comment);
                Console.WriteLine("Genres:     " + (file.Tag.Genres       == null ? String.Empty : System.String.Join ("\n            ", file.Tag.Genres)));
                Console.WriteLine("Year:       " +  file.Tag.Year);
                Console.WriteLine("Track:      " +  file.Tag.Track);
                Console.WriteLine("TrackCount: " +  file.Tag.TrackCount);
                Console.WriteLine("Disc:       " +  file.Tag.Disc);
                Console.WriteLine("DiscCount:  " +  file.Tag.DiscCount);
                Console.WriteLine("Lyrics:\n"    +  file.Tag.Lyrics + "\n");
                
                if ((file.Properties.MediaTypes & TagLib.MediaTypes.Audio) != TagLib.MediaTypes.Unknown)
                {
                    Console.WriteLine("Audio Properties");
                    Console.WriteLine("Bitrate:    " + file.Properties.AudioBitrate);
                    Console.WriteLine("SampleRate: " + file.Properties.AudioSampleRate);
                    Console.WriteLine("Channels:   " + file.Properties.AudioChannels + "\n");
                }
                
                if ((file.Properties.MediaTypes & TagLib.MediaTypes.Video) != TagLib.MediaTypes.Unknown)
                {
                    Console.WriteLine("Video Properties");
                    Console.WriteLine("Width:      " + file.Properties.VideoWidth);
                    Console.WriteLine("Height:     " + file.Properties.VideoHeight + "\n");
                }
                
                if (file.Properties.MediaTypes != TagLib.MediaTypes.Unknown)
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
    private FilePermissions permissions;

    public VfsFileAbstraction(string file)
    {
        name = file;
        permissions = (new FileInfo(name, FileInfoOptions.FollowLinks | 
            FileInfoOptions.GetAccessRights)).Permissions;

        if(!IsReadable) {
            throw new System.IO.IOException("File \"" + name + "\" is not readable.");
        }
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

    public bool IsReadable {
        get { return (permissions | FilePermissions.AccessReadable) != 0; }
    }

    public bool IsWritable {
        get { return (permissions | FilePermissions.AccessWritable) != 0; }
    }

    public static TagLib.File.IFileAbstraction CreateFile(string path)
    {
        return new VfsFileAbstraction(path);
    }
}
