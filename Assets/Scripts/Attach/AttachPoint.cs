using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Attachable attachableObject = null;

    public void AttachObject(Attachable attachable, Transform to) {
        attachable.Attach(to);
        attachableObject = attachable;
    }

    public void DeattachObject(Vector3 to) {
        attachableObject.Deattach(to);
        attachableObject = null;
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
