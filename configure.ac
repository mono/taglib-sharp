AC_INIT(src/TagLib/File.cs)
AC_CANONICAL_SYSTEM

ASSEMBLY_NAME="taglib-sharp"

VERSION="2.0.2.20"
ASSEMBLY_VERSION="$VERSION"

POLICY_2_0_VERSIONS="2.0.0.0-$ASSEMBLY_VERSION"
AC_SUBST(POLICY_2_0_VERSIONS)


AC_SUBST(VERSION)
AC_SUBST(ASSEMBLY_VERSION)
AC_SUBST(ASSEMBLY_NAME)

AM_INIT_AUTOMAKE($ASSEMBLY_NAME, $VERSION)
AM_MAINTAINER_MODE

AC_PROG_INSTALL

MONODOC_REQUIRED=1.1.9

WINDIR="$ASSEMBLY_NAME-$VERSION-windows"
AC_SUBST(WINDIR)

DISTCHECK_CONFIGURE_FLAGS="--disable-docs"
AC_SUBST(DISTCHECK_CONFIGURE_FLAGS)

dnl Check for Mono
AC_PATH_PROG(MCS, gmcs)
if test x$MCS = x; then
	AC_MSG_ERROR(You need mcs)
fi

AC_PATH_PROG(MONO, mono)
if test x$MONO = x; then
	AC_MSG_ERROR(You need mono)
fi

MCS_FLAGS="-codepage:utf8 -debug"
MONO_FLAGS=

if test $USE_MAINTAINER_MODE = yes; then
	MONO_FLAGS="$MONO_FLAGS --debug"
fi

AC_SUBST(MCS_FLAGS)
AC_SUBST(MONO_FLAGS)

dnl GAC
AC_PATH_PROG(GACUTIL, gacutil)
if test x$GACUTIL = x; then
	AC_MSG_ERROR(You need gacutil)
fi

GACUTIL_FLAGS='/package $(ASSEMBLY_NAME) /gacdir $(libdir) /root $(DESTDIR)$(libdir)'
AC_SUBST(GACUTIL_FLAGS)
GACUTIL_POLICY_FLAGS='/gacdir $(libdir) /root $(DESTDIR)$(libdir)'
AC_SUBST(GACUTIL_POLICY_FLAGS)

AC_ARG_ENABLE(docs, AC_HELP_STRING([--disable-docs], [Do not build documentation]), with_docs=no, with_docs=yes)

PKG_PROG_PKG_CONFIG

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

#PKG_CHECK_MODULES(MONO_NUNIT, mono-nunit >= 1.0, do_tests="yes", do_tests="no") AC_SUBST(MONO_NUNIT_LIBS)
#AM_CONDITIONAL(ENABLE_TESTS, test "x$do_tests" = "xyes")
#if test "x$do_tests" = "xno"; then
#	AC_MSG_WARN([Could not find mono-nunit: tests will not be available.])
#fi

AC_OUTPUT([
Makefile
taglib-sharp.pc
src/Makefile
src/AssemblyInfo.cs
src/policy.2.0.taglib-sharp.config
examples/Makefile
docs/Makefile
])

echo "TagLib# ($ASSEMBLY_NAME-$ASSEMBLY_VERSION) is ready to be compiled."
