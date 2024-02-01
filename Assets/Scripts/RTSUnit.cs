using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTSUnit : MonoBehaviour, IDamagable, ITriggerable
{
    public Follower myFollower;
    public NavMeshAgent agent;
    public AudioSource audio;
    [Tooltip("A navigációs tengely offset miatt kell egy holder, ez annak az EGYETLEN gyereke")]
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

    public GameObject meleeWeapon;
    public GameObject rangedWeapon;

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
            meleeWeapon.SetActive(false);
            rangedWeapon.SetActive(true);
            rangedWeapon.GetComponent<SpriteRenderer>().sprite = myFollower.weapon.itemSprite;
            animator.Play("RangedIdle");
        }
        else
        {
            meleeWeapon.SetActive(true);
            rangedWeapon.SetActive(false);
            meleeWeapon.GetComponent<SpriteRenderer>().sprite = myFollower.weapon.itemSprite;
        }
        audio.clip = myFollower.weapon.audio;
    }

    public void MoveTo(Vector2 target)  // CSAK USER INPUTRA HÍVJUK EZT NEM SZABAD FELÜLÍRNIA SEMMILYEN PARANCSNAK
    {
        hasOrder = true;
        Debug.Log("OrderGiven");
        targetDestination = target;

        agent.SetDestination(new Vector3(targetDestination.x, targetDestination.y));
    }

    public void SelectTaget()
    {
        // target pick logika
        targetEnemy = sensor.NearestEnemy(StaticDataProvider.HostilityMatrix);
        if (targetEnemy != null)
        {
            targetEnemyTransform = targetEnemy.GetObject().transform;
        }
    }

    private void Update()
    {
        if (hasOrder && (!agent.hasPath)) //|| agent.pathStatus == NavMeshPathStatus.PathComplete
        {
            hasOrder = false;
            Debug.Log("OrderCompleate!");
        }
        //globalRot = mTransform.rotation;

        if (targetEnemy != null && targetEnemyTransform != null)
        {
            float d = Vector2.Distance(transform.position, targetEnemyTransform.position);
            if (d > myFollower.weapon.weaponStats.range && d < maxChaseDistance && !hasOrder)
            {
                Debug.Log("CHASE");
                agent.SetDestination(new Vector3(targetEnemyTransform.position.x, targetEnemyTransform.position.y));
            }
            else if (d <= myFollower.weapon.weaponStats.range)
            {
                TryToAttack();
            }
            else if (d > maxChaseDistance)
            {
                //targetEnemy = null;
                //targetEnemyTransform = null;
                // majd kell valami custom logika a követés feladására
            }
        }
        else
        {
            ReturnToIdleRotationLocal();
        }
    }

    public void TryToAttack()
    {

        if (Time.time - lastAttackTime >= myFollower.weapon.weaponStats.attackDelay)
        {
            /*if (HasLineOfSight(transform, targetEnemyTransform))
            {*/

            if (IsFacingAngle(GetLookAngleDifference(targetEnemyTransform)))
            {
                //Debug.Log("TryToAttack");
                Attack();
            }
            else
            {
                //Debug.Log("Looking at: " + GetLookAngleDifference(targetEnemyTransform));
                LookAt(targetEnemyTransform);
            }
            //}
        }
    }

    public void Attack()
    {
        if (!myFollower.weapon.weaponStats.usesAmmo)
        {
            animator.Play("Attack");
        }
        else
        {
            animator.Play("RangedAttack");
        }
        audio.Play();
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

    public void LookAt(Transform target)
    {
        Vector2 direction = target.position - mTransform.position;
        //float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg; // ez jó irányba néz

        // Smoothly interpolate the rotation over time
        float angle = Mathf.MoveTowardsAngle(mTransform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        mTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //float angle = Mathf.MoveTowardsAngle(mTransform.rotation.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
        //mTransform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    public void ReturnToIdleRotationLocal()
    {
        float angle = Mathf.MoveTowardsAngle(mTransform.localRotation.eulerAngles.z, 0, rotationSpeed / 2 * Time.deltaTime);
        mTransform.localRotation = Quaternion.Euler(new Vector3(90, 0, angle)); // vagy mégse?
    }

    public bool IsFacingAngle(float desiredAngle)
    {
        float currentAngle = mTransform.rotation.eulerAngles.z;

        // Calculate the absolute difference between current and desired angles
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, desiredAngle));

        // Check if the difference is smaller than the threshold
        return angleDifference < angleThreshold;
    }

    public float GetLookAngleDifference(Transform target) // az egész alapból tartalmaz egy 90 fokos offset-et
    {
        Vector2 direction = target.position - transform.position;
        //return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
    }

    public void TakeDamage(int amount, GameObject sender)
    {
        myFollower.health -= amount;
        if (myFollower.health <= 0)
        {
            // globális feltakarítás és hatásszámolás
            StaticDataProvider.DieInConbat(myFollower);

            // saját halál logika
            GameObject.Find("MANAGER").GetComponent<RTSManager>().LooseUnit(myFollower);
            Destroy(gameObject);
        }
        GameObject.Find("MANAGER").GetComponent<RTSManager>().UpdateUI();
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
        //Debug.Log("Hostile To RTS unit: " + StaticDataProvider.HostilityMatrix[newEnemy.GetTeamId()]);
        if (!targetEnemyTransform && StaticDataProvider.HostilityMatrix[newEnemy.GetTeamId()])
        {
            //Debug.Log("TARGET LOCKED!");
            targetEnemy = newEnemy;
            targetEnemyTransform = targetEnemy.GetObject().transform;
            Debug.Log("TARGET LOCKED: " + targetEnemy.GetObject().name);
        }
    }

    public void OnTargetKIA()
    {
        SelectTaget();
    }
}
