//
// File.cs: Enum for the orientation of an image
//
// Author:
//   Paul Lange (palango@gmx.de)
//
// Copyright (C) 2009 Paul Lange
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

using System;

namespace TagLib.Image
{
	/**

	  1        2       3      4         5            6           7          8

	888888  888888      88  88      8888888888  88                  88  8888888888
	88          88      88  88      88  88      88  88          88  88      88  88
	8888      8888    8888  8888    88          8888888888  8888888888          88
	88          88      88  88
	88          88  888888  888888

	t-l     t-r     b-r     b-l     l-t         r-t         r-b             l-b

	**/

	/// <summary>
	/// Describes the orientation of an image.
	/// Values are viewed in terms of rows and columns.
	/// </summary>
	public enum ImageOrientation : uint
	{
		TopLeft = 1,
		TopRight = 2,
		BottomRight = 3,
		BottomLeft = 4,
		LeftTop = 5,
		RightTop = 6,
		RightBottom = 7,
		LeftBottom = 8
	}
}