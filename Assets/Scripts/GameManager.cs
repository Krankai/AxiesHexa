using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HexGrid hexGrid;

    #region Ground
    [Header("Ground Resources")]
    public GameObject tilesGroup;
    [SerializeField]
    private GameObject defenseTilePrefab;
    [SerializeField]
    private GameObject attackTilePrefab;
    [SerializeField]
    private GameObject barrierTilePrefab;
    public float scaleFactor = 1f;
    public int numberOfRings = 1;

    private int minRings = 1;
    #endregion

    #region Characters
    [Header("Character Resources")]
    public GameObject charactersGroup;
    [SerializeField]
    private GameObject defenseAxiePrefab;
    [SerializeField]
    private GameObject attackAxiePrefab;
    #endregion

    [Header("Test")]
    public SpineAxieModel attacker;
    public SpineAxieModel defender;

    Dictionary<TeamSet, TeamSet> battlePairs = new Dictionary<TeamSet, TeamSet>();
    Dictionary<TeamSet, HexTile> moveList = new Dictionary<TeamSet, HexTile>();

    void Awake()
    {
        if (hexGrid == null)
        {
            hexGrid = GetComponentInChildren<HexGrid>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // if (tilePrefab)
        // {
        //     Instantiate(tilePrefab, new Vector3Int(4, 0, 4), Quaternion.identity);
        // }

        GenerateTiles();
        GenerateAxies();
        //grid.CheckLog();

        // GameObject testObject = Instantiate(attackAxiePrefab, new Vector3(0, 1.05f, 0), attackAxiePrefab.transform.rotation);
        // if (charactersGroup)
        // {
        //     testObject.transform.parent = charactersGroup.transform;
        // }
        
        TeamSet attackTeam = new TeamSet(attacker, null);
        TeamSet defendTeam = new TeamSet(defender, null);

        battlePairs[attackTeam] = defendTeam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool GenerateTiles()
    {
        if (hexGrid == null) return false;

        // Generate and add center tile (= defense tile)
        if (defenseTilePrefab)
        {
            GameObject tileObject = hexGrid.GenerateTileAt(HexGrid.center, defenseTilePrefab, scaleFactor);
            if (tilesGroup)
            {
                tileObject.transform.parent = tilesGroup.transform;
            }
        }

        int numGeneratedRings = numberOfRings > minRings ? numberOfRings : minRings;

        // Determine number of rings for each prefab type: defense, attack, barrier
        int numBarrierRings = 1;     // default;
        int numDefenseRings = (int)((numGeneratedRings - numBarrierRings) / 2);
        int numAttackRings = numGeneratedRings - numBarrierRings - numDefenseRings;

        // Add tiles on each ring
        for (int i = 1; i <= numGeneratedRings; ++i)
        {
            // Decide prefab to be used
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
            
            // Generate and add all corresponding tiles
            foreach (Vector3Int hexCoordinates in hexGrid.GetTilesOnRing(i))
            {
                if (usedTilePrefab)
                {
                    GameObject tileObject = hexGrid.GenerateTileAt(hexCoordinates, usedTilePrefab, scaleFactor);
                    if (tilesGroup)
                    {
                        tileObject.transform.parent = tilesGroup.transform;
                    }
                }
            }
        }

        return true;
    }

    private bool GenerateAxies()
    {
        if (hexGrid == null) return false;

        Dictionary<Vector3Int, HexTile>.ValueCollection values = hexGrid.GetAllTiles();
        foreach (HexTile tile in values)
        {
            GameObject axieObject = null;

            if (tile.type == TileType.Attack)
            {
                axieObject = hexGrid.GenerateAxieAt(tile, attackAxiePrefab);
            }
            else if (tile.type == TileType.Defense)
            {
                axieObject = hexGrid.GenerateAxieAt(tile, defenseAxiePrefab);
            }

            if (axieObject && charactersGroup)
            {
                axieObject.transform.parent = charactersGroup.transform;
            }
        }

        return true;
    }

    public void SimulateAttack()
    {
        foreach (KeyValuePair<TeamSet, TeamSet> pair in battlePairs)
        {
            var attackerSet = pair.Key;
            var defenderSet = pair.Value;

            if (attackerSet.model == null) continue;
            if (defenderSet.model == null) continue;

            // Prepare for battle
            attackerSet.model.Prepare();
            defenderSet.model.Prepare();

            // Batte
            attackerSet.model.TryAttack(defenderSet.model);
            defenderSet.model.TryAttack(attackerSet.model);

            // Aftermath: update corresponding data structures and cleanup death axies
            if (attackerSet.model.IsDeath && attackerSet.tile != null)
            {
                hexGrid.RemoveAxieAt(attackerSet.tile, attackerSet.model);
                Destroy(attackerSet.model.gameObject);
            }

            if (defenderSet.model.IsDeath && defenderSet.tile != null)
            {
                hexGrid.RemoveAxieAt(defenderSet.tile, defenderSet.model);
                Destroy(defenderSet.model.gameObject);
            }
        }
    }

    private void SimulateMove()
    {
        foreach (KeyValuePair<TeamSet, HexTile> pair in moveList)
        {
            var axieSet = pair.Key;
            var destinationTile = pair.Value;

            if (axieSet.model == null) continue;

            // Move to tile
            axieSet.model.TryMove(destinationTile.TilePosition);

            // Update corresponding data structures
            hexGrid.PutAxieAt(destinationTile, axieSet.model);
            if (axieSet.tile != null)
            {
                hexGrid.RemoveAxieAt(axieSet.tile, axieSet.model);
            }
        }
    }
}

public class TeamSet
{
    public SpineAxieModel model;
    public HexTile tile;

    public TeamSet(SpineAxieModel model, HexTile tile)
    {
        this.model = model;
        this.tile = tile;
    }
}
