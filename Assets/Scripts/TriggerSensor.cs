using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensor : MonoBehaviour
{
    private List<IDamagable> damagables = new List<IDamagable>();

    // ha esetleg elt�prengn�l azon hogy ez itt mi, nos annak a k�vetkezm�nye hogy a unity nem enged interface-t serializ�lni...
    // vagyish�t enged, csak nem haszn�lhatsz olyan objektumot ami �gy vagy �gy de a UnityEngine.Object b�l sz�rmazik le...
    // ez�rt a l�tez� legg�nyol�sabb m�don lesz megolva. (�gy lehet m�r maga az interface is �rtelmetlen...)
    // A TRIGGERABLE INTERFACE� SCRIPTNEK A ROOT OBJEKUMON K�NE LENNIE! (de nem musz�j)
    // �S tudom hogy egyszer�bb lett volna �r�k�ltet�ssel megoldani, csak ezt 5369 sorral ez el�tt kellett volna hogy eszembe jusson
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
            if(myTeamID != item.GetTeamId() && item.GetTeamId() != 0)   // teh�t nincs vel�nk �s nem semleges
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
            if (hostilityMatrix[item.GetTeamId()])   // teh�t nincs vel�nk �s nem semleges
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
