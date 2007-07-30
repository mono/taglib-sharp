SUBDIRS = \
   src    \
   examples \
   docs

pkgconfig_in_files = taglib-sharp.pc.in
pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = $(pkgconfig_in_files:.pc.in=.pc)

EXTRA_DIST =             \
   $(pkgconfig_in_files) \
   taglib-sharp.snk \
   taglib-sharp.csproj \
   taglib-sharp.mds

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
