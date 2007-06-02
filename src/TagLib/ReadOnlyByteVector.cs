/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
                         : (C) 2006 Novell, Inc.
    email                : brian.nickel@gmail.com
                         : Aaron Bockover <abockover@novell.com>
    based on             : tbytevector.cpp from TagLib
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

namespace TagLib
{
   public sealed class ReadOnlyByteVector : ByteVector
   {
      #region Constructors
      public ReadOnlyByteVector() : base () {}
      public ReadOnlyByteVector(int size, byte value) : base (size, value) {}
      public ReadOnlyByteVector(int size) : this(size, 0) {}
      public ReadOnlyByteVector(ByteVector vector) : base (vector) {}
      public ReadOnlyByteVector (byte [] data, int length) : base (data, length) {}
      public ReadOnlyByteVector (params byte [] data) : base (data) {}
      #endregion
      
      
        #region Operators
        public static implicit operator ReadOnlyByteVector(byte value)
        {
            return new ReadOnlyByteVector(value);
        }

        public static implicit operator ReadOnlyByteVector(byte [] value)
        {
            return new ReadOnlyByteVector(value);
        }

        public static implicit operator ReadOnlyByteVector(string value)
        {
            return new ReadOnlyByteVector (ByteVector.FromString(value));
        }
        #endregion


        #region IList<T>
        public override bool IsReadOnly {
            get { return true; }
        }
        
        public override bool IsFixedSize {
            get { return true; }
        }
        #endregion
    }
}