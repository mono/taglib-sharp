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
		public uint Tag { get; private set; }
		public uint Type { get; private set; }
		public uint Count { get; private set; }

		public IFDTag IFDTag { get; private set; }

		public SubIFDEntry (uint tag, uint type, uint count, IFDTag ifd_tag)
		{
			Tag = tag;
			Type = type;
			Count = count;
			IFDTag = ifd_tag;
		}
	}
}
