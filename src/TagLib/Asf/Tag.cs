/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace TagLib.Asf
{
   public class Tag : TagLib.Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ContentDescriptionObject         description;
      private ExtendedContentDescriptionObject ext_description;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag () : base ()
      {
         Clear ();
      }
      
      public Tag (HeaderObject header) : this ()
      {
         if (header == null)
            throw new ArgumentNullException ("header");
         
         foreach (Object child in header.Children)
         {
            if (child is ContentDescriptionObject)
               description = (ContentDescriptionObject) child;
            
            if (child is ExtendedContentDescriptionObject)
               ext_description = (ExtendedContentDescriptionObject) child;
         }
      }
      
      public override void Clear ()
      {
         description     = new ContentDescriptionObject ();
         ext_description = new ExtendedContentDescriptionObject ();
      }
      
      public void RemoveDescriptors (string name)
      {
         ext_description.RemoveDescriptors (name);
      }
      
      public IEnumerable<ContentDescriptor> GetDescriptors (params string [] names)
      {
         return ext_description.GetDescriptors (names);
      }

      public void SetDescriptors (string name, params ContentDescriptor [] descriptors)
      {
         ext_description.SetDescriptors (name, descriptors);
      }
      
      public void AddDescriptor (ContentDescriptor descriptor)
      {
         ext_description.AddDescriptor (descriptor);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagTypes TagTypes {get {return TagTypes.Asf;}}
      
      public override string Title
      {
         get {return description.Title;}
         set {description.Title = value;}
      }
      
      public override string [] AlbumArtists
      {
         get {return GetDescriptorStrings ("WM/AlbumArtist", "AlbumArtist");}
         set {SetDescriptorStrings (value, "WM/AlbumArtist", "AlbumArtist");}
      }
      
      public override string [] Performers
      {
         get {return SplitAndClean (description.Author);}
         set {description.Author = String.Join ("; ", value);}
      }
      
      public override string [] Composers
      {
         get {return GetDescriptorStrings ("WM/Composer", "Composer");}
         set {SetDescriptorStrings (value, "WM/Composer", "Composer");}
      }
      
      public override string Album
      {
         get {return GetDescriptorString ("WM/AlbumTitle", "Album");}
         set {SetDescriptorString (value, "WM/AlbumTitle", "Album");}
      }
      
      public override string Comment
      {
         get {return description.Description;}
         set {description.Description = value;}
      }
      public override string [] Genres
      {
         get
         {
            string value = GetDescriptorString ("WM/Genre", "WM/GenreID", "Genre");
            if (value == null || value.Trim ().Length == 0)
               return new string [] {};
            
            StringCollection l = StringCollection.Split (value, ";");
            for (int i = 0; i < l.Count; i ++)
            {
               string genre = l [i].Trim ();
               
               byte genre_id;
               int closing = genre.IndexOf (')');
               if (closing > 0 && genre[0] == '(' && byte.TryParse (genre.Substring (1, closing - 1), out genre_id))
                  genre = TagLib.Genres.IndexToAudio (genre_id);
               
               l [i] = genre;
            }
            return l.ToArray ();
         }
         set
         {
            SetDescriptorString (String.Join ("; ", value), "WM/Genre", "Genre");
         }
      }
      
      public override uint Year
      {
         get
         {
            string text = GetDescriptorString ("WM/Year");
            uint value;
            
            if (text != null && uint.TryParse (text.Length > 4 ? text.Substring (0, 4) : text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
               return value;
            
            return 0;
         }
         set
         {
            if (value == 0)
               RemoveDescriptors ("WM/Year");
            else
               SetDescriptorString (value.ToString (CultureInfo.InvariantCulture), "WM/Year");
         }
      }
      
      public override uint Track
      {
         get
         {
            foreach (ContentDescriptor desc in GetDescriptors ("WM/TrackNumber"))
               if (desc.ToDWord () != 0)
                  return desc.ToDWord ();
            
            return 0;
         }
         set
         {
            if (value == 0)
               RemoveDescriptors ("WM/TrackNumber");
            else
               SetDescriptors ("WM/TrackNumber", new ContentDescriptor ("WM/TrackNumber", value));
         }
      }
      
      // This is not defined in the spec. If correct methods come along, correct.
      public override uint TrackCount
      {
         get
         {
            foreach (ContentDescriptor desc in GetDescriptors ("TrackTotal"))
               if (desc.ToDWord () != 0)
                  return desc.ToDWord ();
            
            return 0;
         }
         set
         {
            if (value == 0)
               RemoveDescriptors ("TrackTotal");
            else
               SetDescriptors ("TrackTotal", new ContentDescriptor ("TrackTotal", value));
         }
      }
      
      public override uint Disc
      {
         get
         {
            string text = GetDescriptorString ("WM/PartOfSet");
            string[] texts;
            uint value;
            
            return (text != null && (texts = text.Split ('/')).Length > 0 && uint.TryParse (texts [0], NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) ? value : 0;
         }
         set
         {
            uint count = DiscCount;
            if (value == 0 && count == 0)
               RemoveDescriptors ("WM/PartOfSet");
            else if (count != 0)
               SetDescriptorString (value.ToString (CultureInfo.InvariantCulture) + "/" + count.ToString (CultureInfo.InvariantCulture), "WM/PartOfSet");
            else
               SetDescriptorString (value.ToString (CultureInfo.InvariantCulture), "WM/PartOfSet");
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            string text = GetDescriptorString ("WM/PartOfSet");
            string[] texts;
            uint value;
            
            return (text != null && (texts = text.Split ('/')).Length > 1 && uint.TryParse (texts [1], NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) ? value : 0;
         }
         set
         {
            uint disc = Disc;
            if (value == 0 && disc == 0)
               RemoveDescriptors ("WM/PartOfSet");
            else if (value != 0)
               SetDescriptorString (disc.ToString (CultureInfo.InvariantCulture) + "/" + value.ToString (CultureInfo.InvariantCulture), "WM/PartOfSet");
            else
               SetDescriptorString (disc.ToString (CultureInfo.InvariantCulture), "WM/PartOfSet");
         }
      }
		
      public override string Lyrics
      {
         get {return GetDescriptorString ("WM/Lyrics");}
         set {SetDescriptorString (value, "WM/Lyrics");}
      }
      
      public override string Copyright
      {
         get {return GetDescriptorString ("Copyright");}
         set {SetDescriptorString (value, "Copyright");}
      }
      
      public override string Grouping
      {
         get {return GetDescriptorString ("WM/ContentGroupDescription");}
         set {SetDescriptorString (value, "WM/ContentGroupDescription");}
      }
      
      public override string Conductor
      {
         get {return GetDescriptorString ("WM/Conductor");}
         set {SetDescriptorString (value, "WM/Conductor");}
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            foreach (ContentDescriptor desc in GetDescriptors ("WM/BeatsPerMinute"))
               if (desc.ToDWord () != 0)
                  return desc.ToDWord ();
           
            return 0;
         }
         set
         {
            if (value == 0)
               RemoveDescriptors ("WM/BeatsPerMinute");
            else
               SetDescriptors ("WM/BeatsPerMinute", new ContentDescriptor ("WM/BeatsPerMinute", value));
         }
      }
    
      public override IPicture [] Pictures
      {
         get
         {
         	List<IPicture> l = new List<IPicture> ();
         	
            foreach (ContentDescriptor descriptor in GetDescriptors ("WM/Picture"))
         	{
         		ByteVector data = descriptor.ToByteVector ();
            	Picture p = new Picture ();
            	
            	if (data.Count < 9)
            		continue;
            	
            	int offset = 0;
            	p.Type = (PictureType) data [0];
         	   offset += 1;
         	   int size = (int) data.Mid (offset, 4).ToUInt (false);
         		offset += 4;
         		
         		int found = data.Find (ByteVector.TextDelimiter (StringType.UTF16LE), offset, 2);
         		if (found == -1)
         			continue;
         		p.MimeType = data.Mid (offset, found - offset).ToString (StringType.UTF16LE);
         		offset = found + 2;
         		
         		found = data.Find (ByteVector.TextDelimiter (StringType.UTF16LE), offset, 2);
         		if (found == -1)
         			continue;
         		p.Description = data.Mid (offset, found - offset).ToString (StringType.UTF16LE);
         		offset = found + 2;
         		
         		p.Data = data.Mid (offset, size);
         		
            	l.Add (p);
            }
            
            return l.ToArray ();
         }
         
         set
         {
         	if (value == null || value.Length == 0)
         	{
         	   RemoveDescriptors ("WM/Picture");
         		return;
         	}
         	
         	List<ContentDescriptor> descriptors = new List<ContentDescriptor> ();
         	for (int i = 0; i < value.Length; i ++)
         	{
         		ByteVector v = new ByteVector ((byte) value [i].Type);
         		v.Add (Object.RenderDWord ((uint) value [i].Data.Count));
         		v.Add (Object.RenderUnicode (value [i].MimeType));
         		v.Add (Object.RenderUnicode (value [i].Description));
         		
         		/// If its too big, ignore it.
         		if (v.Count > 0xFFFF - value [i].Data.Count)
         			continue;
         		
         		v.Add (value [i].Data);
         		descriptors [i] = new ContentDescriptor ("WM/Picture", v);
         	}
         	
            SetDescriptors ("WM/Picture", descriptors.ToArray ());
         }
      }
      
      public override bool IsEmpty
      {
         get
         {
            return description.IsEmpty && ext_description.IsEmpty;
         }
      }
      
      
      public ContentDescriptionObject         ContentDescriptionObject         {get {return description;}}
      public ExtendedContentDescriptionObject ExtendedContentDescriptionObject {get {return ext_description;}}

      public string GetDescriptorString (params string [] names)
      {
         foreach (ContentDescriptor desc in GetDescriptors (names))
            if (desc != null && desc.Type == DataType.Unicode && desc.ToString () != null)
               return desc.ToString ();
         
         return null;
      }
      
      public string [] GetDescriptorStrings (params string [] names)
      {
         return SplitAndClean (GetDescriptorString (names));
      }
      
      public void SetDescriptorString (string value, params string [] names)
      {
         if (names == null)
            throw new ArgumentNullException ("names");
         
         int i = (value != null && value.Trim ().Length != 0) ? 1 : 0;
         
         if (i == 1)
            SetDescriptors (names [0], new ContentDescriptor (names [0], value));
         
         for (; i < names.Length; i ++)
            RemoveDescriptors (names [i]);
      }
		
      public void SetDescriptorStrings (string [] value, params string [] names)
      {
         SetDescriptorString (String.Join ("; ", value), names);
      }
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private static string [] SplitAndClean (string s)
      {
         if (s == null || s.Trim ().Length == 0)
            return new string [] {};
         
         StringCollection l = StringCollection.Split (s, ";");
         for (int i = 0; i < l.Count; i ++)
            l [i] = l [i].Trim ();
         return l.ToArray ();
      }
   }
}
