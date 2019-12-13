using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    RESPAWNING,
    PURSUING,
    STALKING,
    HIDING,
    CATCHING
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyAIProfile profile;

    [SerializeField] private LayerMask sightLayers;

    [Header("DEBUGGING TOGGLES")]
    [SerializeField] private bool disableFadeout = false;
    [SerializeField] private bool disableRespawn = false;
    [SerializeField] private bool disableHiding = false;

    private EnemyState state = EnemyState.RESPAWNING;

    private Transform playerTransform;
    private Transform camTransform;

    private NavMeshAgent myAgent;
    private Animator myAnimator;
    private Collider myCollider;

    private bool hasHidden = false;
    private bool hasStalked = false;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        camTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        myAgent = GetComponent<NavMeshAgent>();
        myAnimator = transform.GetChild(0).GetComponent<Animator>();
        myCollider = GetComponent<Collider>();

        myAgent.speed = profile.moveSpeed;
    }

    private void Update()
    {
        myAnimator.SetFloat("Blend", myAgent.velocity.magnitude / 20.0f);

        if (Input.GetKeyDown(KeyCode.R))
        {
            state = EnemyState.RESPAWNING;
        }

        switch (state)
        {
            case EnemyState.RESPAWNING:
                Respawn();
                break;
            case EnemyState.PURSUING:
                ChasePlayer();
                break;
            case EnemyState.STALKING:
                StalkPlayer();
                break;
            case EnemyState.HIDING:
                HideFromPlayer();
                break;
            case EnemyState.CATCHING:
                // Kill the Player
                break;
        }
    }

    private void Respawn()
    {
        myAgent.isStopped = true;

        if (!disableRespawn)
        {
            Vector3 spawnPosition = GeneratePosition();

            Collider[] hitColliders = Physics.OverlapSphere(spawnPosition + Vector3.up, 1.0f);

            if (hitColliders.Length > 1)
            {
                Respawn();
                return;
            }
            else
            {
                transform.position = spawnPosition;

                if (IsInPlayerSight())
                {
                    Respawn();
                    return;
                }

                switch (profile.followType)
                {
                    case FollowType.CHASE:
                        state = EnemyState.PURSUING;
                        break;

                    case FollowType.STALK:
                        state = EnemyState.STALKING;
                        break;
                }
            }
        }

        myAnimator.SetBool("IsHidden", false);
        myAgent.isStopped = false;
        myCollider.enabled = true;
    }

    private void ChasePlayer()
    {
        Debug.Log("CHASING PLAYER!");

        if (!myAgent.pathPending)
        {
            myAgent.SetDestination(playerTransform.position);
        }

        if (IsInPlayerSight() && !disableHiding)
        {
            state = EnemyState.HIDING;
        }
    }

    private void StalkPlayer()
    {
        Debug.Log("STALKING PLAYER!");

        if (Vector3.Distance(transform.position, playerTransform.position) <= profile.stalkingDistance)
        {
            myAgent.isStopped = true;
        }
        else if (!myAgent.pathPending)
        {
            myAgent.SetDestination(playerTransform.position);
        }

        if (IsInPlayerSight() && !disableHiding)
        {
            hasStalked = false;
            state = EnemyState.HIDING;
        }

        hasStalked = true;
    }

    private void HideFromPlayer()
    {
        if (!hasHidden)
        {
            myAgent.isStopped = true;

            if (!disableFadeout)
            {
                myAnimator.SetBool("IsHidden", true);
            }

            myCollider.enabled = false;
            StartCoroutine(RespawnTimer());
            hasHidden = true;
        }
    }

    private Vector3 GeneratePosition()
    {
        float angle = Random.Range(0, Mathf.PI * 2.0f);

        Vector2 position = new Vector2(Mathf.Sin(angle) * profile.spawnRadius, Mathf.Cos(angle) * profile.spawnRadius);

        return new Vector3(playerTransform.position.x + position.x, 0.0f, playerTransform.position.z + position.y);
    }

    private bool IsInPlayerSight()
    {
        Vector3 direction = transform.position - camTransform.position;
        float angle = Vector3.Angle(direction, camTransform.forward);

        if (angle < profile.playerFieldOfView * 0.5f && !Physics.Linecast(transform.position + Vector3.up, playerTransform.position + Vector3.up, sightLayers))
        {
            return true;
        }

        return false;
    }

    IEnumerator RespawnTimer()
    {
        float respawnTime = Random.Range(profile.minimumRespawnTime, profile.maximumRespawnTime);

        Debug.Log("HALTING FOR " + respawnTime + " seconds!");

        yield return new WaitForSeconds(respawnTime);

        Debug.Log("RESPAWNING!");

        state = EnemyState.RESPAWNING;
        hasHidden = false;
    }
}
