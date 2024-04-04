using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour, IVoxelRepresentation
{
    [SerializeField, Min(1)]
    private int _width = 64;
    [SerializeField, Min(1)]
    private int _height = 64;
    [SerializeField, Min(1)]
    private int _depth = 64;
    [SerializeField, Min(0f)]
    private float _size = 1f;

    private Vector3[] _positions;

    public void Build(IEnumerable<IShape> shapes)
    {
        int wLen = Mathf.FloorToInt(_width / _size);
        int hLen = Mathf.FloorToInt(_height / _size);
        int dLen = Mathf.FloorToInt(_depth / _size);
        int len = wLen * hLen * dLen;
        if (_positions == null || _positions.Length != len)
        {
            _positions = new Vector3[len];
        }

        int idx = 0;
        Vector3 center = transform.position;
        float halfW = center.x + _width / 2f;
        float halfH = center.y + _height / 2f;
        float halfD = center.z + _depth / 2f;
        for (int w = 0; w < wLen; w++)
        {
            float wOffset = w * _size;
            for (int h = 0; h < hLen; h++)
            {
                float hOffset = h * _size;
                for (int d = 0; d < dLen; d++)
                {
                    float dOffset = d * _size;
                    
                    _positions[idx] = new Vector3(-halfW, -halfH, -halfD) + new Vector3(wOffset, hOffset, dOffset);
                    idx++;
                }
            }
        }
    }

    public IEnumerable<Vector3> Positions => _positions;

    public float VoxelSize
    {
        get => _size;
        set => _size = value;
    }
}