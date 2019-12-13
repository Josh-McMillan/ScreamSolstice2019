using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FollowType
{
    CHASE,
    STALK
}

[CreateAssetMenu(fileName = "AIProfile", menuName = "AI/Profile", order = 1)]
public class EnemyAIProfile : ScriptableObject
{
    [Header("General")]

    [Tooltip("How fast the enemy AI agent can go.")]
    public float moveSpeed = 2.0f;

    [Tooltip("The mode the AI is in. In CHASE, the AI will hunt down and kill the player. In STALK, the AI will follow but not approach the player.")]
    public FollowType followType = FollowType.CHASE;

    [Header("Respawning")]

    [Tooltip("How far away the enemy spawns in a circle.")]
    public float spawnRadius = 50.0f;

    [Tooltip("Minimum amount of seconds it takes to respawn.")]
    public float minimumRespawnTime = 8.0f;

    [Tooltip("Maximum amount of seconds it takes to respawn.")]
    public float maximumRespawnTime = 16.0f;


    [Header("Stalking")]

    [Tooltip("How far away in units the AI agent can approach player.")]
    public float stalkingDistance = 25.0f;

    [Header("Hiding")]

    [Tooltip("At what angle from the camera the enemy can hide.")]
    public float playerFieldOfView = 45.0f;
}
