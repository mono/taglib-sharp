AC_INIT([taglib-sharp], [2.2.0.0])
AM_INIT_AUTOMAKE([1.14.1 dist-bzip2 tar-ustar foreign])
AM_MAINTAINER_MODE

ASSEMBLY_NAME="$PACKAGE"
ASSEMBLY_VERSION="$VERSION"

POLICY_2_0_VERSIONS="2.0.0.0-$ASSEMBLY_VERSION"
AC_SUBST(POLICY_2_0_VERSIONS)

AC_SUBST(VERSION)
AC_SUBST(ASSEMBLY_VERSION)
AC_SUBST(ASSEMBLY_NAME)

AC_PROG_INSTALL
AC_PROG_GREP

MONODOC_REQUIRED=1.1.9

WINDIR="$ASSEMBLY_NAME-$VERSION-windows"
AC_SUBST(WINDIR)

DISTCHECK_CONFIGURE_FLAGS="--disable-docs"
AC_SUBST(DISTCHECK_CONFIGURE_FLAGS)

dnl Check for Mono
AC_PATH_PROG(MCS, csc)
if test x$MCS = x; then
	AC_MSG_ERROR(You need mcs)
fi

AC_PATH_PROG(MONO, mono)
if test x$MONO = x; then
	AC_MSG_ERROR(You need mono)
fi

AC_PATH_PROG(AL, al)
if test "x$AL" = "x"; then
	AC_MSG_ERROR([No al tool found. You need to install mono])
fi

dnl GAC
AC_PATH_PROG(GACUTIL, gacutil)
if test x$GACUTIL = x; then
	AC_MSG_ERROR(You need gacutil)
fi

GACUTIL_FLAGS='/package $(ASSEMBLY_NAME) /gacdir $(prefix)/lib /root $(DESTDIR)$(prefix)/lib'
AC_SUBST(GACUTIL_FLAGS)
GACUTIL_POLICY_FLAGS='/gacdir $(prefix)/lib /root $(DESTDIR)$(prefix)/lib'
AC_SUBST(GACUTIL_POLICY_FLAGS)

AC_ARG_ENABLE(docs, AC_HELP_STRING([--disable-docs], [Do not build documentation]), with_docs=no, with_docs=yes)

PKG_PROG_PKG_CONFIG

dnl Monodoc
if test "x$with_docs" = "xyes"; then
	AC_PATH_PROG(MONODOCER, monodocer, no)
	if test "x$MONODOCER" = "xno"; then
		AC_MSG_ERROR([You need to install monodoc, or pass --disable-docs to configure to skip documentation installation])
	fi

	DOCDIR=`$PKG_CONFIG monodoc --variable=sourcesdir`
	AC_SUBST(DOCDIR)
	AM_CONDITIONAL(BUILD_DOCS, true)
else
	AC_MSG_NOTICE([not building TaglibSharp API documentation])
	AM_CONDITIONAL(BUILD_DOCS, false)
fi

PKG_CHECK_MODULES(EXIV2, exiv2, have_exiv2=yes, have_exiv2=no)
if test "x$have_exiv2" = "xyes"; then
	AC_SUBST(EXIV2_LIBS)
	AM_CONDITIONAL(HAVE_EXIV2, true)
else
	AM_CONDITIONAL(HAVE_EXIV2, false)
fi

AC_CONFIG_FILES([
Makefile
taglib-sharp.pc
src/Makefile
src/Debug/Makefile
src/policy.2.0.taglib-sharp.config
src/TaglibSharp/Makefile
src/TaglibSharp.Tests/Makefile
examples/Makefile
docs/Makefile
docs/Package.en.xml
])

AC_OUTPUT

echo "TagLib# ($ASSEMBLY_NAME-$ASSEMBLY_VERSION) is ready to be compiled."
