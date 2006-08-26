/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : tag.cpp from TagLib
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

namespace TagLib.Ogg
{
   public class XiphComment : TagLib.Tag
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Hashtable field_list;
      private string vendor_id;
      private string comment_field;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public XiphComment () : base ()
      {
         field_list = new Hashtable ();
         vendor_id = null;
         comment_field = null;
      }
      
      public XiphComment (ByteVector data) : this ()
      {
         Parse (data);
      }
      
      public StringList GetField (string key)
      {
         return (field_list.ContainsKey (key.ToUpper ())) ? (StringList) field_list [key.ToUpper ()] : null;
      }
      
      public void AddField (string key, string value, bool replace)
      {
         if (replace)
            RemoveField (key.ToUpper ());
         
         if (value != null && value.Trim () != "")
         {
            if (!field_list.ContainsKey (key.ToUpper ()))
               field_list.Add (key.ToUpper (), new StringList ());
         
            ((StringList) field_list [key.ToUpper ()]).Add (value);
         }
      }
      
      public void AddField (string key, string value)
      {
         AddField (key, value, true);
      }
      
      public void AddFields (string key, string [] values, bool replace)
      {
         if (replace)
            RemoveField (key.ToUpper ());
         
         foreach (string s in values)
            AddField (key, s, false);
      }
      
      public void AddFields (string key, string [] values)
      {
         AddFields (key, values, true);
      }
      
      public void RemoveField (string key, string value)
      {
         if (!field_list.ContainsKey (key.ToUpper ()))
            return;
         
         StringList l = (StringList) field_list [key.ToUpper ()];
         
         if (value == null)
            l.Clear ();
         else
         {
            int index;         
            while ((index = l.IndexOf (value)) >=0)
               l.RemoveAt (index);
         }
      }
      
      public void RemoveField (string key)
      {
         RemoveField (key, null);
      }
      
      public ByteVector Render (bool add_framing_bit)
      {
         ByteVector data = new ByteVector ();

         // Add the vendor ID length and the vendor ID.  It's important to use the
         // lenght of the data(String::UTF8) rather than the lenght of the the string
         // since this is UTF8 text and there may be more characters in the data than
         // in the UTF16 string.

         ByteVector vendor_data = ByteVector.FromString (vendor_id, StringType.UTF8);

         data.Add (ByteVector.FromUInt ((uint) vendor_data.Count, false));
         data.Add (vendor_data);

         // Add the number of fields.

         data.Add (ByteVector.FromUInt (FieldCount, false));

         // Iterate over the the field lists.  Our iterator returns a
         // std::pair<String, StringList> where the first String is the field name and
         // the StringList is the values associated with that field.

         foreach (DictionaryEntry de in field_list)
         {
            // And now iterate over the values of the current list.

            string field_name = (string) de.Key;
            StringList values = (StringList) de.Value;

            foreach (string value in values)
            {
               ByteVector field_data = ByteVector.FromString (field_name, StringType.UTF8);
               field_data.Add ((byte) '=');
               field_data.Add (ByteVector.FromString (value, StringType.UTF8));

               data.Add (ByteVector.FromUInt ((uint) field_data.Count, false));
               data.Add (field_data);
            }
         }

         // Append the "framing bit".
         if (add_framing_bit)
            data.Add ((byte) 1);

         return data;
      }
      
      public ByteVector Render ()
      {
         return Render (true);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override bool IsEmpty
      {
         get
         {
            foreach (StringList l in field_list)
               if (!l.IsEmpty)
                  return false;
            
            return true;
         }
      }
      
      public uint FieldCount
      {
         get
         {
            uint count = 0;
            foreach (StringList l in field_list.Values)
               count += (uint) l.Count;
            
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
         get
         {
            StringList l = GetField ("TITLE");
            return (l != null && l.Count != 0) ? l [0] : null;
         }
         set
         {
            AddField ("TITLE", value);
         }
      }
      
      public override string [] Artists
      {
         get
         {
            StringList l = GetField ("ARTIST");
            return (l != null && l.Count != 0) ? l.ToArray () : new string [] {};
         }
         set
         {
            AddFields ("ARTIST", value);
         }
      }
      
      public override string [] Performers
      {
         get
         {
            StringList l = GetField ("PERFORMER");
            return (l != null && l.Count != 0) ? l.ToArray () : new string [] {};
         }
         set
         {
            AddFields ("PERFORMER", value);
         }
      }
      
      public override string [] Composers
      {
         get
         {
            StringList l = GetField ("COMPOSER");
            return (l != null && l.Count != 0) ? l.ToArray () : new string [] {};
         }
         set
         {
            AddFields ("COMPOSER", value);
         }
      }

      public override string Album
      {
         get
         {
            StringList l = GetField ("ALBUM");
            return (l != null && l.Count != 0) ? l [0] : null;
         }
         set
         {
            AddField ("ALBUM", value);
         }
      }

      public override string Comment
      {
         get
         {
            StringList l = GetField ("DESCRIPTION");
            comment_field = "DESCRIPTION";
            
            if (l == null || l.Count == 0)
            {
               l = GetField ("COMMENT");
               comment_field = "COMMENT";
            }
            
            return (l != null && l.Count != 0) ? l [0] : null;
         }
         set
         {
            AddField (comment_field == null ? "DESCRIPTION" : comment_field, value);
         }
      }
      
      public override string [] Genres
      {
         get
         {
            StringList l = GetField ("GENRE");
            return (l != null && l.Count != 0) ? l.ToArray () : new string [] {};
         }
         set
         {
            AddFields ("GENRE", value);
         }
      }
      
      public override uint Year
      {
         get
         {
            try
            {
               StringList l = GetField ("DATE");
               return (l != null && l.Count != 0) ? UInt32.Parse (l [0].Substring (0, 4)) : 0;
            }
            catch {return 0;}
         }
         set
         {
            AddField ("DATE", value.ToString ());
         }
      }
      
      public override uint Track
      {
         get
         {
            try
            {
               StringList l = GetField ("TRACKNUMBER");
               return (l != null && l.Count != 0) ? UInt32.Parse (l [0]) : 0;
            }
            catch {return 0;}
         }
         set
         {
            AddField ("TRACKNUMBER", value.ToString ());
         }
      }
      
      public override uint TrackCount
      {
         get
         {
            try
            {
               StringList l = GetField ("TRACKTOTAL");
               return (l != null && l.Count != 0) ? UInt32.Parse (l [0]) : 0;
            }
            catch {return 0;}
         }
         set
         {
            AddField ("TRACKTOTAL", value.ToString ());
         }
      }
      
      public override uint Disc
      {
         get
         {
            try
            {
               StringList l = GetField ("DISCNUMBER");
               return (l != null && l.Count != 0) ? UInt32.Parse (l [0]) : 0;
            }
            catch {return 0;}
         }
         set
         {
            AddField ("DISCNUMBER", value.ToString ());
         }
      }
      
      public override uint DiscCount
      {
         get
         {
            try
            {
               StringList l = GetField ("DISCTOTAL");
               return (l != null && l.Count != 0) ? UInt32.Parse (l [0]) : 0;
            }
            catch {return 0;}
         }
         set
         {
            AddField ("DISCTOTAL", value.ToString ());
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected void Parse (ByteVector data)
      {
         // The first thing in the comment data is the vendor ID length, followed by a
         // UTF8 string with the vendor ID.

         int pos = 0;

         int vendor_length = (int) data.Mid (0, 4).ToUInt (false);
         pos += 4;

         vendor_id = data.Mid (pos, vendor_length).ToString (StringType.UTF8);
         pos += vendor_length;

         // Next the number of fields in the comment vector.

         int comment_fields = (int) data.Mid (pos, 4).ToUInt (false);
         pos += 4;

         for(int i = 0; i < comment_fields; i++)
         {
            // Each comment field is in the format "KEY=value" in a UTF8 string and has
            // 4 bytes before the text starts that gives the length.

            int comment_length = (int) data.Mid (pos, 4).ToUInt (false);
            pos += 4;

            string comment = data.Mid (pos, comment_length).ToString (StringType.UTF8);
            pos += comment_length;

            int comment_separator_position = comment.IndexOf ('=');

            string key = comment.Substring (0, comment_separator_position);
            string value = comment.Substring (comment_separator_position + 1);

            AddField (key, value, false);
         }
      }
   }
}
