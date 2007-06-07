/***************************************************************************
    copyright            : (C) 2005 by Brian Nickel
    email                : brian.nickel@gmail.com
    based on             : id3v2frame.cpp from TagLib
 ***************************************************************************/

using System.Collections;
using System;

namespace TagLib.Id3v2
{
   public class PlayCountFrame : Frame
   {
      #region Private Properties
      private ulong play_count = 0;
      #endregion
      
      
      
      #region Constructors
      public PlayCountFrame (ByteVector data, byte version) : base (data, version)
      {
         SetData (data, 0, version, true);
      }
      
      public PlayCountFrame () : base (FrameType.PCNT, 4)
      {}
      
      protected internal PlayCountFrame (ByteVector data, int offset, FrameHeader header, byte version) : base(header)
      {
         SetData (data, offset, version, false);
      }
      #endregion
      
      
      
      #region Public Properties
      public ulong PlayCount
      {
         get {return play_count;}
         set {play_count = value;}
      }
      #endregion
      
      
      
      #region Public Methods
      public override string ToString ()
      {
         return null;
      }
      #endregion
      
      
      
      #region Public Static Methods
      public static PlayCountFrame Get (Tag tag, bool create)
      {
         foreach (Frame f in tag)
            if (f is PlayCountFrame)
               return f as PlayCountFrame;
         
         if (!create)
            return null;
         
         PlayCountFrame frame = new PlayCountFrame ();
         tag.AddFrame (frame);
         return frame;
      }
      #endregion
      
      
      
      #region Protected Methods
      protected override void ParseFields (ByteVector data, byte version)
      {
         play_count = data.ToULong ();
      }
      
      protected override ByteVector RenderFields (byte version)
      {
         ByteVector data = ByteVector.FromULong (play_count);
         while (data.Count > 4 && data [0] == 0)
            data.RemoveAt (0);
         
         return data;
      }
      #endregion
   }
}