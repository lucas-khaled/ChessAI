using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : BlockableMovesPiece
{
    private TileCoordinates initialTile;

    private CheckChecker checkChecker = new();

    public King(Environment env) : base(env)
    {
        
    }

    public override Move[] GetMoves()
    {
        List<Move> moves = new List<Move>();

        var horizontal = GetHorizontalMoves(1);
        var vertical = GetVerticalMoves(1);
        var diagonals = GetDiagonalMoves(1);

        moves.AddRange(horizontal);
        moves.AddRange(vertical);
        moves.AddRange(diagonals);
        moves.AddRange(GetCastleMoves());

        return moves.ToArray();
    }

    private List<CastleMove> GetCastleMoves() 
    {
        List<CastleMove> moves = new List<CastleMove>();

        if (checkChecker.IsCheck(Environment, pieceColor)) return moves;
        
        var tiles = Environment.boardManager.GetRookTiles(pieceColor);

        foreach(var tile in tiles) 
            CheckRook(tile, ref moves);

        return moves;
    }

    private void CheckRook(Tile tile, ref List<CastleMove> moves) 
    {
        if (Environment.rules.CanCastle(pieceColor, tile.OccupiedBy as Rook) is false) return;

        var range = tile.TilePosition.column - Column;

        var inBetweenTiles = (range > 0) ?
            actualTile.GetHorizontalsByColor(pieceColor).rightHorizontals ://Environment.boardManager.GetRightHorizontals(Coordinates) :
            actualTile.GetHorizontalsByColor(pieceColor).leftHorizontals;

        var blockingCheckedBetween = CheckForBlockingSquares(inBetweenTiles, includeBlockingPieceSquare: true);

        if (blockingCheckedBetween[blockingCheckedBetween.Count - 1].TilePosition.Equals(tile.TilePosition))
            moves.Add(CreateCastleMove(tile, range));
    }

    private CastleMove CreateCastleMove(Tile rookTile, int range) 
    {
        var iterator = (range > 0) ? 1 : -1;

        Tile rookToTile = Environment.board.GetTiles()[Row][Column + iterator]; 
        Move rookMove = new Move(rookTile, rookToTile, rookTile.OccupiedBy);

        Tile toTile = Environment.board.GetTiles()[Row][Column + iterator * 2];

        return new CastleMove(actualTile, toTile, this, rookMove);
    }
}
