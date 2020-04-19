using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class WaterMesh : MonoBehaviour
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct MeshVertex
    {
        public Vector3 position;
        public uint unitIndex;
        public Vector2 texCoords;
    }

    private MeshFilter _meshFilter;

    private readonly VertexAttributeDescriptor[] _vertexLayout = new[] {
        new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
        new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UInt32, 1),
        new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
    };

    public void UpdateMesh(int unitCount, uint lineIndex, uint textureOffset)
    {
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (_meshFilter.sharedMesh == null)
        {
            _meshFilter.sharedMesh = new Mesh();
        }

        var mesh = _meshFilter.sharedMesh;

        mesh.SetVertexBufferParams(unitCount * 6, _vertexLayout);

        var vertices = new MeshVertex[unitCount * 6];

        var offset = (uint)unitCount * lineIndex;
        for (int i = 0; i < unitCount; ++i)
        {
            var unitIndex = offset + (uint)i;

            var origin = Vector3.right * i;
            var currentVertex = i * 6;

            // 0
            vertices[currentVertex + 0].position = origin + Vector3.up;
            vertices[currentVertex + 0].texCoords = Vector2.up;
            vertices[currentVertex + 0].unitIndex = (textureOffset << 28) | unitIndex;

            // 1
            vertices[currentVertex + 1].position = origin;
            vertices[currentVertex + 1].texCoords = Vector2.zero;
            vertices[currentVertex + 1].unitIndex = (textureOffset << 28) | unitIndex;

            // 2
            vertices[currentVertex + 2].position = origin + Vector3.right * 0.5f + Vector3.up;
            vertices[currentVertex + 2].texCoords = Vector2.right * 0.5f + Vector2.up;
            vertices[currentVertex + 2].unitIndex = (textureOffset << 28) | unitIndex;

            // 3
            vertices[currentVertex + 3].position = origin + Vector3.right * 0.5f;
            vertices[currentVertex + 3].texCoords = Vector2.right * 0.5f;
            vertices[currentVertex + 3].unitIndex = (textureOffset << 28) | unitIndex;

            // 4
            vertices[currentVertex + 4].position = origin + Vector3.right + Vector3.up;
            vertices[currentVertex + 4].texCoords = Vector2.right + Vector2.up;
            vertices[currentVertex + 4].unitIndex = (textureOffset << 28) | unitIndex;

            // 5
            vertices[currentVertex + 5].position = origin + Vector3.right;
            vertices[currentVertex + 5].texCoords = Vector2.right;
            vertices[currentVertex + 5].unitIndex = (textureOffset << 28) | unitIndex;
        }

        var indices = new int[unitCount * 12];

        for (int i = 0; i < unitCount; ++i)
        {
            var indexOffset = i * 6;
            var currentItem = i * 12;

            indices[currentItem + 0] = indexOffset;
            indices[currentItem + 1] = indexOffset + 3;
            indices[currentItem + 2] = indexOffset + 1;

            indices[currentItem + 3] = indexOffset;
            indices[currentItem + 4] = indexOffset + 2;
            indices[currentItem + 5] = indexOffset + 3;

            indices[currentItem + 6] = indexOffset + 2;
            indices[currentItem + 7] = indexOffset + 5;
            indices[currentItem + 8] = indexOffset + 3;

            indices[currentItem + 9] = indexOffset + 2;
            indices[currentItem + 10] = indexOffset + 4;
            indices[currentItem + 11] = indexOffset + 5;
        }

        mesh.SetVertexBufferData(vertices, 0, 0, unitCount * 6);
        mesh.triangles = indices;
        mesh.bounds = new Bounds(Vector3.right * (unitCount / 2.0f) + Vector3.up * 0.5f, Vector3.right * unitCount + Vector3.up);
    }
}
