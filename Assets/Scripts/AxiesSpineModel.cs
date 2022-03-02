using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxiesSpineModel : MonoBehaviour
{
    [Header("Current State")]
    public AxiesSpineModelState state;
    [Range(0.5f, 2f)]
    public float speed;

    [Header("Attributes")]
    public int health;
    public bool movable;

    public event System.Action MoveEvent;
    public event System.Action AttackEvent;
}

public enum AxiesSpineModelState
{
    Idle,
    Moving,
    Attacking
}
