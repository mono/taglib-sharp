// mcs ReadFromUri.cs /pkg:gnome-vfs-sharp-2.0 /pkg:taglib-sharp

using TagLib;
using Gnome.Vfs;

public class ReadFromUri
{
   public static void Main (string [] args)
   {
      if (args.Length != 1)
      {
         System.Console.WriteLine ("USAGE: mono ReadFromUri.exe PATH");
         return;
      }
      
      // Connect to Gnome VFS
      Gnome.Vfs.Vfs.Initialize ();
      
      // Use our custom abstraction.
      TagLib.File.SetFileAbstractionCreator (new TagLib.File.FileAbstractionCreator (VfsFileAbstraction.CreateFile));
      
      // Get the file.
      TagLib.File file = TagLib.File.Create (args [0]);
      
      // ERROR! Give up.
      if (file == null)
      {
         System.Console.WriteLine ("UNREADABLE FILE");
         Gnome.Vfs.Vfs.Shutdown ();
         return;
      }
      
      // All sorts of info is available.
      System.Console.WriteLine ("Title:      " +  file.Tag.Title);
      System.Console.WriteLine ("Artists:    " + (file.Tag.Artists    == null ? "" : System.String.Join ("\n            ", file.Tag.Artists)));
      System.Console.WriteLine ("Performers: " + (file.Tag.Performers == null ? "" : System.String.Join ("\n            ", file.Tag.Performers)));
      System.Console.WriteLine ("Composers:  " + (file.Tag.Composers  == null ? "" : System.String.Join ("\n            ", file.Tag.Composers)));
      System.Console.WriteLine ("Album:      " +  file.Tag.Album);
      System.Console.WriteLine ("Comment:    " +  file.Tag.Comment);
      System.Console.WriteLine ("Genres:     " + (file.Tag.Genres     == null ? "" : System.String.Join ("\n            ", file.Tag.Genres)));
      System.Console.WriteLine ("Year:       " +  file.Tag.Year);
      System.Console.WriteLine ("Track:      " +  file.Tag.Track);
      System.Console.WriteLine ("TrackCount: " +  file.Tag.TrackCount);
      System.Console.WriteLine ("Disc:       " +  file.Tag.Disc);
      System.Console.WriteLine ("DiscCount:  " +  file.Tag.DiscCount);

      System.Console.WriteLine ("Length:     " + file.AudioProperties.Duration);
      System.Console.WriteLine ("Bitrate:    " + file.AudioProperties.Bitrate);
      System.Console.WriteLine ("SampleRate: " + file.AudioProperties.SampleRate);
      System.Console.WriteLine ("Channels:   " + file.AudioProperties.Channels);
      
      // Disconnect from Gnome VFS
      Gnome.Vfs.Vfs.Shutdown ();
   }
}

public class VfsFileAbstraction : TagLib.File.IFileAbstraction
{
   private string name;
   private FilePermissions permissions;
   
   public VfsFileAbstraction (string file)
   {
      name = file;
      
      permissions = (new FileInfo (name, FileInfoOptions.FollowLinks | FileInfoOptions.GetAccessRights)).Permissions;
      
      if (!IsReadable)
         throw new System.Exception ("File is not readable.");
   }
   
   public string Name {get {return name;}}
   
   public System.IO.Stream ReadStream
   {
      get {return new VfsStream (Name, System.IO.FileMode.Open);}
   }
   
   public System.IO.Stream WriteStream
   {
      get {return new VfsStream (Name, System.IO.FileMode.Open);}
   }
            
   public bool IsReadable
   {
      get {return (permissions | FilePermissions.AccessReadable) != 0;}
   }
   
   public bool IsWritable
   {
      get {return (permissions | FilePermissions.AccessWritable) != 0;}
   }
   
   public static TagLib.File.IFileAbstraction CreateFile (string path)
   {
      return new VfsFileAbstraction (path);
   }
}
