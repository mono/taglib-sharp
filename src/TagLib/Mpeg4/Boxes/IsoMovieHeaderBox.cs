using System;

namespace TagLib.Mpeg4
{
   public class IsoMovieHeaderBox : FullBox
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private ulong  creation_time;
      private ulong  modification_time;
      private uint   timescale;
      private ulong  duration;
      
      private uint   rate;
      private ushort volume;
      private uint   next_track_id;
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public IsoMovieHeaderBox (BoxHeader header, Box parent) : base (header, parent)
      {
         ByteVector data = InternalData;
         int pos = 4;
         
         // Read version one (large integers).
         if (Version == 1)
         {
            if (data.Count >= pos + 8)
               creation_time = (ulong) data.Mid (pos, 8).ToLong ();
            pos += 8;
            
            if (data.Count >= pos + 8)
               modification_time = (ulong) data.Mid (pos, 8).ToLong ();
            pos += 8;
            
            if (data.Count >= pos + 4)
               timescale = data.Mid (pos, 4).ToUInt ();
            pos += 4;
            
            if (data.Count >= pos + 8)
               duration = (ulong) data.Mid (pos, 8).ToLong ();
            pos += 8;
         }
         // Read version zero (normal integers).
         else
         {
            if (data.Count >= pos + 4)
               creation_time = data.Mid (pos, 4).ToUInt ();
            pos += 4;
            
            if (data.Count >= pos + 4)
               modification_time = data.Mid (pos, 4).ToUInt ();
            pos += 4;
            
            if (data.Count >= pos + 4)
               timescale = data.Mid (pos, 4).ToUInt ();
            pos += 4;
            
            if (data.Count >= pos + 4)
               duration = (ulong) data.Mid (pos, 4).ToUInt ();
            pos += 4;
         }
         
         // Get rate
         if (data.Count >= pos + 4)
            rate = data.Mid (pos, 4).ToUInt ();
         pos += 4;
         
         // Get volume
         if (data.Count >= pos + 2)
            volume = (ushort) data.Mid (pos, 2).ToShort ();
         pos += 2;
         
         // reserved
         pos += 2;
         
         // reserved
         pos += 8;
         
         // video transformation matrix
         pos += 36;
         
         // pre-defined
         pos += 24;
         
         // Get next track ID
         if (data.Count >= pos + 4)
            next_track_id = (ushort) data.Mid (pos, 4).ToUInt ();
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      public DateTime CreationTime     {get {return (new System.DateTime (1904, 1, 1, 0, 0, 0)).AddTicks ((long)(10000000 * creation_time));}}
      public DateTime ModificationTime {get {return (new System.DateTime (1904, 1, 1, 0, 0, 0)).AddTicks ((long)(10000000 * modification_time));}}
      public uint     TimeScale        {get {return timescale;}}
      public ulong    Duration         {get {return duration;}}
      public double   Rate             {get {return ((double) rate) / ((double) 0x10000);}}
      public double   Volume           {get {return ((double) volume) / ((double) 0x100);}}
      public uint     NextTrackId      {get {return next_track_id;}}
      
      public    override bool       HasChildren  {get {return true;}}
      protected override long       DataPosition {get {return base.DataPosition + (Version == 1 ? 108 : 96);}}
      protected override ulong      DataSize     {get {return base.DataSize - (ulong)(Version == 1 ? 108 : 96);}}
      public    override ByteVector Data         {get {return null;} set {}}
   }
}
