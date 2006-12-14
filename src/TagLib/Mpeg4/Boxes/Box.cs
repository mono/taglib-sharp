using System.Collections.Generic;

namespace TagLib.Mpeg4
{
   public class Box
   {
      //////////////////////////////////////////////////////////////////////////
      // private properties
      //////////////////////////////////////////////////////////////////////////
      private BoxHeader  header;
      private Box        parent;
      private ByteVector data;
      private bool       load_children_started;
      private List<Box>  children;
      
      
      //////////////////////////////////////////////////////////////////////////
      // public methods
      //////////////////////////////////////////////////////////////////////////
      public Box (BoxHeader header, Box parent)
      {
         // Initialize everything.
         this.header   = header;
         this.parent   = parent;
         this.data     = null;
         this.children = new List<Box> ();
         this.load_children_started = false;
      }
      
      // Do the same as above, but accept a box type.
      public Box (ByteVector type, Box parent) : this (new BoxHeader (type), parent)
      {
      }
      
      // Who needs a parent? I'm all grown up.
      public Box (ByteVector type) : this (type, null)
      {
      }
      
      // Render the complete box including children.
      public virtual ByteVector Render ()
      {
         return Render (new ByteVector ());
      }
      
      // Overwrite the box's header with a new header incorporating a size
      // change.
      public virtual void OverwriteHeader (long size_change)
      {
         // If we don't have a header we can't do anything.
         if (header == null || header.Position < 0)
         {
            Debugger.Debug ("Box.OverWriteHeader() - No header to overwrite.");
            return;
         }
         
         // Make sure this alteration won't screw up the reading of children.
         LoadChildren ();
         
         // Save the header's original position and size.
         long position = header.Position;
         long old_size = header.HeaderSize;
         
         // Update the data size.
         header.DataSize = (ulong) ((long) header.DataSize + size_change);
         
         // Render the header onto the file.
         File.Insert (header.Render (), position, old_size);
      }
      
      // Find the first child with a given box type.
      public Box FindChild (ByteVector type)
      {
         foreach (Box child in Children)
            if (child.BoxType == type)
               return child;
         
         return null;
      }
      
      // Find the first child with a given System.Type.
      public Box FindChild (System.Type type)
      {
         foreach (Box child in Children)
            if (child.GetType () == type)
               return child;
         
         return null;
      }
      
      // Recursively find the first child with a given box type giving 
      // preference to the current depth.
      public Box FindChildDeep (ByteVector type)
      {
         foreach (Box child in Children)
            if (child.BoxType == type)
               return child;
         
         foreach (Box child in Children)
         {
            Box success = child.FindChildDeep (type);
            if (success != null)
               return success;
         }
         
         return null;
      }
      
      // Recursively find the first child with a given System.Type giving
      // preference to the current depth.
      public Box FindChildDeep (System.Type type)
      {
         foreach (Box child in Children)
            if (child.GetType () == type)
               return child;
         
         foreach (Box child in Children)
         {
            Box success = child.FindChildDeep (type);
            if (success != null)
               return success;
         }
         
         return null;
      }
      
      // Add a child to this box.
      public void AddChild (Box child)
      {
         children.Add (child);
         child.Parent = this;
      }
      
      // Remove a child from this box.
      public void RemoveChild (Box child)
      {
         children.Remove (child);
         child.Parent = null;
      }
      
      // Remove all children with a given box type.
      public void RemoveChildren (ByteVector type)
      {
         Box box;
         while ((box = FindChild (type)) != null)
            RemoveChild (box);
      }
      
      // Remove all children with a given System.Type.
      public void RemoveChildren (System.Type type)
      {
         Box box;
         while ((box = FindChild (type)) != null)
            RemoveChild (box);
      }
      
      // Detach the current box from its parent.
      public void RemoveFromParent ()
      {
         if (Parent != null)
            Parent.RemoveChild (this);
      }
      
      // Remove all children from this box.
      public void ClearChildren ()
      {
         foreach (Box box in children)
            box.Parent = null;
         children.Clear ();
      }
      
      // Replace a child with a new one.
      public void ReplaceChild (Box old_child, Box new_child)
      {
         int index = children.IndexOf (old_child);
         
         if (index >= 0)
         {
            children [index] = new_child;
            old_child.Parent = null;
            new_child.Parent = this;
         }
         else
            AddChild (new_child);
      }
      
      // Replace this box with another one.
      public void ReplaceWith (Box box)
      {
         if (Parent != null)
            Parent.ReplaceChild (this, box);
      }
      
      // Load this box's children as well as their children. 
      public void LoadChildren ()
      {
         if (!HasChildren || children.Count != 0 || load_children_started)
            return;
         
         load_children_started = true;
         
         Box box = FirstChild;
         while (box != null)
         {
            box.LoadChildren ();
            children.Add (box);
            box = NextChild (box);
         }
      }
      
      // Load the data stored in this box.
      public void LoadData ()
      {
         if (data == null && this.File != null && this.File.Mode != File.AccessMode.Closed)
         {
            File.Seek (DataPosition);
            data = File.ReadBlock ((int) DataSize);
         }
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public properties
      //////////////////////////////////////////////////////////////////////////
      
      // Is the box valid?
      public virtual bool  IsValid           {get {return header.IsValid;}}
      
      // The file associated with this box.
      public virtual File File               {get {return header.File;}}
      
      // The box's parent box.
      public Box Parent                      {get {return parent;} set {parent = value;}}
      
      // The box type.
      public virtual ByteVector BoxType      {get {return header.BoxType;}}
      
      // The total size of the box.
      public virtual ulong BoxSize           {get {return header.BoxSize;}}
      
      // The size of the non-header data.
      protected virtual ulong DataSize       {get {return header.DataSize;}}
      
      // The stream position of the box's data.
      protected virtual long DataPosition    {get {return header.DataPosition;}}
      
      // Whether or not the box can have children.
      public virtual bool  HasChildren       {get {return false;}}
      
      // The stream position of the next box.
      public virtual long NextBoxPosition    {get {return header.NextBoxPosition;}}
      
      // All child boxes of this box.
      public Box [] Children {get {LoadChildren (); return (Box []) children.ToArray ();}}
      
      // The handler used for this box.
      public IsoHandlerBox Handler
      {
         get
         {
            Box box = this;
            
            // Look in all parent boxes.
            while (box != null)
            {
               // Handlers will be contained in "meta" and "mdia" boxes.
               if (box.BoxType == "mdia" || box.BoxType == "meta")
               {
                  // See if you can find a handler, and return if you can.
                  IsoHandlerBox handler = (IsoHandlerBox) box.FindChild (typeof (IsoHandlerBox));
                  if (handler != null)
                     return handler;
               }
               
               // Check the parent next.
               box = box.Parent;
            }
            
            // Failure.
            return null;
         }
      }
      
      //////////////////////////////////////////////////////////////////////////
      // public static methods
      //////////////////////////////////////////////////////////////////////////
      
      // Create a box by reading the file and add it to "parent".
      public static Box Create (File file, long position, Box parent)
      {
         // Read the box header.
         BoxHeader h = new BoxHeader (file, position);
         
         // If we're not even valid, quit.
         if (!h.IsValid)
            return null;
         
         // IF we're in a SampleDescriptionBox and haven't loaded all the
         // entries, try loading an appropriate entry.
         if (parent.BoxType == "stsd" && parent.Children.Length < ((IsoSampleDescriptionBox) parent).EntryCount)
         {
            IsoHandlerBox handler = parent.Handler;
            if (handler != null && handler.HandlerType == "soun")
               return new IsoAudioSampleEntry (h, parent);
            else
               return new IsoSampleEntry (h, parent);
         }
         
         //
         // A bunch of standard items.
         //
         
         if (h.BoxType == "moov")
            return new IsoMovieBox (h, parent);
         
         if (h.BoxType == "mvhd")
            return new IsoMovieHeaderBox (h, parent);
         
         if (h.BoxType == "mdia")
            return new IsoMediaBox (h, parent);
         
         if (h.BoxType == "minf")
            return new IsoMediaInformationBox (h, parent);
         
         if (h.BoxType == "stbl")
            return new IsoSampleTableBox (h, parent);
         
         if (h.BoxType == "stsd")
            return new IsoSampleDescriptionBox (h, parent);
         
         if (h.BoxType == "stco")
            return new IsoChunkOffsetBox (h, parent);
         
         if (h.BoxType == "co64")
            return new IsoChunkLargeOffsetBox (h, parent);
         
         if (h.BoxType == "trak")
            return new IsoTrackBox (h, parent);
         
         if (h.BoxType == "hdlr")
            return new IsoHandlerBox (h, parent);
         
         if (h.BoxType == "udta")
            return new IsoUserDataBox (h, parent);
         
         if (h.BoxType == "meta")
            return new IsoMetaBox (h, parent);
         
         if (h.BoxType == "ilst")
            return new AppleItemListBox (h, parent);
         
         if (h.BoxType == "data")
            return new AppleDataBox (h, parent);
         
         if (h.BoxType == "esds")
            return new AppleElementaryStreamDescriptor (h, parent);
         
         if (h.BoxType == "free" || h.BoxType == "skip")
            return new IsoFreeSpaceBox (h, parent);
         
         if (h.BoxType == "mean" || h.BoxType == "name")
            return new AppleAdditionalInfoBox (h, parent);
         
         // If we still don't have a tag, and we're inside an ItemLisBox, load
         // lthe box as an AnnotationBox (Apple tag item).
         if (parent.GetType () == typeof (AppleItemListBox))
            return new AppleAnnotationBox (h, parent);
         
         // Nothing good. Go generic.
         return new UnknownBox (h, parent);
      }
      
      // The data contained within this box.
      public virtual ByteVector Data
      {
         get
         {
            LoadData ();
            return data;
         }
         set
         {
            data = value;
         }
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // protected methods
      //////////////////////////////////////////////////////////////////////////
      
      // Render a box with the "data" before its content.
      protected virtual ByteVector Render (ByteVector data)
      {
         bool free_found = false;
         
         ByteVector output = new ByteVector ();
         
         // If we have children, render them if they aren't "free", otherwise
         // render the box's data.
         if (HasChildren)
            foreach (Box box in Children)
               if (box.GetType () == typeof (IsoFreeSpaceBox))
                  free_found = true;
               else
                  output.Add (box.Render ());
         else
            output.Add (Data);
         
         // If there was a free, don't take it away, and let meta be a special case.
         if (free_found || BoxType == "meta")
         {
            long size_difference = (long) DataSize - (long) output.Count;
            
            // If we have room for free space, add it so we don't have to resize the file.
            if (header.DataSize != 0 && size_difference >= 8)
               output. Add ((new IsoFreeSpaceBox ((ulong) size_difference, this)).Render ());
            // If we're getting bigger, get a lot bigger so we might not have to again.
            else
               output.Add ((new IsoFreeSpaceBox (2048, this)).Render ());
         }
         
         // Adjust the header's data size to match the content.
         header.DataSize = (ulong) (data.Count + output.Count);
         
         // Render the full box.
         output.Insert (0, data);
         output.Insert (0, header.Render ());
         return output;
      }
      
      
      //////////////////////////////////////////////////////////////////////////
      // private members
      //////////////////////////////////////////////////////////////////////////
      
      // If the file is readable and the box can have children and there is 
      // enough space to read the data, get the first child box by reading the
      // file.
      private Box FirstChild
      {
         get
         {
            if (HasChildren && this.File != null && (GetType () == typeof (FileBox) || DataSize >= 8))
               return Box.Create (File, DataPosition, this);
            return null;
         }
      }
      
      // If the file is readable and the box can have children and there is 
      // enough space to read the data, get the next box by reading from the end
      // of the first box.
      private Box NextChild (Box child)
      {
         if (HasChildren && this.File != null && child.NextBoxPosition >= DataPosition && child.NextBoxPosition < NextBoxPosition)
            return Box.Create (File, child.NextBoxPosition, this);
         return null;
      }
   }
}
