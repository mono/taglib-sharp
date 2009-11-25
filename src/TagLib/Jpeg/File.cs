//
// File.cs: Provides tagging for Jpeg files
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Ruben Vermeersch
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
using System.Collections.Generic;
using System.IO;

using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Xmp;

namespace TagLib.Jpeg
{

	/// <summary>
	///    This class extends <see cref="TagLib.File" /> to provide tagging
	///    and properties support for Jpeg files.
	/// </summary>
	[SupportedMimeType("taglib/jpg", "jpg")]
	[SupportedMimeType("taglib/jpeg", "jpeg")]
	[SupportedMimeType("taglib/jpe", "jpe")]
	[SupportedMimeType("taglib/jif", "jif")]
	[SupportedMimeType("taglib/jfif", "jfif")]
	[SupportedMimeType("taglib/jfi", "jfi")]
	[SupportedMimeType("image/jpeg")]
	public class File : TagLib.Image.File
	{

		/// <summary>
		///    The magic bits used to recognize an Exif segment
		/// </summary>
		private static readonly string EXIF_IDENTIFIER = "Exif\0\0";

#region Private Fields

		/// <summary>
		///    Contains the media properties.
		/// </summary>
		private Properties properties;

		/// <summary>
		///    Contains the position of the Exif segment, if it is not contained in
		///    metadata segment, to delete it.
		/// </summary>
		private long exif_position = 0;

		/// <summary>
		///    Contains the position of the Xmp segment, if it is not contained in
		///    metadata segment, to delete it.
		/// </summary>
		private long xmp_position = 0;

		/// <summary>
		///    Contains the position of the Comment segment, if it is not contained in
		///    metadata segment, to delete it.
		/// </summary>
		private long comment_position = 0;

		/// <summary>
		///   We store the size of a block at the beginning of the file, which contains
		///   the whole metadata we handle. We write this block back at once, so we need
		///   to know how much bytes have to be overwritten.
		/// </summary>
		private long metadata_length = 0;

#endregion

#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified path in the local file
		///    system and specified read style.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public File (string path, ReadStyle propertiesStyle)
			: this (new File.LocalFileAbstraction (path),
				propertiesStyle)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified path in the local file
		///    system.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path of the
		///    file to use in the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public File (string path) : base (path)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified file abstraction and
		///    specified read style.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		public File (File.IFileAbstraction abstraction,
		             ReadStyle propertiesStyle) : base (abstraction)
		{
			ImageTag = new CombinedImageTag (TagTypes.XMP | TagTypes.TiffIFD | TagTypes.JpegComment);

			Mode = AccessMode.Read;
			try {
				Read (propertiesStyle);
				TagTypesOnDisk = TagTypes;
			} finally {
				Mode = AccessMode.Closed;
			}
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="File" /> for a specified file abstraction.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="IFileAbstraction" /> object to use when
		///    reading from and writing to the file.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		protected File (IFileAbstraction abstraction) : base (abstraction)
		{
		}

#endregion

#region Public Properties

		/// <summary>
		///    Gets the media properties of the file represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TagLib.Properties" /> object containing the
		///    media properties of the file represented by the current
		///    instance.
		/// </value>
		public override TagLib.Properties Properties {
			get { return properties; }
		}

#endregion

#region Public Methods

		/// <summary>
		///    Saves the changes made in the current instance to the
		///    file it represents.
		/// </summary>
		public override void Save ()
		{
			Mode = AccessMode.Write;
			try {
				WriteMetadata ();

				TagTypesOnDisk = TagTypes;
			} finally {
				Mode = AccessMode.Closed;
			}
		}

#endregion

#region Private Methods

		/// <summary>
		///    Reads the file with a specified read style.
		/// </summary>
		/// <param name="propertiesStyle">
		///    A <see cref="ReadStyle" /> value specifying at what level
		///    of accuracy to read the media properties, or <see
		///    cref="ReadStyle.None" /> to ignore the properties.
		/// </param>
		private void Read (ReadStyle propertiesStyle)
		{
			Mode = AccessMode.Read;
			try {
				ValidateHeader ();
				ReadMetadata ();

				if (propertiesStyle == ReadStyle.None)
					return;

				properties = ExtractProperties ();
			} finally {
				Mode = AccessMode.Closed;
			}
		}

		/// <summary>
		///    Attempts to extract the media properties of the main
		///    photo.
		/// </summary>
		/// <returns>
		///    A <see cref="Properties" /> object with a best effort guess
		///    at the right values. When no guess at all can be made,
		///    <see langword="null" /> is returned.
		/// </returns>
		private Properties ExtractProperties ()
		{
			// FIXME: extract properties
			return null;
		}

		/// <summary>
		///    Validates if the opened file is actually a JPEG.
		/// </summary>
		private void ValidateHeader ()
		{
			ByteVector segment = ReadBlock (2);
			if (segment.ToUShort () != 0xFFD8)
				throw new CorruptFileException ("Expected SOI marker at the start of the file.");
		}


		/// <summary>
		///    Reads a segment marker for a segment starting at current position.
		///    The second byte of the marker is returned, since the first is equal
		///    to 0xFF in every case.
		/// </summary>
		/// <returns>
		///    A <see cref="TagLib.Jpeg.Marker"/> with the second byte of the segment marker.
		/// </returns>
		private Marker ReadSegmentMarker ()
		{
			if (Tell + 2 >= Length)
				throw new Exception ("Marker size exceeds file size");

			ByteVector segment_header = ReadBlock (2);

			if ( ! segment_header.StartsWith (0xFF))
				throw new Exception ("Start of Segment expected at " + (Tell - 2));

			return (Marker)segment_header[1];
		}


		/// <summary>
		///    Reads the size of a segment at the current position.
		/// </summary>
		/// <returns>
		///    A <see cref="System.UInt16"/> with the size of the current segment.
		/// </returns>
		private ushort ReadSegmentSize ()
		{
			long position = Tell;

			if (Tell + 2 >= Length)
				throw new Exception ("Segment length-description size exceeds file size");

			ushort segment_size = ReadBlock (2).ToUShort ();

			if (position + segment_size >= Length)
				throw new Exception ("Segment size exceeds file size");

			return segment_size;
		}


		/// <summary>
		///    Extracts the metadata from the current file by reading every segment in file.
		///    Method should be called with read position at first segment marker.
		/// </summary>
		private void ReadMetadata ()
		{
			Marker marker = ReadSegmentMarker ();

			// loop while marker is not EOI and not the data segment
			while (marker != Marker.EOI && marker != Marker.SOS) {

				long position = Tell;
				ushort segment_size = ReadSegmentSize ();

				if (segment_size <= 2)
					throw new CorruptFileException ("Segment size must be greater than 2");

				// segment size contains 2 bytes of the size itself, so
				// pure data size is this
				ushort data_size = (ushort) (segment_size - 2);

				bool is_metadata = false;

				// possibly JFIF header
				if (marker == Marker.APP0) {
					is_metadata = ValidateJFIFHeader (data_size);

				// possibly Exif or Xmp data found
				} else if (marker == Marker.APP1) {
					is_metadata = ReadAPP1Segment (data_size);

				// Comment segment found
				} else if (marker == Marker.COM) {
					is_metadata = ReadCOMSegment (data_size);

				}

				// if metadata was read in current segment
				// and all previous segments are metadata segments
				// (or the JFIF header), then add current segment to
				// metadata-block
				if (is_metadata && metadata_length == position - 2 - 2)
					metadata_length = position + segment_size - 2;

				// set position to next segment and start with next segment marker
				Seek (position + segment_size, SeekOrigin.Begin);
				marker = ReadSegmentMarker ();
			}
		}

		/// <summary>
		///    Validates the JFIF header at current position
		/// </summary>
		private bool ValidateJFIFHeader (ushort length)
		{
			if (Tell != 6)
				return false;

			if (ReadBlock (5).ToString ().Equals ("JFIF\0")) {
				return true;
			}

			return false;
		}

		/// <summary>
		///    Reads an APP1 segment to find EXIF or XMP metadata.
		/// </summary>
		/// <param name="length">
		///    The length of the segment that will be read.
		/// </param>
		private bool ReadAPP1Segment (ushort length)
		{
			long position = Tell;
			ByteVector data = null;

			// could be an Exif segment
			if (exif_position == 0 && length >= 14) {

				// for an Exif segment, the data block consists of:
				//    * 6 bytes Exif identifier string
				//    * 2 bytes bigendian indication MM (or II)
				//    * 2 bytes Tiff magic number (42)
				//    * 4 bytes offset of the first IFD in this segment
				//
				//    the last two points are alreay encoded according to
				//    big- or littleendian
				data = ReadBlock (14);

				if (data.Mid (0, 6).ToString ().Equals (EXIF_IDENTIFIER)) {

					bool is_bigendian = data.Mid (6, 2).ToString ().Equals ("MM");

					ushort magic = data.Mid (8, 2).ToUShort (is_bigendian);
					if (magic != 42)
						throw new Exception (String.Format ("Invalid TIFF magic: {0}", magic));

					uint ifd_offset = data.Mid (10, 4).ToUInt (is_bigendian);

					var exif = new IFDTag ();
					var reader = new IFDReader (this, is_bigendian, exif.Structure, position + 6, ifd_offset);
					reader.Read ();
					ImageTag.AddTag (exif);
					exif_position = position;

					return true;
				}
			}

			// could be an Xmp segment
			if (xmp_position == 0
			    && length >= XmpTag.XAP_NS.Length + 1) {

				// if already data is read for determining the Exif segment,
				// just read the remaining bytes.
				if (data == null)
					data = ReadBlock (XmpTag.XAP_NS.Length + 1);
				else
					data.Add (ReadBlock (XmpTag.XAP_NS.Length + 1 - 14));

				if (data.ToString ().Equals (XmpTag.XAP_NS + "\0")) {
					ByteVector xmp_data = ReadBlock (length - XmpTag.XAP_NS.Length - 1);

					ImageTag.AddTag (new XmpTag (xmp_data.ToString ()));
					xmp_position = position;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		///    Deletes a Segment starting at given position
		/// </summary>
		/// <param name="start">
		///    A <see cref="System.Int64"/> with the position of the first byte
		///    of the segment marker (0xFF)
		/// </param>
		private void DeleteSegment (long start)
		{
			Seek (start, SeekOrigin.Begin);

			ReadSegmentMarker ();
			ushort length = ReadSegmentSize ();

			Insert (new ByteVector (), start, length + 2);
		}

		/// <summary>
		///    Writes the metadata back to file. All metadata is stored in the first segments
		///    of the file.
		/// </summary>
		private void WriteMetadata ()
		{
			// Start with ByteVector containing the JFIF Header
			ByteVector data =
				new ByteVector (new byte[] {
					0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00,
					0x01, 0x01, 0x01, 0x00, 0x48, 0x00, 0x48, 0x00, 0x00});

			// Add segments to write in order
			data.Add (RenderExifSegment ());
			//data.Add (RenderXMPSegment ());
			data.Add (RenderCOMSegment ());

			// delete metdata not contained in the metadata block
			// not the efficentest way, but it probably is never really called
			long[] metadata_positions = {comment_position, xmp_position, exif_position};

			// start with greatest offset, because this do not affect the smaller ones
			Array.Sort<long> (metadata_positions);
			for (int i = metadata_positions.Length - 1; i >= 0; i--) {
				long metadata_position = metadata_positions[i];
				if (2 + metadata_length < metadata_position)
					DeleteSegment (metadata_position - 4);
			}

			// reset it, to not delete it the next time the metadata is saved
			comment_position = 0;
			xmp_position = 0;
			exif_position = 0;

			// Insert metadata block at the beginning of the file by overwriting
			// all segments at the beginning already containing metadata
			Insert (data, 2, metadata_length);

			metadata_length = data.Count;
		}

		/// <summary>
		///    Creates a <see cref="ByteVector"/> for the Exif segment of this file
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector"/> with the whole Exif segment, if exif tags
		///    exists, otherwise null.
		/// </returns>
		private ByteVector RenderExifSegment ()
		{
			// Check, if IFD0 is contained
			IFDTag exif = GetTag (TagTypes.TiffIFD) as IFDTag;
			if (exif == null)
				return null;

			// first IFD starts at 8
			uint first_ifd_offset = 8;

			// Render IFD0, we use bigendian every time, since no other parts of the file
			// are affected by this
			var renderer = new IFDRenderer (true, exif.Structure, first_ifd_offset);
			ByteVector exif_data = renderer.Render ();

			// Create whole segment
			ByteVector data = new ByteVector (new byte [] { 0xFF, 0xE1 });
			data.Add (ByteVector.FromUShort ((ushort) (first_ifd_offset + exif_data.Count + 2 + 6)));
			data.Add ("Exif\0\0");
			data.Add (ByteVector.FromString ("MM", StringType.Latin1));
			data.Add (ByteVector.FromUShort (42));
			data.Add (ByteVector.FromUInt (first_ifd_offset));

			// Add ifd data itself
			data.Add (exif_data);

			return data;
		}


		/// <summary>
		///    Creates a <see cref="ByteVector"/> for the Xmp segment of this file
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector"/> with the whole Xmp segment, if xmp tags
		///    exists, otherwise null.
		/// </returns>
		private ByteVector RenderXMPSegment ()
		{
			throw new NotImplementedException ();
		}


		/// <summary>
		///    Reads a COM segment to find the JPEG comment.
		/// </summary>
		/// <param name="length">
		///    The length of the segment that will be read.
		/// </param>
		private bool ReadCOMSegment (int length)
		{
			if (comment_position != 0)
				return false;

			comment_position = Tell;
			ImageTag.AddTag (new JpegCommentTag (ReadBlock (length - 1).ToString ()));
			return true;
		}

		/// <summary>
		///    Creates a <see cref="ByteVector"/> for the comment segment of this file
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector"/> with the whole comment segment, if a comment tag
		///    exists, otherwise null.
		/// </returns>
		private ByteVector RenderCOMSegment ()
		{
			// check, if Comment is contained
			JpegCommentTag com_tag = (GetTag (TagTypes.JpegComment) as JpegCommentTag);
			if (com_tag == null)
				return null;

			// create comment data
			ByteVector com_data = ByteVector.FromString (com_tag.Value + "\0", StringType.Latin1);

			// create segment
			ByteVector data = new ByteVector (new byte [] { 0xFF, 0xFE });
			data.Add (ByteVector.FromUShort ((ushort) (com_data.Count + 2)));

			data.Add (com_data);

			return data;
		}

#endregion
	}
}
