using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    WALKER,
    LEGGY,
    BIGGY,
    SHIELD,
    BIG_SHIELD,
    KNIGHT,
    SHIELD_KNIGHT,
    BIG_KNIGHT,
    BIG_SHIELD_KNIGHT,
    FLAG_BEARER,
    BOSS
}

public enum EnemyState {
    APPROACHING,
    CONVERGING,
    STOPPED,
    LEAVING
}

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;

    protected EnemyState enemyState;

    [Header("Health Bar UI")]
    [SerializeField]
    private GameObject healthBar;       // Health bar background

    [Header("Enemy Properties")]
    public float maxHealth;
    private float currHealth;
    [SerializeField]
    protected float speed;
    public int moneyValue;
    [Range(0f, 1f)]
    public float itemDropChance;
    [SerializeField]
    protected SpriteRenderer[] bodyParts;     // The sprites of this enemy
    [SerializeField]
    protected Color hurtColour;    
    [SerializeField]
    protected Transform treasureCarryPoint;   // Marker for where on this enemy the treasure appears when being carried
    [SerializeField]
    protected float treasureSlowDown;     // Flat speed penalty if carrying the treasure
    protected bool carryingTreasure;
    [HideInInspector]
    public float speedBuffDuration;
    public float speedBuffBonus;

    [Space(10)]
    protected Animator animator;
    [HideInInspector]
    public List<Enemy> enemiesList;
    [HideInInspector]
    public float renderersHeight;
    [SerializeField]
    private GameObject defeatParticles;
    [SerializeField]
    private ParticleSystem speedLinesParticles;

    protected virtual void Start() {
        animator = GetComponent<Animator>();
        currHealth = maxHealth;
    }

    protected virtual void Update() {
        if (speedBuffDuration > 0) {
            if (!speedLinesParticles.isPlaying) {
                speedLinesParticles.Play();
            }
            speedBuffDuration -= Time.deltaTime;
        } else {
            speedLinesParticles.Stop();
        }
    }

    public float GetHealth() {
        return currHealth;
    }

    // Get effective health for entire enemy
    public virtual float GetGroupHealth() {
        return maxHealth;
    }

    private IEnumerator Flash() {
        foreach(SpriteRenderer sprite in bodyParts) {
            sprite.color = hurtColour;
        }
        yield return new WaitForSeconds(0.05f);
        foreach(SpriteRenderer sprite in bodyParts) {
            sprite.color = Color.white;
        }
    }

    public void TakeDamage(float value) {
        if (currHealth == maxHealth) {
            // Reveal health bar upon taking damage
            healthBar.SetActive(true);
        }
        currHealth -= value;
        healthBar.GetComponent<HealthBar>().SetFillAmount(currHealth/maxHealth);
        if(currHealth <= 0) {
            Die();
        } else {
            StartCoroutine(Flash());
        }
    }

    // Flips x of given transform's localPosition
    protected void FlipPosition(Transform t) {
        t.localPosition = new Vector3(-t.localPosition.x, t.localPosition.y, t.localPosition.z);
    }

    // Flips this enemy's sprite and their health bar's position
    public virtual void Flip() {
        foreach(SpriteRenderer sprite in bodyParts) {
            sprite.flipX = true;
            FlipPosition(sprite.transform);
        }
        FlipPosition(healthBar.transform);
        // Flip speed lines
        if (speedLinesParticles != null) {
            var shape = speedLinesParticles.shape;
            shape.rotation = new Vector3(shape.rotation.x, shape.rotation.y * -1, shape.rotation.z);
        }
    }

    // Have this enemy escape, for when they exit the level boundaries
    public virtual void Escape() {
        // If escaped with the treasure, lose wave
        if (carryingTreasure && gameManager.treasure.GetComponent<Treasure>().carried) {
            gameManager.waveManager.LoseWave();
            DropTreasure();
        }
        enemiesList.Remove(this);
        Destroy(gameObject);
    }

    public virtual void Die() {
        if (Random.value < itemDropChance + gameManager.playerTurret.dropBonus) {
            gameManager.DropAmmo(transform.position);
        }
        gameManager.IncreaseMoney(moneyValue);
        // If carrying the treasure, drop it at feet
        if (carryingTreasure) {
            DropTreasure();
            gameManager.player.OnTreasureDropped();
        }
        enemiesList.Remove(this);
        Instantiate(defeatParticles, defeatParticles.transform.position, defeatParticles.transform.rotation).SetActive(true);
        Destroy(gameObject);
    }

    protected void DropTreasure() {
        gameManager.treasure.GetComponent<Treasure>().carried = false;
        gameManager.treasure.parent = null;
        gameManager.treasure.position = transform.position;
    }

    public virtual void BuffSpeed(float duration) {
        speedBuffDuration = duration;
    }
}
