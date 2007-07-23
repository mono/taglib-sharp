//
// Picture.cs: Provides IPicture and Picture.
//
// Author:
//   Aaron Bockover (abockover@novell.com)
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2attachedpictureframe.cpp from TagLib
//
// Copyright (C) 2006 Novell, Inc.
// Copyright (C) 2007 Brian Nickel
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
		///    The picture is the logo of the publisher or record.
		///    company.
		/// </summary>
		PublisherLogo = 0x14
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
		///    Contains the description.
		/// </summary>
		private string description;
		
		/// <summary>
		///    Contains the picture data.
		/// </summary>
		private ByteVector data;
		
		#endregion
		
		
		
		#region Public Static Methods
		
		/// <summary>
		///    Creates a new <see cref="Picture" />, populating it with
		///    the contents of a file.
		/// </summary>
		/// <param name="path">
		///    A <see cref="string" /> object containing the path to a
		///    file to read the picture from.
		/// </param>
		/// <returns>
		///    A new <see cref="Picture" /> object containing the
		///    contents of the file and with a mime-type guessed from
		///    the file's contents.
		/// </returns>
		public static Picture CreateFromPath (string path)
		{
			return CreateFromFile (
				new File.LocalFileAbstraction (path));
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
		public static Picture CreateFromFile (File.IFileAbstraction abstraction)
		{
			byte [] fc;
			string filename = null;
			string mimetype = "image/jpeg";
			string ext = "jpg";
			
			Picture picture = new Picture();
			picture.Data = ByteVector.FromFile (abstraction, out fc,
				true);
			if (fc.Length >= 4 &&
				(fc[1] == 'P' &&
				 fc[2] == 'N' &&
				 fc[3] == 'G')) {
				mimetype = "image/png";
				ext = "png";
			}
			
			picture.MimeType = mimetype;
			picture.Type = PictureType.FrontCover;
			picture.Description = filename == null ?
				("cover." + ext) : mimetype;
			
			return picture;
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
	}
}
