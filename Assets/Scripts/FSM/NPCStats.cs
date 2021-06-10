using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCType {
    PAWN, // Peon
    ROOK, // Torre
    KNIGHT, // Caballo
    BISHOP, // Alfil
    QUEEN, // Reina
    KING, // Rey
};

[CreateAssetMenu(fileName = "NPCStats", menuName = "NPC/NPC Stats", order = 1)]
public class NPCStats : ScriptableObject {
    //public NPCType type;
    public float detectionRange;
    public float patrolRange;
    public Vector2 idleDuration;
    public float patrolSpeed;
    public float chaseSpeed;
    public float ChaseRange;
    public float attackRange;
    public float waitBeforeAttack;
    public float waitAfterAttack;
    public float damage;
    public float scapeDistance;
    [Range(1f, 2f)]
    public float criticalMultiplier;
    [Range(0f,1f)]
    public float criticalChance;
}
