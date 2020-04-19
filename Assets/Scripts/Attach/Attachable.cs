using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Attachable
{
    void Attach(Transform origin);
    void Deattach(Vector3 releasePosition);
}
