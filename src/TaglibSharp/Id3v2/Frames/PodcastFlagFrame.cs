//
// PodcastFlagFrame.cs:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace TagLib.Id3v2
{
	/// <summary>
	///    This class extends <see cref="Frame" />, implementing support for
	///    Podcast Flag (PCST) Frames.
	/// </summary>
	/// <example>
	///    <para>Getting and setting the podcast flag of a file.</para>
	///    <code lang="C#">
	/// using TagLib;
	/// using TagLib.Id3v2;
	///
	/// public static class TrackUtil
	/// {
	/// 	public static bool GetPodcastFlag (string filename)
	/// 	{
	/// 		File file = File.Create (filename, ReadStyle.None);
	/// 		Id3v2.Tag tag = file.GetTag (TagTypes.Id3v2, false) as Id3v2.Tag;
	/// 		if (tag == null)
	/// 			return false;
	/// 		
	/// 		PodcastFlagFrame frame = PodcastFlagFrame.Get (tag, false);
	/// 		if (frame == null)
	/// 			return false;
	///
	/// 		return true;
	/// 	}
	/// 	
	/// 	public static void SetPodcastFlag (string filename)
	/// 	{
	/// 		File file = File.Create (filename, ReadStyle.None);
	/// 		Id3v2.Tag tag = file.GetTag (TagTypes.Id3v2, true) as Id3v2.Tag;
	/// 		if (tag == null)
	/// 			return;
	/// 		
	/// 		PodcastFlagFrame.Get (tag, true);
	/// 		file.Save ();
	/// 	}
	/// }
	///    </code>
	///    <code lang="C++">
	/// #using &lt;System.dll>
	/// #using &lt;taglib-sharp.dll>
	///
	/// using System;
	/// using TagLib;
	/// using TagLib::Id3v2;
	///
	/// public ref class TrackUtil abstract sealed
	/// {
	/// public:
	/// 	static bool GetPodcastFlag (String^ filename)
	/// 	{
	/// 		File^ file = File.Create (filename, ReadStyle.None);
	/// 		Id3v2::Tag^ tag = dynamic_cast&lt;Id3v2::Tag^> (file.GetTag (TagTypes::Id3v2, false));
	/// 		if (tag == null)
	/// 			return false;
	/// 		
	/// 		PodcastFlagFrame^ frame = PodcastFlagFrame::Get (tag, false);
	/// 		if (frame == null)
	/// 			return false;
	///
	/// 		return true;
	/// 	}
	/// 	
	/// 	static void SetPodcastFlag (String^ filename)
	/// 	{
	/// 		File^ file = File::Create (filename, ReadStyle::None);
	/// 		Id3v2.Tag^ tag = dynamic_cast&lt;Id3v2::Tag^> (file.GetTag (TagTypes::Id3v2, true));
	/// 		if (tag == null)
	/// 			return;
	/// 		
	/// 		PodcastFlagFrame::Get (tag, true);
	/// 		file->Save ();
	/// 	}
	/// }
	///    </code>
	/// </example>
	public class PodcastFlagFrame : Frame
	{
		#region Constants

		/// <summary>
		/// The PodcastFlagFrame data is simply an array of 4 null bytes.
		/// </summary>
		private readonly static ReadOnlyByteVector ExpectedData = new ReadOnlyByteVector(new byte[] { 0x0, 0x0, 0x0, 0x0 });

		#endregion

		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="PodcastFlagFrame" />.
		/// </summary>
		/// <remarks>
		///    When a frame is created, it is not automatically added to
		///    the tag. Consider using <see cref="Get" /> for more
		///    integrated frame creation.
		/// </remarks>
		public PodcastFlagFrame () : base (FrameType.PCST, 4)
		{
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="PodcastFlagFrame" /> by reading its raw data in a
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
		public PodcastFlagFrame (ByteVector data, byte version)
			: base (data, version)
		{
			SetData (data, 0, version, true);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="PodcastFlagFrame" /> by reading its raw data in a
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
		protected internal PodcastFlagFrame (ByteVector data, int offset, FrameHeader header, byte version)
			: base (header)
		{
			SetData (data, offset, version, false);
		}

        #endregion

		#region Public Static Methods

		/// <summary>
		///    Gets a podcast flag frame from a specified tag, optionally
		///    creating it if it does not exist.
		/// </summary>
		/// <param name="tag">
		///    A <see cref="Tag" /> object to search in.
		/// </param>
		/// <param name="create">
		///    A <see cref="bool" /> specifying whether or not to create
		///    and add a new frame to the tag if a match is not found.
		/// </param>
		/// <returns>
		///    A <see cref="PodcastFlagFrame" /> object containing the
		///    matching frame, or <see langword="null" /> if a match
		///    wasn't found and <paramref name="create" /> is <see
		///    langword="false" />.
		/// </returns>
		public static PodcastFlagFrame Get (Tag tag, bool create)
		{
			PodcastFlagFrame pcst;
			foreach (Frame frame in tag) {
				pcst = frame as PodcastFlagFrame;

				if (pcst != null)
					return pcst;
			}

			if (!create)
				return null;

			pcst = new PodcastFlagFrame ();
			tag.AddFrame (pcst);
			return pcst;
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
			if (data.CompareTo(ExpectedData) != 0)
				throw new CorruptFileException ("Podcast flag value is incorrect.");
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
			ByteVector data = new ByteVector(ExpectedData);
		
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
			Frame frame = new PodcastFlagFrame();

			return frame;
		}

		#endregion
	}
}