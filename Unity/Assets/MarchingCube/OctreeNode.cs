using System.Collections.Generic;
using UnityEngine;

public class OctreeNode
{
    private Octree _tree;
    private OctreeNode[] _children;

    public OctreeNode(Octree tree, int depth, Vector3 position, float size)
    {
        _tree = tree;
        Depth = depth;
        Bounds = new Bounds(position + Vector3.one * (size * 0.5f), Vector3.one * size);
    }

    public void Add(IShape shape)
    {
        if (Depth >= _tree.MaxDepth)
        {
            return;
        }

        float extent = Bounds.extents.x;
        Vector3[] corners = new Vector3[]
        {
            Bounds.center + new Vector3(-extent, -extent, -extent),
            Bounds.center + new Vector3(-extent, -extent, extent),
            Bounds.center + new Vector3(-extent, extent, -extent),
            Bounds.center + new Vector3(-extent, extent, extent),
            Bounds.center + new Vector3(extent, -extent, -extent),
            Bounds.center + new Vector3(extent, extent, -extent),
            Bounds.center + new Vector3(extent, -extent, extent),
            Bounds.center + new Vector3(extent, extent, extent)
        };

        float min = float.MaxValue, max = float.MinValue;
        foreach (Vector3 corner in corners)
        {
            float dist = shape.SDF(corner);
            min = Mathf.Min(min, dist);
            max = Mathf.Max(max, dist);
        }

        bool containsShape = (Bounds.Contains(shape.Bounds.center) || min <= 0) && max >= 0;
        if (containsShape)
        {
            Subdivide();
            foreach (var child in _children)
            {
                child.Add(shape);
            }
        }
    }

    private void Subdivide()
    {
        if (_children != null)
        {
            return;
        }

        int depth = Depth + 1;
        float size = Bounds.size.x * 0.5f;
        _children = new OctreeNode[8];
        _children[0] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(-size, -size, -size), size);
        _children[1] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(-size, -size, 0f), size);
        _children[2] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(-size, 0f, -size), size);
        _children[3] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(-size, 0f, 0f), size);
        _children[4] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(0f, 0f, 0f), size);
        _children[5] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(0f, 0f, -size), size);
        _children[6] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(0f, -size, -size), size);
        _children[7] = new OctreeNode(_tree, depth, Bounds.center + new Vector3(0f, -size, 0f), size);
    }

    public int Depth { get; }
    public Bounds Bounds { get; }
    public IReadOnlyList<OctreeNode> Children => _children;
}