namespace TaglibSharp.Tests.FileFormats
{
	public interface IFormatTest
	{
		void Init ();
		void TearDown ();
		void ReadAudioProperties ();
		void ReadTags ();
		void TestCorruptionResistance ();
	}
}
