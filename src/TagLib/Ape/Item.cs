/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : apeitem.cpp from TagLib
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
      #region Private Properties
      private ItemType           _type      = ItemType.Text;
      private string             _key       = null;
      private ReadOnlyByteVector _value     = null;
      private StringCollection   _text      = new StringCollection ();
      private bool               _read_only = false;
      #endregion
      
      
      
      #region Constructors
      public Item (ByteVector data, int offset)
      {
         Parse (data, offset);
      }
      
      public Item (string key, string value)
      {
         _key = key;
         _text.Add (value);
      }
      
      public Item (string key, StringCollection value)
      {
         _key = key;
         _text.Add (value);
      }
      
      public Item (string key, ByteVector value)
      {
         _key   = key;
         _type  = ItemType.Binary;
         _value = value is ReadOnlyByteVector ? value as ReadOnlyByteVector : new ReadOnlyByteVector (value);
      }
      #endregion
      
      
      
      #region Public Properties
      public string     Key   {get {return _key;}}
      public ByteVector Value {get {return (_type == ItemType.Binary) ? _value : null;}}
      public int        Size  {get {return 8 + _key.Length + 1 + _value.Count;}}
      public ItemType   Type  {get {return _type;} set {_type = value;}}
      
      public bool ReadOnly
      {
         get {return _read_only;}
         set {_read_only = value;}
      }
      
      public bool IsEmpty
      {
         get
         {
            if (_type != ItemType.Binary)
               return _text.IsEmpty;
            else
               return _value.IsEmpty;
         }
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return _text.ToString ();
      }
      
      public string [] ToStringArray ()
      {
         return (_type != ItemType.Binary) ? _text.ToArray () : new string [0];
      }
      
      public ByteVector Render ()
      {
         ByteVector data = new ByteVector ();
         uint flags = (uint) ((ReadOnly) ? 1 : 0) | ((uint) Type << 1);

         if (IsEmpty)
            return data;
      	
         if(_type != ItemType.Binary)
         {
            ByteVector value = new ByteVector ();
            for (int i = 0; i < _text.Count; i ++)
            {
               if (i != 0)
                  value.Add ((byte) 0);
               
               value.Add (ByteVector.FromString (_text [i], StringType.UTF8));
            }
            _value = new ReadOnlyByteVector (value);
         }
         

         data.Add (ByteVector.FromUInt ((uint) _value.Count, false));
         data.Add (ByteVector.FromUInt (flags, false));
         data.Add (ByteVector.FromString (_key, StringType.UTF8));
         data.Add ((byte) 0);
         data.Add (_value);

         return data;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected void Parse (ByteVector data, int offset)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         // 11 bytes is the minimum size for an APE item
         if(data.Count < offset + 11)
            throw new CorruptFileException ("Not enough data for APE Item");
         
         if (offset > int.MaxValue - 11)
            throw new ArgumentOutOfRangeException ("offset", "offset + 11 must be less that Int32.MaxValue");
         
         uint value_length  = data.Mid (offset, 4).ToUInt (false);
         uint flags         = data.Mid (offset + 4, 4).ToUInt (false);
         
         int pos = data.Find (new ByteVector (1), offset + 8);
         
         _key   = data.Mid (offset + 8, pos - offset - 8).ToString (StringType.UTF8);
         _value = new ReadOnlyByteVector (data.Mid (pos + 1, (int) value_length));

         ReadOnly = (flags & 1) == 1;
         Type = (ItemType) ((flags >> 1) & 3);

         if(Type != ItemType.Binary)
         {
            _text.Clear ();
            _text = new StringCollection (ByteVectorCollection.Split(_value, (byte) 0), StringType.UTF8);
         }
      }
      #endregion
   }
}
