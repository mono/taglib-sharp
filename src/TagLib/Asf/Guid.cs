/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : wmafile.cpp from libtunepimp
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
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111, 0x1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System.Text;
using System.Collections;
using System;

namespace TagLib.Asf
{
   public struct Guid
   {
      public uint   part1;
      public ushort part2;
      public ushort part3;
      public ushort part4;
      public ulong  part5;
      
      public Guid (ByteVector raw)
      {
         this.part1 = raw.Mid ( 0, 4).ToUInt  (false);
         this.part2 = raw.Mid ( 4, 2).ToUShort (false);
         this.part3 = raw.Mid ( 6, 2).ToUShort (false);
         this.part4 = raw.Mid ( 8, 2).ToUShort (true);
         this.part5 = raw.Mid (10, 6).ToULong (true);
      }
      
      public Guid (uint part1, ushort part2, ushort part3, ushort part4, ulong part5)
      {
         this.part1 = part1;
         this.part2 = part2;
         this.part3 = part3;
         this.part4 = part4;
         this.part5 = part5;
      }
      
      public ByteVector Render ()
      {
         ByteVector v = ByteVector.FromUInt  (part1, false).Mid (0, 4);
         v.Add (ByteVector.FromUShort (part2, false).Mid (0, 2));
         v.Add (ByteVector.FromUShort (part3, false).Mid (0, 2));
         v.Add (ByteVector.FromUShort (part4, true).Mid (0, 2));
         v.Add (ByteVector.FromULong  (part5, true).Mid (2, 6));
         return v;
      }
      
      public static Guid AsfHeaderObject                     = new Guid (0x75B22630, 0x668E, 0x11CF, 0xA6D9, 0x00AA0062CE6C);
      public static Guid AsfFilePropertiesObject             = new Guid (0x8CABDCA1, 0xA947, 0x11CF, 0x8EE4, 0x00C00C205365);
      public static Guid AsfStreamPropertiesObject           = new Guid (0xB7DC0791, 0xA9B7, 0x11CF, 0x8EE6, 0x00C00C205365);
      public static Guid AsfContentDescriptionObject         = new Guid (0x75B22633, 0x668E, 0x11CF, 0xA6D9, 0x00AA0062CE6C);
      public static Guid AsfExtendedContentDescriptionObject = new Guid (0xD2D0A440, 0xE307, 0x11D2, 0x97F0, 0x00A0C95EA850);
      public static Guid AsfPaddingObject                    = new Guid (0x1806D474, 0xCADF, 0x4509, 0xA4BA, 0x9AABCB96AAE8);
      public static Guid AsfAudioMedia                       = new Guid (0xF8699E40, 0x5B4D, 0x11CF, 0xA8FD, 0x00805F5C442B);
      public static Guid AsfVideoMedia                       = new Guid (0xBC19EFC0, 0x5B4D, 0x11CF, 0xA8FD, 0x00805F5C442B);
      
      //////////////////////////////////////////////////////////////////////////
      // operators
      //////////////////////////////////////////////////////////////////////////
      public static bool operator== (Guid a, Guid b)
      {
         return a.part1 == b.part1 &&
                a.part2 == b.part2 &&
                a.part3 == b.part3 &&
                a.part4 == b.part4 &&
                a.part5 == b.part5;
      }
      
      public static bool operator!= (Guid a, Guid b)
      {
         return !(a == b);
      }
      
      public override bool Equals (object o)
      {
         Guid v = (Guid) o;
         return v == this;
      }
      
      public override int GetHashCode ()
      {
         return (int) (part1 * part2 * part3 * part4 * part5);
      }
      
      public new string ToString ()
      {
         string dash = "-";
         StringBuilder b = new StringBuilder (36);
         b.Append (RenderNumber (part1, 8));
         b.Append (dash);
         b.Append (RenderNumber (part2, 4));
         b.Append (dash);
         b.Append (RenderNumber (part3, 4));
         b.Append (dash);
         b.Append (RenderNumber (part4, 4));
         b.Append (dash);
         b.Append (RenderNumber (part5, 12));
         return b.ToString ();
      }
      
      private string RenderNumber (ulong value, int length)
      {
         string s = value.ToString ("x");
         if (s.Length > length)
            return s.Substring (s.Length - length);
         
         StringBuilder b = new StringBuilder (length);
         b.Append (s);
         while (b.Length < length)
            b.Insert (0, '0');
         
         return b.ToString ();
      }
   }
}
