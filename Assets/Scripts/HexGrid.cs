using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://www.redblobgames.com/grids/hexagons
public class HexGrid : MonoBehaviour
{
    Dictionary<Vector3Int, HexTile> hexTileDict = new Dictionary<Vector3Int, HexTile>();
    Dictionary<Vector3Int, List<Vector3Int>> hexTileNeightboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();
    Dictionary<int, List<Vector3Int>> hexTileRingDict = new Dictionary<int, List<Vector3Int>>();
    
    [HideInInspector]
    public Dictionary<HexTile, SpineAxieModel> axiesDict = new Dictionary<HexTile, SpineAxieModel>();

    public static Vector3Int center = Vector3Int.zero;


    public void ClearGrid()
    {
        hexTileDict.Clear();
        hexTileNeightboursDict.Clear();
        hexTileRingDict.Clear();
        axiesDict.Clear();
    }

    #region Grid
    public int GetCountTiles() => hexTileDict.Count;
    public HexTile GetTileAt(Vector3Int hexCoordinates)
    {
        HexTile hex = null;
        hexTileDict.TryGetValue(hexCoordinates, out hex);
        return hex;
    }

    public bool AddTile(HexTile tile)
    {
        if (hexTileDict.ContainsKey(tile.HexCoords)) return false;
        if (tile == null) return false;

        hexTileDict[tile.HexCoords] = tile;
        return true;
    }

    public GameObject GenerateTileAt(Vector3Int hexCoordinates, GameObject prefab, float scaleFactor)
    {
        // Check for existing tile at the specified position
        HexTile hexTile = GetTileAt(hexCoordinates);
        if (hexTile) return hexTile.gameObject;

        // Generate new tile at the specified position
        if (prefab == null) return null;

        GameObject newTileObject = Instantiate(prefab, /*hexCoordinates*/ center, Quaternion.identity);
        newTileObject.transform.localScale *= scaleFactor;

        hexTile = newTileObject.GetComponent<HexTile>();
        hexTile.SnapToPosition(hexCoordinates);
        hexTileDict[hexCoordinates] = hexTile;

        return newTileObject;
    }

    public List<Vector3Int> GetNeighbourTiles(Vector3Int hexCoordinates)
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

    public List<Vector3Int> GetTilesOnRing(int radius)
    {
        // NOTE: assume center is at (0, 0, 0)

        // Already exist
        if (hexTileRingDict.ContainsKey(radius))
        {
            return hexTileRingDict[radius];
        }

        // Generate and save hexes on the specified ring
        List<Vector3Int> tiles = new List<Vector3Int>();

        // Pick tile on the ring along Sounth-West direction as the first one
        Vector3Int tile = center + Direction.GetDirectionList()[4] /* (-1, 0, 1) */ * radius;

        // Follow counter-clockwise direction to get all other tiles on the ring
        foreach (Vector3Int direction in Direction.GetDirectionList())
        {
            for (int i = 0; i < radius; ++i)
            {
                tiles.Add(tile);
                tile = tile + direction;
            }
        }
        hexTileRingDict.Add(radius, tiles);

        return hexTileRingDict[radius];
    }

    public Dictionary<Vector3Int, HexTile>.ValueCollection GetAllTiles()
    {
        return hexTileDict.Values;
    }

    public void SyncTiles()
    {
        foreach (HexTile hexTile in FindObjectsOfType<HexTile>())
        {
            hexTileDict[hexTile.HexCoords] = hexTile;
        }
    }
    #endregion

    #region Axies
    public int GetCountAxies() => axiesDict.Count;

    public SpineAxieModel GetAxieAt(HexTile tile)
    {
        SpineAxieModel axieModel = null;
        axiesDict.TryGetValue(tile, out axieModel);
        return axieModel;
    }

    public void RemoveAxieAt(HexTile tile, SpineAxieModel confirmModel = null)
    {
        if (confirmModel != null)
        {
            SpineAxieModel saveModel = GetAxieAt(tile);
            if (saveModel == null) return;
            if (saveModel != confirmModel) return;      // not match axie model (?)
        }

        axiesDict.Remove(tile);
    }

    public GameObject GenerateAxieAt(HexTile tile, GameObject prefab)
    {
        // If the tile is already occuppied, return the corresponding axie
        SpineAxieModel axieModel = GetAxieAt(tile);
        if (axieModel) return axieModel.gameObject;

        // Generate a new Axie at the specified tile
        if (prefab == null) return null;

        GameObject axieObject = Instantiate(prefab, tile.GetPositonForCharacter(), prefab.transform.rotation);
        axieModel = axieObject.GetComponent<SpineAxieModel>();
        axiesDict[tile] = axieModel;

        // Turn to appropriate direction
        if (tile.TilePosition.x < 0)
        {
            if (tile.type == TileType.Defense)
            {
                //axieModel.Turn(FacingDirection.Left);
                axieModel.facingLeft = true;
            }
            else if (tile.type == TileType.Attack)
            {
                //axieModel.Turn(FacingDirection.Right);
                axieModel.facingLeft = false;
            }
        }
        else
        {
            if (tile.type == TileType.Defense)
            {
                //axieModel.Turn(FacingDirection.Right);
                axieModel.facingLeft = false;
            }
            else if (tile.type == TileType.Attack)
            {
                //axieModel.Turn(FacingDirection.Left);
                axieModel.facingLeft = true;
            }
        }

        return axieObject;
    }

    public bool PutAxieAt(HexTile tile, SpineAxieModel axieModel)
    {
        if (axiesDict.ContainsKey(tile)) return false;

        axiesDict[tile] = axieModel;
        return true;
    }
    #endregion

    public void CheckLog()
    {
        // TEST
        foreach (KeyValuePair<Vector3Int, HexTile> tile in hexTileDict)
        {
            Debug.Log("{ Key: " + tile.Key + ", Value: " + tile.Value);
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
        new Vector3Int(1, 0, -1),       // North East
        new Vector3Int(0, 1, -1),       // North West
        new Vector3Int(-1, 1, 0),       // West
        new Vector3Int(-1, 0, 1),       // South West
        new Vector3Int(0, -1, 1),       // South East
    };
}
