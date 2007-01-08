ASSEMBLY_NAME = taglib-sharp
ASSEMBLY = $(ASSEMBLY_NAME).dll
TARGET = $(ASSEMBLY)

CSC = $(MCS) $(MCS_FLAGS) $(CSFLAGS)

TAGLIB_CSFILES =                                                   \
   $(srcdir)/TagLib/Ape/Tag.cs                                     \
   $(srcdir)/TagLib/Ape/Item.cs                                    \
   $(srcdir)/TagLib/Ape/Footer.cs                                  \
   $(srcdir)/TagLib/Asf/Guid.cs                                    \
   $(srcdir)/TagLib/Asf/Properties.cs                              \
   $(srcdir)/TagLib/Asf/StreamPropertiesObject.cs                  \
   $(srcdir)/TagLib/Asf/UnknownObject.cs                           \
   $(srcdir)/TagLib/Asf/Tag.cs                                     \
   $(srcdir)/TagLib/Asf/Object.cs                                  \
   $(srcdir)/TagLib/Asf/ExtendedContentDescriptionObject.cs        \
   $(srcdir)/TagLib/Asf/ContentDescriptionObject.cs                \
   $(srcdir)/TagLib/Asf/HeaderObject.cs                            \
   $(srcdir)/TagLib/Asf/File.cs                                    \
   $(srcdir)/TagLib/Asf/FilePropertiesObject.cs                    \
   $(srcdir)/TagLib/Asf/PaddingObject.cs                           \
   $(srcdir)/TagLib/Asf/ContentDescriptor.cs                       \
   $(srcdir)/TagLib/Mpc/Properties.cs                              \
   $(srcdir)/TagLib/Mpc/File.cs                                    \
   $(srcdir)/TagLib/Ogg/Flac/File.cs                               \
   $(srcdir)/TagLib/Ogg/Page.cs                                    \
   $(srcdir)/TagLib/Ogg/XiphComment.cs                             \
   $(srcdir)/TagLib/Ogg/Vorbis/Properties.cs                       \
   $(srcdir)/TagLib/Ogg/Vorbis/File.cs                             \
   $(srcdir)/TagLib/Ogg/PageHeader.cs                              \
   $(srcdir)/TagLib/Ogg/File.cs                                    \
   $(srcdir)/TagLib/Flac/Properties.cs                             \
   $(srcdir)/TagLib/Flac/Picture.cs                                \
   $(srcdir)/TagLib/Flac/File.cs                                   \
   $(srcdir)/TagLib/Mpeg/Properties.cs                             \
   $(srcdir)/TagLib/Mpeg/XingHeader.cs                             \
   $(srcdir)/TagLib/Mpeg/File.cs                                   \
   $(srcdir)/TagLib/Mpeg/Header.cs                                 \
   $(srcdir)/TagLib/Id3v1/Tag.cs                                   \
   $(srcdir)/TagLib/Id3v1/GenreList.cs                             \
   $(srcdir)/TagLib/Id3v1/StringHandler.cs                         \
   $(srcdir)/TagLib/Id3v2/Frames/PrivateFrame.cs                   \
   $(srcdir)/TagLib/Id3v2/Frames/RelativeVolumeFrame.cs            \
   $(srcdir)/TagLib/Id3v2/Frames/UniqueFileIdentifierFrame.cs      \
   $(srcdir)/TagLib/Id3v2/Frames/UnknownFrame.cs                   \
   $(srcdir)/TagLib/Id3v2/Frames/CommentsFrame.cs                  \
   $(srcdir)/TagLib/Id3v2/Frames/TextIdentificationFrame.cs        \
   $(srcdir)/TagLib/Id3v2/Frames/AttachedPictureFrame.cs           \
   $(srcdir)/TagLib/Id3v2/Frames/GeneralEncapsulatedObjectFrame.cs \
   $(srcdir)/TagLib/Id3v2/FrameFactory.cs                          \
   $(srcdir)/TagLib/Id3v2/Frame.cs                                 \
   $(srcdir)/TagLib/Id3v2/Tag.cs                                   \
   $(srcdir)/TagLib/Id3v2/FrameHeader.cs                           \
   $(srcdir)/TagLib/Id3v2/ExtendedHeader.cs                        \
   $(srcdir)/TagLib/Id3v2/SynchData.cs                             \
   $(srcdir)/TagLib/Id3v2/Header.cs                                \
   $(srcdir)/TagLib/Id3v2/Footer.cs                                \
   $(srcdir)/TagLib/Mpeg4/Properties.cs                            \
   $(srcdir)/TagLib/Mpeg4/Boxes/Box.cs                             \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoMediaInformationBox.cs          \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoFreeSpaceBox.cs                 \
   $(srcdir)/TagLib/Mpeg4/Boxes/UnknownBox.cs                      \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoMediaBox.cs                     \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoUserDataBox.cs                  \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoChunkOffsetBox.cs               \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoChunkLargeOffsetBox.cs          \
   $(srcdir)/TagLib/Mpeg4/Boxes/AppleItemListBox.cs                \
   $(srcdir)/TagLib/Mpeg4/Boxes/AppleAnnotationBox.cs              \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoSampleTableBox.cs               \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoSampleEntry.cs                  \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoMetaBox.cs                      \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoMovieBox.cs                     \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoSampleDescriptionBox.cs         \
   $(srcdir)/TagLib/Mpeg4/Boxes/AppleAdditionalInfoBox.cs          \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoHandlerBox.cs                   \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoTrackBox.cs                     \
   $(srcdir)/TagLib/Mpeg4/Boxes/IsoMovieHeaderBox.cs               \
   $(srcdir)/TagLib/Mpeg4/Boxes/FullBox.cs                         \
   $(srcdir)/TagLib/Mpeg4/Boxes/FileBox.cs                         \
   $(srcdir)/TagLib/Mpeg4/Boxes/AppleElementaryStreamDescriptor.cs \
   $(srcdir)/TagLib/Mpeg4/Boxes/AppleDataBox.cs                    \
   $(srcdir)/TagLib/Mpeg4/AppleTag.cs                              \
   $(srcdir)/TagLib/Mpeg4/BoxHeader.cs                             \
   $(srcdir)/TagLib/Mpeg4/File.cs                                  \
   $(srcdir)/TagLib/WavPack/File.cs                                \
   $(srcdir)/TagLib/WavPack/Properties.cs                          \
   $(srcdir)/TagLib/IntList.cs                                     \
   $(srcdir)/TagLib/Tag.cs                                         \
   $(srcdir)/TagLib/ByteVectorList.cs                              \
   $(srcdir)/TagLib/ByteVector.cs                                  \
   $(srcdir)/TagLib/CombinedTag.cs                                 \
   $(srcdir)/TagLib/Debugger.cs                                    \
   $(srcdir)/TagLib/AudioProperties.cs                             \
   $(srcdir)/TagLib/File.cs                                        \
   $(srcdir)/TagLib/StringList.cs                                  \
   $(srcdir)/TagLib/SupportedMimeType.cs                           \
   $(srcdir)/TagLib/UnsupportedFormatException.cs                  \
   $(srcdir)/TagLib/ReadOnlyException.cs                           \
   $(srcdir)/TagLib/Picture.cs                                     \
   $(srcdir)/TagLib/ListBase.cs                                    \
   $(srcdir)/TagLib/FileTypes.cs                                   \
   $(srcdir)/TagLib/CorruptFileException.cs

taglib-sharp.snk: $(top_srcdir)/taglib-sharp.snk
	cp $(top_srcdir)/taglib-sharp.snk .

taglib_sources_in = AssemblyInfo.cs.in
taglib_generated_sources = $(taglib_sources_in:.in=)

$(ASSEMBLY): $(TAGLIB_CSFILES) $(taglib_generated_sources) taglib-sharp.snk
	$(CSC) -target:library $(LIBFLAGS) -out:$@ $(TAGLIB_CSFILES) $(taglib_generated_sources)

all: $(ASSEMBLY)

EXTRA_DIST = $(TAGLIB_CSFILES) $(taglib_sources_in) AssemblyInfo-windows.cs
DISTCLEANFILES = *.pidb
CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb taglib-sharp.snk 
MAINTAINERCLEANFILES = Makefile.in

install-data-local:
	@if test -n '$(TARGET)'; then						\
	  echo "$(GACUTIL) /i $(ASSEMBLY) /f $(GACUTIL_FLAGS)";	\
	  $(GACUTIL) /i $(ASSEMBLY) /f $(GACUTIL_FLAGS) || exit 1;	\
	fi

uninstall-local:
	@if test -n '$(TARGET)'; then						\
	  echo "$(GACUTIL) /u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS)";	\
	  $(GACUTIL) /u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS) || exit 1;	\
	fi
