//
// Debugger.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2006-2007 Brian Nickel
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

using System;
using System.Collections.Generic;

namespace TagLib {
	public static class Debugger
	{
		public delegate void DebugMessageSentHandler (string message);

		public static event DebugMessageSentHandler DebugMessageSent;

		public static void Debug (string message)
		{
			if (DebugMessageSent != null)
				DebugMessageSent (message);
		}

		public static void DumpHex (ByteVector data)
		{
			foreach (byte b in data)
				Console.Write ("{0:x2}", b);
			Console.WriteLine (String.Empty);
		}

		private static Dictionary <object, Dictionary <object, DebugTimeData>>
			debug_times = new Dictionary <object, Dictionary <object, DebugTimeData>> ();

		public static void AddDebugTime (object o1, object o2, DateTime start)
		{
			DebugTimeData data = new DebugTimeData (DateTime.Now - start, 1);
			if (debug_times.ContainsKey (o1) && debug_times [o1].ContainsKey (o2)) {
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

			foreach (KeyValuePair <object, DebugTimeData> pair in debug_times [o1]) {
				Console.WriteLine ("  {0}", pair.Key.ToString ());
				Console.WriteLine ("    Objects: {0}", pair.Value.time);
				Console.WriteLine ("    Total:   {0}", pair.Value.occurances);
				Console.WriteLine ("    Average: {0}", new TimeSpan (pair.Value.time.Ticks / pair.Value.occurances));
				Console.WriteLine (String.Empty);
			}
			
			debug_times.Remove (o1);
		}

		private struct DebugTimeData
		{
			public TimeSpan time;
			public long occurances;

			public DebugTimeData (TimeSpan time, int occurances)
			{
				this.time = time;
				this.occurances = occurances;
			}
		}
	}
}
