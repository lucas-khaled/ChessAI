using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CheckmatePopup checkmatePopup;
    [SerializeField] private PromotionPopup promotionPopup;
    [SerializeField] private DrawPopup drawPopup;
    [SerializeField] private TurnPanel turnPanel;

    private void Start()
    {
        turnPanel.gameObject.SetActive(false);
    }
    public void ShowTurn(IPlayer player) 
    {
        turnPanel.gameObject.SetActive(true);
        turnPanel.ShowTurn(player);
    }

    public void ShowCheckmateMessage(PieceColor winingColor)
    {
        turnPanel.gameObject.SetActive(false);
        checkmatePopup.Show(winingColor);
    }

    public void ShowDrawMessage(DrawType drawType)
    {
        turnPanel.gameObject.SetActive(false);
        drawPopup.Show(drawType);
    }

    public void ShowPromotionPopup(List<PromotionMove> promotions, Action<PromotionMove> onPromotionChoose = null) 
    {
        turnPanel.gameObject.SetActive(false);
        promotionPopup.Show(promotions, onPromotionChoose);
    }
}
