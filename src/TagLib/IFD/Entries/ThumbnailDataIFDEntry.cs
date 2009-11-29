//
// ThumbnailDataIFDEntry.cs:
//
// Author:
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
	///    Contains the data of a Thumbnail. Since the thumbnail is
	///    referenced by two long entries (offset to the data and length)
	///    we need to take care of this special case.
	///    This entry acts as the offset-entry but holds also the
	///    thumbail data. When rendering the entry, we have to render the
	///    data but write a long entry.
	/// </summary>
	public class ThumbnailDataIFDEntry : IFDEntry
	{
		public ushort Tag { get; private set; }
		public ByteVector Data { get; private set; }

		public ThumbnailDataIFDEntry (ushort tag, ByteVector data)
		{
			Tag = tag;
			Data = data;
		}

		public ByteVector Render (bool is_bigendian, uint offset, out ushort type, out uint count)
		{
			// the entry is a single long entry where the value is an offset to the data
			// the offset is automatically updated by the renderer.
			type = (ushort) IFDEntryType.Long;
			count = 1;

			return Data;
		}
	}
}
