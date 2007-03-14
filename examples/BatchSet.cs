using System;
using System.Collections;
using System.Collections.Specialized;
using TagLib;

public class BatchSet
{
	private enum Mode
	{
		Tag, Value, File
	}
	
    public static void Main(string [] args)
    {
        if(args.Length < 3) {
            Console.Error.WriteLine("USAGE: mono BatchSet.exe -tag value [-tag2 value ...] AUDIO_PATH [...]");
            return;
        }
        
        Mode mode = Mode.Tag;
        ArrayList files = new ArrayList ();
        NameValueCollection tags  = new NameValueCollection ();
        
        string tag = null;
        
        foreach (string str in args)
        {
        	if (mode == Mode.Tag)
        	{
        		
        		if (str [0] == '-')
        		{
	        		if (str == "--")
        				mode = Mode.File;
        			else
        			{
        				tag = str.Substring (1);
        				mode = Mode.Value;
        			}
        			continue;
        		}
        		mode = Mode.File;
        	}
        	
        	if (mode == Mode.Value)
        	{
        		try {tags.Add (tag, str);} catch {}
        		mode = Mode.Tag;
        		continue;
        	}
        	
        	if (mode == Mode.File)
        		files.Add (str);
        }
        
        foreach (string filename in files)
        {
	        TagLib.File file = TagLib.File.Create (filename);
	        if (file == null)
	        	continue;
	        
        	Console.WriteLine ("Updating Tags For: " + filename);
        
        	foreach (string key in tags.AllKeys)
        	{
        		try
        		{
        			switch (key)
        			{
        				case "album":
        					file.Tag.Album = tags [key];
        					break;
        				case "artists":
        					file.Tag.AlbumArtists = tags [key].Split (new char [] {';'});
        					break;
        				case "comment":
        					file.Tag.Comment = tags [key];
        					break;
        				case "lyrics":
        					file.Tag.Lyrics = tags [key];
        					break;
        				case "composers":
        					file.Tag.Composers = tags [key].Split (new char [] {';'});
        					break;
        				case "disc":
        					file.Tag.Disc = UInt32.Parse (tags [key]);
        					break;
        				case "disccount":
        					file.Tag.DiscCount = UInt32.Parse (tags [key]);
        					break;
        				case "genres":
        					file.Tag.Genres = tags [key].Split (new char [] {';'});
        					break;
        				case "performers":
        					file.Tag.Performers = tags [key].Split (new char [] {';'});
        					break;
        				case "title":
        					file.Tag.Title = tags [key];
        					break;
        				case "track":
        					file.Tag.Track = UInt32.Parse (tags [key]);
        					break;
        				case "trackcount":
        					file.Tag.TrackCount = UInt32.Parse (tags [key]);
        					break;
        				case "year":
        					file.Tag.Year = UInt32.Parse (tags [key]);
        					break;
        			}
        		}
        		catch (Exception e)
        		{
        			Console.WriteLine ("Error setting tag " + key + ":");
        			Console.WriteLine (e);
        		}
        	}
        	
	        file.Save();
	    }
    }
}
