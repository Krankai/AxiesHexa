using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPool : MonoBehaviour
{
    Dictionary<HexTile, AxiesSpineModel> axiesDict = new Dictionary<HexTile, AxiesSpineModel>();

    public AxiesSpineModel GetAxiesAt(HexTile tile)
    {
        AxiesSpineModel axieModel = null;
        axiesDict.TryGetValue(tile, out axieModel);
        return axieModel;
    }

    public GameObject GenerateAxieAt(HexTile tile, GameObject prefab)
    {
        // If the tile is already occuppied, return the corresponding axie
        AxiesSpineModel axieModel = GetAxiesAt(tile);
        if (axieModel) return axieModel.gameObject;

        // Generate a new Axie at the specified tile
        if (prefab == null) return null;

        GameObject axieObject = Instantiate(prefab, tile.GetPositonForCharacter(), prefab.transform.rotation);
        axieModel = axieModel.GetComponent<AxiesSpineModel>();
        axiesDict[tile] = axieModel;

        return axieObject;
    }
}
