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
    public uint[] _waterMap;

    private readonly uint EVAPORATION_THRESHOLD = 1;

    private readonly uint LEVEL_MASK = 0x00ff;
    private readonly uint SOURCE_MASK = 0x0100;
    private readonly uint WALL_MASK = 0x0200;
    private readonly uint FLOW_MASK = 0x0C00;

    private readonly uint TOP = 0;
    private readonly uint BOTTOM = 1;
    private readonly uint LEFT = 2;
    private readonly uint RIGHT = 3;

    void Start()
    {
        _waterMap = new uint[width * height];
        for (uint y = 0; y < height; ++y)
        {
            for (uint x = 0; x < width; ++x)
            {
                uint index = (uint)width * y + x;
                _waterMap[index] = 0;
            }
        }

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
        waterMaterial.SetFloat("_Shift", Mathf.Sin(Time.time * 5.0f));

        if (Input.GetKeyDown(KeyCode.D))
        {
            int x = width / 2;
            int y = height / 2;

            //_cellsBuffer.GetData(_waterMap);
            _waterMap[width * (y - 3) + x + 3] = 0xff | SOURCE_MASK;
            _cellsBuffer.SetData(_waterMap);
            waterMaterial.SetBuffer("_CellsBuffer", _cellsBuffer);
        }

        if (Input.GetKey(KeyCode.S))
        {
            int x = width / 2;
            int y = height / 2;

            //_cellsBuffer.GetData(_waterMap);
            _waterMap[width * y + x] |= 0xff;
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
        //computeShader.Dispatch(_baseKernel, width, height, 1);
        for (uint y = 0; y < height; ++y)
        {
            for (uint x = 0; x < width; ++x)
            {
                uint index = (uint)width * y + x;
                uint centerValue = _waterMap[index];

                uint centerLevel = centerValue & LEVEL_MASK;
                if (centerLevel < EVAPORATION_THRESHOLD)
                {
                    _waterMap[index] &= ~LEVEL_MASK;
                    continue;
                }

                if ((centerValue & WALL_MASK) > 0)
                {
                    continue;
                }

                var values = new[]{
                    y > 0 ? _waterMap[index - width] : 0, // TOP
                    y + 1 < height ? _waterMap[index + width] : 0, // BOTTOM
                    x > 0 ? _waterMap[index - 1] : 0, // LEFT
                    x + 1 < width ?  _waterMap[index + 1] : 0, // RIGHT
                };

                uint flow = (centerValue & FLOW_MASK) >> 10;
                var directions = new uint[] { 0, 0, 0, 0 };
                directions[0] = flow;
                for (uint i = 0; i < 3; ++i)
                {
                    directions[i + 1] = i < flow ? i : i + 1;
                }

                while (true)
                {
                    bool changed = false;
                    for (uint i = 0; i < 4 && centerLevel > 0; ++i)
                    {
                        uint value = values[directions[i]];

                        uint level = value & LEVEL_MASK;
                        if ((value & (WALL_MASK | SOURCE_MASK)) == 0 && level < centerLevel)
                        {
                            changed = true;
                            uint delta = (uint)Mathf.Min(Mathf.Min(5, 255 - (int)level), (int)centerLevel);

                            level += delta;
                            if ((centerValue & SOURCE_MASK) == 0)
                            {
                                centerLevel -= delta;
                            }
                            values[directions[i]] = (directions[i] << 10) | (level & LEVEL_MASK);
                        }
                    }

                    if (!changed)
                    {
                        break;
                    }
                }

                if (y > 0)
                {
                    _waterMap[index - width] = values[TOP];
                }
                if (y + 1 < height)
                {
                    _waterMap[index + width] = values[BOTTOM];
                }
                if (x > 0)
                {
                    _waterMap[index - 1] = values[LEFT];
                }
                if (x + 1 < width)
                {
                    _waterMap[index + 1] = values[RIGHT];
                }
                if ((centerValue & SOURCE_MASK) == 0) {
                    _waterMap[index] = centerLevel & LEVEL_MASK;
                }
            }
        }

        _cellsBuffer.SetData(_waterMap);
    }
}
