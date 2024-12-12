using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BitboardSelector : MonoBehaviour
{
    enum PieceVisualizationType 
    {
        KingDanger,
        Attacking,
        Move
    }

    enum VisualizationMode 
    {
        Piece,
        AllMoves
    }

    [SerializeField] private BitBoardVisualizer boardVisualizer;
    [SerializeField] private VisualizationMode visualizationMode;
    [SerializeField] private PieceVisualizationType visualizationType = PieceVisualizationType.Attacking;
    [SerializeField] private Color attackingColor = Color.red;
    [SerializeField] private Color kingDangerColor = Color.magenta;
    [SerializeField] private Color movesColor = Color.green;

    MoveGenerator generator;

    List<Move> whiteMoves;
    List<Move> blackMoves;

    private void Awake()
    {
        VisualTile.onTileSelected += OnTileSelected;
    }

    private void Start()
    {
        generator = new MoveGenerator(boardVisualizer.GameBoard);
        whiteMoves = generator.GenerateMoves(PieceColor.White);
        blackMoves = generator.GenerateMoves(PieceColor.Black);
    }

    private void OnTileSelected(Tile tile)
    {
        Debug.Log("Tile selected: " + tile.visualTile.name);
        if (tile.IsOccupied is false) return;

        switch (visualizationMode) 
        {
            case VisualizationMode.AllMoves:
                ShowAllMoves(tile);
                break;
            case VisualizationMode.Piece:
                ShowPieceMoves(tile);
                break;
        }
    }

    private void ShowPieceMoves(Tile tile)
    {
        switch (visualizationType)
        {
            case PieceVisualizationType.KingDanger:
                tile.OccupiedBy.GenerateBitBoard();
                boardVisualizer.SetBitBoard(tile.OccupiedBy.KingDangerSquares, kingDangerColor);
                break;
            case PieceVisualizationType.Attacking:
                tile.OccupiedBy.GenerateBitBoard();
                boardVisualizer.SetBitBoard(tile.OccupiedBy.AttackingSquares, attackingColor);
                break;

            case PieceVisualizationType.Move:
                var allMoves = (tile.OccupiedBy.pieceColor == PieceColor.White) ? whiteMoves : blackMoves;
                var pieceMoves = allMoves.Where(m => m.piece.Equals(tile.OccupiedBy)).ToList();
                boardVisualizer.SetBitBoard(GenerateBitboardFromMoves(pieceMoves), movesColor);
                break;
        }
    }

    private Bitboard GenerateBitboardFromMoves(List<Move> moves) 
    {
        Bitboard bitboard = new Bitboard();
        foreach(var move in moves) 
            bitboard.Add(move.to.Bitboard);

        return bitboard;
    }

    private void ShowAllMoves(Tile tile)
    {
        var moves = generator.GenerateMoves(tile.OccupiedBy.pieceColor);
        boardVisualizer.SetBitBoard(moves.GetBitboard(), attackingColor);
    }


}