using System;

namespace TagLib.Image
{
	/// <summary>
	/// Describes the orientation of an image.
	/// Values are from http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/EXIF.html
	/// </summary>
	public enum ImageOrientation : uint
	{
		Unknown = 0,
		Horizontal = 1,
		MirrorHorizontal = 2,
		Rotate180Degrees = 3,
		MirrorVertical = 4,
		MirrorHorizontalAndRotate270DegreesClockwise = 5,
		Rotate90DegreesClockwise = 6,
		MirrorHorizontalAndRotate90DegreesClockwise = 7,
		Rotate270DegreesClockwise = 8
	}
}