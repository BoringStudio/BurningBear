using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SinnerAttachController : Attachable
{
    private Collider _collider = null;

    void Awake()
    {
        _collider = _collider ?? GetComponent<Collider>();

        Assert.IsNotNull(_collider, "[Sinner]: Collider is null");
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (!isAttached)
        {
            return;
        }
        if (attachmentTarget == null)
        {
            Debug.LogError("Grabbed origin is null");
            return;
        }

        transform.position = attachmentTarget.transform.position;
    }

    override protected bool AttachImpl(GameObject target)
    {
        _collider.enabled = false;
        return true;
    }

    override protected bool DeattachImpl(Vector3 releasedPosition)
    {
        transform.position = releasedPosition;
        _collider.enabled = true;

        return true;
    }
}
