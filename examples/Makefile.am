READ_FROM_URI = ReadFromUri
SET_PICTURES = SetPictures
LIST_TYPES = ListSupportedMimeTypes
BATCH_SET = BatchSet

REFERENCES = -r:taglib-sharp.dll
EXAMPLES_BUILD = taglib-sharp.dll $(SET_PICTURES).exe $(LIST_TYPES).exe $(BATCH_SET).exe

if HAVE_GNOME_SHARP
EXAMPLES_BUILD += $(READ_FROM_URI).exe
endif

all: $(EXAMPLES_BUILD)

taglib-sharp.dll:
	if [ ! -e $@ ]; then ln -s $(top_builddir)/src/taglib-sharp.dll $@; fi

if HAVE_GNOME_SHARP
$(READ_FROM_URI).exe: $(READ_FROM_URI).cs
	$(MCS) -out:$@ -debug $(REFERENCES) $(GNOME_SHARP_LIBS) $<
endif

$(SET_PICTURES).exe: $(SET_PICTURES).cs
	$(MCS) -out:$@ -debug $(REFERENCES) $<

$(LIST_TYPES).exe: $(LIST_TYPES).cs
	$(MCS) -out:$@ -debug $(REFERENCES) $<

$(BATCH_SET).exe: $(BATCH_SET).cs
	$(MCS) -out:$@ -debug $(REFERENCES) $<

EXTRA_DIST = $(READ_FROM_URI).cs $(SET_PICTURES).cs $(LIST_TYPES).cs $(BATCH_SET).cs
DISTCLEANFILES = *.pidb
CLEANFILES = *.dll *.exe *.mdb
MAINTAINERCLEANFILES = Makefile.in

