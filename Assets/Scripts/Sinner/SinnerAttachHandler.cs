using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SinnerAttachHandler : Attachable
{
    private Sinner _sinner = null;
    private Collider _collider = null;
    private Shadow _shadow = null;
    private BaseSprite _baseSprite = null;

    void Awake()
    {
        _sinner = _sinner ?? GetComponent<Sinner>();
        _collider = _collider ?? GetComponent<Collider>();
        _shadow = _shadow ?? GetComponentInChildren<Shadow>();
        _baseSprite = _baseSprite ?? GetComponentInChildren<BaseSprite>();

        Assert.IsNotNull(_sinner, "[SinnerAttachHandler]: Sinner is null");
        Assert.IsNotNull(_collider, "[SinnerAttachHandler]: Collider is null");
        Assert.IsNotNull(_shadow, "[SinnerAttachHandler]: Shadow is null");
        Assert.IsNotNull(_baseSprite, "[SinnerAttachHandler]: Base sprite is null");
    }

    void LateUpdate()
    {
        if (isAttached)
        {
            transform.position = new Vector3(
                attachmentTarget.transform.position.x + 1.0f,
                attachmentTarget.transform.position.y + 0.3f,
                attachmentTarget.transform.position.z);
        }
    }

    protected override bool OnAttach(GameObject target)
    {
        if (_sinner.AsSpawnable().spawner != null)
        {
            _sinner.AsSpawnable().spawner.FreeSinner();
            _sinner.AsSpawnable().spawner = null;
        }
        
        _baseSprite.transform.rotation = Quaternion.Euler(
            _baseSprite.transform.rotation.eulerAngles.x, 
            _baseSprite.transform.rotation.eulerAngles.y, 
            90);

        _shadow.enabled = false;
        _collider.enabled = false;
        return true;
    }

    protected override bool OnDeattach(Vector3 releasedPosition)
    {
        transform.position = releasedPosition;
        
        _baseSprite.transform.rotation = Quaternion.Euler(
            _baseSprite.transform.rotation.eulerAngles.x, 
            _baseSprite.transform.rotation.eulerAngles.y, 
            0);

        _shadow.enabled = true;
        _collider.enabled = true;
        return true;
    }
}
