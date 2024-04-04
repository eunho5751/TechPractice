using UnityEngine;

public class Sphere : MonoBehaviour, IShape
{
    [SerializeField]
    private float _radius = 0.5f;

    public float SDF(Vector3 point)
    {
        return Vector3.Distance(point, transform.position) - _radius;
    }

    public Bounds Bounds => new Bounds(transform.position, Vector3.one * (_radius * 2f));
}