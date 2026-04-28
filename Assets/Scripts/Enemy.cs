using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : NetworkBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    
    private float attackRange = 6;
    private float sightRange = 17.5f;
    private float timeBetweenAttacks = 2;

    private int power;
    private int maxPlayers = 5;

    private NavMeshAgent thisEnemy;
    private Animator anim;
    private List<GameObject> playerPos = new List<GameObject>();

    private bool isAttacking = false;

    private int targetPoint;

    private void Start()
    {
        thisEnemy = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        targetPoint = Random.Range(0, patrolPoints.Length);
    }

    private void Update()
    {
        if (playerPos.Count == 0)
        {
            try
            {
                playerPos = GameObject.FindGameObjectsWithTag("Player").OrderBy(player => DistanceToPlayer(player)).ToList();
            }
            catch
            {
                playerPos.Clear();
            }
        }

        else
        {
            Debug.Log(playerPos.Count);
            Debug.Log(playerPos[0].name);
            
            float distanceFromPlayer = DistanceToPlayer(playerPos[0]);

            if (distanceFromPlayer <= sightRange && distanceFromPlayer > attackRange && !PlayerHealth.isDead)
            {
                isAttacking = false;
                thisEnemy.isStopped = false;
                StopAllCoroutines();

                ChasePlayer();
            }
            
            else if (distanceFromPlayer > sightRange && distanceFromPlayer > attackRange && !PlayerHealth.isDead)
            {
                isAttacking = false;
                anim.SetBool("Attacking", false);

                Patrol();
            }

            else if (distanceFromPlayer <= attackRange && !isAttacking && !PlayerHealth.isDead)
            {
                thisEnemy.isStopped = true;
                StartCoroutine(AttackPlayer());
            }

            else if (PlayerHealth.isDead)
            {
                thisEnemy.isStopped = true;
            }
        }
    }

    private float DistanceToPlayer(GameObject player)
    {
        return Vector3.Distance(player.transform.position, this.transform.position);
    }

    private void Patrol()
    {   
        if (!thisEnemy.pathPending && thisEnemy.remainingDistance < 0.5f)
        {
            thisEnemy.isStopped = false;
            anim.SetBool("Walking", true);
            int point = Random.Range(0, patrolPoints.Length);
            thisEnemy.SetDestination(patrolPoints[point].position);
        }
    }

    private void ChasePlayer()
    {
        anim.SetBool("Attacking", false);
        anim.SetBool("Walking", true);
        thisEnemy.SetDestination(playerPos[0].transform.position);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        anim.SetBool("Walking", false);
        anim.SetBool("Attacking", true);

        yield return new WaitForSeconds(timeBetweenAttacks);

        power = Random.Range(13, 17);
        
        FindFirstObjectByType<PlayerHealth>().TakeDamage(power);

        anim.SetBool("Attacking", false);
        isAttacking = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
}
