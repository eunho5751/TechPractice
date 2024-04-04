using System.Collections.Generic;
using UnityEngine;

public interface IVoxelRepresentation
{
    void Build(IEnumerable<IShape> shapes);
    IEnumerable<Vector3> Positions { get; }
    float VoxelSize { get; }
}
