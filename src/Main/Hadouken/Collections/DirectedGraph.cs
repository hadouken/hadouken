﻿//﻿
// Copyright (c) 2012 Patrik Svensson
//
// This file is part of Anchor.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Collections
{
    internal class DirectedGraph<T>
    where T : class
    {
        private readonly List<T> _nodes;
        private readonly List<DirectedGraphEdge<T>> _edges;

        public IReadOnlyCollection<T> Nodes
        {
            get { return _nodes; }
        }

        public IReadOnlyCollection<DirectedGraphEdge<T>> Edges
        {
            get { return _edges; }
        }

        public DirectedGraph()
        {
            _nodes = new List<T>();
            _edges = new List<DirectedGraphEdge<T>>();
        }

        public void Add(T node)
        {
            if (!this.Contains(node))
            {
                _nodes.Add(node);
            }
        }

        public void Remove(T node)
        {
            if (this.Contains(node))
            {
                // Remove the processorNode.
                _nodes.Remove(node);

                // Remove all edges where this processorNode exist.
                _edges.RemoveAll(x => x.Start.Equals(node) || x.End.Equals(node));
            }
        }

        public bool Contains(T node)
        {
            return _nodes.Contains(node);
        }

        public void Connect(T start, T end)
        {
            // Don't allow reflexive connections.
            if (start.Equals(end))
            {
                const string message = "Cannot create a reflexive connection.";
                throw new InvalidOperationException(message);
            }

            // Don't allow unidirectional connections.
            if (this.IsConnected(end, start))
            {
                const string message = "Cannot create a unidirectional connection.";
                throw new InvalidOperationException(message);
            }

            if (!this.IsConnected(start, end))
            {
                if (!this.Contains(start))
                {
                    this.Add(start);
                }
                if (!this.Contains(end))
                {
                    this.Add(end);
                }
                _edges.Add(new DirectedGraphEdge<T>(start, end));
            }
        }

        public void Disconnect(T start, T end)
        {
            if (this.IsConnected(start, end))
            {
                var edge = _edges.FirstOrDefault(x => x.Start.Equals(start) && x.End.Equals(end));
                if (edge != null)
                {
                    _edges.Remove(edge);
                }
            }
        }

        public bool IsConnected(T start, T end)
        {
            return _edges.Any(x => x.Start.Equals(start) && x.End.Equals(end));
        }

        public TNode FindNode<TNode>(Func<TNode, bool> func)
        where TNode : class,T
        {
            return this.Nodes.OfType<TNode>().FirstOrDefault(func);
        }

        public IEnumerable<TNode> FindNodes<TNode>(Func<TNode, bool> func)
        where TNode : class, T
        {
            return this.Nodes.OfType<TNode>().Where(func);
        }

        public int GetDegree(T node)
        {
            return _edges.Count(x => x.Start.Equals(node));
        }

        public int GetIndegree(T node)
        {
            return _edges.Count(x => x.End.Equals(node));
        }

        public IEnumerable<T> GetOutgoingNodes(T node)
        {
            return _edges.Where(x => x.Start.Equals(node))
            .Select(x => x.End);
        }

        public IEnumerable<T> GetIncomingNodes(T node)
        {
            return _edges.Where(x => x.End.Equals(node))
            .Select(x => x.Start);
        }

        public bool IsCircular(T root)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();
            return this.IsCircular(root, sorted, visited);
        }

        private bool IsCircular(T node, List<T> sorted, HashSet<T> visited)
        {
            if (!visited.Contains(node))
            {
                visited.Add(node);
                foreach (var child in this.GetOutgoingNodes(node))
                {
                    if (this.IsCircular(child, sorted, visited))
                    {
                        return true;
                    }
                }
                sorted.Add(node);
            }
            else if (!sorted.Contains(node))
            {
                return true;
            }
            return false;
        }

        public List<T> TraverseReverseOrder(T root)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();
            this.TraverseReverseOrder(root, sorted, visited);
            return sorted;
        }

        public List<T> TraverseOrder(T root)
        {
            var sorted = new List<T>();
            var visited = new HashSet<T>();
            this.TraverseOrder(root, sorted, visited);
            return sorted;
        }

        private bool TraverseReverseOrder(T node, List<T> sorted, HashSet<T> visited)
        {
            if (!visited.Contains(node))
            {
                visited.Add(node);
                foreach (var child in this.GetOutgoingNodes(node))
                {
                    if (this.TraverseReverseOrder(child, sorted, visited))
                    {
                        return true;
                    }
                }
                sorted.Add(node);
            }
            else if (!sorted.Contains(node))
            {
                return true;
            }
            return false;
        }

        private bool TraverseOrder(T node, List<T> sorted, HashSet<T> visited)
        {
            if (!visited.Contains(node))
            {
                visited.Add(node);
                foreach (var child in this.GetIncomingNodes(node))
                {
                    if (this.TraverseOrder(child, sorted, visited))
                    {
                        return true;
                    }
                }
                sorted.Add(node);
            }
            else if (!sorted.Contains(node))
            {
                return true;
            }
            return false;
        }

        public void DisconnectAll()
        {
            _edges.Clear();
        }
    }
}