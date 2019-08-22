using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet
{
    [SerializeField]
    private float explosionRadius;
    [SerializeField]
    private LayerMask whatIsEnemy;
    [SerializeField]
    private ParticleSystem smokeParticleSystem;
    [SerializeField]
    private ParticleSystem explosionParticleSystem;

    protected override void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Enemy")) {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.GetHealth() > 0 && !bulletExpended) {
                Explode();
                bulletExpended = true;
            }
        }
    }

    private void Explode() {
        camBehaviour.TriggerShake();
        explosionParticleSystem.Play();
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, explosionRadius, whatIsEnemy);
        foreach(Collider2D collider in enemiesHit) {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
        }
        audioManager.Play(hitSfxName);
        Delete();
    }

    public override void Delete() {
        smokeParticleSystem.Stop();
        smokeParticleSystem.transform.SetParent(null);
        Destroy(smokeParticleSystem.gameObject, smokeParticleSystem.main.startLifetime.constantMax);
        explosionParticleSystem.transform.SetParent(null);
        Destroy(explosionParticleSystem.gameObject, explosionParticleSystem.main.startLifetime.constantMax);
        Destroy(gameObject);
    }

    // Draw area of effect in editor
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
