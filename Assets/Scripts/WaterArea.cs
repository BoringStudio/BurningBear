using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour
{
    public WaterMesh waterMesh;
    public int ticksBeforeUpdate = 5;
    public ComputeShader computeShader;
    public Material waterMaterial;

    public int width = 128;
    public int height = 512;
    public float heightScale = 0.25f;

    private int _currentTicksBeforeUpdate = 0;
    private int _baseKernel = 0;

    public ComputeBuffer _cellsBuffer;
    public float[] _waterMap;

    void Start()
    {
        _waterMap = new float[width * height];

        int x = width / 2;
        int y = height / 2;

        //for (int i = 0; i < 10; ++i)
        //{
        //    _waterMap[width * (y + 2) + x + i - 5] = 10000.0f;
        //}

        //for (int i = 0; i < 10; ++i)
        //{
        //    _waterMap[width * (y - 3) + x + i - 5] = 10000.0f;
        //}

        _cellsBuffer = new ComputeBuffer(_waterMap.Length, sizeof(float));
        _cellsBuffer.SetData(_waterMap);

        waterMaterial.SetBuffer("_CellsBuffer", _cellsBuffer);

        _baseKernel = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(_baseKernel, "Result", _cellsBuffer);

        var origin = transform.position + Vector3.left * width * 0.5f + Vector3.forward * height * heightScale * 0.5f;
        for (int i = 0; i < height; ++i)
        {
            var gameObject = Instantiate(waterMesh, origin + Vector3.back * heightScale * i + Vector3.right * Random.Range(0.0f, 0.25f), Quaternion.Euler(60.0f, 0.0f, 0.0f), transform);
            gameObject.UpdateMesh(width, (uint)i);
        }
    }

    void OnDestroy()
    {
        _cellsBuffer.Release();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            int x = width / 2;
            int y = height / 2;

            _cellsBuffer.GetData(_waterMap);
            _waterMap[width * y + x] = 113.0f;
            _waterMap[width * y + x + height] = 113.0f;
            _waterMap[width * y + x - height] = 113.0f;
            _waterMap[width * y + x + 1] = 113.0f;
            _waterMap[width * y + x - 1] = 113.0f;
            _cellsBuffer.SetData(_waterMap);
            waterMaterial.SetBuffer("_CellsBuffer", _cellsBuffer);
        }
    }

    void FixedUpdate()
    {   
        if (--_currentTicksBeforeUpdate <= 0)
        {
            UpdateCells();
            _currentTicksBeforeUpdate = ticksBeforeUpdate;
        }
    }

    void UpdateCells()
    {
        computeShader.Dispatch(_baseKernel, width, height, 1);
    }
}
