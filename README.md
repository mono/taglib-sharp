# TagLib#

[![Join the chat at https://gitter.im/mono/taglib-sharp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/mono/taglib-sharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Linux Build Status](https://travis-ci.org/mono/taglib-sharp.svg?branch=master)](https://travis-ci.org/mono/taglib-sharp)
[![Windows Build status](https://ci.appveyor.com/api/projects/status/v7vwgphs239i14ya?svg=true)](https://ci.appveyor.com/project/decriptor/taglib-sharp)


(aka *Taglib-sharp*) is a library for reading and writing
metadata in media files, including video, audio, and photo formats.

It supports the following formats (by file extensions):
 * **Video:** mkv, ogv, avi, wmv, asf, mp4 (m4p, m4v), mpeg (mpg, mpe, mpv, mpg, m2v)
	* **Audio:** aa, aax, aac, aiff, ape, flac, m4a, m4b, m4p, mp3, mpc, mpp, ogg, oga, opus, sln, wav, wma, wv, webm
	* **Images:** bmp, gif, jpeg, pbm, pgm, ppm, pnm, pcx, png, tiff, dng, svg

It is API stable, with only API additions (not changes or removals)
occuring in the 2.0 series.

## Website
TagLib# is available on GitHub: <https://github.com/mono/taglib-sharp>
 * **Bugs:**     Create an issue in <https://github.com/mono/taglib-sharp/issues>
 * **Chat:**     Join us at Gitter <https://gitter.im/mono/taglib-sharp>
 * **Git:**      Get the source at <git://github.com/mono/taglib-sharp.git>

## Building and Running

### Command Line  (Linux)

#### To Build From Git:

```sh
git clone https://github.com/mono/taglib-sharp.git
cd taglib-sharp
./autogen.sh && make
```

#### To Build From Tarball:

```
./configure && make
```

#### To Test:

```
make test
```

### MonoDevelop  (Linux)

You can build from MonoDevelop using taglib-sharp.sln

### Visual Studio (Windows):

You can open it in Visual Studio by using taglib-sharp.sln

#### Running regression by using Nunit 3 Test Adapter:
 
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

#### To test some scenarios and take advantage of the debugger:

1. Make the "debug" project the Startup project
    (Right-click on the project, select: "Set as StartUp Project")
2. Just modify the "Program.cs"
3. Set some breakpoints and hit the "Start" button

## Contributions

TagLib# is free/open source software, released under the LGPL.
We welcome contributions!  Please try to match our coding style,
and include unit tests with any patches.  Patches can be submitted
by issuing a Pull Request (Git).

