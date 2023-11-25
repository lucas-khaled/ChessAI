using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text checkmateText;
    [SerializeField] private GameObject checkmatePanel;

    public void ShowCheckmateMessage(PieceColor winingColor) 
    {
        checkmatePanel.SetActive(true);

        var text = string.Format(checkmateText.text, winingColor.ToString());
        checkmateText.SetText(text);
    }
}
