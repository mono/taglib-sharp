/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : apetag.cpp from TagLib
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
using System.Collections.Generic;
using System;

namespace TagLib.Ape
{
   public class Tag : TagLib.Tag
   {
      #region Private Properties
      private Footer footer = new Footer ();
      private Dictionary<string,Item> items = new Dictionary<string,Item> ();
      #endregion
      
      
      
      #region Public Static Properties
      public static readonly ByteVector FileIdentifier = Footer.FileIdentifier;
      #endregion
      
      
      
      #region Constructors
      public Tag () : base ()
      {}
      
      public Tag (File file, long offset) : this ()
      {
         Read (file, offset);
      }
      #endregion
      
      
      
      #region Public Properties
      public override string Title
      {
         get
         {
            Item item = GetItem ("TITLE");
            return item != null ? item.ToString () : null;
         }
         set
         {
            SetValue ("TITLE", value);
         }
      }
      
      public override string [] AlbumArtists
      {
         get
         {
            Item item = GetItem ("ARTIST");
            return item != null ? item.ToStringArray () : new string [] {};
         }
         set
         {
            SetValue ("ARTIST", value);
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
            SetValue ("PERFORMER", value);
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
            SetValue ("COMPOSER", value);
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
            SetValue ("ALBUM", value);
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
            SetValue ("COMMENT", value);
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
            SetValue ("GENRE", value);
         }
      }
      public override uint Year
      {
         get
         {
            Item item = GetItem ("YEAR");
            if (item == null)
               return 0;
            
            string text = item.ToString ();
            uint value;
            
            if (uint.TryParse (text.Length > 4 ? text.Substring (0, 4) : text, out value))
               return value;
            
            return 0;
         }
         set
         {
            SetValue ("YEAR", value, 0);
         }
      }
      
      public override uint Track
      {
         get
         {
            Item item = GetItem ("TRACK");
            string [] values;
            uint value;
            
            if (item != null && (values = item.ToString ().Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetValue ("TRACK", value, TrackCount);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            Item item = GetItem ("TRACK");
            string [] values;
            uint value;
            
            if (item != null && (values = item.ToString ().Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetValue ("TRACK", Track, value);
         }
      }
      
      public override uint Disc
      {
         get
         {
            Item item = GetItem ("DISC");
            string [] values;
            uint value;
            
            if (item != null && (values = item.ToString ().Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetValue ("DISC", value, DiscCount);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            Item item = GetItem ("DISC");
            string [] values;
            uint value;
            
            if (item != null && (values = item.ToString ().Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetValue ("DISC", Disc, value);
         }
      }
      
      public override string Lyrics
      {
         get
         {
            Item item = GetItem ("LYRICS");
            return item != null ? item.ToString () : null;
         }
         set
         {
            SetValue ("LYRICS", value);
         }
      }
      
      public override string Copyright
      {
         get
         {
            Item item = GetItem ("COPYRIGHT");
            return item != null ? item.ToString () : null;
         }
         set
         {
            SetValue ("COPYRIGHT", value);
         }
      }
      
      public override string Conductor
      {
         get
         {
            Item item = GetItem ("CONDUCTOR");
            return item != null ? item.ToString () : null;
         }
         set
         {
            SetValue ("CONDUCTOR", value);
         }
      }
      
      public override string Grouping
      {
         get
         {
            Item item = GetItem ("GROUPING");
            return item != null ? item.ToString () : null;
         }
         set
         {
            SetValue ("GROUPING", value);
         }
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            Item item = GetItem ("TEMPO");
            uint value;
            
            if ((item = GetItem ("TEMPO")) != null && uint.TryParse (item.ToString (), out value))
               return value;
            
            return 0;
         }
         set
         {
            SetValue ("TEMPO", value, 0);
         }
      }
      
      public bool HeaderPresent
      {
         get {return (footer.Flags & FooterFlags.HeaderPresent) != 0;}
         set
         {
            if (value)
               footer.Flags |= FooterFlags.HeaderPresent;
            else
               footer.Flags &= ~FooterFlags.HeaderPresent;
         }
      }
      
      public override bool IsEmpty {get {return items.Count == 0;}}
      #endregion
      
      
      
      #region Public Methods
      public void AddValue (string key, uint number, uint count)
      {
         if (number == 0 && count == 0)
            return;
         else if (count != 0)
            AddValue (key, number.ToString () + "/" + count.ToString ());
         else
            AddValue (key, number.ToString ());
      }
      
      public void SetValue (string key, uint number, uint count)
      {
         RemoveItem (key);
         AddValue (key, number, count);
      }
      
      public void AddValue (string key, string value)
      {
         if (value != null && value != String.Empty)
         {
            StringList l = new StringList ();
            Item old_item = GetItem (key);
            if (old_item != null)
               l.Add (old_item.ToStringArray ());
            
            l.Add (value);
            
            SetItem (new Item (key, l));
         }
      }
      
      public void SetValue (string key, string value)
      {
         RemoveItem (key);
         AddValue (key, value);
      }
      
      public void AddValue (string key, string [] value)
      {
         if (value != null)
            foreach (string s in value)
               AddValue (key, s);
      }

      public void SetValue (string key, string [] value)
      {
         RemoveItem (key);
         AddValue (key, value);
      }
      
      public Item GetItem (string key)
      {
         return items.ContainsKey (key.ToUpper ()) ? items [key.ToUpper ()] : null;
      }
      
      public void SetItem (Item item)
      {
         if (items.ContainsKey (item.Key.ToUpper ()))
            items [item.Key.ToUpper ()] = item;
         else
            items.Add (item.Key.ToUpper (), item);
      }
      
      public void RemoveItem (string key)
      {
         items.Remove (key.ToUpper ());
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
         
         footer.ItemCount = item_count;
         footer.TagSize   = (uint) (data.Count + Footer.Size);
         HeaderPresent    = true;

         data.Insert (0, footer.RenderHeader ());
         data.Add (footer.RenderFooter ());
         return data;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected void Read (File file, long offset)
      {
         if (file == null)
            throw new ArgumentException ("File object is null.", "file");
         
         file.Mode = File.AccessMode.Read;
         file.Seek (offset);
         footer = new Footer (file.ReadBlock (Footer.Size));
         
         if(footer.TagSize == 0 || footer.TagSize > file.Length)
            throw new CorruptFileException ("Tag size out of bounds.");
      	
      	// If we've read a header, we don't have to seek to read the content.
      	// If we've read a footer, we need to move back to the start of the
      	// tag.
      	if ((footer.Flags & FooterFlags.IsHeader) == 0)
            file.Seek (offset + Footer.Size - footer.TagSize);
      	
      	Parse (file.ReadBlock (footer.TagSize - Footer.Size));
      }

      protected void Parse (ByteVector data)
      {
         int pos = 0;
         
         // 11 bytes is the minimum size for an APE item
         for (uint i = 0; i < footer.ItemCount && pos <= data.Count - 11; i++)
         {
            Item item = new Item (data, pos);
            
            SetItem (item);
            
            pos += item.Size;
         }
      }
      #endregion
   }
}
