namespace TaglibSharp.Tests.FileFormats
{
	public interface IFormatTest
	{
		void Init ();
		void ReadAudioProperties ();
		void ReadTags ();
		void TestCorruptionResistance ();
	}
}
