/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
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

namespace TagLib.Id3v2
{
   public class FrameHeader
   {
      ///////////////////////////////////////////////////////////////////////
      // private properties
      ///////////////////////////////////////////////////////////////////////
      private ByteVector frame_id;
      private uint frame_size;
      private uint version;

      // flags
      private bool tag_alter_preservation;
      private bool file_alter_preservation;
      private bool read_only;
      private bool grouping_identity;
      private bool compression;
      private bool encryption;
      private bool unsyncronisation;
      private bool data_length_indicator;
      
      
      ///////////////////////////////////////////////////////////////////////
      // public methods
      ///////////////////////////////////////////////////////////////////////
      public FrameHeader (ByteVector data, uint version)
      {
         frame_id                = null;
         frame_size              = 0;
         this.version            = 4;
         tag_alter_preservation  = false;
         file_alter_preservation = false;
         read_only               = false;
         grouping_identity       = false;
         compression             = false;
         encryption              = false;
         unsyncronisation        = false;
         data_length_indicator   = false;
         
         SetData (data, version);
      }
      
      public void SetData (ByteVector data, uint version)
      {
         this.version = version;
         
         if (version < 3)
         {
            // ID3v2.2

            if (data.Count < 3)
            {
               Debugger.Debug("You must at least specify a frame ID.");
               return;
            }

            // Set the frame ID -- the first three bytes

            frame_id = data.Mid (0, 3);

            // If the full header information was not passed in, do not continue to the
            // steps to parse the frame size and flags.

            if (data.Count < 6)
            {
               frame_size = 0;
               return;
            }

            frame_size = data.Mid (3, 3).ToUInt ();
         }
         else if (version == 3)
         {
            // ID3v2.3

            if (data.Count < 4)
            {
               Debugger.Debug("You must at least specify a frame ID.");
               return;
            }

            // Set the frame ID -- the first four bytes

            frame_id = data.Mid (0, 4);

            // If the full header information was not passed in, do not continue to the
            // steps to parse the frame size and flags.
            
            if(data.Count < 10)
            {
               frame_size = 0;
               return;
            }
            
            // Set the size -- the frame size is the four bytes starting at byte four in
            // the frame header (structure 4)
            
            frame_size = data.Mid (4, 4).ToUInt ();
            
            // read the first byte of flags
            tag_alter_preservation  = ((data[8] >> 7) & 1) == 1; // (structure 3.3.1.a)
            file_alter_preservation = ((data[8] >> 6) & 1) == 1; // (structure 3.3.1.b)
            read_only               = ((data[8] >> 5) & 1) == 1; // (structure 3.3.1.c)

            // read the second byte of flags
            compression             = ((data[9] >> 7) & 1) == 1; // (structure 3.3.1.i)
            encryption              = ((data[9] >> 6) & 1) == 1; // (structure 3.3.1.j)
            grouping_identity       = ((data[9] >> 5) & 1) == 1; // (structure 3.3.1.k)
         }
         else
         {
            // ID3v2.4

            if (data.Count < 4)
            {
               Debugger.Debug("You must at least specify a frame ID.");
               return;
            }

            // Set the frame ID -- the first four bytes

            frame_id = data.Mid (0, 4);

            // If the full header information was not passed in, do not continue to the
            // steps to parse the frame size and flags.
            
            if(data.Count < 10)
            {
               frame_size = 0;
               return;
            }

            // Set the size -- the frame size is the four bytes starting at byte four in
            // the frame header (structure 4)

            frame_size = SynchData.ToUInt (data.Mid (4, 4));

            // read the first byte of flags
            tag_alter_preservation  = ((data[8] >> 6) & 1) == 1; // (structure 4.1.1.a)
            file_alter_preservation = ((data[8] >> 5) & 1) == 1; // (structure 4.1.1.b)
            read_only               = ((data[8] >> 4) & 1) == 1; // (structure 4.1.1.c)

            // read the second byte of flags
            grouping_identity       = ((data[9] >> 6) & 1) == 1; // (structure 4.1.2.h)
            compression             = ((data[9] >> 3) & 1) == 1; // (structure 4.1.2.k)
            encryption              = ((data[9] >> 2) & 1) == 1; // (structure 4.1.2.m)
            unsyncronisation        = ((data[9] >> 1) & 1) == 1; // (structure 4.1.2.n)
            data_length_indicator   = (data[9] & 1) == 1;        // (structure 4.1.2.p)
         }
      }

      public void SetData (ByteVector data)
      {
         SetData (data, 4);
      }
      
      public ByteVector Render ()
      {
         ByteVector flags = new ByteVector (2, (byte) 0); // just blank for the moment
         
         return frame_id + SynchData.FromUInt (frame_size) + flags;
      }
      
      public static uint Size (uint version)
      {
         return (uint) (version < 3 ? 6 : 10);
      }
      
      
      ///////////////////////////////////////////////////////////////////////
      // public properties
      ///////////////////////////////////////////////////////////////////////
      public FrameHeader (ByteVector data) : this (data, 4) {}
      
      public ByteVector FrameId
      {
         get {return frame_id;}
         set {if (value != null) frame_id = value.Mid (0, 4);}
      }
      
      public uint FrameSize
      {
         get {return frame_size;}
         set {frame_size = value;}
      }
      
      public uint Version               {get {return version;}}
      
      public bool TagAlterPreservation
      {
         get {return tag_alter_preservation;}
         set {tag_alter_preservation = value;}
      }
      
      public bool FileAlterPreservation
      {
         get {return file_alter_preservation;}
         set {file_alter_preservation = value;}
      }
      
      public bool ReadOnly
      {
         get {return read_only;}
         set {read_only = value;}
      }
      
      public bool GroupingIdentity    {get {return grouping_identity;}}
      public bool Compression         {get {return compression;}}
      public bool Encryption          {get {return encryption;}}
      public bool Unsycronisation     {get {return unsyncronisation;}}
      public bool DataLengthIndicator {get {return data_length_indicator;}}
   }
}
