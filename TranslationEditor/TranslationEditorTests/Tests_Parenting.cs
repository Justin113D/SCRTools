using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;

namespace SCR.Tools.TranslationEditor.Tests
{
    [TestClass]
    public class Tests_Parenting
    {
        private static TestNode CreateNode()
            => new("Test", null);

        private static void Undo()
            => ChangeTracker.Global.Undo();

        private static void Redo()
            => ChangeTracker.Global.Redo();

        #region Setting in Node

        [TestMethod]
        public void Node_SetParent()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            node.SetParent(parent);

            Assert.AreEqual(node.Parent, parent, "Parent not set");
            Assert.IsTrue(parent.ChildNodes.Contains(node), "Child not part of parent");
        }

        [TestMethod]
        public void Node_SetParent_Null()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            node.SetParent(parent);
            node.SetParent(null);

            Assert.IsNull(node.Parent, "Parent still set");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");
        }

        [TestMethod]
        public void Node_SetParent_Swap()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");
            ParentNode parentTarget = new("ParentTarget");

            node.SetParent(parent);
            node.SetParent(parentTarget);

            Assert.AreEqual(node.Parent, parentTarget, "Parent not set");
            Assert.IsTrue(parentTarget.ChildNodes.Contains(node), "Child not part of parent");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");
        }

        [TestMethod]
        public void Node_SetParent_InvokeParentChangedEvent()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            ParentNode? previousParent = node.Parent;

            bool invokedEvent = false;
            node.ParentChanged += (n, e) =>
            {
                AssertParentEvent(n, e, node, parent, previousParent);
                invokedEvent = true;
            };

            node.SetParent(parent);

            Assert.IsTrue(invokedEvent, "Event not invoked");
        }

        [TestMethod]
        public void Node_SetParent_NoNewValue_DontInvokeParentChangedEvent()
        {
            TestNode node = CreateNode();

            bool invokedEvent = false;
            node.ParentChanged += (n, e) =>
            {
                invokedEvent = true;
            };

            node.SetParent(node.Parent);

            Assert.IsFalse(invokedEvent, "Event got invoked");
        }

        [TestMethod]
        public void Node_SetParent_Undo()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            node.SetParent(parent);
            Undo();

            Assert.IsNull(node.Parent, "Parent still set");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");
        }

        [TestMethod]
        public void Node_SetParent_Redo()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            node.SetParent(parent);
            Undo();
            Redo();

            Assert.AreEqual(node.Parent, parent, "Parent not set");
            Assert.IsTrue(parent.ChildNodes.Contains(node), "Child not part of parent");
        }

        [TestMethod]
        public void Node_EnsureTrackedChange()
        {
            TestNode node = CreateNode();

            var pin = ChangeTracker.Global.PinCurrent();
            node.SetParent(node.Parent);

            Assert.IsFalse(pin.CheckValid(), "Re-setting nodes parent to itself should track a blank change, which did not happen");
        }

        #endregion

        #region Adding to Parent 

        [TestMethod]
        public void ParentNode_AddChild_UndoRedo()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            parent.AddChildNode(node);

            Assert.AreEqual(node.Parent, parent, "Parent not set");
            Assert.IsTrue(parent.ChildNodes.Contains(node), "Child not part of parent");

            Undo();

            Assert.IsNull(node.Parent, "Parent still set");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");

            Redo();

            Assert.AreEqual(node.Parent, parent, "Parent not set");
            Assert.IsTrue(parent.ChildNodes.Contains(node), "Child not part of parent");
        }

        [TestMethod]
        public void ParentNode_AddChild_InvokeParentChangedEvent()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            ParentNode? previousParent = node.Parent;

            bool invokedEvent = false;
            node.ParentChanged += (n, e) =>
            {
                AssertParentEvent(n, e, node, parent, previousParent);
                invokedEvent = true;
            };

            parent.AddChildNode(node);

            Assert.IsTrue(invokedEvent, "Event not invoked");
        }

        [TestMethod]
        public void ParentNode_RemoveChild_UndoRedo()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");

            parent.AddChildNode(node);
            parent.RemoveChildNode(node);

            Assert.IsNull(node.Parent, "Parent still set");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");

            Undo();
            Assert.AreEqual(node.Parent, parent, "Parent not set");
            Assert.IsTrue(parent.ChildNodes.Contains(node), "Child not part of parent");

            Redo();
            Assert.IsNull(node.Parent, "Parent still set");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");
        }

        [TestMethod]
        public void ParentNode_AddChild_ChangeParent_UndoRedo()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");
            ParentNode parentTarget = new("ParentTarget");

            parent.AddChildNode(node);
            parentTarget.AddChildNode(node);

            Assert.AreEqual(node.Parent, parentTarget, "Parent not set");
            Assert.IsTrue(parentTarget.ChildNodes.Contains(node), "Child not part of parent");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");

            Undo();

            Assert.AreEqual(node.Parent, parent, "Parent not set");
            Assert.IsTrue(parent.ChildNodes.Contains(node), "Child not part of parent");
            Assert.IsFalse(parentTarget.ChildNodes.Contains(node), "Child still part of parent");

            Redo();

            Assert.AreEqual(node.Parent, parentTarget, "Parent not set");
            Assert.IsTrue(parentTarget.ChildNodes.Contains(node), "Child not part of parent");
            Assert.IsFalse(parent.ChildNodes.Contains(node), "Child still part of parent");
        }

        #endregion

        #region Index manipulation

        [TestMethod]
        public void ParentNode_InsertChild_UndoRedo()
        {
            ParentNode parent = new("Parent");
            TestNode node = CreateNode();
            TestNode node2 = CreateNode();

            parent.AddChildNode(node);
            parent.InsertChildNodeAt(node2, 0);

            Assert.AreEqual(node2, parent.ChildNodes[0]);
            Assert.AreEqual(node, parent.ChildNodes[1]);

            Undo();

            Assert.AreEqual(node, parent.ChildNodes[0]);
            Assert.IsNull(node2.Parent);

            Redo();

            Assert.AreEqual(node2, parent.ChildNodes[0]);
            Assert.AreEqual(node, parent.ChildNodes[1]);
        }

        #endregion

        #region State Changed event

        [TestMethod]
        public void ParentNode_InvokeStateChangedEvent()
        {
            TestNode node = CreateNode();
            ParentNode parent = new("Parent");
            node.SetParent(parent);

            bool calledEvent = false;
            parent.NodeStateChanged += (n, e) =>
            {
                Assert.AreEqual(n.State, node.State, "State incorrect");
                calledEvent = true;
            };

            node.SetState(NodeState.Translated);
            Assert.IsTrue(calledEvent, "Event not called");
        }

        [TestMethod]
        public void ParentNode_CheckStateUpdate()
        {
            ParentNode parent = new("Parent");
            ParentNode subParent = new("Subparent");
            TestNode node = CreateNode();
            TestNode node2 = CreateNode();

            parent.AddChildNode(subParent);
            Assert.AreEqual(parent.State, NodeState.None);
            Assert.AreEqual(subParent.State, NodeState.None);

            node.SetState(NodeState.Outdated);
            subParent.AddChildNode(node);
            Assert.AreEqual(parent.State, NodeState.Outdated, "Parent not updated");
            Assert.AreEqual(subParent.State, NodeState.Outdated, "Subparent not updated");

            node.SetState(NodeState.Translated);
            Assert.AreEqual(parent.State, NodeState.Translated, "Parent not updated");
            Assert.AreEqual(subParent.State, NodeState.Translated, "Subparent not updated");

            node2.SetState(NodeState.Untranslated);
            subParent.AddChildNode(node2);
            Assert.AreEqual(parent.State, NodeState.Untranslated, "Parent not updated");
            Assert.AreEqual(subParent.State, NodeState.Untranslated, "Subparent not updated");

            subParent.RemoveChildNode(node2);
            Assert.AreEqual(parent.State, NodeState.Translated);
            Assert.AreEqual(subParent.State, NodeState.Translated);

            subParent.RemoveChildNode(node);
            Assert.AreEqual(parent.State, NodeState.None);
            Assert.AreEqual(subParent.State, NodeState.None);
        }

        #endregion

        private static void AssertParentEvent(
            Node node,
            NodeParentChangedEventArgs args,
            Node assertNode,
            ParentNode? newParent,
            ParentNode? oldparent)
        {
            Assert.AreEqual(node, assertNode, "Node in event not invoked node");
            Assert.AreEqual(newParent, node.Parent, "New parent in event args not node parent");
            Assert.AreEqual(args.NewParent, newParent, "New parents do not match");
            Assert.AreEqual(args.OldParent, oldparent, "Old parents do not match");
        }
    }
}