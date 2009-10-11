//
// TiffTag.cs:
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

using TagLib.IFD;
using TagLib.IFD.Entries;


namespace TagLib.Tiff
{
	internal class TiffTag : TagLib.CombinedTag
	{
		internal File File { get; private set; }

#region Constructors

		internal TiffTag (File file)
		{
			File = file;

			// Parse TIFF metadata
			//AddTag (new IFDTag (file, ifd_offset));

			// Try to find embedded XMP Metadata
			//ByteVector xmp_data = FindXMPData ();
			//if (xmp_data != null)
			//	AddTag (new XmpTag (file, xmp_data));
		}

#endregion

#region Internal Methods

		internal void AddIFDTag (IFD.IFDTag tag)
		{
			AddTag (tag);
		}

#endregion
	}
}
