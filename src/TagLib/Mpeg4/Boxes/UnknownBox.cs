namespace TagLib.Mpeg4
{
   // This class is just used for clarity in saying, YES, this box IS unknown.
   public class UnknownBox : Box
   {
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public UnknownBox (BoxHeader header, Box parent) : base (header, parent)
      {
      }
   }
}
