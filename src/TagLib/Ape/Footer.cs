/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : apefooter.cpp from TagLib
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
   public enum FooterFlags : uint
   {
      FooterAbsent  = 0x40000000,
      IsHeader      = 0x20000000,
      HeaderPresent = 0x80000000
   }
   
   public struct Footer
   {
      #region Private Properties
      private uint version;
      private uint flags;
      private uint item_count;
      private uint tag_size;
      #endregion
      
      
      
      #region Public Static Properties
      public static readonly uint Size = 32;
      public static readonly ByteVector FileIdentifier = "APETAGEX";
      #endregion
      
      
      
      #region Constructors
      public Footer (ByteVector data)
      {
         if (data.Count < Size)
            throw new CorruptFileException ("Provided data is smaller than object size.");
         
         if (!data.StartsWith (FileIdentifier))
            throw new CorruptFileException ("Provided data does not start with File Identifier");
         
         version    = data.Mid ( 8, 4).ToUInt (false);
         tag_size   = data.Mid (12, 4).ToUInt (false);
         item_count = data.Mid (16, 4).ToUInt (false);
         flags      = data.Mid (20, 4).ToUInt (false);
      }
      #endregion
      
      
      
      #region Public Properties
      public uint Version       {get {return version == 0 ? 2000 : version;}}
      public FooterFlags Flags {get {return (FooterFlags)flags;} set {flags = (uint)value;}}
      
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
         get {return TagSize + ((Flags & FooterFlags.HeaderPresent) != 0 ? Size : 0);}
      }
      #endregion
      
      
      
      #region Public Methods
      public ByteVector RenderFooter ()
      // Renders the footer.
      {
         return Render (false);
      }
      
      public ByteVector RenderHeader ()
      // Renders the header, if present, otherwise returns an empty ByteVector.
      {
         return (Flags & FooterFlags.HeaderPresent) != 0 ? Render (true) : new ByteVector ();
      }
      #endregion
      
      
      
      #region Private Methods
      private ByteVector Render (bool is_header)
      // Renders either a header or a footer.
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

         flags |= (uint)(((Flags & FooterFlags.HeaderPresent) != 0 ? 1 : 0) << 31);
         // footer is always present
         flags |= (uint)((is_header ? 1 : 0) << 29);

         v.Add (ByteVector.FromUInt (flags, false));

         // add the reserved 64bit
         v.Add (ByteVector.FromULong (0));

         return v;
      }
      #endregion
   }
}
