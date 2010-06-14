SUBDIRS = \
   src    \
   examples \
   docs

if !IN_DIST
SUBDIRS += tests
endif

DIST_SUBDIRS = \
   src    \
   examples \
   docs

pkgconfig_in_files = taglib-sharp.pc.in
pkgconfigdir = $(datadir)/pkgconfig
pkgconfig_DATA = $(pkgconfig_in_files:.pc.in=.pc)

EXTRA_DIST =             \
   $(pkgconfig_in_files) \
   taglib-sharp.snk \
   taglib-sharp.csproj \
   taglib-sharp.sln

DISTCLEANFILES = taglib-sharp.pc

MAINTAINERCLEANFILES = \
	compile \
	INSTALL \
	config.h.in \
	aclocal.m4 \
	ltmain.sh \
	Makefile.in \
	depcomp \
	missing \
	install-sh \
	configure \
	config.sub \
	config.guess \
	mkinstalldirs

test:
	pushd tests; make test; popd;

distwin: all
	mkdir -p $(srcdir)/$(WINDIR)
	cp $(srcdir)/NEWS $(srcdir)/$(WINDIR)/NEWS.txt
	cp $(srcdir)/ChangeLog $(srcdir)/$(WINDIR)/ChangeLog.txt
	cp $(srcdir)/COPYING $(srcdir)/$(WINDIR)/COPYING.txt
	cp $(srcdir)/taglib-sharp.csproj $(srcdir)/$(WINDIR)/taglib-sharp.csproj
	if [ -e $(srcdir)/windows-files ];					\
	  then cp -r $(srcdir)/windows-files/* $(srcdir)/$(WINDIR);		\
	fi
	cd $(srcdir)/src && make distwin
	zip -9 -r $(srcdir)/$(WINDIR).zip $(srcdir)/$(WINDIR)
	rm -rf $(srcdir)/$(WINDIR)

UPLOAD_HOST = "$$BANSHEE_USER@download.banshee-project.org"
UPLOAD_PATH = "download.banshee-project.org/taglib-sharp"
push-release: distcheck distwin
	[[ -z "$$BANSHEE_USER" ]] && { echo "Set BANSHEE_USER"; exit 1; } || true
	[[ -z "$$MONO_USER" ]] && { echo "Set MONO_USER"; exit 1; } || true
	mkdir -p release
	mv $(WINDIR).zip release
	mv $(PACKAGE)-$(VERSION).tar.* release
	cp ChangeLog release/$(PACKAGE)-$(VERSION).changes
	cp NEWS release/$(PACKAGE)-$(VERSION).news
	pushd release; sha1sum * > $(PACKAGE)-$(VERSION).sha1sum; popd
	scp -r release $(UPLOAD_HOST):$(UPLOAD_PATH)/$(VERSION)
	rm -rf release
	ssh $(UPLOAD_HOST) rm -f $(UPLOAD_PATH)/LATEST-IS\*
	ssh $(UPLOAD_HOST) touch $(UPLOAD_PATH)/LATEST-IS-$(VERSION)
	svn cp \
		-m "Tag for $(PACKAGE) release $(VERSION)" \
		svn+ssh://$$MONO_USER@mono-cvs.ximian.com/source/trunk/taglib-sharp \
		svn+ssh://$$MONO_USER@mono-cvs.ximian.com/source/tags/taglib-sharp-$(VERSION)

