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
   public class Footer
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint version;
      private bool footer_present;
      private bool header_present;
      private bool is_header;
      private uint item_count;
      private uint tag_size;
      private static uint size = 32;
      
      //////////////////////////////////////////////////////////////////////////
      // static members
      //////////////////////////////////////////////////////////////////////////
      public static uint Size {get {return size;}}
      
      public static readonly ByteVector FileIdentifier = "APETAGEX";
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Footer ()
      {
         version = 0;
         footer_present = true;
         header_present = false;
         is_header = false;
         item_count = 0;
         tag_size = 0;
      }
      
      public Footer(ByteVector data) : this ()
      {
         Parse (data);
      }
      
      public void SetData (ByteVector data)
      {
         Parse (data);
      }
      
      public ByteVector RenderFooter ()
      {
         return Render (false);
      }
      
      public ByteVector RenderHeader ()
      {
         return HeaderPresent ? Render (true) : new ByteVector ();
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public uint Version       {get {return version;}}
      public bool FooterPresent {get {return footer_present;}}
      public bool IsHeader      {get {return is_header;}}

      public bool HeaderPresent
      {
         get {return header_present;}
         set {header_present = value;}
      }
      
      public uint ItemCount
      {
         get {return item_count;}
         set {item_count = value;}
      }
      
      public uint TagSize
      {
         get {return tag_size;}
         set {tag_size = value;}
      }
      
      public uint CompleteTagSize
      {
         get {return TagSize + (HeaderPresent ? Size : 0);}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected void Parse (ByteVector data)
      {
         if (data.Count < Size)
            throw new CorruptFileException ("Provided data is smaller than object size.");

         // The first eight bytes, data[0..7], are the File Identifier, "APETAGEX".
         if (!data.StartsWith (FileIdentifier))
            throw new CorruptFileException ("Provided data does not start with File Identifier");

         // Read the version number
         version = data.Mid (8, 4).ToUInt (false);

         // Read the tag size
         tag_size = data.Mid (12, 4).ToUInt (false);

         // Read the item count
         item_count = data.Mid (16, 4).ToUInt (false);

         // Read the flags

         uint flags = data.Mid (20, 4).ToUInt (false);
         header_present = ((flags >> 31) & 1) == 1;
         footer_present = ((flags >> 30) & 1) != 1;
         is_header = ((flags >> 29) & 1) == 1;
      }

      protected ByteVector Render (bool is_header)
      {
         ByteVector v = new ByteVector ();

         // add the file identifier -- "APETAGEX"
         v.Add (FileIdentifier);

         // add the version number -- we always render a 2.000 tag regardless of what
         // the tag originally was.
         v.Add (ByteVector.FromUInt (2000, false));

         // add the tag size
         v.Add (ByteVector.FromUInt (tag_size, false));

         // add the item count
         v.Add (ByteVector.FromUInt (item_count, false));

         // render and add the flags
         uint flags = 0;

         flags |= (uint)((HeaderPresent ? 1 : 0) << 31);
         // footer is always present
         flags |= (uint)((is_header ? 1 : 0) << 29);

         v.Add (ByteVector.FromUInt (flags, false));

         // add the reserved 64bit
         v.Add (ByteVector.FromLong (0));

         return v;
      }
   }
}
