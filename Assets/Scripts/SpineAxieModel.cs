using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class SpineAxieModel : MonoBehaviour
{
    public SpineGauge healthGauge;

    [Header("Current State")]
    public SpineAxieModelState state;
    public bool facingLeft;

    [Header("Attributes")]
    public int currentHealth;
    public int maximumHealth;
    public AxieType axieType;
    public bool immovable;

    [Header("Battle")]
    public int rollNumber;
    public int damage;
    public Vector3Int opponentTile;

    [Header("Technical Details")]
    public int flag = 0;

    float moveSpeed = 10f;
    float attackInterval = 1f;      // default: 0.12f???
    float lastAttackTime;
    [HideInInspector]
    public float fadeOutDuration;       // for dying animation

    public event System.Action AttackEvent;
    public event System.Action DieEvent;

    public bool IsDeath => currentHealth <= 0;

    [HideInInspector]
    public GameManager gameManager;

    void Start()
    {
        lastAttackTime = Mathf.NegativeInfinity;
        fadeOutDuration = 0.4f;
        currentHealth = maximumHealth;
        opponentTile = new Vector3Int(99, 99, 99);

        gameManager = FindObjectOfType<GameManager>();
    }

    public void Prepare()
    {
        rollNumber = Random.Range(0, 3);
    }

    public bool TryMove(Vector3 destination)
    {
        if (immovable == true) return false;

        StartCoroutine(MoveRoutine(destination));
        return true;
    }

    public bool TryAttack(SpineAxieModel target)
    {
        // Can only attack while in idle mode
        if (state != SpineAxieModelState.Idle) return false;

        // Prevent repeated attack attemps
        float currentTime = Time.time;
        if (currentTime - lastAttackTime > attackInterval)
        {
            lastAttackTime = currentTime;
            if (AttackEvent != null)
            {
                AttackEvent();         // Fire the "AttackEvent" event
            }

            if (target)
            {
                damage = CalculateDamage(rollNumber, target.rollNumber);
                target.OnReceiveDamage(damage);

                //Debug.Log("Deal damage: " + damage + " (roll: " + rollNumber + ") - Health: " + target.currentHealth);
            }
        }

        return true;
    }

    public void TryDie()
    {
        DieEvent();     // Fire the "DieEvent" event
    }

    IEnumerator MoveRoutine(Vector3 destination)
    {
        if (immovable == true) yield break;
        if (state == SpineAxieModelState.Moving) yield break;

        state = SpineAxieModelState.Moving;

        // "Fake" moving to the specified destination
        {
            while (Vector3.Distance(destination, transform.localPosition) > 0.001f)
            {
                float step = moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, destination, step);
                yield return null;
            }
            transform.position = destination;
        }

        state = SpineAxieModelState.Idle;

        if (gameManager)
        {
            gameManager.OnFinishAxieAnimation();
        }
    }

    private int CalculateDamage(int attackerRoll, int defenderRoll)
    {
        if ((3 + attackerRoll - defenderRoll) % 3 == 0)
        {
            return 4;
        }
        else if ((3 + attackerRoll - defenderRoll) % 3 == 1)
        {
            return 5;
        }
        else if ((3 + attackerRoll - defenderRoll) % 3 == 2)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    private void OnReceiveDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthGauge)
        {
            float percent = 1f * currentHealth / maximumHealth;
            healthGauge.SetGaugePercent(percent);
        }
    }
}

public enum SpineAxieModelState
{
    Idle,
    Moving,
    //Die
}

public enum AxieType
{
    Default,
    Attack,
    Defense,
    Both
}
