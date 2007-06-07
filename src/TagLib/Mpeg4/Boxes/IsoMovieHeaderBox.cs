using System;

namespace TagLib.Mpeg4
{
   public class IsoMovieHeaderBox : FullBox
   {
      #region Private Properties
      private ulong  creation_time;
      private ulong  modification_time;
      private uint   timescale;
      private ulong  duration;
      private uint   rate;
      private ushort volume;
      private uint   next_track_id;
      #endregion
      
      #region Constructors
      public IsoMovieHeaderBox (BoxHeader header, TagLib.File file, IsoHandlerBox handler) : base (header, file, handler)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         int bytes_remaining = DataSize;
         ByteVector data;
         
         // Read version one (large integers).
         if (Version == 1)
         {
            data = file.ReadBlock (Math.Min (28, bytes_remaining));
            if (data.Count >=  8) creation_time     = data.Mid ( 0, 8).ToULong ();
            if (data.Count >= 16) modification_time = data.Mid ( 8, 8).ToULong ();
            if (data.Count >= 20) timescale         = data.Mid (16, 4).ToUInt ();
            if (data.Count >= 28) duration          = data.Mid (20, 8).ToULong ();
            bytes_remaining -= 28;
         }
         // Read version zero (normal integers).
         else
         {
            data = file.ReadBlock (Math.Min (16, bytes_remaining));
            if (data.Count >=  4) creation_time     = data.Mid ( 0, 4).ToUInt ();
            if (data.Count >=  8) modification_time = data.Mid ( 4, 4).ToUInt ();
            if (data.Count >= 12) timescale         = data.Mid ( 8, 4).ToUInt ();
            if (data.Count >= 16) duration          = data.Mid (12, 4).ToUInt ();
            bytes_remaining -= 16;
         }
         
         data = file.ReadBlock (Math.Min (6, bytes_remaining));
         if (data.Count >= 4) rate   = data.Mid (0, 4).ToUInt ();
         if (data.Count >= 6) volume = data.Mid (4, 2).ToUShort ();
         file.Seek (file.Tell + 70);
         bytes_remaining -= 76;
         
         data = file.ReadBlock (Math.Min (4, bytes_remaining));
         if (data.Count >= 4) next_track_id = data.Mid (0, 4).ToUInt ();
      }
      #endregion
      
      #region Public Properties
      public DateTime CreationTime     {get {return (new System.DateTime (1904, 1, 1, 0, 0, 0)).AddTicks ((long)(10000000 * creation_time));}}
      public DateTime ModificationTime {get {return (new System.DateTime (1904, 1, 1, 0, 0, 0)).AddTicks ((long)(10000000 * modification_time));}}
      public TimeSpan Duration
      {
         get
         {
            // The length is the number of ticks divided by ticks per second.
            return TimeSpan.FromSeconds ((double) duration / (double) timescale);
         }
      }
      public double   Rate             {get {return ((double) rate) / ((double) 0x10000);}}
      public double   Volume           {get {return ((double) volume) / ((double) 0x100);}}
      public uint     NextTrackId      {get {return next_track_id;}}
      #endregion
   }
}
