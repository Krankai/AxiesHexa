using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HexTile : MonoBehaviour
{
    public TileType type = TileType.Barrier;
    [SerializeField]
    private Vector3 characterPositionOffset = new Vector3(0f, 1.1f, -1.5f);

    private HexCoordinates hexCoordinates;

    public Vector3Int HexCoords => hexCoordinates.GetHexCoordinates();
    public Vector3 TilePosition => hexCoordinates.GetTilePosition();

    void Awake()
    {
        hexCoordinates = GetComponent<HexCoordinates>();
    }

    public void SnapToPosition(Vector3Int hexCoord)
    {
        hexCoordinates.SetHexCoordinates(hexCoord);
        transform.Translate(hexCoordinates.GetTilePosition());
    }

    public Vector3 GetPositonForCharacter()
    {
        return TilePosition + characterPositionOffset;
    }
}

public enum TileType
{
    Defense,
    Attack,
    Barrier
}
