using UnityEngine;

public class InitialMenu : MonoBehaviour
{
    [SerializeField] private GameObject initialPanel;
    [SerializeField] private PlayersConfig playersConfig;
    [SerializeField] private GameManager gameManager;

    private string colorChoice = "random";
    public void SetColorChoice(string colorChoice) 
    {
        this.colorChoice = colorChoice;
    }

    public void StartGame() 
    {
        initialPanel.SetActive(false);

        IPlayer white = playersConfig.GetFirstPlayer(gameManager);
        IPlayer black = playersConfig.GetSecondPlayer(gameManager);

        bool random = false;

        if (colorChoice.ToLower() == "black")
        {
            var temp = white;
            white = black;
            black = temp;
        }
        else if (colorChoice.ToLower() == "random")
            random = true;

        
        gameManager.Initialize(white, black, random);
    }
}
