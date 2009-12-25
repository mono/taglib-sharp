using System;
using NUnit.Framework;

namespace TagLib.Tests.Images.Validators
{
	public class ImageTest
	{
		public static void Run (string filename, IMetadataInvariantValidator invariant)
		{
			Run (filename, invariant, NoModificationValidator.Instance);
		}

		public static void Run (string filename, IMetadataInvariantValidator invariant, params IMetadataModificationValidator[] modifications)
		{
			foreach (var modification in modifications) {
				ImageTest test = new ImageTest () {
					ImageFileName = filename,
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
		}

		Image.File ReadFile (string filename)
		{
			var full_filename = String.Format ("samples/{0}", filename);
			return File.Create (full_filename) as Image.File;
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
