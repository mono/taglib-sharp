//
// StreamHeader.cs: Provides support for reading Apple's AIFF stream
// properties.
//
// Author:
//   Helmut Wahrmann
//
// Copyright (C) 2009 Helmut Wahrmann
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

namespace TagLib.Aiff
{
	/// <summary>
	///    This struct implements <see cref="IAudioCodec" /> to provide
	///    support for reading Apple's AIFF stream properties.
	/// </summary>
	public struct StreamHeader : IAudioCodec, ILosslessAudioCodec
	{
		#region Private Fields

		/// <summary>
		///    Contains the number of channels.
		/// </summary>
		/// <remarks>
		///    This value is stored in bytes (9,10).
		///    1 is monophonic, 2 is stereo, 4 means 4 channels, etc..
		///    any number of audio channels may be represented
		/// </remarks>
		readonly ushort channels;

		/// <summary>
		///    Contains the number of sample frames in the Sound Data chunk.
		/// </summary>
		/// <remarks>
		///    This value is stored in bytes (11-14).
		/// </remarks>
		readonly ulong total_frames;

		/// <summary>
		///    Contains the number of bits per sample.
		/// </summary>
		/// <remarks>
		///    This value is stored in bytes (15,16).
		///    It can be any number from 1 to 32.
		/// </remarks>
		readonly ushort bits_per_sample;

		/// <summary>
		///    Contains the sample rate.
		/// </summary>
		/// <remarks>
		///    This value is stored in bytes (17-26).
		///    the sample rate at which the sound is to be played back,
		///    in sample frames per second
		/// </remarks>
		readonly ulong sample_rate;

		/// <summary>
		///    Contains the length of the audio stream.
		/// </summary>
		/// <remarks>
		///    This value is provided by the constructor.
		/// </remarks>
		readonly long stream_length;

		#endregion

		#region Public Static Fields

		/// <summary>
		///    The size of an AIFF Common chunk
		/// </summary>
		public const uint Size = 26;

		/// <summary>
		///    The identifier used to recognize a AIFF file.
		///    Altough an AIFF file start with "FORM2, we're interested
		///    in the Common chunk only, which contains the properties we need.
		/// </summary>
		/// <value>
		///    "COMM"
		/// </value>
		public static readonly ReadOnlyByteVector FileIdentifier = "COMM";

		#endregion

		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="StreamHeader" /> for a specified header block and
		///    stream length.
		/// </summary>
		/// <param name="data">
		///    A <see cref="ByteVector" /> object containing the stream
		///    header data.
		/// </param>
		/// <param name="streamLength">
		///    A <see cref="long" /> value containing the length of the
		///    AIFF Audio stream in bytes.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="data" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    <paramref name="data" /> does not begin with <see
		///    cref="FileIdentifier" />
		/// </exception>
		public StreamHeader (ByteVector data, long streamLength)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));


			if (!data.StartsWith (FileIdentifier))
				throw new CorruptFileException ("Data does not begin with identifier.");

			stream_length = streamLength;

			// The first 8 bytes contain the Common chunk identifier "COMM"
			// And the size of the common chunk, which is always 18
			channels = data.Mid (8, 2).ToUShort (true);
			total_frames = data.Mid (10, 4).ToULong (true);
			bits_per_sample = data.Mid (14, 2).ToUShort (true);
			sample_rate = (ulong)data.Mid (16, 10).ToExtendedPrecision ();
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    Gets the duration of the media represented by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="TimeSpan" /> containing the duration of the
		///    media represented by the current instance.
		/// </value>
		public TimeSpan Duration {
			get {
				if (sample_rate <= 0 || total_frames <= 0)
					return TimeSpan.Zero;

				return TimeSpan.FromSeconds (
					total_frames /
					(double)sample_rate);
			}
		}

		/// <summary>
		///    Gets the types of media represented by the current
		///    instance.
		/// </summary>
		/// <value>
		///    Always <see cref="MediaTypes.Audio" />.
		/// </value>
		public MediaTypes MediaTypes {
			get { return MediaTypes.Audio; }
		}

		/// <summary>
		///    Gets a text description of the media represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing a description
		///    of the media represented by the current instance.
		/// </value>
		public string Description {
			get { return "AIFF Audio"; }
		}

		/// <summary>
		///    Gets the bitrate of the audio represented by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing a bitrate of the
		///    audio represented by the current instance.
		/// </value>
		public int AudioBitrate {
			get {
				TimeSpan d = Duration;
				if (d <= TimeSpan.Zero)
					return 0;

				return (int)((stream_length * 8L) / d.TotalSeconds) / 1000;
			}
		}

		/// <summary>
		///    Gets the sample rate of the audio represented by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing the sample rate of
		///    the audio represented by the current instance.
		/// </value>
		public int AudioSampleRate {
			get { return (int)sample_rate; }
		}

		/// <summary>
		///    Gets the number of channels in the audio represented by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing the number of
		///    channels in the audio represented by the current
		///    instance.
		/// </value>
		public int AudioChannels {
			get { return channels; }
		}

		/// <summary>
		///    Gets the number of bits per sample in the audio
		///    represented by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="int" /> value containing the number of bits
		///    per sample in the audio represented by the current
		///    instance.
		/// </value>
		public int BitsPerSample {
			get { return bits_per_sample; }
		}

		#endregion
	}
}
