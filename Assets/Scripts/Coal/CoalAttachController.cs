using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CoalAttachController : Attachable
{
    private Collider _collider = null;

    void Awake() {
        _collider = _collider ?? GetComponent<Collider>();

        Assert.IsNotNull(_collider, "[Coal]: Collider is null");
    }

    override protected bool AttachImpl(GameObject target) {
        _collider.enabled = false;
        return true;
    }

    override protected bool DeattachImpl(Vector3 releasedPosition) {
        transform.position = releasedPosition;
        _collider.enabled = true;
        return true;
    }
}
