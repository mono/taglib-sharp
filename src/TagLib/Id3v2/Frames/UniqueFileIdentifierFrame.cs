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
      #region Private Properties
      private string owner = null;
      private ByteVector identifier = null;
      #endregion
      
      
      
      #region Constructors
      public UniqueFileIdentifierFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }

      public UniqueFileIdentifierFrame (string owner, ByteVector id) : base (FrameType.UFID, 4)
      {
         this.owner = owner;
         identifier = id;
      }
      
      protected internal UniqueFileIdentifierFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public string Owner
      {
         get {return owner;}
      }

      public ByteVector Identifier
      {
         get {return identifier;}
         set {identifier = value;}
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static UniqueFileIdentifierFrame Get (Tag tag, string owner, bool create)
      {
         foreach (Frame f in tag.GetFrames (FrameType.UFID))
            if (f is UniqueFileIdentifierFrame && (f as UniqueFileIdentifierFrame).Owner == owner)
               return f as UniqueFileIdentifierFrame;
         
         if (!create) return null;
         
         UniqueFileIdentifierFrame frame = new UniqueFileIdentifierFrame (owner, null);
         tag.AddFrame (frame);
         return frame;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         ByteVectorCollection fields = ByteVectorCollection.Split(data, (byte) 0);

         if (fields.Count != 2)
            return;

         owner = fields [0].ToString (StringType.Latin1);
         identifier = fields [1];
      }
      
      protected override ByteVector RenderFields (byte version)
      {
         ByteVector data = new ByteVector ();
         
         data.Add (ByteVector.FromString (owner, StringType.Latin1));
         data.Add (ByteVector.TextDelimiter (StringType.Latin1));
         data.Add (identifier);
         
         return data;
      }
      #endregion
   }
}
