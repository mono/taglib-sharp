ASSEMBLY_NAME = taglib-sharp
ASSEMBLY = $(ASSEMBLY_NAME).dll
TARGET = $(ASSEMBLY)

CSC = $(MCS) $(MCS_FLAGS) $(CSFLAGS)

include $(srcdir)/TagLib/TagLib.sources

taglib-sharp.snk: $(top_srcdir)/taglib-sharp.snk
	cp $(top_srcdir)/taglib-sharp.snk .

taglib_sources_in = AssemblyInfo.cs.in
taglib_generated_sources = $(taglib_sources_in:.in=)

$(ASSEMBLY): $(TAGLIB_CSFILES) $(taglib_generated_sources) taglib-sharp.snk
	$(CSC) -target:library $(LIBFLAGS) -out:$@ $(TAGLIB_CSFILES) $(taglib_generated_sources)

all: $(ASSEMBLY)

EXTRA_DIST = $(TAGLIB_CSFILES) $(taglib_sources_in) AssemblyInfo-windows.cs
DISTCLEANFILES = *.pidb
CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb taglib-sharp.snk 
MAINTAINERCLEANFILES = Makefile.in

install-data-local:
	@if test -n '$(TARGET)'; then						\
	  echo "$(GACUTIL) /i $(ASSEMBLY) /f $(GACUTIL_FLAGS)";	\
	  $(GACUTIL) /i $(ASSEMBLY) /f $(GACUTIL_FLAGS) || exit 1;	\
	fi

uninstall-local:
	@if test -n '$(TARGET)'; then						\
	  echo "$(GACUTIL) /u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS)";	\
	  $(GACUTIL) /u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS) || exit 1;	\
	fi
