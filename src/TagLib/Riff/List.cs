/***************************************************************************
    copyright            : (C) 2007 by Brian Nickel
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
namespace TagLib.Riff
{
   public class List : Dictionary <ByteVector,ByteVector>
   {
      public List () : base ()
      {}
      
      public List (ByteVector data) : this ()
      {
         Parse (data);
      }
      
      public List (TagLib.File file, long position, int length) : this ()
      {
         file.Seek (position);
         Parse (file.ReadBlock (length));
      }
      
      private void Parse (ByteVector data)
      {
         int offset = 0;
         while (offset + 8 < data.Count)
         {
            ByteVector id = data.Mid (offset, 4);
            int length = (int) data.Mid (offset + 4, 4).ToUInt (false);
            
            if (length % 2 == 1)
               length ++;
            
            if (!ContainsKey (id))
               Add (id, data.Mid (offset + 8, length));
            
            offset += 8 + length;
         }
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         
         foreach (ByteVector id in Keys)
         {
            ByteVector value = this [id];
            
            if (value.Count == 0)
               continue;
            
            data.Add (id);
            data.Add (ByteVector.FromUInt ((uint) value.Count, false));
            data.Add (value);
         }
         
         return data;
      }
      
      public ByteVector RenderEnclosed (ByteVector id)
      {
         ByteVector data = Render ();
         
         if (data.Count <= 8)
            return new ByteVector ();
         
         ByteVector header = new ByteVector ("LIST");
         header.Add (ByteVector.FromUInt ((uint) (data.Count + 4), false));
         header.Add (id);
         data.Insert (0, header);
         return data;
      }
      
      public ByteVector GetValue (ByteVector id)
      {
         ByteVector value;
         return TryGetValue (id, out value) ? value : null;
      }
      
      public string GetValueAsString (ByteVector id)
      {
         ByteVector data = GetValue (id);
         if (data == null)
            return null;
         
         int str_length = data.Count;
         while (data [str_length - 1] == 0)
            str_length --;
         return data.Mid (0, str_length).ToString (StringType.UTF8);
      }
      
      public uint GetValueAsUInt (ByteVector id)
      {
         uint value;
         string str = GetValueAsString (id);
         return (str != null && uint.TryParse (str, out value)) ? value : 0;
      }
      
      public StringList GetValueAsStringList (ByteVector id)
      {
         string str = GetValueAsString (id);
         return (str != null) ? StringList.Split (str, ";") : new StringList ();
      }
      
      public void SetValue (ByteVector id, ByteVector value)
      {
         if (value == null || value.Count == 0)
            RemoveValue (id);
         else if (ContainsKey (id))
            this [id] = value;
         else
            Add (id, value);
      }
      
      public void SetValue (ByteVector id, string value)
      {
         if (string.IsNullOrEmpty (value))
         {
            RemoveValue (id);
            return;
         }
         
         ByteVector data = ByteVector.FromString (value, StringType.UTF8);
         
         // Nil terminate.
         data.Add (0);
         
         // Keep the number of bytes even.
         if (data.Count % 2 == 1)
            data.Add (0);
         
         SetValue (id, data);
      }
      
      public void SetValue (ByteVector id, uint value)
      {
         if (value == 0)
            RemoveValue (id);
         else
            SetValue (id, value.ToString ());
      }
      
      public void SetValue (ByteVector id, StringList value)
      {
         if (value == null || value.Count == 0)
            RemoveValue (id);
         else
            SetValue (id, string.Join (";", value.ToArray ()));
      }
      
      public void SetValue (ByteVector id, string [] value)
      {
         if (value == null || value.Length == 0)
            RemoveValue (id);
         else
            SetValue (id, string.Join (";", value));
      }
      
      public void RemoveValue (ByteVector id)
      {
         if (ContainsKey (id))
            Remove (id);
      }
   }
}