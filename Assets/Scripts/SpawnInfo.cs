using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    public EnemyType enemyType;
    public int shuffleRange; // Number of times this enemy can be moved behind or forward in the order
}
