using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BitboardSelector : MonoBehaviour
{
    enum PieceVisualizationType 
    {
        KingDanger,
        PinSquares,
        Attacking,
        AspiredMove,
        ValidMove
    }

    enum VisualizationMode 
    {
        Piece,
        AllMoves
    }

    [SerializeField] private BitBoardVisualizer boardVisualizer;
    [SerializeField] private VisualizationMode visualizationMode;
    [SerializeField] private PieceVisualizationType visualizationType = PieceVisualizationType.Attacking;

    [Header("Colors")]
    [SerializeField] private Color attackingColor = Color.red;
    [SerializeField] private Color kingDangerColor = Color.magenta;
    [SerializeField] private Color pinSquaresColor = Color.blue;
    [SerializeField] private Color movesColor = Color.green;
    [SerializeField] private Color selectedPieceColor = Color.yellow;

    MoveGenerator generator;

    List<Move> whiteMoves;
    List<Move> blackMoves;

    private VisualPiece currentSelectedPiece;

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
        if (tile.IsOccupied is false) return;

        HandlePieceVisual(tile.OccupiedBy.visualPiece);

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

    private void HandlePieceVisual(VisualPiece selectedPiece) 
    {
        if (currentSelectedPiece != null)
            currentSelectedPiece.ResetMaterial();

        currentSelectedPiece = selectedPiece;
        currentSelectedPiece.SetColor(selectedPieceColor);
    }

    private void ShowPieceMoves(Tile tile)
    {
        switch (visualizationType)
        {
            case PieceVisualizationType.KingDanger:
                if (tile.OccupiedBy is not PinnerPiece pinner)
                {
                    boardVisualizer.SetBitBoard(new Bitboard(), kingDangerColor);
                    return;
                }

                pinner.GenerateBitBoard();
                boardVisualizer.SetBitBoard(pinner.KingDangerSquares, kingDangerColor);
                break;

            case PieceVisualizationType.PinSquares:
                if (tile.OccupiedBy is not PinnerPiece pinner2)
                {
                    boardVisualizer.SetBitBoard(new Bitboard(), pinSquaresColor);
                    return;
                }

                pinner2.GenerateBitBoard();
                boardVisualizer.SetBitBoard(pinner2.PinSquares, pinSquaresColor);
                break;
            
            case PieceVisualizationType.Attacking:
                tile.OccupiedBy.GenerateBitBoard();
                boardVisualizer.SetBitBoard(tile.OccupiedBy.AttackingSquares, attackingColor);
                break;

            case PieceVisualizationType.AspiredMove:
                tile.OccupiedBy.GenerateBitBoard();
                boardVisualizer.SetBitBoard(tile.OccupiedBy.MovingSquares, movesColor);
                break;

            case PieceVisualizationType.ValidMove:
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