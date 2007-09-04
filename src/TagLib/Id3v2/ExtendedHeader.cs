//
// ExtendedHeader.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2extendedheader.cpp from TagLib
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

using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class ExtendedHeader
   {
      private uint size;
      
      public ExtendedHeader ()
      {
      }
      
      public ExtendedHeader (ByteVector data, byte version)
      {
         Parse (data, version);
      }
      
      public uint Size {get {return size;}}
      
      protected void Parse (ByteVector data, byte version)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         size = SynchData.ToUInt (data.Mid (0, 4));
      }
   }
}
