namespace TagLib.Mpeg4
{
   public static class BoxTypes
   {
      public static ByteVector Aart {get {return "aART";}}
      public static ByteVector Alb  {get {return AppleTag.FixId ("alb");}}
      public static ByteVector Art  {get {return AppleTag.FixId ("ART");}}
      public static ByteVector Cmt  {get {return AppleTag.FixId ("cmt");}}
      public static ByteVector Cond {get {return "cond";}}
      public static ByteVector Covr {get {return "covr";}}
      public static ByteVector Co64 {get {return "co64";}}
      public static ByteVector Cpil {get {return "cpil";}}
      public static ByteVector Cprt {get {return "cprt";}}
      public static ByteVector Data {get {return "data";}}
      public static ByteVector Day  {get {return AppleTag.FixId ("day");}}
      public static ByteVector Disk {get {return "disk";}}
      public static ByteVector Esds {get {return "esds";}}
      public static ByteVector Ilst {get {return "ilst";}}
      public static ByteVector Free {get {return "free";}}
      public static ByteVector Gen  {get {return AppleTag.FixId ("gen");}}
      public static ByteVector Gnre {get {return "gnre";}}
      public static ByteVector Grp  {get {return AppleTag.FixId("grp");}}
      public static ByteVector Hdlr {get {return "hdlr";}}
      public static ByteVector Lyr  {get {return AppleTag.FixId ("lyr");}}
      public static ByteVector Mdia {get {return "mdia";}}
      public static ByteVector Meta {get {return "meta";}}
      public static ByteVector Mean {get {return "mean";}}
      public static ByteVector Minf {get {return "minf";}}
      public static ByteVector Moov {get {return "moov";}}
      public static ByteVector Mvhd {get {return "mvhd";}}
      public static ByteVector Nam  {get {return AppleTag.FixId ("nam");}}
      public static ByteVector Name {get {return "name";}}
      public static ByteVector Skip {get {return "skip";}}
      public static ByteVector Stbl {get {return "stbl";}}
      public static ByteVector Stco {get {return "stco";}}
      public static ByteVector Stsd {get {return "stsd";}}
      public static ByteVector Tmpo {get {return "tmpo";}}
      public static ByteVector Trak {get {return "trak";}}
      public static ByteVector Trkn {get {return "trkn";}}
      public static ByteVector Udta {get {return "udta";}}
      public static ByteVector Uuid {get {return "uuid";}}
      public static ByteVector Wrt  {get {return AppleTag.FixId ("wrt");}}
      public static ByteVector DASH {get {return "----";}}
   }
}
