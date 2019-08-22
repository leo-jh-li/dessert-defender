using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunUi : MonoBehaviour
{
    [SerializeField]
    private int lowAmmoCount;       // Ammo quantity under which this gun is considered low on ammo
    [SerializeField]
    private Color lowAmmoColour;
    [SerializeField]
    private Color noAmmoColour;
    [SerializeField]
    private TextMeshProUGUI ammoDisplay;
    [SerializeField]
    private GameObject highlightedBorder;
    public void UpdateAmmo(float value) {
        int ammo = Mathf.CeilToInt(value);
        if (ammo >= lowAmmoCount) {
            ammoDisplay.color = Color.white;
        } else if (ammo != 0) {
            ammoDisplay.color = lowAmmoColour;
        } else {
            ammoDisplay.color = noAmmoColour;
        }
        ammoDisplay.text = ammo.ToString();
    }
}
