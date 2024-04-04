using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Octree : MonoBehaviour, IVoxelRepresentation
{
    [SerializeField, Min(1)]
    private int _size = 64;
    [SerializeField, Min(0)]
    private int _maxDepth = 3;
    [SerializeField]
    private int _gizmosDepth = -1;

    private OctreeNode _rootNode;

    private void OnValidate()
    {
        _gizmosDepth = Mathf.Clamp(_gizmosDepth, -1, _maxDepth);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Color color = Gizmos.color;
        Gizmos.color = Color.gray;

        foreach (var node in GetNodes(_gizmosDepth))
        {
            Gizmos.DrawWireCube(node.Bounds.center, node.Bounds.size);
        }

        Gizmos.color = color;
    }

    private List<OctreeNode> GetNodes(int depth)
    {
        List<OctreeNode> nodes = new List<OctreeNode>();
        Stack<OctreeNode> stack = new Stack<OctreeNode>();
        stack.Push(_rootNode);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (depth == -1 || node.Depth == depth)
            {
                if (node.Children != null)
                {
                    foreach (var child in node.Children)
                    {
                        stack.Push(child);
                    }
                }
                nodes.Add(node);
            }
            else if (node.Depth < depth && node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    stack.Push(child);
                }
            }
        }
        return nodes;
    }

    public void Build(IEnumerable<IShape> shapes)
    {
        _rootNode = new OctreeNode(this, 0, transform.position - Vector3.one * (_size * 0.5f), _size);
        foreach (var shape in shapes)
        {
            _rootNode.Add(shape);
        }
    }

    public IEnumerable<Vector3> Positions => GetNodes(_maxDepth).Select(x => x.Bounds.min);
    public float VoxelSize => _size * Mathf.Pow(0.5f, _maxDepth);
    public int MaxDepth => _maxDepth;
}