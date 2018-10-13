namespace TagLib.Id3v2
{
	public enum EventType
	{
		Padding = 0x00,

		EndOfInitialSilence = 0x01,

		IntroStart = 0x02,

		MainPartStart = 0x03,

		OutroStart = 0x04,

		OutroEnd = 0x05,

		VerseStart = 0x06,

		RefrainStart = 0x07,

		InterludeStart = 0x08,

		ThemeStart = 0x09,

		VariationStart = 0x0A,

		KeyChange = 0x0B,

		TimeChange = 0x0C,

		MomentaryUnwantedNoise = 0x0D,

		SustainedNoise = 0x0E,

		SustainedNoiseEnd = 0x0F,

		IntroEnd = 0x10,

		MainPartEnd = 0x11,

		VerseEnd = 0x12,

		RefrainEnd = 0x13,

		ThemeEnd = 0x14,

		Profanity = 0x15,

		ProfanityEnd = 0x16,

		AudioEnd = 0xFD,

		AudioFileEnd = 0xFE,
	}
}
