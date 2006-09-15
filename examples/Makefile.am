READ_FROM_URI = ReadFromUri
SET_PICTURES = SetPictures

REFERENCES = -r:taglib-sharp.dll
EXAMPLES_BUILD = taglib-sharp.dll $(SET_PICTURES).exe

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

EXTRA_DIST = $(READ_FROM_URI).cs $(SET_PICTURES).cs

CLEANFILES = *.dll *.exe *.mdb
MAINTAINERCLEANFILES = Makefile.in

