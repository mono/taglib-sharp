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

using System;

using TagLib.Image;
using TagLib.Xmp;
using TagLib.IFD;
using TagLib.IFD.Entries;


namespace TagLib.Tiff
{
	internal class TiffTag : CombinedImageTag
	{
		private IFDTag ifd_tag;
		private XmpTag xmp_tag;

#region Constructors

		internal TiffTag (File file) : base (file) {}

#endregion

#region Internal Methods

		internal void AddIFDTag (IFDTag tag)
		{
			if (ifd_tag != null)
				throw new Exception ("File with multiple IFD directories encountered, this approach doesn't work! (RAW file?)");

			ifd_tag = tag;
			AddTag (tag);

			// Find XMP data
			var xmp_entry = ifd_tag.GetEntry ((ushort) IFDEntryTag.XMP) as ByteVectorIFDEntry;
			if (xmp_entry == null)
				return;

			xmp_tag = new XmpTag (File, xmp_entry.Data);
			AddTag (xmp_tag);
		}

#endregion
	}
}
