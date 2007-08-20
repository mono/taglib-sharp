//
// UnknownBox.cs: Provides a simple implementation of a box of unknown type.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2006-2007 Brian Nickel
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

namespace TagLib.Mpeg4 {
	/// <summary>
	///    This class extends <see cref="Box" /> to provide a simple
	///    implementation of a box of unknown type.
	/// </summary>
	public class UnknownBox : Box
	{
		#region Private Fields
		
		/// <summary>
		///    Contains the box's data.
		/// </summary>
		private ByteVector data;
		
		#endregion
		
		
		
		#region Constructors
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="UnknownBox" /> with a provided header and handler
		///    by reading the contents from a specified file.
		/// </summary>
		/// <param name="header">
		///    A <see cref="BoxHeader" /> object containing the header
		///    to use for the new instance.
		/// </param>
		/// <param name="file">
		///    A <see cref="TagLib.File" /> object to read the contents
		///    of the box from.
		/// </param>
		/// <param name="handler">
		///    A <see cref="IsoHandlerBox" /> object containing the
		///    handler that applies to the new instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///    <paramref name="file" /> is <see langword="null" />.
		/// </exception>
		public UnknownBox (BoxHeader header, TagLib.File file,
		                   IsoHandlerBox handler)
			: base (header, handler)
		{
			if (file == null)
				throw new ArgumentNullException ("file");
			
			this.data = LoadData (file);
		}
		
		#endregion
		
		
		
		#region Public Properties
		
		/// <summary>
		///    Gets and sets the box data contained in the current
		///    instance.
		/// </summary>
		/// <value>
		///    A <see cref="ByteVector" /> object containing the box
		///    data contained in the current instance.
		/// </value>
		public override ByteVector Data {
			get {return data;}
			set {data = value;}
		}
		
		#endregion
	}
}
