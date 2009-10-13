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

using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Tiff
{
	/// <summary>
	///    This class extends <see cref="TagLib.File" /> to provide tagging
	///    and properties support for Tiff files.
	/// </summary>
	[SupportedMimeType("taglib/tiff", "tiff")]
	[SupportedMimeType("taglib/tif", "tif")]
	[SupportedMimeType("image/tiff")]
	public class File : TagLib.File
	{
#region Private Fields

		/// <summary>
		///   Contains the tags for the file.
		/// </summary>
		private TiffTag tiff_tag;

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
			tiff_tag = new TiffTag (this);

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
		///    Gets a abstract representation of all tags stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TagLib.Tag" /> object representing all tags
		///    stored in the current instance.
		/// </value>
		public override Tag Tag {
			get { return tiff_tag; }
		}

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

		/// <summary>
		///    Removes a set of tag types from the current instance.
		/// </summary>
		/// <param name="types">
		///    A bitwise combined <see cref="TagLib.TagTypes" /> value
		///    containing tag types to be removed from the file.
		/// </param>
		/// <remarks>
		///    In order to remove all tags from a file, pass <see
		///    cref="TagTypes.AllTags" /> as <paramref name="types" />.
		/// </remarks>
		public override void RemoveTags (TagLib.TagTypes types)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		///    Gets a tag of a specified type from the current instance,
		///    optionally creating a new tag if possible.
		/// </summary>
		/// <param name="type">
		///    A <see cref="TagLib.TagTypes" /> value indicating the
		///    type of tag to read.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> value specifying whether or not to
		///    try and create the tag if one is not found.
		/// </param>
		/// <returns>
		///    A <see cref="Tag" /> object containing the tag that was
		///    found in or added to the current instance. If no
		///    matching tag was found and none was created, <see
		///    langword="null" /> is returned.
		/// </returns>
		public override TagLib.Tag GetTag (TagLib.TagTypes type,
		                                   bool create)
		{
			if (create)
				throw new NotImplementedException ();

			foreach (Tag tag in tiff_tag.Tags) {
				if ((tag.TagTypes & type) == type)
					return tag;
			}
			return null;
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
				uint next_offset = ReadFirstIFDOffset ();

				tiff_tag.AddIFDTag (new IFDTag (this, next_offset, is_bigendian, out next_offset));

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

			IFDEntry width_entry = tag.GetEntry ((ushort) IFDEntryTag.ImageWidth);
			if (width_entry != null)
				width = (width_entry as ShortIFDEntry).Value;

			IFDEntry height_entry = tag.GetEntry ((ushort) IFDEntryTag.ImageLength);
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
