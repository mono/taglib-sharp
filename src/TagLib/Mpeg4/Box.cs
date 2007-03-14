using System;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class Box : IComparable<Box>
   {
      #region Private   Properties
      private BoxHeader header;
      private Box handler;
      private long data_offset;
      #endregion
      
      #region Constructors
      protected Box (BoxHeader header, File file, Box handler)
      {
         this.header      = header;
         this.data_offset = header.Position + header.DataOffset;
         this.handler     = handler;
      }
      
      protected Box (BoxHeader header) : this (header, null, null)
      {
      }
      
      protected Box (ByteVector type) : this (new BoxHeader (type))
      {
      }
      #endregion
      
      // Render the complete box including children.
      public virtual ByteVector Render ()
      {
         return Render (new ByteVector ());
      }
      
      public    virtual ByteVector BoxType      {get {return header.BoxType;}}
      public    virtual int        Size         {get {return (int)header.TotalBoxSize;}}
      protected         int        DataSize     {get {return (int)(header.DataSize + data_offset - DataOffset);}}
      protected virtual long       DataOffset   {get {return data_offset;}}
      protected         BoxHeader  Header       {get {return header;}}
      
      public virtual ByteVector Data  {get {return null;} set {}}
      public virtual BoxList Children {get {return null;}}
      
      protected BoxList LoadChildren (File file)
      {
         BoxList children = new BoxList ();
         
         long position = DataOffset;
         long end = position + DataSize;
         
         while (position < end)
         {
            Box child = BoxFactory.CreateBox (file, position, header, handler);
            children.Add (child);
            position += child.Size;
         }
         
         return children;
      }
      
      protected ByteVector LoadData (File file)
      {
         file.Seek (DataOffset);
         return file.ReadBlock (DataSize);
      }
      
      // The handler used for this box.
      public IsoHandlerBox Handler
      {
         get {return handler as IsoHandlerBox;}
      }
      
      public int CompareTo (Box box)
      {
         return (int)(DataSize - box.DataSize);
      }
      
      // Render a box with the "data" before its content.
      protected virtual ByteVector Render (ByteVector data)
      {
         bool free_found = false;
         ByteVector output = new ByteVector ();
         
         if (Children != null && Children.Count != 0)
            foreach (Box box in Children)
               if (box.GetType () == typeof (IsoFreeSpaceBox))
                  free_found = true;
               else
                  output.Add (box.Render ());
         else if (Data != null)
            output.Add (Data);
         
         // If there was a free, don't take it away, and let meta be a special case.
         if (free_found || BoxType == BoxTypes.Meta)
         {
            long size_difference =  header.DataSize - output.Count;
            
            // If we have room for free space, add it so we don't have to resize the file.
            if (header.DataSize != 0 && size_difference >= 8)
               output.Add ((new IsoFreeSpaceBox (size_difference)).Render ());
            
            // If we're getting bigger, get a lot bigger so we might not have to again.
            else
               output.Add ((new IsoFreeSpaceBox (2048)).Render ());
         }
         
         // Adjust the header's data size to match the content.
         header.DataSize = data.Count + output.Count;
         
         // Render the full box.
         output.Insert (0, data);
         output.Insert (0, header.Render ());
         return output;
      }
      
      internal void DumpTree (string start)
      {
         if (BoxType == BoxTypes.Data)
            System.Console.WriteLine (start + BoxType.ToString () + " " + (this as AppleDataBox).Text);
         else
            System.Console.WriteLine (start + BoxType.ToString ());
         
         if (Children != null)
            foreach (Box child in Children)
               child.DumpTree (start + "   ");
      }
   }
}
