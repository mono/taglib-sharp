//
// File.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace TagLib.Riff
{
   [SupportedMimeType("taglib/avi", "avi")]
   [SupportedMimeType("taglib/wav", "wav")]
   [SupportedMimeType("taglib/divx", "divx")]
   [SupportedMimeType("video/avi")]
   [SupportedMimeType("video/msvideo")]
   [SupportedMimeType("video/x-msvideo")]
   [SupportedMimeType("image/avi")]
   [SupportedMimeType("application/x-troff-msvideo")]
   [SupportedMimeType("audio/avi")]
   [SupportedMimeType("audio/wav")]
   [SupportedMimeType("audio/wave")]
   [SupportedMimeType("audio/x-wav")]
   public class File : TagLib.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private CombinedTag tag        = new CombinedTag ();
      private InfoTag     info_tag   = null;
      private MovieIdTag  mid_tag    = null;
      private DivXTag     divx_tag   = null;
      private Id3v2.Tag   id32_tag   = null;
      private Properties  properties = null;
      
      public static readonly ReadOnlyByteVector FileIdentifier = "RIFF";
      
      public File (string path, ReadStyle propertiesStyle) : this (new File.LocalFileAbstraction (path), propertiesStyle)
      {}
      
      public File (string path) : this (path, ReadStyle.Average)
      {}
      
		public File (File.IFileAbstraction abstraction,
		             ReadStyle propertiesStyle) : base (abstraction)
		{
			uint riff_size;
			long tag_start, tag_end;

			Mode = AccessMode.Read;
			try {
				Read (true, propertiesStyle, out riff_size,
					out tag_start, out tag_end);
			} finally {
				Mode = AccessMode.Closed;
			}

			TagTypesOnDisk = TagTypes;

			GetTag (TagTypes.Id3v2, true);
			GetTag (TagTypes.RiffInfo, true);
			GetTag (TagTypes.MovieId, true);
			GetTag (TagTypes.DivX, true);
		}
      
      public File (File.IFileAbstraction abstraction) : this (abstraction, ReadStyle.Average)
      {}
      
      private void Read (bool read_tags, ReadStyle style, out uint riff_size, out long tag_start, out long tag_end)
      {
         Seek (0);
         if (ReadBlock (4) != FileIdentifier)
            throw new CorruptFileException ("File does not begin with RIFF identifier");
         
         riff_size = ReadBlock (4).ToUInt (false);
         ByteVector stream_format = ReadBlock (4);
         tag_start = -1;
         tag_end   = -1;
         
         long position = 12;
         long length = Length;
         
         TimeSpan duration = TimeSpan.Zero;
         ICodec[] codecs = new ICodec [0];
         
         do
         {
            bool tag_found = false;
            
            Seek (position);
            string fourcc = ReadBlock (4).ToString (StringType.UTF8);
            uint   size = ReadBlock (4).ToUInt (false);
            switch (fourcc)
            {
            case "fmt ":
               if (stream_format == "WAVE" && style != ReadStyle.None)
               {
                  Seek (position + 8);
                  codecs = new ICodec [] {new WaveFormatEx (ReadBlock (18))};
               }
               break;
            case "data":
               if (stream_format == "WAVE") {
                  if (style != ReadStyle.None && codecs.Length == 1 && codecs [0] is WaveFormatEx)
                     duration += TimeSpan.FromSeconds ((double) size / (double) ((WaveFormatEx) codecs [0]).AverageBytesPerSecond);
                  InvariantStartPosition = position;
                  InvariantEndPosition = position + size;
               }
               break;
            case "LIST":
            {
					switch (ReadBlock (4).ToString (StringType.UTF8))
					{
					case "hdrl":
						if (stream_format == "AVI " && style != ReadStyle.None)
                  {
                     AviHeaderList header_list = new AviHeaderList (this, position + 12, (int) (size - 4));
                     duration = header_list.Header.Duration;
                     codecs = header_list.Codecs;
                  }
                  break;
               case "INFO":
               {
                  if (read_tags && info_tag == null)
                     info_tag = new InfoTag (this, position + 12, (int) (size - 4));
                  tag_found = true;
                  break;
               }
               case "MID ":
                  if (read_tags && mid_tag == null)
                     mid_tag = new MovieIdTag (this, position + 12, (int) (size - 4));
                  tag_found = true;
                  break;
               case "movi":
                  if (stream_format == "AVI ") {
                     InvariantStartPosition = position;
                     InvariantEndPosition = position + size;
                  }
                  break;
               }
               break;
            }
            case "ID32":
               if (read_tags && id32_tag == null)
                  id32_tag = new Id3v2.Tag (this, position + 8);
               tag_found = true;
               break;
            case "IDVX":
               if (read_tags && divx_tag == null)
                  divx_tag = new DivXTag (this, position + 8);
               tag_found = true;
               break;
            case "JUNK":
               if (tag_end == position)
                  tag_end = position + 8 + size;
               break;
            }
            
            if (tag_found)
            {
               if (tag_start == -1)
               {
                  tag_start = position;
                  tag_end = position + 8 + size;
               }
               else if (tag_end == position)
                  tag_end = position + 8 + size;
            }
            
            position += 8 + size;
         }
         while (position + 8 < length);
         
         if (style != ReadStyle.None)
         {
            if (codecs.Length == 0)
               throw new UnsupportedFormatException ("Unsupported RIFF type.");
            
            properties = new Properties (duration, codecs);
         }
         
         if (read_tags)
            tag.SetTags (id32_tag, info_tag, mid_tag, divx_tag);
      }
      
      public override Tag Tag {get {return tag;}}
      
      public override TagLib.Properties Properties {get {return properties;}}
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         TagLib.Tag tag = null;
         
         switch (type)
         {
         case TagTypes.Id3v2:
            if (id32_tag == null && create)
            {
               id32_tag = new Id3v2.Tag ();
               id32_tag.Version = 4;
               id32_tag.Flags |= Id3v2.HeaderFlags.FooterPresent;
               this.tag.CopyTo (id32_tag, true);
            }
            tag = id32_tag;
            break;
         case TagTypes.RiffInfo:
            if (info_tag == null && create)
            {
               info_tag = new InfoTag ();
               this.tag.CopyTo (info_tag, true);
            }
            tag = info_tag;
            break;
         case TagTypes.MovieId:
            if (mid_tag == null && create)
            {
               mid_tag = new MovieIdTag ();
               this.tag.CopyTo (mid_tag, true);
            }
            tag = mid_tag;
            break;
         case TagTypes.DivX:
            if (divx_tag == null && create)
            {
               divx_tag = new DivXTag ();
               this.tag.CopyTo (divx_tag, true);
            }
            tag = divx_tag;
            break;
         }
         
         this.tag.SetTags (id32_tag, info_tag, mid_tag, divx_tag);
         return tag;
      }
      
      public override void RemoveTags (TagTypes types)
      {
         if ((types & TagLib.TagTypes.Id3v2) != TagLib.TagTypes.None)
            id32_tag = null;
         if ((types & TagLib.TagTypes.RiffInfo) != TagLib.TagTypes.None)
            info_tag = null;
         if ((types & TagLib.TagTypes.MovieId) != TagLib.TagTypes.None)
            mid_tag  = null;
         if ((types & TagLib.TagTypes.DivX) != TagLib.TagTypes.None)
            divx_tag = null;
         
         tag.SetTags (id32_tag, info_tag, mid_tag, divx_tag);
      }
      
		public override void Save ()
		{
			Mode = AccessMode.Write;
			try {
				ByteVector data = new ByteVector ();

				if (id32_tag != null) {
					ByteVector tag_data = id32_tag.Render ();
					if (tag_data.Count > 10) {
						if (tag_data.Count % 2 == 1)
							tag_data.Add (0);
						data.Add ("ID32");
						data.Add (ByteVector.FromUInt (
							(uint) tag_data.Count,
							false));
						data.Add (tag_data);
					}
				}

				if (info_tag != null)
					data.Add (info_tag.RenderEnclosed ());

				if (mid_tag != null)
					data.Add (mid_tag.RenderEnclosed ());

				if (divx_tag != null && !divx_tag.IsEmpty) {
					ByteVector tag_data = divx_tag.Render ();
					data.Add ("IDVX");
					data.Add (ByteVector.FromUInt (
						(uint) tag_data.Count, false));
					data.Add (tag_data);
				}

				uint riff_size;
				long tag_start, tag_end;
				Read (false, ReadStyle.None, out riff_size,
					out tag_start, out tag_end);

				if (tag_start < 12 || tag_end < tag_start)
					tag_start = tag_end = Length;

				int length = (int)(tag_end - tag_start);
				int padding_size = length - data.Count - 8;
				if (padding_size < 0)
					padding_size = 1024;

				data.Add ("JUNK");
				data.Add (ByteVector.FromUInt (
					(uint)padding_size, false));
				data.Add (new ByteVector (padding_size));

				Insert (data, tag_start, length);

				if (data.Count - length != 0 &&
					tag_start <= riff_size)
					Insert (ByteVector.FromUInt ((uint)
						(riff_size + data.Count - length),
						false), 4, 4);

				TagTypesOnDisk = TagTypes;
			} finally {
				Mode = AccessMode.Closed;
			}
		}
	}
}
