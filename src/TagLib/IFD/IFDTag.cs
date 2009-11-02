//
// IFDTag.cs: Basic Tag-class to handle an IFD (Image File Directory) with
// its image-tags.
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
using TagLib.IFD.Entries;

namespace TagLib.IFD
{
	/// <summary>
	///    Contains the metadata for one IFD (Image File Directory).
	/// </summary>
	public class IFDTag : ImageTag
	{
		/// <summary>
		///   Marker for an ASCII-encoded UserComment tag.
		/// </summary>
		public static readonly ByteVector COMMENT_ASCII_CODE = new byte[] {0x41, 0x53, 0x43, 0x49, 0x49, 0x00, 0x00, 0x00};

		/// <summary>
		///   Marker for a JIS-encoded UserComment tag.
		/// </summary>
		public static readonly ByteVector COMMENT_JIS_CODE = new byte[] {0x4A, 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00};

		/// <summary>
		///   Marker for a UNICODE-encoded UserComment tag.
		/// </summary>
		public static readonly ByteVector COMMENT_UNICODE_CODE = new byte[] {0x55, 0x4E, 0x49, 0x43, 0x4F, 0x44, 0x45, 0x00};

		/// <summary>
		///   Marker for a UserComment tag with undefined encoding.
		/// </summary>
		public static readonly ByteVector COMMENT_UNDEFINED_CODE = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

#region Private Fields

		public IFDStructure Structure { get; private set; }

		/// <summary>
		///    A reference to the Exif IFD (which can be found by following the
		///    pointer in IFD0, ExifIFD tag). This variable should not be used
		///    directly, use the <see cref="ExifIFD"/> property instead.
		/// </summary>
		private IFDStructure exif_ifd = null;

		/// <summary>
		///    A reference to the GPS IFD (which can be found by following the
		///    pointer in IFD0, GPSIFD tag). This variable should not be used
		///    directly, use the <see cref="GPSIFD"/> property instead.
		/// </summary>
		private IFDStructure gps_ifd = null;

#endregion

#region Constructors

		/// <summary>
		///    Constructor. Creates an empty IFD tag. Can be populated manually, or via
		///    <see cref="IFDReader"/>.
		/// </summary>
		public IFDTag ()
		{
			Structure = new IFDStructure ();
		}

#endregion

#region Public Properties

		/// <summary>
		///    The Exif IFD. Will create one if the file doesn't alread have it.
		/// </summary>
		/// <remarks>
		///    <para>Note how this also creates an empty IFD for exif, even if
		///    you don't set a value. That's okay, empty nested IFDs get ignored
		///    when rendering.</para>
		/// </remarks>
		public IFDStructure ExifIFD {
			get {
				if (exif_ifd == null) {
					var entry = Structure.GetEntry (0, IFDEntryTag.ExifIFD) as SubIFDEntry;
					if (entry == null) {
						exif_ifd = new IFDStructure ();
						entry = new SubIFDEntry ((ushort) IFDEntryTag.ExifIFD, (ushort) IFDEntryType.Long, 1, exif_ifd, SubIFDType.Exif);
						Structure.SetEntry (0, entry);
					}

					exif_ifd = entry.Structure;
				}

				return exif_ifd;
			}
		}

		/// <summary>
		///    The GPS IFD. Will create one if the file doesn't alread have it.
		/// </summary>
		/// <remarks>
		///    <para>Note how this also creates an empty IFD for GPS, even if
		///    you don't set a value. That's okay, empty nested IFDs get ignored
		///    when rendering.</para>
		/// </remarks>
		public IFDStructure GPSIFD {
			get {
				if (gps_ifd == null) {
					var entry = Structure.GetEntry (0, IFDEntryTag.GPSIFD) as SubIFDEntry;
					if (entry == null) {
						gps_ifd = new IFDStructure ();
						entry = new SubIFDEntry ((ushort) IFDEntryTag.GPSIFD, (ushort) IFDEntryType.Long, 1, gps_ifd, SubIFDType.GPS);
						Structure.SetEntry (0, entry);
					}

					gps_ifd = entry.Structure;
				}

				return gps_ifd;
			}
		}

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.TiffIFD" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.TiffIFD; }
		}

#endregion

#region Public Methods

		/// <summary>
		///    Clears the values stored in the current instance.
		/// </summary>
		public override void Clear ()
		{
			throw new NotImplementedException ();
		}

#endregion

#region Metadata fields

		/// <summary>
		///    Gets or sets the comment for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the comment of the
		///    current instace.
		/// </value>
		public override string Comment {
			get {
				var comment_entry =
					ExifIFD.GetEntry (0, (ushort) ExifEntryTag.UserComment) as UndefinedIFDEntry;

				if (comment_entry == null) {
					var description = Structure.GetEntry (0, IFDEntryTag.ImageDescription) as StringIFDEntry;
					return description == null ? null : description.Value;
				}

				ByteVector data = comment_entry.Data;

				if (data.StartsWith (COMMENT_ASCII_CODE))
					return data.ToString (StringType.Latin1, COMMENT_ASCII_CODE.Count, data.Count - COMMENT_ASCII_CODE.Count);

				if (data.StartsWith (COMMENT_UNICODE_CODE))
					return data.ToString (StringType.UTF8, COMMENT_UNICODE_CODE.Count, data.Count - COMMENT_UNICODE_CODE.Count);

				// Some programs like e.g. CanonZoomBrowser inserts just the first 0x00-byte
				// followed by 7-bytes of trash.
				if (data.StartsWith ((byte) 0x00) && data.Count >= 8) {

					// And CanonZoomBrowser fills some trailing bytes of the comment field
					// with '\0'. So we return only the characters before the first '\0'.
					int term = data.Find ("\0", 8);
					if (term != -1)
						return data.ToString (StringType.Latin1, 8, term - 8);

					return data.ToString (StringType.Latin1, 8, data.Count - 8);
				}

				throw new NotImplementedException ("UserComment with other encoding than Latin1 or Unicode");
			}
			set {
				ByteVector data = new ByteVector ();
				data.Add (COMMENT_UNICODE_CODE);
				data.Add (ByteVector.FromString (value, StringType.UTF8));

				ExifIFD.SetEntry (0, new UndefinedIFDEntry ((ushort)ExifEntryTag.UserComment, data));
				Structure.SetEntry (0, new StringIFDEntry ((ushort)IFDEntryTag.ImageDescription, value));
			}
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
		///    The time of capturing.
		/// </summary>
		/// <value>
		///    A <see cref="DateTime" /> with the time of capturing.
		/// </value>
		public DateTime DateTimeOriginal {
			get {
				return ExifIFD.GetDateTimeValue (0, (ushort) ExifEntryTag.DateTimeOriginal);
			}
			set {
				ExifIFD.SetDateTimeValue (0, (ushort) ExifEntryTag.DateTimeOriginal, value);
			}
		}


		/// <summary>
		///    The time of digitization.
		/// </summary>
		/// <value>
		///    A <see cref="DateTime" /> with the time of digitization.
		/// </value>
		public DateTime DateTimeDigitized {
			get {
				return ExifIFD.GetDateTimeValue (0, (ushort) ExifEntryTag.DateTimeDigitized);
			}
			set {
				ExifIFD.SetDateTimeValue (0, (ushort) ExifEntryTag.DateTimeDigitized, value);
			}
		}

		/// <summary>
		///    Gets or sets the latitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the latitude ranging from -90.0
		///    to +90.0 degrees.
		/// </value>
		public override double Latitude {
			get {
				var gps_ifd = GPSIFD;
				var degree_entry = gps_ifd.GetEntry (0, (ushort) GPSEntryTag.GPSLatitude) as RationalArrayIFDEntry;
				var ref_entry = gps_ifd.GetEntry (0, (ushort) GPSEntryTag.GPSLatitudeRef) as StringIFDEntry;

				if (degree_entry == null || ref_entry == null)
					return 0.0d;

				Rational [] values  = degree_entry.Values;
				double deg = values[0] + values[1] / 60.0d + values[2] / 3600.0d;

				if (ref_entry.Value == "S")
					deg *= -1.0d;

				return Math.Max (Math.Min (deg, 90.0d), -90.0d);
			}
			set {
				if (value < -90.0d || value > 90.0d)
					throw new ArgumentException ("value");

				InitGpsDirectory ();
				var gps_ifd = GPSIFD;

				gps_ifd.SetStringValue (0, (ushort) GPSEntryTag.GPSLatitudeRef, value < 0 ? "S" : "N");

				double abs = Math.Abs (value);
				uint deg = (uint) Math.Floor (abs);
				uint min = (uint) ((abs - Math.Floor (abs)) * 60.0);
				uint sec = (uint) ((abs - Math.Floor (abs) - (min / 60.0))  * 360000000.0);

				Rational[] rationals = new Rational [] {
					new Rational (deg, 1),
					new Rational (min, 1),
					new Rational (sec, 100000)
				};

				gps_ifd.SetEntry (0, new RationalArrayIFDEntry ((ushort) GPSEntryTag.GPSLatitude, rationals));
			}
		}

		/// <summary>
		///    Gets or sets the longitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the longitude ranging from 0
		///    to +180.0 degrees.
		/// </value>
		public override double Longitude {
			get {
				var gps_ifd = GPSIFD;
				var degree_entry = gps_ifd.GetEntry (0, (ushort) GPSEntryTag.GPSLongitude) as RationalArrayIFDEntry;
				var ref_entry = gps_ifd.GetEntry (0, (ushort) GPSEntryTag.GPSLongitudeRef) as StringIFDEntry;

				if (degree_entry == null || ref_entry == null)
					return 0.0d;

				Rational [] values  = degree_entry.Values;
				double deg = values[0] + values[1] / 60.0d + values[2] / 3600.0d;

				if (ref_entry.Value == "W")
					deg = 180.0d + (-1.0d * deg);

				return Math.Max (Math.Min (deg, 180.0d), 0.0d);
			}
			set {
				if (value < 0.0d || value > 180.0d)
					throw new ArgumentException ("value");

				InitGpsDirectory ();
				var gps_ifd = GPSIFD;

				if (value >= 90.0d)
					value = value - 180.0d;

				gps_ifd.SetStringValue (0, (ushort) GPSEntryTag.GPSLongitudeRef, value < 0 ? "W" : "E");

				double abs = Math.Abs (value);
				uint deg = (uint) Math.Floor (abs);
				uint min = (uint) ((abs - Math.Floor (abs)) * 60.0);
				uint sec = (uint) ((abs - Math.Floor (abs) - (min / 60.0))  * 360000000.0);

				Rational[] rationals = new Rational [] {
					new Rational (deg, 1),
					new Rational (min, 1),
					new Rational (sec, 100000)
				};

				gps_ifd.SetEntry (0, new RationalArrayIFDEntry ((ushort) GPSEntryTag.GPSLongitude, rationals));
			}
		}

		/// <summary>
		///    Gets or sets the altitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="double" /> with the altitude ranging from -90.0
		///    to +90.0 degrees.
		/// </value>
		public override double Altitude {
			get { return 0.0d; }
			set {}
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
				return ExifIFD.GetRationalValue (0, (ushort) ExifEntryTag.ExposureTime);
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
				return ExifIFD.GetRationalValue (0, (ushort) ExifEntryTag.FNumber);
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
				return ExifIFD.GetLongValue (0, (ushort) ExifEntryTag.ISOSpeedRatings);
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
				return ExifIFD.GetRationalValue (0, (ushort) ExifEntryTag.FocalLength);
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
				return Structure.GetStringValue (0, (ushort) IFDEntryTag.Make);
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
				return Structure.GetStringValue (0, (ushort) IFDEntryTag.Model);
			}
		}

#endregion

#region Private Methods

		/// <summary>
		///    Initilazies the GPS IFD with some basic entries.
		/// </summary>
		private void InitGpsDirectory ()
		{
			GPSIFD.SetStringValue (0, (ushort) GPSEntryTag.GPSVersionID, "2 0 0 0");
			GPSIFD.SetStringValue (0, (ushort) GPSEntryTag.GPSMapDatum, "WGS-84");
		}

#endregion

	}
}
