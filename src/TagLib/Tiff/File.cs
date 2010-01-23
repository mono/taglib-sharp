//
// File.cs:
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
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
using TagLib.IFD.Tags;
using TagLib.Xmp;

namespace TagLib.Tiff
{
	/// <summary>
	///    This class extends <see cref="TagLib.File" /> to provide tagging
	///    and properties support for Tiff files.
	/// </summary>
	[SupportedMimeType("taglib/tiff", "tiff")]
	[SupportedMimeType("taglib/tif", "tif")]
	[SupportedMimeType("image/tiff")]
	public class File : TagLib.Image.File
	{
#region Private Fields

		/// <summary>
		///    Contains the media properties.
		/// </summary>
		private Properties properties;

		/// <summary>
		///    Whether or not the file is big-endian.
		/// </summary>
		private bool is_bigendian;

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
			ImageTag = new CombinedImageTag (TagTypes.TiffIFD | TagTypes.XMP);

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
			throw new NotImplementedException ();
		}

#endregion

#region Private Methods


		/// <summary>
		///    Starts parsing the TIFF header of the file from beginning
		///    and sets <see cref="is_bigendian"/> according to the header.
		///    The method returns the offset to the first IFD.
		/// </summary>
		/// <returns>
		///    A <see cref="System.UInt32"/> representing the offset to first
		///    IFD of this file.
		/// </returns>
		private uint ReadFirstIFDOffset ()
		{
			// 2 + 2 + 4 (Byte Order + TIFF Magic Number + Offset)
			int tiff_header_size = 8;

			Seek (0, SeekOrigin.Begin);

			ByteVector header = ReadBlock (tiff_header_size);

			is_bigendian = header.Mid (0, 2).ToString ().Equals ("MM");

			ushort magic = header.Mid (2, 2).ToUShort (is_bigendian);
			if (magic != 42)
				throw new Exception (String.Format ("Invalid TIFF magic: {0}", magic));

			return header.Mid (4, 4).ToUInt (is_bigendian);
		}


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
				uint offset = ReadFirstIFDOffset ();

				var ifd_tag = new IFDTag ();
				var reader = new IFDReader (this, is_bigendian, ifd_tag.Structure, 0, offset, (uint) Length);
				reader.Read ();
				ImageTag.AddTag (ifd_tag);

				// Find XMP data
				var xmp_entry = ifd_tag.Structure.GetEntry (0, (ushort) IFDEntryTag.XMP) as ByteVectorIFDEntry;
				if (xmp_entry != null) {
					ImageTag.AddTag (new XmpTag (xmp_entry.Data.ToString ()));
				}

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
			int width = 0, height = 0;

			IFDTag tag = GetTag (TagTypes.TiffIFD) as IFDTag;

			IFDEntry width_entry = tag.Structure.GetEntry (0, (ushort) IFDEntryTag.ImageWidth);
			if (width_entry != null)
				width = (width_entry as ShortIFDEntry).Value;

			IFDEntry height_entry = tag.Structure.GetEntry (0, (ushort) IFDEntryTag.ImageLength);
			if (height_entry != null)
				height = (height_entry as ShortIFDEntry).Value;

			if (width > 0 && height > 0) {
				return new Properties (TimeSpan.Zero, new Codec (width, height));
			}

			return null;
		}

#endregion
	}
}
