using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BorderFiller : MonoBehaviour
{
    public Image tile;

    private Vector2Int _resolution;

    void Awake()
    {
        UpdateResolution();
        UpdateTiles();
    }

    void Update()
    {
        if (_resolution.x != Screen.width || _resolution.y != Screen.height)
        {
            var recreate = _resolution.x < Screen.width;
            UpdateResolution();

            if (recreate)
            {
                UpdateTiles();
            }
        }
    }

    void UpdateTiles()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var count = _resolution.x / (int)tile.rectTransform.sizeDelta.x + 2;

        var offset = Vector3.left * _resolution.x / 2.0f;
        for (var i = 0; i < count; ++i)
        {
            var spawned = Instantiate(tile, transform);
            spawned.rectTransform.localPosition = offset + Vector3.right * (tile.rectTransform.sizeDelta.x - 2.0f) * i;
        }
    }

    void UpdateResolution()
    {
        _resolution.x = Screen.width;
        _resolution.y = Screen.height;
    }
}
