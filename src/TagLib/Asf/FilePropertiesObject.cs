/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
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

using System;

namespace TagLib.Asf
{
   public class FilePropertiesObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Guid file_id;
      private long file_size;
      private long creation_date;
      private long data_packets_count;
      private long play_duration;
      private long send_duration;
      private long preroll;
      private uint flags;
      private uint minimum_data_packet_size;
      private uint maximum_data_packet_size;
      private uint maximum_bitrate;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public FilePropertiesObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfFilePropertiesObject))
            throw new CorruptFileException ("Object GUID incorrect.");
         
         if (OriginalSize < 104)
            throw new CorruptFileException ("Object size too small.");
         
         file_id                  = file.ReadGuid ();
         file_size                = file.ReadQWord ();
         creation_date            = file.ReadQWord ();
         data_packets_count       = file.ReadQWord ();
         play_duration            = file.ReadQWord ();
         send_duration            = file.ReadQWord ();
         preroll                  = file.ReadQWord ();
         flags                    = file.ReadDWord ();
         minimum_data_packet_size = file.ReadDWord ();
         maximum_data_packet_size = file.ReadDWord ();
         maximum_bitrate          = file.ReadDWord ();
      }
      
      public override ByteVector Render ()
      {
         ByteVector output = file_id.Render ();
         output.Add (RenderQWord (file_size));
         output.Add (RenderQWord (creation_date));
         output.Add (RenderQWord (data_packets_count));
         output.Add (RenderQWord (play_duration));
         output.Add (RenderQWord (send_duration));
         output.Add (RenderQWord (preroll));
         output.Add (RenderDWord (flags));
         output.Add (RenderDWord (minimum_data_packet_size));
         output.Add (RenderDWord (maximum_data_packet_size));
         output.Add (RenderDWord (maximum_bitrate));
         
         return Render (output);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public Guid     FileId {get {return file_id;}}
      public long     FileSize {get {return file_size;}}
      public DateTime CreationDate {get {return new DateTime (creation_date);}}
      public long     DataPacketsCount {get {return data_packets_count;}}
      public TimeSpan PlayDuration {get {return new TimeSpan (play_duration);}}
      public TimeSpan SendDuration {get {return new TimeSpan (send_duration);}}
      public long     Preroll {get {return preroll;}}
      public uint     Flags {get {return flags;}}
      public uint     MinimumDataPacketSize {get {return minimum_data_packet_size;}}
      public uint     MaximumDataPacketSize {get {return maximum_data_packet_size;}}
      public uint     MaximumBitrate {get {return maximum_bitrate;}}
   }
}
