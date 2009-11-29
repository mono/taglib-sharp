//
// XmpNode.cs:
//
// Author:
//   Ruben Vermeersch (ruben@savanne.be)
//
// Copyright (C) 2009 Ruben Vermeersch
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;
using System.Collections.Generic;
using System.Xml;

namespace TagLib.Xmp
{
	public class XmpNode
	{
		private List<XmpNode> children;
		private Dictionary<string, Dictionary<string, XmpNode>> qualifiers;
		string name = String.Empty;

		public string Namespace { get; private set; }
		public string Name {
			get { return name; }
			internal set {
				if (name != String.Empty)
					throw new Exception ("Cannot change named node");
				name = value;
			}
		}
		public string Value { get; set; }
		public XmpNodeType Type { get; internal set; }

		public int QualifierCount {
			get {
				if (qualifiers == null)
					return 0;
				int count = 0;
				foreach (var collection in qualifiers.Values) {
					count += collection == null ? 0 : collection.Count;
				}
				return count;
			}
		}

		public XmpNode (string ns, string name)
		{
			Namespace = ns;
			Name = name;
			Type = XmpNodeType.Simple;
			Value = String.Empty;
		}

		public XmpNode (string ns, string name, string value) : this (ns, name)
		{
			Value = value;
		}

		public List<XmpNode> Children {
			get { return children ?? new List<XmpNode> (); }
		}

		public void AddChild (XmpNode node)
		{
			if (children == null)
				children = new List<XmpNode> ();
			children.Add (node);
		}

		public void AddQualifier (XmpNode node)
		{
			if (qualifiers == null)
				qualifiers = new Dictionary<string, Dictionary<string, XmpNode>> ();
			if (!qualifiers.ContainsKey (node.Namespace))
				qualifiers [node.Namespace] = new Dictionary<string, XmpNode> ();
			qualifiers [node.Namespace][node.Name] = node;
		}

		public XmpNode GetQualifier (string ns, string name)
		{
			if (qualifiers == null)
				return null;
			if (!qualifiers.ContainsKey (ns))
				return null;
			if (!qualifiers [ns].ContainsKey (name))
				return null;
			return qualifiers [ns][name];
		}

		/// <summary>
		///    Print a debug output of the node.
		/// </summary>
		public void Dump ()
		{
			Dump ("");
		}

		internal void Dump (string prefix) {
			Console.WriteLine ("{0}{1}{2} ({4}) = \"{3}\"", prefix, Namespace, Name, Value, Type);
			if (qualifiers != null) {
				Console.WriteLine ("{0}Qualifiers:", prefix);

				foreach (string ns in qualifiers.Keys) {
					foreach (string name in qualifiers [ns].Keys) {
						qualifiers [ns][name].Dump (prefix+"  ->  ");
					}
				}
			}
			if (children != null) {
				Console.WriteLine ("{0}Children:", prefix);

				foreach (XmpNode child in children) {
					child.Dump (prefix+"  ->  ");
				}
			}
		}

		public void Accept (XmpNodeVisitor visitor)
		{
			visitor.Visit (this);
			if (children != null) {
				foreach (XmpNode child in children) {
					child.Accept (visitor);
				}
			}
		}

		/// <summary>
		///    Is this a node that we can transform into an attribute of the
		///    parent node? Yes if it has no qualifiers or children, nor is
		///    it part of a list.
		/// </summary>
		private bool IsReallySimpleType {
			get {
				return Type == XmpNodeType.Simple && (children == null || children.Count == 0)
					&& QualifierCount == 0 && (Name != XmpTag.LI_URI || Namespace != XmpTag.RDF_NS);
			}
		}

		/// <summary>
		///    Is this the root node of the tree?
		/// </summary>
		private bool IsRootNode {
			get { return Name == String.Empty && Namespace == String.Empty; }
		}

		public void RenderInto (XmlNode parent)
		{
			if (IsRootNode) {
				AddAllChildrenTo (parent);

			} else if (IsReallySimpleType && parent.Attributes.GetNamedItem (XmpTag.PARSE_TYPE_URI, XmpTag.RDF_NS) == null) {
				// Simple values can be added as attributes of the parent node. Not allowed when the parent has an rdf:parseType.
				XmlAttribute attr = XmpTag.CreateAttribute (parent.OwnerDocument, Name, Namespace);
				attr.Value = Value;
				parent.Attributes.Append (attr);

			} else if (Type == XmpNodeType.Simple || Type == XmpNodeType.Struct) {
				var node = XmpTag.CreateNode (parent.OwnerDocument, Name, Namespace);
				node.InnerText = Value;

				if (Type == XmpNodeType.Struct) {
					// Structured types are always handled as a parseType=Resource node. This way, IsReallySimpleType will
					// not match for child nodes, which makes sure they are added as extra nodes to this node. Does the
					// trick well, unit tests that prove this are in XmpSpecTest.
					XmlAttribute attr = XmpTag.CreateAttribute (parent.OwnerDocument, XmpTag.PARSE_TYPE_URI, XmpTag.RDF_NS);
					attr.Value = "Resource";
					node.Attributes.Append (attr);
				}

				AddAllQualifiersTo (node);
				AddAllChildrenTo (node);
				parent.AppendChild (node);

			} else if (Type == XmpNodeType.Bag) {
				var node = XmpTag.CreateNode (parent.OwnerDocument, Name, Namespace);
				// TODO: Add all qualifiers.
				if (QualifierCount > 0)
					throw new NotImplementedException ();
				var bag = XmpTag.CreateNode (parent.OwnerDocument, XmpTag.BAG_URI, XmpTag.RDF_NS);
				foreach (var child in children)
					child.RenderInto (bag);
				node.AppendChild (bag);
				parent.AppendChild (node);

			} else if (Type == XmpNodeType.Alt) {
				var node = XmpTag.CreateNode (parent.OwnerDocument, Name, Namespace);
				// TODO: Add all qualifiers.
				if (QualifierCount > 0)
					throw new NotImplementedException ();
				var bag = XmpTag.CreateNode (parent.OwnerDocument, XmpTag.ALT_URI, XmpTag.RDF_NS);
				foreach (var child in children)
					child.RenderInto (bag);
				node.AppendChild (bag);
				parent.AppendChild (node);

			} else {
				// Probably some combination of things we don't fully cover yet.
				Dump ();
				throw new NotImplementedException ();
			}
		}

		private void AddAllQualifiersTo (XmlNode xml)
		{
			if (qualifiers == null)
				return;
			foreach (var collection in qualifiers.Values) {
				foreach (XmpNode node in collection.Values) {
					XmlAttribute attr = XmpTag.CreateAttribute (xml.OwnerDocument, node.Name, node.Namespace);
					attr.Value = node.Value;
					xml.Attributes.Append (attr);
				}
			}
		}

		private void AddAllChildrenTo (XmlNode parent)
		{
			if (children == null)
				return;
			foreach (var child in children)
				child.RenderInto (parent);
		}
	}
}
