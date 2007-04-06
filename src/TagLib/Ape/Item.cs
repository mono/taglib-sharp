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
   public enum ItemType
   {
      Text = 0,   // Item contains text information coded in UTF-8
      Binary = 1, // Item contains binary information
      Locator = 2 // Item is a locator of external stored information
   }
   
   public class Item
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ItemType type;
      private string key;
      private ByteVector value;
      private StringList text;
      private bool read_only;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Item ()
      {
         type = ItemType.Text;
         key = null;
         value = null;
         text = new StringList ();
         read_only = false;
      }
      
      public Item (string key, string value) : this ()
      {
         this.key = key;
         this.text.Add (value);
      }
      
      public Item (string key, StringList value) : this ()
      {
         this.key = key;
         text.Add (value);
      }
      
      public Item (string key, ByteVector value) : this ()
      {
         this.key = key;
         this.type = ItemType.Binary;
         this.value = value;
      }
      
      public Item (Item item) : this ()
      {
         type = item.Type;
         key = item.Key;
         value = item.Value;
         text = new StringList (item.ToStringArray ());
         read_only = false;
      }
      public override string ToString ()
      {
         return text.ToString ();
      }
      
      public string [] ToStringArray ()
      {
         return text.ToArray ();
      }
      
      public void Parse (ByteVector data)
      {
         // 11 bytes is the minimum size for an APE item

         if(data.Count < 11)
            throw new CorruptFileException ("Not enough data for APE Item");

         uint value_length  = data.Mid (0, 4).ToUInt (false);
         uint flags        = data.Mid (4, 4).ToUInt (false);
         
         int pos = data.Find (new ByteVector (1), 8);
         
         key   = data.Mid (8, pos - 8).ToString (StringType.UTF8);
         value = data.Mid (pos + 1, (int) value_length);

         ReadOnly = (flags & 1) == 1;
         Type = (ItemType) ((flags >> 1) & 3);

         if(Type != ItemType.Binary)
         {
            text.Clear ();
            text = new StringList (ByteVectorList.Split(value, (byte) 0), StringType.UTF8);
         }
      }

      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         uint flags = (uint) ((ReadOnly) ? 1 : 0) | ((uint) Type << 1);

         if (IsEmpty)
            return data;

         if(type != ItemType.Binary)
         {
            value = new ByteVector ();
            for (int i = 0; i < text.Count; i ++)
            {
               if (i != 0)
                  value.Add ((byte) 0);
               
               value.Add (ByteVector.FromString (text [i], StringType.UTF8));
            }
         }

         data.Add (ByteVector.FromUInt ((uint) value.Count, false));
         data.Add (ByteVector.FromUInt (flags, false));
         data.Add (ByteVector.FromString (key, StringType.UTF8));
         data.Add ((byte) 0);
         data.Add (value);

         return data;
      }

      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public string Key {get {return key;}}
      public ByteVector Value {get {return value;}}
      public int Size {get {return 8 + key.Length + 1 + value.Count;}}
      
      public ItemType Type
      {
         get {return type;}
         set {type = value;}
      }
      
      public bool ReadOnly
      {
         get {return read_only;}
         set {read_only = value;}
      }
      
      public bool IsEmpty
      {
         get
         {
            if (type != ItemType.Binary)
               return text.IsEmpty;
            else
               return value.IsEmpty;
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
   }
}
