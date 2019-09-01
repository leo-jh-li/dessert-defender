using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [Header("Treasure Properties")]
    public string treasureName;
    public Sprite filledLifeSprite;
    public Sprite emptyLifeSprite;
    public Sprite gameOverSprite;

    [Space(10)]
    public bool carried;
    public bool lost;       // Set to true if treasure has been stolen this wave

    public bool CanBePickedUp() {
        return !carried && !lost;
    }
}
