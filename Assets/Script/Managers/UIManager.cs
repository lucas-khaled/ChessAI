using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CheckmatePopup checkmatePopup;
    [SerializeField] private PromotionPopup promotionPopup;
    [SerializeField] private DrawPopup drawPopup;

    public void ShowCheckmateMessage(PieceColor winingColor)
    {
        checkmatePopup.Show(winingColor);
    }

    public void ShowDrawMessage(DrawType drawType)
    {
        drawPopup.Show(drawType);
    }

    public void ShowPromotionPopup(List<PromotionMove> promotions, Action<PromotionMove> onPromotionChoose = null) 
    {
        promotionPopup.Show(promotions, onPromotionChoose);
    }
}
