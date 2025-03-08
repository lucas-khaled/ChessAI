using TMPro;
using UnityEngine;

public class TurnPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text turnText;

    public void ShowTurn(IPlayer player) 
    {
        if (player is HumanPlayer)
            turnText.text = "It's your turn, pathetic human";
        else
            turnText.text = "It's my turn, and I'll show you how it's done!";
    }
}
