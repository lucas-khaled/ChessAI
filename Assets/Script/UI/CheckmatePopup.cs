using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckmatePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text checkmateText;

    private const string CHECKMATE_BASE_TEXT = "It's checkmate! {0} won!!";

    public void Show(PieceColor winingColor)
    {
        gameObject.SetActive(true);

        var text = string.Format(CHECKMATE_BASE_TEXT, winingColor.ToString());
        checkmateText.SetText(text);
    }

    public void Hide() 
    {
        gameObject.SetActive(true);
    }
}
