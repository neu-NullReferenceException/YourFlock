using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensor : MonoBehaviour
{
    private List<IDamagable> damagables = new List<IDamagable>();

    // ha esetleg eltöprengnél azon hogy ez itt mi, nos annak a következménye hogy a unity nem enged interface-t serializálni...
    // vagyishát enged, csak nem használhatsz olyan objektumot ami így vagy úgy de a UnityEngine.Object bõl származik le...
    // ezért a létezõ leggányolósabb módon lesz megolva. (így lehet már maga az interface is értelmetlen...)
    // A TRIGGERABLE INTERFACEÛ SCRIPTNEK A ROOT OBJEKUMON KÉNE LENNIE! (de nem muszáj)
    // ÉS tudom hogy egyszerûbb lett volna örököltetéssel megoldani, csak ezt 5369 sorral ez elõtt kellett volna hogy eszembe jusson
    public ITriggerable triggerable;

    private void Awake()
    {
        triggerable = transform.root.GetComponentInChildren<ITriggerable>();
    }

    public List<IDamagable> GetPossibleTagets()
    {
        return damagables;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(transform.root.name + "  GOT TRIGGERED BY" + other.name);
        if(other.transform.root.TryGetComponent<IDamagable>(out IDamagable target))
        {
            Debug.Log(transform.root.name + "  found damagable");
            if (!damagables.Contains(target))
            {
                Debug.Log(transform.root.name + "  Got new target");
                damagables.Add(target);
                triggerable.OnNewEnemyInRange(target);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.root.TryGetComponent<IDamagable>(out IDamagable target))
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

    /*public void TriggerTriggerable(IDamagable target)
    {
        if (zombie)
        {
            zombie.OnNewEnemyInRange();
        }
    }*/
}

public interface ITriggerable
{
    public void OnNewEnemyInRange(IDamagable newEnemy);
}
