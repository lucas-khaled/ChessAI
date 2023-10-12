using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PiecesSetup setup;

    public static GameManager instance;

    public static Board Board { get; private set; }

    private void Awake()
    {
        if(instance != null) 
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (boardManager != null)
            Board = boardManager.StartNewBoard();

        setup.SetInitialPieces();
    }
}
