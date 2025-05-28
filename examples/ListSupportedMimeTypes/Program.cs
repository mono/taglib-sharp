using System;
using TagLib;

namespace ListSupportedMimeTypes;

public class Program
{
	public static void Main()
	{
		foreach(string type in SupportedMimeType.AllMimeTypes) {
			Console.WriteLine(type);
		}
	}
}

