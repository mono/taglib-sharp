//
// Picture.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
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

namespace TagLib.Flac
{
	/// <summary>
	///    This class implements <see cref="IPicture" /> to provide support
	///    for reading and writing Flac picture metadata.
	/// </summary>
	public class Picture : IPicture
	{

		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> by reading the contents of a raw Flac
		///    image structure.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the raw
		///    Flac image.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    <paramref name="data" /> contains less than 32 bytes.
		/// </exception>
		public Picture (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			if (data.Count < 32)
				throw new CorruptFileException ("Data must be at least 32 bytes long");

			int pos = 0;
			Type = (PictureType)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			int mimetype_length = (int)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			MimeType = data.ToString (StringType.Latin1, pos,
				mimetype_length);
			pos += mimetype_length;

			int description_length = (int)data.Mid (pos, 4)
				.ToUInt ();
			pos += 4;

			Description = data.ToString (StringType.UTF8, pos,
				description_length);
			pos += description_length;

			Width = (int)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			Height = (int)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			ColorDepth = (int)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			IndexedColors = (int)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			int data_length = (int)data.Mid (pos, 4).ToUInt ();
			pos += 4;

			Data = data.Mid (pos, data_length);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Picture" /> by copying the properties of a <see
		///    cref="IPicture" /> object.
		/// </summary>
		/// <param name="picture">
		///    A <see cref="IPicture" /> object to use for the new
		///    instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="picture" /> is <see langword="null" />.
		/// </exception>
		public Picture (IPicture picture)
		{
			if (picture == null)
				throw new ArgumentNullException (nameof (picture));

			Type = picture.Type;
			MimeType = picture.MimeType;
			Filename = picture.Filename;
			Description = picture.Description;
			Data = picture.Data;

			if (!(picture is Picture flac_picture))
				return;

			Width = flac_picture.Width;
			Height = flac_picture.Height;
			ColorDepth = flac_picture.ColorDepth;
			IndexedColors = flac_picture.IndexedColors;
		}

		#endregion



		#region Public Methods

		/// <summary>
		///    Renders the current instance as a raw Flac picture.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		public ByteVector Render ()
		{
			var data = new ByteVector {
				ByteVector.FromUInt ((uint)Type)
			};

			var mime_data = ByteVector.FromString (MimeType,
				StringType.Latin1);
			data.Add (ByteVector.FromUInt ((uint)mime_data.Count));
			data.Add (mime_data);

			var decription_data = ByteVector.FromString (Description, StringType.UTF8);
			data.Add (ByteVector.FromUInt ((uint)decription_data.Count));
			data.Add (decription_data);

			data.Add (ByteVector.FromUInt ((uint)Width));
			data.Add (ByteVector.FromUInt ((uint)Height));
			data.Add (ByteVector.FromUInt ((uint)ColorDepth));
			data.Add (ByteVector.FromUInt ((uint)IndexedColors));

			data.Add (ByteVector.FromUInt ((uint)Data.Count));
			data.Add (Data);

			return data;
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
		public string MimeType { get; set; }

		/// <summary>
		///    Gets and sets the type of content visible in the picture
		///    stored in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="PictureType" /> containing the type of
		///    content visible in the picture stored in the current
		///    instance.
		/// </value>
		public PictureType Type { get; set; }

		/// <summary>
		///    Gets and sets a filename of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing a fielname, with
		///    extension, of the picture stored in the current instance.
		/// </value>
		public string Filename { get; set; }

		/// <summary>
		///    Gets and sets a description of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing a description
		///    of the picture stored in the current instance.
		/// </value>
		public string Description { get; set; }

		/// <summary>
		///    Gets and sets the picture data stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the picture
		///    data stored in the current instance.
		/// </value>
		public ByteVector Data { get; set; }

		/// <summary>
		///    Gets and sets the width of the picture in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing width of the
		///    picture stored in the current instance.
		/// </value>
		public int Width { get; set; }

		/// <summary>
		///    Gets and sets the height of the picture in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing height of the
		///    picture stored in the current instance.
		/// </value>
		public int Height { get; set; }

		/// <summary>
		///    Gets and sets the color depth of the picture in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing color depth of the
		///    picture stored in the current instance.
		/// </value>
		public int ColorDepth { get; set; }

		/// <summary>
		///    Gets and sets the number of indexed colors in the picture
		///    in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing number of indexed
		///    colors in the picture, or zero if the picture is not
		///    stored in an indexed format.
		/// </value>
		public int IndexedColors { get; set; }

		#endregion
	}
}
