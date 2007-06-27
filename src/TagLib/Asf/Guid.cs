/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Text;
using System.Collections;
using System;

namespace TagLib.Asf {
	public static class Guid
	{
		public static readonly System.Guid AsfAudioMedia                       = new System.Guid ("F8699E40-5B4D-11CF-A8FD-00805F5C442B");
		public static readonly System.Guid AsfContentDescriptionObject         = new System.Guid ("75B22633-668E-11CF-A6D9-00AA0062CE6C");
		public static readonly System.Guid AsfExtendedContentDescriptionObject = new System.Guid ("D2D0A440-E307-11D2-97F0-00A0C95EA850");
		public static readonly System.Guid AsfFilePropertiesObject             = new System.Guid ("8CABDCA1-A947-11CF-8EE4-00C00C205365");
		public static readonly System.Guid AsfHeaderExtensionObject            = new System.Guid ("5FBF03B5-A92E-11CF-8EE3-00C00C205365");
		public static readonly System.Guid AsfHeaderObject                     = new System.Guid ("75B22630-668E-11CF-A6D9-00AA0062CE6C");
		public static readonly System.Guid AsfMetadataLibraryObject            = new System.Guid ("44231C94-9498-49D1-A141-1D134E457054");
		public static readonly System.Guid AsfPaddingObject                    = new System.Guid ("1806D474-CADF-4509-A4BA-9AABCB96AAE8");
		public static readonly System.Guid AsfReserved1                        = new System.Guid ("ABD3D211-A9BA-11cf-8EE6-00C00C205365");
		public static readonly System.Guid AsfStreamPropertiesObject           = new System.Guid ("B7DC0791-A9B7-11CF-8EE6-00C00C205365");
		public static readonly System.Guid AsfVideoMedia                       = new System.Guid ("BC19EFC0-5B4D-11CF-A8FD-00805F5C442B");
	}
}
