using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text drawText;

    public void Show(DrawType type) 
    {
        gameObject.SetActive(true);
        drawText.text = "The match was draw by " + type.ToDescriptionString();
    }
}
