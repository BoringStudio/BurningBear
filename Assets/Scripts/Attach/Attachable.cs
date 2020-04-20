using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public bool isAttached
    {
        get => attachmentTarget != null;
    }

    public GameObject attachmentTarget = null;

    public void Attach(GameObject target) {
        bool success = OnAttach(target);
        if (success) {
            Debug.Log("Attached successfully");
            attachmentTarget = target;
        }
    }

    public void Deattach(Vector3 deattachPosition) {
        bool success = OnDeattach(deattachPosition);
        if (success) {
            Debug.Log("Deattached successfully");
            attachmentTarget = null;
        }
    }

    
    protected virtual bool OnAttach(GameObject target) { return true; }
    protected virtual bool OnDeattach(Vector3 releasePosition) { return true; }
}
