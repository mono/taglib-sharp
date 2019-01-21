READ_FROM_URI = ReadFromUri/ReadFromUri
SET_PICTURES = SetPictures/SetPictures
LIST_TYPES = ListSupportedMimeTypes
BATCH_SET = BatchSet
PARSE_PHOTO = ParsePhoto
STRIP_IMAGE = StripImageData
GENERATE_TEST_FIXTURE = GenerateTestFixture

REFERENCES = -r:taglib-sharp.dll
EXAMPLES_BUILD = taglib-sharp.dll $(SET_PICTURES).exe $(LIST_TYPES).exe $(BATCH_SET).exe $(PARSE_PHOTO).exe $(STRIP_IMAGE).exe

if HAVE_EXIV2
EXAMPLES_BUILD += $(GENERATE_TEST_FIXTURE).exe
endif

all: $(EXAMPLES_BUILD)

taglib-sharp.dll:
	if [ ! -e $@ ]; then ln -s $(top_builddir)/src/TaglibSharp/bin/$(PROFILE_NAME)/$(TARGET_FRAMEWORK)/taglib-sharp.dll $@; fi

if HAVE_EXIV2
$(GENERATE_TEST_FIXTURE).exe: $(GENERATE_TEST_FIXTURE).cs listData extractKey
	$(CSC) -out:$@ -debug $(REFERENCES) $<

listData: listData.cpp
	g++ -o $@ $(EXIV2_LIBS) $<

extractKey: extractKey.cpp
	g++ -o $@ $(EXIV2_LIBS) $<
endif

$(SET_PICTURES).exe: $(SET_PICTURES).cs
	$(CSC) -out:$@ -debug $(REFERENCES) $<

$(LIST_TYPES).exe: $(LIST_TYPES).cs
	$(CSC) -out:$@ -debug $(REFERENCES) $<

$(BATCH_SET).exe: $(BATCH_SET).cs
	$(CSC) -out:$@ -debug $(REFERENCES) $<

$(PARSE_PHOTO).exe: $(PARSE_PHOTO).cs
	$(CSC) -out:$@ -debug $(REFERENCES) $<

$(STRIP_IMAGE).exe: $(STRIP_IMAGE).cs
	$(CSC) -out:$@ -debug $(REFERENCES) $<

EXTRA_DIST = $(READ_FROM_URI).cs $(SET_PICTURES).cs $(LIST_TYPES).cs $(BATCH_SET).cs $(PARSE_PHOTO).cs $(STRIP_IMAGE).cs $(GENERATE_TEST_FIXTURE).cs \
	$(srcdir)/covers/sample*

DISTCLEANFILES = *.pidb
CLEANFILES = *.dll *.exe *.pdb
MAINTAINERCLEANFILES = Makefile.in

