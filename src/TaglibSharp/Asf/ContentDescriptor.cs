//
// ContentDescriptor.cs: Provides a representation of an ASF Content Descriptor
// to be used in combination with ExtendedContentDescriptionObject.
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
	///    Indicates the type of data stored in a <see
	///    cref="ContentDescriptor" /> or <see cref="DescriptionRecord" />
	///    object.
	/// </summary>
	public enum DataType
	{
		/// <summary>
		///    The descriptor contains Unicode (UTF-16LE) text.
		/// </summary>
		Unicode = 0,

		/// <summary>
		///    The descriptor contains binary data.
		/// </summary>
		Bytes = 1,

		/// <summary>
		///    The descriptor contains a boolean value.
		/// </summary>
		Bool = 2,

		/// <summary>
		///    The descriptor contains a 4-byte DWORD value.
		/// </summary>
		DWord = 3,

		/// <summary>
		///    The descriptor contains a 8-byte QWORD value.
		/// </summary>
		QWord = 4,

		/// <summary>
		///    The descriptor contains a 2-byte WORD value.
		/// </summary>
		Word = 5,

		/// <summary>
		///    The descriptor contains a 16-byte GUID value.
		/// </summary>
		Guid = 6
	}

	/// <summary>
	///    This class provides a representation of an ASF Content
	///    Descriptor to be used in combination with <see
	///    cref="ExtendedContentDescriptionObject" />.
	/// </summary>
	public class ContentDescriptor
	{
		#region Private Fields

		/// <summary>
		///    Contains the string value.
		/// </summary>
		string strValue;

		/// <summary>
		///    Contains the byte value.
		/// </summary>
		ByteVector byteValue;

		/// <summary>
		///    Contains the long value.
		/// </summary>
		ulong longValue;

		#endregion



		#region Constructors

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> with a specified name and
		///    and value.
		/// </summary>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name of the
		///    new instance.
		/// </param>
		/// <param name="value">
		///    A <see cref="string" /> object containing the value for
		///    the new instance.
		/// </param>
		public ContentDescriptor (string name, string value)
		{
			Name = name;
			strValue = value;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> with a specified name and
		///    and value.
		/// </summary>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name of the
		///    new instance.
		/// </param>
		/// <param name="value">
		///    A <see cref="ByteVector" /> object containing the value
		///    for the new instance.
		/// </param>
		public ContentDescriptor (string name, ByteVector value)
		{
			Name = name;
			Type = DataType.Bytes;
			byteValue = new ByteVector (value);
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> with a specified name and
		///    and value.
		/// </summary>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name of the
		///    new instance.
		/// </param>
		/// <param name="value">
		///    A <see cref="uint" /> value containing the value
		///    for the new instance.
		/// </param>
		public ContentDescriptor (string name, uint value)
		{
			Name = name;
			Type = DataType.DWord;
			longValue = value;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> with a specified name and
		///    and value.
		/// </summary>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name of the
		///    new instance.
		/// </param>
		/// <param name="value">
		///    A <see cref="ulong" /> value containing the value
		///    for the new instance.
		/// </param>
		public ContentDescriptor (string name, ulong value)
		{
			Name = name;
			Type = DataType.QWord;
			longValue = value;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> with a specified name and
		///    and value.
		/// </summary>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name of the
		///    new instance.
		/// </param>
		/// <param name="value">
		///    A <see cref="ushort" /> value containing the value
		///    for the new instance.
		/// </param>
		public ContentDescriptor (string name, ushort value)
		{
			Name = name;
			Type = DataType.Word;
			longValue = value;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> with a specified name and
		///    and value.
		/// </summary>
		/// <param name="name">
		///    A <see cref="string" /> object containing the name of the
		///    new instance.
		/// </param>
		/// <param name="value">
		///    A <see cref="bool" /> value containing the value
		///    for the new instance.
		/// </param>
		public ContentDescriptor (string name, bool value)
		{
			Name = name;
			Type = DataType.Bool;
			longValue = value ? 1uL : 0;
		}

		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="ContentDescriptor" /> by reading its contents from
		///    a file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="Asf.File" /> object to read the raw ASF
		///    Description Record from.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CorruptFileException">
		///    A valid descriptor could not be read.
		/// </exception>
		/// <remarks>
		///    <paramref name="file" /> must be at a seek position at
		///    which the descriptor can be read.
		/// </remarks>
		protected internal ContentDescriptor (File file)
		{
			if (file == null)
				throw new ArgumentNullException (nameof (file));

			if (!Parse (file))
				throw new CorruptFileException ("Failed to parse content descriptor.");
		}

		#endregion



		#region Public Properties

		/// <summary>
		///    Gets the name of the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> object containing the name of the
		///    current instance.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		///    Gets the type of data contained in the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="DataType" /> value indicating type of data
		///    contained in the current instance.
		/// </value>
		public DataType Type { get; private set; } = DataType.Unicode;

		#endregion



		#region Public Methods

		/// <summary>
		///    Gets a string representation of the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> object containing the value of
		///    the current instance.
		/// </returns>
		public override string ToString ()
		{
			if (Type == DataType.Unicode)
				return strValue;

			if (Type == DataType.Bytes)
				return byteValue.ToString (StringType.UTF16LE);

			return longValue.ToString ();
		}

		/// <summary>
		///    Gets the binary contents of the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    contents of the current instance, or <see langword="null"
		///    /> if <see cref="Type" /> is unequal to <see
		///    cref="DataType.Bytes" />.
		/// </returns>
		public ByteVector ToByteVector ()
		{
			return byteValue;
		}

		/// <summary>
		///    Gets the boolean value contained in the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="bool" /> value containing the value of the
		///    current instance.
		/// </returns>
		public bool ToBool ()
		{
			return longValue != 0;
		}

		/// <summary>
		///    Gets the DWORD value contained in the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="uint" /> value containing the value of the
		///    current instance.
		/// </returns>
		public uint ToDWord ()
		{
			if (Type == DataType.Unicode && strValue != null && uint.TryParse (strValue, out var value))
				return value;

			return (uint)longValue;
		}

		/// <summary>
		///    Gets the QWORD value contained in the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="ulong" /> value containing the value of the
		///    current instance.
		/// </returns>
		public ulong ToQWord ()
		{
			if (Type == DataType.Unicode && strValue != null && ulong.TryParse (strValue, out var value))
				return value;

			return longValue;
		}

		/// <summary>
		///    Gets the WORD value contained in the current instance.
		/// </summary>
		/// <returns>
		///    A <see cref="ushort" /> value containing the value of the
		///    current instance.
		/// </returns>
		public ushort ToWord ()
		{
			if (Type == DataType.Unicode && strValue != null && ushort.TryParse (strValue, out var value))
				return value;

			return (ushort)longValue;
		}

		/// <summary>
		///    Renders the current instance as a raw ASF Description
		///    Record.
		/// </summary>
		/// <returns>
		///    A <see cref="ByteVector" /> object containing the
		///    rendered version of the current instance.
		/// </returns>
		public ByteVector Render ()
		{
			ByteVector value;

			switch (Type) {
			case DataType.Unicode:
				value = Object.RenderUnicode (strValue);
				break;
			case DataType.Bytes:
				value = byteValue;
				break;
			case DataType.Bool:
			case DataType.DWord:
				value = Object.RenderDWord ((uint)longValue);
				break;
			case DataType.QWord:
				value = Object.RenderQWord (longValue);
				break;
			case DataType.Word:
				value = Object.RenderWord ((ushort)longValue);
				break;
			default:
				return null;
			}

			ByteVector name = Object.RenderUnicode (Name);

			var output = new ByteVector {
				Object.RenderWord ((ushort)name.Count),
				name,
				Object.RenderWord ((ushort)Type),
				Object.RenderWord ((ushort)value.Count),
				value
			};

			return output;
		}

		#endregion



		#region Protected Methods

		/// <summary>
		///    Populates the current instance by reading in the contents
		///    from a file.
		/// </summary>
		/// <param name="file">
		///    A <see cref="Asf.File" /> object to read the raw ASF
		///    Content Descriptor from.
		/// </param>
		/// <returns>
		///    <see langword="true" /> if the data was read correctly.
		///    Otherwise <see langword="false" />.
		/// </returns>
		protected bool Parse (File file)
		{
			int name_count = file.ReadWord ();
			Name = file.ReadUnicode (name_count);

			Type = (DataType)file.ReadWord ();

			int value_count = file.ReadWord ();
			switch (Type) {
			case DataType.Word:
				longValue = file.ReadWord ();
				break;

			case DataType.Bool:
				longValue = file.ReadDWord ();
				break;

			case DataType.DWord:
				longValue = file.ReadDWord ();
				break;

			case DataType.QWord:
				longValue = file.ReadQWord ();
				break;

			case DataType.Unicode:
				strValue = file.ReadUnicode (value_count);
				break;

			case DataType.Bytes:
				byteValue = file.ReadBlock (value_count);
				break;

			default:
				return false;
			}

			return true;
		}

		#endregion
	}
}
