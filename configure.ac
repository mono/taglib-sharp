AC_INIT(src/TagLib/File.cs)
AC_CANONICAL_SYSTEM
AM_INIT_AUTOMAKE(taglib, 1.0.0.0)
AM_MAINTAINER_MODE

AC_PROG_INSTALL

MONO_REQUIRED=1.1
MONODOC_REQUIRED=1.1.9

dnl Pkgconfig
PKG_CHECK_MODULES(MONO,
		  mono >= $MONO_REQUIRED)

dnl Check for Mono
AC_PATH_PROG(MCS, mcs)
if test x$MCS = x; then
        AC_MSG_ERROR(You need mcs)
fi
AC_PATH_PROG(MONO, mono)
if test x$MONO = x; then
        AC_MSG_ERROR(You need mono)
fi

MCS_FLAGS="-codepage:utf8 -r:Mono.Posix"
MONO_FLAGS=
if test $USE_MAINTAINER_MODE = yes; then
	MCS_FLAGS="$MCS_FLAGS -debug"
	MONO_FLAGS="$MONO_FLAGS --debug"
fi
AC_SUBST(MCS_FLAGS)
AC_SUBST(MONO_FLAGS)

dnl GAC
AC_PATH_PROG(GACUTIL, gacutil)
if test x$GACUTIL = x; then
	AC_MSG_ERROR(You need gacutil)
fi

GACUTIL_FLAGS='/package taglib-sharp /gacdir $(libdir) /root $(DESTDIR)$(libdir)'
AC_SUBST(GACUTIL_FLAGS)

AC_ARG_ENABLE(docs, AC_HELP_STRING([--disable-docs], [Do not build documentation]), with_docs=no, with_docs=yes)

dnl Monodoc
if test "x$with_docs" = "xyes"; then
	AC_PATH_PROG(MONODOCER, monodocer, no)
	if test "x$MONODOCER" = "xno"; then
		AC_MSG_ERROR([You need to install monodoc, or pass --disable-docs to configure to skip documentation installation])
	fi

	AC_PATH_PROG(MDASSEMBLER, mdassembler, no)
	if test "x$MDASSEMBLER" = "xno"; then
		AC_MSG_ERROR([You need to install mdassembler, or pass --disable-docs to configure to skip documentation installation])
	fi

	DOCDIR=`$PKG_CONFIG monodoc --variable=sourcesdir`
	AC_SUBST(DOCDIR)
	AM_CONDITIONAL(BUILD_DOCS, true)
else
	AC_MSG_NOTICE([not building Banshee API documentation])
	AM_CONDITIONAL(BUILD_DOCS, false)
fi

PKG_CHECK_MODULES(GNOME_SHARP, gnome-sharp-2.0, have_gnome_sharp=yes, have_gnome_sharp=no)
if test "x$have_gnome_sharp" = "xyes"; then
	AC_SUBST(GNOME_SHARP_LIBS)
	AM_CONDITIONAL(HAVE_GNOME_SHARP, true)
else
	AM_CONDITIONAL(HAVE_GNOME_SHARP, false)
fi

AC_OUTPUT([
Makefile
taglib-sharp.pc
src/Makefile
src/AssemblyInfo.cs
examples/Makefile
docs/Makefile
])

