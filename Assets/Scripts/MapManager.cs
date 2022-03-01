using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;
    public int ring = 2;

    private HexGrid map;

    void Awake()
    {
        map = GetComponent<HexGrid>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // if (tilePrefab)
        // {
        //     Instantiate(tilePrefab, new Vector3Int(4, 0, 4), Quaternion.identity);
        // }

        GenerateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool GenerateTiles()
    {
        if (!tilePrefab) return false;

        // Center
        Instantiate(tilePrefab, new Vector3Int(0, 0, 0), Quaternion.identity);

        // ... Add more (hex) tiles here


        // Save all tiles into grid
        map.Init();

        return true;
    }
}
