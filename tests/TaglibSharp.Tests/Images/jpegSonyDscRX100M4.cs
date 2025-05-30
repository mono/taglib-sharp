//
//  jpegSonyDscRX100M4.cs
//
//  Author:
//       david <codingdave@gmail.com>
//
//  Copyright (c) 2016 david
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using File = TagLib.File;

namespace TaglibSharp.Tests.Images
{
	[TestClass]
	public class jpegSonyDscRX100M4
	{
		[TestMethod]
		public void Test ()
		{
			bool isSuccess = true;
			try {
				File.Create (TestPath.Samples + "sample_sony_DSC-RX100M4.jpg");
			} catch (ArithmeticException) {
				// Old versions of TagLib were throwing an ArithmeticException on reading in Sony DSC RX100M4 images
				isSuccess = false;
			} catch (Exception) {
				// All excetions shall make this test fail
				isSuccess = false;
			}
			Assert.IsTrue (isSuccess);
		}
	}
}

