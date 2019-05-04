//
// AppleElementaryStreamDescriptor.cs: Provides an implementation of an Apple
// ItemListBox.
//
// Author:
//  Brian Nickel (brian.nickel@gmail.com)
//  RBoy1
//
// Copyright (C) 2006-2007 Brian Nickel
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

namespace TagLib.Mpeg4
{
	/// <summary>
	///    This class extends <see cref="FullBox" /> to provide an
	///    implementation of an Apple ElementaryStreamDescriptor.
	/// </summary>
	/// <remarks>
	///    This box may appear as a child of a <see
	///    cref="IsoAudioSampleEntry" /> and provided further information
	///    about an audio stream.
	/// </remarks>
	public class AppleElementaryStreamDescriptor : FullBox
	{
		#region Private Fields

		/// <summary>
		/// Descriptor Tags
		/// </summary>
		enum DescriptorTag
		{
			Forbidden_00 = 0,
			ObjectDescrTag = 1,
			InitialObjectDescrTag = 2,
			ES_DescrTag = 3,
			DecoderConfigDescrTag = 4,
			DecSpecificInfoTag = 5,
			SLConfigDescrTag = 6,
			ContentIdentDescrTag = 7,
			SupplContentIdentDescrTag = 8,
			IPI_DescrPointerTag = 9,
			IPMP_DescrPointerTag = 10,
			IPMP_DescrTag = 11,
			QoS_DescrTag = 12,
			RegistrationDescrTag = 13,
			ES_ID_IncTag = 14,
			ES_ID_RefTag = 15,
			MP4_IOD_Tag = 16,
			MP4_OD_Tag = 17,
			IPL_DescrPointerRefTag = 18,
			ExtensionProfileLevelDescrTag = 19,
			profileLevelIndicationIndexDescrTag = 20,
			ReservedForFutureISOUse_15_TO_3F = 21,
			ContentClassificationDescrTag = 64,
			KeyWordDescrTag = 65,
			RatingDescrTag = 66,
			LanguageDescrTag = 67,
			ShortTextualDescrTag = 68,
			ExpandedTextualDescrTag = 69,
			ContentCreatorNameDescrTag = 70,
			ContentCreationDateDescrTag = 71,
			OCICreatorNameDescrTag = 72,
			OCICreationDateDescrTag = 73,
			SmpteCameraPositionDescrTag = 74,
			SegmentDescrTag = 75,
			MediaTimeDescrTag = 76,
			ReservedForFutureISOUseOCI = 77,
			IPMP_ToolsListDescrTag = 96,
			IPMP_ToolTag = 97,
			M4MuxTimingDescrTag = 98,
			M4MuxCodeTableDescrTag = 99,
			ExtSLConfigDescrTag = 100,
			M4MuxBufferSizeDescrTag = 101,
			M4MuxIdentDescrTag = 102,
			DependencyPointerTag = 103,
			DependencyMarkerTag = 104,
			M4MuxChannelDescrTag = 105,
			ReservedForFutureISO_6A_TO_BF = 106,
			UserPrivate = 192,
			Forbidden_FF = 255,
		}

		/// <summary>
		/// the ES_ID of another elementary stream on which this elementary stream depends
		/// </summary>
		ushort dependsOn_ES_ID;

		/// <summary>
		/// Indicates that a dependsOn_ES_ID will follow
		/// </summary>
		readonly bool stream_dependence_flag;

		/// <summary>
		/// OCR Stream Flag
		/// </summary>
		readonly bool ocr_stream_flag;

		/// <summary>
		/// OCR ES_ID
		/// </summary>
		ushort OCR_ES_Id;

		/// <summary>
		/// Indicates that a URLstring will follow
		/// </summary>
		readonly bool URL_flag;

		/// <summary>
		/// Length of URL String
		/// </summary>
		readonly byte URLlength;

		/// <summary>
		/// URL String of URLlength, contains a URL that shall point to the location of an SL-packetized stream by name
		/// </summary>
		string URLstring;

		/// <summary>
		/// Indicates that this stream is used for upstream information
		/// </summary>
		bool upStream;

		/// <summary>
		///    Contains the maximum bitrate.
		/// </summary>
		readonly uint max_bitrate;

		/// <summary>
		///    Contains the average bitrate.
		/// </summary>
		readonly uint average_bitrate;

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AppleElementaryStreamDescriptor" /> with a provided
		///    header and handler by reading the contents from a
		///    specified file.
		/// </summary>
		/// <param name="header">
		///    A <see cref="BoxHeader" /> object containing the header
		///    to use for the new instance.
		/// </param>
		/// <param name="file">
		///    A <see cref="TagLib.File" /> object to read the contents
		///    of the box from.
		/// </param>
		/// <param name="handler">
		///    A <see cref="IsoHandlerBox" /> object containing the
		///    handler that applies to the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    Valid data could not be read.
		/// </exception>
		public AppleElementaryStreamDescriptor (BoxHeader header, TagLib.File file, IsoHandlerBox handler)
			: base (header, file, handler)
		{
			/* ES_Descriptor Specifications
             *  Section 7.2.6.5 http://ecee.colorado.edu/~ecen5653/ecen5653/papers/ISO%2014496-1%202004.PDF
             */

			ByteVector box_data = file.ReadBlock (DataSize);
			DecoderConfig = new ByteVector ();
			int offset = 0;

			// Elementary Stream Descriptor Tag
			if ((DescriptorTag)box_data[offset++] != DescriptorTag.ES_DescrTag)
				throw new CorruptFileException ("Invalid Elementary Stream Descriptor, missing tag.");

			// We have a descriptor tag. Check that the remainder of the tag is at least [Base (3 bytes) + DecoderConfigDescriptor (15 bytes) + SLConfigDescriptor (3 bytes) + OtherDescriptors] bytes long
			uint es_length = ReadLength (box_data, ref offset);
			uint min_es_length = 3 + 15 + 3; // Base minimum length
			if (es_length < min_es_length)
				throw new CorruptFileException ("Insufficient data present.");

			StreamId = box_data.Mid (offset, 2).ToUShort ();
			offset += 2; // Done with ES_ID

			stream_dependence_flag = ((byte)((box_data[offset] >> 7) & 0x1) == 0x1 ? true : false); // 1st bit
			URL_flag = ((byte)((box_data[offset] >> 6) & 0x1) == 0x1 ? true : false); // 2nd bit
			ocr_stream_flag = ((byte)((box_data[offset] >> 5) & 0x1) == 0x1 ? true : false); // 3rd bit
			StreamPriority = (byte)(box_data[offset++] & 0x1F); // Last 5 bits and we're done with this byte

			if (stream_dependence_flag) {
				min_es_length += 2; // We need 2 more bytes
				if (es_length < min_es_length)
					throw new CorruptFileException ("Insufficient data present.");

				dependsOn_ES_ID = box_data.Mid (offset, 2).ToUShort ();
				offset += 2; // Done with stream dependence
			}

			if (URL_flag) {
				min_es_length += 2; // We need 1 more byte
				if (es_length < min_es_length)
					throw new CorruptFileException ("Insufficient data present.");

				URLlength = box_data[offset++]; // URL Length
				min_es_length += URLlength; // We need URLength more bytes
				if (es_length < min_es_length)
					throw new CorruptFileException ("Insufficient data present.");

				URLstring = box_data.Mid (offset, URLlength).ToString (); // URL name
				offset += URLlength; // Done with URL name
			}

			if (ocr_stream_flag) {
				min_es_length += 2; // We need 2 more bytes
				if (es_length < min_es_length)
					throw new CorruptFileException ("Insufficient data present.");

				OCR_ES_Id = box_data.Mid (offset, 2).ToUShort ();
				offset += 2; // Done with OCR
			}

			while (offset < DataSize) // Loop through all trailing Descriptors Tags
			{
				DescriptorTag tag = (DescriptorTag)box_data[offset++];
				switch (tag) {
				case DescriptorTag.DecoderConfigDescrTag: // DecoderConfigDescriptor
					{
						// Check that the remainder of the tag is at least 13 bytes long (13 + DecoderSpecificInfo[] + profileLevelIndicationIndexDescriptor[])
						if (ReadLength (box_data, ref offset) < 13)
							throw new CorruptFileException ("Could not read data. Too small.");

						// Read a lot of good info.
						ObjectTypeId = box_data[offset++];

						StreamType = (byte)(box_data[offset] >> 2); // First 6 bits
						upStream = ((byte)((box_data[offset++] >> 1) & 0x1) == 0x1 ? true : false); // 7th bit and we're done with the stream bits

						BufferSizeDB = box_data.Mid (offset, 3).ToUInt ();
						offset += 3; // Done with bufferSizeDB

						max_bitrate = box_data.Mid (offset, 4).ToUInt ();
						offset += 4; // Done with maxBitrate

						average_bitrate = box_data.Mid (offset, 4).ToUInt ();
						offset += 4; // Done with avgBitrate

						// If there's a DecoderSpecificInfo[] array at the end it'll pick it up in the while loop
					}
					break;

				case DescriptorTag.DecSpecificInfoTag: // DecoderSpecificInfo
					{
						// The rest of the info is decoder specific.
						uint length = ReadLength (box_data, ref offset);

						DecoderConfig = box_data.Mid (offset, (int)length);
						offset += (int)length; // We're done with the config
					}
					break;


				case DescriptorTag.SLConfigDescrTag: // SLConfigDescriptor
					{
						// The rest of the info is SL specific.
						uint length = ReadLength (box_data, ref offset);

						offset += (int)length; // Skip the rest of the descriptor as reported in the length so we can move onto the next one
					}
					break;

				case DescriptorTag.Forbidden_00:
				case DescriptorTag.Forbidden_FF:
					throw new CorruptFileException ("Invalid Descriptor tag.");

				default: {
						/* TODO: Should we handle other optional descriptor tags?
						 *  ExtensionDescriptor extDescr[0 .. 255];
							LanguageDescriptor langDescr[0 .. 1];
							IPI_DescPointer ipiPtr[0 .. 1];
							IP_IdentificationDataSet ipIDS[0 .. 1];
							QoS_Descriptor qosDescr[0 .. 1];
						 */
						uint length = ReadLength (box_data, ref offset); // Every descriptor starts with a length

						offset += (int)length; // Skip the rest of the descriptor as reported in the length so we can move onto the next one
						break;
					}
				}
			}
		}

		#endregion



		#region Public Properties

		/// <summary>
		///    Gets the ID of the stream described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ushort" /> value containing the ID of the
		///    stream described by the current instance.
		/// </value>
		public ushort StreamId { get; }

		/// <summary>
		///    Gets the priority of the stream described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> value containing the priority of
		///    the stream described by the current instance.
		/// </value>
		public byte StreamPriority { get; }

		/// <summary>
		///    Gets the object type ID of the stream described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> value containing the object type ID
		///    of the stream described by the current instance.
		/// </value>
		public byte ObjectTypeId { get; }

		/// <summary>
		///    Gets the type the stream described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> value containing the type the
		///    stream described by the current instance.
		/// </value>
		public byte StreamType { get; }

		/// <summary>
		///    Gets the buffer size DB value the stream described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the buffer size DB
		///    value the stream described by the current instance.
		/// </value>
		public uint BufferSizeDB { get; }

		/// <summary>
		///    Gets the maximum bitrate the stream described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the maximum
		///    bitrate the stream described by the current instance.
		/// </value>
		public uint MaximumBitrate { get { return max_bitrate / 1000; } }

		/// <summary>
		///    Gets the maximum average the stream described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the average
		///    bitrate the stream described by the current instance.
		/// </value>
		public uint AverageBitrate { get { return average_bitrate / 1000; } }

        /// <summary>
        ///    Gets the decoder config data of stream described by the
        ///    current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="ByteVector" /> object containing the decoder
        ///    config data of the stream described by the current
        ///    instance.
        /// </value>
        public ByteVector DecoderConfig { get; }

        #endregion



        #region Private Methods

        /// <summary>
        ///    Reads a section length and updates the offset to the end
        ///    of of the length block.
        /// </summary>
        /// <param name="data">
        ///    A <see cref="ByteVector" /> object to read from.
        /// </param>
        /// <param name="offset">
        ///    A <see cref="int" /> value reference specifying the
        ///    offset at which to read. This value gets updated to the
        ///    position following the size data.
        /// </param>
        /// <returns>
        ///    A <see cref="uint" /> value containing the length that
        ///    was read.
        /// </returns>
        static uint ReadLength (ByteVector data, ref int offset)
		{
			byte b;
			int end = offset + 4;
			uint length = 0;

			do {
				b = data[offset++];
				length = (uint)(length << 7) | (uint)(b & 0x7f);
			} while ((b & 0x80) != 0 && offset <= end); // The Length could be between 1 and 4 bytes for each descriptor

			return length;
		}

		#endregion
	}
}
