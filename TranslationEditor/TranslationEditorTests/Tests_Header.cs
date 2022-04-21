using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.Tests
{
    [TestClass]
    public class Tests_Header
    {
        private static void Undo()
            => ChangeTracker.Global.Undo();

        private static void Redo()
            => ChangeTracker.Global.Redo();

        private static HeaderNode TestFormat()
        {
            HeaderNode header = new();

            // single node
            StringNode single = new("Single", "Test");

            // under parent
            ParentNode parent = new("Parent");
            StringNode childNode1 = new("ChildNode1", "Test");
            StringNode childNode2 = new("ChildNode2", "Test");

            // parenting this to the already added parent
            ParentNode subParent = new("Subparent");
            StringNode subchildNode1 = new("SubChildNode1", "Test");
            StringNode subchildNode2 = new("SubChildNode2", "Test");

            parent.AddChildNode(childNode1);
            parent.AddChildNode(childNode2);

            subParent.AddChildNode(subchildNode1);
            subParent.AddChildNode(subchildNode2);

            header.AddChildNode(single);
            header.AddChildNode(parent);
            parent.AddChildNode(subParent);

            return header;
        }

        [TestMethod]
        public void Header_StringNodeParenting()
        {
            HeaderNode header = new();

            // single node
            StringNode single = new("Single", "Test");

            // under parent
            ParentNode parent = new("Parent");
            StringNode childNode1 = new("ChildNode1", "Test");
            StringNode childNode2 = new("ChildNode2", "Test");

            // parenting this to the already added parent
            ParentNode subParent = new("Subparent");
            StringNode subchildNode1 = new("SubChildNode1", "Test");
            StringNode subchildNode2 = new("SubChildNode2", "Test");

            parent.AddChildNode(childNode1);
            parent.AddChildNode(childNode2);

            subParent.AddChildNode(subchildNode1);
            subParent.AddChildNode(subchildNode2);

            header.AddChildNode(single);
            header.AddChildNode(parent);
            parent.AddChildNode(subParent);

            // check if all nodes exist
            Assert.AreEqual(single, header[single.Name]);
            Assert.AreEqual(childNode1, header[childNode1.Name]);
            Assert.AreEqual(childNode2, header[childNode2.Name]);
            Assert.AreEqual(subchildNode1, header[subchildNode1.Name]);
            Assert.AreEqual(subchildNode2, header[subchildNode2.Name]);

            header.RemoveChildNode(parent);

            Assert.IsFalse(header.TryGetStringNode(childNode1.Name));
            Assert.IsFalse(header.TryGetStringNode(childNode2.Name));
            Assert.IsFalse(header.TryGetStringNode(subchildNode1.Name));
            Assert.IsFalse(header.TryGetStringNode(subchildNode2.Name));
        }

        [TestMethod]
        public void Header_StringNodeNameAltering()
        {
            const string Name = "String";
            const string NameNumbered = Name + ".001";
            const string NameNumbered2 = Name + ".002";
            const string NameLower = "string";
            const string NameLowerNumbered = NameLower + ".003";
            const string AltName = "AltString";
            const string AltNameNumbered = AltName + ".001";
            const string AltNameNumbered2 = AltName + ".002";

            HeaderNode header = new();

            StringNode stringNode = new(Name, "Test");
            StringNode stringNode2 = new(Name, "Test");
            StringNode stringNode3 = new(NameNumbered, "Test");
            StringNode stringNode4 = new(NameLower, "Test");

            StringNode altStringNode = new(AltName, "Test");
            StringNode altStringNode2 = new(AltName, "Test");
            StringNode altStringNode3 = new(AltName, "Test");

            header.AddChildNode(stringNode); // regular adding
            header.AddChildNode(stringNode2); // adding .001
            header.AddChildNode(stringNode3); // counting .001 up to .002
            header.AddChildNode(stringNode4); // even though its lower case, it should have a .003 added

            header.AddChildNode(altStringNode);
            altStringNode.Name = AltNameNumbered; // renaming an already added node

            header.AddChildNode(altStringNode2); // adding node of previous existing name
            header.AddChildNode(altStringNode3); // adding .002


            Assert.AreEqual(stringNode, header[Name]);
            Assert.AreEqual(stringNode2, header[NameNumbered]);
            Assert.AreEqual(stringNode3, header[NameNumbered2]);
            Assert.AreEqual(stringNode4, header[NameLowerNumbered]);
            Assert.AreEqual(stringNode.Name, Name);
            Assert.AreEqual(stringNode2.Name, NameNumbered);
            Assert.AreEqual(stringNode3.Name, NameNumbered2);
            Assert.AreEqual(stringNode4.Name, NameLowerNumbered);

            Assert.AreEqual(altStringNode, header[AltNameNumbered]);
            Assert.AreEqual(altStringNode2, header[AltName]);
            Assert.AreEqual(altStringNode3, header[AltNameNumbered2]);
            Assert.AreEqual(altStringNode.Name, AltNameNumbered);
            Assert.AreEqual(altStringNode2.Name, AltName);
            Assert.AreEqual(altStringNode3.Name, AltNameNumbered2);
        }

        [TestMethod]
        public void Header_StringNodeParenting_UndoRedo()
        {
            const string ChildNodeName = "ChildNode";
            const string ChildNodeNameNumbered = ChildNodeName + ".001";
            const string SubChildNodeName = "SubChildNode";
            const string SubChildNodeNameNumbered = SubChildNodeName + ".001";

            HeaderNode header = new();

            // single node
            StringNode single = new("Single", "Test");

            // under parent
            ParentNode parent = new("Parent");
            StringNode childNode1 = new(ChildNodeName, "Test");
            StringNode childNode2 = new(ChildNodeName, "Test");

            // parenting this to the already added parent
            ParentNode subParent = new("Subparent");
            StringNode subchildNode1 = new(SubChildNodeName, "Test");
            StringNode subchildNode2 = new(SubChildNodeName, "Test");

            parent.AddChildNode(childNode1);
            parent.AddChildNode(childNode2);

            subParent.AddChildNode(subchildNode1);
            subParent.AddChildNode(subchildNode2);

            header.AddChildNode(single);
            header.AddChildNode(parent);
            parent.AddChildNode(subParent);

            ChangeTracker.Global.Undo();
            Assert.IsFalse(header.TryGetStringNode(subchildNode1.Name));
            Assert.IsFalse(header.TryGetStringNode(subchildNode2.Name));
            Assert.AreEqual(subchildNode1.Name, SubChildNodeName);
            Assert.AreEqual(subchildNode2.Name, SubChildNodeName);

            ChangeTracker.Global.Undo();
            Assert.IsFalse(header.TryGetStringNode(childNode1.Name));
            Assert.IsFalse(header.TryGetStringNode(childNode2.Name));
            Assert.AreEqual(childNode1.Name, ChildNodeName);
            Assert.AreEqual(childNode2.Name, ChildNodeName);

            ChangeTracker.Global.Undo();
            Assert.IsFalse(header.TryGetStringNode(single.Name));

            ChangeTracker.Global.Redo();
            Assert.AreEqual(single, header[single.Name]);

            ChangeTracker.Global.Redo();
            Assert.AreEqual(childNode1, header[childNode1.Name]);
            Assert.AreEqual(childNode2, header[childNode2.Name]);
            Assert.AreEqual(childNode1.Name, ChildNodeName);
            Assert.AreEqual(childNode2.Name, ChildNodeNameNumbered);

            ChangeTracker.Global.Redo();
            Assert.AreEqual(subchildNode1, header[subchildNode1.Name]);
            Assert.AreEqual(subchildNode2, header[subchildNode2.Name]);
            Assert.AreEqual(subchildNode1.Name, SubChildNodeName);
            Assert.AreEqual(subchildNode2.Name, SubChildNodeNameNumbered);
        }

        [TestMethod]
        public void Header_StringNode_ChangeHeader()
        {
            HeaderNode header = new();
            HeaderNode otherHeader = new();

            StringNode node = new("Test", "test");

            node.SetParent(header);
            node.SetParent(otherHeader);

            Assert.IsFalse(header.TryGetStringNode(node.Name));
            Assert.AreEqual(node, otherHeader[node.Name]);
        }

        [TestMethod]
        public void Header_StringNode_ChangeHeader_UndoRedo()
        {
            HeaderNode header = new();
            HeaderNode otherHeader = new();

            StringNode node = new("Test", "test");

            node.SetParent(header);
            node.SetParent(otherHeader);

            Undo();

            Assert.IsFalse(otherHeader.TryGetStringNode(node.Name));
            Assert.AreEqual(node, header[node.Name]);

            Redo();

            Assert.IsFalse(header.TryGetStringNode(node.Name));
            Assert.AreEqual(node, otherHeader[node.Name]);
        }

        [TestMethod]
        public void Header_Version_Set()
        {
            Version OneZero = new(1, 0);
            Version ZeroFive = new(0, 5);
            Version TwoZero = new(2, 0);
            Version ThreeZero = new(3, 0);
            Version FourZero = new(4, 0);

            HeaderNode header = new();
            StringNode version0 = new("String", "test");
            StringNode version0Again = new("String", "test");
            StringNode version1 = new("String", "test");
            StringNode version2 = new("String", "test");

            header.Version = OneZero;
            header.AddChildNode(version0);

            header.Version = ZeroFive;
            header.AddChildNode(version0Again);

            header.Version = TwoZero;
            header.AddChildNode(version1);

            header.Version = ThreeZero;

            header.Version = FourZero;
            header.AddChildNode(version2);

            Assert.AreEqual(header.Versions.Count, 3);
            Assert.AreEqual(version0.VersionIndex, 0);
            Assert.AreEqual(version0Again.VersionIndex, 0);
            Assert.AreEqual(version1.VersionIndex, 1);
            Assert.AreEqual(version2.VersionIndex, 2);

            Assert.AreEqual(header.Versions[0], OneZero);
            Assert.AreEqual(header.Versions[1], TwoZero);
            Assert.AreEqual(header.Versions[2], FourZero);
        }

        [TestMethod]
        public void Header_JsonConversion()
        {
            HeaderNode header = TestFormat();

            string format = header.WriteFormat();

            HeaderNode readHeader = JsonFormatHandler.ReadFormat(format);

            Assert.AreEqual(readHeader.Language, header.Language);
            Assert.AreEqual(readHeader.Author, header.Author);
            Assert.AreEqual(readHeader.Name, header.Name);

            Assert.AreEqual(readHeader.StringNodes.Count, header.StringNodes.Count);
        }

        [TestMethod]
        public void Header_Project_CompileLoad()
        {
            HeaderNode header = TestFormat();
            StringNode node1 = header["single"];
            StringNode node2 = header["childnode1"];
            StringNode node3 = header["subchildnode1"];

            node1.NodeValue += "+";
            node2.NodeValue += "\n\t+";
            node3.KeepDefault = true;

            string project = header.CompileProject();

            HeaderNode loadHeader = TestFormat();
            loadHeader.LoadProject(project);

            Assert.AreEqual(loadHeader[node1.Name].NodeValue, node1.NodeValue);
            Assert.AreEqual(loadHeader[node2.Name].NodeValue, node2.NodeValue);
            Assert.AreEqual(loadHeader[node3.Name].NodeValue, node3.NodeValue);
        }

        [TestMethod]
        public void Header_Project_ImportExport()
        {
            HeaderNode header = TestFormat();
            StringNode node1 = header["single"];
            StringNode node2 = header["childnode1"];
            StringNode node3 = header["subchildnode1"];

            node1.NodeValue += "+";
            node2.NodeValue += "\n\t+";

            (string keys, string values) = header.ExportLanguageData();

            HeaderNode loadHeader = TestFormat();
            loadHeader.ImportLanguageData(keys, values);

            Assert.AreEqual(loadHeader[node1.Name].NodeValue, node1.NodeValue);
            Assert.AreEqual(loadHeader[node2.Name].NodeValue, node2.NodeValue);
            Assert.AreEqual(loadHeader[node3.Name].NodeValue, node3.NodeValue);
        }
    }
}
