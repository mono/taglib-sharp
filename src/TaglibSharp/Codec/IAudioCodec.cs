//
// IAudioCodec.cs
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace TagLib
{
	/// <summary>
	///    This interface inherits <see cref="ICodec" /> to provide
	///    information about an audio codec.
	/// </summary>
	/// <remarks>
	///    <para>When dealing with a <see cref="ICodec" />, if <see
	///    cref="ICodec.MediaTypes" /> contains <see cref="MediaTypes.Audio"
	///    />, it is safe to assume that the object also inherits <see
	///    cref="IAudioCodec" /> and can be recast without issue.</para>
	/// </remarks>
	public interface IAudioCodec : ICodec
	{
		/// <summary>
		///    Gets the bitrate of the audio represented by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing a bitrate of the
		///    audio represented by the current instance.
		/// </value>
		int AudioBitrate { get; }

		/// <summary>
		///    Gets the sample rate of the audio represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing the sample rate of
		///    the audio represented by the current instance.
		/// </value>
		int AudioSampleRate { get; }

		/// <summary>
		///    Gets the number of channels in the audio represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing the number of
		///    channels in the audio represented by the current
		///    instance.
		/// </value>
		int AudioChannels { get; }
	}
}