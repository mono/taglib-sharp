DOC_UPDATER = @MONODOCER@ -delete -pretty
DOC_ASSEMBLER = @MDASSEMBLER@ --out taglib-sharp-docs --ecma

ASSEMBLIES = \
	$(top_builddir)/src/taglib-sharp.dll

ASSEMBLIES_BUILD = $(foreach asm,$(ASSEMBLIES),$(addprefix -assembly:,$(asm)))

if BUILD_DOCS
all: MonodocNodeConfig.exe

MonodocNodeConfig.exe: MonodocNodeConfig.cs
	mcs -out:$@ -r:System.Xml $<

monodocdir = $(DESTDIR)$(DOCDIR)
monodoc_DATA = \
	taglib-sharp-docs.zip \
	taglib-sharp-docs.tree \
	taglib-sharp-docs.source

update-docs: $(ASSEMBLIES)
	$(DOC_UPDATER) $(ASSEMBLIES_BUILD) -path:en

update-html:
	if [ -d taglib-sharp-web-docs ]; then \
		rm -rf taglib-sharp-web-docs; \
	fi; \
	mkdir taglib-sharp-web-docs; \
	monodocs2html --source en --dest taglib-sharp-web-docs;

update-slashdoc:
	monodocs2slashdoc en;

taglib-sharp-docs.zip taglib-sharp-docs.tree: $(srcdir)/en/*/*.xml $(srcdir)/en/*.xml
	$(DOC_ASSEMBLER) $(srcdir)/en

install-data-hook:
	$(MONO) $(top_builddir)/docs/MonodocNodeConfig.exe --insert "TagLib#" classlib-taglib-sharp $(DOCDIR)/../monodoc.xml

uninstall-hook:
	$(MONO) $(top_builddir)/docs/MonodocNodeConfig.exe --remove classlib-taglib-sharp $(DOCDIR)/../monodoc.xml
endif

EXTRA_DIST = \
	$(srcdir)/en/*.xml \
	$(srcdir)/en/*/*.xml \
	taglib-sharp-docs.source \
	MonodocNodeConfig.cs
	
DISTCLEANFILES = \
	taglib-sharp-docs.zip \
	taglib-sharp-docs.tree

MAINTAINERCLEANFILES = \
	Makefile.in

CLEANFILES = \
	MonodocNodeConfig.exe

