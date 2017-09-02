//
// GeneralEncapsulatedObjectFrame.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   generalencapsulatedobjectframe.cpp from TagLib
//
// Copyright (C) 2007 Brian Nickel
// Copyright (C) 2007 Scott Wheeler (Original Implementation)
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

namespace TagLib.Id3v2 {
	/// <summary>
	///    This class extends <see cref="Frame" />, implementing support for
	///    ID3v2 General Encapsulated Object (GEOB) Frames.
	/// </summary>
	/// <remarks>
	///    <para>A <see cref="GeneralEncapsulatedObjectFrame" /> should be
	///    used for storing files and other objects relevant to the file but
	///    not supported by other frames.</para>
	/// </remarks>
	public class GeneralEncapsulatedObjectFrame : Frame
	{
#region Private Fields
		
		/// <summary>
		///    Contains the text encoding to use when rendering the
		///    current instance.
		/// </summary>
		private StringType encoding = Tag.DefaultEncoding;
		
		/// <summary>
		///    Contains the mime type of <see cref="data" />.
		/// </summary>
		private string mime_type = null;
		
		/// <summary>
		///    Contains the original file name.
		/// </summary>
		string file_name = null;
		
		/// <summary>
		///    Contains the description.
		/// </summary>
		private string description = null;
		
		/// <summary>
		///    Contains the data.
		/// </summary>
		private ByteVector data = null;
		
#endregion
		
		
		
#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="GeneralEncapsulatedObjectFrame" /> with no
		///    contents.
		/// </summary>
		public GeneralEncapsulatedObjectFrame ()
			: base (FrameType.GEOB, 4)
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="GeneralEncapsulatedObjectFrame" /> by reading its
		///    raw data in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object starting with the raw
		///    representation of the new frame.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    raw frame is encoded in.
		/// </param>
		public GeneralEncapsulatedObjectFrame (ByteVector data,
		                                       byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="GeneralEncapsulatedObjectFrame" /> by reading its
		///    raw data in a specified ID3v2 version.
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
		protected internal GeneralEncapsulatedObjectFrame (ByteVector data,
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
			get {return encoding;}
			set {encoding = value;}
		}
		
		/// <summary>
		///    Gets and sets the mime-type of the object stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the mime-type of the
		///    object stored in the current instance.
		/// </value>
		public string MimeType {
			get {
				if (mime_type != null)
					return mime_type;
				
				return string.Empty;
			}
			set {mime_type = value;}
		}
		
		/// <summary>
		///    Gets and sets the file name of the object stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the file name of the
		///    object stored in the current instance.
		/// </value>
		public string FileName {
			get {
				if (file_name != null)
					return file_name;
				
				return string.Empty;
			}
			set {file_name = value;}
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
				if (description != null)
					return description;
				
				return string.Empty;
			}
			set {description = value;}
		}
		
		/// <summary>
		///    Gets and sets the object data stored in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> containing the object data
		///    stored in the current instance.
		/// </value>
		public ByteVector Object {
			get {return data != null ? data : new ByteVector ();}
			set {data = value;}
		}
		
#endregion
		
		
		
#region Public Methods
		
		/// <summary>
		///    Creates a text description of the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> object containing a description
		///    of the current instance.
		/// </returns>
		public override string ToString ()
		{
			System.Text.StringBuilder builder
				= new System.Text.StringBuilder ();
			
			if (Description.Length == 0) {
				builder.Append (Description);
				builder.Append (" ");
			}
			
			builder.AppendFormat (
				System.Globalization.CultureInfo.InvariantCulture,
				"[{0}] {1} bytes", MimeType, Object.Count);
			
			return builder.ToString ();
		}
		
#endregion
		
		
		
#region Public Static Methods
		
		/// <summary>
		///    Gets a specified encapsulated object frame from the
		///    specified tag, optionally creating it if it does not
		///    exist.
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
		///    A <see cref="GeneralEncapsulatedObjectFrame" /> object
		///    containing the matching frame, or <see langword="null" />
		///    if a match wasn't found and <paramref name="create" /> is
		///    <see langword="false" />.
		/// </returns>
		public static GeneralEncapsulatedObjectFrame Get (Tag tag,
		                                                  string description,
		                                                  bool create)
		{
			GeneralEncapsulatedObjectFrame geob;
			foreach (Frame frame in tag.GetFrames (FrameType.GEOB)) {
				geob = frame as GeneralEncapsulatedObjectFrame;
				
				if (geob == null)
					continue;
				
				if (geob.Description != description)
					continue;
				
				return geob;
			}
			
			if (!create)
				return null;
			
			geob = new GeneralEncapsulatedObjectFrame ();
			geob.Description = description;
			tag.AddFrame (geob);
			return geob;
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
		protected override void ParseFields (ByteVector data,
		                                     byte version)
		{
			if (data.Count < 4)
				throw new CorruptFileException (
					"An object frame must contain at least 4 bytes.");
			
			int start = 0;
			
			encoding =  (StringType) data [start++];
			
			int end = data.Find (
				ByteVector.TextDelimiter (StringType.Latin1),
				start);
			
			if (end < start)
				return;
			
			mime_type = data.ToString (StringType.Latin1, start,
				end - start);
			
			ByteVector delim = ByteVector.TextDelimiter (
				encoding);
			start = end + 1;
			end = data.Find (delim, start, delim.Count);
			
			if (end < start)
				return;
			
			file_name = data.ToString (encoding, start,
				end - start);
			start = end + delim.Count;
			end = data.Find (delim, start, delim.Count);
			
			if (end < start)
				return;
			
			description = data.ToString (encoding, start,
				end - start);
			start = end + delim.Count;
			
			data.RemoveRange (0, start);
			this.data = data;
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
			StringType encoding = CorrectEncoding (this.encoding,
				version);
			ByteVector v = new ByteVector ();
			
			v.Add ((byte) encoding);
			
			if (MimeType != null)
				v.Add (ByteVector.FromString (MimeType,
					StringType.Latin1));
			v.Add (ByteVector.TextDelimiter (StringType.Latin1));
			
			if (FileName != null)
				v.Add (ByteVector.FromString (FileName,
					encoding));
			v.Add (ByteVector.TextDelimiter (encoding));
			
			if (Description != null)
				v.Add (ByteVector.FromString (Description,
					encoding));
			v.Add (ByteVector.TextDelimiter (encoding));
			
			v.Add (data);
			return v;
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
			GeneralEncapsulatedObjectFrame frame =
				new GeneralEncapsulatedObjectFrame ();
			frame.encoding = encoding;
			frame.mime_type = mime_type;
			frame.file_name = file_name;
			frame.description = description;
			if (data != null)
				frame.data = new ByteVector (data);
			return frame;
		}
		
#endregion
	}
}
