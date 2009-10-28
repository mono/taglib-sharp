//
// IFDEntry.cs:
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Ruben Vermeersch
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

namespace TagLib.IFD
{
	public enum IFDEntryType : ushort
	{
		Unknown = 0,
		Byte = 1,
		Ascii = 2,
		Short = 3,
		Long = 4,
		Rational = 5,
		SByte = 6,
		Undefined = 7,
		SShort = 8,
		SLong = 9,
		SRational = 10,
		Float = 11,
		Double = 12
	}

	/// <summary>
	///    An IFD entry, which is a key/value pair inside an IFD.
	/// </summary>
	public interface IFDEntry
	{
		uint Tag {
			get;
		}

		ByteVector Render (bool is_bigendian, uint offset, out ushort type, out uint count);
	}

	public abstract class ArrayIFDEntry<T> : IFDEntry
	{
		public uint Tag { get; private set; }
		public T [] Values { get; protected set; }

		public ArrayIFDEntry (uint tag)
		{
			Tag = tag;
		}

		public abstract ByteVector Render (bool is_bigendian, uint offset, out ushort type, out uint count);
	}
}
