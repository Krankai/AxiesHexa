using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://www.redblobgames.com/grids/hexagons
public class HexCoordinates : MonoBehaviour
{
    private static float offsetX = 0.5f, offsetZ = 0.866f;

    [Header("Offset Coordinates")]
    [SerializeField]
    private Vector3Int offsetCoordinates;

    [Header("Tile Position")]
    [SerializeField]
    private Vector3 hexTilePosition;

    public Vector3Int GetHexCoordinates() => offsetCoordinates;
    public Vector3 GetTilePosition() => hexTilePosition;

    void Awake()
    {
        offsetCoordinates = ConvertToOffsetCoordinates(transform.position);
        hexTilePosition = ConvertToPosition(offsetCoordinates);
    }

    private Vector3Int ConvertToOffsetCoordinates(Vector3 position)
    {    
        int ratioR = Mathf.RoundToInt(transform.position.z / offsetZ);

        int r = -1 * ratioR;
        int q = Mathf.RoundToInt(transform.position.x + ratioR * offsetX);
        int s = 0 - q - r;

        return new Vector3Int(q, s, r);
    }

    private Vector3 ConvertToPosition(Vector3Int offsetCoordinates)
    {
        float y = 0f;   // default
        float z = -1f * offsetCoordinates.z /* r */ * offsetZ;
        float x = offsetCoordinates.x /* q */ + offsetCoordinates.z /* r */ * offsetX;

        return new Vector3(x, y, z);
    }
}
