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
      private BlockHeader _header;
      private ByteVector  _data;
      
      public Block (BlockHeader header, ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         if (header.BlockSize != data.Count)
            throw new CorruptFileException ("Data count not equal to block size.");
         
         _header = header;
         _data   = data;
      }
      
      public Block (BlockType type, ByteVector data)
      {
         if (data == null)
            throw new ArgumentNullException ("data");
         
         _header = new BlockHeader (type, (uint) data.Count);
         
         if (_header.BlockSize != data.Count)
            throw new CorruptFileException ("Data count not equal to block size.");
         
         _data   = data;
      }
      
      public ByteVector Render (bool isLastBlock)
      {
         if (_data == null)
            throw new InvalidOperationException ("Cannot render empty blocks.");
         
         ByteVector data = _header.Render (isLastBlock);
         data.Add (_data);
         return data;
      }
      
      public   BlockType   Type        {get {return _header.BlockType;}}
      public   bool        IsLastBlock {get {return _header.IsLastBlock;}}
      public   uint        DataSize    {get {return _header.BlockSize;}}
      public   uint        TotalSize   {get {return DataSize + BlockHeader.Size;}}
      public   ByteVector  Data        {get {return _data;}}
   }
}
