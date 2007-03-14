namespace TagLib.Mpeg4
{
	public static class BoxTypes
	{
	   public static readonly ByteVector Alb  = FixId ("alb");
	   public static readonly ByteVector Art  = FixId ("ART");
	   public static readonly ByteVector Cmt  = FixId ("cmt");
	   public static readonly ByteVector Covr = "covr";
	   public static readonly ByteVector Co64 = "co64";
	   public static readonly ByteVector Cpil = "cpil";
	   public static readonly ByteVector Data = "data";
	   public static readonly ByteVector Day  = FixId ("day");
	   public static readonly ByteVector Disk = "disk";
	   public static readonly ByteVector Esds = "esds";
	   public static readonly ByteVector Ilst = "ilst";
	   public static readonly ByteVector Free = "free";
	   public static readonly ByteVector Gen  = FixId ("gen");
	   public static readonly ByteVector Gnre = "gnre";
	   public static readonly ByteVector Hdlr = "hdlr";
	   public static readonly ByteVector Lyr  = FixId ("lyr");
	   public static readonly ByteVector Mdia = "mdia";
	   public static readonly ByteVector Meta = "meta";
	   public static readonly ByteVector Mean = "mean";
	   public static readonly ByteVector Minf = "minf";
	   public static readonly ByteVector Moov = "moov";
	   public static readonly ByteVector Mvhd = "mvhd";
	   public static readonly ByteVector Nam  = FixId ("nam");
	   public static readonly ByteVector Name = "name";
	   public static readonly ByteVector Skip = "skip";
	   public static readonly ByteVector Stbl = "stbl";
	   public static readonly ByteVector Stco = "stco";
	   public static readonly ByteVector Stsd = "stsd";
	   public static readonly ByteVector Trak = "trak";
	   public static readonly ByteVector Trkn = "trkn";
	   public static readonly ByteVector Udta = "udta";
	   public static readonly ByteVector Uuid = "uuid";
	   public static readonly ByteVector Wrt  = FixId ("wrt");
	   public static readonly ByteVector DASH = "----";
	   
      private static ByteVector FixId (ByteVector v)
      {
         // IF we have a three byte type (like "wrt"), add the extra byte.
         if (v.Count == 3)
            v.Insert (0, 0xa9);
         return v;
      }
	}
}
