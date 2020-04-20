using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SphereIndicator : MonoBehaviour
{
    public Sprite[] levelSprites;
    public Image image;

    void Update()
    {

    }

    public void SetFilled(int current, int outOf)
    {
        var level = (int)(levelSprites.Length * (float)current / (float)outOf);

        if (level == 0)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            image.sprite = levelSprites[Mathf.Min(level - 1, levelSprites.Length - 1)];
        }
    }
}
