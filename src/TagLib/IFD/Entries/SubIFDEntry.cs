//
// SubIFDEntry.cs:
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

namespace TagLib.IFD.Entries
{
	/// <summary>
	///    Contains a Sub IFD.
	/// </summary>
	public class SubIFDEntry : IFDEntry
	{
		public ushort Tag { get; private set; }
		public uint Type { get; private set; }
		public uint Count { get; private set; }

		public IFDStructure Structure { get; private set; }

		public SubIFDEntry (ushort tag, uint type, uint count, IFDStructure structure)
		{
			Tag = tag;
			Type = type;
			Count = count;
			Structure = structure;
		}

		public ByteVector Render (bool is_bigendian, uint offset, out ushort type, out uint count)
		{
			type = (ushort) Type;
			count = Count;

			// Don't render empty SubIFDEntry
			/*int sum = 0;
			foreach (var directory in sub_ifd.Structure.Directories)
				sum += directory.Count;
			if (sum == 0)
				return;
			*/
			var renderer = new IFDRenderer (is_bigendian, Structure, offset);

			return renderer.Render ();
		}
	}
}
