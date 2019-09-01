using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [SerializeField]
    protected Animator emoteAnim;
    protected Vector3 destination;        // Position where enemy moves toward in search of the treasure
    private bool destinationLocked;     // If true, don't change destination

    protected override void Start() {
        base.Start();
        enemyState = EnemyState.APPROACHING;
        // Default destination is the starting spot
        destination = gameManager.treasureStartingLocation;
    }

    protected override void Update() {
        base.Update();
        Treasure treasure = gameManager.treasure.GetComponent<Treasure>();
        switch (enemyState) {
            case EnemyState.APPROACHING:
                transform.position = transform.position + new Vector3(-GetMovement(), 0, 0);
                // If treasure is close enough and on the floor, set destination to it instead
                if (treasure.CanBePickedUp() && !destinationLocked &&
                        treasure.transform.position.x <= transform.position.x &&
                        transform.position.x <= treasure.transform.position.x + gameManager.treasureConvergenceDist) {
                    destination = gameManager.treasure.position;
                }
                // If within set distance of destination, begin converging
                if (destination.x <= transform.position.x && transform.position.x <= destination.x + gameManager.treasureConvergenceDist) {
                    enemyState = EnemyState.CONVERGING;
                }
                break;
            case EnemyState.CONVERGING:
                // If treasure has been picked up while converging upon it, set destination to treasure's starting location and lock it
                if (destination != gameManager.treasureStartingLocation && treasure.carried) {
                    destinationLocked = true;
                    destination = gameManager.treasureStartingLocation;
                    // Approach new destination
                    enemyState = EnemyState.APPROACHING;
                    transform.position = transform.position + new Vector3(-GetMovement(), 0, 0);
                    break;
                }
                // Approach treasure on x axis
                if (transform.position.x != destination.x) {
                    transform.position = new Vector3(Mathf.Max(transform.position.x - GetMovement(), destination.x), transform.position.y, transform.position.z);
                }
                // Approach treasure on y axis
                if (transform.position.y < destination.y) {
                    transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y + GetMovement(), destination.y), transform.position.z);
                } else if (transform.position.y > destination.y) {
                    transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y - GetMovement(), destination.y), transform.position.z);
                }
                if (transform.position.x == destination.x && transform.position.y == destination.y) {
                    // Pick up treasure if it's at the destination
                    if (treasure.CanBePickedUp() && treasure.transform.position == transform.position) {
                        PickUpTreasure();
                        StartLeaving();
                    } else {
                        StartCoroutine(ActConfused());
                    }
                }
                break;
            case EnemyState.LEAVING:
                // Pick up treasure if it's on the floor and this enemy walks over it
                if (treasure.CanBePickedUp() &&
                        (transform.position.x <= treasure.transform.position.x && treasure.transform.position.x <= transform.position.x + GetMovement())) {
                    PickUpTreasure();
                }
                transform.position = transform.position + new Vector3(GetMovement(), 0, 0);
                break;
        }
    }

    protected virtual void StartLeaving() {
        Flip();
        enemyState = EnemyState.LEAVING;
        animator.SetTrigger("Walk");
    }

    protected virtual IEnumerator ActConfused() {
        enemyState = EnemyState.STOPPED;
        emoteAnim.SetTrigger("Popup");
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(2f);
        StartLeaving();
    }

    private void PickUpTreasure() {
        carryingTreasure = true;
        gameManager.treasure.GetComponent<Treasure>().carried = true;
        gameManager.treasure.SetParent(transform);
        gameManager.treasure.position = treasureCarryPoint.position;
        gameManager.player.OnTreasureStolen();
    }

    protected virtual float CalculateSpeedModifier() {
        float mod = 0;
        mod += carryingTreasure ? -treasureSlowDown : 0;
        mod += speedBuffDuration > 0 ? speedBuffBonus : 0;
        return mod;
    }

    // Get distance moved based on speed, time since last update, and whether this enemy is carrying the treasure
    protected float GetMovement() {
        float flatModifier = CalculateSpeedModifier();
        float movement = (speed + flatModifier) * Time.deltaTime;
        return movement;
    }
}
