//
// UnknownFrame.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   unknownframe.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2002 Scott Wheeler (Original Implementation)
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
   public class UnknownFrame : Frame
   {
      #region Private Properties
      private ByteVector field_data = null;
      #endregion
      
      
      
      #region Constructors
      public UnknownFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }
      
      public UnknownFrame (ByteVector type, ByteVector data) : base (type, 4)
      {
         field_data = data;
      }
      
      public UnknownFrame (ByteVector type) : this (type, null)
      {}
      
      protected internal UnknownFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public ByteVector Data
      {
         get {return field_data;}
         set {field_data = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return null;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         field_data = data;
      }
      
      protected override ByteVector RenderFields (byte version)
      {
         return field_data != null ? field_data : new ByteVector ();
      }
      #endregion
   }
}
