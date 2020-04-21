using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CoalAttachHandler : Attachable
{
    private Collider _collider = null;
    private Coal _coal = null;

    void Awake()
    {
        _collider = _collider ?? GetComponent<Collider>();
        _coal = _coal ?? GetComponent<Coal>();

        Assert.IsNotNull(_collider, "[CoalAttachHandler]: Collider is null");
        Assert.IsNotNull(_coal, "[CoalAttachHandler]: Coal is null");
    }

    void LateUpdate()
    {
        if (isAttached)
        {
            transform.position = attachmentTarget.transform.position;
        }
    }

    override protected bool OnAttach(GameObject target)
    {
        if (_coal.AsSpawnable().spawner != null)
        {
            _coal.AsSpawnable().spawner.FreeCoalCore();
            _coal.AsSpawnable().spawner = null;
        }
        
        _collider.enabled = false;
        return true;
    }

    override protected bool OnDeattach(Vector3 releasedPosition)
    {
        transform.position = releasedPosition;
        _collider.enabled = true;
        return true;
    }
}
