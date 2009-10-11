//
// IFDEntryTag.cs:
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

namespace TagLib.IFD
{
	/// <summary>
	///    In TIFF, each entry has an associated key (the tag), which indicates
	///    the meaning of the value. This can be any of the following, but
	///    other values exist as well (mostly in proprietary RAW formats).
	/// </summary>
	/// <remarks>
	///    These mostly come from the TIFF specification:
	///    http://partners.adobe.com/public/developer/en/tiff/TIFF6.pdf
	///    And the excellent TIFF documentation at awaresystems.be:
	///    http://www.awaresystems.be/imaging/tiff.html
	///
	//     Not all values are present yet, they can be added when needed.
	/// </remarks>
	public enum IFDEntryTag : ushort
	{
		/// <summary>
		///     A general indication of the kind of data contained in this subfile. (Hex: 0x00FE)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/newsubfiletype.html
		/// </summary>
		NewSubFileType                                 = 254,

		/// <summary>
		///     A general indication of the kind of data contained in this subfile. (Hex: 0x00FF)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/subfiletype.html
		/// </summary>
		SubFileType                                    = 255,

		/// <summary>
		///     The number of columns in the image, i.e., the number of pixels per row. (Hex: 0x0100)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/imagewidth.html
		/// </summary>
		ImageWidth                                     = 256,

		/// <summary>
		///     The number of rows of pixels in the image (Hex: 0x0101)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/imagelength.html
		/// </summary>
		ImageLength                                    = 257,

		/// <summary>
		///     Number of bits per component. (Hex: 0x0102)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/bitspersample.html
		/// </summary>
		BitsPerSample                                  = 258,

		/// <summary>
		///     Compression scheme used on the image data. (Hex: 0x0103)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/compression.html
		/// </summary>
		Compression                                    = 259,

		/// <summary>
		///     The color space of the image data. (Hex: 0x0106)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/photometricinterpretation.html
		/// </summary>
		PhotoMetricInterpretation                      = 262,

		/// <summary>
		///     The name of the document from which this image was scanned. (Hex: 0x010D)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/documentname.html
		/// </summary>
		DocumentName                                   = 269,

		/// <summary>
		///     A string that describes the subject of the image. (Hex: 0x010E)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/imagedescription.html
		/// </summary>
		ImageDescription                               = 270,

		/// <summary>
		///     The scanner manufacturer. (Hex: 0x010F)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/make.html
		/// </summary>
		Make                                           = 271,

		/// <summary>
		///     The scanner model name or number. (Hex: 0x0110)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/model.html
		/// </summary>
		Model                                          = 272,

		/// <summary>
		///     For each strip, the byte offset of that strip. (Hex: 0x0111)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/stripoffsets.html
		/// </summary>
		StripOffsets                                   = 273,

		/// <summary>
		///     The orientation of the image with respect to the rows and columns. (Hex: 0x0112)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/orientation.html
		/// </summary>
		Orientation                                    = 274,

		/// <summary>
		///     The number of components per pixel. (Hex: 0x0115)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/samplesperpixel.html
		/// </summary>
		SamplesPerPixel                                = 277,

		/// <summary>
		///     The number of rows per strip. (Hex: 0x0116)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/rowsperstrip.html
		/// </summary>
		RowsPerStrip                                   = 278,

		/// <summary>
		///     For each strip, the number of bytes in the strip after compression. (Hex: 0x0117)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/stripbytecounts.html
		/// </summary>
		StripByteCounts                                = 279,

		/// <summary>
		///     The number of pixels per ResolutionUnit in the ImageWidth direction. (Hex: 0x011A)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/xresolution.html
		/// </summary>
		XResolution                                    = 282,

		/// <summary>
		///     The number of pixels per ResolutionUnit in the ImageLength direction. (Hex: 0x011B)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/yresolution.html
		/// </summary>
		YResolution                                    = 283,

		/// <summary>
		///     How the components of each pixel are stored. (Hex: 0x011C)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/planarconfiguration.html
		/// </summary>
		PlanarConfiguration                            = 284,

		/// <summary>
		///     The unit of measurement for XResolution and YResolution. (Hex: 0x0128)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/resolutionunit.html
		/// </summary>
		ResolutionUnit                                 = 296,

		/// <summary>
		///     Name and version number of the software package(s) used to create the image. (Hex: 0x0131)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/software.html
		/// </summary>
		Software                                       = 305,

		/// <summary>
		///     Date and time of image creation. (Hex: 0x0132)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/datetime.html
		/// </summary>
		DateTime                                       = 306,

		/// <summary>
		///     A mathematical operator that is applied to the image data before an encoding scheme is applied. (Hex: 0x013D)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/predictor.html
		/// </summary>
		Predictor                                      = 317,

		/// <summary>
		///    If a JPEG interchange format bitstream is present, then this field should point to the Start of Image
		///    (SOI) marker code. (Hex: 0x0201)
		///    http://www.awaresystems.be/imaging/tiff/tifftags/jpeginterchangeformat.html
		/// </summary>
		JPEGInterchangeFormat                          = 513,

		/// <summary>
		///    This field was originally intended to indicate the length of the JPEG stream pointed to by
		///    JPEGInterchangeFormat tag. (Hex: 0x0202)
		///    http://www.awaresystems.be/imaging/tiff/tifftags/jpeginterchangeformatlength.html
		/// </summary>
		JPEGInterchangeFormatLength                    = 514,

		/// <summary>
		///     XML packet containing XMP metadata. (Hex: 0x02BC)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/xmp.html
		/// </summary>
		XMP                                            = 700,

		/// <summary>
		///     Exposure Time, the image was taken with. (Hex: 0x829A)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/exposuretime.html
		/// </summary>
		ExposureTime                                   = 33434,

		/// <summary>
		///     F Number, the image was taken with. (Hex: 0x829D)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/fnumber.html
		/// </summary>
		FNumber                                        = 33437,

		/// <summary>
		///     ISO Speed, the image was taken with. (Hex: 0x8827)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/isospeedratings.html
		/// </summary>
		ISOSpeedRatings                                = 34855,

		/// <summary>
		///     IPTC (International Press Telecommunications Council) metadata. (Hex: 0x83BB)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/iptc.html
		/// </summary>
		IPTC                                           = 33723,

		/// <summary>
		///     A pointer to the Exif IFD. (Hex: 0x8769)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/exififd.html
		/// </summary>
		ExifIFD                                        = 34665,

		/// <summary>
		///     A pointer to the GPS IFD. (Hex: 0x8825)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/gpsifd.html
		/// </summary>
		GPSIFD                                         = 34853,

		/// <summary>
		///     A pointer to the MakerNote data. Used to record any desired information.
		///     The contents are up to the manufacturer, but this tag should not be used
		///     for any other than its intended purpose. (Hex: 0x927C)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/makernote.html
		/// </summary>
		MakerNoteIFD                                   = 37500,

		/// <summary>
		///     User Comment (Hex: 0x9286)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/gpsifd.html
		/// </summary>
		UserComment                                    = 37510,

		/// <summary>
		///    When a compressed file is recorded, the valid width of the meaningful image shall be
		///    recorded in this tag. (Hex: 0xA002)
		///    http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/pixelxdimension.html
		/// </summary>
		PixelXDimension                                = 40962,

		/// <summary>
		///    When a compressed file is recorded, the valid height of the meaningful image shall be
		///    recorded in this tag. (Hex: 0xA003)
		///    http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/pixelydimension.html
		/// </summary>
		PixelYDimension                                = 40963,

		/// <summary>
		///     A pointer to the Interoperability IFD. (Hex: 0xA005)
		///     http://www.awaresystems.be/imaging/tiff/tifftags/interoperabilityifd.html
		/// </summary>
		IopIFD                                         = 40965,

	}
}
