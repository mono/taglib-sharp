using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using File = TagLib.File;

namespace TaglibSharp.Tests.Images.Validators
{
	public class ImageTest
	{
		string pre_hash;
		string post_hash;

		public static bool CompareLargeImages => Environment.GetEnvironmentVariable ("COMPARE_LARGE_FILES") == "1";

		public bool CompareImageData { get; set; }

		/// <summary>
		/// Run image tests with TestFixtureBase for proper file isolation
		/// </summary>
		public static void Run (TestFixtureBase testFixture, string filename, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (testFixture, filename, true, invariant, modifications);
		}

		/// <summary>
		/// Run image tests with TestFixtureBase for proper file isolation
		/// </summary>
		public static void Run (TestFixtureBase testFixture, string directory, string filename, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (testFixture, directory, filename, true, invariant, modifications);
		}

		/// <summary>
		/// Run image tests with TestFixtureBase for proper file isolation
		/// </summary>
		public static void Run (TestFixtureBase testFixture, string filename, bool compare_image_data, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (testFixture, TestPath.Samples, filename, compare_image_data, invariant, modifications);
		}

		/// <summary>
		/// Run image tests with TestFixtureBase for proper file isolation
		/// </summary>
		public static void Run (TestFixtureBase testFixture, string directory, string filename, bool compare_image_data, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			if (modifications.Length == 0) {
				var test = new ImageTest {
					TestFixture = testFixture,
					ImageFileName = filename,
					ImageDirectory = directory,
					CompareImageData = compare_image_data,
					InvariantValidator = invariant,
					ModificationValidator = null
				};
				test.TestImage ();
			} else {
				foreach (var modification in modifications) {
					var test = new ImageTest {
						TestFixture = testFixture,
						ImageFileName = filename,
						ImageDirectory = directory,
						CompareImageData = compare_image_data,
						InvariantValidator = invariant,
						ModificationValidator = modification
					};
					test.TestImage ();
				}
			}
		}

		/// <summary>
		/// Legacy methods for backwards compatibility - these should be avoided in new tests
		/// </summary>
		[Obsolete("Use Run(TestFixtureBase, ...) overloads instead for proper file isolation")]
		public static void Run (string filename, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (filename, true, invariant, modifications);
		}

		[Obsolete("Use Run(TestFixtureBase, ...) overloads instead for proper file isolation")]
		public static void Run (string directory, string filename, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (directory, filename, true, invariant, modifications);
		}

		[Obsolete("Use Run(TestFixtureBase, ...) overloads instead for proper file isolation")]
		public static void Run (string filename, bool compare_image_data, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (TestPath.Samples, filename, compare_image_data, invariant, modifications);
		}

		[Obsolete("Use Run(TestFixtureBase, ...) overloads instead for proper file isolation")]
		public static void Run (string directory, string filename, bool compare_image_data, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			if (modifications.Length == 0) {
				var test = new ImageTest {
					ImageFileName = filename,
					ImageDirectory = directory,
					TempDirectory = directory,
					CompareImageData = compare_image_data,
					InvariantValidator = invariant,
					ModificationValidator = null
				};
				test.TestImage ();
			} else {
				foreach (var modification in modifications) {
					var test = new ImageTest {
						ImageFileName = filename,
						ImageDirectory = directory,
						TempDirectory = directory,
						CompareImageData = compare_image_data,
						InvariantValidator = invariant,
						ModificationValidator = modification
					};
					test.TestImage ();
				}
			}
		}

		void TestImage ()
		{
			var file = ParseUnmodifiedFile ();

			if (file.Writeable && ModificationValidator == null)
				throw new Exception ("Wrong usage of test. A writeable file must be tested at least with a NoModificationValidator");

			if (ModificationValidator == null)
				return;

			ModifyFile ();
			ParseModifiedFile ();
		}

		/// <summary>
		///    Parse the unmodified file.
		/// </summary>
		TagLib.Image.File ParseUnmodifiedFile ()
		{
			var file = ReadFile (ImageFile);
			InvariantValidator.ValidateMetadataInvariants (file);
			if (CompareImageData)
				pre_hash = ReadImageData (file);

			return file;
		}

		/// <summary>
		///    Modify and save the file.
		/// </summary>
		void ModifyFile ()
		{
			CreateTmpFile ();
			var tmp = ReadFile (TempImageFilePath ?? TempImageFile);
			InvariantValidator.ValidateMetadataInvariants (tmp);
			ModificationValidator.ValidatePreModification (tmp);
			ModificationValidator.ModifyMetadata (tmp);
			ModificationValidator.ValidatePostModification (tmp);
			Assert.IsTrue (tmp.Writeable, "File should be writeable");
			Assert.IsFalse (tmp.PossiblyCorrupt, "Corrupt files should never be written");
			tmp.Save ();
		}

		/// <summary>
		///    Re-parse the modified file.
		/// </summary>
		void ParseModifiedFile ()
		{
			var tmp = ReadFile (TempImageFilePath ?? TempImageFile);
			InvariantValidator.ValidateMetadataInvariants (tmp);
			ModificationValidator.ValidatePostModification (tmp);
			if (CompareImageData) {
				post_hash = ReadImageData (tmp);
				ValidateImageData ();
			}
		}

		TagLib.Image.File ReadFile (string path)
		{
			return File.Create (path) as TagLib.Image.File;
		}

		/// <summary>
		///    Only files we can actually render are supported, for most types
		///    this just means adding the type here. RAW files need extra
		///    attention.
		/// </summary>
		bool IsSupportedImageFile (TagLib.Image.File file)
		{
			return (file is TagLib.Jpeg.File) || (file is TagLib.Tiff.File) || (file is TagLib.Gif.File) || (file is TagLib.Png.File);
		}

		string ReadImageData (TagLib.Image.File file)
		{
			if (!IsSupportedImageFile (file))
				Assert.Fail ("Unsupported type for data reading: " + file);

			file.Mode = File.AccessMode.Read;
			var v = file.ReadBlock ((int)file.Length);

            using var image = Image.Load<Rgba32>(v.Data);
            byte[] result = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(result);
            string md5Sum = Utils.Md5Encode(result);
            file.Mode = File.AccessMode.Closed;

			return md5Sum;
		}

		void ValidateImageData ()
		{
			string label = $"Image data mismatch for {ImageFileName}/{ModificationValidator}";
			Assert.AreEqual (pre_hash, post_hash, label);
		}

		void CreateTmpFile ()
		{
			if (TestFixture != null) {
				// Use TestFixtureBase for proper file isolation
				TempImageFilePath = TestFixture.CreateTempFile(ImageFile, TempImageFileName);
			} else {
				// Legacy behavior for backwards compatibility
				Directory.CreateDirectory(Path.GetDirectoryName(TempImageFile));
				if (System.IO.File.Exists (TempImageFile))
					System.IO.File.Delete (TempImageFile);
				System.IO.File.Copy (ImageFile, TempImageFile);
				TempImageFilePath = TempImageFile;
			}
		}

		/// <summary>
		///    The TestFixtureBase instance for file management (null for legacy behavior)
		/// </summary>
		TestFixtureBase TestFixture { get; set; }

		/// <summary>
		///    The filename of the file to test. Name only, no paths.
		/// </summary>
		string ImageFileName { get; set; }

		string TempImageFileName => $"tmpwrite_{ImageFileName}";

		string ImageDirectory { get; set; }

		string TempDirectory { get; set; }

		string ImageFile => $"{ImageDirectory}/{ImageFileName}";

		string TempImageFile => Path.Combine (TempDirectory, Environment.Version.ToString(), TempImageFileName);

		/// <summary>
		///    The actual path to the temporary image file (set by CreateTmpFile)
		/// </summary>
		string TempImageFilePath { get; set; }

		/// <summary>
		///    The invariant validator tests for properties that are
		///    never changed during the course of testing and should
		///    stay the same, even during writing.
		/// </summary>
		public IMetadataInvariantValidator InvariantValidator { get; set; }

		/// <summary>
		///    The modification validator modifies a file and tests the
		///    assumptions about this modification. Default behavior is
		///    to write a file unmodified and see if it's still the same.
		///    Setting this to null disables write testing.
		/// </summary>
		public IMetadataModificationValidator ModificationValidator { get; set; }
	}
}