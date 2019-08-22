using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCell : MonoBehaviour
{
    [SerializeField]
    private Image cellContent;
    [SerializeField]
    private Sprite emptyCellSprite;
    [SerializeField]
    private Sprite filledCellSprite;

    public void EmptyCell() {
        cellContent.sprite = emptyCellSprite;
    }

    public void FillCell() {
        cellContent.sprite = filledCellSprite;
    }
}
