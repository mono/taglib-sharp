//
// Marker.cs:
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
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

namespace TagLib.Jpeg
{
	/// <summary>
	///    This enum defines the different markers used in JPEG segments.
	/// </summary>
	public enum Marker {
		/// <summary>
		///    Restart
		/// </summary>
		RST0 = 0xD0,

		/// <summary>
		///    Restart
		/// </summary>
		RST1 = 0xD1,

		/// <summary>
		///    Restart
		/// </summary>
		RST2 = 0xD2,

		/// <summary>
		///    Restart
		/// </summary>
		RST3 = 0xD3,

		/// <summary>
		///    Restart
		/// </summary>
		RST4 = 0xD4,

		/// <summary>
		///    Restart
		/// </summary>
		RST5 = 0xD5,

		/// <summary>
		///    Restart
		/// </summary>
		RST6 = 0xD6,

		/// <summary>
		///    Restart
		/// </summary>
		RST7 = 0xD7,

		/// <summary>
		///    Start of Image
		/// </summary>
		SOI = 0xD8,

		/// <summary>
		///    End of Image
		/// </summary>
		EOI = 0xD9,
	}
}
