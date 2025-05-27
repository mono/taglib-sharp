using File = TagLib.File;

namespace TaglibSharp.Tests
{
	public class Debugger
	{
		public static void DumpHex (ByteVector data)
		{
			DumpHex (data.Data);
		}

		public static void DumpHex (byte[] data)
		{
			int cols = 16;
			int rows = data.Length / cols +
				(data.Length % cols != 0 ? 1 : 0);

			for (int row = 0; row < rows; row++) {
				for (int col = 0; col < cols; col++) {
					if (row == rows - 1 &&
						data.Length % cols != 0 &&
						col >= data.Length % cols)
						Console.Write ("   ");
					else
						Console.Write (" {0:x2}",
							data[row * cols + col]);
				}

				Console.Write (" | ");

				for (int col = 0; col < cols; col++) {
					if (row == rows - 1 &&
						data.Length % cols != 0 &&
						col >= data.Length % cols)
						Console.Write (" ");
					else
						WriteByte2 (data[row * cols + col]);
				}

				Console.WriteLine ();
			}
			Console.WriteLine ();
		}

		static void WriteByte2 (byte data)
		{
			foreach (char c in allowed)
				if (c == data) {
					Console.Write (c);
					return;
				}

			Console.Write (".");
		}

		static readonly string allowed = "0123456789abcdefghijklmnopqr" +
			"stuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ`~!@#$%^&*()_+-={}" +
			"[];:'\",.<>?/\\|";
	}

	public class MemoryFileAbstraction : File.IFileAbstraction
	{
		readonly MemoryStream stream;

		public MemoryFileAbstraction (int maxSize, byte[] data)
		{
			stream = new MemoryStream (maxSize);
			stream.Write (data, 0, data.Length);
			stream.Position = 0;
		}

		public string Name => "MEMORY";

		public Stream ReadStream => stream;

		public Stream WriteStream => stream;

		public void CloseStream (Stream stream)
		{
			// This causes a stackoverflow
			//stream?.Close();
		}
	}

	public class CodeTimer : IDisposable
	{
		readonly DateTime start;
		TimeSpan elapsed_time = TimeSpan.Zero;
		readonly string label;

		public CodeTimer ()
		{
			start = DateTime.Now;
		}

		public CodeTimer (string label) : this ()
		{
			this.label = label;
		}

		public TimeSpan ElapsedTime {
			get {
				var now = DateTime.Now;
				return elapsed_time == TimeSpan.Zero ? now - start : elapsed_time;
			}
		}

		public void WriteElapsed (string message)
		{
			Console.WriteLine ("{0} {1} {2}", label, message, ElapsedTime);
		}

		public void Dispose ()
		{
			elapsed_time = DateTime.Now - start;
			if (label != null)
				WriteElapsed ("timer stopped:");
		}
	}
}
