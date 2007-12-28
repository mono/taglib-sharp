//
// XiphComment.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   xiphcomment.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2003 Scott Wheeler (Original Implementation)
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TagLib.Ogg
{
   public class XiphComment : TagLib.Tag, IEnumerable<string>
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Dictionary<string,string[]> field_list = new Dictionary<string,string[]> ();
      private string vendor_id;
      private string comment_field = "DESCRIPTION";
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
		public XiphComment ()
		{
		}
		
		public XiphComment (ByteVector data)
		{
			Parse (data);
		}
      
      public override void Clear ()
      {
         field_list.Clear ();
      }
      
		public string [] GetField (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			key = key.ToUpper (CultureInfo.InvariantCulture);
			
			if (!field_list.ContainsKey (key))
				return new string [0];
			
			return (string []) field_list [key].Clone ();
		}
		
		public string GetFirstField (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			string [] values = GetField (key);
			return (values.Length > 0) ? values [0] : null;
		}
		
		public void SetField (string key, uint number)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			if (number == 0)
				RemoveField (key);
			else
				SetField (key, number.ToString (
					CultureInfo.InvariantCulture));
		}
		
		public void SetField (string key, params string [] values)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			key = key.ToUpper (CultureInfo.InvariantCulture);
			
			if (values == null || values.Length == 0) {
				RemoveField (key);
				return;
			}
			
			List <string> result = new List<string> ();
			foreach (string text in values)
				if (text != null && text.Trim ().Length != 0)
					result.Add (text);
			
			if (result.Count == 0)
				RemoveField (key);
			else if (field_list.ContainsKey (key))
				field_list [key] = result.ToArray ();
			else
				field_list.Add (key, result.ToArray ());
		}
		
		public void RemoveField (string key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			
			key = key.ToUpper (CultureInfo.InvariantCulture);
			
			field_list.Remove (key);
		}
      
      public ByteVector Render (bool addFramingBit)
      {
         ByteVector data = new ByteVector ();

         // Add the vendor ID length and the vendor ID.  It's important to use the
         // length of the data(String::UTF8) rather than the lenght of the the string
         // since this is UTF8 text and there may be more characters in the data than
         // in the UTF16 string.

         ByteVector vendor_data = ByteVector.FromString (vendor_id, StringType.UTF8);

         data.Add (ByteVector.FromUInt ((uint) vendor_data.Count, false));
         data.Add (vendor_data);

         // Add the number of fields.

         data.Add (ByteVector.FromUInt (FieldCount, false));

         foreach (KeyValuePair<string,string[]> entry in field_list)
         {
            // And now iterate over the values of the current list.

            foreach (string value in entry.Value) {
               ByteVector field_data = ByteVector.FromString (entry.Key, StringType.UTF8);
               field_data.Add ((byte) '=');
               field_data.Add (ByteVector.FromString (value, StringType.UTF8));

               data.Add (ByteVector.FromUInt ((uint) field_data.Count, false));
               data.Add (field_data);
            }
         }

         // Append the "framing bit".
         if (addFramingBit)
            data.Add ((byte) 1);

         return data;
      }
      
		public IEnumerator<string> GetEnumerator()
		{
			return field_list.Keys.GetEnumerator();
		}
      
      IEnumerator IEnumerable.GetEnumerator()
      {
         return field_list.Keys.GetEnumerator();
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override TagTypes TagTypes {get {return TagTypes.Xiph;}}
      
		public override bool IsEmpty {
			get {
				foreach (string [] values in field_list.Values)
					if (values.Length != 0)
						return false;
				
				return true;
			}
		}
		
		public uint FieldCount {
			get {
				uint count = 0;
				foreach (string [] values in field_list.Values)
					count += (uint) values.Length;
				
				return count;
			}
		}
      
      public string VendorId
      {
         get
         {
            return vendor_id;
         }
      }

      public override string Title
      {
         get {return GetFirstField ("TITLE");}
         set {SetField ("TITLE", value);}
      }
      
      public override string [] AlbumArtists
      {
         get {return GetField ("ALBUMARTIST");}
         set {SetField ("ALBUMARTIST", value);}
      }
      
      public override string [] Performers
      {
         get {return GetField ("ARTIST");}
         set {SetField ("ARTIST", value);}
      }
      
      public override string [] Composers
      {
         get {return GetField ("COMPOSER");}
         set {SetField ("COMPOSER", value);}
      }

      public override string Album
      {
         get {return GetFirstField ("ALBUM");}
         set {SetField ("ALBUM", value);}
      }

      public override string Comment
      {
         get
         {
            string value = GetFirstField (comment_field);
            if (value != null || comment_field == "COMMENT")
               return value;
            
            comment_field = "COMMENT";
            return GetFirstField (comment_field);
         }
         set {SetField (comment_field, value);}
      }
      
      public override string [] Genres
      {
         get {return GetField ("GENRE");}
         set {SetField ("GENRE", value);}
      }
      
      public override uint Year
      {
         get
         {
            string text = GetFirstField ("DATE");
            uint value;
            return (text != null && uint.TryParse (text.Length > 4 ? text.Substring (0, 4) : text, out value)) ? value : 0;
         }
         set {SetField ("DATE", value);}
      }
      
      public override uint Track
      {
         get
         {
            string text = GetFirstField ("TRACKNUMBER");
            string [] values;
            uint value;
            
            if (text != null && (values = text.Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetField ("TRACKTOTAL", TrackCount);
            SetField ("TRACKNUMBER", value);
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            string text;
            string [] values;
            uint value;
            
            if ((text = GetFirstField ("TRACKTOTAL")) != null && uint.TryParse (text, out value))
               return value;
            
            if ((text = GetFirstField ("TRACKNUMBER")) != null && (values = text.Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
               return value;
            
            return 0;
         }
         set {SetField ("TRACKTOTAL", value);}
      }
      
      public override uint Disc
      {
         get
         {
            string text = GetFirstField ("DISCNUMBER");
            string [] values;
            uint value;
            
            if (text != null && (values = text.Split ('/')).Length > 0 && uint.TryParse (values [0], out value))
               return value;
            
            return 0;
         }
         set
         {
            SetField ("DISCTOTAL", TrackCount);
            SetField ("DISCNUMBER", value);
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            string text;
            string [] values;
            uint value;
            
            if ((text = GetFirstField ("DISCTOTAL")) != null && uint.TryParse (text, out value))
               return value;
            
            if ((text = GetFirstField ("DISCNUMBER")) != null && (values = text.Split ('/')).Length > 1 && uint.TryParse (values [1], out value))
               return value;
            
            return 0;
         }
         set {SetField ("DISCTOTAL", value);}
      }
      
      public override string Lyrics
      {
         get {return GetFirstField ("LYRICS");}
         set {SetField ("LYRICS", value);}
      }
      
      public override string Copyright
      {
         get {return GetFirstField ("COPYRIGHT");}
         set {SetField ("COPYRIGHT", value);}
      }
      
      public override string Conductor
      {
         get {return GetFirstField ("CONDUCTOR");}
         set {SetField ("CONDUCTOR", value);}
      }
      
      public override string Grouping
      {
         get {return GetFirstField ("GROUPING");}
         set {SetField ("GROUPING", value);}
      }
      
      public override uint BeatsPerMinute
      {
         get
         {
            string text = GetFirstField ("TEMPO");
            double value;
            return (text != null && double.TryParse (text, out value)
		&& value > 0) ? (uint) Math.Round (value) : 0;
         }
         set {SetField ("TEMPO", value);}
      }
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
		protected void Parse (ByteVector data)
		{
			if (data == null)
			return;

			// The first thing in the comment data is the vendor ID
			// length, followed by a UTF8 string with the vendor ID.
			int pos = 0;
			int vendor_length = (int) data.Mid (pos, 4)
				.ToUInt (false);
			pos += 4;

			vendor_id = data.ToString (StringType.UTF8, pos,
				vendor_length);
			pos += vendor_length;

			// Next the number of fields in the comment vector.

			int comment_fields = (int) data.Mid (pos, 4)
				.ToUInt (false);
			pos += 4;

			for(int i = 0; i < comment_fields; i++) {
				// Each comment field is in the format
				// "KEY=value" in a UTF8 string and has 4 bytes
				// before the text starts that gives the length.

				int comment_length = (int) data.Mid (pos, 4)
					.ToUInt (false);
				pos += 4;

				string comment = data.ToString (StringType.UTF8,
					pos, comment_length);
				pos += comment_length;

				int comment_separator_position = comment
					.IndexOf ('=');

				if (comment_separator_position < 0)
					continue;

				string key = comment.Substring (0,
					comment_separator_position)
					.ToUpper (
						CultureInfo.InvariantCulture);
				string value = comment.Substring (
					comment_separator_position + 1);
				string [] values;
				
				if (field_list.TryGetValue (key, out values)) {
					Array.Resize <string> (ref values,
						values.Length + 1);
					values [values.Length - 1] = value;
					field_list [key] = values;
				} else {
					SetField (key, value);
				}
			}
		}
	}
}
