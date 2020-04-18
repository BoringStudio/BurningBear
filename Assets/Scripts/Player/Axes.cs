using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axes : MonoBehaviour
{
    public float Horizontal {
        get;
        private set;
    }
    public float Vertical {
        get;
        private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
		Vertical = Input.GetAxis("Vertical");
    }
}
