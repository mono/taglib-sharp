/***************************************************************************
    copyright            : (C) 2007 by Brian Nickel
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

using System.Collections.Generic;
using System;

namespace TagLib.Flac
{
   public enum BlockType
   {
      StreamInfo = 0,
      Padding,
      Application,
      SeekTable,
      VorbisComment,
      CueSheet,
      Picture
   }
   
   public class BlockHeader
   {
      private BlockType block_type;
      private bool      is_last_block;
      private uint      block_length;
      
      public BlockHeader (ByteVector data)
      {
         block_type    = (BlockType) (data[0] & 0x7f);
         is_last_block = (data[0] & 0x80) != 0;
         block_length  = data.Mid (1,3).ToUInt ();
      }
      
      public BlockHeader (BlockType type, uint length)
      {
         block_type    = type;
         is_last_block = false;
         block_length  = length;
      }
      
      public ByteVector Render (bool is_last_block)
      {
         ByteVector data = ByteVector.FromUInt (block_length);
         data [0] = (byte)(block_type + (is_last_block ? 0x80 : 0));
         return data;
      }
      
      public static uint Length    {get {return 4;}}
      
      public BlockType BlockType   {get {return block_type;}}
      public bool      IsLastBlock {get {return is_last_block;}}
      public uint      BlockLength {get {return block_length;}}
   }
}