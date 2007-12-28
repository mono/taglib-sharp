//
// Frame.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2frame.cpp from TagLib
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

namespace TagLib.Id3v2 {
	/// <summary>
	///    This abstract class provides a basic framework for representing
	///    ID3v2.4 frames.
	/// </summary>
	public abstract class Frame : ICloneable
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the frame's header.
		/// </summary>
		private FrameHeader header;
		
		/// <summary>
		///    Contains the frame's grouping ID.
		/// </summary>
		private byte group_id;
		
		/// <summary>
		///    Contains the frame's encryption ID.
		/// </summary>
		private byte encryption_id;
		
		#endregion
		
		
		
		#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Frame" /> by reading the raw header encoded in the
		///    specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier or header data to use for the new instance.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> value indicating the ID3v2 version
		///    which <paramref name="data" /> is encoded in.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="data" /> does not contain a complete
		///    identifier.
		/// </exception>
		protected Frame (ByteVector data, byte version)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Count < ((version < 3) ? 3 : 4))
				throw new ArgumentException (
					"Data contains an incomplete identifier.",
					"data");
			
			header = new FrameHeader (data, version);
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Frame" /> with a specified header.
		/// </summary>
		/// <param name="header">
		///    A <see cref="FrameHeader" /> value containing the header
		///    to use for the new instance.
		/// </param>
		protected Frame (FrameHeader header)
		{
			this.header = header;
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		/// <summary>
		///    Gets the frame ID for the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="ReadOnlyByteVector" /> object containing the
		///    four-byte ID3v2.4 frame header for the current instance.
		/// </value>
		public ReadOnlyByteVector FrameId {
			get {return header.FrameId;}
		}
		
		/// <summary>
		///    Gets the size of the current instance as it was last
		///    stored on disk.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the size of the
		///    current instance as it was last stored on disk.
		/// </value>
		public uint Size {
			get {return header.FrameSize;}
		}
		
		/// <summary>
		///    Gets and sets the frame flags applied to the current
		///    instance.
		/// </summary>
		/// <value>
		///    A bitwise combined <see cref="FrameFlags" /> value
		///    containing the frame flags applied to the current
		///    instance.
		/// </value>
		/// <remarks>
		///    If the value includes either <see
		///    cref="FrameFlags.Encryption" /> or <see
		///    cref="FrameFlags.Compression" />, <see cref="Render" />
		///    will throw a <see cref="NotImplementedException" />.
		/// </remarks>
		public FrameFlags Flags {
			get {return header.Flags;}
			set {header.Flags = value;}
		}
		
		/// <summary>
		///    Gets and sets the grouping ID applied to the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="short" /> value containing the grouping
		///    identifier for the current instance, or -1 if not set.
		/// </value>
		/// <remarks>
		///    Grouping identifiers can be between 0 and 255. Setting
		///    any other value will unset the grouping identity and set
		///    the value to -1.
		/// </remarks>
		public short GroupId {
			get {
				return (Flags & FrameFlags.GroupingIdentity)
					!= 0 ? group_id : (short) -1;
			}
			set {
				if (value >= 0x00 && value <= 0xFF) {
					group_id = (byte) value;
					Flags |= FrameFlags.GroupingIdentity;
				} else {
					Flags &= ~FrameFlags.GroupingIdentity;
				}
			}
		}
		
		/// <summary>
		///    Gets and sets the encryption ID applied to the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="short" /> value containing the encryption
		///    identifier for the current instance, or -1 if not set.
		/// </value>
		/// <remarks>
		///    <para>Encryption identifiers can be between 0 and 255.
		///    Setting any other value will unset the grouping identity
		///    and set the value to -1.</para>
		///    <para>If set, <see cref="Render" /> will throw a <see
		///    cref="NotImplementedException" />.</para>
		/// </remarks>
		public short EncryptionId {
			get {
				return (Flags & FrameFlags.Encryption) != 0 ?
					encryption_id : (short) -1;
			}
			set {
				if (value >= 0x00 && value <= 0xFF) {
					encryption_id = (byte) value;
					Flags |= FrameFlags.Encryption;
				} else {
					Flags &= ~FrameFlags.Encryption;
				}
			}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		/// <summary>
		///    Renders the current instance, encoded in a specified
		///    ID3v2 version.
		/// </summary>
		/// <param name="version">
		///    A <see cref="byte" /> value specifying the version of
		///    ID3v2 to use when encoding the current instance.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		/// <exception cref="NotImplementedException">
		///    The current instance uses some feature that cannot be
		///    implemented in the specified ID3v2 version, or uses a
		///    feature, such as encryption or compression, which is not
		///    yet implemented in the library.
		/// </exception>
		public virtual ByteVector Render (byte version)
		{
			// Remove flags that are not supported by older versions
			// of ID3v2.
			if (version < 4)
				Flags &= ~(FrameFlags.DataLengthIndicator |
					FrameFlags.Unsychronisation);
			
			if (version < 3)
				Flags &= ~(FrameFlags.Compression |
					FrameFlags.Encryption |
					FrameFlags.FileAlterPreservation |
					FrameFlags.GroupingIdentity |
					FrameFlags.ReadOnly |
					FrameFlags.TagAlterPreservation);
			
			ByteVector field_data = RenderFields (version);
			
			// If we don't have any content, don't render anything.
			// This will cause the frame to not be rendered.
			if (field_data.Count == 0)
				return new ByteVector ();
			
			ByteVector front_data = new ByteVector ();
			
			if ((Flags & (FrameFlags.Compression |
				FrameFlags.DataLengthIndicator)) != 0)
				front_data.Add (ByteVector.FromUInt ((uint)
					field_data.Count));
			
			if ((Flags & FrameFlags.GroupingIdentity) != 0)
				front_data.Add (group_id);
			
			if ((Flags & FrameFlags.Encryption) != 0)
				front_data.Add (encryption_id);
			
			// FIXME: Implement compression.
			if ((Flags & FrameFlags.Compression) != 0)
				throw new NotImplementedException (
					"Compression not yet supported");
			
			// FIXME: Implement encryption.
			if ((Flags & FrameFlags.Encryption) != 0)
				throw new NotImplementedException (
					"Encryption not yet supported");
			
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
		
		/// <summary>
		///    Gets the text delimiter for a specified encoding.
		/// </summary>
		/// <param name="type">
		///    A <see cref="StringType" /> value specifying the encoding
		///    to get the delimiter for.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    delimiter for the specified encoding.
		/// </returns>
		[Obsolete("Use ByteVector.TextDelimiter.")]
		public static ByteVector TextDelimiter (StringType type)
		{
			return ByteVector.TextDelimiter (type);
		}
		
		#endregion
		
		
		
		#region Protected Methods
		
		protected static StringType CorrectEncoding (StringType type,
		                                             byte version)
		{
			if (Tag.ForceDefaultEncoding)
				type = Tag.DefaultEncoding;
			
			return (version < 4 && type == StringType.UTF8) ?
				StringType.UTF16 : type;
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
         
         if (data_length > frameData.Count - data_offset)
         	throw new CorruptFileException ("Frame size exceeds bounds.");
         if (data_length < 0 )
         	throw new CorruptFileException ("Frame size less than zero.");
         
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
		
		
		
#region IClonable
		
		public virtual Frame Clone ()
		{
			int index = 0;
			return FrameFactory.CreateFrame (Render (4), ref index, 4);
		}
		
		object ICloneable.Clone ()
		{
			return Clone ();
		}
		
#endregion
	}
}
