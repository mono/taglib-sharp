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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TagLib.Ape
{
   public class Tag : TagLib.Tag, IEnumerable<string>
   {
      #region Private Properties
      private Footer _footer = new Footer ();
      private List<Item> items = new List<Item> ();
      #endregion
      
      
      
      #region Public Static Properties
      public static readonly ReadOnlyByteVector FileIdentifier = Footer.FileIdentifier;
      #endregion
      
      
      
      #region Constructors
      public Tag ()
      {}
      
		public Tag (TagLib.File file, long position)
		{
			if (file == null)
				throw new ArgumentNullException ("file");
			
			if (position < 0 ||
				position > file.Length - Footer.Size)
				throw new ArgumentOutOfRangeException (
					"position");
			
			Read (file, position);
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		public override TagTypes TagTypes {
			get {return TagTypes.Ape;}
		}
		
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
            Item item = GetItem ("ALBUM ARTIST");
            return item != null ? item.ToStringArray () : new string [] {};
         }
         set
         {
            SetValue ("ALBUM ARTIST", value);
         }
      }
      
      public override string [] Performers
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
            double value;
            
            if ((item = GetItem ("TEMPO")) != null && double.TryParse (item.ToString (), out value))
               return (uint) Math.Round (value);
            
            return 0;
         }
         set
         {
            SetValue ("TEMPO", value, 0);
         }
      }
      
		public override IPicture [] Pictures {
			get {
				Item item = GetItem ("Cover Art (front)");
				if (item == null || item.Type != ItemType.Binary)
					return new IPicture [0];
				
				int index = item.Value.Find (
					ByteVector.TextDelimiter (StringType.UTF8));
				
				if (index < 0)
					return new IPicture [0];
				
				Picture pic = new Picture (item.Value.Mid (index + 1));
				pic.Description = item.Value.Mid (0, index
					).ToString (StringType.UTF8);
				
				return new IPicture [] {pic};
			}
			set {
				if (value == null || value.Length == 0)
					RemoveItem ("Cover Art (front)");
				
				ByteVector data = ByteVector.FromString (
					value [0].Description, StringType.UTF8);
				data.Add (ByteVector.TextDelimiter (
					StringType.UTF8));
				data.Add (value [0].Data);
				
				SetItem (new Item ("Cover Art (front)", data));
			}
		}
		
      public bool HeaderPresent
      {
         get {return (_footer.Flags & FooterFlags.HeaderPresent) != 0;}
         set
         {
            if (value)
               _footer.Flags |= FooterFlags.HeaderPresent;
            else
               _footer.Flags &= ~FooterFlags.HeaderPresent;
         }
      }
      
      public override bool IsEmpty {get {return items.Count == 0;}}
      #endregion
      
      
      
		#region Public Methods
		
		public IEnumerator<string> GetEnumerator ()
		{
			foreach (Item item in items)
				yield return item.Key;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator ();
		}
      
		public void AddValue (string key, uint number, uint count)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (number == 0 && count == 0)
				return;
			else if (count != 0)
				AddValue (key, string.Format (
					CultureInfo.InvariantCulture, "{0}/{1}",
					number, count));
			else
				AddValue (key, number.ToString (
					CultureInfo.InvariantCulture));
		}
		
		public void SetValue (string key, uint number, uint count)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (number == 0 && count == 0)
				RemoveItem (key);
			else if (count != 0)
				SetValue (key, string.Format (
					CultureInfo.InvariantCulture, "{0}/{1}",
					number, count));
			else
				SetValue (key, number.ToString (
					CultureInfo.InvariantCulture));
		}
		
		public void AddValue (string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (string.IsNullOrEmpty (value))
				return;
			
			AddValue (key, new string [] {value});
		}
		
		public void SetValue (string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (string.IsNullOrEmpty (value))
				RemoveItem (key);
			else
				SetValue (key, new string [] {value});
		}
		
		public void AddValue (string key, string [] value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (value == null || value.Length == 0)
				return;
			
			int index = GetItemIndex (key);
			
			List<string> values = new List<string> ();
			
			if (index >= 0)
				values.AddRange (items [index].ToStringArray ());
			
			values.AddRange (value);
			
			Item item = new Item (key, values.ToArray ());
			
			if (index >= 0)
				items [index] = item;
			else
				items.Add (item);
		}
		
		public void SetValue (string key, string [] value)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (value == null || value.Length == 0) {
				RemoveItem (key);
				return;
			}
			
			Item item = new Item (key, value);
			
			int index = GetItemIndex (key);
			if (index >= 0)
				items [index] = item;
			else
				items.Add (item);
			
		}
      
		public Item GetItem (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			StringComparison comparison =
				StringComparison.InvariantCultureIgnoreCase;
			
			foreach (Item item in items)
				if (key.Equals (item.Key, comparison))
					return item;
			
			return null;
		}
		
		public void SetItem (Item item)
		{
			if (item == null)
				throw new ArgumentNullException ("item");
			
			int index = GetItemIndex (item.Key);
			if (index >= 0)
				items [index] = item;
			else
				items.Add (item);
		}
		
		public void RemoveItem (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			StringComparison comparison =
				StringComparison.InvariantCultureIgnoreCase;
			
			for (int i = items.Count - 1; i >= 0; i --)
				if (key.Equals (items [i].Key, comparison))
					items.RemoveAt (i);
		}
		
		public ByteVector Render ()
		{
			ByteVector data = new ByteVector ();
			uint item_count = 0;
			
			foreach (Item item in items) {
				data.Add (item.Render ());
				item_count ++;
			}
			
			_footer.ItemCount = item_count;
         _footer.TagSize   = (uint) (data.Count + Footer.Size);
         HeaderPresent    = true;

         data.Insert (0, _footer.RenderHeader ());
         data.Add (_footer.RenderFooter ());
         return data;
      }
      
		public override void Clear ()
		{
			items.Clear ();
		}
		
		#endregion
		
		
		
		#region Protected Methods
		
		protected void Read (TagLib.File file, long position)
		{
			file.Mode = File.AccessMode.Read;
         file.Seek (position);
         _footer = new Footer (file.ReadBlock ((int)Footer.Size));
         
         if(_footer.TagSize == 0 || _footer.TagSize > file.Length)
            throw new CorruptFileException ("Tag size out of bounds.");
      	
      	// If we've read a header, we don't have to seek to read the content.
      	// If we've read a footer, we need to move back to the start of the
      	// tag.
      	if ((_footer.Flags & FooterFlags.IsHeader) == 0)
            file.Seek (position + Footer.Size - _footer.TagSize);
      	
      	Parse (file.ReadBlock ((int)(_footer.TagSize - Footer.Size)));
      }

		protected void Parse (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			int pos = 0;
			
			try {
				// 11 bytes is the minimum size for an APE item
				for (uint i = 0; i < _footer.ItemCount &&
					pos <= data.Count - 11; i++) {
					Item item = new Item (data, pos);
					SetItem (item);
					pos += item.Size;
				}
			} catch (CorruptFileException) {
				// A corrupt item was encountered, considered
				// the tag finished with what has been read.
			}
		}
		
		#endregion
		
		
		
		#region Private Methods
		
		private int GetItemIndex (string key)
		{
			StringComparison comparison =
				StringComparison.InvariantCultureIgnoreCase;
			
			for (int i = 0; i < items.Count; i ++)
				if (key.Equals (items [i].Key, comparison))
					return i;
			
			return -1;
		}
		
		#endregion
	}
}
