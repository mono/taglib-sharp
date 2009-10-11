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

using TagLib.Xmp;
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

			ByteVector xmp_data = FindXMPData ();
			if (xmp_data != null)
				AddTag (new XmpTag (File, xmp_data));
		}

#endregion

#region Private Methods

		private ByteVector FindXMPData ()
		{
			IFDTag ifd_tag = null;
			foreach (Tag tag in Tags) {
				if (tag is IFDTag) {
					ifd_tag = tag as IFDTag;
					break;
				}
			}

			if (ifd_tag == null)
				return null;

			var xmp_entry = ifd_tag.GetEntry ((ushort) IFDEntryTag.XMP) as ByteVectorIFDEntry;
			if (xmp_entry == null)
				return null;

			return xmp_entry.Data;
		}

#endregion
	}
}
