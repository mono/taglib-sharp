using System;

namespace TagLib.Image
{
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