//
// ExifTag.cs:
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

using TagLib.IFD;
using TagLib.IFD.Entries;

namespace TagLib.Exif
{

	public class ExifTag : IFDTag
	{
		public static readonly ByteVector COMMENT_ASCCI_CODE = new byte[] {0x41, 0x53, 0x43, 0x49, 0x49, 0x00, 0x00, 0x00};
		public static readonly ByteVector COMMENT_JIS_CODE = new byte[] {0x4A, 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00};
		public static readonly ByteVector COMMENT_UNICODE_CODE = new byte[] {0x55, 0x4E, 0x49, 0x43, 0x4F, 0x44, 0x45, 0x00};
		public static readonly ByteVector COMMENT_UNDEFINED_CODE = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

#region Constructors

		public ExifTag (File file, long base_offset, uint ifd_offset, bool is_bigendian)
			: base (file, base_offset, ifd_offset, is_bigendian) {}


		public ExifTag (File file, uint ifd_offset, bool is_bigendian)
			: base (file, ifd_offset, is_bigendian) {}

		public ExifTag (File file) : base (file) {}

#endregion


#region Public Properties

		public string UserComment {
			get {
				UndefinedIFDEntry comment_entry =
					GetEntry (IFDEntryTag.UserComment) as UndefinedIFDEntry;

				if (comment_entry == null)
					return null;

				ByteVector data = comment_entry.Data;

				if (data.StartsWith (COMMENT_ASCCI_CODE))
					return data.ToString (StringType.Latin1, COMMENT_ASCCI_CODE.Count);

				if (data.StartsWith (COMMENT_UNICODE_CODE))
					return data.ToString (StringType.UTF8, COMMENT_UNICODE_CODE.Count);

				throw new NotImplementedException ("UserComment with other encoding than Latin1 or Unicode");
			}
			set {

				ByteVector data = new ByteVector ();
				data.Add (COMMENT_UNICODE_CODE);
				data.Add (ByteVector.FromString (value, StringType.UTF8));

				UndefinedIFDEntry comment_entry =
					new UndefinedIFDEntry ((uint)IFDEntryTag.UserComment, data);

				SetEntry (comment_entry);
			}
		}

		public DateTime DateTimeOriginal {
			get {
				return GetDateTimeValue ((ushort) IFDEntryTag.DateTimeOriginal);
			}
			set {
				SetDateTimeValue ((ushort) IFDEntryTag.DateTimeOriginal, value);
			}
		}

		public DateTime DateTimeDigitized {
			get {
				return GetDateTimeValue ((ushort) IFDEntryTag.DateTimeDigitized);
			}
			set {
				SetDateTimeValue ((ushort) IFDEntryTag.DateTimeDigitized, value);
			}
		}

		/// <summary>
		///    Gets or sets the comment for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the comment of the
		///    current instace.
		/// </value>
		public override string Comment {
			get { return UserComment; }
			set { UserComment = value; }
		}

		/// <summary>
		///    Gets or sets the time when the image, the current instance
		///    belongs to, was taken.
		/// </summary>
		/// <value>
		///    A <see cref="DateTime" /> with the time the image was taken.
		/// </value>
		public override DateTime DateTime {
			get { return DateTimeOriginal; }
			set { DateTimeOriginal = value; }
		}

		/// <summary>
		///    Gets the exposure time the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the exposure time in seconds.
		/// </value>
		public override double ExposureTime {
			get {
				return GetRationalValue ((ushort) IFDEntryTag.ExposureTime);
			}
		}

		//// <summary>
		///    Gets the FNumber the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the FNumber.
		/// </value>
		public override double FNumber {
			get {
				return GetRationalValue ((ushort) IFDEntryTag.FNumber);
			}
		}

		/// <summary>
		///    Gets the ISO speed the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> with the ISO speed as defined in ISO 12232.
		/// </value>
		public override uint ISOSpeedRatings {
			get {
				return GetLongValue ((ushort) IFDEntryTag.ISOSpeedRatings);
			}
		}

		/// <summary>
		///    Gets the focal length the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the focal length in millimeters.
		/// </value>
		public override double FocalLength {
			get {
				return GetRationalValue ((ushort) IFDEntryTag.FocalLength);
			}
		}

		/// <summary>
		///    Gets the manufacture of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the manufacture name.
		/// </value>
		public override string Make {
			get {
				return GetStringValue ((ushort) IFDEntryTag.Make);
			}
		}

		/// <summary>
		///    Gets the model name of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the model name.
		/// </value>
		public override string Model {
			get {
				return GetStringValue ((ushort) IFDEntryTag.Model);
			}
		}

#endregion

#region Public Methods

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.Exif" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.Exif; }
		}

#endregion

		/// <summary>
		///    Try to parse the given IFD entry, used to discover format-specific entries.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="System.UInt16"/> with the tag of the entry.
		/// </param>
		/// <param name="type">
		///    A <see cref="System.UInt16"/> with the type of the entry.
		/// </param>
		/// <param name="count">
		///    A <see cref="System.UInt32"/> with the data count of the entry.
		/// </param>
		/// <param name="base_offset">
		///    A <see cref="System.Int64"/> with the base offset which every offsets in the
		///    IFD are relative to.
		/// </param>
		/// <param name="offset">
		///    A <see cref="System.UInt32"/> with the offset of the entry.
		/// </param>
		/// <returns>
		///    A <see cref="IFDEntry"/> with the given parameters, or null if none was parsed, after
		///    which the normal TIFF parsing is used.
		/// </returns>
		protected override IFDEntry ParseIFDEntry (ushort tag, ushort type, uint count, long base_offset, uint offset) {
			IFDTag ifd_tag;

			switch (tag) {
				case (ushort) IFDEntryTag.ExifIFD:
				case (ushort) IFDEntryTag.IopIFD:
				case (ushort) IFDEntryTag.GPSIFD:
					ifd_tag = new IFDTag (File, base_offset, offset, is_bigendian);
					return new SubIFDEntry (tag, type, count, ifd_tag);

				case (ushort) IFDEntryTag.MakerNoteIFD:
					// A maker note may be a Sub IFD, but it may also be in an arbitrary
					// format. We try to parse a Sub IFD, if this fails, go ahead to read
					// it as an Undefined Entry below.
					try {
						ifd_tag = new IFDTag (File, base_offset, offset, is_bigendian);
						return new SubIFDEntry (tag, type, count, ifd_tag);
					} catch {
						return null;
					}

				default:
					return null;
			}
		}

	}
}
