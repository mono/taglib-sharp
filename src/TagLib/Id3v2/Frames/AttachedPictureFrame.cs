//
// AttachedPictureFrame.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   attachedpictureframe.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
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

using System.Collections;
using System;

using TagLib;

namespace TagLib.Id3v2 {
	/// <summary>
	///    This class extends <see cref="Frame" />, implementing support for
	///    ID3v2 Attached Picture (APIC) Frames.
	/// </summary>
	/// <remarks>
	///    <para>A <see cref="AttachedPictureFrame" /> is used for storing
	///    pictures that complement the media, including the album cover,
	///    the physical medium, leaflets, file icons, etc. Other file and
	///    object data can be encapulsated via <see
	///    cref="GeneralEncapsulatedObjectFrame" />.</para>
	///    <para>Additionally, <see cref="TagLib.Tag.Pictures" /> provides a
	///    generic way or getting and setting pictures which is preferable
	///    to format specific code.</para>
	/// </remarks>
	public class AttachedPictureFrame : Frame, IPicture
	{
		#region Private Properties
		
		/// <summary>
		///    Contains the text encoding to use when rendering.
		/// </summary>
		private StringType text_encoding = Tag.DefaultEncoding;
		
		/// <summary>
		///    Contains the mime type of <see cref="data" />.
		/// </summary>
		private string mime_type = null;
		
		/// <summary>
		///    Contains the type of picture.
		/// </summary>
		private PictureType type = PictureType.Other;
		
		/// <summary>
		///    Contains the description.
		/// </summary>
		private string description = null;
		
		/// <summary>
		///    Contains the picture data.
		/// </summary>
		private ByteVector data = null;
		
		/// <summary>
		///    Contains the raw field data of the current instance as
		///    sent to <see cref="ParseFields" /> or <see
		///    langword="null" /> if <see cref="ParseFields" /> has not
		///    been called or <see cref="ParseRawData" /> has been
		///    called.
		/// </summary>
		/// <remarks>
		///    As this frame takes a while to parse and isn't read in
		///    all cases, the raw data is stored here until it is
		///    needed. This speeds up the file read time significantly.
		/// </remarks>
		private ByteVector raw_data = null;
		
		/// <summary>
		///    Contains the ID3v2 version <see cref="raw_data" /> is
		///    stored in.
		/// </summary>
		private byte raw_version = 0;
		
		#endregion
		
		
		
		#region Constructors
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AttachedPictureFrame" /> with no contents and the
		///    default values.
		/// </summary>
		/// <remarks>
		///    <para>When a frame is created, it is not automatically
		///    added to the tag. Consider using <see
		///    cref="Get(Tag,string,PictureType,bool)" /> for more
		///    integrated frame creation.</para>
		///    <para>Additionally, <see cref="TagLib.Tag.Pictures" />
		///    provides a generic way or getting and setting
		///    pictures which is preferable to format specific
		///    code.</para>
		/// </remarks>
		public AttachedPictureFrame () : base (FrameType.APIC, 4)
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AttachedPictureFrame" /> by populating it with
		///    the contents of another <see cref="IPicture" /> object.
		/// </summary>
		/// <param name="picture">
		///    A <see cref="IPicture" /> object containing values to use
		///    in the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="picture" /> is <see langword="null" />.
		/// </exception>
		/// <remarks>
		///    <para>When a frame is created, it is not automatically
		///    added to the tag. Consider using <see
		///    cref="Get(Tag,string,PictureType,bool)" /> for more
		///    integrated frame creation.</para>
		///    <para>Additionally, <see cref="TagLib.Tag.Pictures" />
		///    provides a generic way or getting and setting
		///    pictures which is preferable to format specific
		///    code.</para>
		/// </remarks>
		/// <example>
		///    <para>Add a picture to a file.</para>
		///    <code lang="C#">
		/// using TagLib;
		/// using TagLib.Id3v2;
		///
		/// public static class AddId3v2Picture
		/// {
		/// 	public static void Main (string [] args)
		/// 	{
		/// 		if (args.Length != 2)
		/// 			throw new ApplicationException (
		/// 				"USAGE: AddId3v2Picture.exe AUDIO_FILE PICTURE_FILE");
		///
		/// 		// Create the file. Can throw file to TagLib# exceptions.
		/// 		File file = File.Create (args [0]);
		///
		/// 		// Get or create the ID3v2 tag.
		/// 		TagLib.Id3v2.Tag tag = file.GetTag (TagTypes.Id3v2, true) as TagLib.Id3v2.Tag;
		/// 		if (tag == null)
		/// 			throw new ApplicationException ("File does not support ID3v2 tags.");
		///
		/// 		// Create a picture. Can throw file related exceptions.
		///		TagLib.Picture picture = TagLib.Picture.CreateFromPath (path);
		///
		/// 		// Add a new picture frame to the tag.
		/// 		tag.AddFrame (new AttachedPictureFrame (picture));
		///
		/// 		// Save the file.
		/// 		file.Save ();
		/// 	}
		/// }
		///    </code>
		/// </example>
		public AttachedPictureFrame (IPicture picture)
			: base(FrameType.APIC, 4)
		{
			if (picture == null)
				throw new ArgumentNullException ("picture");
			
			mime_type   = picture.MimeType;
			type        = picture.Type;
			description = picture.Description;
			data        = picture.Data;
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AttachedPictureFrame" /> by reading its raw data in
		///    a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object starting with the raw
		///    representation of the new frame.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    raw frame is encoded in.
		/// </param>
		public AttachedPictureFrame (ByteVector data, byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="AttachedPictureFrame" /> by reading its raw data
		///    in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the raw
		///    representation of the new frame.
		/// </param>
		/// <param name="offset">
		///    A <see cref="int" /> indicating at what offset in
		///    <paramref name="data" /> the frame actually begins.
		/// </param>
		/// <param name="header">
		///    A <see cref="FrameHeader" /> containing the header of the
		///    frame found at <paramref name="offset" /> in the data.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    raw frame is encoded in.
		/// </param>
		protected internal AttachedPictureFrame (ByteVector data,
		                                         int offset,
		                                         FrameHeader header,
		                                         byte version)
			: base(header)
		{
			SetData (data, offset, version, false);
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		/// <summary>
		///    Gets and sets the text encoding to use when storing the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the text encoding to
		///    use when storing the current instance.
		/// </value>
		/// <remarks>
		///    This encoding is overridden when rendering if <see
		///    cref="Tag.ForceDefaultEncoding" /> is <see
		///    langword="true" /> or the render version does not support
		///    it.
		/// </remarks>
		public StringType TextEncoding {
			get {ParseRawData (); return text_encoding;}
			set {text_encoding = value;}
		}
		
		/// <summary>
		///    Gets and sets the mime-type of the picture stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the mime-type of the
		///    picture stored in the current instance.
		/// </value>
		public string MimeType {
			get {
				ParseRawData ();
				if (mime_type != null)
					return mime_type;
				
				return string.Empty;
			}
			set {mime_type = value;}
		}
		
		/// <summary>
		///    Gets and sets the picture type stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the picture type
		///    stored in the current instance.
		/// </value>
		/// <remarks>
		///    There should only be one frame with a matching
		///    description and type per tag.
		/// </remarks>
		public PictureType Type {
			get {ParseRawData (); return type;}
			set {type = value;}
		}
		
		/// <summary>
		///    Gets and sets the description stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the description
		///    stored in the current instance.
		/// </value>
		/// <remarks>
		///    There should only be one frame with a matching
		///    description and type per tag.
		/// </remarks>
		public string Description {
			get {
				ParseRawData ();
				if (description != null)
					return description;
				
				return string.Empty;
			}
			set {description = value;}
		}
		
		/// <summary>
		///    Gets and sets the image data stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> containing the image data
		///    stored in the current instance.
		/// </value>
		public ByteVector Data {
			get {
				ParseRawData ();
				return data != null ? data : new ByteVector ();
			}
			set {data = value;}
		}
		
		#endregion
		
		
		
		#region Public Methods
		
		/// <summary>
		///    Gets a string representation of the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> representing the current
		///    instance.
		/// </returns>
		public override string ToString ()
		{
			System.Text.StringBuilder builder
				= new System.Text.StringBuilder ();
			
			if (string.IsNullOrEmpty (Description)) {
				builder.Append (Description);
				builder.Append (" ");
			}
			
			builder.AppendFormat (
				System.Globalization.CultureInfo.InvariantCulture,
				"[{0}] {1} bytes", MimeType, Data.Count);
			
			return builder.ToString ();
		}
		
		#endregion
		
		
		
		#region Public Static Methods
		
		/// <summary>
		///    Gets a specified picture frame from the specified tag,
		///    optionally creating it if it does not exist.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="description">
		///    A <see cref="string" /> specifying the description to
		///    match.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="AttachedPictureFrame" /> object containing
		///    the matching frame, or <see langword="null" /> if a match
		///    wasn't found and <paramref name="create" /> is <see
		///    langword="false" />.
		/// </returns>
		public static AttachedPictureFrame Get (Tag tag,
		                                        string description,
		                                        bool create)
		{
			return Get (tag, description, PictureType.Other, create);
		}
		
		/// <summary>
		///    Gets a specified picture frame from the specified tag,
		///    optionally creating it if it does not exist.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="type">
		///    A <see cref="PictureType" /> specifying the picture type
		///    to match.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="AttachedPictureFrame" /> object containing
		///    the matching frame, or <see langword="null" /> if a match
		///    wasn't found and <paramref name="create" /> is <see
		///    langword="false" />.
		/// </returns>
		public static AttachedPictureFrame Get (Tag tag,
		                                        PictureType type,
		                                        bool create)
		{
			return Get (tag, null, type, create);
		}
		
		/// <summary>
		///    Gets a specified picture frame from the specified tag,
		///    optionally creating it if it does not exist.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="description">
		///    A <see cref="string" /> specifying the description to
		///    match.
		/// </param>
		/// <param name="type">
		///    A <see cref="PictureType" /> specifying the picture type
		///    to match.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="AttachedPictureFrame" /> object containing
		///    the matching frame, or <see langword="null" /> if a match
		///    wasn't found and <paramref name="create" /> is <see
		///    langword="false" />.
		/// </returns>
		/// <example>
		///    <para>Sets a cover image with a description. Because <see
		///    cref="Get(Tag,string,PictureType,bool)" /> is used, if
		///    the program is called again with the same audio file and
		///    desciption, the picture will be overwritten with the new
		///    one.</para>
		///    <code lang="C#">
		/// using TagLib;
		/// using TagLib.Id3v2;
		///
		/// public static class SetId3v2Cover
		/// {
		/// 	public static void Main (string [] args)
		/// 	{
		/// 		if (args.Length != 3)
		/// 			throw new ApplicationException (
		/// 				"USAGE: SetId3v2Cover.exe AUDIO_FILE PICTURE_FILE DESCRIPTION");
		///
		/// 		// Create the file. Can throw file to TagLib# exceptions.
		/// 		File file = File.Create (args [0]);
		///
		/// 		// Get or create the ID3v2 tag.
		/// 		TagLib.Id3v2.Tag tag = file.GetTag (TagTypes.Id3v2, true) as TagLib.Id3v2.Tag;
		/// 		if (tag == null)
		/// 			throw new ApplicationException ("File does not support ID3v2 tags.");
		///
		/// 		// Create a picture. Can throw file related exceptions.
		///		TagLib.Picture picture = TagLib.Picture.CreateFromPath (args [1]);
		///
		/// 		// Get or create the picture frame.
		/// 		AttachedPictureFrame frame = AttachedPictureFrame.Get (
		/// 			tag, args [2], PictureType.FrontCover, true);
		///
		/// 		// Set the data from the picture.
		/// 		frame.MimeType = picture.MimeType;
		/// 		frame.Data     = picture.data;
		/// 		
		/// 		// Save the file.
		/// 		file.Save ();
		/// 	}
		/// }
		///    </code>
		/// </example>
		public static AttachedPictureFrame Get (Tag tag,
		                                        string description,
		                                        PictureType type,
		                                        bool create)
		{
			AttachedPictureFrame apic;
			foreach (Frame frame in tag.GetFrames (FrameType.APIC)) {
				apic = frame as AttachedPictureFrame;
				
				if (apic == null)
					continue;
				
				if (description != null && apic.Description != description)
					continue;
				
				if (type != PictureType.Other && apic.Type != type)
					continue;
				
				return apic;
			}
			
			if (!create)
				return null;
			
			apic = new AttachedPictureFrame ();
			apic.Description = description;
			apic.Type = type;
			
			tag.AddFrame (apic);
			
			return apic;
		}
		
		#endregion
		
		
		
		#region Protected Methods
		
		/// <summary>
		///    Populates the values in the current instance by parsing
		///    its field data in a specified version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the
		///    extracted field data.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    field data is encoded in.
		/// </param>
		/// <exception cref="CorruptFileException">
		///    <paramref name="data" /> contains less than 5 bytes.
		/// </exception>
		protected override void ParseFields (ByteVector data, byte version)
		{
			if (data.Count < 5)
				throw new CorruptFileException (
					"A picture frame must contain at least 5 bytes.");
			
			raw_data = data;
			raw_version = version;
		}
		
		/// <summary>
		///    Performs the actual parsing of the raw data.
		/// </summary>
		/// <remarks>
		///    Because of the high parsing cost and relatively low usage
		///    of the class, <see cref="ParseFields" /> only stores the
		///    field data so it can be parsed on demand. Whenever a
		///    property or method is called which requires the data,
		///    this method is called, and only on the first call does it
		///    actually parse the data.
		/// </remarks>
		protected void ParseRawData ()
		{
			if (raw_data == null)
				return;
			
			int pos = 0;
			int offset;
			
			text_encoding = (StringType) raw_data [pos++];
			
			if (raw_version > 2) {
				offset = raw_data.Find (ByteVector.TextDelimiter (
					StringType.Latin1), pos);
				
				if(offset < pos)
					return;
				
				mime_type = raw_data.ToString (
					StringType.Latin1, pos, offset - pos);
				pos = offset + 1;
			} else {
				ByteVector ext = raw_data.Mid (pos, 3);
				
				if (ext == "JPG")
					mime_type = "image/jpeg";
				else if (ext == "PNG")
					mime_type = "image/png";
				else
					mime_type = "image/unknown";
				
				pos += 3;
			}
			
			ByteVector delim = ByteVector.TextDelimiter (
				text_encoding);
			
			type = (PictureType) raw_data [pos++];
			
			offset = raw_data.Find (delim, pos, delim.Count);
			
			if(offset < pos)
				return;
			
			description = raw_data.ToString (text_encoding, pos,
				offset - pos);
			pos = offset + delim.Count;
			raw_data.RemoveRange (0, pos);
			this.data = raw_data;
			
			this.raw_data = null;
		}
		
		/// <summary>
		///    Renders the values in the current instance into field
		///    data for a specified version.
		/// </summary>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    field data is to be encoded in.
		/// </param>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered field data.
		/// </returns>
		protected override ByteVector RenderFields (byte version)
		{
			if (raw_data != null && raw_version == version)
				return raw_data;
			
			StringType encoding = CorrectEncoding (TextEncoding,
				version);
			ByteVector data = new ByteVector ();
			
			data.Add ((byte) encoding);
			
			if (version == 2) {
				switch (MimeType) {
				case "image/png":
					data.Add ("PNG");
					break;
				case "image/jpeg":
					data.Add ("JPG");
					break;
				default:
					data.Add ("XXX");
					break;
				}
			} else {
				data.Add (ByteVector.FromString (MimeType,
					StringType.Latin1));
				data.Add (ByteVector.TextDelimiter (
					StringType.Latin1));
			}
			
			data.Add ((byte) type);
			data.Add (ByteVector.FromString (Description, encoding));
			data.Add (ByteVector.TextDelimiter (encoding));
			data.Add (this.data);
			
			return data;
		}
		
#endregion
		
		
		
#region ICloneable
		
		/// <summary>
		///    Creates a deep copy of the current instance.
		/// </summary>
		/// <returns>
		///    A new <see cref="Frame" /> object identical to the
		///    current instance.
		/// </returns>
		public override Frame Clone ()
		{
			AttachedPictureFrame frame = new AttachedPictureFrame ();
			frame.text_encoding = text_encoding;
			frame.mime_type = mime_type;
			frame.type = type;
			frame.description = description;
			if (data != null)
				frame.data = new ByteVector (data);
			if (raw_data != null)
				frame.data = new ByteVector (raw_data);
			frame.raw_version = raw_version;
			return frame;
		}
		
#endregion
	}
}
