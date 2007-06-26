/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : xingheader.cpp from TagLib
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

namespace TagLib.Mpeg
{
   public struct XingHeader
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private uint frames;
      private uint size;
      private bool present;
      
      public static readonly XingHeader Unknown = new XingHeader (0, 0);
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      private XingHeader (uint frame, uint size)
      {
         this.frames = frame;
         this.size = size;
         this.present = false;
      }
      
      public XingHeader (ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         // Check to see if a valid Xing header is available.
         if (!data.StartsWith ("Xing"))
            throw new CorruptFileException ("Not a valid Xing header");
         
         int position = 8;
         
         if ((data [7] & 0x01) != 0) {
            frames = data.Mid (position, 4).ToUInt ();
            position += 4;
         } else
            frames = 0;
         
         if ((data [7] & 0x02) != 0) {
            size = data.Mid (position, 4).ToUInt ();
            position += 4;
         } else
            size = 0;

         present = true;
      }

      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public uint TotalFrames {get {return frames;}}
      public uint TotalSize {get {return size;}}
      public bool Present   {get {return present;}}

      //////////////////////////////////////////////////////////////////////////
      // public static methods
      //////////////////////////////////////////////////////////////////////////
      public static int XingHeaderOffset (Version version, ChannelMode channelMode)
      {
         if (version == Version.Version1)
         {
            if (channelMode == ChannelMode.SingleChannel)
               return 0x15;
            else
               return 0x24;
         }
         else
         {
            if (channelMode == ChannelMode.SingleChannel)
               return 0x0D;
            else
               return 0x15;
         }
      }
   }
}
