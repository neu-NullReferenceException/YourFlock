using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(int amount, GameObject sender);

    public int GetTeamId();
    /*
    0 - netural (mondjuk egy doboz)
    1 - saj�t ember
    2 - zombie
    3 - t�bbi t�l�l�
    4 - bandita
    */

    public GameObject GetObject();
}
