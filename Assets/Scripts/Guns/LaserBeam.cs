using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Bullet
{
    [SerializeField]
    private float laserDuration;
    private float remainingDuration;
    [SerializeField]
    private float fadeoutTime;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private TurretController turret;

    public void FireBeam(float damage, float rotation) {
        this.damage = damage;
        Vector3 beamRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(beamRotation.x, beamRotation.y, rotation);
        remainingDuration = laserDuration;
    }
    
    protected override void Update() {
        // Deactivate laser after set time
        remainingDuration -= Time.deltaTime;

        if (remainingDuration <= 0) {
            laser.SetActive(false);
            turret.audioManager.Stop(Constants.LASER_FIRE);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.CompareTag("Enemy")) {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.GetHealth() > 0) {
                enemy.TakeDamage(damage);
                // Don't play hit SFX
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other) {
        
    }
}
