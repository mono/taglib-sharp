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
   public class StreamPropertiesObject : Object
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private Guid stream_type;
      private Guid error_correction_type;
      private long time_offset;
      private short flags;
      private uint reserved;
      private ByteVector type_specific_data;
      private ByteVector error_correction_data;

      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public StreamPropertiesObject (Asf.File file, long position) : base (file, position)
      {
         if (!Guid.Equals (Asf.Guid.AsfStreamPropertiesObject))
            throw new System.Exception ("Object GUID incorrect.");
         
         if (OriginalSize < 78)
            throw new System.Exception ("Object size too small.");
         
         stream_type                       = file.ReadGuid  ();
         error_correction_type             = file.ReadGuid  ();
         time_offset                       = file.ReadQWord ();
         int type_specific_data_length     = (int) file.ReadDWord ();
         int error_correction_data_length  = (int) file.ReadDWord ();
         flags										 = file.ReadWord  ();
         reserved                          = file.ReadDWord ();
         type_specific_data                = file.ReadBlock (type_specific_data_length);
         error_correction_data             = file.ReadBlock (error_correction_data_length);
      }
      
      public override ByteVector Render ()
      {
         ByteVector output = stream_type.Render ();
         output.Add (error_correction_type.Render ());
         output.Add (RenderQWord (time_offset));
         output.Add (RenderDWord ((uint) type_specific_data.Count));
         output.Add (RenderDWord ((uint) error_correction_data.Count));
         output.Add (RenderWord  (flags));
         output.Add (RenderDWord (reserved));
         output.Add (type_specific_data);
         output.Add (error_correction_data);
         
         return Render (output);
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public Guid       StreamType {get {return stream_type;}}
      public Guid       ErrorCorrectionType {get {return error_correction_type;}}
      public TimeSpan   TimeOffset {get {return new TimeSpan (time_offset);}}
      public short      Flags {get {return flags;}}
      public ByteVector TypeSpecificData {get {return type_specific_data;}}
      public ByteVector ErrorCorrectionData {get {return error_correction_data;}}
   }
}
