using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Board
{
    public int BoardRowSize;
    public int BoardColumnSize;

    public List<List<Tile>> tiles;
    public PiecesHolder piecesHolder;

    public EspecialRules rules;
    public BoardEvents events;

    public string ActualHash;

    public MoveGenerator moveGenerator { get; private set; }
    public List<Turn> turns { get; private set; } = new List<Turn>();
    public Turn LastTurn => turns.Count > 0 ? turns[turns.Count - 1] : new Turn();
    public PieceColor ActualTurn { get; private set; } = PieceColor.White;
    public FENManager FENManager { get; private set; }

    public List<Move> currentTurnMoves { get; private set; }
    public bool IsCheckMate { get; private set; }
    public bool HasMoves { get; private set; }

    public string Name { get; set; }

    public Board(int row, int column)
    {
        BoardRowSize = row;
        BoardColumnSize = column;
        tiles = new();
        piecesHolder = new();
        events = new BoardEvents();
        rules = new EspecialRules(this);
        FENManager = new FENManager(this);
        moveGenerator = new MoveGenerator(this);
    }

    public Board Copy()
    {
        List<List<Tile>> virtualTiles = new List<List<Tile>>();
        PiecesHolder pieces = new PiecesHolder();
        List<Piece> whitePieces = new List<Piece>();
        List<Piece> blackPieces = new List<Piece>();
        Board board = new Board(BoardRowSize, BoardColumnSize);
        board.Name = "Copy board";

        foreach (var list in tiles)
        {
            List<Tile> virtualList = new();

            foreach (var tile in list)
            {
                var copyTile = tile.Copy(board);
                virtualList.Add(copyTile);

                if (tile.IsOccupied)
                {
                    Piece piece = copyTile.OccupiedBy;
                    pieces.AddPiece(piece);

                    if (piece.pieceColor == PieceColor.White)
                        whitePieces.Add(piece);
                    else
                        blackPieces.Add(piece);
                }
            }

            virtualTiles.Add(virtualList);
        }

        board.tiles = virtualTiles;

        board.currentTurnMoves = new List<Move>();
        for (int i = 0; i<currentTurnMoves.Count; i++) 
        {
            var move = currentTurnMoves[i].VirtualizeTo(board);
            board.currentTurnMoves.Add(move);
        }

        board.piecesHolder = pieces;
        board.rules = rules.Copy(board);
        board.ActualHash = ActualHash;
        board.ActualTurn = ActualTurn;
        board.turns = turns;
        
        
        board.HasMoves = HasMoves;
        board.IsCheckMate = IsCheckMate;

        return board;
    }

    public List<List<Tile>> GetTiles()
    {
        return tiles;
    }

    public Tile GetTileByIndex(int index) 
    {
        double division = index / BoardRowSize;
        int row = (int)Math.Floor(division);

        int column = index - row * BoardColumnSize;

        return tiles[row][column];
    }

    public Tile GetKingTile(PieceColor color)
    {
        return (color == PieceColor.White) ? piecesHolder.whiteKing.GetTile() : piecesHolder.blackKing.GetTile();
    }

    public Tile[] GetRookTiles(PieceColor color)
    {
        List<Tile> rookTiles = new();
        foreach (var row in this.tiles)
        {
            var rookTile = row.Where(t => t.OccupiedBy is Rook rook && rook.pieceColor == color);
            if (rookTile != null && rookTile.ToList().Count > 0)
            {
                rookTiles.AddRange(rookTile);
                if (rookTiles.Count >= 2) break;
            }
        }

        return rookTiles.ToArray();
    }

    public Piece[] GetAllPieces(PieceColor pieceColor)
    {
        return pieceColor == PieceColor.White ? piecesHolder.whitePieces.ToArray() : piecesHolder.blackPieces.ToArray();
    }

    public void Clear()
    {
        foreach (var row in tiles)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }

    public override string ToString()
    {
        return Name;
    }

    public void SetTurn(PieceColor color) 
    {
        ActualTurn = color;

        Profiler.BeginSample("Move Generation");
        currentTurnMoves = moveGenerator.GenerateMoves(color);
        Profiler.EndSample();

        HasMoves = currentTurnMoves.Count > 0;
        IsCheckMate = moveGenerator.IsCheck() && HasMoves is false;
    }
}