/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
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

using System.Collections;
using System;

namespace TagLib.Ape
{
   public class Tag : TagLib.Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private long      tag_offset;
      private Footer    footer;
      private Hashtable items;
      
      
      //////////////////////////////////////////////////////////////////////////
      // static properties
      //////////////////////////////////////////////////////////////////////////
      public static readonly ByteVector FileIdentifier = Footer.FileIdentifier;


      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Tag () : base ()
      {
         tag_offset = -1;
         footer = new Footer ();
         items = new Hashtable ();
      }
      
      public Tag (File file, long tag_offset) : this ()
      {
         this.tag_offset = tag_offset;
         Read (file);
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         uint item_count = 0;

         foreach (Item item in items.Values)
         {
            data.Add (item.Render ());
            item_count ++;
         }
         
         footer.ItemCount     = item_count;
         footer.TagSize       = (uint) (data.Count + Footer.Size);
         footer.HeaderPresent = true;

         return footer.RenderHeader () + data + footer.RenderFooter ();
      }
      
      public void RemoveItem (string key)
      {
         items.Remove (key.ToUpper ());
      }
      
      public Item GetItem (string key)
      {
         return items.ContainsKey (key.ToUpper ()) ? (Item) items [key.ToUpper ()] : null;
      }

      public void AddValue (string key, string value, bool replace)
      {
         if (replace)
            RemoveItem (key);
         
         if (value != null && value != "")
         {
            StringList l = new StringList ();
            
            if (GetItem (key) != null && !replace)
               l.Add (GetItem (key).ToStringArray ());
            
            l.Add (value);
                    
            SetItem (key, new Item (key, l));
         }
      }
      
      public void AddValue (string key, string value)
      {
         AddValue (key, value, true);
      }
      
      public void AddValues (string key, string [] values, bool replace)
      {
         if (replace)
            RemoveItem (key);
         
         if (values != null)
            foreach (string s in values)
               AddValue (key, s, false);
      }

      public void AddValues (string key, string [] values)
      {
         AddValues (key, values, true);
      }

      public void SetItem (string key, Item item)
      {
         if (items.ContainsKey (key.ToUpper ()))
            items [key.ToUpper ()] = item;
         else
            items.Add (key.ToUpper (), item);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override string Title
      {
         get
         {
            Item item = GetItem ("TITLE");
            return item != null ? item.ToString () : null;
         }
         set
         {
            AddValue ("TITLE", value, true);
         }
      }
      
      public override string [] Artists
      {
         get
         {
            Item item = GetItem ("ARTIST");
            return item != null ? item.ToStringArray () : new string [] {};
         }
         set
         {
            AddValues ("ARTIST", value, true);
         }
      }
      
      public override string [] Performers
      {
         get
         {
            Item item = GetItem ("PERFORMER");
            return item != null ? item.ToStringArray () : new string [] {};
         }
         set
         {
            AddValues ("PERFORMER", value, true);
         }
      }
      
      public override string [] Composers
      {
         get
         {
            Item item = GetItem ("COMPOSER");
            return item != null ? item.ToStringArray () : new string [] {};
         }
         set
         {
            AddValues ("COMPOSER", value, true);
         }
      }
      
      public override string Album
      {
         get
         {
            Item item = GetItem ("ALBUM");
            return item != null ? item.ToString () : null;
         }
         set
         {
            AddValue ("ALBUM", value, true);
         }
      }
      
      public override string Comment
      {
         get
         {
            Item item = GetItem ("COMMENT");
            return item != null ? item.ToString () : null;
         }
         set
         {
            AddValue ("COMMENT", value, true);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            Item item = GetItem ("GENRE");
            return item != null ? item.ToStringArray () : new string [] {};
         }
         set
         {
            AddValues ("GENRE", value, true);
         }
      }
      public override uint Year
      {
         get
         {
            try
            {
               if (GetItem ("YEAR") != null)
                  return UInt32.Parse (GetItem ("YEAR").ToString ().Substring (0, 4));
            }
            catch {}
            
            return 0;
         }
         set
         {
            AddValue ("YEAR", value.ToString (), true);
         }
      }
      
      public override uint Track
      {
         get
         {
            try
            {
               if (GetItem ("TRACK") != null)
                  return UInt32.Parse (GetItem ("TRACK").ToString ().Split (new char [] {'/'}) [0]);
            }
            catch {}
            
            return 0;
         }
         set
         {
            uint count = TrackCount;
            if (count != 0)
               AddValue ("TRACK", value + "/" + count, true);
            else
               AddValue ("TRACK", value.ToString (), true);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            try
            {
               if (GetItem ("TRACK") != null)
                  return UInt32.Parse (GetItem ("TRACK").ToString ().Split (new char [] {'/'}) [1]);
            }
            catch {}
            
            return 0;
         }
         set
         {
            AddValue ("TRACK", Track + "/" + value, true);
         }
      }
      
      public override uint Disc
      {
         get
         {
            try
            {
               if (GetItem ("DISC") != null)
                  return UInt32.Parse (GetItem ("DISC").ToString ().Split (new char [] {'/'}) [0]);
            }
            catch {}
            
            return 0;
         }
         set
         {
            uint count = DiscCount;
            if (count != 0)
               AddValue ("DISC", value + "/" + count, true);
            else
               AddValue ("DISC", value.ToString (), true);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            try
            {
               if (GetItem ("DISC") != null)
                  return UInt32.Parse (GetItem ("DISC").ToString ().Split (new char [] {'/'}) [1]);
            }
            catch {}
            
            return 0;
         }
         set
         {
            AddValue ("DISC", Disc + "/" + value, true);
         }
      }
      
      public override bool IsEmpty {get {return items.Count == 0;}}
      
      public Footer Footer {get {return footer;}}
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected void Read (File file)
      {
         if (file == null)
            return;
            
         try {file.Mode = File.AccessMode.Read;}
         catch {return;}
         
         file.Seek (tag_offset);
         footer.SetData (file.ReadBlock ((int) Footer.Size));

         if(footer.TagSize == 0 || footer.TagSize > (uint) file.Length)
            return;

         file.Seek (tag_offset + Footer.Size - footer.TagSize);
         Parse (file.ReadBlock ((int) (footer.TagSize - Footer.Size)));
      }

      protected void Parse (ByteVector data)
      {
         int pos = 0;
         
         // 11 bytes is the minimum size for an APE item
         for (uint i = 0; i < footer.ItemCount && pos <= data.Count - 11; i++)
         {
            Item item = new Item ();
            item.Parse (data.Mid (pos));
            
            SetItem (item.Key.ToUpper (), item);
            
            pos += item.Size;
         }
      }
   }
}
