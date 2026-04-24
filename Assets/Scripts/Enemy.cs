using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : NetworkBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    
    private float attackRange = 6;
    private float sightRange = 17.5f;
    private float timeBetweenAttacks = 3;

    private int power;

    private NavMeshAgent thisEnemy;
    private Animator anim;
    private Transform playerPos;

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
        if (playerPos == null)
        {
            try
            {
                playerPos = FindFirstObjectByType<PlayerHealth>().transform;
            }
            catch
            {
                playerPos = null;
            }
        }
        else
        {
            float distanceFromPlayer = Vector3.Distance(playerPos.position, this.transform.position);

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
        thisEnemy.SetDestination(playerPos.position);
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
