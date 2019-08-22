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
}
