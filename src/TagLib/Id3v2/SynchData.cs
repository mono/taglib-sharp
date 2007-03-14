/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2synchdata.cpp from TagLib
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
   public class SynchData
   {
      public static uint ToUInt (ByteVector data)
      {
         uint sum = 0;
         int last = data.Count > 4 ? 3 : data.Count - 1;

         for(int i = 0; i <= last; i++)
            sum |= (uint) (data [i] & 0x7f) << ((last - i) * 7);

         return sum;
      }

      public static ByteVector FromUInt (uint value)
      {
         ByteVector v = new ByteVector (4, 0);

         for (int i = 0; i < 4; i++)
            v [i] = (byte) (value >> ((3 - i) * 7) & 0x7f);

         return v;
      }
      
      public static ByteVector UnsynchByteVector (ByteVector data)
      {
         for (int i = data.Count - 2; i >= 0; i --)
            if (data [i] == 0xFF && (data [i+1] == 0 || (data [i+1] & 0xE0) != 0))
               data.Insert (i+1, 0);
         return data;
      }
      
      public static ByteVector ResynchByteVector (ByteVector data)
      {
         for (int i = data.Count - 2; i >= 0; i --)
            if (data [i] == 0xFF && data [i+1] == 0)
               data.RemoveAt (i+1);
         return data;
      }
   }
}
