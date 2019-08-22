using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpedallingItemCarrier : ItemCarrier
{
    [SerializeField]
    protected float backwardsMovingPenalty;     // Flat speed penalty while moving backwards

    protected override void StartLeaving () {
        if (item == null) {
            Flip();
        }
        enemyState = EnemyState.LEAVING;
        // Set walk animation based on whether item is being carried
        if (item != null) {
            animator.SetTrigger("Walk_Carry");
        } else {
            animator.SetTrigger("Walk");
        }
    }

    protected override float CalculateSpeedModifier() {
        float modifier = base.CalculateSpeedModifier();
        // Apply penalty if moving backwards (i.e., if leaving and also carrying item)
        if (enemyState == EnemyState.LEAVING && item != null) {
            modifier -= backwardsMovingPenalty;
        }
        return modifier;
    }

    public override void OnItemDestroyed(GameObject item) {
        base.OnItemDestroyed(item);
        if (enemyState == EnemyState.LEAVING) {
            Flip();
        }
    }
}
