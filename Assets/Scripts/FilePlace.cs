using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilePlace : MonoBehaviour
{
    public GameObject[] fireSprites;

    public float height = 3.0f;

    private byte[][] KERNEL = new byte[][]
    {
             new byte[] { 2, 2, },
            new byte[] { 2, 2, 2, },
          new byte[] { 2, 2, 2, 2, },
         new byte[] { 2, 2, 2, 2, 2, },
        new byte[] {   2, 2, 2, 2,  },
         new byte[] { 2, 2, 2, 2, 2, },
           new byte[] { 2, 2, 2, 2, },
            new byte[] { 2, 2, 2, },
              new byte[] { 2, 2, },
    };

    void Start()
    {
        var origin = transform.position - Vector3.forward * height / 2 + Vector3.right * 0.5f;
        var verticalStep = Vector3.forward * height / KERNEL.Length;

        for (int y = 0; y < KERNEL.Length; ++y)
        {
            for (int x = 0; x < KERNEL[y].Length; ++x)
            {
                var localOrigin = origin + verticalStep * y + Vector3.right * x - KERNEL[y].Length * Vector3.right / 2.0f;
                Instantiate(fireSprites[KERNEL[y][x]], localOrigin, Quaternion.Euler(60, 0, 0), transform);

            }
        }
    }

    void Update()
    {
        
    }
}
