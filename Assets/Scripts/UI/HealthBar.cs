using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthContent;        // Health bar fill content
    [SerializeField]
    private Image healthContentTrail;   // Red trail under/following the health bar
    [SerializeField]
    private float trailSpeed;
    [SerializeField]
    private float freezeTrailTime;      // Delay before health trail starts to lerp to actual health
    private float remainingFreezeTrailTime;

    void Update()
    {
        remainingFreezeTrailTime -= Time.deltaTime;
        if (healthContent.fillAmount != healthContentTrail.fillAmount && remainingFreezeTrailTime <= 0) {
            healthContentTrail.fillAmount = Mathf.Max(healthContentTrail.fillAmount - trailSpeed, healthContent.fillAmount);
        }
    }

    public void SetFillAmount(float value) {
        healthContent.fillAmount = value;
        remainingFreezeTrailTime = freezeTrailTime;
    }
}
