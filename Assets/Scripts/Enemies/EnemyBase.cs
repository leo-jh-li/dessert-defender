using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The area on the ground where an Enemy is considered standing
public class EnemyBase : MonoBehaviour
{
    // Refresh associated Enemy's speed buff
    public void BuffSpeed(float duration) {
        GetComponentInParent<Enemy>().BuffSpeed(duration);
    }
}
