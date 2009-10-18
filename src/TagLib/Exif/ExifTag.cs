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

		/// <summary>
		///    A reference to the Exif IFD (which can be found by following the
		///    pointer in IFD0, ExifIFD tag). This variable should not be used
		///    directly, use the <see cref="ExifIFD"/> property instead.
		/// </summary>
		private IFDTag exif_ifd = null;

		/// <summary>
		///    The Exif IFD. Will create one if the file doesn't alread have it.
		/// </summary>
		private IFDTag ExifIFD {
			get {
				if (exif_ifd == null) {
					var entry = GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
					if (entry == null) {
						exif_ifd = new IFDTag ();
						entry = new SubIFDEntry ((uint) IFDEntryTag.ExifIFD, (ushort) IFDEntryType.Long, 1, exif_ifd);
						SetEntry (0, entry);
					}

					exif_ifd = entry.IFDTag;
				}

				return exif_ifd;
			}
		}

#region Public Properties

		public string UserComment {
			get {
				var comment_entry = ExifIFD.GetEntry (0, IFDEntryTag.UserComment) as UndefinedIFDEntry;

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

				var comment_entry = new UndefinedIFDEntry ((uint)IFDEntryTag.UserComment, data);

				ExifIFD.SetEntry (0, comment_entry);
			}
		}

		public DateTime DateTimeOriginal {
			get {
				return ExifIFD.GetDateTimeValue (0, (ushort) IFDEntryTag.DateTimeOriginal);
			}
			set {
				ExifIFD.SetDateTimeValue (0, (ushort) IFDEntryTag.DateTimeOriginal, value);
			}
		}

		public DateTime DateTimeDigitized {
			get {
				return ExifIFD.GetDateTimeValue (0, (ushort) IFDEntryTag.DateTimeDigitized);
			}
			set {
				ExifIFD.SetDateTimeValue (0, (ushort) IFDEntryTag.DateTimeDigitized, value);
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
				return ExifIFD.GetRationalValue (0, (ushort) IFDEntryTag.ExposureTime);
			}
		}

		/// <summary>
		///    Gets the FNumber the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the FNumber.
		/// </value>
		public override double FNumber {
			get {
				return ExifIFD.GetRationalValue (0, (ushort) IFDEntryTag.FNumber);
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
				return ExifIFD.GetLongValue (0, (ushort) IFDEntryTag.ISOSpeedRatings);
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
				return ExifIFD.GetRationalValue (0, (ushort) IFDEntryTag.FocalLength);
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
				return ExifIFD.GetStringValue (0, (ushort) IFDEntryTag.Make);
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
				return ExifIFD.GetStringValue (0, (ushort) IFDEntryTag.Model);
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

	}
}
