using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    RESPAWNING,
    PURSUING,
    HIDING,
    CATCHING
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 50.0f;

    [SerializeField] private float playerFieldOfView = 45.0f;
    [SerializeField] private LayerMask sightLayers;

    [SerializeField] private float minimumRespawnTime = 8.0f;
    [SerializeField] private float maximumRespawnTime = 16.0f;

    private EnemyState state = EnemyState.RESPAWNING;

    private Transform playerTransform;
    private Transform camTransform;

    private NavMeshAgent myAgent;
    private Animator myAnimator;
    private Collider myCollider;

    private bool hasHidden = false;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        camTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        myAgent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
    }

    private void Update()
    {
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

            state = EnemyState.PURSUING;
        }

        myAnimator.SetTrigger("DissolveIn");
        myAgent.isStopped = false;
        myCollider.enabled = true;
    }

    private void ChasePlayer()
    {
        if (!myAgent.pathPending)
        {
            myAgent.SetDestination(playerTransform.position);
        }

        if (IsInPlayerSight())
        {
            state = EnemyState.HIDING;
        }
    }

    private void HideFromPlayer()
    {
        if (!hasHidden)
        {
            Debug.Log("HIDING FROM PLAYER!");
            myAgent.isStopped = true;
            myAnimator.SetTrigger("DissolveOut");
            myCollider.enabled = false;
            StartCoroutine(RespawnTimer());
            hasHidden = true;
        }
    }

    private Vector3 GeneratePosition()
    {
        float angle = Random.Range(0, Mathf.PI * 2.0f);

        Vector2 position = new Vector2(Mathf.Sin(angle) * spawnRadius, Mathf.Cos(angle) * spawnRadius);

        return new Vector3(playerTransform.position.x + position.x, 0.0f, playerTransform.position.z + position.y);
    }

    private bool IsInPlayerSight()
    {
        Vector3 direction = transform.position - camTransform.position;
        float angle = Vector3.Angle(direction, camTransform.forward);

        if (angle < playerFieldOfView * 0.5f && !Physics.Linecast(transform.position + Vector3.up, playerTransform.position + Vector3.up, sightLayers))
        {
            return true;
        }

        return false;
    }

    IEnumerator RespawnTimer()
    {
        float respawnTime = Random.Range(minimumRespawnTime, maximumRespawnTime);

        Debug.Log("HALTING FOR " + respawnTime + " seconds!");

        yield return new WaitForSeconds(respawnTime);

        Debug.Log("RESPAWNING!");

        state = EnemyState.RESPAWNING;
        hasHidden = false;
    }
}
