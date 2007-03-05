/***************************************************************************
 *  MonodocNodeConfig.cs
 *
 *  Copyright (C) 2005 Novell
 *  Written by Aaron Bockover (aaron@aaronbock.net)
 ****************************************************************************/

/*  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW: 
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),  
 *  to deal in the Software without restriction, including without limitation  
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,  
 *  and/or sell copies of the Software, and to permit persons to whom the  
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 *  DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Xml;

public static class MonodocNodeConfig
{
    private static XmlDocument document;

    public static int Main(string [] args)
    {
        bool insert_mode = false;
        string xml_file = null;
        string label = null;
        string name = null;
        
        switch(args[0]) {
            case "--insert":
                if(args.Length != 4) {
                    PrintUsage();
                    return 0;
                }
                
                insert_mode = true;
                label = args[1];
                name = args[2];
                xml_file = args[3];
                break;
            case "--remove":
                if(args.Length != 3) {
                    PrintUsage();
                    return 0;
                }
                
                insert_mode = false;
                name = args[1];
                xml_file = args[2];
                break;
            default:
                PrintUsage();
                return 0;
        }
        
        document = new XmlDocument();
        document.Load(xml_file);
        
        RemoveNode(name);
            
        if(insert_mode) {
            InsertNode(label, name);
        }
        
        try {
            document.Save(xml_file);
        } catch {
			Console.WriteLine("Couldn\'t insert node!");
			return 1;
        }
		return 0;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage is:");
        Console.WriteLine("MonodocNodeConfig.exe --insert|--remove [options] <monodoc XML file>\n");
        Console.WriteLine("Where [options] for --insert must be:");
        Console.WriteLine("  <label> <name>\n");
        Console.WriteLine("Where [options] for --remove must be:");
        Console.WriteLine("  <name>\n");
    }
    
    private static void RemoveNode(string name)
    {
        foreach(XmlNode node in document.DocumentElement) {
            if(node.Attributes["name"].Value != "various") {
                continue;
            }
            
            foreach(XmlNode child in node) {
                if(child.Attributes["name"].Value == name) {
                    node.RemoveChild(child);
                    return;
                }
            }
        }
    }
    
    private static void InsertNode(string label, string name)
    {
        foreach(XmlNode node in document.DocumentElement) {
            if(node.Attributes["name"].Value != "various") {
                continue;
            }
            
            XmlNode insert_node = document.CreateNode(XmlNodeType.Element, "node", String.Empty);
            
            XmlAttribute label_attr = document.CreateAttribute("label");
            label_attr.Value = label;
            insert_node.Attributes.Append(label_attr);
            
            XmlAttribute name_attr = document.CreateAttribute("name");
            name_attr.Value = name;
            insert_node.Attributes.Append(name_attr);
            
            node.AppendChild(insert_node);
            
            return;
        }
    }
}
