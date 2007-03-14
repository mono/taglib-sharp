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
using System.Collections.Generic;

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
            Console.Write (s.Substring (b / 16, 1) + s.Substring (b % 16, 1) + " ");
         Console.WriteLine (String.Empty);
      }
      
      private static Dictionary <object, Dictionary <object, DebugTimeData>>
         debug_times = new Dictionary <object, Dictionary <object, DebugTimeData>> ();
      
      public static void AddDebugTime (object o1, object o2, DateTime start)
      {
         DebugTimeData data = new DebugTimeData (DateTime.Now - start, 1);
         if (debug_times.ContainsKey (o1) && debug_times [o1].ContainsKey (o2))
         {
            data.time       += debug_times [o1][o2].time;
            data.occurances += debug_times [o1][o2].occurances;
         }
         
         if (!debug_times.ContainsKey (o1))
            debug_times.Add (o1, new Dictionary <object, DebugTimeData> ());
         
         if (!debug_times [o1].ContainsKey (o2))
            debug_times [o1].Add (o2, data);
         else
            debug_times [o1][o2] = data;
      }
      
      public static void DumpDebugTime (object o1)
      {
         Console.WriteLine (o1.ToString ());
         if (!debug_times.ContainsKey (o1))
            return;
         
         foreach (KeyValuePair <object, DebugTimeData> pair in debug_times [o1])
         {
            Console.WriteLine ("  " + pair.Key.ToString ());
            Console.WriteLine ("    Objects: " + pair.Value.time);
            Console.WriteLine ("    Total:   " + pair.Value.occurances);
            Console.WriteLine ("    Average: " + new TimeSpan (pair.Value.time.Ticks / pair.Value.occurances));
            Console.WriteLine (String.Empty);
         }
         debug_times.Remove (o1);
      }
      
      private Debugger () {}
      
      private struct DebugTimeData
      {
         public TimeSpan time;
         public long     occurances;
         
         public DebugTimeData (TimeSpan time, int occurances)
         {
            this.time = time;
            this.occurances = occurances;
         }
      }
   }
}
