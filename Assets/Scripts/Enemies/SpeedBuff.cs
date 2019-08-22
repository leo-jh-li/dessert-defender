using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    [SerializeField]
    private float buffDuration;

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.CompareTag("EnemyBase")) {
            other.GetComponent<EnemyBase>().BuffSpeed(buffDuration);
        }
    }
}
