using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.Data
{
    internal class NodeHierarchyEnumerator : IEnumerator<Node>
    {
        private readonly Node[] _init;

        private readonly Stack<(IList<Node> children, int index)> _treeStack;

        private IList<Node>? _currentChildren;

        private int _currentIndex;

        public Node Current => _currentChildren?[_currentIndex] ?? _init[0];

        object IEnumerator.Current => Current;

        public NodeHierarchyEnumerator(ParentNode parent)
        {
            _init = new Node[] { parent };
            _currentChildren = _init;
            _treeStack = new();
        }

        public void Dispose()
        {
            _currentChildren = null;
        }

        public bool MoveNext()
        {
            if(_currentChildren == null)
            {
                return false;
            }

            if(_currentChildren[_currentIndex] is ParentNode parent && parent.ChildNodes.Count > 0)
            {
                _treeStack.Push((_currentChildren, _currentIndex + 1));
                _currentChildren = parent.ChildNodes;
                _currentIndex = -1;
            }

            _currentIndex++;

            while(_currentIndex >= _currentChildren.Count)
            {
                if(_treeStack.Count == 0)
                {
                    return false;
                }

                (_currentChildren, _currentIndex) = _treeStack.Pop();
            }

            return true;
        }

        public void Reset()
        {
            _currentChildren = _init;
            _currentIndex = 0;
            _treeStack.Clear();
        }
    }
}
