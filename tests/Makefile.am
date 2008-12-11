MCS_FLAGS = -debug
NUNIT_FLAGS = @MONO_NUNIT_LIBS@

ASSEMBLY_NAME = tests
ASSEMBLY = $(ASSEMBLY_NAME).dll
ASSEMBLY_CSFILES =  \
	fixtures/Helpers.cs \
	fixtures/TagLib.Tests.Collections/ByteVectorCollectionTest.cs \
	fixtures/TagLib.Tests.Collections/ByteVectorTest.cs \
	fixtures/TagLib.Tests.Collections/StringCollectionTest.cs \
	fixtures/TagLib.Tests.FileFormats/AsfFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/FlacFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3BothFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3V1FormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/Id3V2FormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/IFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/M4aFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/MpcFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/OggFormatTest.cs \
	fixtures/TagLib.Tests.FileFormats/StandardTests.cs \
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

NUNIT_TESTER_NAME = ConsoleUi
NUNIT_TESTER = $(NUNIT_TESTER_NAME).exe
NUNIT_TESTER_CSFILES = $(srcdir)/$(NUNIT_TESTER_NAME).cs $(srcdir)/AssemblyInfo.cs

$(ASSEMBLY): $(ASSEMBLY_CSFILES)
	$(MCS) $(MCS_FLAGS) $(NUNIT_FLAGS) -out:$@ -target:library -r:$(top_builddir)/src/taglib-sharp.dll $(ASSEMBLY_CSFILES)

$(NUNIT_TESTER): $(NUNIT_TESTER_CSFILES)
	$(MCS) $(MCS_FLAGS) -out:$@ $(NUNIT_FLAGS) $(NUNIT_TESTER_CSFILES)

if ENABLE_TESTS
run-test: $(NUNIT_TESTER) $(ASSEMBLY)
	MONO_PATH="../src" mono --debug $(NUNIT_TESTER) $(ASSEMBLY)
endif

CLEANFILES = $(ASSEMBLY) $(NUNIT_TESTER) TestResult.xml *.mdb *.dll *.exe
DISTCLEANFILES = *.pidb
MAINTAINERCLEANFILES = Makefile.in
EXTRA_DIST = \
	$(NUNIT_TESTER_CSFILES) \
	$(ASSEMBLY_CS_FILES) \
	$(srcdir)/samples/* \
	$(srcdir)/samples/unsupported/*

