//
// ShortIFDEntry.cs:
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//   Mike Gemuende (mike@gemuende.de)
//
// Copyright (C) 2009 Ruben Vermeersch
// Copyright (C) 2009 Mike Gemuende
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

namespace TagLib.IFD.Entries
{
	/// <summary>
	///    Contains a SHORT value.
	/// </summary>
	public class ShortIFDEntry : IFDEntry
	{
		public ushort Tag { get; private set; }
		public ushort Value { get; private set; }

		public ShortIFDEntry (ushort tag, ushort value)
		{
			Tag = tag;
			Value = value;
		}

		public ByteVector Render (bool is_bigendian, uint offset, out ushort type, out uint count)
		{
			type = (ushort) IFDEntryType.Short;
			count = 1;

			return ByteVector.FromUShort (Value, is_bigendian);
		}
	}
}
