//
// EBMLIDs.cs:
//
// Author:
//   Julien Moutte <julien@fluendo.com>
//
// Copyright (C) 2011 FLUENDO S.A.
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

namespace TagLib.Matroska
{
    public enum EBMLID
    {
        EBMLHeader = 0x1A45DFA3,

        EBMLVersion = 0x4286,

        EBMLReadVersion = 0x42F7,

        EBMLMaxIDLength = 0x42F2,

        EBMLMaxSizeLength = 0x42F3,

        EBMLDocType = 0x4282,

        EBMLDocTypeVersion = 0x4287,

        EBMLDocTypeReadVersion = 0x4285,

        EBMLVoid = 0xEC
    }
}
