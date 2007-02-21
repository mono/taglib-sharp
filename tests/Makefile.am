MCS_FLAGS = -debug
NUNIT_FLAGS = @MONO_NUNIT_LIBS@

ASSEMBLY_NAME = tests
ASSEMBLY = $(ASSEMBLY_NAME).dll
ASSEMBLY_CSFILES = \
	$(srcdir)/CodeTimer.cs \
	$(srcdir)/fixtures/*/*.cs

NUNIT_TESTER_NAME = ConsoleUi
NUNIT_TESTER = $(NUNIT_TESTER_NAME).exe
NUNIT_TESTER_CSFILES = $(srcdir)/$(NUNIT_TESTER_NAME).cs $(srcdir)/AssemblyInfo.cs

$(ASSEMBLY): $(ASSEMBLY_CSFILES)
	$(MCS) $(MCS_FLAGS) $(NUNIT_FLAGS) -out:$@ -target:library -r:$(top_builddir)/src/taglib-sharp.dll $(ASSEMBLY_CSFILES)

$(NUNIT_TESTER): $(NUNIT_TESTER_CSFILES)
	$(MCS) $(MCS_FLAGS) -out:$@ $(NUNIT_FLAGS) $(NUNIT_TESTER_CSFILES)

if ENABLE_TESTS
run-test: $(NUNIT_TESTER) $(ASSEMBLY)
	MONO_PATH="../src" mono --debug $(NUNIT_TESTER) $(ASSEMBLY)
endif

CLEANFILES = $(ASSEMBLY) $(NUNIT_TESTER) TestResult.xml *.mdb *.dll *.exe
DISTCLEANFILES = *.pidb
MAINTAINERCLEANFILES = Makefile.in
EXTRA_DIST = \
	$(NUNIT_TESTER_CSFILES) \
	$(ASSEMBLY_CS_FILES) \
	$(srcdir)/samples/* \
	$(srcdir)/samples/unsupported/*

