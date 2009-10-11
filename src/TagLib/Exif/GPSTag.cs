//
// GPSTag.cs:
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

using TagLib.IFD;

namespace TagLib.Exif
{

	public class GPSTag : IFDTag
	{

#region Constructors

		public GPSTag (File file, long base_offset, uint ifd_offset, bool is_bigendian, out uint next_offset)
			: base (file, base_offset, ifd_offset, is_bigendian, out next_offset) {}


		public GPSTag (File file, uint ifd_offset, bool is_bigendian, out uint next_offset)
			: base (file, ifd_offset, is_bigendian, out next_offset) {}

#endregion


#region Public Methods

		/// <summary>
		///    Gets the tag types contained in the current instance.
		/// </summary>
		/// <value>
		///    Always <see cref="TagTypes.GPS" />.
		/// </value>
		public override TagTypes TagTypes {
			get { return TagTypes.GPS; }
		}

#endregion

	}
}
