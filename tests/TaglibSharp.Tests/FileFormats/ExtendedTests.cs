using NUnit.Framework;
using TagLib;

using File = TagLib.File;

namespace TaglibSharp.Tests.FileFormats
{
	public static class ExtendedTests
	{
		public static void WriteExtendedTags (string sample_file, string tmp_file)
		{
			if (System.IO.File.Exists (tmp_file))
				System.IO.File.Delete (tmp_file);

			try {
				System.IO.File.Copy (sample_file, tmp_file);

				var tmp = File.Create (tmp_file);
				SetTags (tmp.Tag);
				tmp.Save ();

				tmp = File.Create (tmp_file);
				CheckTags (tmp.Tag);
			} finally {
				if (System.IO.File.Exists (tmp_file))
					System.IO.File.Delete (tmp_file);
			}
		}

		public static void SetTags (Tag tag)
		{
			tag.ReplayGainTrackGain = -10.28;
			tag.ReplayGainTrackPeak = 0.999969;
			tag.ReplayGainAlbumGain = -9.98;
			tag.ReplayGainAlbumPeak = 0.999980;
		}

		public static void CheckTags (Tag tag)
		{
			ClassicAssert.AreEqual (-10.28, tag.ReplayGainTrackGain);
			ClassicAssert.AreEqual (0.999969, tag.ReplayGainTrackPeak);
			ClassicAssert.AreEqual (-9.98, tag.ReplayGainAlbumGain);
			ClassicAssert.AreEqual (0.999980, tag.ReplayGainAlbumPeak);
		}

		public static void TestCorruptionResistance (string path)
		{
			try {
				File.Create (path);
			} catch (CorruptFileException) {
			} catch (NullReferenceException) {
				throw;
			} catch {
			}
		}
	}
}
