using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    const int minRings = 3;
    const int maxRings = 11;
    #endregion

    #region Characters
    [Header("Character Resources")]
    public GameObject charactersGroup;
    [SerializeField]
    private GameObject defenseAxiePrefab;
    [SerializeField]
    private GameObject attackAxiePrefab;
    #endregion

    #region Control System
    [Header("Control System")]
    public InputField ringInputField;
    public Slider ringSlider;

    public Button generateButton;
    public Button clearButton;
    public Button simulateButton;

    public PlaybackControls playbackControls;

    public GameOverScreenController gameoverController;

    public Text countdownText;

    public float delayGameOverMessage = 1;
    #endregion

    Dictionary<TeamSet, TeamSet> battlePairs = new Dictionary<TeamSet, TeamSet>();
    Dictionary<TeamSet, HexTile> moveList = new Dictionary<TeamSet, HexTile>();

    int countWaitingAxieAnimations = 0;     // number of axies that the manager is waiting for (to finish animation)
    int countDefenders;
    int countAttackers;
    int currentFlag = 1;        // to check: whether current axie/character was handled or not (avoid handling 1 char twice)

    bool isWaitingAxieAnimations;
    bool isGenerated;
    bool isSelectValue;
    bool isStarted;
    bool isFinished;

    public void OnDefenderDeath() => --countDefenders;
    public void OnAttackerDeath() => --countAttackers;
    public bool IsGameStarted() => isStarted;
    public bool IsObjectsGenerated() => isGenerated;

    public int i = 0; // test

    void Awake()
    {
        if (hexGrid == null) hexGrid = GetComponentInChildren<HexGrid>();
        if (playbackControls == null) playbackControls = GetComponentInChildren<PlaybackControls>();

        // Set min/max for slider
        if (ringSlider != null)
        {
            ringSlider.minValue = minRings;
            ringSlider.maxValue = maxRings;
        }

        isWaitingAxieAnimations = false;
        isGenerated = false;
        isSelectValue = false;
        isStarted = false;
        isFinished = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (generateButton != null) generateButton.interactable = false;
        if (simulateButton != null) simulateButton.interactable = false;

        if (countdownText != null) countdownText.gameObject.SetActive(false);

        if (playbackControls != null) playbackControls.gameObject.SetActive(false);

        if (gameoverController != null) gameoverController.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelectValue && generateButton != null) generateButton.interactable = !isGenerated;

        if (isStarted && !isFinished)
        {
            if (isWaitingAxieAnimations)
            {
                //Debug.Log("Current count: " + countWaitingAxieAnimations);
                if (countWaitingAxieAnimations <= 0)
                {
                    isWaitingAxieAnimations = false;
                    //Debug.Log("Can simulate next step...");
                }
            }
            else
            {
                AxieType winnerType = AxieType.Default;
                if (HaveWinner(out winnerType))
                {
                    isFinished = true;

                    // Disable playback controls
                    if (playbackControls) playbackControls.ToggleButtons(false);

                    // Show gameover message
                    StartCoroutine(ShowGameOverMessage(winnerType));
                }
                else
                {
                    SimulateStep();
                }
            }
        }
    }

    private bool GenerateTiles()
    {
        if (numberOfRings <= 0 || scaleFactor <= 0) return false;
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
        if (hexGrid.GetCountTiles() <= 0) return false;

        Dictionary<Vector3Int, HexTile>.ValueCollection values = hexGrid.GetAllTiles();
        foreach (HexTile tile in values)
        {
            GameObject axieObject = null;

            if (tile.type == TileType.Attack)
            {
                axieObject = hexGrid.GenerateAxieAt(tile, attackAxiePrefab);
                if (axieObject != null) ++countAttackers;
            }
            else if (tile.type == TileType.Defense)
            {
                axieObject = hexGrid.GenerateAxieAt(tile, defenseAxiePrefab);
                if (axieObject != null) ++countDefenders;
            }

            if (axieObject && charactersGroup)
            {
                axieObject.transform.parent = charactersGroup.transform;
            }
        }

        return true;
    }

    public void OnFinishAxieAnimation()
    {
        --countWaitingAxieAnimations;
    }

    public bool HaveWinner(out AxieType winnerType)
    {
        winnerType = AxieType.Default;
        if (!isStarted) return false;

        bool haveWinner = countAttackers <= 0 || countDefenders <= 0;
        if (haveWinner)
        {
            if (countAttackers <= 0 && countDefenders <= 0)
            {
                winnerType = AxieType.Both;
            }
            else
            {
                winnerType = (countAttackers <= 0) ? AxieType.Defense : AxieType.Attack;
            }
        }

        return haveWinner;
    }

    IEnumerator ShowGameOverMessage(AxieType winnerType)
    {
        if (winnerType == AxieType.Default) yield break;
        if (gameoverController == null) yield break;

        yield return new WaitForSeconds(delayGameOverMessage);

        if (winnerType == AxieType.Defense)
        {
            gameoverController.OnDefendersWin();
        }
        else if (winnerType == AxieType.Attack)
        {
            gameoverController.OnAttackersWin();
        }
        else if (winnerType == AxieType.Both)
        {
            gameoverController.OnDraw();
        }

        yield return null;
    }

    public async void SimulateStep()
    {
        // Debug.Log("New step " + i++);

        if (!isStarted || isFinished) return;
        if (hexGrid == null) return;
        if (hexGrid.GetCountTiles() <= 0 || hexGrid.GetCountAxies() <= 0) return;

        // Traverse through all axies on the field
        foreach (var pair in hexGrid.axiesDict)
        {
            HexTile currentTile = pair.Key;
            SpineAxieModel currentAxie = pair.Value;

            // If the current axie has already been handle, skip
            if (currentAxie.flag == currentFlag) continue;

            var neighbourHexCoordinates = hexGrid.GetNeighbourTiles(pair.Key.HexCoords);

            bool success = false;

            // 1. Check for possible attack
            foreach (var coordinate in neighbourHexCoordinates)
            {
                HexTile examineTile = hexGrid.GetTileAt(coordinate);
                if (examineTile == null) continue;                              // invalid tile

                SpineAxieModel examineAxie = hexGrid.GetAxieAt(examineTile);
                if (examineAxie == null) continue;                              // no axie

                if (examineAxie.flag == currentFlag) continue;                  // same flag means this axie has already been handled

                if (examineAxie.axieType == currentAxie.axieType) continue;     // cannot attack Axie with same type (defense / attack)

                // Can attack -> register both axies to handle battle later
                TeamSet attackerSet = new TeamSet(currentAxie, currentTile);
                TeamSet defenderSet = new TeamSet(examineAxie, examineTile);

                battlePairs.Add(attackerSet, defenderSet);

                // Set flag for both axies
                currentAxie.flag = currentFlag;
                examineAxie.flag = currentFlag;

                success = true;
                break;
            }

            if (success) continue;

            // 2. Check for possible moving (ONLY if attacking axies - defending axies cannot move)
            if (currentAxie.axieType == AxieType.Attack)
            {
                float currentDistanceToCenter = Mathf.Abs(currentTile.HexCoords.x) + Mathf.Abs(currentTile.HexCoords.y) + Mathf.Abs(currentTile.HexCoords.z);
                foreach (var coordinate in neighbourHexCoordinates)
                {
                    // Compromise #1: move towards the center instead of the closest enemy

                    HexTile examineTile = hexGrid.GetTileAt(coordinate);
                    if (examineTile == null) continue;                              // invalid tile

                    SpineAxieModel possibleAxie = hexGrid.GetAxieAt(examineTile);
                    if (possibleAxie != null) continue;                             // occupied hence cannot move into

                    float examineDistanceToCenter = Mathf.Abs(examineTile.HexCoords.x) + Mathf.Abs(examineTile.HexCoords.y) + Mathf.Abs(examineTile.HexCoords.z);
                    if (examineDistanceToCenter >= currentDistanceToCenter)         // if not closer to center, don't move
                    {
                        continue;
                    }

                    if (moveList.ContainsValue(examineTile)) continue;              // if the tile is already set as destination for another axie, skip

                    // Can move into -> register to handle moving later
                    TeamSet axieSet = new TeamSet(currentAxie, currentTile);
                    moveList.Add(axieSet, examineTile);

                    // Set flag for axie
                    currentAxie.flag = currentFlag;

                    success = true;
                    break;
                }
            }

            if (success) continue;

            // 3. Else, stay idling
            currentAxie.flag = currentFlag;
        }

        isWaitingAxieAnimations = true;

        // Update counter: number of axies to wait (for them to finish animation)
        countWaitingAxieAnimations = battlePairs.Count * 2 + moveList.Count;
        // Debug.Log("Count (" + i + "): " + countWaitingAxieAnimations);
        // Debug.Log("[" + i + "] Battle: " + battlePairs.Count * 2 + ", Move: " + moveList.Count);

        // Simulate battle
        SimulateAttack();

        // Simulate moving
        SimulateMove();

        // Change current flag (basically, set all axies into not-handle state)
        currentFlag = (currentFlag == 1) ? 0 : 1;
    }

    public void SimulateAttack()
    {
        foreach (KeyValuePair<TeamSet, TeamSet> pair in battlePairs)
        {
            var attackerSet = pair.Key;
            var defenderSet = pair.Value;

            if (attackerSet.model == null) continue;
            if (defenderSet.model == null) continue;

            if (attackerSet.tile)
            {
                attackerSet.model.opponentTile = defenderSet.tile.HexCoords;
            }
            if (defenderSet.tile)
            {
                defenderSet.model.opponentTile = attackerSet.tile.HexCoords;
            }

            // Turn both opponents towards each other
            bool isAttackerBehind = (attackerSet.model.gameObject.transform.position.x < defenderSet.model.gameObject.transform.position.x);
            if (attackerSet.model.facingLeft == defenderSet.model.facingLeft)   // same direction
            {
                bool isLeft = attackerSet.model.facingLeft;

                if (isLeft)
                {
                    if (isAttackerBehind) attackerSet.model.facingLeft = false;
                    else defenderSet.model.facingLeft = false;
                }
                else
                {
                    if (isAttackerBehind) defenderSet.model.facingLeft = true;
                    else attackerSet.model.facingLeft = true;
                }
            }
            else
            {
                if (isAttackerBehind && attackerSet.model.facingLeft == true)
                {
                    attackerSet.model.facingLeft = false;
                    defenderSet.model.facingLeft = true;
                }
                else if (!isAttackerBehind && defenderSet.model.facingLeft == true)
                {
                    attackerSet.model.facingLeft = true;
                    defenderSet.model.facingLeft = false;
                }
            }

            // Prepare for battle
            attackerSet.model.Prepare();
            defenderSet.model.Prepare();

            // Batte
            bool attackSuccess = attackerSet.model.TryAttack(defenderSet.model);
            bool counterSuccess = defenderSet.model.TryAttack(attackerSet.model);

            // Aftermath: update corresponding data structures and cleanup death axies
            if (attackSuccess && defenderSet.model.IsDeath && defenderSet.tile != null)
            {
                hexGrid.RemoveAxieAt(defenderSet.tile, defenderSet.model);

                // NOTE: only Destroy gameobject after finish "dying" animation
                //Destroy(defenderSet.model.gameObject);
            }

            if (counterSuccess && attackerSet.model.IsDeath && attackerSet.tile != null)
            {
                hexGrid.RemoveAxieAt(attackerSet.tile, attackerSet.model);

                // NOTE: only Destroy gameobject after finish "dying" animation
                //Destroy(attackerSet.model.gameObject);
            }
        }

        // Reset
        battlePairs.Clear();
    }

    public void SimulateMove()
    {
        foreach (KeyValuePair<TeamSet, HexTile> pair in moveList)
        {
            var axieSet = pair.Key;
            var destinationTile = pair.Value;

            if (axieSet.model == null) continue;

            // Move to tile
            bool moveSuccess = axieSet.model.TryMove(destinationTile.GetPositonForCharacter());
            if (moveSuccess)
            {
                // Update corresponding data structures
                hexGrid.PutAxieAt(destinationTile, axieSet.model);
                if (axieSet.tile != null)
                {
                    hexGrid.RemoveAxieAt(axieSet.tile, axieSet.model);
                }
            }
        }

        // Reset
        moveList.Clear();
    }

    public void OnValueChangedRingSlider(float value)
    {
        isSelectValue = true;
        numberOfRings = (int)value;

        // Update display value of corresponding input field
        if (ringInputField == null) return;
        ringInputField.text = ((int)value).ToString();
    }

    public void OnEditEndInputField(string value)
    {
        if (value.Length <= 0) value = "0";
        numberOfRings = Mathf.Clamp(int.Parse(value), minRings, maxRings);
        ringInputField.text = (Mathf.Clamp(int.Parse(value), minRings, maxRings)).ToString();
    }

    public void OnValueChangedInputField(string value)
    {
        isSelectValue = true;

        if (value.Length <= 0) value = "0";
        numberOfRings = Mathf.Clamp(int.Parse(value), minRings, maxRings);

        // Update value of slider
        if (ringSlider == null) return;
        ringSlider.value = (int)float.Parse(value);
    }

    public void OnClear()
    {
        // Reset camera
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null) cameraController.ResetOriginalCamera();

        // Clear object data
        if (tilesGroup != null)
        {
            foreach (Transform child in tilesGroup.transform)
            {
                Destroy(child.gameObject);
            }
        }
        if (charactersGroup != null)
        {
            foreach (Transform child in charactersGroup.transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (hexGrid != null) hexGrid.ClearGrid();
        isGenerated = false;
        countDefenders = countAttackers = 0;

        // Disable buttons
        if (simulateButton != null) simulateButton.interactable = false;
        if (generateButton != null) generateButton.interactable = true;
        isGenerated = false;
    }

    public void OnGenerate()
    {
        // Adjust camera(s) accordingly
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController)
        {
            cameraController.AdjustMainCamera(numberOfRings);
            cameraController.AdjustMiniMapcamera(numberOfRings);
        }

        // Generate game objects
        GenerateTiles();
        GenerateAxies();

        isGenerated = true;

        // Enable 'simulate' button
        if (simulateButton != null) simulateButton.interactable = true;
    }

    public void OnSimulate()
    {
        // Disable all input and corresponding button controls
        ToggleInitialInputAndButtonControls(false);

        // Enable playback control buttons
        if (playbackControls != null)
        {
            playbackControls.gameObject.SetActive(true);
            playbackControls.ToggleButtons(false);
        }

        // Reset camera
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.ResetOriginalCamera();
        }

        // Initiate countdown into simulation's start
        StartCoroutine(CountDownStartGame());
    }

    public void ReSetup()
    {
        OnClear();

        // Re-init fields
        isWaitingAxieAnimations = false;
        isGenerated = false;
        //isSelectValue = false;
        isStarted = false;
        isFinished = false;

        // Reset state of input and button controls
        ToggleInitialInputAndButtonControls(true);

        if (ringInputField != null) ringInputField.text = "";
        isSelectValue = false;

        if (generateButton != null) generateButton.interactable = false;
        if (simulateButton != null) simulateButton.interactable = false;

        // Hide playback controls
        if (playbackControls)
        {
            playbackControls.ResetTimeScale();
            playbackControls.gameObject.SetActive(false);
        }

        // Hide gameover panels
        if (gameoverController)
        {
            gameoverController.gameObject.SetActive(false);
        }
    }

    private void ToggleInitialInputAndButtonControls(bool interactable)
    {
        if (ringInputField != null) ringInputField.interactable = interactable;
        if (ringSlider != null) ringSlider.interactable = interactable;
        if (generateButton != null) generateButton.interactable = interactable;
        if (clearButton != null) clearButton.interactable = interactable;
        if (simulateButton != null) simulateButton.interactable = interactable;
    }

    IEnumerator CountDownStartGame()
    {
        yield return new WaitForEndOfFrame();

        if (countdownText == null)
        {
            yield return new WaitForSeconds(3);
            yield break;
        }

        countdownText.gameObject.SetActive(true);
        countdownText.text = "3";
        yield return new WaitForSeconds(1);

        countdownText.text = "2";
        yield return new WaitForSeconds(1);

        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        countdownText.text = "START";
        yield return new WaitForSeconds(1);
        countdownText.gameObject.SetActive(false);

        isStarted = true;

        if (playbackControls)
        {
            playbackControls.ToggleButtons(true);
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
