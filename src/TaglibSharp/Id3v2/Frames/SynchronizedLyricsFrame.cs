//
// SynchronizedLyricsFrame.cs:
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

using System.Collections.Generic;

namespace TagLib.Id3v2
{
	/// <summary>
	///    This class extends <see cref="Frame" />, implementing support for
	///    ID3v2 Synchronised Lyrics and Text (SYLT) Frames.
	/// </summary>
	public class SynchronisedLyricsFrame : Frame
	{

		#region Private Properties

		/// <summary>
		///    Contains the ISO-639-2 language code.
		/// </summary>
		string language;

		/// <summary>
		///    Contains the text.
		/// </summary>
		SynchedText[] text = new SynchedText[0];

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="SynchronisedLyricsFrame" /> with a specified
		///    description, ISO-639-2 language code, text type, and text
		///    encoding.
		/// </summary>
		/// <param name="description">
		///    A <see cref="string" /> object containing the description
		///    of the new instnace.
		/// </param>
		/// <param name="language">
		///    A <see cref="string" /> object containing the ISO-639-2
		///    language code of the new instance.
		/// </param>
		/// <param name="type">
		///    A <see cref="SynchedTextType" /> containing the type of
		///    text to be stored in the new instance.
		/// </param>
		/// <param name="encoding">
		///    A <see cref="StringType" /> containing the text encoding
		///    to use when rendering the new instance.
		/// </param>
		/// <remarks>
		///    When a frame is created, it is not automatically added to
		///    the tag. Consider using <see cref="Get" /> for more
		///    integrated frame creation.
		/// </remarks>
		public SynchronisedLyricsFrame (string description, string language, SynchedTextType type, StringType encoding)
			: base (FrameType.SYLT, 4)
		{
			TextEncoding = encoding;
			this.language = language;
			Description = description;
			Type = type;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="SynchronisedLyricsFrame" /> with a specified
		///    description, ISO-639-2 language code, and text type.
		/// </summary>
		/// <param name="description">
		///    A <see cref="string" /> object containing the description
		///    of the new instnace.
		/// </param>
		/// <param name="language">
		///    A <see cref="string" /> object containing the ISO-639-2
		///    language code of the new instance.
		/// </param>
		/// <param name="type">
		///    A <see cref="SynchedTextType" /> containing the type of
		///    text to be stored in the new instance.
		/// </param>
		/// <remarks>
		///    When a frame is created, it is not automatically added to
		///    the tag. Consider using <see cref="Get" /> for more
		///    integrated frame creation.
		/// </remarks>
		public SynchronisedLyricsFrame (string description, string language, SynchedTextType type)
			: this (description, language, type, Tag.DefaultEncoding)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="SynchronisedLyricsFrame" /> by reading its raw data
		///    in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object starting with the raw
		///    representation of the new instance.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    raw frame is encoded in.
		/// </param>
		public SynchronisedLyricsFrame (ByteVector data, byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="SynchronisedLyricsFrame" /> by reading its raw data
		///    in a specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the raw
		///    representation of the new instance.
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
		protected internal SynchronisedLyricsFrame (ByteVector data, int offset, FrameHeader header, byte version)
			: base (header)
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
		public StringType TextEncoding { get; set; } = Tag.DefaultEncoding;

		/// <summary>
		///    Gets and sets the ISO-639-2 language code stored in the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the ISO-639-2 language
		///    code stored in the current instance.
		/// </value>
		/// <remarks>
		///    There should only be one frame with a matching
		///    description, type, and ISO-639-2 language code per tag.
		/// </remarks>
		public string Language {
			get {
				return (language != null && language.Length > 2)
					? language.Substring (0, 3) : "XXX";
			}
			set { language = value; }
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
		///    description, type, and ISO-639-2 language code per tag.
		/// </remarks>
		public string Description { get; set; }

		/// <summary>
		///    Gets and sets the timestamp format used by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="TimestampFormat" /> value describing the
		///    timestamp format used by the current instance.
		/// </value>
		public TimestampFormat Format { get; set; } = TimestampFormat.Unknown;

		/// <summary>
		///    Gets and sets the type of text contained in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="TimestampFormat" /> value describing the
		///    type of text contained in the current instance.
		/// </value>
		public SynchedTextType Type { get; set; } = SynchedTextType.Other;

		/// <summary>
		///    Gets and sets the text contained in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="T:SynchedText[]" /> containing the text
		///    contained in the current instance.
		/// </value>
		public SynchedText[] Text {
			get { return text; }
			set { text = value ?? (new SynchedText[0]); }
		}

		#endregion



		#region Public Static Methods

		/// <summary>
		///    Gets a specified lyrics frame from the specified tag,
		///    optionally creating it if it does not exist.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="description">
		///    A <see cref="string" /> object specifying the description
		///    to match.
		/// </param>
		/// <param name="language">
		///    A <see cref="string" /> object specifying the ISO-639-2
		///    language code to match.
		/// </param>
		/// <param name="type">
		///    A <see cref="SynchedTextType" /> value specifying the
		///    text type to match.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="SynchronisedLyricsFrame" /> object
		///    containing the matching frame, or <see langword="null" />
		///    if a match wasn't found and <paramref name="create" /> is
		///    <see langword="false" />.
		/// </returns>
		public static SynchronisedLyricsFrame Get (Tag tag, string description, string language, SynchedTextType type, bool create)
		{
			foreach (Frame f in tag) {
				if (!(f is SynchronisedLyricsFrame lyr))
					continue;

				if (lyr.Description == description &&
					(language == null ||
						language == lyr.Language) &&
					type == lyr.Type)
					return lyr;
			}

			if (!create)
				return null;

			var frame = new SynchronisedLyricsFrame (description, language, type);
			tag.AddFrame (frame);
			return frame;
		}

		/// <summary>
		///    Gets a specified lyrics frame from the specified tag,
		///    trying to to match the description and language but
		///    accepting an incomplete match.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="description">
		///    A <see cref="string" /> object specifying the description
		///    to match.
		/// </param>
		/// <param name="language">
		///    A <see cref="string" /> object specifying the ISO-639-2
		///    language code to match.
		/// </param>
		/// <param name="type">
		///    A <see cref="SynchedTextType" /> value specifying the
		///    text type to match.
		/// </param>
		/// <returns>
		///    A <see cref="SynchronisedLyricsFrame" /> object
		///    containing the matching frame, or <see langword="null" />
		///    if a match wasn't found.
		/// </returns>
		/// <remarks>
		///    <para>The method tries matching with the following order
		///    of precidence:</para>
		///    <list type="number">
		///       <item><term>The first frame with a matching
		///       description, language, and type.</term></item>
		///       <item><term>The first frame with a matching
		///       description and language.</term></item>
		///       <item><term>The first frame with a matching
		///       language.</term></item>
		///       <item><term>The first frame with a matching
		///       description.</term></item>
		///       <item><term>The first frame with a matching
		///       type.</term></item>
		///       <item><term>The first frame.</term></item>
		///    </list>
		/// </remarks>
		public static SynchronisedLyricsFrame GetPreferred (Tag tag, string description, string language, SynchedTextType type)
		{
			// This is weird, so bear with me. The best thing we can
			// have is something straightforward and in our own
			// language. If it has a description, then it is
			// probably used for something other than an actual
			// comment. If that doesn't work, we'd still rather have
			// something in our language than something in another.
			// After that all we have left are things in other
			// languages, so we'd rather have one with actual
			// content, so we try to get one with no description
			// first.

			int best_value = -1;
			SynchronisedLyricsFrame best_frame = null;

			foreach (Frame f in tag) {
				if (!(f is SynchronisedLyricsFrame cf))
					continue;

				int value = 0;
				if (cf.Language == language)
					value += 4;
				if (cf.Description == description)
					value += 2;
				if (cf.Type == type)
					value += 1;

				if (value == 7)
					return cf;

				if (value <= best_value)
					continue;

				best_value = value;
				best_frame = cf;
			}

			return best_frame;
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
			if (data.Count < 6)
				throw new CorruptFileException ("Not enough bytes in field.");

			TextEncoding = (StringType)data[0];
			language = data.ToString (StringType.Latin1, 1, 3);
			Format = (TimestampFormat)data[4];
			Type = (SynchedTextType)data[5];

			var delim = ByteVector.TextDelimiter (TextEncoding);
			int delim_index = data.Find (delim, 6, delim.Count);

			if (delim_index < 0)
				throw new CorruptFileException ("Text delimiter expected.");

			Description = data.ToString (TextEncoding, 6, delim_index - 6);

			int offset = delim_index + delim.Count;
			var l = new List<SynchedText> ();

			while (offset + delim.Count + 4 < data.Count) {
				delim_index = data.Find (delim, offset, delim.Count);

				if (delim_index < offset)
					throw new CorruptFileException ("Text delimiter expected.");

				string text = data.ToString (TextEncoding, offset, delim_index - offset);
				offset = delim_index + delim.Count;

				if (offset + 4 > data.Count)
					break;

				l.Add (new SynchedText (data.Mid (offset, 4).ToUInt (), text));
				offset += 4;
			}

			this.text = l.ToArray ();
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
			var encoding = CorrectEncoding (TextEncoding, version);
			var delim = ByteVector.TextDelimiter (encoding);
			var v = new ByteVector {
				(byte)encoding,
				ByteVector.FromString (Language, StringType.Latin1),
				(byte)Format,
				(byte)Type,
				ByteVector.FromString (Description, encoding),
				delim
			};

			foreach (SynchedText t in text) {
				v.Add (ByteVector.FromString (t.Text, encoding));
				v.Add (delim);
				v.Add (ByteVector.FromUInt ((uint)t.Time));
			}

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
			var frame = new SynchronisedLyricsFrame (Description, language, Type, TextEncoding) {
				Format = Format,
				text = (SynchedText[])text.Clone ()
			};
			return frame;
		}

		#endregion
	}


	/// <summary>
	///    This structure contains a single entry in a <see
	///    cref="SynchronisedLyricsFrame" /> object.
	/// </summary>
	public struct SynchedText
	{
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="SynchedText" /> with a specified time and text.
		/// </summary>
		/// <param name="time">
		///    A <see cref="long" /> value representing an amount of
		///    time in a format define in the class using it. The
		///    specific format is specified in <see
		///    cref="SynchronisedLyricsFrame.Format" />.
		/// </param>
		/// <param name="text">
		///    A <see cref="string" /> object containing the text
		///    for the point in time.
		/// </param>
		public SynchedText (long time, string text)
		{
			Time = time;
			Text = text;
		}

		/// <summary>
		///    Gets and sets the time offset of the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="long" /> value representing an amount of
		///    time in a format define in the class using it. The
		///    specific format is specified in <see
		///    cref="SynchronisedLyricsFrame.Format" />.
		/// </value>
		public long Time { get; set; }

		/// <summary>
		///    Gets and sets the text for the point in time represented
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the text
		///    for the point in time.
		/// </value>
		public string Text { get; set; }
	}
}
