using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected Vector3 moveDirection;
    protected float speed;
    protected float damage;
    protected bool bulletExpended;              // Set to true to indicate this bullet has hit a target
    protected AudioManager audioManager;
    protected CameraBehaviour camBehaviour;
    [SerializeField]
    protected string hitSfxName;

    public void Initialize(Vector3 moveDirection, float speed, float damage, AudioManager audioManager, CameraBehaviour camBehaviour) {
        this.moveDirection = moveDirection;
        this.speed = speed;
        this.damage = damage;
        this.audioManager = audioManager;
        this.camBehaviour = camBehaviour;
    }

    protected virtual void Update() {
        transform.position = transform.position + moveDirection * speed * Time.deltaTime;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Enemy")) {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy.GetHealth() > 0 && !bulletExpended) {
                enemy.TakeDamage(damage);
                Delete();
                bulletExpended = true;
                audioManager.Play(hitSfxName);
            }
        }
    }
    
    public virtual void Delete() {
        Destroy(gameObject);
    }
}
