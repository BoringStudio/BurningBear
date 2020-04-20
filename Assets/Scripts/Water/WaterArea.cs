﻿using UnityEngine;
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

    private readonly uint LEVEL_MASK    = 0x000000ff;
    private readonly uint SOURCE_MASK   = 0x00000100;
    private readonly uint WALL_MASK     = 0x00000200;
    private readonly uint FLOW_MASK     = 0x00000c00;
    private readonly uint DRAIN_MASK    = 0x0000f000;
    private readonly uint SPREAD_MASK   = 0xffff0000;

    private readonly uint TOP = 0;
    private readonly uint BOTTOM = 1;
    private readonly uint LEFT = 2;
    private readonly uint RIGHT = 3;

    private readonly int[] CIRCLE_KERNEL = new int[]
    {
        5, 7, 9, 9, 9, 7, 5
    };

    private readonly int[] SECTOR_KERNEL = new int[]
    {
        1, 3, 3, 5, 5, 5, 5, 7, 7, 7, 7, 7, 9, 7, 5, 3
    };

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

        _waterMap[width * y + x] = SOURCE_MASK | LEVEL_MASK;
    }

    public void EvaporateSector(Vector3 worldPoint, int direction, uint max, uint value)
    {
        var offset = worldPoint + new Vector3(width / 2, 0, height / 2);
        var x = (int)offset.x;
        var y = height - (int)offset.z;

        switch (direction)
        {
            case 0:
                for (int i = 0; i < SECTOR_KERNEL.Length && i < max; ++i)
                {
                    for (int j = 0; j < SECTOR_KERNEL[i]; ++j)
                    {
                        var index = width * (y - SECTOR_KERNEL[i] / 2 + j) + x - i;
                        var current = _waterMap[index];

                        if ((current & WALL_MASK) == 0)
                        {
                            if ((current & SOURCE_MASK) != 0)
                            {
                                current &= ~(SPREAD_MASK | SOURCE_MASK);
                            }

                            var currentLevel = current & LEVEL_MASK;
                            currentLevel -= (uint)Mathf.Min((int)currentLevel, value);

                            _waterMap[index] = (current & ~LEVEL_MASK) | (currentLevel & LEVEL_MASK);
                        }
                    }
                }
                break;
        }
    }

    public void EvaporateCircle(Vector3 worldPoint)
    {
        var offset = worldPoint + new Vector3(width / 2, 0, height / 2);
        var x = (int)offset.x;
        var y = height - (int)offset.z;

        for (int i = 0; i < CIRCLE_KERNEL.Length; ++i)
        {
            for (int j = 0; j < CIRCLE_KERNEL[i]; ++j)
            {
                var index = width * (y - CIRCLE_KERNEL.Length / 2 + i) + x - CIRCLE_KERNEL[i] / 2 + j;
                var current = _waterMap[index];

                if ((current & WALL_MASK) == 0)
                {
                    if ((current & SOURCE_MASK) != 0)
                    {
                        current &= ~(SPREAD_MASK | SOURCE_MASK);
                    }                    
                    _waterMap[index] = ((1 << 12) & DRAIN_MASK) | (current & ~DRAIN_MASK);
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
                uint centerDrain = (centerValue & DRAIN_MASK) >> 12;
                if (centerDrain == 0 && centerLevel < EVAPORATION_THRESHOLD)
                {
                    _waterMap[index] &= ~LEVEL_MASK;
                    continue;
                }

                bool centerIsSource = (centerValue & SOURCE_MASK) != 0;
                uint centerFlow = (centerValue & FLOW_MASK) >> 10;
                uint centerSpread = (centerValue & SPREAD_MASK) >> 16;

                var values = new[]{
                    y > 0 ? _waterMap[index - width] : 0, // TOP
                    y + 1 < height ? _waterMap[index + width] : 0, // BOTTOM
                    x > 0 ? _waterMap[index - 1] : 0, // LEFT
                    x + 1 < width ?  _waterMap[index + 1] : 0, // RIGHT
                };

                var directions = new uint[] { TOP, BOTTOM, LEFT, RIGHT };
                directions[0] = centerFlow;
                for (uint i = 0; i < 3; ++i)
                {
                    directions[i + 1] = i < centerFlow ? i : i + 1;
                }

                if (centerDrain == 0)
                {
                    if (centerSpread < 200)
                    {
                        while (true)
                        {
                            bool changed = false;
                            for (uint i = 0; i < 4 && centerLevel > 0; ++i)
                            {
                                uint value = values[directions[i]];
                                uint valueLevel = value & LEVEL_MASK;
                                bool valueIsSource = (value & SOURCE_MASK) != 0;
                                bool valueIsWall = (value & WALL_MASK) != 0;
                                uint valueDrain = (value & DRAIN_MASK) >> 12;

                                if (!valueIsWall && !valueIsSource && valueLevel < centerLevel && valueDrain == 0)
                                {
                                    changed = true;

                                    uint delta = (uint)Mathf.Min(Mathf.Min(5, LEVEL_MASK - (int)valueLevel), (int)centerLevel);
                                    valueLevel += delta;

                                    if (!centerIsSource)
                                    {
                                        centerLevel -= delta;
                                    }

                                    values[directions[i]] = (directions[i] << 10) | (valueLevel & LEVEL_MASK);
                                }
                            }

                            if (!changed)
                            {
                                break;
                            }
                        }

                        if (centerIsSource)
                        {
                            centerSpread++;
                        }
                    }
                    else
                    {
                        centerSpread = 0;
                        for (uint i = 0; i < 4; ++i)
                        {
                            uint value = values[directions[i]];

                            if ((value & (DRAIN_MASK | WALL_MASK | SOURCE_MASK)) == 0)
                            {
                                values[directions[i]] = (directions[i] << 10) | SOURCE_MASK | LEVEL_MASK;
                            }
                        }
                    }
                }
                else
                {
                    if (centerSpread < 200)
                    {
                        if (centerLevel > 0)
                        {
                            centerLevel -= (uint)Mathf.Min((int)centerValue, 1);
                        } else
                        {
                            centerLevel = 0;
                        }

                        for (uint i = 0; i < 4; ++i)
                        {
                            uint value = values[directions[i]];
                            uint valueLevel = value & LEVEL_MASK;
                            bool valueIsWall = (value & WALL_MASK) != 0;

                            if (!valueIsWall)
                            {
                                uint delta = (uint)Mathf.Min((int)valueLevel, 1);
                                //value -= delta;

                                values[directions[i]] = value;
                            }
                        }

                        centerSpread++;
                    }
                    else
                    {
                        centerSpread = 0;

                        if (centerDrain > 1)
                        {
                            for (uint i = 0; i < 4; ++i)
                            {
                                uint value = values[directions[i]];
                                uint valueLevel = value & LEVEL_MASK;
                                bool valueIsWall = (value & WALL_MASK) != 0;
                                uint valueDrain = (value & DRAIN_MASK) >> 12;

                                if (!valueIsWall && valueDrain < centerDrain - 1)
                                {
                                    values[directions[i]] = (centerDrain - 1) << 12 | valueLevel;
                                }
                            }
                        }

                        centerDrain--;
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

                if (centerIsSource)
                {
                    _waterMap[index] = ((centerSpread << 16) & SPREAD_MASK) | SOURCE_MASK | LEVEL_MASK;
                }
                else
                {
                    _waterMap[index] = ((centerSpread << 16) & SPREAD_MASK) | ((centerDrain << 12) & DRAIN_MASK) | ((centerFlow << 10) & FLOW_MASK) | centerLevel;
                }
            }
        }

        _cellsBuffer.SetData(_waterMap);
    }
}
