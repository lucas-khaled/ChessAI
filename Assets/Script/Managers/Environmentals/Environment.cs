using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment
{
    public Board board;
    public BoardManager boardManager;
    public MoveMaker moveMaker;
    public TurnManager turnManager;
    public MoveChecker moveChecker;

    public bool isVirtual;

    public Environment Copy()
    {
        return new Environment()
        {
            board = board.Copy() as Board,

        };
    }

    public void StartRealEnvironment(Board board) 
    {
        isVirtual = false;

        this.board = board;
        boardManager = new BoardManager();
        moveMaker = new MoveMaker();
        turnManager = new TurnManager();
        moveChecker = new MoveChecker();
    }
}
