#if UNITY_EDITOR

using UnityEngine;
using Random = UnityEngine.Random;

namespace Dustyroom {
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CurveRenderer : MonoBehaviour {
    public Transform end1;
    public Transform end2;

    [Space]
    public int points = 20;

    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 0);
    public float curveMultiplier = 1;

    [Space]
    public float thickness = 0.02f;

    [Space]
    public int quantity = 1;

    public float curveVariability = 0.2f;

    [Range(0, 1)]
    public float thicknessVariability = 0.1f;

    public int randomSeed = 0;
    public Vector3 interval = new Vector3(0.5f, 0, 0);

    MeshFilter _meshFilter;
    Mesh _mesh;

    private void OnValidate() {
        points = Mathf.Max(points, 2);
        quantity = Mathf.Max(quantity, 1);
        curveVariability = Mathf.Max(curveVariability, 0);
        Refresh();
    }

    private void Refresh() {
        if (!end1 || !end2) return;

        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null) {
            return;
        }

        _mesh = _meshFilter.sharedMesh;
        if (_mesh == null) {
            _mesh = new Mesh();
            _meshFilter.sharedMesh = _mesh;
        }

        var numPathPoints = (points + 1) * quantity;
        const int numVerticesPerPathPoint = 4;
        Vector3[] verts = new Vector3[numPathPoints * numVerticesPerPathPoint];

        int numTris = 2 * numVerticesPerPathPoint * (numPathPoints - 1);
        int[] triangles = new int[numTris * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // @formatter:off
        int[] triangleMap = {
            0, 4, 1, 1, 4, 5,
            0, 3, 7, 0, 7, 4,
            2, 7, 3, 2, 6, 7,
            2, 1, 5, 2, 5, 6
        };
        // @formatter:on

        // Fix the random seed for deterministic output.
        var state = Random.state;
        Random.InitState(randomSeed);

        for (int line = 0; line < quantity; line++) {
            var scale = curveMultiplier + Random.Range(-curveVariability, curveVariability);
            var lineThickness = thickness * (1f + Random.Range(-thicknessVariability, thicknessVariability));
            for (int i = 0; i <= points; ++i) {
                float alpha = i / (float)points;
                var positionLine = Vector3.Lerp(end1.position, end2.position, alpha) +
                                   Vector3.up * curve.Evaluate(alpha) * scale;
                var pointPosition = transform.InverseTransformPoint(positionLine + line * interval);

                {
                    Vector3 localUp = transform.up;
                    Vector3 localRight = transform.right;

                    float ht = lineThickness * 0.5f;
                    // 0 --- 1
                    // |  p  |
                    // 3 --- 2
                    verts[vertIndex + 0] = pointPosition - localRight * ht + localUp * ht;
                    verts[vertIndex + 1] = pointPosition + localRight * ht + localUp * ht;
                    verts[vertIndex + 2] = pointPosition + localRight * ht - localUp * ht;
                    verts[vertIndex + 3] = pointPosition - localRight * ht - localUp * ht;

                    if (i < points) {
                        for (int j = 0; j < triangleMap.Length; j++) {
                            triangles[triIndex + j] = vertIndex + triangleMap[j];
                        }
                    }

                    vertIndex += 4;
                    triIndex += triangleMap.Length;
                }
            }
        }

        Random.state = state;

        _mesh.Clear();
        _mesh.vertices = verts;
        _mesh.SetTriangles(triangles, 0);
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }
}
}

#endif