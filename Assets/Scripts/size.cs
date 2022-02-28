using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class size : MonoBehaviour
{
    public double offsetX = 0;
    public double width = 0;
    public double height = 0;

    public double tileSize = 0;

    private double radius = 0.5;

    // Start is called before the first frame update
    void Start()
    {
        tileSize = radius / Mathf.Cos(Mathf.PI * 30 / 180);
        width = Mathf.Sin(Mathf.PI * 60 / 180) * tileSize;
        height = 2 * tileSize;

        offsetX = 1.5 * tileSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
