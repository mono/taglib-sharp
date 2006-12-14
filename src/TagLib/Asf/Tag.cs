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
using System.Collections;

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
      public Tag (HeaderObject header) : base ()
      {
         description     = new ContentDescriptionObject ();
         ext_description = new ExtendedContentDescriptionObject ();
         
         foreach (Object child in header.Children)
         {
            if (child is ContentDescriptionObject)
               description = (ContentDescriptionObject) child;
            
            if (child is ExtendedContentDescriptionObject)
               ext_description = (ExtendedContentDescriptionObject) child;
         }
      }
      
      public void RemoveDescriptors (string name)
      {
         ext_description.RemoveDescriptors (name);
      }
      
      public ContentDescriptor [] GetDescriptors (string name)
      {
         return ext_description.GetDescriptors (name);
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
      public override string Title
      {
         get {return description.Title;}
         set {description.Title = value;}
      }
      
      // This may seem unintuitive, but the artists field is actually
      // performers. This makes sense as Artists should descibe the album
      // artist and Performers should describe who is in the song.
      // Because this may not be set, we'll return performers if we can't get
      // an artist.
      public override string [] AlbumArtists
      {
         get
         {
            string value = GetDescriptorString ("WM/AlbumArtist", "AlbumArtist");
            return (value != null) ? SplitAndClean (value) : Performers;
         }
         set {SetDescriptorString (String.Join ("; ", value), "WM/AlbumArtist", "AlbumArtist");}
      }
      
      public override string [] Performers
      {
         get {return SplitAndClean (description.Author);}
         set {description.Author = String.Join ("; ", value);}
      }
      
      public override string [] Composers
      {
         get {return SplitAndClean (GetDescriptorString ("WM/Composer", "Composer"));}
         set {SetDescriptorString (String.Join ("; ", value), "WM/Composer", "Composer");}
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
            string value = GetDescriptorString ("WM/Genre", "Genre");
            return (value != null) ? SplitAndClean (value) : new string [] {};
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
            string value = GetDescriptorString ("WM/Year");
            if (value != null)
               try
               {
                  return System.UInt32.Parse (value.Substring (0, 4));
               }
               catch {}
            
            return 0;
         }
         set
         {
            SetDescriptorString (value.ToString (), "WM/Year");
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
            SetDescriptors ("TrackTotal", new ContentDescriptor ("TrackTotal", value));
         }
      }
      
      public override uint Disc
      {
         get
         {
            string value = GetDescriptorString ("WM/PartOfSet");
            if (value != null)
               try
               {
                  return UInt32.Parse (value.Split (new char [] {'/'}) [0]);
               }
               catch {}
            
            return 0;
         }
         set
         {
            uint count = DiscCount;
            if (count != 0)
               SetDescriptorString (value.ToString () + "/" + count.ToString (), "WM/PartOfSet");
            else
               SetDescriptorString (value.ToString (), "WM/PartOfSet");
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            string value = GetDescriptorString ("WM/PartOfSet");
            if (value != null && value.IndexOf ('/') != -1)
	            try
   	         {
      	         return UInt32.Parse (value.Split (new char [] {'/'}) [1]);
         	   }
            	catch {}
            
            return 0;
         }
         set
         {
               SetDescriptorString (Disc.ToString () + "/" + value.ToString (), "WM/PartOfSet");
         }
      }
      
      public ContentDescriptionObject         ContentDescriptionObject         {get {return description;}}
      public ExtendedContentDescriptionObject ExtendedContentDescriptionObject {get {return ext_description;}}

      public string GetDescriptorString (params string [] names)
      {
         foreach (string name in names)
         {
            foreach (ContentDescriptor desc in GetDescriptors (name))
               if (desc != null && desc.Type == DataType.Unicode && desc.ToString () != null)
                  return desc.ToString ();
         }
         return null;
      }
      
      public void SetDescriptorString (string value, params string [] names)
      {
         int i = (value != null && value.Trim () != "") ? 1 : 0;
         
         if (i == 1)
            SetDescriptors (names [0], new ContentDescriptor (names [0], value));
         
         for (; i < names.Length; i ++)
            RemoveDescriptors (names [i]);
      }
		
      public override IPicture [] Pictures
      {
         get
         {
         	ArrayList l = new ArrayList ();
         	
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
         		
         		int found = data.Find ("\0\0", offset, 2);
         		if (found == -1)
         			continue;
         		p.MimeType = data.Mid (offset, found - offset).ToString (StringType.UTF16LE);
         		offset = found + 2;
         		
         		found = data.Find ("\0\0", offset, 2);
         		if (found == -1)
         			continue;
         		p.Description = data.Mid (offset, found - offset).ToString (StringType.UTF16LE);
         		offset = found + 2;
         		
         		p.Data = data.Mid (offset, size);
         		
            	l.Add (p);
            }
            
            return (Picture []) l.ToArray (typeof (Picture));
         }
         
         set
         {
         	if (value == null || value.Length == 0)
         	{
         	   RemoveDescriptors ("WM/Picture");
         		return;
         	}
         	
         	ContentDescriptor [] descriptors = new ContentDescriptor [value.Length];
         	for (int i = 0; i < value.Length; i ++)
         	{
         		ByteVector v = new ByteVector ((byte) value [i].Type);
         		v.Add (Object.RenderDWord ((uint) value [i].Data.Count));
         		v.Add (Object.RenderUnicode (value [i].MimeType));
         		v.Add (Object.RenderUnicode (value [i].Description));
         		v.Add (value [i].Data);
         		
            	descriptors [i] = new ContentDescriptor ("WM/Picture", v);
         	}
         	
            SetDescriptors ("WM/Picture", descriptors);
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private string [] SplitAndClean (string s)
      {
         if (s == null || s.Trim () == "")
            return new string [] {};
         
         StringList l = StringList.Split (s, ";");
         for (int i = 0; i < l.Count; i ++)
            l [i] = l [i].Trim ();
         return l.ToArray ();
      }
   }
}
