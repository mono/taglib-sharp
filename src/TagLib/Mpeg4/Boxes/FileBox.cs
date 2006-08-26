namespace TagLib.Mpeg4
{
   public class FileBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private File file;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      
      // FileBox cannot be read from a file and is a seed box for a file.
      public FileBox (File file) : base (null)
      {
         this.file = file;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public    override bool       IsValid           {get {return file.IsValid;}}
      public    override bool       HasChildren       {get {return true;}}
      public    override File       File              {get {return file;}}
      public    override ByteVector BoxType           {get {return "FILE";}}
      public    override ulong      BoxSize           {get {return (ulong) file.Length;}}
      protected override long       DataPosition      {get {return 0;}}
      public    override long       NextBoxPosition   {get {return file.Length;}}
      public    override ByteVector Data              {get {return null;} set {}}
   }
}
