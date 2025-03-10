using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class MoveGenerator
{
    private Board board;

    private Bitboard enemiesAttackingSquares = new Bitboard();
    private Bitboard enemiesKingDangerSquares = new Bitboard();

    private Bitboard kingAttackersSquaresBitboard = new Bitboard();
    private Bitboard inBetweenKingAndAttackersBitboard = new Bitboard();

    private Dictionary<PinnerPiece, Bitboard> enemyInBetweenPinners = new Dictionary<PinnerPiece, Bitboard>();

    private Bitboard piecesPositionBitboard = new Bitboard();
    private Bitboard enemyPiecesPositionBitboard = new Bitboard();

    private Bitboard queenSideCastleWhiteBitboard = new Bitboard(1) |new Bitboard(2) | new Bitboard(3);
    private Bitboard queenSideAttackCastleWhiteBitboard = new Bitboard(2) | new Bitboard(3);
    private Bitboard kingSideCastleWhiteBitboard = new Bitboard(5) | new Bitboard(6);

    private Bitboard queenSideCastleBlackBitboard = new Bitboard(57) | new Bitboard(58) | new Bitboard(59);
    private Bitboard queenSideAttackCastleBlackBitboard = new Bitboard(58) | new Bitboard(59);
    private Bitboard kingSideCastleBlackBitboard = new Bitboard(61) | new Bitboard(62);

    private Piece[] myPieces;
    private Piece[] enemyPieces;
    private List<Piece> kingAttackers;
    private Piece kingPiece;
    private PieceColor actualColor;

    public MoveGenerator(Board board) 
    {
        this.board = board;
    }

    public List<Move> GenerateMoves(PieceColor color) 
    {
        List <Move> moves = new List<Move>();

        Profiler.BeginSample("Move Generation > Initialize");
        Initialize(color);
        Profiler.EndSample();

        Profiler.BeginSample("Move Generation > GenerateBitboards");
        GenerateBitboards();
        Profiler.EndSample();

        if (IsCheck())
        {
            moves.AddRange(GenerateKingMoves());
            if (IsDoubleCheck())
                return moves;

            moves.AddRange(GenerateCapturesAndBlocks());

            return moves;
        }
        else 
        {
            Profiler.BeginSample("Move Generation > GeneratePiecesMove");
            moves.AddRange(GeneratePiecesMove());
            Profiler.EndSample();

            return moves;
        }
    }

    private List<Move> GenerateCapturesAndBlocks()
    {
        List<Move> moves = new();

        foreach (var piece in myPieces)
        {
            if (piece is King || piece.PinnedBy != null) continue;

            var enPassantBitboard = (piece is Pawn && board.rules.HasEnPassant) 
                ? new Bitboard(board.rules.enPassantTileCoordinates.row * board.BoardRowSize + board.rules.enPassantTileCoordinates.column) 
                : new Bitboard();

            var validBitboard = piece.MovingSquares & (kingAttackersSquaresBitboard | inBetweenKingAndAttackersBitboard | enPassantBitboard);

            if (validBitboard <= 0) continue;
            FillMovesFromPiece(moves, piece, validBitboard);
        }

        return moves;
    }

    private void FillMovesFromPiece(List<Move> moves, Piece piece, Bitboard movesBitboard)
    {
        Profiler.BeginSample("Move Generation > GeneratePiecesMove > FillMovesFromPiece > ConvertToIndexes");
        var indexes = movesBitboard.ConvertToIndexes();
        Profiler.EndSample();

        foreach (var index in indexes)
        {
            var toTile = board.GetTileByIndex(index);
            Profiler.BeginSample("Move Generation > GeneratePiecesMove > FillMovesFromPiece > GeneratePieceMove");
            GeneratePieceMove(moves, piece, toTile);
            Profiler.EndSample();
        }
    }

    private void GeneratePieceMove(List<Move> moves, Piece piece, Tile toTile)
    {
        if (WillRevealAttack(piece.GetTile().Bitboard, toTile.Bitboard)) 
            return;

        if(piece is King king) 
        {
            if (IsImpossibleForKing(toTile)) return;

            int columnDiff = king.Coordinates.column - toTile.TilePosition.column;
            if (Mathf.Abs(columnDiff) == 2)
            {
                int columnDeltaSign = (int)Mathf.Sign(columnDiff);
                bool isQueenSide = columnDeltaSign > 0;
                bool canCastle = (isQueenSide) ? board.rules.CanCastleQueenSide(piece.pieceColor) : board.rules.CanCastleKingSide(piece.pieceColor);
                if (!canCastle) return;

                Bitboard attackCheckBitboard = GetCastleAttackCheckBitboard(piece.pieceColor, isQueenSide);
                Bitboard pieceCheckBitBoard = GetCastleCheckBitboard(piece.pieceColor, isQueenSide);
                if (HasAnyAttackIn(attackCheckBitboard) || HasAnyPieceIn(pieceCheckBitBoard)) return;

                var rookCoord = EspecialRules.GetRookCoordinates(isQueenSide, piece.pieceColor);
                Piece rookPiece = board.GetTiles()[rookCoord.row][rookCoord.column].OccupiedBy;
                if (rookPiece is null || rookPiece is not Rook rook) return;

                Move rookMove = new Move(rook.GetTile(), board.GetTiles()[king.Coordinates.row][king.Coordinates.column - columnDeltaSign], rook);
                CastleMove castleMove = new CastleMove(king.GetTile(), toTile, king, rookMove);
                moves.Add(castleMove);
                return;
            }
        }
        else if(piece is Pawn pawn) 
        {
            if (IsPromotion(pawn)) 
            {
                PromotionMove[] promotions = GetPossiblePromotions(pawn, toTile);
                moves.AddRange(promotions);
                return;
            }

            if (board.rules.HasEnPassant && board.rules.enPassantTileCoordinates.Equals(toTile.TilePosition)) 
            {
                int offset = (piece.pieceColor == PieceColor.White) ? -1 : 1;
                Piece enPassantPiece = board.GetTiles()[toTile.TilePosition.row+offset][toTile.TilePosition.column].OccupiedBy;

                if (WillRevealAttack(piece.GetTile().Bitboard | enPassantPiece.GetTile().Bitboard, toTile.Bitboard)) return;
                
                EnPassantMove enPassantCapture = new EnPassantMove(piece.GetTile(), toTile, pawn, enPassantPiece);
                moves.Add(enPassantCapture);
                return;
            }
        }

        Move move = new Move(piece.GetTile(), toTile, piece, toTile.OccupiedBy);
        moves.Add(move);
    }

    private bool WillRevealAttack(Bitboard willLeave, Bitboard willGo)
    {
        foreach (var inBetween in enemyInBetweenPinners) 
        {
            Bitboard afterLeaveBoard = new Bitboard(inBetween.Value);
            afterLeaveBoard.Remove(willLeave);
            afterLeaveBoard.Remove(inBetween.Key.GetTile().Bitboard);

            if ((HasAnyPieceIn(afterLeaveBoard) is false) && ((willGo & inBetween.Value) <= 0)) return true;
        }

        return false;
    }

    private PromotionMove[] GetPossiblePromotions(Pawn pawn, Tile toTile)
    {
        return new PromotionMove[4]
        {
            new PromotionMove(pawn.GetTile(), toTile, pawn, new Rook(board), toTile.OccupiedBy),
            new PromotionMove(pawn.GetTile(), toTile, pawn, new Bishop(board), toTile.OccupiedBy),
            new PromotionMove(pawn.GetTile(), toTile, pawn, new Knight(board), toTile.OccupiedBy),
            new PromotionMove(pawn.GetTile(), toTile, pawn, new Queen(board), toTile.OccupiedBy)
        };
    }

    private bool IsPromotion(Pawn pawn)
    {
        return (pawn.Coordinates.row == 6 && pawn.pieceColor == PieceColor.White)
            || (pawn.Coordinates.row == 1 && pawn.pieceColor == PieceColor.Black);
    }

    private bool HasAnyAttackIn(Bitboard checkBitboard)
    {
        return (enemiesAttackingSquares & checkBitboard) > 0;
    }

    private bool HasAnyPieceIn(Bitboard checkBitboard)
    {
        return ((enemyPiecesPositionBitboard | piecesPositionBitboard) & checkBitboard) > 0;
    }

    private Bitboard GetCastleAttackCheckBitboard(PieceColor pieceColor, bool isQueenSide)
    {
        return (pieceColor == PieceColor.White)
            ? (isQueenSide)
                ? queenSideAttackCastleWhiteBitboard : kingSideCastleWhiteBitboard
            : (isQueenSide)
                ? queenSideAttackCastleBlackBitboard : kingSideCastleBlackBitboard;
    }

    private Bitboard GetCastleCheckBitboard(PieceColor pieceColor, bool isQueenSide)
    {
        return (pieceColor == PieceColor.White)
            ? (isQueenSide)
                ? queenSideCastleWhiteBitboard : kingSideCastleWhiteBitboard
            : (isQueenSide)
                ? queenSideCastleBlackBitboard : kingSideCastleBlackBitboard;
    }

    private void Initialize(PieceColor color)
    {
        actualColor = color;
        myPieces = board.GetAllPieces(color);
        enemyPieces = board.GetAllPieces(color.GetOppositeColor());

        kingPiece = board.GetKingTile(color).OccupiedBy;
        kingAttackers = new();
    }

    private void GenerateBitboards()
    {
        enemyInBetweenPinners.Clear();
        inBetweenKingAndAttackersBitboard.Clear();
        enemiesAttackingSquares.Clear();
        enemiesKingDangerSquares.Clear();
        piecesPositionBitboard.Clear();
        enemyPiecesPositionBitboard.Clear();

        GenerateMyBitboards();
        GenerateEnemyBitboards();
    }

    private void GenerateEnemyBitboards()
    {
        kingAttackersSquaresBitboard = new Bitboard();

        foreach (var piece in enemyPieces)
        {
            enemyPiecesPositionBitboard.Add(piece.GetTile().Bitboard);

            piece.GenerateBitBoard();
            enemiesAttackingSquares.Add(piece.AttackingSquares);

            if (piece is PinnerPiece pinner)
            {
                if(pinner.InBetweenSquares > 0)
                    enemyInBetweenPinners.Add(pinner, pinner.InBetweenSquares);

                enemiesKingDangerSquares.Add(pinner.KingDangerSquares);
            }

            if ((piece.AttackingSquares & kingPiece.GetTile().Bitboard) > 0)
            {
                kingAttackersSquaresBitboard.Add(piece.GetTile().Bitboard);
                kingAttackers.Add(piece);

                GenerateInBetwwen(piece);
            }
        }
    }

    private void GenerateInBetwwen(Piece piece)
    {
        if (piece is not SlidingPieces || piece is Pawn) return;

        TileCoordinates coord = piece.Coordinates;
        TileCoordinates kingCoord = kingPiece.Coordinates;

        int pieceIndex = piece.GetTile().Index;
        int kingIndex = kingPiece.GetTile().Index;

        int delta = pieceIndex - kingIndex;
        int rate;

        if (coord.row == kingCoord.row)
            rate = 1;
        else if (coord.column == kingCoord.column)
            rate = 8;
        else
            rate = (delta % 9 == 0) ? 9 : 7;
        
        int difference = delta / rate;

        if (Mathf.Abs(difference) == 1) return;

        for (int i = (int)Mathf.Sign(difference); Mathf.Abs(i) < Mathf.Abs(difference); i += (int)Mathf.Sign(difference))
        {
            int index = kingIndex + i * rate;
            Tile tile = board.GetTileByIndex(index);

            inBetweenKingAndAttackersBitboard.Add(tile.Bitboard);
        }
    }

    private void GenerateMyBitboards()
    {
        foreach (var piece in myPieces)
        {
            piecesPositionBitboard.Add(piece.GetTile().Bitboard);

            piece.GenerateBitBoard();
        }
    }

    public bool IsCheck()
    {
        return kingAttackers.Count > 0;
    }

    public bool IsDoubleCheck() 
    {
        return kingAttackers.Count > 1;
    }

    private List<Move> GenerateKingMoves() 
    {
        var moves = new List<Move>();
        var possibleTiles = new List<TileCoordinates>();

        var kingTile = kingPiece.GetTile();
        var verticals = kingTile.GetVerticalsByColor(actualColor);
        var horizontals = kingTile.GetHorizontalsByColor(actualColor);
        var diagonals = kingTile.GetDiagonalsByColor(actualColor);

        if(verticals.frontVerticals.Count>0) possibleTiles.Add(verticals.frontVerticals.First());
        if(verticals.backVerticals.Count > 0) possibleTiles.Add(verticals.backVerticals.First());
        if(horizontals.rightHorizontals.Count > 0) possibleTiles.Add(horizontals.rightHorizontals.First());
        if (horizontals.leftHorizontals.Count > 0) possibleTiles.Add(horizontals.leftHorizontals.First());
        if (diagonals.topRightDiagonals.Count > 0) possibleTiles.Add(diagonals.topRightDiagonals.First());
        if (diagonals.topLeftDiagonals.Count > 0) possibleTiles.Add(diagonals.topLeftDiagonals.First());
        if (diagonals.downLeftDiagonals.Count > 0) possibleTiles.Add(diagonals.downLeftDiagonals.First());
        if (diagonals.downRightDiagonals.Count > 0) possibleTiles.Add(diagonals.downRightDiagonals.First());

        
        foreach (var tileCoord in possibleTiles)
        {
            var tile = board.GetTiles()[tileCoord.row][tileCoord.column];
            if (IsImpossibleForKing(tile)) continue;

            if (tile.IsOccupied is false || (tile.OccupiedBy.pieceColor != actualColor))
                moves.Add(new Move(kingTile, tile, kingPiece, tile.OccupiedBy));
        }

        return moves;
    }

    private bool IsImpossibleForKing(Tile tile)
    {
        var impossibleSquares = enemiesKingDangerSquares | enemiesAttackingSquares;
        return (impossibleSquares & tile.Bitboard) > 0;
    }

    private List<Move> GeneratePiecesMove() 
    {
        List<Move> moves = new();
        foreach (Piece piece in myPieces) 
        {
            var filteredMoves = new Bitboard(piece.MovingSquares);
            filteredMoves.Remove(piecesPositionBitboard);

            if (filteredMoves <= 0) continue;

            Profiler.BeginSample("Move Generation > GeneratePiecesMove > FillMovesFromPiece");
            FillMovesFromPiece(moves, piece, filteredMoves);
            Profiler.EndSample();
        }

        return moves;
    }

    public Bitboard GetCurrentBoardBitboard() 
    {
        return GetCurrentPiecesBitboard() | GetEnemyPiecesBitboard();
    }

    public Bitboard GetCurrentPiecesBitboard() 
    {
        return piecesPositionBitboard;
    }

    public Bitboard GetEnemyPiecesBitboard()
    {
        return enemyPiecesPositionBitboard;
    }
}
