# TagLib# [![Join the chat at https://gitter.im/mono/taglib-sharp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/mono/taglib-sharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Build Status](https://travis-ci.org/mono/taglib-sharp.svg?branch=master)](https://travis-ci.org/mono/taglib-sharp)

(aka taglib-sharp) is a library for reading and writing
metadata in media files, including video, audio, and photo formats.

It is API stable, with only API additions (not changes or removals)
occuring in the 2.0 series.

 * Bugs:     <https://bugzilla.gnome.org/page.cgi?id=browse.html&product=taglib-sharp>
 * Tarballs: <http://banshee.fm/download/>
 * IRC:      Several TagLib# developers are often in #banshee on irc.gnome.org
 * Git:      <https://github.com/mono/taglib-sharp> or <git://github.com/mono/taglib-sharp.git>

TagLib# is free/open source software, released under the LGPL.
We welcome contributions!  Please try to match our coding style,
and include unit tests with any patches.  Patches can be submitted
by filing a bug and attaching the diff to it.

## Building and Running

### To Build From Git:

```sh
git clone https://github.com/mono/taglib-sharp.git
cd taglib-sharp
./autogen.sh && make
```

### To Build From Tarball:

```
./configure && make
```

### To Test:

```
make test
```

You can also build from MonoDevelop or Visual Studio using taglib-sharp.sln

### To Test/Run from Visual Studio (Windows):

Running regression by using Nunit 3 Test Adapter:
 
1. Ensure NuGet packages have been restored
    1. See: <https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore>
2. In Visual Studio, go to menu: Tools > Extensions and Updates > Online
3. Search: Nunit 3 Test Adapter
4. Download and install it
5. Open from menu: Test > Windows > Test Explorer
6. You can run your tests from this panel (*not* using the "Start" button)
7. You can debug your tests from this panel:
   1. Double click on a test. Set some breakpoints in the test in the editor panel.
   2. right-click on the same test, select "Debug Selected tests".

To test some scenarios and take advantage of the debugger:

1. Make the "debug" project the Startup project
    (Right-click on the project, select: "Set as StartUp Project")
2. Just modify the "Program.cs"
3. Set some breakpoints and hit the "Start" button
