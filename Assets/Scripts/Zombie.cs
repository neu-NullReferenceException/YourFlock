using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamagable, ITriggerable
{
    public int heath = 100;
    public NavMeshAgent agent;
    public IDamagable targetEnemy;
    public Transform targetEnemyTransform;
    private float lastAttackTime;
    public Animator animator;
    public bool isTriggered;

    public float attackDelay = 1;
    public float attackRange = 1;
    public int minDamage;
    public int maxDamage;

    public GameObject GetObject()
    {
        return gameObject;
    }

    public int GetTeamId()
    {
        return 2;
    }

    public void TakeDamage(int amount, GameObject sender)
    {
        heath -= amount;
        if (!isTriggered)
        {
            if(sender.TryGetComponent<IDamagable>(out IDamagable co))
            {
                targetEnemy = co;
                targetEnemyTransform = co.GetObject().transform;
                isTriggered = true;
            }
            
        }

        if(heath <= 0)
        {
            // halál logika - most csak töröl
            Destroy(gameObject);
        }
    }
    private void Awake()
    {
        //agent.stoppingDistance = 1;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
        {
            //place it on the nearest NavMesh point
            transform.position = hit.position;
        }
        else
        {
            Debug.LogError("No valid NavMesh found at the initial position.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered && targetEnemyTransform)
        {
            agent.SetDestination(targetEnemyTransform.position);
            TryToAttack();
        }
    }

    public void TryToAttack()
    {
        if (Time.time - lastAttackTime >= attackDelay && Vector2.Distance(transform.position, targetEnemyTransform.position) <= attackRange)
        {
            animator.Play("Attack");
            lastAttackTime = Time.time;
            targetEnemy.TakeDamage(Random.Range(minDamage, maxDamage), gameObject);
        }
    }

    public void OnNewEnemyInRange(IDamagable newEnemy)
    {
        if (!targetEnemyTransform && newEnemy.GetTeamId() != 2)
        {
            Debug.Log("Zombie Engaging!");
            targetEnemy = newEnemy;
            targetEnemyTransform = newEnemy.GetObject().transform;
            isTriggered = true;
        }
    }
}
