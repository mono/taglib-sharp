//
// FilePropertiesObject.cs: Provides a representation of an ASF File Properties
// object which can be read from and written to disk.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2006-2007 Brian Nickel
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

namespace TagLib.Asf
{
	/// <summary>
	///    This class extends <see cref="Object" /> to provide a
	///    representation of an ASF File Properties object which can be read
	///    from and written to disk.
	/// </summary>
	public class FilePropertiesObject : Object
	{
		#region Constant Values

		static readonly DateTime FileTimeOffset = new DateTime (1601, 1, 1);

		#endregion

		#region Private Fields

		/// <summary>
		///    Contains the GUID for the file.
		/// </summary>
		System.Guid file_id;

		/// <summary>
		///    Contains the creation date.
		/// </summary>
		readonly ulong creation_date;

		/// <summary>
		///    Contains the play duration.
		/// </summary>
		readonly ulong play_duration;

		/// <summary>
		///    Contains the send duration.
		/// </summary>
		readonly ulong send_duration;

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="FilePropertiesObject" /> by reading the contents
		///    from a specified position in a specified file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="Asf.File" /> object containing the file from
		///    which the contents of the new instance are to be read.
		/// </param>
		/// <param name="position">
		///    A <see cref="long" /> value specify at what position to
		///    read the object.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///    <paramref name="position" /> is less than zero or greater
		///    than the size of the file.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    The object read from disk does not have the correct GUID
		///    or smaller than the minimum size.
		/// </exception>
		public FilePropertiesObject (File file, long position)
			: base (file, position)
		{
			if (!Guid.Equals (Asf.Guid.AsfFilePropertiesObject))
				throw new CorruptFileException ("Object GUID incorrect.");

			if (OriginalSize < 104)
				throw new CorruptFileException ("Object size too small.");

			file_id = file.ReadGuid ();
			FileSize = file.ReadQWord ();
			creation_date = file.ReadQWord ();
			DataPacketsCount = file.ReadQWord ();
			play_duration = file.ReadQWord ();
			send_duration = file.ReadQWord ();
			Preroll = file.ReadQWord ();
			Flags = file.ReadDWord ();
			MinimumDataPacketSize = file.ReadDWord ();
			MaximumDataPacketSize = file.ReadDWord ();
			MaximumBitrate = file.ReadDWord ();
		}

		#endregion



		#region Public Properties

		/// <summary>
		///    Gets the GUID for the file described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="System.Guid" /> value containing the GUID
		///    for the file described by the current instance.
		/// </value>
		public System.Guid FileId {
			get { return file_id; }
		}

		/// <summary>
		///    Gets the size of the file described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ulong" /> value containing the size of the
		///    file described by the current instance.
		/// </value>
		public ulong FileSize { get; private set; }

		/// <summary>
		///    Gets the creation date of the file described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="DateTime" /> value containing the creation
		///    date of the file described by the current instance.
		/// </value>
		public DateTime CreationDate {
			get { return new DateTime ((long)creation_date + FileTimeOffset.Ticks); }
		}

		/// <summary>
		///    Gets the number of data packets in the file described by
		///    the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="ulong" /> value containing the number of
		///    data packets in the file described by the current
		///    instance.
		/// </value>
		public ulong DataPacketsCount { get; private set; }

		/// <summary>
		///    Gets the play duration of the file described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TimeSpan" /> value containing the play
		///    duration of the file described by the current instance.
		/// </value>
		public TimeSpan PlayDuration {
			get { return new TimeSpan ((long)play_duration); }
		}

		/// <summary>
		///    Gets the send duration of the file described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TimeSpan" /> value containing the send
		///    duration of the file described by the current instance.
		/// </value>
		public TimeSpan SendDuration {
			get { return new TimeSpan ((long)send_duration); }
		}

		/// <summary>
		///    Gets the pre-roll of the file described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ulong" /> value containing the pre-roll of
		///    the file described by the current instance.
		/// </value>
		public ulong Preroll { get; private set; }

		/// <summary>
		///    Gets the flags of the file described by the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the flags of the
		///    file described by the current instance.
		/// </value>
		public uint Flags { get; private set; }

		/// <summary>
		///    Gets the minimum data packet size of the file described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the minimum data
		///    packet size of the file described by the current
		///    instance.
		/// </value>
		public uint MinimumDataPacketSize { get; private set; }

		/// <summary>
		///    Gets the maximum data packet size of the file described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the maximum data
		///    packet size of the file described by the current
		///    instance.
		/// </value>
		public uint MaximumDataPacketSize { get; private set; }

		/// <summary>
		///    Gets the maximum bitrate of the file described by the
		///    current instance.
		/// </summary>
		/// <value>
		///    A <see cref="uint" /> value containing the maximum
		///    bitrate of the file described by the current instance.
		/// </value>
		public uint MaximumBitrate { get; private set; }

		#endregion



		#region Public Methods

		/// <summary>
		///    Renders the current instance as a raw ASF object.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		public override ByteVector Render ()
		{
			ByteVector output = file_id.ToByteArray ();
			output.Add (RenderQWord (FileSize));
			output.Add (RenderQWord (creation_date));
			output.Add (RenderQWord (DataPacketsCount));
			output.Add (RenderQWord (play_duration));
			output.Add (RenderQWord (send_duration));
			output.Add (RenderQWord (Preroll));
			output.Add (RenderDWord (Flags));
			output.Add (RenderDWord (MinimumDataPacketSize));
			output.Add (RenderDWord (MaximumDataPacketSize));
			output.Add (RenderDWord (MaximumBitrate));

			return Render (output);
		}

		#endregion
	}
}
