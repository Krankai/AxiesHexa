using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Object")]
    public GameObject tilesGroup;
    [SerializeField]
    private GameObject defenseTilePrefab;
    [SerializeField]
    private GameObject attackTilePrefab;
    [SerializeField]
    private GameObject barrierTilePrefab;

    [Header("Attributes")]
    public float tileScaleFactor = 1f;
    public int numberOfRings = 2;

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

        //GenerateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool GenerateTiles()
    {
        // Center - use defense tile (always)
        if (defenseTilePrefab)
        {
            GameObject centerTileObject = Instantiate(defenseTilePrefab, HexGrid.center, Quaternion.identity);
            centerTileObject.transform.localScale *= tileScaleFactor;      // apply scale

            // Add to group
            if (tilesGroup)
            {
                centerTileObject.transform.parent = tilesGroup.transform;
            }
        }

        int numGeneratedRings = numberOfRings > minRing ? numberOfRings : minRing;

        // Determine number of rings for each prefab type: defense, attack, barrier
        int numBarrierRings = 1;     // default;
        int numDefenseRings = (int)((numGeneratedRings - numBarrierRings) / 2);
        int numAttackRings = numGeneratedRings - numBarrierRings - numDefenseRings;

        // Rings
        for (int i = 1; i <= numGeneratedRings; ++i)
        {
            // Decide which tile prefab to be used for the current ring
            GameObject usedTilePrefab = barrierTilePrefab;
            if (numDefenseRings > 0)
            {
                usedTilePrefab = defenseTilePrefab;
                --numDefenseRings;
            }
            else if (numBarrierRings > 0)
            {
                //usedTile = barrierTilePrefab;
                --numBarrierRings;
            }
            else
            {
                usedTilePrefab = attackTilePrefab;
            }
            
            // Generate all tiles on the current ring
            foreach (Vector3Int hexCoordinates in grid.GetHexesOnRing(i))
            {
                if (usedTilePrefab)
                {
                    GameObject tileObject = Instantiate(usedTilePrefab, HexGrid.center, Quaternion.identity);
                    tileObject.transform.localScale *= tileScaleFactor;      // apply scale

                    HexCoordinates hexCoord = tileObject.GetComponent<HexCoordinates>();
                    hexCoord.SetHexCoordinates(hexCoordinates);

                    // Add to group
                    if (tilesGroup)
                    {
                        tileObject.transform.parent = tilesGroup.transform;
                    }
                }
            }
        }

        // Save all tiles into grid
        grid.UpdateTiles();

        return true;
    }
}
