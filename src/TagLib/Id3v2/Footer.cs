//
// Footer.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2header.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2002,2003 Scott Wheeler (Original Implementation)
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

namespace TagLib.Id3v2
{
   public struct Footer
   {
      #region Private Properties
      private byte        major_version;
      private byte        revision_number;
      private HeaderFlags flags;
      private uint        tag_size;
      #endregion
      
      
      
      #region Public Static Properties
      public const uint Size = 10;
      public static readonly ReadOnlyByteVector FileIdentifier = "3DI";
      #endregion
      
      
      
      #region Constructors
      public Footer (ByteVector data)
      {
         if (data.Count < Size)
            throw new CorruptFileException ("Provided data is smaller than object size.");
         
         if (!data.StartsWith (FileIdentifier))
            throw new CorruptFileException ("Provided data does not start with File Identifier");
         
         major_version   = data [3];
         revision_number = data [4];
         flags           = (HeaderFlags) data [5];
         
         
         if (major_version == 2 && (flags & (HeaderFlags) 127) != 0)
            throw new CorruptFileException ("Invalid flags set on version 2 tag.");
         
         if (major_version == 3 && (flags & (HeaderFlags) 15) != 0)
            throw new CorruptFileException ("Invalid flags set on version 3 tag.");
         
         if (major_version == 4 && (flags & (HeaderFlags) 7) != 0)
            throw new CorruptFileException ("Invalid flags set on version 4 tag.");
         
         
         ByteVector size_data = data.Mid (6, 4);
         
         foreach (byte b in size_data)
            if (b >= 128)
               throw new CorruptFileException ("One of the bytes in the header was greater than the allowed 128.");

         tag_size = SynchData.ToUInt (size_data);
      }
      
      public Footer (Header header)
      {
         major_version   = header.MajorVersion;
         revision_number = header.RevisionNumber;
         flags           = header.Flags | HeaderFlags.FooterPresent;
         tag_size        = header.TagSize;
      }
      #endregion
      
      
      
      #region Public Properties
      public byte MajorVersion
      {
         get {return major_version == 0 ? Tag.DefaultVersion : major_version;}
         set
         {
            if (value != 4)
               throw new ArgumentException ("Version unsupported");
            
            major_version = value;
         }
      }
      
      public byte RevisionNumber
      {
         get {return revision_number;}
         set {revision_number = value;}
      }
      
      public HeaderFlags Flags
      {
         get {return flags;}
         set
         {
            if (0 != (value & (HeaderFlags.ExtendedHeader | HeaderFlags.ExperimentalIndicator)) && MajorVersion < 3)
               throw new ArgumentException ("Feature only supported in version 2.3+");
            
            if (0 != (value & (HeaderFlags.FooterPresent)) && MajorVersion < 3)
               throw new ArgumentException ("Feature only supported in version 2.4+");
            
            flags = value;
         }
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
      #endregion
      
      
      
      #region Public Methods
      public ByteVector Render ()
      {
         ByteVector v = new ByteVector ();
         v.Add (FileIdentifier);
         v.Add (MajorVersion);
         v.Add (RevisionNumber);
         v.Add ((byte)flags);
         v.Add (SynchData.FromUInt (TagSize));
         return v;
      }
      #endregion
   }
}
