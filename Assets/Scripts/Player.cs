using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public RandomRange angryTime;
    private Animator anim;
    private bool calmingDown;           // True iff treasure has been dropped
    private float remainingAngryTime;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (calmingDown) {
            if (remainingAngryTime <= 0) {
                CalmDown();
                calmingDown = false;
            }
            remainingAngryTime -= Time.deltaTime;
        }
    }

    public void CalmDown() {
        anim.SetTrigger("Neutral");
    }

    public void OnTreasureStolen() {
        anim.SetTrigger("Angry");
        calmingDown = false;

    }

    public void OnTreasureDropped() {
        calmingDown = true;
        remainingAngryTime = angryTime.GetRandom();
    }
}
