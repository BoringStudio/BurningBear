﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SinnerAttachHandler : Attachable
{
    private Collider _collider = null;

    void Awake()
    {
        _collider = _collider ?? GetComponent<Collider>();

        Assert.IsNotNull(_collider, "[Sinner]: Collider is null");
    }

    void LateUpdate()
    {
        if (isAttached)
        {
            transform.position = attachmentTarget.transform.position;
        }
    }

    protected override bool OnAttach(GameObject target)
    {
        _collider.enabled = false;
        return true;
    }

    protected override bool OnDeattach(Vector3 releasedPosition)
    {
        transform.position = releasedPosition;
        _collider.enabled = true;
        return true;
    }
}
