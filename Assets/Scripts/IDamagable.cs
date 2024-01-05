using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(int amount, GameObject sender);

    public int GetTeamId();
    /*
    0 - netural (mondjuk egy doboz)
    1 - saját ember
    2 - zombie
    3 - többi túlélõ
    4 - bandita
    */

    public GameObject GetObject();
}
