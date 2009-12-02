MCS_FLAGS = -debug
NUNIT_FLAGS = @MONO_NUNIT_LIBS@

ASSEMBLY_NAME = tests
ASSEMBLY = $(ASSEMBLY_NAME).dll
ASSEMBLY_CSFILES =  \
	fixtures/Helpers.cs \
	fixtures/TagLib.Tests.Collections/ByteVectorCollectionTest.cs \
	fixtures/TagLib.Tests.Collections/ByteVectorTest.cs \
	fixtures/TagLib.Tests.Collections/FileTest.cs \
	fixtures/TagLib.Tests.Collections/StringCollectionTest.cs \
	fixtures/TagLib.Tests.FileFormats/AacFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/AiffFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/AsfFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/AviFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/FlacFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3BothFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3V1FormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3V24FormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3V2FormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/IFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/JpegFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/M4aFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/MpcFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/OggFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/TiffFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/StandardTests.cs \
	fixtures/TagLib.Tests.Images/AddImageMetadataTests.cs \
	fixtures/TagLib.Tests.Images/JpegEmptyTest.cs \
	fixtures/TagLib.Tests.Images/JpegCanonZoombrowserTest.cs \
	fixtures/TagLib.Tests.Images/JpegNikon1Test.cs \
	fixtures/TagLib.Tests.Images/JpegNikon2Test.cs \
	fixtures/TagLib.Tests.Images/JpegNikon3Test.cs \
	fixtures/TagLib.Tests.Images/JpegNikon4Test.cs \
	fixtures/TagLib.Tests.Images/JpegSony1Test.cs \
	fixtures/TagLib.Tests.Images/JpegOlympus1Test.cs \
	fixtures/TagLib.Tests.Images/JpegOlympus2Test.cs \
	fixtures/TagLib.Tests.Images/JpegOlympus3Test.cs \
	fixtures/TagLib.Tests.Images/JpegPanasonicTest.cs \
	fixtures/TagLib.Tests.Images/JpegSegmentSizeTest.cs \
	fixtures/TagLib.Tests.Images/JpegTangledTest.cs \
	fixtures/TagLib.Tests.Images/TiffXmpTest.cs \
	fixtures/TagLib.Tests.Images/Utils.cs \
	fixtures/TagLib.Tests.Images/XapTest.cs \
	fixtures/TagLib.Tests.Images/XmpCrashTest.cs \
	fixtures/TagLib.Tests.Images/XmpSpecTest.cs \
	fixtures/TagLib.Tests.Performance/PerformanceTest.cs \
	fixtures/TagLib.Tests.TaggingFormats/ApeTest.cs \
	fixtures/TagLib.Tests.TaggingFormats/AsfTest.cs \
	fixtures/TagLib.Tests.TaggingFormats/DivXTest.cs \
	fixtures/TagLib.Tests.TaggingFormats/Id3V1Test.cs \
	fixtures/TagLib.Tests.TaggingFormats/Id3V2Test.cs \
	fixtures/TagLib.Tests.TaggingFormats/InfoTagTest.cs \
	fixtures/TagLib.Tests.TaggingFormats/MovieIdTagTest.cs \
	fixtures/TagLib.Tests.TaggingFormats/Mpeg4Test.cs \
	fixtures/TagLib.Tests.TaggingFormats/XiphTest.cs

$(ASSEMBLY): $(ASSEMBLY_CSFILES)
	$(MCS) $(MCS_FLAGS) $(NUNIT_FLAGS) -out:$@ -target:library -r:$(top_builddir)/src/taglib-sharp.dll $(ASSEMBLY_CSFILES)

TAGLIB_ASM = ../src/taglib-sharp.dll

if ENABLE_TESTS
test: $(ASSEMBLY) $(TAGLIB_ASM)
	MONO_PATH="../src" $(NUNIT_CMD) -noshadow $(ASSEMBLY)
endif

CLEANFILES = $(ASSEMBLY) TestResult.xml *.mdb *.dll *.exe
DISTCLEANFILES = *.pidb
MAINTAINERCLEANFILES = Makefile.in
EXTRA_DIST = \
	$(ASSEMBLY_CSFILES) \
	$(srcdir)/samples/sample* \
	$(srcdir)/samples/vector.bin \
	$(srcdir)/samples/LICENSE \
	$(srcdir)/samples/corrupt/a.* \
	$(srcdir)/samples/corrupt/null_title_v2.mp3 \
	$(srcdir)/samples/unsupported/sample*

