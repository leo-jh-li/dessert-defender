using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoDrop : MonoBehaviour
{
    [SerializeField]
    private float animSpeed;        // Speed of UI moving up during animation
    [SerializeField]
    private float totalAnimTime;
    private float remainingAnimTime;     // Remaining time of animation
    [SerializeField]
    private float opaqueTime;    // Duration for which UI's alpha is unchanged
    [SerializeField]
    private Image iconSprite;
    private Color colour;
    private Color fadedColour;
    private Color32 textColor;
    private Color32 fadedTextColor;

    [SerializeField]
    private TextMeshProUGUI multText;
    [SerializeField]
    private TextMeshProUGUI quantityDisplay;

    void Start()
    {
        remainingAnimTime = totalAnimTime;
        textColor = new Color(255, 255, 255, 255);
        fadedTextColor = new Color(255, 255, 255, 0);
    }

    void Update() {
        remainingAnimTime -= Time.deltaTime;
        transform.position += new Vector3(0, animSpeed, 0);
        if (totalAnimTime - remainingAnimTime > opaqueTime) {
            SetAlpha();
        }
        if (remainingAnimTime <= 0) {
            Destroy(gameObject);
        }
    }

    // Set ammo drop's alpha based on time elapsed
    private void SetAlpha() {
        float interpolant = (totalAnimTime - opaqueTime - remainingAnimTime) / (totalAnimTime - opaqueTime);
        iconSprite.color = Color.Lerp(colour, fadedColour, interpolant);
        multText.color = Color.Lerp(textColor, fadedTextColor, interpolant);
        quantityDisplay.color = Color.Lerp(textColor, fadedTextColor, interpolant);
    }

    public void SetIconColour(Color colour) {
        iconSprite.color = colour;
        this.colour = colour;
        fadedColour = colour;
        fadedColour.a = 0;
    }

    public void SetQuantityDisplay(int quantity) {
        quantityDisplay.text = quantity.ToString();
    }
}
