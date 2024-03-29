using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class RaymarchingRenderer : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    private GraphicsBuffer _buffer;

    private void Start()
    {
        UpdateBuffer();
    }

    private void OnDestroy()
    {
        if (_buffer != null)
            _buffer.Release();
    }

    private void Update()
    {
        UpdateBuffer();
    }

    private void UpdateBuffer()
    {
        if (_buffer != null)
            _buffer.Release();

        Shape[] shapes = GetComponentsInChildren<RaymarchingShape>().Select(x => x.Shape).ToArray();
        if (shapes.Length == 0)
            return;

        _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, shapes.Length, Marshal.SizeOf<Shape>());
        _buffer.SetData(shapes);
        
        _material.SetBuffer("_Shapes", _buffer);
        _material.SetInteger("_NumShapes", _buffer.count);
    }
}
