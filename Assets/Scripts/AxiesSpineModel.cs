using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class AxiesSpineModel : MonoBehaviour
{
    [Header("Current State")]
    public AxiesSpineModelState state;

    [Header("Attributes")]
    public int health;
    public bool immovable;

    float moveSpeed = 10f;
    float attackInterval = 0.12f;
    float lastAttackTime;
    [HideInInspector]
    public float fadeOutDuration;       // for dying animation

    public event System.Action AttackEvent;
    public event System.Action DieEvent;

    void Awake()
    {
        fadeOutDuration = 0.8f;
    }

    public void TryMove(Vector3 destination)
    {
        if (immovable == true) return;
        StartCoroutine(MoveRoutine(destination));
    }

    public void TryAttack()
    {
        // Can only attack while in idle mode
        if (state != AxiesSpineModelState.Idle) return;

        // Prevent repeated attack attemps
        float currentTime = Time.time;
        if (currentTime - lastAttackTime > attackInterval)
        {
            lastAttackTime = currentTime;
            if (AttackEvent != null)
            {
                AttackEvent();         // Fire the "AttackEvent" event
            }
        }
    }

    public void TryDie()
    {
        DieEvent();     // Fire the "DieEvent" event
    }

    IEnumerator MoveRoutine(Vector3 destination)
    {
        if (immovable == true) yield break;
        if (state == AxiesSpineModelState.Moving) yield break;

        state = AxiesSpineModelState.Moving;

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

        state = AxiesSpineModelState.Idle;
    }
}

public enum AxiesSpineModelState
{
    Idle,
    Moving,
    Die
}
