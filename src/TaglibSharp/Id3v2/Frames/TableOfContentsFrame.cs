using System;
using System.Collections.Generic;

namespace TagLib.Id3v2
{
	/// <summary>
	///    This class extends <see cref="Frame" /> to provide support for
	///    Table of Contents Frames, i.e. "<c>CTOC</c>",
	///    (ID3v2 Chapter Frame Addendum 1.0, https://id3.org/id3v2-chapters-1.0).
	/// </summary>
	/// <remarks>
	///    The <see cref="TableOfContentsFrame"/> is special in that it can hold
	///    an arbitrary amount of sub-frames, which are made available here as a
	///    List of <see cref="Frame"/>s in the property <see cref="SubFrames"/>.
	///    
	///    A tag may contain multiple <see cref="TableOfContentsFrame"/>s, there
	///    may however be only one top-level "<c>CTOC</c>" as stated by the
	///    property <see cref="IsTopLevel"/>.
	///
	///    Each <see cref="TableOfContentsFrame"/> must have an identifying string
	///    that is unique across all <see cref="TableOfContentsFrame"/>s and
	///    <see cref="ChapterFrame"/>s in the tag. This is the <see cref="Id"/>
	///    property. It is not intended for humans and players will not display it.
	///    For humans, add a "<c>TIT2</c>" <see cref="TextInformationFrame"/>.
	/// </remarks>
	public class TableOfContentsFrame : Frame
	{
		[System.Flags]
		private enum CTOCFlags : byte
		{
			TopLevel = 1,
			Ordered  = 2
		}


		#region Constructors

		/// <summary>
		///    Constructs and initializes a new empty instance of <see
		///    cref="TableOfContentsFrame" />.
		/// </summary>
		public TableOfContentsFrame ()
			: base (FrameType.CTOC, 4)
		{
		}

		/// <summary>
		///    Constructs and initializes a new empty instance of <see
		///    cref="TableOfContentsFrame" /> with the given TOC Id.
		/// </summary>
		public TableOfContentsFrame(string id)
			: this()
		{
			Id = id;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="TableOfContentsFrame" /> with the given TOC Id
		///    and adds a TIT2 <see cref="TextInformationFrame"/>
		///    with the given title.
		/// </summary>
		public TableOfContentsFrame(string id, string title)
			: this(id)
		{
			SubFrames.Add(new TextInformationFrame("TIT2") { Text = new[] { title } });
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="TableOfContentsFrame" /> by reading its raw data in
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
		public TableOfContentsFrame (ByteVector data, byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="TableOfContentsFrame" /> by reading its raw data in a
		///    specified ID3v2 version.
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
		protected internal TableOfContentsFrame (ByteVector data, int offset, FrameHeader header, byte version)
			: base (header)
		{
			SetData (data, offset, version, false);
		}

		#endregion


		#region Public Properties

		/// <summary>
		///    Gets and sets the internal table of contents id.
		///    This should be <see cref="StringType.Latin1" />
		///    and must be unique with respect to any other
		///    "<c>CTOC</c>" or "<c>CHAP</c>" frame in the tag.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///    Gets and sets the boolean stating that this is the root
		///    of all "<c>CTOC</c>"s. As such there must be only one
		///    "<c>CTOC</c>" with this set to true.
		/// </summary>
		public bool IsTopLevel { get; set; }

		/// <summary>
		///    Gets and sets the boolean stating that this table of
		///    contents’ chapters should be played in order.
		/// </summary>
		public bool IsOrdered { get; set; }

		/// <summary>
		///    Gets and sets the list of chapters in this table of contents
		///    identified by their <see cref="ChapterFrame.Id"/>s.
		///    Because the number of chapters is stored in this frame using
		///    only one byte for parsing purposes, this should not contain
		///    more than 255 chapters.
		/// </summary>
		public List<string> ChapterIds { get; set; } = new List<string>();

		/// <summary>
		///    Gets and sets the descriptive sub-fields for this chapter. It
		///    is recommended by the spec to have at least a "<c>TIT2</c>"
		///    <see cref="TextInformationFrame"/> with the chapter title, but
		///    it can contain anything.
		/// </summary>
		/// <value>
		///    A List of arbitrary <see cref="Frame" />s.
		/// </value>
		public List<Frame> SubFrames { get; set; } = new List<Frame>();

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
			// https://id3.org/id3v2-chapters-1.0

			int idLength = data.IndexOf((byte)0) + 1;
			Id  = data.ToString(StringType.Latin1, 0, idLength - 1);

			var flags = (CTOCFlags)data[idLength]; //Flags %000000ab Entry count $xx (8-bit unsigned int)
			IsTopLevel = flags.HasFlag(CTOCFlags.TopLevel); //a
			IsOrdered = flags.HasFlag(CTOCFlags.Ordered); //b

			var chapterCount = data[idLength + 1]; //Entry count $xx (8-bit unsigned int)

			if (data.Count <= idLength + 2)
				return; //no chapter ids and no subframes

			/* Get chapter ids according to the chapterCount byte.
			 * Anything left after that should be subframes.
			 * TODO: If there are more chapters than fit inside
			 * the chapterCount byte, I guess the file is malformed?
			 */
			int position = idLength + 2;
			for (int i = 0; i < chapterCount; i++)
			{
				int nextPosition = data.Find((byte)0, position) + 1;
				ChapterIds.Add(data.Mid(position, nextPosition-position-1).ToString(StringType.Latin1));
				position = nextPosition;
			}

			SubFrames = new List<Frame>();
			int frame_data_endposition = data.Count;
			while (position < frame_data_endposition)
			{
				Frame frame;
				try
				{
					frame = FrameFactory.CreateFrame(data, null, ref position, version, true /* ? */);
				}
				catch (NotImplementedException)
				{
					continue;
				}
				catch (CorruptFileException)
				{
					throw;
				}

				if (frame == null)
					break;

				// Only add frames that contain data.
				if (frame.Size == 0)
					continue;

				SubFrames.Add(frame);
			}
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
			var data = ByteVector.FromString(Id, StringType.Latin1);
			data.Add((byte)0);
			data.Add((byte)((IsTopLevel ? CTOCFlags.TopLevel : 0) | (IsOrdered ? CTOCFlags.Ordered : 0)));
			data.Add(ChapterIds.Count >= byte.MaxValue ? byte.MaxValue /*TODO: throw?*/ : (byte)ChapterIds.Count);

			foreach(var chap in ChapterIds)
			{
				data.Add(ByteVector.FromString(chap, StringType.Latin1));
				data.Add((byte)0);
			}

			foreach (var f in SubFrames)
				data.Add(f.Render(version));

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
		public override Frame Clone()
		{
			var frame = new TableOfContentsFrame(Id);
			frame.IsTopLevel = IsTopLevel;
			frame.IsOrdered = IsOrdered;

			foreach (var c in ChapterIds)
				frame.ChapterIds.Add(c);

			foreach (var f in SubFrames)
				frame.SubFrames.Add(f.Clone());

			return frame;
		}

		#endregion
	}
}
