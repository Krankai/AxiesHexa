using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reference: https://www.redblobgames.com/grids/hexagons
public class HexCoordinates : MonoBehaviour
{
    private float tileOffsetX = 0.5f, tileOffsetZ = 0.866f;

    [Header("Coordinates")]
    [SerializeField]
    private Vector3Int tileOffset;
    [SerializeField]
    private Vector3 tilePosition;
    

    public Vector3Int GetHexCoordinates() => tileOffset;
    public Vector3 GetTilePosition() => tilePosition;

    public void SetHexCoordinates(Vector3Int hexCoordinates)
    {
        tileOffset = hexCoordinates;
        tilePosition = ConvertToPosition(hexCoordinates);

        //transform.Translate(tilePosition);
    }

    void Awake()
    {
        UpdateScaleOffset();

        tileOffset = ConvertToOffsetCoordinates(transform.position);
        tilePosition = ConvertToPosition(tileOffset);
    }

    private Vector3Int ConvertToOffsetCoordinates(Vector3 position)
    {
        UpdateScaleOffset();

        int ratioR = Mathf.RoundToInt(transform.position.z / tileOffsetZ);

        int r = -1 * ratioR;
        int q = Mathf.RoundToInt(transform.position.x / (2 * tileOffsetX) + ratioR * 0.5f);
        int s = 0 - q - r;

        return new Vector3Int(q, s, r);
    }

    private Vector3 ConvertToPosition(Vector3Int offsetCoordinates)
    {
        UpdateScaleOffset();
        
        float y = 0f;   // default
        float z = -1f * offsetCoordinates.z /* r */ * tileOffsetZ;
        float x = (offsetCoordinates.x /* q */ + 0.5f * offsetCoordinates.z /* r */) * 2 * tileOffsetX;

        return new Vector3(x, y, z);
    }

    private void UpdateScaleOffset()
    {
        tileOffsetX *= transform.localScale.x;
        tileOffsetZ *= transform.localScale.z;
    }
}
