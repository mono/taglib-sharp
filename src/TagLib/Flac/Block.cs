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
   public class Block
   {
      private BlockHeader header;
      private ByteVector  data;
      private long        position;
      
      public Block (BlockHeader header, ByteVector data, long position)
      {
         this.header   = header;
         this.data     = data;
         this.position = position;
      }
      
      public Block (BlockType type, ByteVector data)
      {
         this.header   = new BlockHeader (type, (uint) data.Count);
         this.data     = data;
         this.position = 0;
      }
      
      public ByteVector Render (bool is_last_block)
      {
         ByteVector data = header.Render (is_last_block);
         data.Add (this.data);
         return data;
      }
      
      public BlockType   Type              {get {return header.BlockType;}}
      public bool        IsLastBlock       {get {return header.IsLastBlock;}}
      public uint        Length            {get {return header.BlockLength;}}
      public uint        TotalLength       {get {return Length + 4;}}
      public ByteVector  Data              {get {return data;}}
      public long        Position          {get {return position;}}
      public long        NextBlockPosition {get {return Position + BlockHeader.Length + header.BlockLength;}}
   }
}
