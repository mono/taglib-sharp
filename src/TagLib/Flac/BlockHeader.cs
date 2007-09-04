//
// BlockHeader.cs:
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
      XiphComment,
      CueSheet,
      Picture
   }
   
   public struct BlockHeader
   {
      private BlockType _block_type;
      private bool      _is_last_block;
      private uint      _block_size;
      
      public BlockHeader (ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         if (data.Count < Size)
            throw new CorruptFileException ("Not enough data in Flac header.");
         _block_type    = (BlockType) (data[0] & 0x7f);
         _is_last_block = (data[0] & 0x80) != 0;
         _block_size    = data.Mid (1,3).ToUInt ();
      }
      
      public BlockHeader (BlockType type, uint blockSize)
      {
         _block_type    = type;
         _is_last_block = false;
         _block_size    = blockSize;
      }
      
      public ByteVector Render (bool isLastBlock)
      {
         ByteVector data = ByteVector.FromUInt (_block_size);
         data [0] = (byte)(_block_type + (isLastBlock ? 0x80 : 0));
         return data;
      }
      
      public const uint Size = 4;
      
      public BlockType BlockType   {get {return _block_type;}}
      public bool      IsLastBlock {get {return _is_last_block;}}
      public uint      BlockSize   {get {return _block_size;}}
   }
}