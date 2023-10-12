using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager board;
    [SerializeField] private PiecesSetup setup;

    public static GameManager instance;

    public static BoardManager Board => instance.board;

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
        if (board != null)
            board.StartBoard();

        setup.SetInitialPieces();
    }
}
