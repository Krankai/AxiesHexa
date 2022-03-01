using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;
    public int ring = 2;

    private HexGrid grid;
    private int minRing = 4;

    void Awake()
    {
        grid = GetComponent<HexGrid>();
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
        Instantiate(tilePrefab, HexGrid.center, Quaternion.identity);

        // Rings
        for (int i = 1, limit = ring > minRing ? ring : minRing; i <= limit; ++i)
        {
            foreach (Vector3Int hexCoordinates in grid.GetHexesOnRing(i))
            {
                GameObject tileObject = Instantiate(tilePrefab, HexGrid.center, Quaternion.identity);
                HexCoordinates hexCoord = tileObject.GetComponent<HexCoordinates>();
                hexCoord.SetHexCoordinates(hexCoordinates);
            }
        }

        // Save all tiles into grid
        grid.UpdateTiles();

        return true;
    }
}
