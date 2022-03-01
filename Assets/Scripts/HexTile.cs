using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HexTile : MonoBehaviour
{
    private HexCoordinates hexCoordinates;

    public Vector3Int HexCoords => hexCoordinates.GetHexCoordinates();
    public Vector3 TilePosition => hexCoordinates.GetTilePosition();

    void Awake()
    {
        hexCoordinates = GetComponent<HexCoordinates>();
    }
}
