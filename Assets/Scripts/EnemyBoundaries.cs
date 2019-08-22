using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoundaries : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Enemy")) {
            other.GetComponent<Enemy>().Escape();
        }
    }
}
