AC_INIT([taglib-sharp], [2.2.0.0])
AM_INIT_AUTOMAKE([1.14.1 dist-bzip2 tar-ustar foreign])
AM_MAINTAINER_MODE

ASSEMBLY_NAME="$PACKAGE"
ASSEMBLY_VERSION="$VERSION"
TARGET_FRAMEWORK="net462"

AC_SUBST(VERSION)
AC_SUBST(ASSEMBLY_VERSION)
AC_SUBST(ASSEMBLY_NAME)
AC_SUBST(TARGET_FRAMEWORK)

AC_PROG_INSTALL
AC_PROG_GREP


MONODOC_REQUIRED=1.1.9

WINDIR="$ASSEMBLY_NAME-$VERSION-windows"
AC_SUBST(WINDIR)

DISTCHECK_CONFIGURE_FLAGS="--disable-docs"
AC_SUBST(DISTCHECK_CONFIGURE_FLAGS)

dnl Check for Mono
AC_PATH_PROG(NUGET, nuget)
if test x$NUGET = x; then
	AC_MSG_ERROR(You need nuget)
fi

AC_PATH_PROG(CSC, csc)
if test x$CSC = x; then
	AC_MSG_ERROR(You need csc)
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

	AC_PATH_PROG(MDASSEMBLER, mdassembler, no)
	if test "x$MDASSEMBLER" = "xno"; then
		AC_MSG_ERROR([You need to install mdassembler, or pass --disable-docs to configure to skip documentation installation])
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

AC_ARG_ENABLE(release,
	      AC_HELP_STRING([--enable-release],
			     [Use release configuration]),
			     enable_release=yes, enable_release=`(test ! -d .git) && echo "yes" || echo "no"`)
AM_CONDITIONAL(ENABLE_RELEASE, test "x$enable_release" = "xyes")
ENABLE_RELEASE=$enable_release
AC_SUBST(ENABLE_RELEASE)
if test "x$enable_release" = "xyes"; then
	CSC_FLAGS="-define:RElEASE"
	PROFILE_NAME="Release"
else
	PROFILE_NAME="Debug"
fi
AC_SUBST(CSC_FLAGS)
AC_SUBST(PROFILE_NAME)

AC_CONFIG_FILES([
Makefile
taglib-sharp.pc
src/Makefile
src/TaglibSharp/Makefile
src/Debug/Makefile
src/TaglibSharp.Tests/Makefile
examples/Makefile
docs/Makefile
docs/Package.en.xml
])

AC_OUTPUT

echo "TagLib# ($ASSEMBLY_NAME-$ASSEMBLY_VERSION) is ready to be compiled."
