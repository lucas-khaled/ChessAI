using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : IEnvironmentable
{
    public Environment Environment { get; }

    public BoardManager(Environment environment) 
    {
        Environment = environment;
    }

    public Tile GetKingTile(PieceColor color) 
    {
        foreach (var row in Environment.board.GetTiles())
        {
            var kingTile = row.Find(t => t.OccupiedBy is King king && king.pieceColor == color);
            if (kingTile != null)
                return kingTile;
        }

        return null;
    }

    public Tile[] GetRookTiles(PieceColor color) 
    {
        List<Tile> tiles = new();
        foreach (var row in Environment.board.GetTiles())
        {
            var rookTile = row.Where(t => t.OccupiedBy is Rook rook && rook.pieceColor == color);
            if (rookTile != null && rookTile.ToList().Count > 0)
            {
                tiles.AddRange(rookTile);
                if (tiles.Count >= 2) break;
            }
        }

        return tiles.ToArray();
    }

    public Piece[] GetAllPieces(PieceColor pieceColor) 
    {
        List<Piece> pieces = new();
        foreach(var tileList in Environment.board.GetTiles()) 
        {
            foreach (var tile in tileList)
            {
                if (tile.IsOccupied && tile.OccupiedBy.pieceColor == pieceColor)
                    pieces.Add(tile.OccupiedBy);
            }
        }

        return pieces.ToArray();
    }

    public void Clear()
    {
        foreach (var row in Environment.board.tiles)
        {
            foreach (var tile in row)
            {
                tile.DeOccupy();
            }
        }
    }

    public IEnvironmentable Copy(Environment environment)
    {
        return new BoardManager(environment);
    }
}

