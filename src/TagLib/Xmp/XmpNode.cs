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
	}
}
