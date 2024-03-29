using System;
using UnityEngine;

public enum SDF
{
    Sphere = 0,
    Cube
}

[Serializable]
public struct Shape
{
    public Color Color;
    [HideInInspector]
    public Vector3 Position;
    public float Size;
    public float Blend;
    public SDF SDF;
}

public class RaymarchingShape : MonoBehaviour
{
    [SerializeField]
    private Shape _shape;

    public Shape Shape
    {
        get
        {
            return new()
            {
                Color = _shape.Color,
                Position = transform.position,
                Size = _shape.Size,
                Blend = _shape.Blend,
                SDF = _shape.SDF
            };
        }
    }
}