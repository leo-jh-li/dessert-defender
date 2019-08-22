using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeOption : MonoBehaviour
{
    public UpgradeType upgradeType;
    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private string upgradeText;
    [SerializeField]
    private TextMeshProUGUI label;
    [SerializeField]
    private Transform upgradeCellsContainer;
    [SerializeField]
    private TextMeshProUGUI buffAmountText;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private GameObject cellPrefab;
    private UpgradeCell[] cells;    // Reference to this upgrade's cells

    public void InstantiateCells(int quantity) {
        cells = new UpgradeCell[quantity];
        float cellWidth = cellPrefab.GetComponent<RectTransform>().rect.width;
        for(int i = 0; i < quantity; i++) {
            UpgradeCell newCell = Instantiate(cellPrefab, upgradeCellsContainer).GetComponent<UpgradeCell>();
            newCell.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(cellWidth * i, 0, 0);
            cells[i] = newCell;
        }
    }

    public void FillCells(int level) {
        for(int i = 0; i < level && i < cells.Length; i++) {
            cells[i].FillCell();
        }
        for(int i = level; i < cells.Length; i++) {
            cells[i].EmptyCell();
        }
    }

    public void SetUpgradeInfo(int level, float totalPercent, float nextBuffAmount, int cost, bool maxed) {
        label.text = "+" + (totalPercent * 100).ToString() + "% " + upgradeText;
        FillCells(level);
        buffAmountText.text = "+" + (nextBuffAmount * 100).ToString() + "%";
        if (maxed) {
            costText.text = "—";
            costText.fontStyle = FontStyles.Bold;
        } else {
            costText.text = cost.ToString();
            costText.fontStyle = FontStyles.Normal;
        }
    }

    public void SetInteractable(bool interactable) {
        if (interactable) {
            upgradeButton.interactable = true;
        } else {
            upgradeButton.interactable = false;
        }
    }
}
