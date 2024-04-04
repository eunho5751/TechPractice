using UnityEngine;

public class Cube : MonoBehaviour, IShape
{
    [SerializeField]
    private Vector3 _size = Vector3.one;

    public float SDF(Vector3 point)
    {
        Vector3 diff = point - transform.position;
        Vector3 o = new Vector3(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z)) - _size * 0.5f;
        float ud = new Vector3(Mathf.Max(o.x, 0f), Mathf.Max(o.y, 0f), Mathf.Max(o.z, 0f)).magnitude;
        float n = Mathf.Min(Mathf.Max(o.x, Mathf.Max(o.y, o.z)), 0f);
        return ud + n;
    }

    public Bounds Bounds => new Bounds(transform.position, _size);
}