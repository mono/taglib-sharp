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
   public abstract class Frame
   {
      #region Private Properties
      private FrameHeader header;
      private byte group_id;
      private byte encryption_id;
      #endregion
      
      
      
      #region Constructors
      protected Frame (ByteVector data, byte version)
      {
         header = new FrameHeader (data, version);
      }
      
      protected Frame (FrameHeader header)
      {
         this.header = header;
      }
      #endregion
      
      
      
      #region Public Properties
      public ReadOnlyByteVector FrameId {get {return header.FrameId;}}
      public uint       Size    {get {return header.FrameSize;}}
      public FrameFlags Flags
      {
         get {return header.Flags;}
         set {header.Flags = value;}
      }
      
      public short GroupId
      {
         get {return (Flags & FrameFlags.GroupingIdentity) != 0 ? group_id : (short) -1;}
         set
         {
            if (value >= 0x00 && value <= 0xFF)
            {
               group_id = (byte) value;
               Flags |= FrameFlags.GroupingIdentity;
            }
            else
               Flags &= ~FrameFlags.GroupingIdentity;
         }
      }
      
      public short EncryptionId
      {
         get {return (Flags & FrameFlags.Encryption) != 0 ? encryption_id : (short) -1;}
         set
         {
            if (value >= 0x00 && value <= 0xFF)
            {
               encryption_id = (byte) value;
               Flags |= FrameFlags.Encryption;
            }
            else
               Flags &= ~FrameFlags.Encryption;
         }
      }
      #endregion
      
      
      
      #region Public Methods
      public virtual ByteVector Render (byte version)
      {
         if (version < 4)
            Flags &= ~(FrameFlags.DataLengthIndicator | FrameFlags.Unsychronisation);
         
         if (version < 3)
            Flags &= ~(FrameFlags.Compression |
                                   FrameFlags.Encryption |
                                   FrameFlags.FileAlterPreservation |
                                   FrameFlags.GroupingIdentity |
                                   FrameFlags.ReadOnly |
                                   FrameFlags.TagAlterPreservation);
         
         ByteVector field_data = RenderFields (version);
         
         // If we don't have any content, don't render anything. This will cause
         // the frame to not be rendered.
         if (field_data.Count == 0)
            return new ByteVector ();
         
         ByteVector front_data = new ByteVector ();
         
         if ((Flags & (FrameFlags.Compression | FrameFlags.DataLengthIndicator)) != 0)
            front_data.Add (ByteVector.FromUInt ((uint) field_data.Count));
         
         if ((Flags & FrameFlags.GroupingIdentity) != 0)
            front_data.Add (group_id);
         
         if ((Flags & FrameFlags.Encryption) != 0)
            front_data.Add (encryption_id);
         
         // FIXME: Implement compression.
         if ((Flags & FrameFlags.Compression) != 0)
            throw new NotImplementedException ("Compression not yet supported");
         
         // FIXME: Implement encryption.
         if ((Flags & FrameFlags.Encryption) != 0)
            throw new NotImplementedException ("Encryption not yet supported");
         
         if ((Flags & FrameFlags.Unsychronisation) != 0)
            SynchData.UnsynchByteVector (field_data);
         
         if (front_data.Count > 0)
            field_data.Insert (0, front_data);
         
         header.FrameSize = (uint) field_data.Count;
         ByteVector header_data = header.Render (version);
         header_data.Add (field_data);
         
         return header_data;
      }
      #endregion
      
      
      
      #region Public Static Methods
      
      [Obsolete("Use ByteVector.TextDelimiter.")]
      public static ByteVector TextDelimiter (StringType type)
      {
         return ByteVector.TextDelimiter (type);
      }
      
      #endregion
      
      
      
      #region Protected Methods
      protected static StringType CorrectEncoding (StringType type, byte version)
      {
         if (Tag.ForceDefaultEncoding)
            type = Tag.DefaultEncoding;
         return (version < 4 && type == StringType.UTF8) ? StringType.UTF16 : type;
      }
      
      protected void SetData (ByteVector data, int offset, byte version, bool readHeader)
      {
         if (readHeader)
            header = new FrameHeader (data, version);
         ParseFields (FieldData (data, offset, version), version);
      }
      
      protected abstract void ParseFields(ByteVector data, byte version);
      protected abstract ByteVector RenderFields (byte version);
      
      protected ByteVector FieldData (ByteVector frameData, int offset, byte version)
      {
         if (frameData == null)
            throw new ArgumentNullException ("frameData");
         
         int data_offset              = offset + (int) FrameHeader.Size (version);
         int data_length              = (int) Size;
         /*int uncompressed_data_length;*/
         
         if ((Flags & (FrameFlags.Compression | FrameFlags.DataLengthIndicator)) != 0)
         {
            /*uncompressed_data_length = (int) frame_data.Mid (data_offset, 4).ToUInt () + 4;*/
            data_offset += 4;
            data_offset -= 4;
         }
         
         if ((Flags & FrameFlags.GroupingIdentity) != 0)
         {
            group_id = frameData [data_offset++];
            data_length--;
         }
         
         if ((Flags & FrameFlags.Encryption) != 0)
         {
            encryption_id = frameData [data_offset++];
            data_length--;
         }
         
         ByteVector data = frameData.Mid (data_offset, data_length);
         
         if ((Flags & FrameFlags.Unsychronisation) != 0)
            SynchData.ResynchByteVector (data);
         
         // FIXME: Implement encryption.
         if ((Flags & FrameFlags.Encryption) != 0)
            throw new NotImplementedException ();
         
         // FIXME: Implement compression.
         if ((Flags & FrameFlags.Compression) != 0)
            throw new NotImplementedException ();
         /*
            if(d->header->compression()) {
               ByteVector data(frameDataLength);
               uLongf uLongTmp = frameDataLength;
               ::uncompress((Bytef *) data.data(),
                           (uLongf *) &uLongTmp,
                           (Bytef *) frameData.data() + frameDataOffset,
                           size());
               return data;
            }
         */
         
         return data;
      }
      #endregion
   }
}
