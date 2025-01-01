using System;
using UnityEngine;

public class EspecialRules
{
    public CastleRights whiteCastleRights { get; private set; }
    public CastleRights blackCastleRights { get; private set; }


    public Tile enPassantTile { get; private set; }
    public Pawn enPassantPawn { get; private set; }

    public Board Board { get; private set; }

    public EspecialRules(Board board) 
    {
        whiteCastleRights = new();
        blackCastleRights = new();

        Board = board;
    }

    public EspecialRules Copy(Board board)
    {
        return new EspecialRules(board)
        {
            whiteCastleRights = this.whiteCastleRights.Copy(),
            blackCastleRights = this.blackCastleRights.Copy(),
            enPassantPawn = this.enPassantPawn,
            enPassantTile = this.enPassantTile
        };
    }

    public bool CanCastleQueenSide(PieceColor color) 
    {
        if (HasCastledAllSides(color)) return false;

        var castleRights = GetCastleRightsByColor(color);
        var queensideBool = castleRights.CanCastleQueenSide;
        var coord = GetRookCoordinates(true, color);

        return queensideBool && Board.GetTiles()[coord.row][coord.column].OccupiedBy is Rook rook && rook.pieceColor == color;
    }

    public bool CanCastleKingSide(PieceColor color) 
    {
        if (HasCastledAllSides(color)) return false;

        var castleRights = GetCastleRightsByColor(color);
        var kingsideBool = castleRights.CanCastleKingSide;
        var coord = GetRookCoordinates(false, color);

        return kingsideBool && Board.GetTiles()[coord.row][coord.column].OccupiedBy is Rook rook && rook.pieceColor == color;
    }

    public bool CanCastle(PieceColor color, Rook rook)
    {
        if (HasCastledAllSides(color)) return false;

        var coord = GetRookCoordinates(true, color);
        var castleRights = GetCastleRightsByColor(color);

        var queensideBool = castleRights.CanCastleQueenSide;
        if (rook.GetTile().TilePosition.Equals(coord) && queensideBool)
            return true;

        coord = GetRookCoordinates(false, color);
        var kingsideBool = castleRights.CanCastleKingSide;

        if (rook.GetTile().TilePosition.Equals(coord) && kingsideBool)
            return true;

        return false;
    }

    public bool HasCastledAllSides(PieceColor pieceColor) 
    {
        var castleRights = GetCastleRightsByColor(pieceColor);

        return castleRights.CanCastleKingSide is false && castleRights.CanCastleQueenSide is false;
    }

    public static TileCoordinates GetRookCoordinates(bool isQueenSide, PieceColor pieceColor) 
    {
        bool isWhite = pieceColor == PieceColor.White;

        if(isQueenSide)
            return isWhite ? new TileCoordinates(0, 0) : new TileCoordinates(7, 0);
        else
            return isWhite ? new TileCoordinates(0, 7) : new TileCoordinates(7, 7);
    }

    public void OnPieceMoved(Move move) 
    {
        if (move.piece is Pawn)
        {
            CheckEnPassant(move);
            return;
        }

        if (move is CastleMove || move.piece is King)
            SetKingMove(move);
        else if (move.piece is Rook)
            RookMoved(move);

        enPassantTile = null;
        enPassantPawn = null;
    }

    private void SetKingMove(Move move)
    {
        var castleRights = GetCastleRightsByColor(move.piece.pieceColor);

        if (castleRights.KingFirstMove == null && castleRights.WasSetByFEN is false)
            castleRights.SetKingMove(move);
    }

    private void CheckEnPassant(Move move)
    {
        TileCoordinates toCoord = move.to.TilePosition;
        var moveRange = Mathf.Abs(move.from.TilePosition.row - move.to.TilePosition.row);
        if (moveRange < 2)
        {
            enPassantTile = null;
            enPassantPawn = null;
            return;
        }

        var row = (move.piece.pieceColor == PieceColor.White) ? toCoord.row - 1 : toCoord.row + 1;
        Tile tile = Board.GetTiles()[row][toCoord.column];
        SetEnPassant(tile, move.piece as Pawn);
    }

    public void SetEnPassant(Tile tile, Pawn pawn) 
    {
        enPassantTile = tile;
        enPassantPawn = pawn;
    }

    private void RookMoved(Move rookMove)
    {
        var pieceColor = rookMove.piece.pieceColor;
        if (HasCastledAllSides(pieceColor)) return;

        var queensidePosition = GetRookCoordinates(true, pieceColor);
        var kingsidePosition = GetRookCoordinates(false, pieceColor);

        if (rookMove.from.TilePosition.Equals(queensidePosition)) 
        {
            var castleRights = GetCastleRightsByColor(pieceColor);

            if (castleRights.QueenRookFirstMove == null)
            {
                castleRights.SetQueenRookFirstMove(rookMove);
            }
            return;
        }

        if (rookMove.from.TilePosition.Equals(kingsidePosition))
        {
            var castleRights = GetCastleRightsByColor(pieceColor);

            if (castleRights.KingRookFirstMove == null)
            {
                castleRights.SetKingRookFirstMove(rookMove);
            }
            return;
        }
    }

    public void SetCastleKingSide(PieceColor color, bool can, bool setByFEN = false) 
    {
        if (color == PieceColor.White)
        {
            whiteCastleRights.CanCastleKingSide = can;
            whiteCastleRights.WasSetByFEN = setByFEN;
        }
        else
        {
            blackCastleRights.CanCastleKingSide = can;
            blackCastleRights.WasSetByFEN = setByFEN;
        }
    }

    public void SetCastleQueenSide(PieceColor color, bool can, bool setByFEN = false)
    {
        if (color == PieceColor.White)
        {
            whiteCastleRights.CanCastleQueenSide = can;
            whiteCastleRights.WasSetByFEN = setByFEN;
        }
        else
        {
            blackCastleRights.CanCastleQueenSide = can;
            blackCastleRights.WasSetByFEN = setByFEN;
        }
    }

    public void OnPieceUnmoved(Move move)
    {
        enPassantPawn = null;
        enPassantTile = null;

        if (move is CastleMove || move.piece is King)
            UndoKingMove(move);
        if (move.piece is Rook)
            UndoRookMove(move);
    }

    private void UndoKingMove(Move move)
    {
        var castleRights = GetCastleRightsByColor(move.piece.pieceColor);
        bool undo = castleRights.KingFirstMove != null && castleRights.WasSetByFEN is false && castleRights.KingFirstMove.Equals(move);

        if (undo)
            castleRights.SetKingMove(null);
    }

    private void UndoRookMove(Move move)
    {
        var color = move.piece.pieceColor;
        var castleRights = GetCastleRightsByColor(color);

        if (castleRights.KingRookFirstMove != null && castleRights.WasSetByFEN is false && castleRights.KingRookFirstMove.Equals(move)) 
            castleRights.SetKingRookFirstMove(null);

        if (castleRights.QueenRookFirstMove != null && castleRights.WasSetByFEN is false && castleRights.QueenRookFirstMove.Equals(move))
            castleRights.SetQueenRookFirstMove(null);
    }

    public CastleRights GetCastleRightsByColor(PieceColor color) 
    {
        return (color == PieceColor.White) ? whiteCastleRights : blackCastleRights;
    }

    public class CastleRights 
    {
        public bool WasSetByFEN { get; set; }
        public bool CanCastleKingSide { get; set; }
        public bool CanCastleQueenSide { get; set; }

        public Move KingFirstMove { get; private set; }
        public Move QueenRookFirstMove { get; private set; }
        public Move KingRookFirstMove { get; private set; }

        public CastleRights()
        {
            CanCastleKingSide = true;
            CanCastleQueenSide = true;
        }

        public CastleRights Copy() 
        {
            return new CastleRights()
            {
                WasSetByFEN = false,
                CanCastleKingSide = CanCastleKingSide,
                CanCastleQueenSide = CanCastleQueenSide,
                KingFirstMove = KingFirstMove,
                QueenRookFirstMove = QueenRookFirstMove,
                KingRookFirstMove = KingRookFirstMove
            };
        }

        public void SetCastleOnBothSides(bool canCastle) 
        {
            CanCastleKingSide = canCastle;
            CanCastleQueenSide = canCastle;
        }

        public void SetKingRookFirstMove(Move move) 
        {
            KingRookFirstMove = move;
            CanCastleKingSide = move == null;
        }

        public void SetQueenRookFirstMove(Move move)
        {
            QueenRookFirstMove = move;
            CanCastleQueenSide = move == null;
        }

        public void SetKingMove(Move move) 
        {
            SetCastleOnBothSides(move == null);
            KingFirstMove = move;
        }
    }
}
