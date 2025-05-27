namespace TaglibSharp.Tests.FileFormats;

public interface IFormatTest
{
	void ReadAudioProperties ();
	void ReadTags ();
	void TestCorruptionResistance ();
}
