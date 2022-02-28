using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    const float offsetX = 0.5f;
    const float offsetZ = 0.866f;

    public int q = 0, s = 0, r = 0;

    // Start is called before the first frame update
    void Start()
    {
        int ratioR = Mathf.RoundToInt(transform.position.z / offsetZ);

        r = -1 * ratioR;
        q = Mathf.RoundToInt(transform.position.x + ratioR * offsetX);
        s = 0 - q - r;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
