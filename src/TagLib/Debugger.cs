/***************************************************************************
    copyright            : (C) 2006 by Brian Nickel
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

using System;

namespace TagLib
{
   public  class Debugger
   {
      public delegate void DebugMessageSentHandler (string message);
      
      public static event DebugMessageSentHandler DebugMessageSent = null;
      
      public static void Debug (string message)
      {
         if (DebugMessageSent != null)
            DebugMessageSent (message);
      }
      
      public static void DumpHex (ByteVector data)
      {
         string s = "0123456789abcdef";
         foreach (byte b in data)
            System.Console.Write (s.Substring (b / 16, 1) + s.Substring (b % 16, 1) + " ");
         System.Console.WriteLine ("");
      }
      
      private Debugger () {}
   }
}
