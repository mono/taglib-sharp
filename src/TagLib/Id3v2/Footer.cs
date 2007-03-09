/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2header.cpp from TagLib
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

namespace TagLib.Id3v2
{
   public class Footer
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint major_version;
      private uint revision_number;

      private bool unsynchronisation;
      private bool extended_header;
      private bool experimental_indicator;

      private uint tag_size;

      private static uint size = 10;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public static fields
      //////////////////////////////////////////////////////////////////////////
      
      public static uint Size {get {return size;}}
      
      public static readonly ByteVector FileIdentifier = "3DI";
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Footer ()
      {
         major_version          = 0;
         revision_number        = 0;
         unsynchronisation      = false;
         extended_header        = false;
         experimental_indicator = false;
         tag_size               = 0;
      }
      
      public Footer (ByteVector data) : this ()
      {
         Parse (data);
      }
      
      public Footer (Header header) : this ()
      {
         major_version          = header.MajorVersion;
         revision_number        = header.RevisionNumber;
         unsynchronisation      = header.Unsynchronisation;
         extended_header        = header.ExtendedHeader;
         experimental_indicator = header.ExperimentalIndicator;
         tag_size               = header.TagSize;
      }
      
      public void SetData (ByteVector data)
      {
         Parse (data);
      }
      
      public ByteVector Render ()
      {
         ByteVector v = new ByteVector ();

         // add the file identifier -- "3DI"
         v.Add (FileIdentifier);

         // add the version number -- we always render a 2.4.0 tag regardless of what
         // the tag originally was.
         v.Add ((byte)4);
         v.Add ((byte)0);
         
         // render and add the flags
         byte flags = 0;
         if (Unsynchronisation)     flags |= 128;
         if (ExtendedHeader)        flags |= 64;
         if (ExperimentalIndicator) flags |= 32;
                                    flags |= 16;
         v.Add (flags);

         // add the size
         v.Add (SynchData.FromUInt (TagSize));

         return v;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public uint MajorVersion
      {
         get {return major_version;}
         set {major_version = value;}
      }
      
      public uint RevisionNumber
      {
         get {return revision_number;}
         set {revision_number = value;}
      }
      
      public bool Unsynchronisation
      {
         get {return unsynchronisation;}
         set {unsynchronisation = value;}
      }
      
      public bool ExtendedHeader
      {
         get {return extended_header;}
         set {extended_header = value;}
      }
      
      public bool ExperimentalIndicator
      {
         get {return experimental_indicator;}
         set {experimental_indicator = value;}
      }
      
      public bool FooterPresent
      {
         get {return true;}
      }
      
      public uint TagSize
      {
         get {return tag_size;}
         set {tag_size = value;}
      }
      
      public uint CompleteTagSize
      {
         get
         {
            return TagSize + Header.Size + Size;
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////

      protected void Parse (ByteVector data)
      {
         if(data.Count < Size)
            return;
         
         // do some sanity checking -- even in ID3v2.3.0 and less the tag size is a
         // synch-safe integer, so all bytes must be less than 128.  If this is not
         // true then this is an invalid tag.
         
         // note that we're doing things a little out of order here -- the size is
         // later in the bytestream than the version
         
         if (!data.StartsWith (FileIdentifier))
            throw new CorruptFileException ("The footer block does not start with the correct File Identifier.");
         
         ByteVector size_data = data.Mid (6, 4);

         if (size_data.Count != 4)
            throw new CorruptFileException ("The tag size data is not long enough.");
         
         foreach (byte b in size_data)
            if (b >= 128)
               throw new CorruptFileException ("One of the bytes in the footer was greater than the allowed 128.");

         // The first three bytes, data[0..2], are the File Identifier, "ID3". (structure 3.1 "file identifier")

         // Read the version number from the fourth and fifth bytes.
         major_version = data [3];   // (structure 3.1 "major version")
         revision_number = data [4]; // (structure 3.1 "revision number")

         // Read the flags, the first four bits of the sixth byte.
         byte flags = data [5];

         unsynchronisation      = ((flags >> 7) & 1) == 1; // (structure 3.1.a)
         extended_header        = ((flags >> 6) & 1) == 1; // (structure 3.1.b)
         experimental_indicator = ((flags >> 5) & 1) == 1; // (structure 3.1.c)

         // Get the size from the remaining four bytes (read above)

         tag_size = SynchData.ToUInt (size_data); // (structure 3.1 "size")
      }
   }
}
