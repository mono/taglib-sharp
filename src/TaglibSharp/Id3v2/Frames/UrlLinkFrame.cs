//
// UrlLinkFrame.cs: Provides support ID3v2 Url Link Frames
// (Section 4.3.1), covering "W000" to "WZZZ", excluding "WXXX".
//
// Author:
//   Helmut Wahrmann
//
// Original Source:
//   textidentificationframe.cpp from TagLib
//
// Copyright (C) 2008 Helmut Wahrmann
// Copyright (C) 2002,2003 Scott Wheeler (Original Implementation)
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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;


namespace TagLib.Id3v2
{
	/// <summary>
	///    This class extends <see cref="Frame" /> to provide support ID3v2
	///    Url Link Frames (Section 4.3.1), covering "<c>W000</c>" to
	///    "<c>WZZZ</c>", excluding "<c>WXXX</c>".
	/// </summary>
	/// <remarks>
	///    <para>With these frames dynamic data such as webpages with touring
	///    information, price information or plain ordinary news can be added to
	///    the tag. There may only be one URL [URL] link frame of its kind in an
	///    tag, except when stated otherwise in the frame description. If the
	///    text string is followed by a string termination, all the following
	///    information should be ignored and not be displayed.</para>
	///    <para>The following table contains types and descriptions as
	///    found in the ID3 2.4.0 native frames specification. (Copyright
	///    (C) Martin Nilsson 2000.)</para>
	///
	///    <list type="table">
	///       <listheader>
	///          <term>ID</term>
	///          <description>Description</description>
	///       </listheader>
	///       <item>
	///          <term>WCOM</term>
	///          <description>The 'Commercial information' frame is a URL pointing at a webpage
	///          with information such as where the album can be bought. There may be
	///          more than one "WCOM" frame in a tag, but not with the same content.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WCOP</term>
	///          <description>The 'Copyright/Legal information' frame is a URL pointing at a
	///          webpage where the terms of use and ownership of the file is described.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WOAF</term>
	///          <description>The 'Official audio file webpage' frame is a URL pointing at a file
	///          specific webpage.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WOAR</term>
	///          <description>The 'Official artist/performer webpage' frame is a URL pointing at
	///          the artists official webpage. There may be more than one "WOAR" frame
	///          in a tag if the audio contains more than one performer, but not with
	///          the same content.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WOAS</term>
	///          <description>The 'Official audio source webpage' frame is a URL pointing at the
	///          official webpage for the source of the audio file, e.g. a movie.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WORS</term>
	///          <description>The 'Official Internet radio station homepage' contains a URL
	///          pointing at the homepage of the internet radio station.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WPAY</term>
	///          <description>The 'Payment' frame is a URL pointing at a webpage that will handle
	///          the process of paying for this file.
	///          </description>
	///       </item>
	///       <item>
	///          <term>WPUB</term>
	///          <description>The 'Publishers official webpage' frame is a URL pointing at the
	///          official webpage for the publisher.
	///          </description>
	///       </item>
	///    </list>
	/// </remarks>
	public class UrlLinkFrame : Frame
	{
		#region Private Fields

		string url;

		/// <summary>
		///    Contains the raw data from the frame, or
		///    <see langword="null" /> if it has been processed.
		/// </summary>
		/// <remarks>
		///    Rather than processing the data when the frame is loaded,
		///    it is parsed on demand, reducing the ammount of
		///    unnecessary conversion.
		/// </remarks>
		protected ByteVector raw_data;

		/// <summary>
		///    Contains the ID3v2 version of <see cref="raw_data" />.
		/// </summary>
		byte raw_version;

		#endregion

		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UrlLinkFrame" /> with a specified
		///    identifier and text encoding.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing an ID3v2.4
		///    frame identifier.
		/// </param>
		public UrlLinkFrame (ByteVector ident)
		  : base (ident, 4)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UrlLinkFrame" /> by reading its raw
		///    contents in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object starting with the
		///    frame to read.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> value containing the ID3v2 version
		///    in which <paramref name="data" /> is encoded.
		/// </param>
		public UrlLinkFrame (ByteVector data, byte version)
		  : base (data, version)
		{
			SetData (data, 0, version, true);
		}

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UrlLinkFrame" /> by reading its raw
		///    contents from a specifed position in a
		///    <see cref="ByteVector" /> object in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the frame
		///    to read.
		/// </param>
		/// <param name="offset">
		///    A <see cref="int" /> value specifying the offset in
		///    <paramref name="data" /> at which the frame begins.
		/// </param>
		/// <param name="header">
		///    A <see cref="FrameHeader" /> value containing the header
		///    that would be read in the frame.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> value containing the ID3v2 version
		///    in which <paramref name="data" /> is encoded.
		/// </param>
		protected internal UrlLinkFrame (ByteVector data, int offset, FrameHeader header, byte version)
		  : base (header)
		{
			if (data[offset] != (byte)'W') {
				throw new ArgumentException("Invalid header data. Expecting a WNNN frame");
			}
			SetData (data, offset, version, false);
		}

		#endregion

		#region Public Properties
		/// <summary>
		///    Gets and sets the text contained in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="T:string[]" /> containing the text contained
		///    in the current instance.
		/// </value>
		/// <remarks>
		///    <para>Modifying the contents of the returned value will
		///    not modify the contents of the current instance. The
		///    value must be reassigned for the value to change.</para>
		/// </remarks>
		/// <example>
		///    <para>Modifying the values text values of a frame.</para>
		///    <code> UrlLinkFrame frame = UrlLinkFrame.Get (myTag, "WCOP", true);
		/// /* Upper casing all the text: */
		/// string[] text = frame.Text;
		/// for (int i = 0; i &lt; text.Length; i++)
		///	text [i] = text [i].ToUpper ();
		/// frame.Text = text;
		///
		/// /* Replacing the value completely: */
		/// frame.Text = new string [] {"http://www.somewhere.com"};</code>
		/// </example>
		[Obsolete("Use property Url instead")]
		public virtual string[] Text {
			get {
				return new string[] { Url };
			}
			set {
				if (value?.Length > 0){
					raw_data = null;
					Url = value[0];
					return;
				}

				throw new ArgumentException ("Text must be a one-element array");
			}
		}

		/// <summary>
		/// Gets or sets the url of the frame.
		/// </summary>
		public string Url {
			get {
				ParseRawData();
				return url;
			}
			set {
				url = value;
			}
		}


		#endregion

		#region Public Methods

		/// <summary>
		///    Gets a string representation of the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> containing the joined text.
		/// </returns>
		public override string ToString ()
		{
			ParseRawData ();
			return Url;
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		///    Gets a <see cref="UrlLinkFrame" /> object of a
		///    specified type from a specified tag, optionally creating
		///    and adding one with a specified encoding if none is
		///    found.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search for the specified
		///    tag in.
		/// </param>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the frame
		///    identifer to search for.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> value specifying whether or not to
		///    create a new frame if an existing frame was not found.
		/// </param>
		/// <returns>
		///    A <see cref="UrlLinkFrame" /> object containing
		///    the frame found in or added to <paramref name="tag" /> or
		///    <see langword="null" /> if no value was found
		///    <paramref name="create" /> is <see langword="false" />.
		/// </returns>
		/// <remarks>
		///    To create a frame without having to specify the encoding,
		///    use <see cref="Get(Tag,ByteVector,bool)" />.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="tag" /> or <paramref name="ident" /> is
		///    <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		public static UrlLinkFrame Get (Tag tag, ByteVector ident, bool create)
		{
			if (tag == null)
				throw new ArgumentNullException (nameof (tag));

			if (ident == null)
				throw new ArgumentNullException (nameof (ident));

			if (ident.Count != 4)
				throw new ArgumentException ("Identifier must be four bytes long.",
				  nameof (ident));

			foreach (var frame in tag.GetFrames<UrlLinkFrame> (ident))
				return frame;

			if (!create)
				return null;

			var new_frame = new UrlLinkFrame (ident);
			tag.AddFrame (new_frame);
			return new_frame;
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
		protected override void ParseFields (ByteVector data, byte version)
		{
			raw_data = data;
			raw_version = version;
		}

		/// <summary>
		///    Performs the actual parsing of the raw data.
		///
		///  <para>
		///   With these frames dynamic data such as webpages with touring information, price information or plain ordinary news can be added to the tag.
		///   There may only be one URL link frame of its kind in an tag, except when stated otherwise in the frame description.
		///   If the textstring is followed by a termination ($00 (00)) all the following information should be ignored and not be displayed.
		///   All URL link frame identifiers begins with "W".
		///   Only URL link frame identifiers begins with "W". All URL link frames have the following format:
		///
		///   &lt;Header for 'URL link frame', ID: "W000" - "WZZZ", excluding "WXXX" described in 4.3.2.&gt;
		///   URL&lt;text string&gt;
		/// </para>
		/// </summary>
		/// <remarks>
		///    Because of the high parsing cost and relatively low usage
		///    of the class, <see cref="ParseFields" /> only stores the
		///    field data so it can be parsed on demand. Whenever a
		///    property or method is called which requires the data,
		///    this method is called, and only on the first call does it
		///    actually parse the data.
		/// </remarks>
		protected virtual void ParseRawData ()
		{
			if (raw_data == null)
				return;

			ByteVector data = raw_data;
			raw_data = null;
			Url = data.ToString ();
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

			return ByteVector.FromString (Url, StringType.Latin1);
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
			return new UrlLinkFrame (header.FrameId) { Url = Url, raw_version = raw_version };
		}

		#endregion
	}



	/// <summary>
	///    This class extends <see cref="UrlLinkFrame" /> to provide
	///    support for ID3v2 User Url Link (WXXX) Frames.
	/// </summary>
	public sealed class UserUrlLinkFrame : UrlLinkFrame
	{
		string description;
		private StringType descriptionEncoding;

		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UserUrlLinkFrame" /> with a specified
		///    description and text encoding.
		/// </summary>
		/// <param name="description">
		///    A <see cref="string" /> containing the description of the
		///    new frame.
		/// </param>
		/// <param name="encoding">
		///    A <see cref="StringType" /> containing the text encoding
		///    to use when rendering the new frame.
		/// </param>
		/// <remarks>
		///    When a frame is created, it is not automatically added to
		///    the tag. Consider using
		///    <see cref="Get(Tag,string,StringType,bool)" /> for more
		///    integrated frame creation.
		/// </remarks>
		public UserUrlLinkFrame (string description, StringType encoding)
		  : base (FrameType.WXXX)
		{
			if (string.IsNullOrEmpty (description)) {
				throw new ArgumentException ("A description must not be null or empty.");
			}
			Description = description;
			DescriptionEncoding = encoding;
		}

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UserUrlLinkFrame" /> with a specified
		///    description.
		/// </summary>
		/// <param name="description">
		///    A <see cref="string" /> containing the description of the
		///    new frame.
		/// </param>
		/// <remarks>
		///    When a frame is created, it is not automatically added to
		///    the tag. Consider using
		///    <see cref="Get(Tag,string,bool)" /> for more integrated frame
		///    creation.
		/// </remarks>
		public UserUrlLinkFrame (string description)
		  : this(description, StringType.Latin1)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UserUrlLinkFrame" /> by reading its raw
		///    data in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object starting with the raw
		///    representation of the new frame.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    raw frame is encoded in.
		/// </param>
		public UserUrlLinkFrame (ByteVector data, byte version)
		  : base (data, version)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of
		///    <see cref="UserUrlLinkFrame" /> by reading its raw
		///    data in a specified ID3v2 version.
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
		internal UserUrlLinkFrame (ByteVector data, int offset, FrameHeader header, byte version)
		  : base (data, offset, header, version)
		{
		}

		#endregion



		#region Public Properties

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
		///    description per tag.
		/// </remarks>
		public string Description {
			get {
				ParseRawData();
				return description;
			}
			set => description = value;
		}

		/// <summary>
		///    Gets and sets the text contained in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="T:string[]" /> containing the text contained
		///    in the current instance.
		/// </value>
		/// <remarks>
		///    <para>Modifying the contents of the returned value will
		///    not modify the contents of the current instance. The
		///    value must be reassigned for the value to change.</para>
		/// </remarks>
		[Obsolete("Use property Url instead.")]
		public override string[] Text {
			get {
				return new string[] { Url };
			}
			set {
				
				Url = value[0];
			}
		}

		/// <summary>
		/// The encoding of the description. Defaults to <see cref="StringType.Latin1"/>.
		/// </summary>
		public StringType DescriptionEncoding {
			get {
				ParseRawData();
				return descriptionEncoding;
			}
			set => descriptionEncoding = value;
		}

		#endregion



		#region Public Methods

		/// <summary>
		///    Gets a string representation of the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> containing the joined text.
		/// </returns>
		public override string ToString ()
		{
			return new StringBuilder ().Append ("[")
			  .Append (Description)
			  .Append ("] ")
			  .Append (Url).ToString ();
		}

		#endregion


		/// <summary>
		/// Performs the actual parsing of the raw data.
		///
		/// <para>
		/// This frame is intended for URL links concerning the audiofile in a similar way to the other "W"-frames.
		/// The frame body consists of a description of the string, represented as a terminated string, followed by the actual URL.
		/// The URL is always encoded with ISO-8859-1.
		/// There may be more than one "WXXX" frame in each tag, but only one with the same description.
		/// </para>
		/// <code>
		///  <![CDATA[
		///    <Header for 'User defined URL link frame', ID: "WXXX">
		///    Text encoding	$xx
		///    Description<text string according to encoding> $00 (00)
		///    URL<text string>
		/// ]]>
		/// </code>
		/// </summary>
		protected override void ParseRawData ()
		{
			if (raw_data == null) {
				return;
			}

			ByteVector data = raw_data;
			raw_data = null;

			var descUrlSplit = data.Find (ByteVector.TextDelimiter (StringType.Latin1), 1);
			var urlStartIndex = descUrlSplit + 1;
			var urlEndOffset = data.Find (ByteVector.TextDelimiter (StringType.Latin1), urlStartIndex);
			if (urlEndOffset == -1) {
				urlEndOffset = data.Count;
			}

			var descriptionText = data.ToString (StringType.Latin1, 1, descUrlSplit - 1);
			var url = data.ToString (StringType.Latin1, urlStartIndex, urlEndOffset - urlStartIndex);

			Description = descriptionText;
			Url = url;
		}

		/// <inheritdoc />
		public override Frame Clone ()
		{
			return new UserUrlLinkFrame (Description, DescriptionEncoding) { Url = Url };
		}

		/// <inheritdoc />
		protected override ByteVector RenderFields (byte version)
		{
			// render the frame header
			var result = new ByteVector ();
			result.Add((byte)DescriptionEncoding);
			result.Add (ByteVector.FromString(Description, DescriptionEncoding));
			result.Add (0);
			result.Add (ByteVector.FromString(Url, StringType.Latin1));
			result.Add (0);

			return result;
		}

		#region Public Static Methods

		/// <summary>
		///    Gets a specified user text frame from the specified tag,
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
		///    A <see cref="StringType" /> specifying the encoding to
		///    use if creating a new frame.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="UserUrlLinkFrame" /> object
		///    containing the matching frame, or <see langword="null" />
		///    if a match wasn't found and <paramref name="create" /> is
		///    <see langword="false" />.
		/// </returns>
		public static UserUrlLinkFrame Get (Tag tag, string description, StringType type, bool create)
		{
			if (tag == null)
				throw new ArgumentNullException (nameof (tag));

			if (description == null)
				throw new ArgumentNullException (nameof (description));

			if (description.Length == 0)
				throw new ArgumentException ("Description must not be empty.",
				  nameof (description));

			foreach (var frame in tag.GetFrames<UserUrlLinkFrame> (FrameType.WXXX))
				if (description.Equals (frame.Description))
					return frame;

			if (!create)
				return null;

			var new_frame = new UserUrlLinkFrame (description, type);
			tag.AddFrame (new_frame);
			return new_frame;
		}

		/// <summary>
		///    Gets a specified user text frame from the specified tag,
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
		///    A <see cref="UserUrlLinkFrame" /> object
		///    containing the matching frame, or <see langword="null" />
		///    if a match wasn't found and <paramref name="create" /> is
		///    <see langword="false" />.
		/// </returns>
		public static UserUrlLinkFrame Get (Tag tag, string description, bool create)
		{
			return Get (tag, description, Tag.DefaultEncoding, create);
		}
		#endregion
	}
}
