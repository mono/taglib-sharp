using System;
using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class Box
   {
      #region Private       Properties
      private BoxHeader     header;
      private IsoHandlerBox handler;
      private long          data_position;
      #endregion
      
      #region Constructors
      protected Box (BoxHeader header, IsoHandlerBox handler)
      {
         this.header        = header;
         this.data_position = header.Position + header.DataOffset;
         this.handler       = handler;
      }
      
      protected Box (BoxHeader header) : this (header, null)
      {
      }
      
      protected Box (ByteVector type) : this (new BoxHeader (type))
      {
      }
      #endregion
      
      // Render the complete box including children.
      public ByteVector Render ()
      {
         return Render (new ByteVector ());
      }
      
      public    virtual ByteVector BoxType      {get {return header.BoxType;}}
      public    virtual int        Size         {get {return (int)header.TotalBoxSize;}}
      protected         int        DataSize     {get {return (int)(header.DataSize + data_position - DataPosition);}}
      protected virtual long       DataPosition {get {return data_position;}}
      protected         BoxHeader  Header       {get {return header;}}
      
      public virtual ByteVector Data  {get {return null;} set {}}
      public virtual IEnumerable<Box> Children {get {return null;}}
      
      protected IEnumerable<Box> LoadChildren (TagLib.File file)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         List<Box> children = new List<Box> ();
         
         long position = DataPosition;
         long end = position + DataSize;
         
         header.Box = this;
         while (position < end)
         {
            Box child = BoxFactory.CreateBox (file, position, header, handler, children.Count);
            children.Add (child);
            position += child.Size;
         }
         header.Box = null;
         
         return children;
      }
      
      protected ByteVector LoadData (TagLib.File file)
      {
         if (file == null)
            throw new ArgumentNullException ("file");
         
         file.Seek (DataPosition);
         return file.ReadBlock (DataSize);
      }
      
      // The handler used for this box.
      public IsoHandlerBox Handler
      {
         get {return handler;}
      }
      
      // Render a box with the "data" before its content.
      protected virtual ByteVector Render (ByteVector topData)
      {
         bool free_found = false;
         ByteVector output = new ByteVector ();
         
         if (Children != null)
            foreach (Box box in Children)
               if (box.GetType () == typeof (IsoFreeSpaceBox))
                  free_found = true;
               else
                  output.Add (box.Render ());
         else if (Data != null)
            output.Add (Data);
         
         // If there was a free, don't take it away, and let meta be a special case.
         if (free_found || BoxType == Mpeg4.BoxType.Meta)
         {
            long size_difference = DataSize - output.Count;
            
            // If we have room for free space, add it so we don't have to resize the file.
            if (header.DataSize != 0 && size_difference >= 8)
               output.Add ((new IsoFreeSpaceBox (size_difference)).Render ());
            
            // If we're getting bigger, get a lot bigger so we might not have to again.
            else
               output.Add ((new IsoFreeSpaceBox (2048)).Render ());
         }
         
         // Adjust the header's data size to match the content.
         header.DataSize = topData.Count + output.Count;
         
         // Render the full box.
         output.Insert (0, topData);
         output.Insert (0, header.Render ());
         return output;
      }
      
      /*internal void DumpTree (string start)
      {
         if (BoxType == BoxType.Data)
            System.Console.WriteLine (start + BoxType.ToString () + " " + (this as AppleDataBox).Text);
         else
            System.Console.WriteLine (start + BoxType.ToString ());
         
         if (Children != null)
            foreach (Box child in Children)
               child.DumpTree (start + "   ");
      }*/
      
      public Box GetChild (ByteVector type)
      {
         if (Children != null)
            foreach (Box box in Children)
               if (box.BoxType == type)
                  return box;
         
         return null;
      }
      
/*      public Box GetChild (System.Type type)
      {
         if (Children != null)
            foreach (Box box in Children)
               if (box.GetType () == type)
                  return box;
         
         return null;
      }*/
      
      public Box GetChildRecursively (ByteVector type)
      {
         if (Children == null)
            return null;
         
         foreach (Box box in Children)
            if (box.BoxType == type)
               return box;
         
         foreach (Box box in Children)
         {
            Box child_box = box.GetChildRecursively (type);
            if (child_box != null)
               return child_box;
         }
         
         return null;
      }
      
/*      public Box GetChildRecursively (System.Type type)
      {
         if (Children == null)
            return null;
         
         foreach (Box box in Children)
            if (box.GetType () == type)
               return box;
         
         foreach (Box box in Children)
         {
            Box child_box = box.GetChildRecursively (type);
            if (child_box != null)
               return child_box;
         }
         
         return null;
      }*/
      
      public void RemoveChild (ByteVector type)
      {
         if (Children != null && Children is ICollection<Box>)
            foreach (Box b in new List<Box> (Children))
               if (b.BoxType == type)
                  (Children as ICollection<Box>).Remove (b);
      }
      
/*      public void RemoveChild (System.Type type)
      {
         if (Children != null && Children is ICollection<Box>)
            foreach (Box b in Children)
               if (b.GetType () == type)
                  (Children as ICollection<Box>).Remove (b);
      }*/
      
      public void RemoveChild (Box box)
      {
         if (Children != null && Children is ICollection<Box>)
            (Children as ICollection<Box>).Remove (box);
      }
      
      public void AddChild (Box box)
      {
         if (Children != null && Children is ICollection<Box>)
            (Children as ICollection<Box>).Add (box);
      }
      
      public void ClearChildren ()
      {
         if (Children != null && Children is ICollection<Box>)
            (Children as ICollection<Box>).Clear ();
      }
      
      public bool HasChildren {
         get {
            return Children != null && Children is ICollection<Box>
               && (Children as ICollection<Box>).Count != 0;
         }
      }
   }
}
