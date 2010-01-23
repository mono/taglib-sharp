using Gdk;
using System;
using NUnit.Framework;

namespace TagLib.Tests.Images.Validators
{
	public class ImageTest
	{
		static ImageTest () {
			// Initialize GDK
			var args = Environment.GetCommandLineArgs ();
			Global.InitCheck (ref args);
		}

		byte [] pre_bytes;
		byte [] post_bytes;

		public bool CompareImageData {
			get; set;
		}

		public static void Run (string filename, IMetadataInvariantValidator invariant)
		{
			Run (filename, true, invariant);
		}

		public static void Run (string filename, bool compare_image_data, IMetadataInvariantValidator invariant)
		{
			Run (filename, compare_image_data, invariant, NoModificationValidator.Instance);
		}


		public static void Run (string filename, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			Run (filename, true, invariant, modifications);
		}

		public static void Run (string filename, bool compare_image_data, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			foreach (var modification in modifications) {
				ImageTest test = new ImageTest () {
					ImageFileName = filename,
					CompareImageData = compare_image_data,
					InvariantValidator = invariant,
					ModificationValidator = modification
				};
				test.TestImage ();
			}
		}

		void TestImage ()
		{
			ParseUnmodifiedFile ();

			if (ModificationValidator == null)
				return;

			ModifyFile ();
			ParseModifiedFile ();
		}

		/// <summary>
		///    Parse the unmodified file.
		/// </summary>
		void ParseUnmodifiedFile ()
		{
			var file = ReadFile (ImageFileName);
			InvariantValidator.ValidateMetadataInvariants (file);
			if (CompareImageData)
				pre_bytes = ReadImageData (file);
		}

		/// <summary>
		///    Modify and save the file.
		/// </summary>
		void ModifyFile ()
		{
			CreateTmpFile ();
			var tmp = ReadFile (TempImageFileName);
			InvariantValidator.ValidateMetadataInvariants (tmp);
			ModificationValidator.ValidatePreModification (tmp);
			ModificationValidator.ModifyMetadata (tmp);
			ModificationValidator.ValidatePostModification (tmp);
			tmp.Save ();
		}

		/// <summary>
		///    Re-parse the modified file.
		/// </summary>
		void ParseModifiedFile ()
		{
			var tmp = ReadFile (TempImageFileName);
			InvariantValidator.ValidateMetadataInvariants (tmp);
			ModificationValidator.ValidatePostModification (tmp);
			if (CompareImageData) {
				post_bytes = ReadImageData (tmp);
				ValidateImageData ();
			}
		}

		Image.File ReadFile (string filename)
		{
			var full_filename = String.Format ("samples/{0}", filename);
			return File.Create (full_filename) as Image.File;
		}

		/// <summary>
		///    Only files we can actually render are supported, for most types
		///    this just means adding the type here. RAW files need extra
		///    attention.
		/// </summary>
		bool IsSupportedImageFile (Image.File file)
		{
			return file is Jpeg.File;
		}

		byte[] ReadImageData (Image.File file)
		{
			if (!IsSupportedImageFile (file))
				Assert.Fail("Unsupported type for data reading: "+file);

			file.Mode = File.AccessMode.Read;
			ByteVector v = file.ReadBlock ((int) file.Length);
			byte [] result = null;
			using (Pixbuf buf = new Pixbuf(v.Data))
				result = buf.SaveToBuffer("png");
			file.Mode = File.AccessMode.Closed;
			return result;
		}

		void ValidateImageData ()
		{
			string label = String.Format ("Image data mismatch for {0}/{1}", ImageFileName, ModificationValidator);
			Assert.AreEqual (pre_bytes, post_bytes, label);
		}

		void CreateTmpFile ()
		{
			var orig = String.Format ("samples/{0}", ImageFileName);
			var tmp = String.Format ("samples/{0}", TempImageFileName);
			if (System.IO.File.Exists (tmp))
				System.IO.File.Delete (tmp);
			System.IO.File.Copy (orig, tmp);
		}

		/// <summary>
		///    The filename of the file to test. Name only, no paths.
		/// </summary>
		string ImageFileName {
			get; set;
		}

		string TempImageFileName {
			get { return String.Format ("tmpwrite_{0}", ImageFileName); }
		}

		/// <summary>
		///    The invariant validator tests for properties that are
		///    never changed during the course of testing and should
		///    stay the same, even during writing.
		/// </summary>
		public IMetadataInvariantValidator InvariantValidator {
			get; set;
		}

		/// <summary>
		///    The modification validator modifies a file and tests the
		///    assumptions about this modification. Default behavior is
		///    to write a file unmodified and see if it's still the same.
		///    Setting this to null disables write testing.
		/// </summary>
		public IMetadataModificationValidator ModificationValidator {
			get; set;
		}
	}
}
