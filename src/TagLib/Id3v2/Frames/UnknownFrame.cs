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

namespace TagLib.Id3v2
{
   public class UnknownFrame : Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ByteVector field_data;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public UnknownFrame (ByteVector data, uint version) : base (data, version)
      {
         field_data = null;
         SetData (data, 0, version);
      }
      
      public UnknownFrame (ByteVector type, ByteVector data) : base (type, 4)
      {
         field_data = data;
      }
      
      public UnknownFrame (ByteVector type) : this (type, null)
      {}
      
      public override string ToString ()
      {
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public ByteVector Data
      {
         get {return field_data;}
         set {field_data = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data, uint version)
      {
         field_data = data;
      }
      
      protected override ByteVector RenderFields (uint version)
      {
         return field_data != null ? field_data : new ByteVector ();
      }
      
      protected internal UnknownFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         field_data = null;
         ParseFields (FieldData (data, offset, version), version);
      }
   }
}
