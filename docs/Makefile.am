DOC_UPDATER = @MONODOCER@ -delete -pretty
DOC_ASSEMBLER = @MDASSEMBLER@ --out taglib-sharp-docs --ecma

VARIABLES = $(PROFILE_NAME)/$(TARGET_FRAMEWORK)/$(ASSEMBLY_NAME)
ASSEMBLY = $(top_builddir)/src/TaglibSharp/bin/$(VARIABLES).dll
SLASHDOC = $(top_builddir)/src/TaglibSharp/bin/$(VARIABLES).xml

if BUILD_DOCS
all: XmlInjector.exe

XmlInjector.exe: XmlInjector.cs
	$(CSC) -out:$@ $<

monodocdir = $(DOCDIR)
monodoc_DATA = \
	taglib-sharp-docs.zip \
	taglib-sharp-docs.tree \
	taglib-sharp-docs.source

en/index.xml: $(ASSEMBLY) $(SLASHDOC) XmlInjector.exe Package.en.xml
# monodocer will create duplicate tags if run multiple times. Delete the whole
# directory and start over.
	if [ -d en ]; then \
		rm -rf en; \
	fi
	$(DOC_UPDATER) -assembly:$(ASSEMBLY) -importslashdoc:$(SLASHDOC) -path:en > /dev/null
	$(MONO) XmlInjector.exe Package.en.xml InsertMissingValues

update-html: en
	if [ -d taglib-sharp-web-docs ]; then \
		rm -rf taglib-sharp-web-docs; \
	fi; \
	mkdir taglib-sharp-web-docs; \
	monodocs2html -o taglib-sharp-web-docs en;

taglib-sharp-docs.tree: taglib-sharp-docs.zip

taglib-sharp-docs.zip: en/index.xml
	$(DOC_ASSEMBLER) en

install-data-hook: XmlInjector.exe Package.en.xml
	$(MONO) XmlInjector.exe Package.en.xml InjectMenuItem

uninstall-hook: XmlInjector.exe Package.en.xml
	$(MONO) XmlInjector.exe Package.en.xml RemoveMenuItem

clean-local:
	if [ -d en ]; then \
		rm -rf en; \
	fi

endif

EXTRA_DIST = \
     	     taglib-sharp-docs.source \
     	     XmlInjector.cs \
     	     Package.en.xml.in

DISTCLEANFILES = \
	 	 taglib-sharp-docs.zip \
	 	 taglib-sharp-docs.tree

MAINTAINERCLEANFILES = \
	       	       Makefile.in

CLEANFILES = \
	     XmlInjector.exe
