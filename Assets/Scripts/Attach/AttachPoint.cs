using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Attachable attachedObject = null;

    public void AttachObject(Attachable toAttach, GameObject to) {
        toAttach.Attach(to);
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
