using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCR.Tools.UndoRedo;
using System;

namespace SCR.Tools.TranslationEditor.Data.Tests
{
    [TestClass]
    public class Tests_SharedNodeFunctionality
    {
        private static TestNode CreateNode()
            => new("Test", null);

        private static void Undo()
            => ChangeTracker.Global.Undo();

        private static void Redo()
            => ChangeTracker.Global.Redo();

        #region Constructor

        [TestMethod]
        public void Constructor_EnsureNoChangeTrack()
        {
            var pin = ChangeTracker.Global.PinCurrent();
            TestNode node = new("Test", "Test Description");
            Assert.IsTrue(pin.CheckValid(), "A change was tracked where no change should be tracked");
        }

        [TestMethod]
        public void Constructor_Name()
        {
            const string name = "Test";
            TestNode node = new(name, null);
            Assert.AreEqual(node.Name, name, "Name not set");
        }

        [TestMethod]
        public void Constructor_Name_Trimmed()
        {
            const string name = " Test ";
            string nameTrimmed = name.Trim();
            TestNode node = new(name, null);
            Assert.AreEqual(node.Name, nameTrimmed, "Name not trimmed");
        }

        [TestMethod]
        public void Constructor_Description_Null()
        {
            TestNode node = new("Test", null);
            Assert.IsNull(node.Description, "Description was not set null");
        }

        [TestMethod]
        public void Constructor_Description_Whitespace()
        {
            TestNode node = new("Test", " ");
            Assert.IsNull(node.Description, "Description was converted to null");
        }

        [TestMethod]
        public void Constructor_Description_Trimmed()
        {
            const string TestDescription = " Test Description ";
            string TestDescriptionTrimmed = TestDescription.Trim();

            TestNode node = new("Test", TestDescription);
            Assert.AreEqual(node.Description, TestDescriptionTrimmed, "Description not trimmed");
        }

        #endregion

        #region Name Property

        [TestMethod]
        public void Name_Set()
        {
            TestNode node = CreateNode();
            node.Name = "Test2";
            Assert.AreEqual(node.Name, "Test2", "Setting failed");
        }
        
        [TestMethod]
        public void Name_Trimming()
        {
            const string TestName = " Test2 ";
            string TestNameTrimmed = TestName.Trim();

            TestNode node = CreateNode();
            node.Name = TestName;
            Assert.AreEqual(node.Name, TestNameTrimmed, "Trimming failed");
        }

        [TestMethod]
        public void Name_InvokeChangedEvent()
        {
            TestNode node = CreateNode();

            string oldName = node.Name;
            string newName = oldName + "_";

            bool nameChanged = false;
            node.NameChanged += (o, args) =>
            {
                nameChanged = true;
                Assert.AreEqual(o.Name, newName);
                Assert.AreEqual(args.OldName, oldName);
                Assert.AreEqual(args.NewName, newName);
            };

            node.Name = newName;

            Assert.IsTrue(nameChanged, "Event not called");
        }

        [TestMethod]
        public void Name_NoNewValue_DontInvokeChangedEvent()
        {
            TestNode node = CreateNode();

            bool nameChanged = false;
            node.NameChanged += (o, args) =>
                nameChanged = true;

            node.Name = node.Name;

            Assert.IsFalse(nameChanged, "Event called despite no actual change occuring");
        }

        [TestMethod]
        public void Name_WhiteSpace_Exception()
        {
            TestNode node = CreateNode();

            try
            {
                node.Name = " ";
            }
            catch (ArgumentException)
            {
                return;
            }


            Assert.Fail("Exception wasnt thrown");
        }

        [TestMethod]
        public void Name_Undo()
        {
            TestNode node = CreateNode();
            string previousName = node.Name;
            node.Name += "_";
            Undo();

            Assert.AreEqual(node.Name, previousName, "Undo failed");
        }

        [TestMethod]
        public void Name_Redo()
        {
            TestNode node = CreateNode();
            string newName = node.Name + "_";
            node.Name = newName;
            Undo();
            Redo();

            Assert.AreEqual(node.Name, newName, "Redo failed");
        }

        [TestMethod]
        public void Name_EnsureTrackedChanged()
        {
            TestNode node = CreateNode();

            var pin = ChangeTracker.Global.PinCurrent();
            node.Name = node.Name;

            Assert.IsFalse(pin.CheckValid(), "Re-setting name to itself should track a blank change, which did not happen");
        }

        #endregion

        #region Description Property

        [TestMethod]
        public void Description_Set()
        {
            const string TestDescription = "Test Description";
            TestNode node = CreateNode();
            node.Description = TestDescription;
            Assert.AreEqual(node.Description, TestDescription, "Value not set");
        }

        [TestMethod]
        public void Description_Trimming()
        {
            const string TestDescription = " Test Description ";
            string TestDescriptionTrimmed = TestDescription.Trim();

            TestNode node = CreateNode();
            node.Description = TestDescription;
            Assert.AreEqual(node.Description, TestDescriptionTrimmed, "Value not trimmed");
        }

        [TestMethod]
        public void Description_Whitespace()
        {
            TestNode node = CreateNode();
            node.Description = " ";
            Assert.IsNull(node.Description, "Whitespace not converted to null");
        }

        [TestMethod]
        public void Description_Undo()
        {
            TestNode node = CreateNode();

            string? previous = node.Description;
            node.Description += "_";
            Undo();

            Assert.AreEqual(node.Description, previous, "Undo failed");
        }

        [TestMethod]
        public void Description_Redo()
        {
            const string TestDescription = "Test Description";
            TestNode node = CreateNode();

            node.Description = TestDescription;
            Undo();
            Redo();

            Assert.AreEqual(node.Description, TestDescription, "Redo failed");
        }

        [TestMethod]
        public void Description_EnsureTrackedChanged()
        {
            TestNode node = CreateNode();

            var pin = ChangeTracker.Global.PinCurrent();
            node.Description = node.Description;

            Assert.IsFalse(pin.CheckValid(), "Re-setting description to itself should track a blank change, which did not happen");
        }

        #endregion

        #region State Property

        [TestMethod]
        public void State_Set()
        {
            const NodeState newState = NodeState.Translated;
            TestNode node = CreateNode();

            node.SetState(newState);

            Assert.AreEqual(node.State, newState, "Setting failed");
        }

        [TestMethod]
        public void State_Undo()
        {
            TestNode node = CreateNode();
            NodeState previousState = node.State;

            node.SetState(NodeState.Translated);
            Undo();

            Assert.AreEqual(node.State, previousState, "Undo failed");
        }

        [TestMethod]
        public void State_Redo()
        {
            const NodeState newState = NodeState.Translated;

            TestNode node = CreateNode();

            node.SetState(newState);
            Undo();
            Redo();

            Assert.AreEqual(node.State, newState, "Redo failed");
        }

        [TestMethod]
        public void State_InvokeChangedEvent()
        {
            const NodeState newState = NodeState.Translated;
            TestNode node = CreateNode();
            NodeState previousState = node.State;

            bool eventInvoked = false;
            node.NodeStateChanged += (n, e) =>
            {
                Assert.AreEqual(n, node, "Node in event not invoked node");
                Assert.AreEqual(newState, node.State, "New state in event args not node state");
                Assert.AreEqual(e.NewState, newState, "New state in event args not passed state");
                Assert.AreEqual(e.OldState, previousState, "Old states do not match");
                eventInvoked = true;
            };
            node.SetState(newState);

            Assert.IsTrue(eventInvoked, "Event not invoked");
        }

        [TestMethod]
        public void State_NoNewValue_DontInvokeChangedEvent()
        {
            TestNode node = CreateNode();

            bool eventInvoked = false;
            node.NodeStateChanged += (n, e) => eventInvoked = true;

            node.SetState(node.State);

            Assert.IsFalse(eventInvoked, "Event invoked");
        }

        [TestMethod]
        public void State_EnsureTrackedChange()
        {
            TestNode node = CreateNode();

            var pin = ChangeTracker.Global.PinCurrent();
            node.SetState(node.State);

            Assert.IsFalse(pin.CheckValid(), "Re-setting state to itself should track a blank change, which did not happen");
        }

        #endregion
    }
}
