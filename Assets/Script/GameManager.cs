using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board;

    private static GameManager instance;

    public static Board Board => instance.board;

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
    }
}
