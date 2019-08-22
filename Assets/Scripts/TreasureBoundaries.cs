using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBoundaries : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Treasure")) {
            gameManager.waveManager.LoseWave();
        }
    }
}
