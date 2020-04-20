using UnityEngine;
using UnityEngine.UI;

public class SphereIndicator : MonoBehaviour
{
    public Sprite[] levelSprites;
    public Image image;

    public void SetFilled(float current, float outOf)
    {
        var level = (int)(levelSprites.Length * current / outOf);

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
