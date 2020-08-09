using System;
using System.Collections.Generic;

namespace TagLib.Id3v2
{
	/// <summary>
	///    This class extends <see cref="Frame" /> to provide support for
	///    Chapter Frames, i.e. "<c>CHAP</c>", (ID3v2 Chapter Frame Addendum 1.0,
	///    https://id3.org/id3v2-chapters-1.0).
	/// </summary>
	/// <remarks>
	///    The Chapter Frame is special in that it can hold an arbitrary amount
	///    of sub-frames, which are made available here in the SubFrames list.
	///
	///    Each Chapter Frame must have an identifying string that is unique across
	///    all <see cref="ChapterFrame"/>s and <see cref="TableOfContentsFrame"/>s
	///    in the tag. This is the property <see cref="Id"/>. It is not intended
	///    for humans consumption and players will not display it. A chapter can
	///    be titled by adding a "<c>TIT2</c>" <see cref="TextInformationFrame"/>.
	///
	///    There are two ways the Chapter Frame can state a chapter’s beginning
	///    and end: by milliseconds or by byte offset, accessible here as
	///    StartMilliseconds/EndMilliseconds and StartByteOffset/EndByteOffset
	///    respectively. The byte offsets are the zero-based byte positions of
	///    the first audio frame in the chapter or the first audio frame folliwing
	///    the chapter, counted from the beginning of the file. The byte offsets
	///    are to be ignored according to the spec if they are FF FF FF FF. This
	///    class does not synchronize the two ways in any way, so make sure to set
	///    both appropriately. The byte offsets are however initialized to be
	///    ignored, so with blank frames, you can focus on the milliseconds.
	///
	///    According to the spec, chapters may overlap and have gaps.
	/// </remarks>
	public class ChapterFrame : Frame
	{
		#region Constructors

		/// <summary>
		///    Constructs and initializes a new empty instance of <see
		///    cref="ChapterFrame" />.
		/// </summary>
		public ChapterFrame ()
			: base(FrameType.CHAP, 4)
		{
		}

		/// <summary>
		///    Constructs and initializes a new empty instance of <see
		///    cref="ChapterFrame" /> with the given chapter ID.
		/// </summary>
		public ChapterFrame (string id)
			: this()
		{
			Id = id;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see cref="ChapterFrame" />
		///    with the given chapter ID and adds a <see cref="TextInformationFrame"/>
		///    "<c>TIT2</c>" with the given title.
		/// </summary>
		public ChapterFrame (string id, string title)
			: this(id)
		{
			SubFrames.Add(new TextInformationFrame("TIT2") { Text = new[] { title } });
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ChapterFrame" /> by reading its raw data in a
		///    specified ID3v2 version.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object starting with the raw
		///    representation of the new frame.
		/// </param>
		/// <param name="version">
		///    A <see cref="byte" /> indicating the ID3v2 version the
		///    raw frame is encoded in.
		/// </param>
		public ChapterFrame (ByteVector data, byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ChapterFrame" /> by reading its raw data in a
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
		protected internal ChapterFrame (ByteVector data, int offset, FrameHeader header, byte version)
			: base (header)
		{
			SetData (data, offset, version, false);
		}

		#endregion


		#region Public Properties

		/// <summary>
		///    Gets and sets the internal chapter id. This should be
		///    <see cref="StringType.Latin1" /> .
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///    Gets and sets the start time of the chapter in milliseconds.
		/// </summary>
		public uint StartMilliseconds { get; set; }

		/// <summary>
		///    Gets and sets the end time of the chapter in milliseconds.
		/// </summary>
		public uint EndMilliseconds { get; set; }

		/// <summary>
		///    Gets and sets the chapter’s first audio frame’s byte position
		///    from the beginning of the file.
		///    The spec makes this ignorable if it is FF FF FF FF, which is
		///    the initial value.
		/// </summary>
		public uint StartByteOffset { get; set; } = 0xFFFFFFFF;

		/// <summary>
		///    Gets and sets the byte position of the first audio frame following
		///    the chapter from the beginning of the file.
		///    The spec makes this ignorable if it is FF FF FF FF, which is
		///    the initial value.
		/// </summary>
		public uint EndByteOffset { get; set; } = 0xFFFFFFFF;

		/// <summary>
		///    Gets and sets the descriptive sub-fields for this chapter. It
		///    is recommended by the spec to have at least a "<c>TIT2</c>"
		///    <see cref="TextInformationFrame"/> with the chapter title, but
		///    it can contain anything. Particularly, players like to display
		///    per-chapter "<c>APIC</c>" <see cref="AttachmentFrame"/>s and
		///    <see cref="UrlLinkFrame"/>s.
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

			Id                = data.ToString(StringType.Latin1, 0, idLength - 1); //Always Latin1, at least there is no mention of encoding in the spec
			StartMilliseconds = data.Mid(idLength,      4).ToUInt();
			EndMilliseconds   = data.Mid(idLength +  4, 4).ToUInt();
			StartByteOffset   = data.Mid(idLength +  8, 4).ToUInt(); //I don’t really know why one would use the offsets.
			EndByteOffset     = data.Mid(idLength + 12, 4).ToUInt(); //They are to be ignored if all 4 Bytes are FF, i.e. 4,294,967,295.

			SubFrames = new List<Frame>();
			int frame_data_position = idLength + 16;
			int frame_data_endposition = data.Count;
			while (frame_data_position < frame_data_endposition)
			{
				Frame frame;
				try
				{
					frame = FrameFactory.CreateFrame(data, null, ref frame_data_position, version, true /* ? */);
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
			data.Add((byte)0); //it would be neat if Add were chainable…
			data.Add(ByteVector.FromUInt(StartMilliseconds));
			data.Add(ByteVector.FromUInt(EndMilliseconds));
			data.Add(ByteVector.FromUInt(StartByteOffset));
			data.Add(ByteVector.FromUInt(EndByteOffset));

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
			var frame = new ChapterFrame(Id);
			frame.StartMilliseconds = StartMilliseconds;
			frame.EndMilliseconds = EndMilliseconds;
			frame.StartByteOffset = StartByteOffset;
			frame.EndByteOffset = EndByteOffset;

			foreach(var f in SubFrames)
				frame.SubFrames.Add(f.Clone());

			return frame;
		}

		#endregion
	}
}
