using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTSUnit : MonoBehaviour, IDamagable, ITriggerable
{
    public Follower myFollower;
    public NavMeshAgent agent;
    [Tooltip("A navig�ci�s tengely offset miatt kell egy holder, ez annak az EGYETLEN gyereke")]
    [SerializeField] private Transform mTransform;
    public Vector2 targetDestination;
    public IDamagable targetEnemy;
    public Transform targetEnemyTransform;
    public float rotationSpeed = 1;
    public float angleThreshold = 30;
    public float maxChaseDistance;
    [SerializeField] private bool hasOrder = false;
    public LayerMask obstacleLayer;
    public Animator animator;
    private float lastAttackTime;

    public TriggerSensor sensor;

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

    public void Setup()
    {
        if (myFollower.weapon.weaponStats.usesAmmo)
        {
            angleThreshold = 5;
        }
    }

    public void MoveTo(Vector2 target)  // CSAK USER INPUTRA H�VJUK EZT NEM SZABAD FEL�L�RNIA SEMMILYEN PARANCSNAK
    {
        hasOrder = true;
        targetDestination = target;
        agent.SetDestination(new Vector3(targetDestination.x, targetDestination.y));
    }

    public void SelectTaget()
    {
        // target pick logika
        targetEnemy = sensor.NearestEnemy(StaticDataProvider.HostilityMatrix);
        if(targetEnemy != null)
        {
            targetEnemyTransform = targetEnemy.GetObject().transform;
        }
    }

    private void Update()
    {
        if (hasOrder && !agent.hasPath)
        {
            hasOrder = false;
        }

        if (targetEnemy != null)
        {
            float d = Vector2.Distance(transform.position, targetEnemyTransform.position);
            if (d > myFollower.weapon.weaponStats.range && d < maxChaseDistance && !hasOrder)
            {
                agent.SetDestination(new Vector3(targetEnemyTransform.position.x, targetEnemyTransform.position.y));
            }
            else if(d <= myFollower.weapon.weaponStats.range)
            {
                TryToAttack();
            }
            else if (d > maxChaseDistance)
            {
                targetEnemy = null;
                targetEnemyTransform = null;
            }
        }
    }

    public void TryToAttack()
    {
        if(Time.time - lastAttackTime >= myFollower.weapon.weaponStats.attackDelay)
        {
            if (HasLineOfSight(transform, targetEnemyTransform))
            {
                if (IsFacingAngle(GetLookAngleDifference(targetEnemyTransform)))
                {
                    Attack();
                }
                else
                {
                    LookAt(targetEnemyTransform);
                }
            }
        }
    }

    public void Attack()
    {
        if (!myFollower.weapon.weaponStats.usesAmmo)
        {
            animator.Play("Attack");
        }
        lastAttackTime = Time.time;
        targetEnemy.TakeDamage(myFollower.CalculateDamage(), gameObject);
    }

    public bool HasLineOfSight(Transform unit1, Transform unit2)
    {
        Vector2 direction = (Vector2)unit2.position - (Vector2)unit1.position;
        RaycastHit2D hit = Physics2D.Raycast(unit1.position, direction, direction.magnitude, obstacleLayer);
        if (hit.collider != null)
        {
            Debug.DrawLine(unit1.position, hit.point, Color.red);
            return false;
        }
        else
        {
            Debug.DrawLine(unit1.position, unit2.position, Color.green);
            return true;
        }
    }

    public void LookAt(Transform target) //Lehet �t kell �rni Y tengejre 
    {
        Vector2 direction = target.position - mTransform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly interpolate the rotation over time
        float angle = Mathf.MoveTowardsAngle(mTransform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        mTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void ReturnToIdleRotationLocal(Transform target)
    {
        float angle = Mathf.MoveTowardsAngle(mTransform.localRotation.eulerAngles.y, 0, rotationSpeed * Time.deltaTime);
        mTransform.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    public bool IsFacingAngle(float desiredAngle) // Lehet �t kell �rni Y tengejre
    {
        float currentAngle = mTransform.rotation.eulerAngles.z;

        // Calculate the absolute difference between current and desired angles
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, desiredAngle));

        // Check if the difference is smaller than the threshold
        return angleDifference < angleThreshold;
    }

    public float GetLookAngleDifference(Transform target) // Lehet �t kell �rni Y tengejre
    {
        Vector2 direction = target.position - transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public void TakeDamage(int amount, GameObject sender)
    {
        myFollower.health -= amount;
        if(myFollower.health <= 0)
        {
            // glob�lis feltakar�t�s �s hat�ssz�mol�s
            StaticDataProvider.DieInConbat(myFollower);

            // saj�t hal�l logika
            Destroy(gameObject);
        }

        if (!targetEnemyTransform)
        {
            SelectTaget();
        }
    }

    public int GetTeamId()
    {
        return 1;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public void OnNewEnemyInRange(IDamagable newEnemy)
    {
        if (!targetEnemyTransform && StaticDataProvider.HostilityMatrix[newEnemy.GetTeamId()])
        {
            targetEnemy = newEnemy;
            targetEnemyTransform = newEnemy.GetObject().transform;
        }
    }
}