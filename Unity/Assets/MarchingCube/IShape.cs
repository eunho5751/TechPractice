using UnityEngine;

public interface IShape
{
    float SDF(Vector3 point);
    Bounds Bounds { get; }
}