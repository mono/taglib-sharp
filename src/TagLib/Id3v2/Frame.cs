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
   public class Frame
   {
      #region Private Properties
      private FrameHeader header;
      private byte group_id;
      private byte encryption_id;
      #endregion
      
      
      
      #region Constructors
      protected Frame (ByteVector data, uint version)
      {
         header = new FrameHeader (data, version);
      }
      
      protected Frame (FrameHeader header)
      {
         this.header = header;
      }
      #endregion
      
      
      
      #region Public Properties
      public ByteVector FrameId {get {return (header != null) ? header.FrameId : null;}}
      
      public uint       Size    {get {return (header != null) ? header.FrameSize : 0;}}
      
      public short GroupId
      {
         get {return header.GroupingIdentity ? group_id : (short) -1;}
         set
         {
            if (value >= 0x00 && value <= 0xFF)
            {
               group_id = (byte) value;
               header.GroupingIdentity = true;
            }
            else
               header.GroupingIdentity = false;
         }
      }
      
      public short EncryptionId
      {
         get {return header.Encryption ? encryption_id : (short) -1;}
         set
         {
            if (value >= 0x00 && value <= 0xFF)
            {
               encryption_id = (byte) value;
               header.Encryption = true;
            }
            else
               header.Encryption = false;
         }
      }
      #endregion
      
      
      
      #region Public Methods
      public ByteVector Render (uint version)
      {
         if (version < 4)
         {
            header.DataLengthIndicator = false;
            header.Unsychronisation = false;
         }
         
         if (version < 3)
         {
            header.Compression = false;
            header.Encryption = false;
            header.FileAlterPreservation = false;
            header.GroupingIdentity = false;
            header.ReadOnly = false;
            header.TagAlterPreservation = false;
         }
         
         ByteVector field_data = RenderFields (version);
         
         // If we don't have any content, don't render anything. This will cause
         // the frame to not be rendered.
         if (field_data.Count == 0)
            return new ByteVector ();
         
         ByteVector front_data = new ByteVector ();
         
         if (header.Compression || header.DataLengthIndicator)
            front_data.Add (ByteVector.FromUInt ((uint) field_data.Count));
         
         if (header.GroupingIdentity)
            front_data.Add (group_id);
         
         if (header.Encryption)
            front_data.Add (encryption_id);
         
         // FIXME: Implement compression.
         if (header.Compression)
            throw new UnsupportedFormatException ("Compression not yet supported");
         
         // FIXME: Implement encryption.
         if (header.Encryption)
            throw new UnsupportedFormatException ("Compression not yet supported");
         
         if (header.Unsychronisation)
            field_data = SynchData.UnsynchByteVector (field_data);
         
         if (front_data.Count > 0)
            field_data.Insert (0, front_data);
         
         header.FrameSize = (uint) field_data.Count;
         ByteVector header_data = header.Render (version);
         header_data.Add (field_data);
         
         return header_data;
      }
      
      public virtual void SetText (string text) {}
      #endregion
      
      
      
      #region Public Static Methods
      public static ByteVector TextDelimiter (StringType t)
      {
         return new ByteVector ((t == StringType.UTF16 || t == StringType.UTF16BE) ? 2 : 1, (byte) 0);
      }
      #endregion
      
      
      
      #region Protected Methods
      #endregion
      
      
      
      
      
      protected StringType CorrectEncoding (StringType type, uint version)
      {
         if (Tag.ForceDefaultEncoding)
            type = Tag.DefaultEncoding;
         return (version < 4 && type == StringType.UTF8) ? StringType.UTF16 : type;
      }
      
      protected void SetData (ByteVector data, int offset, uint version)
      {
         header = new FrameHeader (data, version);
         ParseFields (FieldData (data, offset, version), version);
      }

      protected internal FrameHeader Header
      {
         get {return header;}
         set {header = value;}
      }
      
      protected virtual void ParseFields(ByteVector data, uint version) {}
      protected virtual ByteVector RenderFields (uint version) {return new ByteVector ();}
      protected ByteVector FieldData (ByteVector frame_data, int offset, uint version)
      {
         int data_offset              = offset + (int) FrameHeader.Size (version);
         int data_length              = (int) Size;
         /*int uncompressed_data_length;*/
         
         if (header.Compression || header.DataLengthIndicator)
         {
            /*uncompressed_data_length = (int) frame_data.Mid (data_offset, 4).ToUInt () + 4;*/
            data_offset += 4;
            data_offset -= 4;
         }
         
         if (header.GroupingIdentity)
         {
            group_id = frame_data [data_offset++];
            data_length--;
         }
         
         if (header.Encryption)
         {
            encryption_id = frame_data [data_offset++];
            data_length--;
         }
         
         ByteVector data = frame_data.Mid (data_offset, data_length);
         
         if (header.Unsychronisation)
            data = SynchData.ResynchByteVector (data);
         
         // FIXME: Implement encryption.
         if (header.Encryption)
            throw new UnsupportedFormatException ();
         
         // FIXME: Implement compression.
         if (header.Compression)
            throw new UnsupportedFormatException ();
         
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
   }
}
