/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : oggfile.cpp from TagLib
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Collections;
using System;

namespace TagLib.Ogg.Vorbis
{
   [SupportedMimeType("taglib/ogg")]
   [SupportedMimeType("application/ogg")]
   [SupportedMimeType("application/x-ogg")]
   [SupportedMimeType("audio/vorbis")]
   [SupportedMimeType("audio/x-vorbis")]
   [SupportedMimeType("audio/x-vorbis+ogg")]
   [SupportedMimeType("audio/ogg")]
   [SupportedMimeType("audio/x-ogg")]
   public class File : Ogg.File
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Ogg.XiphComment comment;
      private Properties      properties;
      
      private static byte [] vorbis_comment_header_id = {0x03, (byte)'v', (byte)'o', (byte)'r', (byte)'b', (byte)'i', (byte)'s'};
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public File (string file, Properties.ReadStyle properties_style) : base (file)
      {
         comment = null;
         properties = null;
         
         try {Mode = AccessMode.Read;}
         catch {return;}
         
         Read (properties_style);
         
         Mode = AccessMode.Closed;
      }
      
      public File (string file) : this (file, Properties.ReadStyle.Average)
      {
      }
      
      public override void Save ()
      {
         ClearPageData (); // Force re-reading of the file.

         ByteVector v = vorbis_comment_header_id;

         GetTag (TagTypes.Xiph, true);
            
         v.Add (comment.Render ());

         SetPacket (1, v);

         base.Save ();
      }
      
      public override TagLib.Tag GetTag (TagTypes type, bool create)
      {
         if (type == TagTypes.Xiph)
         {
            if (comment == null && create)
               comment = new Ogg.XiphComment ();
            
            return comment;
         }
         
         return null;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public override Tag Tag {get {return GetTag (TagTypes.Xiph, true);}}
      
      public override AudioProperties AudioProperties {get {return properties;}}      
      
      //////////////////////////////////////////////////////////////////////////
      // private methods
      //////////////////////////////////////////////////////////////////////////
      private void Read (Properties.ReadStyle properties_style)
      {
         ByteVector comment_header_data = GetPacket (1);

         if (comment_header_data.Mid (0, 7) != vorbis_comment_header_id)
         {
            Debugger.Debug ("Vorbis.File.Read() - Could not find the Vorbis comment header.");
            SetValid (false);
            return;
         }

         comment = new Ogg.XiphComment (comment_header_data.Mid (7));

         if(properties_style != Properties.ReadStyle.None)
            properties = new Properties (this, properties_style);
      }
   }
}
