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
   taglib-sharp.csproj

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


