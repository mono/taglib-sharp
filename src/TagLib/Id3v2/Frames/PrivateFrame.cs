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
   public class PrivateFrame : Frame
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      string owner;
      ByteVector data;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public PrivateFrame (string owner, ByteVector data) : base ("PRIV", 4)
      {
         this.owner = owner;
         this.data = data;
      }
      
      public PrivateFrame (string owner) : this (owner, null)
      {
      }

      public PrivateFrame (ByteVector data, uint version) : base (data, version)
      {
         this.owner = null;
         this.data = null;
         SetData (data, 0, version);
      }

      public override string ToString ()
      {
         return owner;
      }
      
      public static PrivateFrame Find (Tag tag, string owner)
      {
         foreach (PrivateFrame f in tag.GetFrames ("PRIV"))
            if (f != null && f.Owner == owner)
               return f;
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public string Owner
      {
         get {return owner;}
      }
      
      public ByteVector PrivateData
      {
         get {return data;}
         set {data = value;}
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      protected override void ParseFields (ByteVector data, uint version)
      {
         if (data.Count < 1)
            throw new CorruptFileException ("A private frame must contain at least 1 byte.");
         
         ByteVectorList l = ByteVectorList.Split (data, TextDelimiter (StringType.Latin1), 1, 2);
         
         if (l.Count == 2)
         {
            owner = l [0].ToString (StringType.Latin1);
            data  = l [1];
         }
      }

      protected override ByteVector RenderFields (uint version)
      {
         ByteVector v = new ByteVector ();
         
         if (version > 2)
         {
            v.Add (ByteVector.FromString (owner, StringType.Latin1));
            v.Add (TextDelimiter (StringType.Latin1));
            v.Add (data);
         }
         
         return v;
      }

      protected internal PrivateFrame (ByteVector data, int offset, FrameHeader h, uint version) : base (h)
      {
         this.owner = null;
         this.data = null;
         ParseFields (FieldData (data, offset, version), version);
      }
   }
}
