//
// MediaTypes.cs
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace TagLib
{
	/// <summary>
	///    Indicates the types of media represented by a <see cref="ICodec"
	///    /> or <see cref="Properties" /> object.
	/// </summary>
	/// <remarks>
	///    These values can be bitwise combined to represent multiple media
	///    types.
	/// </remarks>
	[Flags]
	public enum MediaTypes
	{
		/// <summary>
		///    No media is present.
		/// </summary>
		None = 0,

		/// <summary>
		///    Audio is present.
		/// </summary>
		Audio = 1,

		/// <summary>
		///    Video is present.
		/// </summary>
		Video = 2,

		/// <summary>
		///    A Photo is present.
		/// </summary>
		Photo = 4,

		/// <summary>
		///    Text is present.
		/// </summary>
		Text = 8
	}
}