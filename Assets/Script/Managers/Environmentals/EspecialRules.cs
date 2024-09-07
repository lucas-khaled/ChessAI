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

        board.events.onMoveMade += OnPieceMoved;
        board.events.onMoveUnmade += OnPieceUnmoved;

        Board = board;
    }

    public EspecialRules Copy(Board board)
    {
        return new EspecialRules(board)
        {
            whiteCastleRights = this.whiteCastleRights,
            blackCastleRights = this.blackCastleRights,
            enPassantPawn = this.enPassantPawn,
            enPassantTile = this.enPassantTile
        };
    }

    public bool CanCastle(PieceColor color, Rook rook)
    {
        if (HasCastledAllSides(color)) return false;

        var coord = GetRookCoordinates(true, color);
        var castleRights = (color == PieceColor.White) ? whiteCastleRights : blackCastleRights;

        var queensideBool = castleRights.CanCastleQueenSide;
        if (rook.GetTile().TilePosition.Equals(coord) && queensideBool)
            return true;

        coord = GetRookCoordinates(false, color);
        var kingsideBool = castleRights.CanCastleKingSide;

        if (rook.GetTile().TilePosition.Equals(coord) && kingsideBool)
            return true;

        return false;
    }

    private bool HasCastledAllSides(PieceColor pieceColor) 
    {
        var castleRights = pieceColor == PieceColor.White ? whiteCastleRights : blackCastleRights;

        return castleRights.CanCastleKingSide is false && castleRights.CanCastleQueenSide is false;
    }

    private TileCoordinates GetRookCoordinates(bool isQueenSide, PieceColor pieceColor) 
    {
        bool isWhite = pieceColor == PieceColor.White;

        if(isQueenSide)
            return isWhite ? new TileCoordinates(0, 0) : new TileCoordinates(7, 0);
        else
            return isWhite ? new TileCoordinates(0, 7) : new TileCoordinates(7, 7);
    }

    private void OnPieceMoved(Move move) 
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
        Debug.Log($"<color=red>[Rules {Board.Name}] Setting King first move {move}</color>");

        var castleRights = move.piece.pieceColor == PieceColor.White ? whiteCastleRights : blackCastleRights;
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
            var castleRights = (pieceColor == PieceColor.White) ? whiteCastleRights : blackCastleRights;

            if (castleRights.QueenRookFirstMove == null)
            {
                if (Board.Name == GameManager.GAME_BOARD_NAME)
                    Debug.Log($"<color=red>[Rules {Board.Name}] Setting Queen rook first move {rookMove} </color>");
                castleRights.SetQueenRookFirstMove(rookMove);
            }
            return;
        }

        if (rookMove.from.TilePosition.Equals(kingsidePosition))
        {
            var castleRights = (pieceColor == PieceColor.White) ? whiteCastleRights : blackCastleRights;

            if (castleRights.KingRookFirstMove == null)
            {
                if(Board.Name == GameManager.GAME_BOARD_NAME)
                    Debug.Log($"<color=red>[Rules {Board.Name}] Setting King rook first move {rookMove} </color>");
                castleRights.SetKingRookFirstMove(rookMove);
            }
            return;
        }
    }

    public void SetCastleKingSide(PieceColor color, bool can) 
    {
        Debug.Log($"<color=red>[Rules {Board.Name}] Setting Castle Kingside {can}</color>");
        if (color == PieceColor.White)
            whiteCastleRights.CanCastleKingSide = can;
        else
            blackCastleRights.CanCastleKingSide = can;
    }

    public void SetCastleQueenSide(PieceColor color, bool can)
    {
        Debug.Log($"<color=red>[Rules {Board.Name}] Setting Castle Queenside {can}</color>");
        if (color == PieceColor.White)
            whiteCastleRights.CanCastleQueenSide = can;
        else
            blackCastleRights.CanCastleQueenSide = can;
    }

    private void OnPieceUnmoved(Move move)
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
        var castleRights = (move.piece.pieceColor == PieceColor.White) ? whiteCastleRights : blackCastleRights;
        bool undo = castleRights.KingFirstMove != null && castleRights.KingFirstMove.Equals(move);

        Debug.Log($">>> KING FIRST MOVE IS {castleRights.KingFirstMove}\n MOVE DONE WAS {move} \nSHOULD UNDO? {undo}");
        if (undo)
            castleRights.SetKingMove(null);
    }

    private void UndoRookMove(Move move)
    {
        var color = move.piece.pieceColor;
        var castleRights = (color == PieceColor.White) ? whiteCastleRights : blackCastleRights;

        if (castleRights.KingRookFirstMove != null && castleRights.KingRookFirstMove.Equals(move)) 
            castleRights.SetKingRookFirstMove(null);

        if (castleRights.QueenRookFirstMove != null && castleRights.QueenRookFirstMove.Equals(move))
            castleRights.SetQueenRookFirstMove(null);
    }

    public class CastleRights 
    {
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
            Debug.Log("KING MOVE: "+move + "\nFirst Move: "+KingFirstMove);
            SetCastleOnBothSides(move == null);
            KingFirstMove = move;
        }
    }
}
