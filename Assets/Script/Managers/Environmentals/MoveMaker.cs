using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaker : IEnvironmentable
{
    public Environment Environment { get; }
    public MoveMaker(Environment env) 
    {
        Environment = env;
    }


    public IEnvironmentable Copy(Environment environment)
    {
        return new MoveMaker(environment);
    }

    public Move[] GetMoves(Piece piece) 
    {
        var pieceMoves = piece.GetMoves();
        
        return Environment.moveChecker.GetLegalMoves(pieceMoves);
    }
}
