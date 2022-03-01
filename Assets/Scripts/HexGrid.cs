using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://www.redblobgames.com/grids/hexagons
public class HexGrid : MonoBehaviour
{
    Dictionary<Vector3Int, HexTile> hexTileDict = new Dictionary<Vector3Int, HexTile>();
    Dictionary<Vector3Int, List<Vector3Int>> hexTileNeightboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();

    // Start is called before the first frame update
    void Start()
    {
        // foreach (HexTile hexTile in FindObjectsOfType<HexTile>())
        // {
        //     hexTileDict[hexTile.HexCoords] = hexTile;
        // }

        // =====
        // TEST neighbours
        // List<Vector3Int> neighbours = GetNeighbours(new Vector3Int(0, 0, 0));
        // foreach (Vector3Int neighbour in neighbours)
        // {
        //     Debug.Log(neighbour);
        // }
        // =====
    }

    public HexTile GetTileAt(Vector3Int hexCoordinates)
    {
        HexTile hex = null;
        hexTileDict.TryGetValue(hexCoordinates, out hex);
        return hex;
    }

    public List<Vector3Int> GetNeighbours(Vector3Int hexCoordinates)
    {
        // Invalid hex tile
        if (!hexTileDict.ContainsKey(hexCoordinates))
        {
            return new List<Vector3Int>();
        }

        // Already exist
        if (hexTileNeightboursDict.ContainsKey(hexCoordinates))
        {
            return hexTileNeightboursDict[hexCoordinates];
        }

        // Generate and save neighbours
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (Vector3Int direction in Direction.GetDirectionList())
        {
            Vector3Int tile = hexCoordinates + direction;
            if (hexTileDict.ContainsKey(tile))
            {
                neighbours.Add(tile);
            }
        }
        hexTileNeightboursDict.Add(hexCoordinates, neighbours);

        return hexTileNeightboursDict[hexCoordinates];
    }

    public void Init()
    {
        foreach (HexTile hexTile in FindObjectsOfType<HexTile>())
        {
            hexTileDict[hexTile.HexCoords] = hexTile;

            // TEST
            Debug.Log(hexTile.HexCoords);
        }
    }
}

public static class Direction
{
    public static List<Vector3Int> GetDirectionList() => directions;

    private static List<Vector3Int> directions = new List<Vector3Int>
    {
        // Format: q s r (axiel/cube coordinates)
        new Vector3Int(1, -1, 0),       // East
        new Vector3Int(-1, 1, 0),       // West
        new Vector3Int(1, 0, -1),       // North East
        new Vector3Int(-1, 0, 1),       // South West
        new Vector3Int(0, 1, -1),       // North West
        new Vector3Int(0, -1, 1),       // South East  
    };
}
