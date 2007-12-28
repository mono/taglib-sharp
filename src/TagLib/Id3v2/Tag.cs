//
// Tag.cs: Provide support for reading and writing ID3v2 tags.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2tag.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TagLib.Id3v2 {
	/// <summary>
	///    This class extends <see cref="TagLib.Tag" /> and implements <see
	///    cref="T:System.Collections.Generic.IEnumerable`1" /> to provide support for reading and
	///    writing ID3v2 tags.
	/// </summary>
	public class Tag : TagLib.Tag, IEnumerable<Frame>
	{
#region Private Static Fields
		
		/// <summary>
		///    Contains the language to use for language specific
		///    fields.
		/// </summary>
		private static string language = "XXX";
		
		/// <summary>
		///    Contains the field to use for new tags.
		/// </summary>
		private static byte default_version = 4;
		
		/// <summary>
		///    Indicates whether or not all tags should be saved in
		///    <see cref="default_version" />.
		/// </summary>
		private static bool force_default_version = false;
		
		/// <summary>
		///    Specifies the default string type to use for new frames.
		/// </summary>
		private static StringType default_string_type = StringType.UTF8;
		
		/// <summary>
		///    Specifies whether or not all frames shoudl be saved in
		///    <see cref="default_string_type" />.
		/// </summary>
		private static bool force_default_string_type = false;
		
		/// <summary>
		///    Specifies whether or not numeric genres should be used
		///    when available.
		/// </summary>
		private static bool use_numeric_genres = true;
		
#endregion
		
		
		
#region Private Fields
		
		/// <summary>
		///    Contains the tag's header.
		/// </summary>
		private Header header = new Header ();
		
		/// <summary>
		///    Contains the tag's extended header.
		/// </summary>
		private ExtendedHeader extended_header = null;
		
		/// <summary>
		///    Contains the tag's frames.
		/// </summary>
		private List<Frame> frame_list = new List<Frame> ();
		
#endregion
		
		
		
#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Tag" /> with no contents.
		/// </summary>
		public Tag ()
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="Tag" /> by reading the contents from a specified
		///    position in a specified file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File" /> object containing the file from
		///    which the contents of the new instance is to be read.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specify at what position to
		///    read the tag.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="position" /> is less than zero or greater
		///    than the size of the file.
		/// </exception>
		public Tag (File file, long position)
		{
			if (file == null)
				throw new ArgumentNullException ("file");
			
			file.Mode = TagLib.File.AccessMode.Read;
			
			if (position < 0 ||
				position > file.Length - Header.Size)
				throw new ArgumentOutOfRangeException (
					"position");
			
			Read (file, position);
		}
		
		public Tag (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			if (data.Count < Header.Size)
				throw new CorruptFileException (
					"Does not contain enough header data.");
			
			header = new Header (data);
			
			// If the tag size is 0, then this is an invalid tag.
			// Tags must contain at least one frame.
			
			if(header.TagSize == 0)
				return;
			
			if (data.Count - Header.Size < header.TagSize)
				throw new CorruptFileException (
					"Does not contain enough tag data.");
			
			Parse (data.Mid ((int) Header.Size,
				(int) header.TagSize));
		}
		
#endregion
		
		
		
#region Public Methods
		
		/// <summary>
		///    Gets all frames contained in the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating
		///    through the frames.
		/// </returns>
		public IEnumerable<Frame> GetFrames ()
		{
			return frame_list;
		}
		
		/// <summary>
		///    Gets all frames with a specified identifier contained in
		///    the current instance.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier of the frames to return.
		/// </param>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating
		///    through the frames.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="ident" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		public IEnumerable<Frame> GetFrames (ByteVector ident)
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			
			if (ident.Count != 4)
				throw new ArgumentException (
					"Identifier must be four bytes long.",
					"ident");
			
			foreach (Frame f in frame_list)
				if (f.FrameId.Equals (ident))
					yield return f;
		}
		
		/// <summary>
		///    Gets all frames with of a specified type contained in
		///    the current instance.
		/// </summary>
		/// <typeparam name="T">
		///    The type of object, derived from <see cref="Frame" />,
		///    to return from in the current instance.
		/// </typeparam>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating
		///    through the frames.
		/// </returns>
		public IEnumerable<T> GetFrames <T> () where T : Frame
		{
			foreach (Frame f in frame_list) {
				T tf = f as T;
				if (tf != null)
					yield return tf;
			}
		}
		
		/// <summary>
		///    Gets all frames with a of type <typeparamref name="T" />
		///    with a specified identifier contained in the current
		///    instance.
		/// </summary>
		/// <typeparam name="T">
		///    The type of object, derived from <see cref="Frame" />,
		///    to return from in the current instance.
		/// </typeparam>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier of the frames to return.
		/// </param>
		/// <returns>
		///    A <see cref="T:System.Collections.Generic.IEnumerable`1" /> object enumerating
		///    through the frames.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="ident" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		public IEnumerable<T> GetFrames <T> (ByteVector ident)
			where T : Frame
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			
			if (ident.Count != 4)
				throw new ArgumentException (
					"Identifier must be four bytes long.",
					"ident");
			
			foreach (Frame f in frame_list) {
				T tf = f as T;
				if (tf != null && f.FrameId.Equals (ident))
					yield return tf;
			}
		}
		
		/// <summary>
		///    Adds a frame to the current instance.
		/// </summary>
		/// <param name="frame">
		///    A <see cref="Frame" /> object to add to the current
		///    instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="frame" /> is <see langref="null" />.
		/// </exception>
		public void AddFrame (Frame frame)
		{
			if (frame == null)
				throw new ArgumentNullException ("frame");
			
			frame_list.Add (frame);
		}
		
		/// <summary>
		///    Replaces an existing frame with a new one in the list
		///    contained in the current instance, or adds a new one if
		///    the existing one is not contained.
		/// </summary>
		/// <param name="oldFrame">
		///    A <see cref="Frame" /> object to be replaced.
		/// </param>
		/// <param name="newFrame">
		///    A <see cref="Frame" /> object to add to the current
		///    instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="oldFrame" /> or <paramref name="newFrame"
		///    /> is <see langref="null" />.
		/// </exception>
		public void ReplaceFrame (Frame oldFrame, Frame newFrame)
		{
			if (oldFrame == null)
				throw new ArgumentNullException ("oldFrame");
			
			if (newFrame == null)
				throw new ArgumentNullException ("newFrame");
			
			if (oldFrame == newFrame)
				return;
			
			int i = frame_list.IndexOf (oldFrame);
			if (i >= 0)
				frame_list [i] = newFrame;
			else
				frame_list.Add (newFrame);
		}
		
		/// <summary>
		///    Removes a specified frame from the current instance.
		/// </summary>
		/// <param name="frame">
		///    A <see cref="Frame" /> object to remove from the current
		///    instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="frame" /> is <see langref="null" />.
		/// </exception>
		public void RemoveFrame (Frame frame)
		{
			if (frame == null)
				throw new ArgumentNullException ("frame");
			
			if (frame_list.Contains (frame))
				frame_list.Remove (frame);
		}
		
		/// <summary>
		///    Removes all frames with a specified identifier from the
		///    current instance.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier of the frames to remove.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="ident" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		public void RemoveFrames (ByteVector ident)
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			
			if (ident.Count != 4)
				throw new ArgumentException (
					"Identifier must be four bytes long.",
					"ident");
			
			for (int i = frame_list.Count - 1; i >= 0; i --)
				if (frame_list [i].FrameId.Equals (ident))
					frame_list.RemoveAt (i);
		}
		
		/// <summary>
		///    Sets the text for a specified Text Information Frame.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier of the frame to set the data for.
		/// </param>
		/// <param name="text">
		///    A <see cref="string[]" /> containing the text to set for
		///    the specified frame, or <see langword="null" /> to unset
		///    the value.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="ident" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		public void SetTextFrame (ByteVector ident,
		                          params string [] text)
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			
			if (ident.Count != 4)
				throw new ArgumentException (
					"Identifier must be four bytes long.",
					"ident");
			
			bool empty = true;
			
			if (text != null)
				for (int i = 0; empty && i < text.Length; i ++)
					if (!string.IsNullOrEmpty (text [i]))
						empty = false;
			
			if (empty) {
				RemoveFrames (ident);
				return;
			}
			
			TextInformationFrame frame =
				TextInformationFrame.Get (this, ident, true);
			
			frame.Text = text;
			frame.TextEncoding = DefaultEncoding;
		}
		
		/// <summary>
		///    Sets the text for a specified Text Information Frame.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier of the frame to set the data for.
		/// </param>
		/// <param name="text">
		///    A <see cref="StringCollection" /> object containing the
		///    text to set for the specified frame, or <see
		///    langword="null" /> to unset the value.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="ident" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		[Obsolete("Use SetTextFrame(ByteVector,String[])")]
		public void SetTextFrame (ByteVector ident,
		                          StringCollection text)
		{
			if (text == null || text.Count == 0)
				RemoveFrames (ident);
			else
				SetTextFrame (ident, text.ToArray ());
		}
		
		/// <summary>
		///    Sets the numeric values for a specified Text Information
		///    Frame.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the
		///    identifier of the frame to set the data for.
		/// </param>
		/// <param name="number">
		///    A <see cref="uint" /> value containing the number to
		///    store.
		/// </param>
		/// <param name="count">
		///    A <see cref="uint" /> value representing a total which
		///    <paramref name="number" /> is a part of, or zero if
		///    <paramref name="number" /> is not part of a set.
		/// </param>
		/// <remarks>
		///    If both <paramref name="number" /> and <paramref
		///    name="count" /> are equal to zero, the value will be
		///    cleared. If <paramref name="count" /> is zero, <paramref
		///    name="number" /> by itself will be stored. Otherwise, the
		///    values will be stored as "<paramref name="number"
		///    />/<paramref name="count" />".
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="ident" /> is <see langref="null" />.
		/// </exception>
		/// <exception cref="ArgumentException">
		///    <paramref name="ident" /> is not exactly four bytes long.
		/// </exception>
		public void SetNumberFrame (ByteVector ident, uint number,
		                            uint count)
		{
			if (ident == null)
				throw new ArgumentNullException ("ident");
			
			if (ident.Count != 4)
				throw new ArgumentException (
					"Identifier must be four bytes long.",
					"ident");
			
			if (number == 0 && count == 0) {
				RemoveFrames (ident);
			} else if (count != 0) {
				SetTextFrame (ident, string.Format (
					CultureInfo.InvariantCulture, "{0}/{1}",
					number, count));
			} else {
				SetTextFrame (ident, number.ToString (
					CultureInfo.InvariantCulture));
			}
		}
		
		/// <summary>
		///    Renders the current instance as a raw ID3v2 tag.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered tag.
		/// </returns>
		/// <remarks>
		///    By default, tags will be rendered in the version they
		///    were loaded in, and new tags using the version specified
		///    by <see cref="DefaultVersion" />. If <see
		///    cref="ForceDefaultVersion" /> is <see langword="true" />,
		///    all tags will be rendered in using the version specified
		///    by <see cref="DefaultVersion" />, except for tags with
		///    footers, which must be in version 4.
		/// </remarks>
		public ByteVector Render ()
		{
			// We need to render the "tag data" first so that we
			// have to correct size to render in the tag's header.
			// The "tag data" (everything that is included in
			// Header.TagSize) includes the extended header, frames
			// and padding, but does not include the tag's header or
			// footer.
			
			bool has_footer = (header.Flags &
				HeaderFlags.FooterPresent) != 0;
			
			header.MajorVersion = has_footer ? (byte) 4 : Version;
			
			ByteVector tag_data = new ByteVector ();
			
			// TODO: Render the extended header.
			header.Flags &= ~HeaderFlags.ExtendedHeader;
			
			// Loop through the frames rendering them and adding
			// them to the tag_data.
			foreach (Frame frame in frame_list) {
				if ((frame.Flags &
					FrameFlags.TagAlterPreservation) != 0)
					continue;
				
				try {
					tag_data.Add (frame.Render (
						header.MajorVersion));
				} catch (NotImplementedException) {
				}
			}
			
			// Add unsyncronization bytes if necessary.
			if ((header.Flags & HeaderFlags.Unsynchronisation) != 0)
				SynchData.UnsynchByteVector (tag_data);
			
			// Compute the amount of padding, and append that to
			// tag_data.
			
			
			if (!has_footer)
				tag_data.Add (new ByteVector ((int)
					((tag_data.Count < header.TagSize) ? 
					(header.TagSize - tag_data.Count) :
					1024)));
			
			// Set the tag size.
			header.TagSize = (uint) tag_data.Count;
			
			tag_data.Insert (0, header.Render ());
			if (has_footer)
				tag_data.Add (new Footer (header).Render ());
			
			return tag_data;
		}
		
#endregion
		
		
		
#region Public Properties
		
		/// <summary>
		///    Gets and sets the header flags applied to the current
		///    instance.
		/// </summary>
		/// <value>
		///    A bitwise combined <see cref="HeaderFlags" /> value
		///    containing flags applied to the current instance.
		/// </value>
		public HeaderFlags Flags {
			get {return header.Flags;}
			set {header.Flags = value;}
		}
		
		/// <summary>
		///    Gets and sets the ID3v2 version of the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> value specifying the ID3v2 version
		///    of the current instance.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="value" /> is less than 2 or more than 4.
		/// </exception>
		public byte Version {
			get {
				return ForceDefaultVersion ?
					DefaultVersion : header.MajorVersion;
			}
			set {
				if (value < 2 || value > 4)
					throw new ArgumentOutOfRangeException (
						"value",
						"Version must be 2, 3, or 4");
				
				header.MajorVersion = value;
			}
		}
		
		#endregion
		
		
		
		#region Public Static Properties
		
		/// <summary>
		///    Gets and sets the ISO-639-2 language code to use when
		///    searching for and storing language specific values.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing an ISO-639-2
		///    language code fto use when searching for and storing
		///    language specific values.
		/// </value>
		/// <remarks>
		///    If the language is unknown, "XXX" is the appropriate
		///    filler.
		/// </remarks>
		public static string Language {
			get {return language;}
			set {
				language = (value == null || value.Length < 3) ?
					"XXX" : value.Substring (0,3);
			}
		}
		
		/// <summary>
		///    Gets and sets the the default version to use when
		///    creating new tags.
		/// </summary>
		/// <value>
		///    A <see cref="byte" /> value specifying the default ID3v2
		///    version.
		/// </value>
		/// <remarks>
		///    If <see cref="ForceDefaultVersion" /> is <see
		///    langword="true" />, all tags will be rendered with this
		///    version.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="value" /> is less than 2 or more than 4.
		/// </exception>
		public static byte DefaultVersion {
			get {return default_version;}
			set {
				if (value < 2 || value > 4)
					throw new ArgumentOutOfRangeException (
						"value",
						"Version must be 2, 3, or 4");
				
				default_version = value;
			}
		}
		
		public static bool ForceDefaultVersion {
			get {return force_default_version;}
			set {force_default_version = value;}
		}
		
		public static StringType DefaultEncoding {
			get {return default_string_type;}
			set {default_string_type = value;}
		}
		
		public static bool ForceDefaultEncoding {
			get {return force_default_string_type;}
			set {force_default_string_type = value;}
		}
		
		/// <summary>
		///    Gets and sets whether or not to use ID3v1 style numeric
		///    genres when possible.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> value specifying whether or not to
		///    use genres with numeric values when possible.
		/// </value>
		/// <remarks>
		///    If <see langword="true" />, TagLib# will try looking up
		///    the numeric genre code when storing the value. For
		///    ID3v2.2 and ID3v2.3, "Rock" would be stored as "(17)" and
		///    for ID3v2.4 it would be stored as "17".
		/// </remarks>
		public static bool UseNumericGenres {
			get {return use_numeric_genres;}
			set {use_numeric_genres = value;}
		}
		
#endregion
		
		
		
#region Protected Methods
		
		/// <summary>
		///    Populates the current instance be reading in a tag from
		///    a specified position in a specified file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="File" /> object to read the tag from.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specifying the seek position
		///    at which to read the tag.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception name="ArgumentOutOfRangeException">
		///    <paramref name="position" /> is less than 0 or greater
		///    than the size of the file.
		/// </exception>
		protected void Read (File file, long position)
		{
			if (file == null)
				throw new ArgumentNullException ("file");
			
			file.Mode = File.AccessMode.Read;
			
			if (position < 0 || position > file.Length - Header.Size)
				throw new ArgumentOutOfRangeException (
					"position");
			
			file.Seek (position);
			
			header = new Header (file.ReadBlock ((int) Header.Size));
			
			// If the tag size is 0, then this is an invalid tag.
			// Tags must contain at least one frame.
			
			if(header.TagSize == 0)
				return;
			
			Parse (file.ReadBlock ((int) header.TagSize));
		}
		
		/// <summary>
		///    Populates the current instance by parsing the contents of
		///    a raw ID3v2 tag, minus the header.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the content
		///    of an ID3v2 tag, minus the header.
		/// </param>
		/// <remarks>
		///    This method must only be called after the internal
		///    header has been read from the file, otherwise the data
		///    cannot be parsed correctly.
		/// </remarks>
		protected void Parse (ByteVector data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			
			// If the entire tag is marked as unsynchronized,
			// resynchronize it.
			
			if ((header.Flags & HeaderFlags.Unsynchronisation) != 0)
				SynchData.ResynchByteVector (data);
			
			int frame_data_position = 0;
			int frame_data_length = data.Count;
			
			// Check for the extended header.
			
			if ((header.Flags & HeaderFlags.ExtendedHeader) != 0) {
				extended_header = new ExtendedHeader (data,
					header.MajorVersion);
				
				if (extended_header.Size <= data.Count) {
					frame_data_position += (int)
						extended_header.Size;
					frame_data_length -= (int)
						extended_header.Size;
				}
			}
			
			// Parse the frames. TDRC, TDAT, and TIME will be needed
			// for post-processing, so check for them as they are
			// loaded.
			TextInformationFrame tdrc = null;
			TextInformationFrame tdat = null;
			TextInformationFrame time = null;
			
			while (frame_data_position < frame_data_length -
				FrameHeader.Size (header.MajorVersion)) {
				
				// If the next data is position is 0, assume
				// that we've hit the padding portion of the
				// frame data.
				if(data [frame_data_position] == 0)
					break;
				
				Frame frame = null;
				
				try {
					frame = FrameFactory.CreateFrame (data,
						ref frame_data_position,
						header.MajorVersion);
				} catch (NotImplementedException) {
					continue;
				}
				
				if(frame == null)
					break;
				
				// Only add frames that contain data.
				if (frame.Size == 0)
					continue;
				
				AddFrame (frame);
				
				// If the tag is version 4, no post-processing
				// is needed.
				if (header.MajorVersion == 4)
					continue;
					
				// Load up the first instance of each, for
				// post-processing.
				
				if (tdrc == null &&
					frame.FrameId.Equals (FrameType.TDRC)) {
					tdrc = frame as TextInformationFrame;
				} else if (tdat == null &&
					frame.FrameId.Equals (FrameType.TDAT)) {
					tdat = frame as TextInformationFrame;
				} else if (time == null &&
					frame.FrameId.Equals (FrameType.TIME)) {
					time = frame as TextInformationFrame;
				}
			}
			
			// Post-processing: Combine the three frames into one
			// TDRC frame.
			if (tdrc == null || tdat == null) {
				if (tdat != null)
					RemoveFrames (FrameType.TDAT);
				
				if (time != null)
					RemoveFrames (FrameType.TIME);
				
				return;
			}
			
			StringBuilder tdrc_text = new StringBuilder (
				tdrc.ToString ());
			
			string tdat_text = tdat.ToString ();
			
			if (tdrc_text.Length != 4 || tdat_text.Length != 4) {
				RemoveFrames (FrameType.TDAT);
				
				if (time != null)
					RemoveFrames (FrameType.TIME);
				
				return;
			}
			
			tdrc_text.Append ("-").Append (tdat_text, 0, 2)
				.Append ("-").Append (tdat_text, 2, 2);
			
			RemoveFrames (FrameType.TDAT);
			
			if (time == null) {
				tdrc.Text = new string [] {tdrc_text.ToString ()};
				return;
			}
			
			string time_text = time.ToString ();
				
			if (time_text.Length == 4)
				tdrc_text.Append ("T").Append (time_text, 0, 2)
					.Append (":").Append (time_text, 2, 2);
					
			tdrc.Text = new string [] {tdrc_text.ToString ()};
			
			RemoveFrames (FrameType.TIME);
		}
		
#endregion
		
		
		
#region Private Methods
		
		// TODO: These should become public some day.
		
		/// <summary>
		///    Gets the text value from a specified Text Information
		///    Frame.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the frame
		///    identifier of the Text Information Frame to get the value
		///    from.
		/// </param>
		/// <returns>
		///    A <see cref="string" /> object containing the text of the
		///    specified frame, or <see langref="null" /> if no value
		///    was found.
		/// </returns>
		private string GetTextAsString (ByteVector ident)
		{
			TextInformationFrame frame = TextInformationFrame.Get (
				this, ident, false);
			
			string result = frame == null ? null : frame.ToString ();
			return string.IsNullOrEmpty (result) ? null : result;
		}
		
		/// <summary>
		///    Gets the text values from a specified Text Information
		///    Frame.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the frame
		///    identifier of the Text Information Frame to get the value
		///    from.
		/// </param>
		/// <returns>
		///    A <see cref="string[]" /> containing the text of the
		///    specified frame, or an empty array if no values were
		///    found.
		/// </returns>
		private string [] GetTextAsArray (ByteVector ident)
		{
			TextInformationFrame frame = TextInformationFrame.Get (
				this, ident, false);
			return frame == null ? new string [0] : frame.Text;
		}
		
		/// <summary>
		///    Gets an integer value from a "/" delimited list in a
		///    specified Text Information Frame.
		/// </summary>
		/// <param name="ident">
		///    A <see cref="ByteVector" /> object containing the frame
		///    identifier of the Text Information Frame to read from.
		/// </param>
		/// <param name="index">
		///    A <see cref="int" /> value specifying the index in the
		///    integer list of the value to return.
		/// </param>
		/// <returns>
		///    A <see cref="uint" /> value read from the list in the
		///    frame, or 0 if the value wasn't found.
		/// </returns>
		private uint GetTextAsUInt32 (ByteVector ident, int index)
		{
			string text = GetTextAsString (ident);
			
			if (text == null)
				return 0;
			
			string [] values = text.Split (new char [] {'/'},
				index + 2);
			
			if (values.Length < index + 1)
				return 0;
			
			uint result;
			if (uint.TryParse (values [index], out result))
				return result;
			
			return 0;
		}
		
		#endregion
		
		
		
		#region IEnumerable
		
		public IEnumerator<Frame> GetEnumerator ()
		{
			return frame_list.GetEnumerator ();
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return frame_list.GetEnumerator ();
		}
		
#endregion
		
		
		
#region TagLib.Tag
		
		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.Id3v2" />.
		/// </value>
		public override TagTypes TagTypes {
			get {return TagTypes.Id3v2;}
		}
		
		/// <summary>
		///    Gets and sets the title for the media described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the title for
		///    the media described by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TIT2" Text
		///    Information Frame.
		/// </remarks>
		public override string Title {
			get {return GetTextAsString (FrameType.TIT2);}
			set {SetTextFrame (FrameType.TIT2, value);}
		}
		
		/// <summary>
		///    Gets and sets the performers or artists who performed in
		///    the media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the performers or
		///    artists who performed in the media described by the
		///    current instance or an empty array if no value is
		///    present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TPE1" Text
		///    Information Frame.
		/// </remarks>
		public override string [] Performers {
			get {return GetTextAsArray (FrameType.TPE1);}
			set {SetTextFrame (FrameType.TPE1, value);}
		}
		
		/// <summary>
		///    Gets and sets the band or artist who is credited in the
		///    creation of the entire album or collection containing the
		///    media described by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the band or artist
		///    who is credited in the creation of the entire album or
		///    collection containing the media described by the current
		///    instance or an empty array if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TPE2" Text
		///    Information Frame.
		/// </remarks>
		public override string [] AlbumArtists {
			get {return GetTextAsArray (FrameType.TPE2);}
			set {SetTextFrame (FrameType.TPE2, value);}
		}
		
		/// <summary>
		///    Gets and sets the composers of the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the composers of the
		///    media represented by the current instance or an empty
		///    array if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TCOM" Text
		///    Information Frame.
		/// </remarks>
		public override string [] Composers {
			get {return GetTextAsArray (FrameType.TCOM);}
			set {SetTextFrame (FrameType.TCOM, value);}
		}
		
		/// <summary>
		///    Gets and sets the album of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the album of
		///    the media represented by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TALB" Text
		///    Information Frame.
		/// </remarks>
		public override string Album {
			get {return GetTextAsString (FrameType.TALB);}
			set {SetTextFrame (FrameType.TALB, value);}
		}
		
		/// <summary>
		///    Gets and sets a user comment on the media represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing user comments
		///    on the media represented by the current instance or <see
		///    langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "COMM" Comments
		///    Frame with an empty description and the language
		///    specified by <see cref="Language" />.
		/// </remarks>
		public override string Comment {
			get {
				CommentsFrame f =
					CommentsFrame.GetPreferred (this,
						String.Empty, Language);
				return f != null ? f.ToString () : null;
			}
			set {
				CommentsFrame frame;
				
				if (string.IsNullOrEmpty (value)) {
					while ((frame = CommentsFrame
						.GetPreferred (this,
							string.Empty,
							Language)) != null)
						RemoveFrame (frame);
					
					return;
				}
				
				frame = CommentsFrame.Get (this, String.Empty,
					Language, true);
				
				frame.Text = value;
				frame.TextEncoding = DefaultEncoding;
			}
		}
		
		/// <summary>
		///    Gets and sets the genres of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the genres of the
		///    media represented by the current instance or an empty
		///    array if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TCON" Text
		///    Information Frame.
		/// </remarks>
		public override string [] Genres {
			get {
				string [] text = GetTextAsArray (FrameType.TCON);
				
				if (text.Length == 0)
					return text;
				
				List<string> list = new List<string> ();
				
				foreach (string genre in text) {
					if (string.IsNullOrEmpty (genre))
						continue;
					
					// The string may just be a genre
					// number.
					
					string genre_from_index =
						TagLib.Genres.IndexToAudio (
							genre);
					
					if (genre_from_index != null)
						list.Add (genre_from_index);
					else
						list.Add (genre);
				}
				
				return list.ToArray ();
			}
			set {
				if (value == null || !use_numeric_genres) {
					SetTextFrame (FrameType.TCON, value);
					return;
				}
				
				// Clone the array so changes made won't effect
				// the passed array.
				value = (string []) value.Clone ();
				
				for (int i = 0; i < value.Length; i ++) {
					int index = TagLib.Genres.AudioToIndex (
						value [i]);
					
					if (index != 255)
						value [i] = index.ToString (
							CultureInfo.InvariantCulture);
				}
				
				SetTextFrame (FrameType.TCON, value);
			}
		}
		
		/// <summary>
		///    Gets and sets the year that the media represented by the
		///    current instance was recorded.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the year that the media
		///    represented by the current instance was created or zero
		///    if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TDRC" Text
		///    Information Frame. If a value greater than 9999 is set,
		///    this property will be cleared.
		/// </remarks>
		public override uint Year {
			get {
				string text = GetTextAsString (FrameType.TDRC);
				
				if (text == null || text.Length < 4)
					return 0;
				
				uint value;
				if (uint.TryParse (text.Substring (0, 4),
					out value))
					return value;
				
				return 0;
			}
			set {
				if (value > 9999)
					value = 0;
				
				SetNumberFrame (FrameType.TDRC, value, 0);
			}
		}
		
		/// <summary>
		///    Gets and sets the position of the media represented by
		///    the current instance in its containing album.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the position of the
		///    media represented by the current instance in its
		///    containing album or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TRCK" Text
		///    Information Frame.
		/// </remarks>
		public override uint Track {
			get {return GetTextAsUInt32 (FrameType.TRCK, 0);}
			set {SetNumberFrame (FrameType.TRCK, value, TrackCount);}
		}
		
		/// <summary>
		///    Gets and sets the number of tracks in the album
		///    containing the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of tracks in
		///    the album containing the media represented by the current
		///    instance or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TRCK" Text
		///    Information Frame.
		/// </remarks>
		public override uint TrackCount {
			get {return GetTextAsUInt32 (FrameType.TRCK, 1);}
			set {SetNumberFrame (FrameType.TRCK, Track, value);}
		}
		
		/// <summary>
		///    Gets and sets the number of the disc containing the media
		///    represented by the current instance in the boxed set.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of the disc
		///    containing the media represented by the current instance
		///    in the boxed set.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TPOS" Text
		///    Information Frame.
		/// </remarks>
		public override uint Disc {
			get {return GetTextAsUInt32 (FrameType.TPOS, 0);}
			set {SetNumberFrame (FrameType.TPOS, value, DiscCount);}
		}
		
		/// <summary>
		///    Gets and sets the number of discs in the boxed set
		///    containing the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of discs in
		///    the boxed set containing the media represented by the
		///    current instance or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TPOS" Text
		///    Information Frame.
		/// </remarks>
		public override uint DiscCount {
			get {return GetTextAsUInt32 (FrameType.TPOS, 1);}
			set {SetNumberFrame (FrameType.TPOS, Disc, value);}
		}
		
		/// <summary>
		///    Gets and sets the lyrics or script of the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the lyrics or
		///    script of the media represented by the current instance
		///    or <see langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "USLT"
		///    Unsynchronized Lyrics Frame with an empty description and
		///    the language specified by <see cref="Language" />.
		/// </remarks>
		public override string Lyrics {
			get {
				UnsynchronisedLyricsFrame f =
					UnsynchronisedLyricsFrame.GetPreferred (
						this, string.Empty, Language);
				
				return f != null ? f.ToString () : null;
			}
			set {
				UnsynchronisedLyricsFrame frame;
				
				if (string.IsNullOrEmpty (value)) {
					while ((frame = UnsynchronisedLyricsFrame
						.GetPreferred (this,
							string.Empty,
							Language)) != null)
						RemoveFrame (frame);
					
					return;
				}
				
				frame = UnsynchronisedLyricsFrame.Get (this,
						String.Empty, Language, true);
				
				frame.Text = value;
				frame.TextEncoding = DefaultEncoding;
			}
		}
		
		/// <summary>
		///    Gets and sets the grouping on the album which the media
		///    in the current instance belongs to.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the grouping on
		///    the album which the media in the current instance belongs
		///    to or <see langword="null" /> if no value is present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TIT1" Text
		///    Information Frame.
		/// </remarks>
		public override string Grouping {
			get {return GetTextAsString (FrameType.TIT1);}
			set {SetTextFrame (FrameType.TIT1, value);}
		}
		
		/// <summary>
		///    Gets and sets the number of beats per minute in the audio
		///    of the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> containing the number of beats per
		///    minute in the audio of the media represented by the
		///    current instance, or zero if not specified.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TBPM" Text
		///    Information Frame.
		/// </remarks>
		public override uint BeatsPerMinute {
			get {
				string text = GetTextAsString (FrameType.TBPM);
				
				if (text == null)
					return 0;
				
				double result;
				if (double.TryParse (text, out result) &&
					result >= 0.0)
					return (uint) Math.Round (result);
				
				return 0;
			}
			set {SetNumberFrame (FrameType.TBPM, value, 0);}
		}
		
		/// <summary>
		///    Gets and sets the conductor or director of the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the conductor
		///    or director of the media represented by the current
		///    instance or <see langref="null" /> if no value present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TPE3" Text
		///    Information Frame.
		/// </remarks>
		public override string Conductor {
			get {return GetTextAsString (FrameType.TPE3);}
			set {SetTextFrame (FrameType.TPE3, value);}
		}
		
		/// <summary>
		///    Gets and sets the copyright information for the media
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the copyright
		///    information for the media represented by the current
		///    instance or <see langword="null" /> if no value present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TCOP" Text
		///    Information Frame.
		/// </remarks>
		public override string Copyright {
			get {return GetTextAsString (FrameType.TCOP);}
			set {SetTextFrame (FrameType.TCOP, value);}
		}
		
		/// <summary>
		///    Gets and sets a collection of pictures associated with
		///    the media represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="IPicture[]" /> containing a collection of
		///    pictures associated with the media represented by the
		///    current instance or an empty array if none are present.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "APIC" Attached
		///    Picture Frame.
		/// </remarks>
		public override IPicture [] Pictures {
			get {
				return new List<AttachedPictureFrame> (
					GetFrames <AttachedPictureFrame> (
						FrameType.APIC)).ToArray ();
			}
			set {
				RemoveFrames(FrameType.APIC);
				
				if(value == null || value.Length == 0)
					return;
				
				foreach(IPicture picture in value) {
					AttachedPictureFrame frame =
						picture as AttachedPictureFrame;
					
					if (frame == null)
						frame = new AttachedPictureFrame (
							picture);
					
					AddFrame (frame);
				}
			}
		}
		
		/// <summary>
		///    Gets whether or not the current instance is empty.
		/// </summary>
		/// <value>
		///    <see langword="true" /> if the current instance does not
		///    any values. Otherwise <see langword="false" />.
		/// </value>
		public override bool IsEmpty {
			get {return frame_list.Count == 0;}
		}
		
		/// <summary>
		///    Clears the values stored in the current instance.
		/// </summary>
		public override void Clear ()
		{
			frame_list.Clear ();
		}
		
		/// <summary>
		///    Gets and sets whether or not the album described by the
		///    current instance is a compilation.
		/// </summary>
		/// <value>
		///    A <see cref="bool" /> value indicating whether or not the
		///    album described by the current instance is a compilation.
		/// </value>
		/// <remarks>
		///    This property is implemented using the "TCMP" Text
		///    Information Frame to provide support for a feature of the
		///    Apple iPod and iTunes products.
		/// </remarks>
		public bool IsCompilation {
			get {
				string val = GetTextAsString (FrameType.TCMP);
				return !string.IsNullOrEmpty (val) && val != "0";
			}
			set {SetTextFrame (FrameType.TCMP, value ? "1" : null);}
		}
		
		/// <summary>
		///    Copies the values from the current instance to another
		///    <see cref="TagLib.Tag" />, optionally overwriting
		///    existing values.
		/// </summary>
		/// <param name="target">
		///    A <see cref="TagLib.Tag" /> object containing the target
		///    tag to copy values to.
		/// </param>
		/// <param name="overwrite">
		///    A <see cref="bool" /> specifying whether or not to copy
		///    values over existing one.
		/// </param>
		/// <remarks>
		///    <para>If <paramref name="target" /> is of type <see
		///    cref="TagLib.Ape.Tag" /> a complete copy of all values
		///    will be performed. Otherwise, only standard values will
		///    be copied.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="target" /> is <see langword="null" />.
		/// </exception>
		public override void CopyTo (TagLib.Tag target, bool overwrite)
		{
			if (target == null)
				throw new ArgumentNullException ("target");
			
			TagLib.Id3v2.Tag match = target as TagLib.Id3v2.Tag;
			
			if (match == null) {
				base.CopyTo (target, overwrite);
				return;
			}
			
			List<Frame> frames = new List<Frame> (frame_list);
			while (frames.Count > 0) {
				ByteVector ident = frames [0].FrameId;
				bool copy = true;
				if (overwrite) {
					match.RemoveFrames (ident);
				} else {
					foreach (Frame f in match.frame_list)
						if (f.FrameId.Equals (ident)) {
							copy = false;
							break;
						}
				}
				
				for (int i = 0; i < frames.Count;) {
					if (frames [i].FrameId.Equals (ident)) {
						if (copy)
							match.frame_list.Add (
								frames [i].Clone ());
						
						frames.RemoveAt (i);
					} else {
						i ++;
					}
				}
			}
		}
		
#endregion
	}
}
