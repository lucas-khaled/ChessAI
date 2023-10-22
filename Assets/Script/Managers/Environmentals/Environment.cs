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
        var env = new Environment();

        env.board = board.Copy(env) as Board;
        env.boardManager = boardManager.Copy(env) as BoardManager;
        env.moveMaker = moveMaker.Copy(env) as MoveMaker;
        return env;
    }

    public void StartRealEnvironment(Board board) 
    {
        isVirtual = false;

        this.board = board;
        boardManager = new BoardManager(this);
        moveMaker = new MoveMaker(this);
        turnManager = new TurnManager();
        moveChecker = new MoveChecker();
    }
}
