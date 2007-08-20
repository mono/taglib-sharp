//
// BoxTypes.cs: Contains common box names.
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

namespace TagLib.Mpeg4 {
	internal static class BoxType
	{
		public static readonly ReadOnlyByteVector Aart = "aART";
		public static readonly ReadOnlyByteVector Alb  = AppleTag.FixId ("alb");
		public static readonly ReadOnlyByteVector Art  = AppleTag.FixId ("ART");
		public static readonly ReadOnlyByteVector Cmt  = AppleTag.FixId ("cmt");
		public static readonly ReadOnlyByteVector Cond = "cond";
		public static readonly ReadOnlyByteVector Covr = "covr";
		public static readonly ReadOnlyByteVector Co64 = "co64";
		public static readonly ReadOnlyByteVector Cpil = "cpil";
		public static readonly ReadOnlyByteVector Cprt = "cprt";
		public static readonly ReadOnlyByteVector Data = "data";
		public static readonly ReadOnlyByteVector Day  = AppleTag.FixId ("day");
		public static readonly ReadOnlyByteVector Disk = "disk";
		public static readonly ReadOnlyByteVector Esds = "esds";
		public static readonly ReadOnlyByteVector Ilst = "ilst";
		public static readonly ReadOnlyByteVector Free = "free";
		public static readonly ReadOnlyByteVector Gen  = AppleTag.FixId ("gen");
		public static readonly ReadOnlyByteVector Gnre = "gnre";
		public static readonly ReadOnlyByteVector Grp  = AppleTag.FixId("grp");
		public static readonly ReadOnlyByteVector Hdlr = "hdlr";
		public static readonly ReadOnlyByteVector Lyr  = AppleTag.FixId ("lyr");
		public static readonly ReadOnlyByteVector Mdat = "mdat";
		public static readonly ReadOnlyByteVector Mdia = "mdia";
		public static readonly ReadOnlyByteVector Meta = "meta";
		public static readonly ReadOnlyByteVector Mean = "mean";
		public static readonly ReadOnlyByteVector Minf = "minf";
		public static readonly ReadOnlyByteVector Moov = "moov";
		public static readonly ReadOnlyByteVector Mvhd = "mvhd";
		public static readonly ReadOnlyByteVector Nam  = AppleTag.FixId ("nam");
		public static readonly ReadOnlyByteVector Name = "name";
		public static readonly ReadOnlyByteVector Skip = "skip";
		public static readonly ReadOnlyByteVector Stbl = "stbl";
		public static readonly ReadOnlyByteVector Stco = "stco";
		public static readonly ReadOnlyByteVector Stsd = "stsd";
		public static readonly ReadOnlyByteVector Tmpo = "tmpo";
		public static readonly ReadOnlyByteVector Trak = "trak";
		public static readonly ReadOnlyByteVector Trkn = "trkn";
		public static readonly ReadOnlyByteVector Udta = "udta";
		public static readonly ReadOnlyByteVector Uuid = "uuid";
		public static readonly ReadOnlyByteVector Wrt  = AppleTag.FixId ("wrt");
		public static readonly ReadOnlyByteVector DASH = "----";

		// Handler types.
		public static readonly ReadOnlyByteVector Soun = "soun";
		public static readonly ReadOnlyByteVector Vide = "vide";
	}
}