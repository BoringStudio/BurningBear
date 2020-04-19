using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerBodyChanger : MonoBehaviour
{
    [SerializeField] private Sprite _spriteUp;
    [SerializeField] private Sprite _spriteDown;

    private SpriteRenderer _spriteRenderer = null;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = _spriteRenderer ?? GetComponent<SpriteRenderer>();

        Assert.IsNotNull(_spriteRenderer, "[PlayerBodyChanger]: Sprite renderer is null");
        Assert.IsNotNull(_spriteUp, "[PlayerBodyChanger]: Sprite up is null");
        Assert.IsNotNull(_spriteDown, "[PlayerBodyChanger]: Sprite down is null");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToUp()
    {
        _spriteRenderer.sprite = _spriteUp;
    }

    public void ToDown()
    {
        _spriteRenderer.sprite = _spriteDown;
    }
}
