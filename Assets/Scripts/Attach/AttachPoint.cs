using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public bool hasAttachedObject{
        get => attachedObject != null;
    }
    public Attachable attachedObject = null;

    public void AttachObject(Attachable toAttach) {
        toAttach.Attach(gameObject);
        attachedObject = toAttach;
    }

    public void DeattachObject(Vector3 deattachPosition) {
        attachedObject.Deattach(deattachPosition);
        attachedObject = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
