using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBearer : Walker
{
    public RandomRange idleDistanceRange;  // Range of distance at which flag bearer will stop
    [SerializeField]
    private float minWalkDistance;  // Minimum distance flag bearer must walk before stopping


    protected override void Start() {
        base.Start();
        animator.SetTrigger("Walk_Carry");
        // Choose a random distance from the treasure at which to stop
        Vector3 treasurePos = gameManager.treasure.transform.position;
        destination = transform.position;
        destination.x = treasurePos.x + idleDistanceRange.GetRandom();
        // Ensure this flag bearer walks at least the minWalkDistance
        destination.x = Mathf.Min(destination.x, transform.position.x - minWalkDistance);
    }

    protected override void Update() {
        if (enemyState == EnemyState.APPROACHING) {
            transform.position = transform.position + new Vector3(-GetMovement(), 0, 0);
            // Stop moving if close enough to destination
            if (transform.position.x <= destination.x) {
                animator.SetTrigger("Idle_Carry");
                enemyState = EnemyState.STOPPED;
            }
        }
    }

    public override void BuffSpeed(float duration) {
        
    }
}
