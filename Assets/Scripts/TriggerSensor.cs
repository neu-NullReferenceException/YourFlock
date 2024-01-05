using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensor : MonoBehaviour
{
    private List<IDamagable> damagables = new List<IDamagable>();
    
    public List<IDamagable> GetPossibleTagets()
    {
        return damagables;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<IDamagable>(out IDamagable target))
        {
            if (!damagables.Contains(target))
            {
                damagables.Add(target);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamagable>(out IDamagable target))
        {
            if (damagables.Contains(target))
            {
                damagables.Remove(target);
            }
        }
    }

    public IDamagable NearestEnemy(int myTeamID)
    {
        float minDist = Mathf.Infinity;
        IDamagable closest = null;
        foreach (IDamagable item in damagables)
        {
            if(myTeamID != item.GetTeamId() && item.GetTeamId() != 0)   // tehát nincs velünk és nem semleges
            {
                float d = Vector2.Distance(transform.root.position, item.GetObject().transform.position);
                if (d < minDist)
                {
                    closest = item;
                    minDist = d;
                }
            }
        }

        return closest;
    }

    public IDamagable NearestEnemy(bool[] hostilityMatrix)
    {
        float minDist = Mathf.Infinity;
        IDamagable closest = null;
        foreach (IDamagable item in damagables)
        {
            if (hostilityMatrix[item.GetTeamId()])   // tehát nincs velünk és nem semleges
            {
                float d = Vector2.Distance(transform.root.position, item.GetObject().transform.position);
                if (d < minDist)
                {
                    closest = item;
                    minDist = d;
                }
            }
        }

        return closest;
    }
}

public interface ITriggerable
{
    public void OnNewEnemyInRange(IDamagable newEnemy);
}
