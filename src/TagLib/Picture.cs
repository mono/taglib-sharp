//
// Picture.cs: Provides IPicture and Picture.
//
// Author:
//   Aaron Bockover (abockover@novell.com)
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   attachedpictureframe.cpp from TagLib
//
// Copyright (C) 2006 Novell, Inc.
// Copyright (C) 2007 Brian Nickel
// Copyright (C) 2004 Scott Wheeler (Original Implementation)
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

namespace TagLib {
	/// <summary>
	///    Specifies the type of content appearing in the picture.
	/// </summary>
	public enum PictureType
	{
		/// <summary>
		///    The picture is of a type other than those specified.
		/// </summary>
		Other = 0x00,
		
		/// <summary>
		///    The picture is a 32x32 PNG image that should be used when
		///    displaying the file in a browser.
		/// </summary>
		FileIcon = 0x01,
		
		/// <summary>
		///    The picture is of an icon different from <see
		///    cref="FileIcon" />.
		/// </summary>
		OtherFileIcon = 0x02,
		
		/// <summary>
		///    The picture is of the front cover of the album.
		/// </summary>
		FrontCover = 0x03,
		
		/// <summary>
		///    The picture is of the back cover of the album.
		/// </summary>
		BackCover = 0x04,
		
		/// <summary>
		///    The picture is of a leaflet page including with the
		///    album.
		/// </summary>
		LeafletPage = 0x05,
		
		/// <summary>
		///    The picture is of the album or disc itself.
		/// </summary>
		Media = 0x06,
		// Image from the album itself
		
		/// <summary>
		///    The picture is of the lead artist or soloist.
		/// </summary>
		LeadArtist = 0x07,
		
		/// <summary>
		///    The picture is of the artist or performer.
		/// </summary>
		Artist = 0x08,
		
		/// <summary>
		///    The picture is of the conductor.
		/// </summary>
		Conductor = 0x09,
		
		/// <summary>
		///    The picture is of the band or orchestra.
		/// </summary>
		Band = 0x0A,
		
		/// <summary>
		///    The picture is of the composer.
		/// </summary>
		Composer = 0x0B,
		
		/// <summary>
		///    The picture is of the lyricist or text writer.
		/// </summary>
		Lyricist = 0x0C,
		
		/// <summary>
		///    The picture is of the recording location or studio.
		/// </summary>
		RecordingLocation = 0x0D,
		
		/// <summary>
		///    The picture is one taken during the track's recording.
		/// </summary>
		DuringRecording = 0x0E,
		
		/// <summary>
		///    The picture is one taken during the track's performance.
		/// </summary>
		DuringPerformance = 0x0F,
		
		/// <summary>
		///    The picture is a capture from a movie screen.
		/// </summary>
		MovieScreenCapture = 0x10,
		
		/// <summary>
		///    The picture is of a large, colored fish.
		/// </summary>
		ColoredFish = 0x11,
		
		/// <summary>
		///    The picture is an illustration related to the track.
		/// </summary>
		Illustration = 0x12,
		
		/// <summary>
		///    The picture contains the logo of the band or performer.
		/// </summary>
		BandLogo = 0x13,
		
		/// <summary>
		///    The picture is the logo of the publisher or record
		///    company.
		/// </summary>
		PublisherLogo = 0x14,


		/// <summary>
		///    In fact, this is not a Picture, but another file-type.
		/// </summary>
		NotAPicture = 0xff

	}

	/// <summary>
	///    This interface provides generic information about a picture,
	///    including its contents, as used by various formats.
	/// </summary>
	public interface IPicture
	{
		/// <summary>
		///    Gets and sets the mime-type of the picture data
		///    stored in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the mime-type
		///    of the picture data stored in the current instance.
		/// </value>
		string MimeType {get; set;}
		
		/// <summary>
		///    Gets and sets the type of content visible in the picture
		///    stored in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="PictureType" /> containing the type of
		///    content visible in the picture stored in the current
		///    instance.
		/// </value>
		PictureType Type {get; set;}



		/// <summary>
		///    Gets and sets a filename of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the filename,
		///    with its extension, of the picture stored in the current 
		///    instance.
		/// </value>
		string Filename { get; set; }


		/// <summary>
		///    Gets and sets a description of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing a description
		///    of the picture stored in the current instance.
		/// </value>
		string Description {get; set;}
		
		/// <summary>
		///    Gets and sets the picture data stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the picture
		///    data stored in the current instance.
		/// </value>
		ByteVector Data {get; set;}
	}
	
	/// <summary>
	///    This class implements <see cref="IPicture" /> and provides
	///    mechanisms for loading pictures from files.
	/// </summary>
	public class Picture : IPicture
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the mime-type.
		/// </summary>
		private string mime_type;
		
		/// <summary>
		///    Contains the content type.
		/// </summary>
		private PictureType type;

		/// <summary>
		///    Contains the filename.
		/// </summary>
		private string filename;

		/// <summary>
		///    Contains the description.
		/// </summary>
		private string description;
		
		/// <summary>
		///    Contains the picture data.
		/// </summary>
		private ByteVector data;
		
		#endregion
		
		
		
		#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> with no data or values.
		/// </summary>
		public Picture ()
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> by reading in the contents of a
		///    specified file.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string"/> object containing the path of the
		///    file to read.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public Picture (string path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");
			
			Data = ByteVector.FromPath (path);
			filename = System.IO.Path.GetFileName(path);
			description = filename;
			FillInMimeFromExt();
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> by reading in the contents of a
		///    specified file abstraction.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="File.IFileAbstraction"/> object containing
		///    abstraction of the file to read.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="abstraction" /> is <see langword="null"
		///    />.
		/// </exception>
		public Picture (File.IFileAbstraction abstraction)
		{
			if (abstraction == null)
				throw new ArgumentNullException ("abstraction");
			
			Data = ByteVector.FromFile (abstraction);
			filename = abstraction.Name;
			description = abstraction.Name;

			if (!string.IsNullOrEmpty(filename) && filename.Contains("."))
			{
				FillInMimeFromExt();
			}
			else
			{
				FillInMimeFromData();
			}
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> by using the contents of a <see
		///    cref="ByteVector" /> object.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector"/> object containing picture data
		///    to use.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		public Picture (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			Data = new ByteVector (data);
			FillInMimeFromData ();
		}


		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> by doing a shallow copy of <see 
		///    cref="IPicture" />.
		/// </summary>
		/// <param name="picture">
		///    A <see cref="IPicture"/> object containing picture data
		///    to convert to an Picture.
		/// </param>
		public Picture(IPicture picture)
		{
			mime_type = picture.MimeType;
			type = picture.Type;
			filename = picture.Filename;
			description = picture.Description;
			data = picture.Data;
		}



		#endregion



		#region Public Static Methods

		/// <summary>
		///    Creates a new <see cref="Picture" />, populating it with
		///    the contents of a file.
		/// </summary>
		/// <param name="filename">
		///    A <see cref="string" /> object containing the path to a
		///    file to read the picture from.
		/// </param>
		/// <returns>
		///    A new <see cref="Picture" /> object containing the
		///    contents of the file and with a mime-type guessed from
		///    the file's contents.
		/// </returns>
		[Obsolete("Use Picture(string filename) constructor instead.")]
		public static Picture CreateFromPath (string filename)
		{
			return new Picture (filename);
		}
		
		/// <summary>
		///    Creates a new <see cref="Picture" />, populating it with
		///    the contents of a file.
		/// </summary>
		/// <param name="abstraction">
		///    A <see cref="File.IFileAbstraction" /> object containing
		///    the file abstraction to read the picture from.
		/// </param>
		/// <returns>
		///    A new <see cref="Picture" /> object containing the
		///    contents of the file and with a mime-type guessed from
		///    the file's contents.
		/// </returns>
		[Obsolete("Use Picture(File.IFileAbstraction abstraction) constructor instead.")]
		public static Picture CreateFromFile (File.IFileAbstraction abstraction)
		{
			return new Picture (abstraction);
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		/// <summary>
		///    Gets and sets the mime-type of the picture data
		///    stored in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the mime-type
		///    of the picture data stored in the current instance.
		/// </value>
		public string MimeType {
			get { return mime_type; }
			set { mime_type = value; }
		}
		
		/// <summary>
		///    Gets and sets the type of content visible in the picture
		///    stored in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="PictureType" /> containing the type of
		///    content visible in the picture stored in the current
		///    instance.
		/// </value>
		public PictureType Type {
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		///    Gets and sets a filename of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing a fielname, with
		///    extension, of the picture stored in the current instance.
		/// </value>
		public string Filename
		{
			get { return filename; }
			set { filename = value; }
		}

		/// <summary>
		///    Gets and sets a description of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing a description
		///    of the picture stored in the current instance.
		/// </value>
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		/// <summary>
		///    Gets and sets the picture data stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the picture
		///    data stored in the current instance.
		/// </value>
		public ByteVector Data {
			get { return data; }
			set { data = value; }
		}
		
		#endregion
		
		
		
		#region Private Methods
		
		/// <summary>
		///    Fills in the mime type of the current instance by reading
		///    the first few bytes of the file. If the format cannot be
		///    identified, it assumed to be a Binary file.
		/// </summary>
		private void FillInMimeFromData ()
		{
			string mimetype = null;
			string ext = null;

			// No picture, unless it is corrupted, can fit in a file of less than 4 bytes
			if (Data.Count >= 4)
			{
				if (Data[1] == 'P' && Data[2] == 'N' && Data[3] == 'G')
				{
					mimetype = "image/png";
					ext = "png";
				}
				else if (Data[0] == 'G' && Data[1] == 'I' && Data[2] == 'F')
				{
					mimetype = "image/gif";
					ext = "gif";
				}
				else if (Data[0] == 'B' && Data[1] == 'M')
				{
					mimetype = "image/bmp";
					ext = "bmp";
				}
				else if (Data[0] == 0xFF && Data[1] == 0xD8 && Data[2] == 0xFF && Data[3] == 0xE0 )
				{
					mimetype = "image/jpeg";
					ext = "jpg";
				}

			}

			if (ext != null)
			{
				MimeType = mimetype;
				type = PictureType.FrontCover;
				filename = description = "cover." + ext;
			}
			else
			{
				// Default
				mimetype = "application/octet-stream";
				type = PictureType.NotAPicture;
				filename = "UnknownType";
			}
		}

		/// <summary>
		///    Fills in the mime type of the current instance by reading
		///    its file-extension. More accurate than <see cref="FillInMimeFromData"/>.
		///    If the format cannot be identified, it assumed to be a Binary file.
		/// </summary>
		private void FillInMimeFromExt()
		{
			// Default
			mime_type = "application/octet-stream";
			type = PictureType.NotAPicture;

			// Get extension from Filename
			if (string.IsNullOrEmpty(Filename)) return;
			var ext = System.IO.Path.GetExtension(filename);
			if (string.IsNullOrEmpty(ext)) return;

			switch (ext)
			{
				case ".aac": mime_type = "audio/aac"; break; // AAC audio file
				case ".abw": mime_type = "application/x-abiword"; break; // AbiWord document
				case ".arc": mime_type = "application/octet-stream"; break; // Archive document (multiple files embedded)
				case ".avi": mime_type = "video/x-msvideo"; break; // AVI: Audio Video Interleave
				case ".azw": mime_type = "application/vnd.amazon.ebook"; break; // Amazon Kindle eBook format
				case ".bin": mime_type = "application/octet-stream"; break; // Any kind of binary data
				case ".bz": mime_type = "application/x-bzip"; break; // BZip archive
				case ".bz2": mime_type = "application/x-bzip2"; break; // BZip2 archive
				case ".csh": mime_type = "application/x-csh"; break; // C-Shell script
				case ".css": mime_type = "text/css"; break; // Cascading Style Sheets (CSS)
				case ".csv": mime_type = "text/csv"; break; // Comma-separated values (CSV)
				case ".doc": mime_type = "application/msword"; break; // Microsoft Word
				case ".eot": mime_type = "application/vnd.ms-fontobject"; break; // MS Embedded OpenType fonts
				case ".epub": mime_type = "application/epub+zip"; break; // Electronic publication (EPUB)
				case ".gif":  mime_type = "image/gif"; break; // Graphics Interchange Format (GIF)
				case ".htm":
				case ".html": mime_type = "text/html"; break; // HyperText Markup Language (HTML)text / html
				case ".ico": mime_type = "image/x-icon"; break; // Icon format
				case ".ics": mime_type = "text/calendar"; break; // iCalendar format
				case ".jar": mime_type = "application/java-archive"; break; // Java Archive (JAR)
				case ".jpeg":
				case ".jpg": mime_type = "image/jpeg"; break; // JPEG images
				case ".js": mime_type = "application/javascript"; break; // JavaScript (ECMAScript)
				case ".json": mime_type = "application/json"; break; // JSON format
				case ".mid":
				case ".midi": mime_type = "audio/midi"; break; // Musical Instrument Digital Interface (MIDI)
				case ".mp1":
				case ".mp2":
				case ".mp3":
				case ".mpg": mime_type = "audio/mpeg"; break;
				case ".mpeg": mime_type = "video/mpeg"; break; // MPEG Video
				case ".m4a": mime_type = "audio/mp4"; break;
				case ".mp4":
				case ".m4v": mime_type = "video/mp4"; break;
				case ".mpkg": mime_type = "application/vnd.apple.installer+xml"; break; // Apple Installer Package
				case ".odp": mime_type = "application/vnd.oasis.opendocument.presentation"; break; // OpenDocuemnt presentation document
				case ".ods": mime_type = "application/vnd.oasis.opendocument.spreadsheet"; break; // OpenDocuemnt spreadsheet document
				case ".odt": mime_type = "application/vnd.oasis.opendocument.text"; break; // OpenDocument text document
				case ".oga": mime_type = "audio/ogg"; break; // OGG audio
				case ".ogg": mime_type = "audio/ogg"; break;
				case ".ogx": mime_type = "application/ogg"; break; // OGG
				case ".ogv": mime_type = "video/ogg"; break;
				case ".otf": mime_type = "font/otf"; break; // OpenType font
				case ".png": mime_type = "image/png"; break; // Portable Network Graphics
				case ".pdf": mime_type = "application/pdf"; break; // Adobe Portable Document Format (PDF)
				case ".ppt": mime_type = "application/vnd.ms-powerpoint"; break; // Microsoft PowerPoint
				case ".rar": mime_type = "application/x-rar-compressed"; break; // RAR archive
				case ".rtf": mime_type = "application/rtf"; break; // Rich Text Format (RTF)
				case ".sh": mime_type = "application/x-sh"; break; // Bourne shell script
				case ".svg": mime_type = "image/svg+xml"; break; // Scalable Vector Graphics (SVG)
				case ".swf": mime_type = "application/x-shockwave-flash"; break; // Small web format (SWF) or Adobe Flash document
				case ".tar": mime_type = "application/x-tar"; break; // Tape Archive (TAR)
				case ".tif":
				case ".tiff": mime_type = "image/tiff"; break; //  Tagged Image File Format(TIFF)
				case ".ts": mime_type = "video/vnd.dlna.mpeg-tts"; break; // Typescript file
				case ".ttf": mime_type = "font/ttf"; break; // TrueType Font
				case ".vsd": mime_type = "application/vnd.visio"; break; // Microsoft Visio
				case ".wav": mime_type = "audio/x-wav"; break; // Waveform Audio Format
				case ".weba": mime_type = "audio/webm"; break; // WEBM audio
				case ".webm": mime_type = "video/webm"; break; // WEBM video
				case ".webp": mime_type = "image/webp"; break; // WEBP image
				case ".woff": mime_type = "font/woff"; break; // Web Open Font Format (WOFF)
				case ".woff2": mime_type = "font/woff2"; break; // Web Open Font Format (WOFF)
				case ".xhtml": mime_type = "application/xhtml+xml"; break; // XHTML
				case ".xls": mime_type = "application/vnd.ms"; break; // excel application
				case ".xlsx": mime_type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break; // excel 2007 application
				case ".xml": mime_type = "application/xml"; break; // XML
				case ".xul": mime_type = "application/vnd.mozilla.xul+xml"; break; // XUL
				case ".zip": mime_type = "application/zip"; break; // ZIP archive
				case ".3gp": mime_type = "video/3gpp"; break; // 3GPP audio/video container
				case "audio/3gpp": mime_type = "video"; break; // if it doesn't contain
				case ".3g2": mime_type = "video/3gpp2"; break; // 3GPP2 audio/video container
				case "audio/3gpp2": mime_type = "video"; break; // if it doesn't contain
				case ".7z": mime_type = "application/x-7z-compressed"; break; // 7-zip archive
			}

			if(mime_type.StartsWith("image/")) type = PictureType.FrontCover;

		}



		#endregion
	}
}
