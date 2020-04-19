using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterArea : Singleton<WaterArea>
{
    public int ticksBeforeUpdate = 5;
    public WaterMesh waterMesh;
    public Material waterMaterial;
    public Tilemap worldTileMap; 

    public int width = 128;
    public int height = 128;

    private int _currentTicksBeforeUpdate = 0;

    private uint[] _waterMap;
    private ComputeBuffer _cellsBuffer;

    private readonly uint EVAPORATION_THRESHOLD = 20;

    private readonly uint LEVEL_MASK = 0x00ff;
    private readonly uint SOURCE_MASK = 0x0100;
    private readonly uint WALL_MASK = 0x0200;
    private readonly uint FLOW_MASK = 0x0c00;
    private readonly uint SPREAD_MASK = 0xffff0000;

    private readonly uint TOP = 0;
    private readonly uint BOTTOM = 1;
    private readonly uint LEFT = 2;
    private readonly uint RIGHT = 3;

    void Start()
    {
        _waterMap = new uint[width * height];

        var origin = transform.position + Vector3.left * width * 0.5f + Vector3.forward * height * 0.5f;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x) {
                var worldPosition = origin + Vector3.back * y + Vector3.right * x + (Vector3.right + Vector3.back) * 0.5f;

                if (!worldTileMap.HasTile(worldTileMap.WorldToCell(worldPosition)))
                {
                    _waterMap[width * y + x] = WALL_MASK | LEVEL_MASK;
                }
            }

            var localOrigin = origin + Vector3.back * y;

            for (int j = 0; j < 4; ++j)
            {
                var gameObject = Instantiate(waterMesh, localOrigin + Vector3.back * j * 0.25f + Vector3.right * Random.Range(0.0f, 0.25f), Quaternion.Euler(60.0f, 0.0f, 0.0f), transform);
                gameObject.UpdateMesh(width, (uint)y, (uint)(j % 2));
            }
        }

        _cellsBuffer = new ComputeBuffer(_waterMap.Length, sizeof(float));
        _cellsBuffer.SetData(_waterMap);

        waterMaterial.SetBuffer("_CellsBuffer", _cellsBuffer);
    }

    void OnDestroy()
    {
        _cellsBuffer.Release();
    }

    void Update()
    {
        waterMaterial.SetFloat("_Shift", Mathf.Sin(Time.time * 5.0f));
    }

    void FixedUpdate()
    {
        if (--_currentTicksBeforeUpdate <= 0)
        {
            UpdateCells();
            _currentTicksBeforeUpdate = ticksBeforeUpdate;
        }
    }

    public void Liquify(Vector3 worldPoint)
    {
        var offset = worldPoint + new Vector3(width / 2 + 1, 0, height / 2 + 1);
        var x = (int)offset.x;
        var y = height - (int)offset.z;

        _waterMap[width * y + x] |= SOURCE_MASK | LEVEL_MASK;
    }

    public void Evaporate(Vector3 worldPoint)
    {
        var offset = worldPoint + new Vector3(width / 2 + 1, 0, height / 2 + 1);
        var x = (int)offset.x;
        var y = height - (int)offset.z;

        var kernel = new uint[] { 3, 5, 7, 9, 9, 9, 7, 5, 3 };
        for (int i = 0; i < kernel.Length; ++i)
        {
            for (int j = 0; j < kernel[i]; ++j)
            {
                if (j == 0 || j + 1 == kernel[i])
                {
                    _waterMap[width * (y - kernel.Length / 2 + i) + x - kernel[i] / 2 + j] &= ~SPREAD_MASK;
                }
                else
                {
                    _waterMap[width * (y - kernel.Length / 2 + i) + x - kernel[i] / 2 + j] = 0;
                }
            }
        }
    }

    void UpdateCells()
    {
        for (uint y = 0; y < height; ++y)
        {
            for (uint x = 0; x < width; ++x)
            {
                uint index = (uint)width * y + x;
                uint centerValue = _waterMap[index];

                if ((centerValue & WALL_MASK) > 0)
                {
                    continue;
                }

                uint centerLevel = centerValue & LEVEL_MASK;
                if (centerLevel < EVAPORATION_THRESHOLD)
                {
                    _waterMap[index] &= ~LEVEL_MASK;
                    continue;
                }

                var values = new[]{
                    y > 0 ? _waterMap[index - width] : 0, // TOP
                    y + 1 < height ? _waterMap[index + width] : 0, // BOTTOM
                    x > 0 ? _waterMap[index - 1] : 0, // LEFT
                    x + 1 < width ?  _waterMap[index + 1] : 0, // RIGHT
                };

                uint flow = (centerValue & FLOW_MASK) >> 10;
                var directions = new uint[] { TOP, BOTTOM, LEFT, RIGHT };

                uint centerSpread = (centerValue & SPREAD_MASK) >> 16;
                if (centerSpread < 20)
                {
                    directions[0] = flow;
                    for (uint i = 0; i < 3; ++i)
                    {
                        directions[i + 1] = i < flow ? i : i + 1;
                    }

                    while (true) {
                        bool changed = false;
                        for (uint i = 0; i < 4 && centerLevel > 0; ++i)
                        {
                            uint value = values[directions[i]];

                            uint level = value & LEVEL_MASK;
                            if ((value & (WALL_MASK | SOURCE_MASK | SPREAD_MASK)) == 0 && level < centerLevel)
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

                    if ((centerValue & SOURCE_MASK) > 0)
                    {
                        centerSpread++;
                    }
                }
                else 
                {
                    centerSpread = 0;
                    for (uint i = 0; i < 4 && centerLevel > 0; ++i)
                    {
                        uint value = values[directions[i]];

                        if ((value & (WALL_MASK | SOURCE_MASK | SPREAD_MASK)) == 0)
                        {
                            values[directions[i]] = (directions[i] << 10) | SOURCE_MASK | LEVEL_MASK;
                        }
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
                else
                {
                    _waterMap[index] = ((centerSpread << 16) & SPREAD_MASK) | SOURCE_MASK | LEVEL_MASK;
                }
            }
        }

        _cellsBuffer.SetData(_waterMap);
    }
}
