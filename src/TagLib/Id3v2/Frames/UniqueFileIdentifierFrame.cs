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
   public class UniqueFileIdentifierFrame : Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private string owner;
      private ByteVector identifier;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public UniqueFileIdentifierFrame (ByteVector data, uint version) : base (data, version)
      {
         owner = null;
         identifier = null;
         
         SetData (data, 0, version);
      }

      public UniqueFileIdentifierFrame (string owner, ByteVector id) : base ("UFID", 4)
      {
         this.owner = owner;
         identifier = id;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public string Owner
      {
         get {return owner;}
         set {owner = value;}
      }

      public ByteVector Identifier
      {
         get {return identifier;}
         set {identifier = value;}
      }


      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data, uint version)
      {
         ByteVectorList fields = ByteVectorList.Split(data, (byte) 0);

         if (fields.Count != 2)
            return;

         owner = fields [0].ToString (StringType.Latin1);
         identifier = fields [1];
      }
      
      protected override ByteVector RenderFields (uint version)
      {
         ByteVector data = new ByteVector ();
         
         data.Add (ByteVector.FromString (owner, StringType.Latin1));
         data.Add (TextDelimiter (StringType.Latin1));
         data.Add (identifier);
         
         return data;
      }
      
      protected internal UniqueFileIdentifierFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         owner = null;
         identifier = null;
         ParseFields (FieldData (data, offset, version), version);
      }
   }
}
