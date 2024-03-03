using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

public class AstarNode {
    public Node Node;
    public float PathLengthFromStart { get; set; }
    public AstarNode CameFrom { get; set; }
    public float HeuristicEstimatePathLength { get; set; }

    public float EstimateFullPathLength {
        get {
            return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
        }
    }

    public AstarNode(Node node, AstarNode cameFrom, float pathLengthFromStart, float heuristicEstimatePathLength) {
        Node = node;
        CameFrom = cameFrom;
        PathLengthFromStart = pathLengthFromStart;
        HeuristicEstimatePathLength = heuristicEstimatePathLength;
    }
}

[Serializable]
public class Node {
    public Vector2 Position;
    public bool IsFree;
    public List<Edge> Edges = new List<Edge>();

    public Node(Vector2 position) {
        Position = position;
        IsFree = true;
    }

    public IEnumerable<Node> IncidentNodes {
        get {
            return Edges.Select(z => z.OtherNode(this));
        }
    }

    public IEnumerable<Edge> IncidentEdges {
        get {
            foreach (var e in Edges)
                yield return e;
        }
    }

    public bool IncidentNodesExist {
        get {
            return Edges.Exists(z => z.OtherNode(this) != null);
        }
    }

    public static Edge Connect(Node node1, Node node2, Graph graph) {
        if (!graph.Nodes.ContainsKey(node1.Position) || !graph.Nodes.ContainsKey(node2.Position))
            throw new ArgumentException();
        var edge = new Edge(node1, node2);

        if (!node1.Edges.Exists(x => (x.From == node1 && x.To == node2) || (x.From == node2 && x.To == node1)))
            node1.Edges.Add(edge);
        if (!node2.Edges.Exists(x => (x.From == node1 && x.To == node2) || (x.From == node2 && x.To == node1)))
            node2.Edges.Add(edge);

        return edge;
    }

    public static void Disconnect(Edge edge) {
        edge.From.Edges.Remove(edge);
        edge.To.Edges.Remove(edge);
    }

    public override string ToString() {
        return string.Format("{0}", Position);
    }
}

public class Edge {
    public Node From;
    public Node To;

    public Edge(Node from, Node to) {
        From = from;
        To = to;
    }

    public bool IsIncident(Node node) {
        return node == From || node == To;
    }

    public Node OtherNode(Node node) {
        if (From == node)
            return To;
        else if (To == node)
            return From;
        return null;
    }

    public override string ToString() {
        return string.Format("({0} - {1})", From.Position, To.Position);
    }
}

public class Graph {
    public Dictionary<Vector2, Node> Nodes = new Dictionary<Vector2, Node>();

    public Graph() {
    }

    public Graph(Dictionary<Vector2, Node> nodes) {
        Nodes = nodes;
    }

    public void AddNode(Node node) {
        if (Nodes.ContainsKey(node.Position)) {
            throw new NullReferenceException("node with such position already exist");
        }
        Nodes.Add(node.Position, node);
    }

    public void AddNode(Node from, Node to) {
        if (!Nodes.ContainsKey(from.Position))
            Nodes.Add(from.Position, from);
        if (!Nodes.ContainsKey(to.Position))
            Nodes.Add(to.Position, to);

        Node.Connect(from, to, this);
    }

    public void RemoveNode(Node node) {
        List<Edge> edges = node.IncidentEdges.Where(x => x.To == node || x.From == node).ToList();

        for (int i = 0; i < edges.Count; i++) {
            Node.Disconnect(edges[i]);
        }

        foreach (var o in Nodes.Where(x => x.Value == node).ToList()) {
            Nodes.Remove(o.Key);
        }
    }
}

public class AstarAlgorithm {
    public static List<Node> CalculatePath(Graph graph, Node start, Node end) {
        var closedSet = new Collection<AstarNode>();
        var openSet = new Collection<AstarNode>();

        AstarNode startNode = new AstarNode(start, null, 0, GetHeuristicPathLength(start.Position, end.Position));
        AstarNode bestNode = null;

        openSet.Add(startNode);
        while (openSet.Count > 0) {
            var currentNode = openSet.OrderBy(node =>
              node.EstimateFullPathLength).First();

            if (bestNode == null) bestNode = currentNode;
            else {
                if (Vector2.Distance(currentNode.Node.Position, end.Position) < Vector2.Distance(bestNode.Node.Position, end.Position)) bestNode = currentNode;
            }

            if (currentNode.Node == end) {
                return GetPathForNode(currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (var neighbourNode in GetNeighbours(currentNode, end)) {
                if (closedSet.Count(node => node.Node == neighbourNode.Node) > 0)
                    continue;
                var openNode = openSet.FirstOrDefault(node =>
                                                      node.Node == neighbourNode.Node);

                if (openNode == null)
                    openSet.Add(neighbourNode);
                else
                  if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart) {
                    openNode.CameFrom = currentNode;
                    openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                }
            }
        }
        return null;
    }

    public static List<Node> CalculatePathWithoutBlock(Graph graph, Node start, Node end, List<Node> visited = null) {
        var closedSet = new Collection<AstarNode>();
        var openSet = new Collection<AstarNode>();

        AstarNode startNode = new AstarNode(start, null, 0, GetHeuristicPathLength(start.Position, end.Position));
        AstarNode bestNode = null;

        openSet.Add(startNode);
        while (openSet.Count > 0) {
            var currentNode = openSet.OrderBy(node =>
              node.EstimateFullPathLength).First();

            if (bestNode == null) bestNode = currentNode;
            else {
                if (Vector2.Distance(currentNode.Node.Position, end.Position) < Vector2.Distance(bestNode.Node.Position, end.Position)) bestNode = currentNode;
            }

            if (currentNode.Node == end) {
                return GetPathForNode(currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (var neighbourNode in GetNeighbours(currentNode, end, visited, false)) {
                if (closedSet.Count(node => node.Node == neighbourNode.Node) > 0)
                    continue;
                var openNode = openSet.FirstOrDefault(node =>
                                                      node.Node == neighbourNode.Node);

                if (openNode == null)
                    openSet.Add(neighbourNode);
                else
                  if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart) {
                    openNode.CameFrom = currentNode;
                    openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                }
            }
        }
        return null;
    }

    public static List<List<Node>> CalculateAllPaths(Node start, Node end) {
        List<List<Node>> paths = new List<List<Node>>();
        Recursive(start, end, paths, new HashSet<Node>());
        return paths;
    }

    private static void Recursive (Node current, Node destination, List<List<Node>> paths, HashSet<Node> path) {
        path.Add(current);

        if (current == destination) {
            paths.Add(new List<Node>(path));
            path.Remove(current);
            return;
        }

        List<Node> nodes = current.IncidentNodes.ToList();

        foreach (Node n in nodes) {
            if (!path.Contains(n)) {
                Recursive(n, destination, paths, path);
            }
        }

        path.Remove(current);
    }    

    private static Collection<AstarNode> GetNeighbours(AstarNode node, Node end, List<Node> visited = null, bool checkWalkable = true) {
        var result = new Collection<AstarNode>();

        foreach (var e in node.Node.IncidentNodes) {
            if (checkWalkable && (!e.IsFree && e != end)) continue;
            if (visited != null && visited.Contains(e)) continue;

            AstarNode anode = new AstarNode(e, node, node.PathLengthFromStart + GetHeuristicPathLength(node.Node.Position, e.Position), GetHeuristicPathLength(e.Position, end.Position));
            result.Add(anode);
        }

        return result;
    }

    private static List<Node> GetPathForNode(AstarNode pathNode) {
        var result = new List<Node>();
        var currentNode = pathNode;
        while (currentNode != null) {
            result.Add(currentNode.Node);
            currentNode = currentNode.CameFrom;
        }
        result.Reverse();
        return result;
    }

    private static float GetHeuristicPathLength(Vector2 from, Vector2 to) {
        return Vector2.Distance(to, from);
    }

    public static bool PathIsFree(Graph graph, Node start, Node end) {
        return CalculatePath(graph, start, end) != null;
    }
}