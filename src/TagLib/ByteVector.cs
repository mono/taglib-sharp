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

using System.Collections;
using System.Collections.Generic;
using System;

namespace TagLib
{
    public enum StringType
    {
        Latin1  = 0,
        UTF16   = 1,
        UTF16BE = 2,
        UTF8    = 3,
        UTF16LE = 4
    }

    public class ByteVector : IList<byte>, IComparable<ByteVector>
    {
        private List<byte> data = new List<byte>();

        private static uint [] crc_table = new uint[256] {
            0x00000000, 0x04c11db7, 0x09823b6e, 0x0d4326d9, 0x130476dc, 0x17c56b6b,
            0x1a864db2, 0x1e475005, 0x2608edb8, 0x22c9f00f, 0x2f8ad6d6, 0x2b4bcb61,
            0x350c9b64, 0x31cd86d3, 0x3c8ea00a, 0x384fbdbd, 0x4c11db70, 0x48d0c6c7,
            0x4593e01e, 0x4152fda9, 0x5f15adac, 0x5bd4b01b, 0x569796c2, 0x52568b75,
            0x6a1936c8, 0x6ed82b7f, 0x639b0da6, 0x675a1011, 0x791d4014, 0x7ddc5da3,
            0x709f7b7a, 0x745e66cd, 0x9823b6e0, 0x9ce2ab57, 0x91a18d8e, 0x95609039,
            0x8b27c03c, 0x8fe6dd8b, 0x82a5fb52, 0x8664e6e5, 0xbe2b5b58, 0xbaea46ef,
            0xb7a96036, 0xb3687d81, 0xad2f2d84, 0xa9ee3033, 0xa4ad16ea, 0xa06c0b5d,
            0xd4326d90, 0xd0f37027, 0xddb056fe, 0xd9714b49, 0xc7361b4c, 0xc3f706fb,
            0xceb42022, 0xca753d95, 0xf23a8028, 0xf6fb9d9f, 0xfbb8bb46, 0xff79a6f1,
            0xe13ef6f4, 0xe5ffeb43, 0xe8bccd9a, 0xec7dd02d, 0x34867077, 0x30476dc0,
            0x3d044b19, 0x39c556ae, 0x278206ab, 0x23431b1c, 0x2e003dc5, 0x2ac12072,
            0x128e9dcf, 0x164f8078, 0x1b0ca6a1, 0x1fcdbb16, 0x018aeb13, 0x054bf6a4,
            0x0808d07d, 0x0cc9cdca, 0x7897ab07, 0x7c56b6b0, 0x71159069, 0x75d48dde,
            0x6b93dddb, 0x6f52c06c, 0x6211e6b5, 0x66d0fb02, 0x5e9f46bf, 0x5a5e5b08,
            0x571d7dd1, 0x53dc6066, 0x4d9b3063, 0x495a2dd4, 0x44190b0d, 0x40d816ba,
            0xaca5c697, 0xa864db20, 0xa527fdf9, 0xa1e6e04e, 0xbfa1b04b, 0xbb60adfc,
            0xb6238b25, 0xb2e29692, 0x8aad2b2f, 0x8e6c3698, 0x832f1041, 0x87ee0df6,
            0x99a95df3, 0x9d684044, 0x902b669d, 0x94ea7b2a, 0xe0b41de7, 0xe4750050,
            0xe9362689, 0xedf73b3e, 0xf3b06b3b, 0xf771768c, 0xfa325055, 0xfef34de2,
            0xc6bcf05f, 0xc27dede8, 0xcf3ecb31, 0xcbffd686, 0xd5b88683, 0xd1799b34,
            0xdc3abded, 0xd8fba05a, 0x690ce0ee, 0x6dcdfd59, 0x608edb80, 0x644fc637,
            0x7a089632, 0x7ec98b85, 0x738aad5c, 0x774bb0eb, 0x4f040d56, 0x4bc510e1,
            0x46863638, 0x42472b8f, 0x5c007b8a, 0x58c1663d, 0x558240e4, 0x51435d53,
            0x251d3b9e, 0x21dc2629, 0x2c9f00f0, 0x285e1d47, 0x36194d42, 0x32d850f5,
            0x3f9b762c, 0x3b5a6b9b, 0x0315d626, 0x07d4cb91, 0x0a97ed48, 0x0e56f0ff,
            0x1011a0fa, 0x14d0bd4d, 0x19939b94, 0x1d528623, 0xf12f560e, 0xf5ee4bb9,
            0xf8ad6d60, 0xfc6c70d7, 0xe22b20d2, 0xe6ea3d65, 0xeba91bbc, 0xef68060b,
            0xd727bbb6, 0xd3e6a601, 0xdea580d8, 0xda649d6f, 0xc423cd6a, 0xc0e2d0dd,
            0xcda1f604, 0xc960ebb3, 0xbd3e8d7e, 0xb9ff90c9, 0xb4bcb610, 0xb07daba7,
            0xae3afba2, 0xaafbe615, 0xa7b8c0cc, 0xa379dd7b, 0x9b3660c6, 0x9ff77d71,
            0x92b45ba8, 0x9675461f, 0x8832161a, 0x8cf30bad, 0x81b02d74, 0x857130c3,
            0x5d8a9099, 0x594b8d2e, 0x5408abf7, 0x50c9b640, 0x4e8ee645, 0x4a4ffbf2,
            0x470cdd2b, 0x43cdc09c, 0x7b827d21, 0x7f436096, 0x7200464f, 0x76c15bf8,
            0x68860bfd, 0x6c47164a, 0x61043093, 0x65c52d24, 0x119b4be9, 0x155a565e,
            0x18197087, 0x1cd86d30, 0x029f3d35, 0x065e2082, 0x0b1d065b, 0x0fdc1bec,
            0x3793a651, 0x3352bbe6, 0x3e119d3f, 0x3ad08088, 0x2497d08d, 0x2056cd3a,
            0x2d15ebe3, 0x29d4f654, 0xc5a92679, 0xc1683bce, 0xcc2b1d17, 0xc8ea00a0,
            0xd6ad50a5, 0xd26c4d12, 0xdf2f6bcb, 0xdbee767c, 0xe3a1cbc1, 0xe760d676,
            0xea23f0af, 0xeee2ed18, 0xf0a5bd1d, 0xf464a0aa, 0xf9278673, 0xfde69bc4,
            0x89b8fd09, 0x8d79e0be, 0x803ac667, 0x84fbdbd0, 0x9abc8bd5, 0x9e7d9662,
            0x933eb0bb, 0x97ffad0c, 0xafb010b1, 0xab710d06, 0xa6322bdf, 0xa2f33668,
            0xbcb4666d, 0xb8757bda, 0xb5365d03, 0xb1f740b4
        };

        #region Constructors

        public ByteVector() 
        {
        }

        public ByteVector(int size, byte value)
        {
            if(size > 0) {
                byte [] data = new byte[size];

                for(int i = 0; i < size; i ++) {
                    data[i] = value;
                }

                SetData(data);
            }
        }

        public ByteVector(int size) : this(size, 0)
        {
        }

        public ByteVector(ByteVector vector)
        {
            Add(vector);
        }
      
        public ByteVector(byte value) : this(1, value) 
        {
        }

        public ByteVector (byte [] data, int length)
        {
            SetData(data, length);
        }

        public ByteVector(byte [] data)
        {
            SetData(data);
        }
        
        #endregion
        
        #region Properties

        public byte [] Data {
            get { return data.ToArray(); }
        }

        public bool IsEmpty {
            get { return Count == 0; }
        }

        public uint CheckSum {
            get {
                uint sum = 0;
                foreach(byte b in this) {
                    sum = (sum << 8) ^ crc_table[((sum >> 24) & 0xFF) ^ b];
                }
                return sum;
            }
        }
        
        #endregion
        
        #region Methods

        public void SetData(byte [] value, int length)
        {
            if(length >= value.Length) {
                SetData(value);
            } else {
                byte [] array = new byte[length];
                for(int i = 0; i < length; i++) {
                    array[i] = value[i];
                }
                SetData(array);
            }
        }

        public void SetData(byte [] value)
        {
            Clear();
            Add(value);
        }

        public ByteVector Mid(int index, int length)
        {
            if (length <= 0)
                return new ByteVector ();
            
            if(length == Int32.MaxValue || index + length > Count) {
                length = Count - index;
            }
            
            byte [] data = new byte [length];
           
            this.data.CopyTo (index, data, 0, length);
            return data;
        }

        public ByteVector Mid(int index)
        {
            return Mid(index, Int32.MaxValue);
        }
        
        public int Find (ByteVector pattern, int offset, int byte_align)
        {
            if (pattern.Count > Count || offset >= Count - 1)
                return -1;

            // Let's go ahead and special case a pattern of size one since that's common
            // and easy to make fast.

            if (pattern.Count == 1)
            {
                byte p = pattern [0];
                for (int i = offset; i < Count; i += byte_align)
                    if (this [i] == p)
                        return i;
                return -1;
            }

            int [] last_occurrence = new int [256];

            for (int i = 0; i < 256; ++i)
                last_occurrence [i] = pattern.Count;

            for (int i = 0; i < pattern.Count - 1; ++i)
                last_occurrence [pattern [i]] = pattern.Count - i - 1;

            for (int i = pattern.Count - 1 + offset; i < Count; i += last_occurrence [this [i]])
            {
                int iBuffer = i;
                int iPattern = pattern.Count - 1;
                
                while(iPattern >= 0 && this [iBuffer] == pattern [iPattern])
                {
                    --iBuffer;
                    --iPattern;
                }

                if(-1 == iPattern && (iBuffer + 1) % byte_align == 0)
                    return iBuffer + 1;
            }

            return -1;
        }

        public int Find(ByteVector pattern, int offset)
        {
            return Find(pattern, offset, 1);
        }

        public int Find(ByteVector pattern)
        {
            return Find(pattern, 0, 1);
        }
      
        public int RFind (ByteVector pattern, int offset, int byte_align)
        {
            if (pattern.Count == 0 || pattern.Count > Count || Count - pattern.Count - offset < 0)
                return -1;

            // Let's go ahead and special case a pattern of size one since that's common
            // and easy to make fast.

            if (pattern.Count == 1)
            {
                byte p = pattern [0];
                for (int i = Count - offset - 1; i >= 0; i -= byte_align)
                    if (this [i] == p)
                        return i;
                return -1;
            }

            int [] first_occurrence = new int [256];

            for (int i = 0; i < 256; ++i)
                first_occurrence [i] = pattern.Count;

            for (int i = pattern.Count - 1; i > 0; --i)
                first_occurrence [pattern [i]] = i;
            
            for (int i = Count - offset - pattern.Count; i >= 0; i -= first_occurrence [this [i]])
                if (ContainsAt (pattern, i))
                    return i;

            return -1;
        }
        
        public int RFind(ByteVector pattern, int offset)
        {
            return RFind(pattern, offset, 1);
        }

        public int RFind(ByteVector pattern)
        {
            return RFind(pattern, 0, 1);
        }
      
        public bool ContainsAt(ByteVector pattern, int offset, 
            int patternOffset, int patternLength)
        {
            if(pattern.Count < patternLength) {
                patternLength = pattern.Count;
            }

            // do some sanity checking -- all of these things are 
            // needed for the search to be valid
            if(patternLength > Count || offset >= Count || 
                patternOffset >= pattern.Count || patternLength == 0) {
                return false;
            }
            
            // loop through looking for a mismatch
            for(int i = 0; i < patternLength - patternOffset; i++) {
                if(this[i + offset] != pattern[i + patternOffset]) {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsAt(ByteVector pattern, int offset, int pattern_offset)
        {
            return ContainsAt(pattern, offset, pattern_offset, Int32.MaxValue);
        }

        public bool ContainsAt(ByteVector pattern, int offset)
        {
            return ContainsAt(pattern, offset, 0);
        }
      
        public bool StartsWith(ByteVector pattern)
        {
            return ContainsAt(pattern, 0);
        }
      
        public bool EndsWith(ByteVector pattern)
        {
            return ContainsAt(pattern, Count - pattern.Count);
        }

        public int EndsWithPartialMatch(ByteVector pattern)
        {
            if(pattern.Count > Count) {
                return -1;
            }

            int start_index = Count - pattern.Count;

            // try to match the last n-1 bytes from the vector (where n is 
            // the pattern size) -- continue trying to match n-2, n-3...1 bytes

            for(int i = 1; i < pattern.Count; i++) {
                if(ContainsAt(pattern, start_index + i, 0, pattern.Count - i)) {
                    return start_index + i;
                }
            }

            return -1;
        }

        public void Add(ByteVector vector)
        {
            if(vector != null) {
                data.AddRange(vector);
            }
        }

        public void Add(byte [] vector)
        {
            if(vector != null) {
                data.AddRange(vector);
            }
        }
        
        public void Insert (int index, ByteVector vector)
        {
            if(vector != null) {
                data.InsertRange (index, vector);
            }
        }
        
        public void Insert (int index, byte [] vector)
        {
            if(vector != null) {
                data.InsertRange (index, vector);
            }
        }
        
        public ByteVector Resize(int size, byte padding)
        {
            if(Count > size) {
                data.RemoveRange(size, Count - size);
            }
            
            while(Count < size) {
                Add(0);
            }
            
            return this;
        }

        public ByteVector Resize(int size)
        {
            return Resize(size, 0);
        }
        
        #endregion
        
        #region Conversions

        public uint ToUInt(bool msbFirst)
        {
            uint sum = 0;
            for(int i = 0, last = Count > 4 ? 3 : Count - 1; i <= last; i++) {
                sum |= (uint)this[i] << ((msbFirst ? last - i : i) * 8);
            }
            return sum;
        }

        public uint ToUInt()
        {
            return ToUInt(true);
        }

        public short ToShort(bool msbFirst)
        {
            short sum = 0;
            for(int i = 0, last = Count > 2 ? 1 : Count - 1; i <= last; i++) {
                sum |= (short)(this[i] << ((msbFirst ? last - i : i) * 8));
            }
            return sum;
        }

        public short ToShort()
        {
            return ToShort(true);
        }

        public long ToLong(bool msbFirst)
        {
            long sum = 0;
            for(int i = 0, last = Count > 8 ? 7 : Count - 1; i <= last; i++) {
                sum |= (long)this [i] << ((msbFirst ? last - i : i) * 8);
            }
            return sum;
        }

        public long ToLong()
        {
            return ToLong(true);
        }

        public string ToString(StringType type, int offset)
        {
            ByteVector bom = type == StringType.UTF16 ? Mid(offset, 2) : null;
            string s = StringTypeToEncoding(type, bom).GetString(Data, offset, Count - offset);
            
            if(s.Length != 0 && (s[0] == 0xfffe || s[0] == 0xfeff)) { // UTF16 BOM
                return s.Substring (1);
            }
            
            return s;
        }

        public string ToString(StringType type)
        {
            return ToString (type, 0);
        }

        public override string ToString()
        {
            return ToString(StringType.UTF8);
        }
        
        public string[] ToStrings (StringType type, int offset)
        {
            return ToStrings (type, offset, 0);
        }

        public string[] ToStrings (StringType type, int offset, int count)
        {
            string[] split = count <= 0 ? ToString (type, offset).Split ('\0') :
                ToString (type, offset).Split (new char[] {'\0'}, count);
            
            for (int i = 0; i < split.Length; i++)
            {
                string s = split [i];
                if (s.Length != 0 && (s[0] == 0xfffe || s[0] == 0xfeff))
                { // UTF16 BOM
                    split[i] = s.Substring (1);
                }
            }
            return split;
        }
        #endregion
        
        #region Operators
      
        public static bool operator==(ByteVector a, ByteVector b)
        {
            if((object) a == null && (object) b == null) {
                return true;
            } else if((object) a == null || (object) b == null) {
                return false;
            }
            
            return a.Count == b.Count && a.StartsWith(b);
        }
              
        public static bool operator!=(ByteVector a, ByteVector b)
        {
            return !(a == b);
        }

        public static bool operator<(ByteVector a, ByteVector b)
        {
            for(int i = 0; i < a.Count && i < b.Count; i ++) {
                if(a[i] < b[i]) {
                    return true;
                }
            }
            
            return a.Count < b.Count;
        }

        public static bool operator<=(ByteVector a, ByteVector b)
        {
            return a < b || a == b;
        }

        public static bool operator>(ByteVector a, ByteVector b)
        {
            for(int i = 0; i < a.Count && i < b.Count; i ++) {
                if(a[i] > b[i]) {
                    return true;
                }
            }

            return a.Count > b.Count;
        }

        public static bool operator>=(ByteVector a, ByteVector b)
        {
            return a > b || a == b;
        }

        public static ByteVector operator+(ByteVector a, ByteVector b)
        {
            ByteVector sum = new ByteVector(a);
            sum.Add(b);
            return sum;
        }

        public static implicit operator ByteVector(byte c)
        {
            return new ByteVector(c);
        }

        public static implicit operator ByteVector(byte [] b)
        {
            return new ByteVector(b);
        }

        public static implicit operator ByteVector(string s)
        {
            return ByteVector.FromString(s);
        }
        
        #endregion

        #region Static Conversions

        public static ByteVector FromUInt(uint value, bool msbFirst)
        {
            ByteVector vector = new ByteVector();
            for(int i = 0; i < 4; i++) {
                vector.Add((byte)(value >> ((msbFirst ? 3 - i : i) * 8) & 0xFF));
            }
            return vector;
        }

        public static ByteVector FromUInt(uint value)
        {
            return FromUInt(value, true);
        }

        public static ByteVector FromShort(short value, bool msbFirst)
        {
            ByteVector vector = new ByteVector();
            for(int i = 0; i < 2; i++) {
                vector.Add((byte)(value >> ((msbFirst ? 1 - i : i) * 8) & 0xFF));
            }
            return vector;
        }

        public static ByteVector FromShort(short value)
        {
            return FromShort(value, true);
        }

        public static ByteVector FromLong(long value, bool msbFirst)
        {
            ByteVector vector = new ByteVector();
            for(int i = 0; i < 8; i++) {
                vector.Add((byte)(value >> ((msbFirst ? 7 - i : i) * 8) & 0xFF));
            }
            return vector;
        }

        public static ByteVector FromLong(long value)
        {
            return FromLong(value, true);
        }

        public static ByteVector FromString(string s, StringType type, int length)
        {
            ByteVector data = new ByteVector ();
            
            if (type == StringType.UTF16)
                data.Add (new byte [] {0xff, 0xfe});
            
            if (s == null || s.Length == 0)
                return data;
            
            if (s.Length > length)
                s = s.Substring (0, length);
            
            data.Add (StringTypeToEncoding (type, null).GetBytes (s));
            
            return data;
        }

        public static ByteVector FromString(string s, StringType type)
        {
            return FromString(s, type, Int32.MaxValue);
        }

        public static ByteVector FromString(string s, int length)
        {
            return FromString(s, StringType.UTF8, length);
        }

        public static ByteVector FromString(string s)
        {
            return FromString (s, StringType.UTF8);
        }
      
        public static ByteVector FromUri(string uri)
        {
            byte [] tmp_out;
            return FromUri(uri, out tmp_out, false);
        }

        internal static ByteVector FromUri(string uri, out byte [] firstChunk, 
            bool copyFirstChunk)
        {
            File.FileAbstractionCreator creator = File.GetFileAbstractionCreator();
            File.IFileAbstraction abstraction = creator(uri);

            using(System.IO.Stream stream = abstraction.ReadStream) {
                return FromStream(stream, out firstChunk, copyFirstChunk);
            }
        }

        public static ByteVector FromStream(System.IO.Stream stream)
        {
            byte [] tmp_out;
            return FromStream(stream, out tmp_out, false);
        }

        internal static ByteVector FromStream(System.IO.Stream stream, 
            out byte [] firstChunk, bool copyFirstChunk)
        {
            ByteVector vector = new ByteVector();
            byte [] bytes = new byte[4096];
            int read_size = bytes.Length;
            int bytes_read = 0;
            bool set_first_chunk = false;

            firstChunk = null;

            while(true) {
                Array.Clear(bytes, 0, bytes.Length);
                int n = stream.Read(bytes, 0, read_size);
                vector.Add(bytes);
                bytes_read += n;

                if(!set_first_chunk) {
                    if(copyFirstChunk) {
                        if(firstChunk == null || firstChunk.Length != read_size) {
                            firstChunk = new byte[read_size];
                        }

                        Array.Copy(bytes, 0, firstChunk, 0, n);
                    }
                    set_first_chunk = true;
                }

                if((bytes_read == stream.Length && stream.Length > 0) || 
                    (n < read_size && stream.Length <= 0)) {
                    break;
                }
            }

            if(stream.Length > 0 && vector.Count != stream.Length) {
                vector.Resize((int)stream.Length);
            }

            return vector;
        }
      
        #endregion
      
        #region Utilities
      
        private static System.Text.Encoding StringTypeToEncoding(StringType type, ByteVector bom)
        {
            switch(type) {
                case StringType.UTF16:
                    return (bom == null || (bom [0] == 0xFF && bom [1] == 0xFE)) 
                        ? System.Text.Encoding.Unicode 
                        : System.Text.Encoding.BigEndianUnicode;
                case StringType.UTF16BE:
                    return System.Text.Encoding.BigEndianUnicode;
                case StringType.UTF8:
                    return System.Text.Encoding.UTF8;
                case StringType.UTF16LE:
                    return System.Text.Encoding.Unicode;
            }
            
            try
            {
               // The right format but not ECMA.
               return System.Text.Encoding.GetEncoding("latin1");
            }
            catch
            {
               return System.Text.Encoding.UTF8;
            }
        }
      
        #endregion
      
        #region System.Object
      
        public override bool Equals(object o)
        {
            ByteVector vector = (ByteVector)o;
            return vector != null && vector == this;
        }

        public override int GetHashCode ()
        {
            return Count;
        }
        
        #endregion
        
        #region IComparable<T>
        
        public int CompareTo(ByteVector vector)
        {
            if(this == vector) {
                return 0;
            } else if(this < vector) {
                return -1;
            } else {
                return 1;
            }
        }
        
        #endregion
        
        #region IEnumerable<T>

        public IEnumerator<byte> GetEnumerator()
        {
            return data.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        
        #endregion

        #region ICollection<T>
        
        public void Clear()
        {
            data.Clear();
        }

        public void Add(byte value)
        {
            data.Add(value);
        }

        public bool Remove(byte value)
        {
            return data.Remove(value);
        }
        
        public void CopyTo(byte [] array, int index)
        {
            data.CopyTo(array, index);
        }
        
        public bool Contains(byte value)
        {
            return data.Contains(value);
        }

        public int Count {
            get { return data.Count; }
        }

        public bool IsSynchronized {
            get { return false; }
        }

        public object SyncRoot {
            get { return this; }
        }
        
        #endregion
        
        #region IList<T>
        
        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
        }
        
        public void Insert(int index, byte value)
        {
            data.Insert(index, value);
        }
        
        public int IndexOf(byte value)
        {
            return data.IndexOf(value);
        }
        
        public bool IsReadOnly {
            get { return false; }
        }
        
        public bool IsFixedSize {
            get { return false; }
        }
        
        public byte this[int index] {
            get { return data[index]; }
            set { data[index] = value; }
        }
        
        #endregion
    }
}
