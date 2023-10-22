using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : IEnvironmentable
{
    public Board board;
    public BoardManager boardManager;
    public MoveMaker moveMaker;
    public TurnManager turnManager;
    public MoveChecker moveChecker;

    public bool isVirtual;

    public IEnvironmentable Copy()
    {
        return new Environment()
        {
            board = board.Copy()
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
