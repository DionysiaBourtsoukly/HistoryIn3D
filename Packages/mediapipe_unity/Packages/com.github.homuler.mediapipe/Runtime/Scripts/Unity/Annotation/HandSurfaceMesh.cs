using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Tasks.Components.Containers;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HandSurfaceMesh : MonoBehaviour
{
    private Mesh _mesh;
    private Vector3[] _vertices = new Vector3[21];

    // Predefined triangle topology connecting the 21 MediaPipe hand landmarks
    private static readonly int[] HandTriangles = new int[]
    {
        // Palm
        0, 1, 2,   0, 2, 5,   0, 5, 9,   0, 9, 13,   0, 13, 17,
        // Thumb
        1, 2, 3,   2, 3, 4,
        // Index Finger
        5, 6, 7,   6, 7, 8,
        // Middle Finger
        9, 10, 11, 10, 11, 12,
        // Ring Finger
        13, 14, 15, 14, 15, 16,
        // Pinky
        17, 18, 19, 18, 19, 20
    };

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    public void UpdateMesh(Vector3[] worldVertices)
    {
        if (worldVertices == null || worldVertices.Length < 21) return;

        _mesh.Clear();
        // Use the exact world positions calculated by HandOverlayAR
        _mesh.vertices = worldVertices; 
        _mesh.triangles = HandTriangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }
}