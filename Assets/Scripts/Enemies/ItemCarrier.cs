using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCarrier : Walker
{
    [SerializeField]
    protected CarriedObject item;     // Reference to item this enemy is carrying
    [SerializeField]
    protected float carryPenalty;     // Flat speed penalty while holding this enemy's object

    protected override void Start() {
        base.Start();
        animator.SetTrigger("Walk_Carry");
    }

    public override void Flip() {
        base.Flip();
        if (item != null) {
            item.Flip();
        }
    }

    protected override IEnumerator ActConfused() {
        enemyState = EnemyState.STOPPED;
        emoteAnim.SetTrigger("Popup");
        if (item != null) {
            animator.SetTrigger("Idle_Carry");
        } else {
            animator.SetTrigger("Idle");
        }
        yield return new WaitForSeconds(2f);
        StartLeaving();
    }

    public override float GetGroupHealth() {
        return maxHealth + item.maxHealth;
    }

    protected override float CalculateSpeedModifier() {
        float modifier = base.CalculateSpeedModifier();
        modifier += item != null ? -carryPenalty : 0;
        return modifier;
    }

    public virtual void OnItemDestroyed(GameObject item) {
        // Update walking animation if carried item is destroyed
        if (enemyState != EnemyState.STOPPED) {
            animator.SetTrigger("Walk");
        } else {
            animator.SetTrigger("Idle");
        }
    }

    
    protected override void StartLeaving() {
        Flip();
        enemyState = EnemyState.LEAVING;
        // Set walk animation based on whether item is being carried
        if (item != null) {
            animator.SetTrigger("Walk_Carry");
        } else {
            animator.SetTrigger("Walk");
        }
    }
}
