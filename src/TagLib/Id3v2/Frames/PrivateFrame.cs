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
      #region Private Properties
      private string     owner = null;
      private ByteVector data  = null;
      #endregion
      
      
      
      #region Constructors
      public PrivateFrame (string owner, ByteVector data) : base (FrameType.PRIV, 4)
      {
         this.owner = owner;
         this.data  = data;
      }
      
      public PrivateFrame (string owner) : this (owner, null)
      {
      }

      public PrivateFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }

      protected internal PrivateFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public string Owner
      {
         get {return owner;}
      }
      
      public ByteVector PrivateData
      {
         get {return data;}
         set {data = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return owner;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static PrivateFrame Get (Tag tag, string owner, bool create)
      {
         foreach (Frame f in tag.GetFrames (FrameType.PRIV))
            if (f is PrivateFrame && (f as PrivateFrame).Owner == owner)
               return f as PrivateFrame;
         
         if (!create) return null;
         
         PrivateFrame frame = new PrivateFrame (owner);
         tag.AddFrame (frame);
         return frame;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         if (data.Count < 1)
            throw new CorruptFileException ("A private frame must contain at least 1 byte.");
         
         ByteVectorCollection l = ByteVectorCollection.Split (data, ByteVector.TextDelimiter (StringType.Latin1), 1, 2);
         
         if (l.Count == 2)
         {
            owner = l [0].ToString (StringType.Latin1);
            data  = l [1];
         }
      }

      protected override ByteVector RenderFields (byte version)
      {
         ByteVector v = new ByteVector ();
         
         if (version > 2)
         {
            v.Add (ByteVector.FromString (owner, StringType.Latin1));
            v.Add (ByteVector.TextDelimiter (StringType.Latin1));
            v.Add (data);
         }
         
         return v;
      }
      #endregion
   }
}
