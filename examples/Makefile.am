READ_FROM_URI = ReadFromUri

REFERENCES = -r:taglib-sharp.dll
EXAMPLES_BUILD = taglib-sharp.dll

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

EXTRA_DIST = $(READ_FROM_URI).cs

CLEANFILES = *.dll *.exe *.mdb
MAINTAINERCLEANFILES = Makefile.in

