DOC_UPDATER = @MONODOCER@ -delete -pretty
DOC_ASSEMBLER = @MDASSEMBLER@ --out taglib-sharp-docs --ecma

ASSEMBLY = $(top_builddir)/src/$(ASSEMBLY_NAME).dll
SLASHDOC = $(ASSEMBLY).xml

if BUILD_DOCS
all: XmlInjector.exe

XmlInjector.exe: XmlInjector.cs
	mcs -out:$@ -r:System.Xml $<

monodocdir = $(DESTDIR)$(DOCDIR)
monodoc_DATA = \
	taglib-sharp-docs.zip \
	taglib-sharp-docs.tree \
	taglib-sharp-docs.source

$(srcdir)/en/index.xml: $(ASSEMBLY) $(SLASHDOC) $(srcdir)/XmlInjector.exe $(srcdir)/Package.en.xml
# monodocer will create duplicate tags if run multiple times. Delete the whole
# directory and start over.
	if [ -d $(srcdir)/en ]; then \
		rm -rf $(srcdir)/en; \
	fi
	monodocer /assembly:$(ASSEMBLY) /importslashdoc:$(SLASHDOC) /path:en
	$(MONO) $(srcdir)/XmlInjector.exe $(srcdir)/Package.en.xml InsertMissingValues

update-html: $(srcdir)/en
	if [ -d taglib-sharp-web-docs ]; then \
		rm -rf taglib-sharp-web-docs; \
	fi; \
	mkdir taglib-sharp-web-docs; \
	monodocs2html --source en --dest taglib-sharp-web-docs;

taglib-sharp-docs.zip taglib-sharp-docs.tree: $(srcdir)/en/index.xml
	$(DOC_ASSEMBLER) $(srcdir)/en

install-data-hook: $(srcdir)/XmlInjector.exe $(srcdir)/Package.en.xml
	$(MONO) $(srcdir)/XmlInjector.exe $(srcdir)/Package.en.xml InjectMenuItem

uninstall-hook: $(srcdir)/XmlInjector.exe $(srcdir)/Package.en.xml
	$(MONO) $(srcdir)/XmlInjector.exe $(srcdir)/Package.en.xml RemoveMenuItem

clean-local:
	if [ -d $(srcdir)/en ]; then \
		rm -rf $(srcdir)/en; \
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
