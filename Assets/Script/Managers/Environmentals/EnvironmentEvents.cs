using System;

public class EnvironmentEvents
{
    public Action<Move> onMoveMade;
    public Action<Piece> onPieceCaptured;
}