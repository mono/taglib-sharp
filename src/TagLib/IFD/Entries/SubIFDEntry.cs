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

	public enum SubIFDType {
		Exif,
		Interoperability,
		GPS,
		CanonMakernote,
		PanasonicMakernote,
		PentaxMakernote,
		NikonMakernote1,
		NikonMakernote2,
		NikonMakernote3,
		OlympusMakernote1,
		OlympusMakernote2,
		SonyMakernote
	}


	/// <summary>
	///    Contains a Sub IFD.
	/// </summary>
	public class SubIFDEntry : IFDEntry
	{
		public ushort Tag { get; private set; }
		public ushort Type { get; private set; }
		public uint Count { get; private set; }
		public SubIFDType SubIFDType { get; private set; }

		public IFDStructure Structure { get; private set; }

		public SubIFDEntry (ushort tag, ushort type, uint count, IFDStructure structure, SubIFDType sub_ifd_type)
		{
			Tag = tag;
			Type = type;
			Count = count;
			Structure = structure;
			SubIFDType = sub_ifd_type;
		}

		public ByteVector Render (bool is_bigendian, uint offset, out ushort type, out uint count)
		{
			type = (ushort) Type;

			if (SubIFDType == SubIFDType.PanasonicMakernote) {
				var renderer = new IFDRenderer (is_bigendian, Structure, offset + 12);
				ByteVector data = renderer.Render ();
				data.Insert (0, "Panasonic\0\0\0");

				count = (uint) data.Count;

				return data;
			}

			if (SubIFDType == SubIFDType.NikonMakernote3) {
				var renderer = new IFDRenderer (is_bigendian, Structure, 8);
				ByteVector data = renderer.Render ();

				ByteVector header = "Nikon\0";
				header.Add (new byte[] {2, 0, 0, 0});
				header.Add (is_bigendian ? "MM" : "II");
				header.Add (ByteVector.FromUShort (42, is_bigendian));
				header.Add (new byte[] {0, 0, 0, 8});

				data.Insert (0, header);

				count = (uint) data.Count;

				return data;
			}

			if (SubIFDType == SubIFDType.OlympusMakernote1) {
				var renderer = new IFDRenderer (is_bigendian, Structure, offset + 8);
				ByteVector data = renderer.Render ();

				data.Insert (0, new byte [] {0x01, 0x00});
				data.Insert (0, "OLYMP\0");

				count = (uint) data.Count;

				return data;
			}

			if (SubIFDType == SubIFDType.OlympusMakernote2) {
				var renderer = new IFDRenderer (is_bigendian, Structure, 12);
				ByteVector data = renderer.Render ();

				data.Insert (0, new byte [] {0x03, 0x00});
				data.Insert (0, "OLYMPUS\0II");

				count = (uint) data.Count;

				return data;
			}

			/*if (SubIFDType == SubIFDType.PentaxMakernote) {
				var renderer = new IFDRenderer (is_bigendian, Structure, offset + 6);
				ByteVector data = renderer.Render ();
				data.Insert (0, "AOC\0");

			}*/

			// Don't render empty SubIFDEntry
			/*int sum = 0;
			foreach (var directory in sub_ifd.Structure.Directories)
				sum += directory.Count;
			if (sum == 0)
				return;
			*/

			count = Count;
			return new IFDRenderer (is_bigendian, Structure, offset).Render ();
		}
	}
}
