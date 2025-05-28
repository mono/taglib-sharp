//
// This application parses a photo and compares the output to the output of exiv2.
//
// It can be used to make test fixtures. Manual validation is always required.
//
// You need the exiv2 app for this to work.
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;
using TagLib;
using TagLib.IFD;
using TagLib.IFD.Tags;
using TagLib.Xmp;

namespace GenerateTestFixture;

public class Program
{
	static MD5 md5 = MD5.Create ();

	// Helper to run a process and capture output, error, and exit code
	static bool RunProcess (string exe, string args, out string output, out string error, out int exitCode)
	{
		var startInfo = new ProcessStartInfo {
			FileName = exe,
			Arguments = args,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		using var process = new Process { StartInfo = startInfo };
		process.Start ();
		output = process.StandardOutput.ReadToEnd ();
		error = process.StandardError.ReadToEnd ();
		process.WaitForExit ();
		exitCode = process.ExitCode;
		return exitCode == 0;
	}

	public static void Main (string[] args)
	{
		if (args.Length != 2) {
			Console.Error.WriteLine ("USAGE: mono GenerateTestFixture.exe NAME PATH");
			return;
		}

		string name = args[0];
		string path = args[1];

		EmitHeader (name, path);
		GenerateIFDFixture (name, path);
		GenerateXMPFixture (name, path);
		EmitFooter ();
	}

	static Dictionary<string, int> sub_ifds = new Dictionary<string, int> ();
	static Dictionary<string, bool> sub_ifds_emitted = new Dictionary<string, bool> ();

	static void GenerateIFDFixture (string name, string path)
	{
		// First run exiv2 on it.
		var result = RunProcess ("listData", $"e {path}", out var output, out var err, out var code);
		if (!result) {
			Console.Error.WriteLine ("Invoking listData failed, are you running from the examples folder?\n" + err);
			return;
		}

		Write ("//  ---------- Start of IFD tests ----------");

		foreach (string line in output.Split ('\n')) {
			string[] parts = line.Split (['\t'], 5);
			if (parts.Length == 0 || line.Trim ().Equals (string.Empty) || parts.Length != 5)
				continue;
			string tag_label = parts[0];
			ushort tag = ushort.Parse (parts[1].Substring (2), System.Globalization.NumberStyles.HexNumber);
			string ifd = parts[2];
			string type = parts[3];
			uint length = uint.Parse (parts[4]);

			if (ifd == "NikonSi02xx" || ifd == "NikonVr" || ifd == "NikonPc" || ifd == "NikonWt" || ifd == "NikonIi" || ifd == "NikonLd3") {
				continue; // Exiv2 makes these up.
			}

			string val = ExtractKey (path, $"Exif.{ifd}.{tag_label}");

			if (tag_label == "SubIFDs") {
				for (int i = 0; i < val.Split (' ').Length; i++) {
					var sub_ifd = $"SubImage{sub_ifds.Count + 1}";
					sub_ifds.Add (sub_ifd, sub_ifds.Count);
				}
				continue;
			}

			EnsureIFD (ifd);

			if (tag_label.Equals ("ExifTag"))
				type = "SubIFD";
			if (tag_label.Equals ("MakerNote")) {
				type = "MakerNote";
				val = string.Empty; // No need to echo.
			}
			if (tag_label.Equals ("InteroperabilityTag"))
				type = "SubIFD";
			if (tag_label.Equals ("GPSTag"))
				type = "SubIFD";
			if (tag_label.Equals ("JPEGInterchangeFormat"))
				type = "ThumbnailDataIFD";
			if (tag_label.Equals ("Preview") && ifd.Equals ("Nikon3"))
				type = "SubIFD";
			if (tag_label.Equals ("UserComment") && ifd.Equals ("Photo"))
				type = "UserComment";
			if (tag_label.Equals ("StripOffsets"))
				type = "StripOffsets";
			if (tag_label.Equals ("IPTCNAA"))
				type = "IPTCNAA";
			if (tag_label.Equals ("XMLPacket"))
				type = "XMLPacket";

			if (ifd.Equals ("MakerNote"))
				continue; // Exiv2 makes these up.

			Write ($"// {ifd}.0x{tag:X4} ({tag_label}/{type}/{length}) \"{(length > 512 ? "(Value ommitted)" : val)}\"");

			if (ifd.Equals ("Image")) {
				EmitTestIFDEntryOpen ("structure", 0, tag, ifd);
			} else if (ifd.Equals ("Thumbnail")) {
				EmitTestIFDEntryOpen ("structure", 1, tag, ifd);
			} else if (ifd.Equals ("Image2")) {
				EmitTestIFDEntryOpen ("structure", 2, tag, ifd);
			} else if (ifd.Equals ("Image3")) {
				EmitTestIFDEntryOpen ("structure", 3, tag, ifd);
			} else if (ifd.Equals ("Photo")) {
				EmitTestIFDEntryOpen ("exif_structure", 0, tag, ifd);
			} else if (IsPartOfMakernote (ifd)) {
				EmitTestIFDEntryOpen ("makernote_structure", 0, tag, ifd);
			} else if (ifd.Equals ("NikonPreview")) {
				EmitTestIFDEntryOpen ("nikonpreview_structure", 0, tag, ifd);
			} else if (ifd.Equals ("Iop")) {
				EmitTestIFDEntryOpen ("iop_structure", 0, tag, ifd);
			} else if (ifd.Equals ("GPSInfo")) {
				EmitTestIFDEntryOpen ("gps_structure", 0, tag, ifd);
			} else if (ifd.Equals ("CanonCs")) {
				EmitTestIFDEntryOpen ("makernote_structure", 0, (ushort)CanonMakerNoteEntryTag.CameraSettings, ifd);
			} else if (ifd.Equals ("CanonSi")) {
				EmitTestIFDEntryOpen ("makernote_structure", 0, (ushort)CanonMakerNoteEntryTag.ShotInfo, ifd);
			} else if (ifd.Equals ("CanonCf")) {
				EmitTestIFDEntryOpen ("makernote_structure", 0, (ushort)CanonMakerNoteEntryTag.CustomFunctions, ifd);
			} else if (ifd.Equals ("CanonPi")) {
				EmitTestIFDEntryOpen ("makernote_structure", 0, (ushort)CanonMakerNoteEntryTag.PictureInfo, ifd);
			} else if (ifd.Equals ("CanonFi")) {
				EmitTestIFDEntryOpen ("makernote_structure", 0, (ushort)0x93, ifd);
			} else if (ifd.Equals ("PanasonicRaw")) {
				EmitTestIFDEntryOpen ("pana_structure", 0, tag, ifd);
			} else if (sub_ifds.ContainsKey (ifd)) {
				EmitTestIFDEntryOpen ($"{ifd}_structure", 0, tag, ifd);
			} else {
				throw new Exception ($"Unknown IFD: {ifd}");
			}

			if (ifd.Equals ("CanonCs") || ifd.Equals ("CanonSi") || ifd.Equals ("CanonCf") || ifd.Equals ("CanonPi")) {
				// This are a made-up directory by exiv2
				EmitTestIFDIndexedShortEntry (tag, val);
			} else if (ifd.Equals ("CanonFi")) {
				// This are a made-up directory by exiv2
				// And the fist both entries are combined to a long by exiv2.
				if (tag == 0x0001) {
					string val1 = ((ushort)uint.Parse (val)).ToString ();
					string val2 = ((ushort)(uint.Parse (val) >> 16)).ToString ();
					EmitTestIFDIndexedShortEntry (tag, val1);
					EmitTestIFDIndexedShortEntry (tag + 1, val2);
				} else {
					EmitTestIFDIndexedShortEntry (tag, val);
				}
			} else if (type.Equals ("Ascii")) {
				EmitTestIFDStringEntry (val);
			} else if (type.Equals ("Short") && length == 1) {
				EmitTestIFDShortEntry (val);
			} else if (type.Equals ("Short") && length > 1) {
				EmitTestIFDShortArrayEntry (val);
			} else if (type.Equals ("SShort") && length == 1) {
				EmitTestIFDSShortEntry (val);
			} else if (type.Equals ("SShort") && length > 1) {
				EmitTestIFDSShortArrayEntry (val);
			} else if (type.Equals ("Rational") && length == 1) {
				EmitTestIFDRationalEntry (val);
			} else if (type.Equals ("Rational") && length > 1) {
				EmitTestIFDRationalArrayEntry (val);
			} else if (type.Equals ("SRational") && length == 1) {
				EmitTestIFDSRationalEntry (val);
			} else if (type.Equals ("SRational") && length > 1) {
				EmitTestIFDSRationalArrayEntry (val);
			} else if (type.Equals ("Long") && length == 1) {
				EmitTestIFDLongEntry (val);
			} else if (type.Equals ("Long") && length > 1) {
				EmitTestIFDLongArrayEntry (val);
			} else if (type.Equals ("SLong") && length == 1) {
				EmitTestIFDSLongEntry (val);
			} else if (type.Equals ("Byte") && length == 1) {
				EmitTestIFDByteEntry (val);
			} else if (type.Equals ("Byte") && length > 1) {
				EmitTestIFDByteArrayEntry (val);
			} else if (type.Equals ("SByte") && length == 1) {
				EmitTestIFDSByteEntry (val);
			} else if (type.Equals ("SubIFD")) {
				EmitTestIFDSubIFDEntry (val);
			} else if (type.Equals ("ThumbnailDataIFD")) {
				EmitTestIFDThumbnailDataIFDEntry (val);
			} else if (type.Equals ("MakerNote")) {
				EmitTestIFDMakerNoteIFDEntry (val);
			} else if (type.Equals ("UserComment")) {
				EmitTestIFDUserCommentIFDEntry (val);
			} else if (type.Equals ("Undefined")) {
				EmitTestIFDUndefinedEntry (val);
			} else if (type.Equals ("StripOffsets")) {
				EmitTestIFDStripOffsetsEntry (val);
			} else if (type.Equals ("IPTCNAA")) {
				EmitTestIFDIPTCNAAEntry (val);
			} else if (type.Equals ("XMLPacket")) {
				EmitTestIFDXMLPacketEntry (val);
			} else {
				throw new Exception ("Unknown type: " + type);
			}

			EmitTestIFDEntryClose ();
		}

		Write ();
		Write ("//  ---------- End of IFD tests ----------");
		Write ();
	}

	static Dictionary<string, string> xmp_prefixes = new Dictionary<string, string> ();

	static void GenerateXMPFixture (string name, string path)
	{
		// First run exiv2 on it.
		var result = RunProcess ("listData", $"x {path}", out var output, out var err, out var code);
		if (!result) {
			Console.Error.WriteLine ("Invoking exiv2 failed, do you have it installed?\n" + err);
			return;
		}

		if (output.Trim ().Equals (""))
			return;

		Write ();
		Write ("//  ---------- Start of XMP tests ----------");
		Write ();

		Write ("XmpTag xmp = file.GetTag (TagTypes.XMP) as XmpTag;");

		// Build prefix lookup dictionary.
		Type t = typeof (XmpTag);
		foreach (var member in t.GetMembers ()) {
			if (!member.Name.EndsWith ("_NS"))
				continue;
			string val = (member as System.Reflection.FieldInfo).GetValue (null) as string;
			string prefix = XmpTag.NamespacePrefixes[val];
			xmp_prefixes[prefix] = member.Name;
		}

		foreach (string line in output.Split ('\n')) {
			string[] parts = line.Split (['\t'], 3);
			if (parts.Length == 0 || line.Trim ().Equals (string.Empty))
				continue;
			string label = parts[0];
			string type = parts[1];
			uint length = uint.Parse (parts[2]);
			string val = ExtractKey (path, label).Trim ();

			EmitXmpTest (label, type, length, val);
		}

		Write ();
		Write ("//  ---------- End of XMP tests ----------");
		Write ();
	}

	static void EmitXmpTest (string label, string type, uint length, string val)
	{
		if (label.Equals ("Xmp.xmpMM.InstanceID"))
			return; // Continue this, exiv2 makes it up from the about attr
		if (label.Equals ("Xmp.tiff.Orientation"))
			return; // exiv2 destroys this value

		var node_path = label.Split ('/');
		Write ($"// {label} ({type}/{length}) \"{val}\"");
		Write ("{");
		Write ("var node = xmp.NodeTree;");

		// Navigate to the correct node.
		foreach (var node in node_path) {
			var parts = node.Split ('.');
			var partscolon = node.Split (':');
			if (parts.Length == 3) {
				// Plain node
				int index = 0;
				string name = parts[2];
				if (parts[2].EndsWith ("]")) {
					int index_start = parts[2].LastIndexOf ("[");
					string index_str = parts[2].Substring (index_start + 1, parts[2].Length - index_start - 2);
					index = int.Parse (index_str);
					name = parts[2].Substring (0, index_start);
				}
				string ns = GetXmpNs (parts[1]);
				Write ($"node = node.GetChild ({ns}, \"{name}\");");
				Write ("Assert.IsNotNull (node);");

				if (index > 0) {
					Write ($"node = node.Children [{index - 1}];");
					Write ("Assert.IsNotNull (node);");
				}
			} else if (partscolon.Length == 2) {
				string ns = GetXmpNs (partscolon[0]);
				string name = partscolon[1];
				Write ($"node = node.GetChild ({ns}, \"{name}\");");
				Write ("Assert.IsNotNull (node);");
			} else {
				throw new Exception ("Can't navigate to " + node);
			}
		}

		if (length > 0 && type.Equals ("XmpText")) {
			Write ($"Assert.AreEqual (\"{val}\", node.Value);");
			Write ("Assert.AreEqual (XmpNodeType.Simple, node.Type);");
			Write ("Assert.AreEqual (0, node.Children.Count);");
		} else if (type.Equals ("XmpBag") && length == 1) {
			Write ("Assert.AreEqual (XmpNodeType.Bag, node.Type);");
			Write ("Assert.AreEqual (\"\", node.Value);");
			Write ($"Assert.AreEqual ({length}, node.Children.Count);");
			Write ($"Assert.AreEqual (\"{val}\", node.Children [0].Value);");
		} else if (type.Equals ("LangAlt") && length == 1) {
			var langparts = val.Split ([' '], 2);
			string lang = langparts[0].Substring (langparts[0].IndexOf ('"') + 1, langparts[0].Length - langparts[0].IndexOf ('"') - 2);
			Write ($"Assert.AreEqual (\"{lang}\", node.Children [0].GetQualifier (XmpTag.XML_NS, \"lang\").Value);");
			Write ($"Assert.AreEqual (\"{langparts[1]}\", node.Children [0].Value);");
		} else if (type.Equals ("XmpSeq") && length == 1) {
			Write ("Assert.AreEqual (XmpNodeType.Seq, node.Type);");
			Write ("Assert.AreEqual (\"\", node.Value);");
			Write ($"Assert.AreEqual ({length}, node.Children.Count);");
			Write ($"Assert.AreEqual (\"{val}\", node.Children [0].Value);");
		} else if (type.Equals ("XmpSeq") && length > 1) {
			string[] vals = val.Split (',');
			Write ("Assert.AreEqual (XmpNodeType.Seq, node.Type);");
			Write ("Assert.AreEqual (\"\", node.Value);");
			Write ($"Assert.AreEqual ({length}, node.Children.Count);");
			var per_iter = vals.Length / length;
			for (int i = 0; i < length; i++) {
				var builder = new List<string> ();
				for (int j = 0; j < per_iter; j++) {
					builder.Add (vals[per_iter * i + j].Trim ());
				}
				Write ($"Assert.AreEqual (\"{string.Join (", ", builder.ToArray ())}\", node.Children [{i}].Value);");
			}
		} else if (type.Equals ("XmpBag") && length > 1) {
			string[] vals = val.Split (',');
			Write ("Assert.AreEqual (XmpNodeType.Bag, node.Type);");
			Write ("Assert.AreEqual (\"\", node.Value);");
			Write ($"Assert.AreEqual ({length}, node.Children.Count);");
			Write ("var children_array = new System.Collections.Generic.List<string> ();");
			Write ("foreach (var child in node.Children)");
			Write ("{");
			Write ("children_array.Add (child.Value);");
			Write ("}");
			var per_iter = vals.Length / length;
			for (int i = 0; i < length; i++) {
				var builder = new List<string> ();
				for (int j = 0; j < per_iter; j++) {
					builder.Add (vals[per_iter * i + j].Trim ());
				}
				Write ($"Assert.IsTrue (children_array.Contains (\"{string.Join (", ", builder.ToArray ())}\"));");
			}
		} else if (type.Equals ("XmpText") && length == 0 && val.StartsWith ("type=")) {
			if (val.Equals ("type=\"Bag\"")) {
				Write ("Assert.AreEqual (XmpNodeType.Bag, node.Type);");
			} else if (val.Equals ("type=\"Struct\"")) {
				// We disagree with exiv2 on the meaning of Struct. In Taglib#,
				// struct is meant to denote parseType=Resource types only, not
				// the shorthand equivalent. Also see XmpNode.RenderInto()
				//Write ("Assert.AreEqual (XmpNodeType.Struct, node.Type);");
			} else {
				throw new Exception ("Unknown type");
			}
		} else {
			throw new Exception ($"Can't test this (type: {type}, length: {length})");
		}
		Write ("}");
	}

	static string ExtractKey (string file, string key)
	{
		var result = RunProcess ("extractKey", $"{file} {key}", out var output, out var err, out var code);
		if (!result) {
			Console.Error.WriteLine ("Invoking extractKey failed, are you running from the examples folder?\n" + err);
			return string.Empty;
		}

		return output;
	}

	static string GetXmpNs (string prefix)
	{
		if (prefix.Equals ("xmpBJ"))
			prefix = "xapBJ";
		if (prefix.Equals ("xmpMM"))
			prefix = "xapMM";
		if (prefix.Equals ("xmpRights"))
			prefix = "xapRights";
		if (prefix.Equals ("MicrosoftPhoto_1_")) // We correct this invalid namespace internally
			prefix = "MicrosoftPhoto";
		if (xmp_prefixes.TryGetValue (prefix, out var result))
			return $"XmpTag.{result}";
		throw new Exception ("Unknown namespace prefix: " + prefix);
	}

	static bool IsPartOfMakernote (string ifd)
	{
		return ifd.Equals ("MakerNote") ||
			   ifd.Equals ("Canon") ||
			   ifd.Equals ("Sony") ||
			   ifd.Equals ("Nikon1") ||
			   ifd.Equals ("Nikon2") ||
			   ifd.Equals ("Nikon3") ||
			   ifd.Equals ("Panasonic") ||
			   ifd.Equals ("Olympus") ||
			   ifd.Equals ("Pentax");
	}

	static void EmitHeader (string name, string path)
	{
		int start = path.LastIndexOf ('/');
		string filename = path.Substring (start + 1);
		Write ("// TODO: This file is automatically generated");
		Write ("// TODO: Further manual verification is needed");
		Write ();
		Write ("using System;");
		Write ("using NUnit.Framework;");
		Write ("using TagLib.IFD;");
		Write ("using TagLib.IFD.Entries;");
		Write ("using TagLib.IFD.Tags;");
		Write ("using TagLib.Xmp;");
		Write ("using TagLib.Tests.Images.Validators;");
		Write ();
		Write ("namespace TagLib.Tests.Images");
		Write ("{");
		Write ("[TestFixture]");
		Write ($"public class {name}");
		Write ("{");
		Write ("[Test]");
		Write ("public void Test ()");
		Write ("{");
		Write ($"ImageTest.Run (\"{filename}\",");
		level++;
		Write ($"new {name}InvariantValidator (),");
		Write ("NoModificationValidator.Instance");
		level--;
		Write (");");
		Write ("}");
		Write ("}");
		Write ();
		Write ($"public class {name}InvariantValidator : IMetadataInvariantValidator");
		Write ("{");
		Write ("public void ValidateMetadataInvariants (Image.File file)");
		Write ("{");
		Write ("Assert.IsNotNull (file);");
	}

	static void EmitFooter ()
	{
		Write ("}"); // Method
		Write ("}"); // Class
		Write ("}"); // Namespace
	}

	static bool is_panasonic_raw = false;

	static bool structure_emitted = false;
	static bool exif_emitted = false;
	static bool makernote_emitted = false;
	static bool makernote_is_canon = false;
	static bool makernote_is_nikon1 = false;
	static bool makernote_is_nikon2 = false;
	static bool makernote_is_nikon3 = false;
	static bool makernote_is_panasonic = false;
	static bool nikonpreview_emitted = false;
	static bool iop_emitted = false;
	static bool gps_emitted = false;

	static void EnsureIFD (string ifd)
	{
		if (ifd.Equals ("PanasonicRaw")) {
			if (is_panasonic_raw)
				return;

			Write ();
			Write ("var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;");
			Write ("Assert.IsNotNull (tag, \"IFD tag not found\");");
			Write ();
			Write ("var pana_structure = tag.Structure;");
			Write ();
			Write ("var jpg_file = (file as TagLib.Tiff.Rw2.File).JpgFromRaw;");
			Write ("Assert.IsNotNull (tag, \"JpgFromRaw not found!\");");
			Write ("var jpg_tag = jpg_file.GetTag (TagTypes.TiffIFD) as IFDTag;");
			Write ("Assert.IsNotNull (tag, \"Jpg has no Exif tag!\");");
			Write ("var structure = jpg_tag.Structure;");

			is_panasonic_raw = true;
		}

		if (ifd.Equals ("Image") && !is_panasonic_raw) {
			if (structure_emitted)
				return;
			Write ();
			Write ("var tag = file.GetTag (TagTypes.TiffIFD) as IFDTag;");
			Write ("Assert.IsNotNull (tag, \"IFD tag not found\");");
			Write ();
			Write ("var structure = tag.Structure;");
			Write ();
			structure_emitted = true;
		}

		if (ifd.Equals ("Photo")) {
			if (exif_emitted)
				return;
			EnsureIFD ("Image");
			Write ();
			Write ("var exif = structure.GetEntry (0, (ushort) IFDEntryTag.ExifIFD) as SubIFDEntry;");
			Write ("Assert.IsNotNull (exif, \"Exif tag not found\");");
			Write ("var exif_structure = exif.Structure;");
			Write ();
			exif_emitted = true;
		}

		if (ifd.Equals ("MakerNote")) {
			if (makernote_emitted)
				return;
			EnsureIFD ("Photo");
			Write ();
			Write ("var makernote = exif_structure.GetEntry (0, (ushort) ExifEntryTag.MakerNote) as MakernoteIFDEntry;");
			Write ("Assert.IsNotNull (makernote, \"MakerNote tag not found\");");
			Write ("var makernote_structure = makernote.Structure;");
			Write ();
			makernote_emitted = true;
		}

		if (ifd.Equals ("Canon") || ifd.Equals ("CanonCs") || ifd.Equals ("CanonSi")) {
			if (makernote_is_canon)
				return;
			EnsureIFD ("MakerNote");
			Write ();
			Write ("Assert.AreEqual (MakernoteType.Canon, makernote.MakernoteType);");
			Write ();
			makernote_is_canon = true;
		}

		if (ifd.Equals ("Nikon1")) {
			if (makernote_is_nikon1)
				return;
			EnsureIFD ("MakerNote");
			Write ();
			Write ("Assert.AreEqual (MakernoteType.Nikon1, makernote.MakernoteType);");
			Write ();
			makernote_is_nikon1 = true;
		}

		if (ifd.Equals ("Nikon2")) {
			if (makernote_is_nikon2)
				return;
			EnsureIFD ("MakerNote");
			Write ();
			Write ("Assert.AreEqual (MakernoteType.Nikon2, makernote.MakernoteType);");
			Write ();
			makernote_is_nikon2 = true;
		}

		if (ifd.Equals ("Nikon3")) {
			if (makernote_is_nikon3)
				return;
			EnsureIFD ("MakerNote");
			Write ();
			Write ("Assert.AreEqual (MakernoteType.Nikon3, makernote.MakernoteType);");
			Write ();
			makernote_is_nikon3 = true;
		}

		if (ifd.Equals ("NikonPreview")) {
			if (nikonpreview_emitted)
				return;
			EnsureIFD ("Nikon3");
			Write ();
			Write ("var nikonpreview = makernote_structure.GetEntry (0, (ushort) Nikon3MakerNoteEntryTag.Preview) as SubIFDEntry;");
			Write ("Assert.IsNotNull (nikonpreview, \"Nikon preview tag not found\");");
			Write ("var nikonpreview_structure = nikonpreview.Structure;");
			Write ();
			nikonpreview_emitted = true;
		}

		if (ifd.Equals ("Panasonic")) {
			if (makernote_is_panasonic)
				return;
			EnsureIFD ("MakerNote");
			Write ();
			Write ("Assert.AreEqual (MakernoteType.Panasonic, makernote.MakernoteType);");
			Write ();
			makernote_is_panasonic = true;
		}

		if (ifd.Equals ("Iop")) {
			if (iop_emitted)
				return;
			EnsureIFD ("Photo");
			Write ();
			Write ("var iop = exif_structure.GetEntry (0, (ushort) IFDEntryTag.InteroperabilityIFD) as SubIFDEntry;");
			Write ("Assert.IsNotNull (iop, \"Iop tag not found\");");
			Write ("var iop_structure = iop.Structure;");
			Write ();
			iop_emitted = true;
		}

		if (ifd.Equals ("GPSInfo")) {
			if (gps_emitted)
				return;
			EnsureIFD ("Image");
			Write ();
			Write ("var gps = structure.GetEntry (0, (ushort) IFDEntryTag.GPSIFD) as SubIFDEntry;");
			Write ("Assert.IsNotNull (gps, \"GPS tag not found\");");
			Write ("var gps_structure = gps.Structure;");
			Write ();
			gps_emitted = true;
		}

		if (sub_ifds.ContainsKey (ifd) && !sub_ifds_emitted.ContainsKey (ifd)) {
			Write ();
			Write ($"var {ifd}_structure = (structure.GetEntry (0, (ushort) IFDEntryTag.SubIFDs) as SubIFDArrayEntry).Entries [{sub_ifds[ifd]}];");
			Write ($"Assert.IsNotNull ({ifd}_structure, \"{ifd} structure not found\");");
			Write ();
			sub_ifds_emitted.Add (ifd, true);
		}
	}

	static void EmitTestIFDEntryOpen (string src, int ifd, ushort tag, string ifd_label)
	{
		Write ("{");
		Write ($"var entry = {src}.GetEntry ({ifd}, (ushort) {StringifyEntryTag (ifd_label, tag)});");
		Write ($"Assert.IsNotNull (entry, \"Entry 0x{tag:X4} missing in IFD {ifd}\");");
	}

	static void EmitTestIFDEntryClose ()
	{
		Write ("}");
	}

	static void EmitTestIFDStringEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as StringIFDEntry, \"Entry is not a string!\");");
		Write ($"Assert.AreEqual (\"{val}\", (entry as StringIFDEntry).Value{(val == string.Empty ? ".Trim ()" : "")});");
	}

	static void EmitTestIFDShortEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as ShortIFDEntry, \"Entry is not a short!\");");
		Write ($"Assert.AreEqual ({val}, (entry as ShortIFDEntry).Value);");
	}

	static void EmitTestIFDSShortEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as SShortIFDEntry, \"Entry is not a signed short!\");");
		Write ($"Assert.AreEqual ({val}, (entry as SShortIFDEntry).Value);");
	}

	static void EmitTestIFDShortArrayEntry (string val)
	{
		val = $"new ushort [] {{ {string.Join (", ", val.Split (' '))} }}";
		Write ("Assert.IsNotNull (entry as ShortArrayIFDEntry, \"Entry is not a short array!\");");
		Write ($"Assert.AreEqual ({val}, (entry as ShortArrayIFDEntry).Values);");
	}

	static void EmitTestIFDSShortArrayEntry (string val)
	{
		val = $"new short [] {{ {string.Join (", ", val.Split (' '))} }}";
		Write ("Assert.IsNotNull (entry as SShortArrayIFDEntry, \"Entry is not a signed short array!\");");
		Write ($"Assert.AreEqual ({val}, (entry as SShortArrayIFDEntry).Values);");
	}

	static void EmitTestIFDRationalEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as RationalIFDEntry, \"Entry is not a rational!\");");
		string[] parts = val.Split ('/');
		Write ($"Assert.AreEqual ({parts[0]}, (entry as RationalIFDEntry).Value.Numerator);");
		Write ($"Assert.AreEqual ({parts[1]}, (entry as RationalIFDEntry).Value.Denominator);");
	}

	static void EmitTestIFDRationalArrayEntry (string val)
	{
		var parts = val.Split (' ');
		Write ("Assert.IsNotNull (entry as RationalArrayIFDEntry, \"Entry is not a rational array!\");");
		Write ("var parts = (entry as RationalArrayIFDEntry).Values;");
		Write ($"Assert.AreEqual ({parts.Length}, parts.Length);");
		for (int i = 0; i < parts.Length; i++) {
			var pieces = parts[i].Split ('/');
			Write ($"Assert.AreEqual ({pieces[0]}, parts[{i}].Numerator);");
			Write ($"Assert.AreEqual ({pieces[1]}, parts[{i}].Denominator);");
		}
	}

	static void EmitTestIFDSRationalEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as SRationalIFDEntry, \"Entry is not a srational!\");");
		string[] parts = val.Split ('/');
		Write ($"Assert.AreEqual ({parts[0]}, (entry as SRationalIFDEntry).Value.Numerator);");
		Write ($"Assert.AreEqual ({parts[1]}, (entry as SRationalIFDEntry).Value.Denominator);");
	}

	static void EmitTestIFDSRationalArrayEntry (string val)
	{
		var parts = val.Split (' ');
		Write ("Assert.IsNotNull (entry as SRationalArrayIFDEntry, \"Entry is not a srational array!\");");
		Write ("var parts = (entry as SRationalArrayIFDEntry).Values;");
		Write ($"Assert.AreEqual ({parts.Length}, parts.Length);");
		for (int i = 0; i < parts.Length; i++) {
			var pieces = parts[i].Split ('/');
			Write ($"Assert.AreEqual ({pieces[0]}, parts[{i}].Numerator);");
			Write ($"Assert.AreEqual ({pieces[1]}, parts[{i}].Denominator);");
		}
	}

	static void EmitTestIFDLongEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as LongIFDEntry, \"Entry is not a long!\");");
		Write ($"Assert.AreEqual ({val}, (entry as LongIFDEntry).Value);");
	}

	static void EmitTestIFDLongArrayEntry (string val)
	{
		val = $"new long [] {{ {string.Join (", ", val.Split (' '))} }}";
		Write ("Assert.IsNotNull (entry as LongArrayIFDEntry, \"Entry is not a long array!\");");
		Write ($"Assert.AreEqual ({val}, (entry as LongArrayIFDEntry).Values);");
	}

	static void EmitTestIFDSLongEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as SLongIFDEntry, \"Entry is not a signed long!\");");
		Write ($"Assert.AreEqual ({val}, (entry as SLongIFDEntry).Value);");
	}

	static void EmitTestIFDByteEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as ByteIFDEntry, \"Entry is not a byte!\");");
		Write ($"Assert.AreEqual ({val}, (entry as ByteIFDEntry).Value);");
	}

	static void EmitTestIFDByteArrayEntry (string val)
	{
		EmitByteArrayComparison (val, "ByteVectorIFDEntry", "a byte array");
	}

	static void EmitTestIFDSByteEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as SByteIFDEntry, \"Entry is not a signed byte!\");");
		Write ($"Assert.AreEqual ({val}, (entry as SByteIFDEntry).Value);");
	}

	static void EmitTestIFDIPTCNAAEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as ByteVectorIFDEntry, \"Entry is not a byte array!\");");
	}

	static void EmitTestIFDXMLPacketEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as ByteVectorIFDEntry, \"Entry is not a byte array!\");");
	}

	static void EmitTestIFDUndefinedEntry (string val)
	{
		EmitByteArrayComparison (val, "UndefinedIFDEntry", "an undefined IFD entry");
	}

	static void EmitByteArrayComparison (string val, string type, string type_desc)
	{
		Write ($"Assert.IsNotNull (entry as {type}, \"Entry is not {type_desc}!\");");
		Write ($"var parsed_bytes = (entry as {type}).Data.Data;");
		var parts = val.Trim ().Split (' ');
		if (parts.Length < 512) {
			Write ($"var bytes = new byte [] {{ {string.Join (", ", parts)} }};");
			Write ("Assert.AreEqual (bytes, parsed_bytes);");
		} else {
			// Starting with 512 byte items, we compare based on an MD5 hash, should be faster and reduces
			// the size of the test fixtures.
			byte[] data = new byte[parts.Length];
			for (int i = 0; i < parts.Length; i++) {
				data[i] = byte.Parse (parts[i]);
			}
			var hash = md5.ComputeHash (data);

			var shash = new StringBuilder ();
			for (int i = 0; i < hash.Length; i++) {
				shash.Append (hash[i].ToString ("x2"));
			}

			Write ("var parsed_hash = Utils.Md5Encode (parsed_bytes);");
			Write ($"Assert.AreEqual (\"{shash}\", parsed_hash);");
			Write ($"Assert.AreEqual ({parts.Length}, parsed_bytes.Length);");
		}
	}

	static void EmitTestIFDSubIFDEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as SubIFDEntry, \"Entry is not a sub IFD!\");");
	}

	static void EmitTestIFDThumbnailDataIFDEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as ThumbnailDataIFDEntry, \"Entry is not a thumbnail IFD!\");");
	}

	static void EmitTestIFDMakerNoteIFDEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as MakernoteIFDEntry, \"Entry is not a makernote IFD!\");");
	}

	static void EmitTestIFDUserCommentIFDEntry (string val)
	{
		Write ("Assert.IsNotNull (entry as UserCommentIFDEntry, \"Entry is not a user comment!\");");
		if (val.StartsWith ("charset=\"Ascii\""))
			val = val.Substring (15).Trim ();
		Write ($"Assert.AreEqual (\"{val}\", (entry as UserCommentIFDEntry).Value.Trim ());");
	}

	static void EmitTestIFDStripOffsetsEntry (string val)
	{
		// The offsets may change after writing. Therfore we cannot compare them directly.
		string offset_count = $"{val.Split (' ').Length}";
		//val = String.Format ("new long [] {{ {0} }}", String.Join (", ", val.Split(' ')));
		Write ("Assert.IsNotNull (entry as StripOffsetsIFDEntry, \"Entry is not a strip offsets entry!\");");
		//Write ("Assert.AreEqual ({0}, (entry as StripOffsetsIFDEntry).Values);", val);
		Write ($"Assert.AreEqual ({offset_count}, (entry as StripOffsetsIFDEntry).Values.Length);");
	}

	static void EmitTestIFDIndexedShortEntry (int index, string val)
	{
		Write ("Assert.IsNotNull (entry as ShortArrayIFDEntry, \"Entry is not a short array!\");");
		var parts = val.Trim ().Split (' ');
		Write ($"Assert.IsTrue ({index + parts.Length} <= (entry as ShortArrayIFDEntry).Values.Length);");
		for (int i = 0; i < parts.Length; i++)
			Write ($"Assert.AreEqual ({parts[i]}, (entry as ShortArrayIFDEntry).Values [{index + i}]);");
	}

	#region IFD tag names lookup

	static Dictionary<string, Dictionary<ushort, string>> tag_names = null;

	static string StringifyEntryTag (string src, ushort tag)
	{
		if (tag_names == null)
			BuildTagNamesTable ();
		if (tag_names.TryGetValue (src, out var table)) {
			if (table.TryGetValue (tag, out var result))
				return result;
		}
		Write ($"// TODO: Unknown IFD tag: {src} / 0x{tag:X4}");
		return $"0x{tag:X4}";
	}

	static void BuildTagNamesTable ()
	{
		tag_names = new Dictionary<string, Dictionary<ushort, string>> ();

		IndexTagType ("Image", typeof (IFDEntryTag), "IFDEntryTag");
		IndexTagType ("Image2", typeof (IFDEntryTag), "IFDEntryTag");
		IndexTagType ("Image3", typeof (IFDEntryTag), "IFDEntryTag");
		IndexTagType ("SubImage1", typeof (IFDEntryTag), "IFDEntryTag");
		IndexTagType ("SubImage2", typeof (IFDEntryTag), "IFDEntryTag");
		IndexTagType ("Thumbnail", typeof (IFDEntryTag), "IFDEntryTag"); // IFD1, for thumbnails
		IndexTagType ("Photo", typeof (IFDEntryTag), "IFDEntryTag");
		IndexTagType ("Photo", typeof (ExifEntryTag), "ExifEntryTag");
		IndexTagType ("Image", typeof (ExifEntryTag), "ExifEntryTag"); // Also put exif into Image, for DNG
		IndexTagType ("GPSInfo", typeof (GPSEntryTag), "GPSEntryTag");
		IndexTagType ("Iop", typeof (IOPEntryTag), "IOPEntryTag");
		IndexTagType ("Canon", typeof (CanonMakerNoteEntryTag), "CanonMakerNoteEntryTag");
		IndexTagType ("CanonCs", typeof (CanonMakerNoteEntryTag), "CanonMakerNoteEntryTag");
		IndexTagType ("CanonSi", typeof (CanonMakerNoteEntryTag), "CanonMakerNoteEntryTag");
		IndexTagType ("CanonCf", typeof (CanonMakerNoteEntryTag), "CanonMakerNoteEntryTag");
		IndexTagType ("CanonFi", typeof (CanonFileInfoEntryTag), "CanonFileInfoEntryTag");
		IndexTagType ("CanonFi", typeof (CanonMakerNoteEntryTag), "CanonMakerNoteEntryTag");
		IndexTagType ("CanonPi", typeof (CanonPictureInfoEntryTag), "CanonPictureInfoEntryTag");
		IndexTagType ("CanonPi", typeof (CanonMakerNoteEntryTag), "CanonMakerNoteEntryTag");
		IndexTagType ("Sony", typeof (SonyMakerNoteEntryTag), "SonyMakerNoteEntryTag");
		IndexTagType ("Olympus", typeof (OlympusMakerNoteEntryTag), "OlympusMakerNoteEntryTag");
		IndexTagType ("Pentax", typeof (PentaxMakerNoteEntryTag), "PentaxMakerNoteEntryTag");
		IndexTagType ("Nikon3", typeof (Nikon3MakerNoteEntryTag), "Nikon3MakerNoteEntryTag");
		IndexTagType ("NikonPreview", typeof (NikonPreviewMakerNoteEntryTag), "NikonPreviewMakerNoteEntryTag");
		IndexTagType ("Panasonic", typeof (PanasonicMakerNoteEntryTag), "PanasonicMakerNoteEntryTag");
		IndexTagType ("PanasonicRaw", typeof (IFDEntryTag), "IFDEntryTag");
	}

	static void IndexTagType (string ifd, Type t, string typename)
	{
		if (!tag_names.ContainsKey (ifd))
			tag_names[ifd] = new Dictionary<ushort, string> ();
		foreach (string name in Enum.GetNames (t)) {
			ushort tag = (ushort)Enum.Parse (t, name);
			tag_names[ifd][tag] = $"{typename}.{name}";
		}
	}

	#endregion

	#region Code emission

	static int level = 0;

	static void Write (string str, params object[] p)
	{
		Console.Write (new string ('\t', level));
		Console.WriteLine (str, p);
	}

	static void Write ()
	{
		Console.WriteLine ();
	}

	static void Write (string str)
	{
		if (str.Equals ("}"))
			level--;
		Console.Write (new string ('\t', level));
		Console.WriteLine (str);
		if (str.Equals ("{"))
			level++;
	}

	#endregion
}
