using System;

public class Tile
{
    public VisualTile visualTile { get; private set; }

    public Piece OccupiedBy { get; private set; }
    public bool IsOccupied => OccupiedBy != null;

    public Bitboard Bitboard { get; set; }
    public TileCoordinates TilePosition { get; set; }
    public Board Board { get; private set; }

    private Diagonals Diagonals;
    private Diagonals InvertedDiagonals;

    private Verticals Verticals;
    private Verticals InvertedVerticals;

    private Horizontals Horizontals;
    private Horizontals InvertedHorizontals;

    public Tile(Board board) 
    {
        Board = board;
    }

    public void SetVisual(VisualTile visualTile)
    {
        this.visualTile = visualTile;
        visualTile.SetTile(this);
    }

    public void Occupy(Piece piece)
    {
        OccupiedBy = piece;
    }

    public void DeOccupy()
    {
        OccupiedBy = null;
    }

    public bool IsVirtual()
    {
        return visualTile == null;
    }

    public Tile Copy(Board board)
    {
        var tile = new Tile(board)
        {
            TilePosition = this.TilePosition,
            Bitboard = Bitboard,
            visualTile = null
        };

        tile.Diagonals = Diagonals;
        tile.InvertedDiagonals = InvertedDiagonals;
        tile.Verticals = Verticals;
        tile.InvertedVerticals = InvertedVerticals;
        tile.Horizontals = Horizontals;
        tile.InvertedHorizontals = Horizontals;

        tile.OccupiedBy = (IsOccupied) ? this.OccupiedBy.Copy(tile) : null;

        return tile;
    }

    public void SetVerticals(Verticals verticals) 
    {
        Verticals = verticals;
        
        InvertedVerticals = new Verticals();
        InvertedVerticals.frontVerticals = verticals.backVerticals;
        InvertedVerticals.backVerticals = verticals.frontVerticals;
    }

    public void SetHorizontals(Horizontals horizontals) 
    {
        Horizontals = horizontals;

        InvertedHorizontals = new Horizontals();
        InvertedHorizontals.rightHorizontals = horizontals.leftHorizontals;
        InvertedHorizontals.leftHorizontals = horizontals.rightHorizontals;
    }

    public void SetDiagonals(Diagonals diagonals) 
    {
        Diagonals = diagonals;

        InvertedDiagonals = new Diagonals();
        InvertedDiagonals.topRightDiagonals = diagonals.downLeftDiagonals;
        InvertedDiagonals.topLeftDiagonals = diagonals.downRightDiagonals;
        InvertedDiagonals.downRightDiagonals = diagonals.topLeftDiagonals;
        InvertedDiagonals.downLeftDiagonals = diagonals.topRightDiagonals;
    }

    public Verticals GetVerticalsByColor(PieceColor color) 
    {
        return (color == PieceColor.White) ? Verticals : InvertedVerticals;
    }

    public Horizontals GetHorizontalsByColor(PieceColor color) 
    {
        return (color == PieceColor.White) ? Horizontals : InvertedHorizontals;
    }

    public Diagonals GetDiagonalsByColor(PieceColor color) 
    {
        return (color == PieceColor.White) ? Diagonals : InvertedDiagonals;
    }

    public override bool Equals(object obj)
    {
        return obj is Tile tile && tile.TilePosition.Equals(TilePosition);
    }
}
