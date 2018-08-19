using Gtk;

namespace TaglibSharp.Tests
{
	public class MainWindow : Window
	{
		static void Main (string[] args)
		{
			Application.Init ();

			new MainWindow ();

			Application.Run ();
		}

		public MainWindow () : base ("MainWindow")
		{
			// Setup ui
			var textview = new TextView ();
			Add (textview);

			// Setup tag
			var tag = new TextTag ("helloworld-tag") {
				Scale = Pango.Scale.XXLarge,
				Style = Pango.Style.Italic,
				Underline = Pango.Underline.Double,
				Foreground = "blue",
				Background = "pink",
				Justification = Justification.Center
			};
			var buffer = textview.Buffer;
			buffer.TagTable.Add (tag);

			// Insert "Hello world!" into textview buffer
			var insertIter = buffer.StartIter;
			buffer.InsertWithTagsByName (ref insertIter, "Hello World!\n", "helloworld-tag");
			buffer.Insert (ref insertIter, "Simple Hello World!");

			ShowAll ();
		}
	}
}
