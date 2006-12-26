namespace TagLib.FormatTests 
{
    public interface IFormatTest
    {
        void Init();
        void ReadAudioProperties();
        void ReadTags();
        void TestCorruptionResistance();
    }
}
