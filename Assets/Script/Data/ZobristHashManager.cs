using System;
using UnityEngine;

public class ZobristHashManager
{
    private TileHash[] hashPerTile;
    private CastlingHash castlingHashes;
    private long[] enPassantHashes;
    private long isBlackMoveHash;
    private int generationCount = 1;

    public void InitializeHashes() 
    {
        hashPerTile = new TileHash[64];
        enPassantHashes = new long[8];
        isBlackMoveHash = GenerateRandomLong();

        for (int i = 0; i < hashPerTile.Length; i++) 
        {
            hashPerTile[i] = new TileHash();
            GenerateTileHash(hashPerTile[i]);

            if(i < 8) 
            {
                enPassantHashes[i] = GenerateRandomLong();
            }
        }

        castlingHashes = GenerateCastleHash();
    }

    private void GenerateTileHash(TileHash tileHash) 
    {
        tileHash.blackKingHash = GenerateRandomLong();
        tileHash.whiteKingHash = GenerateRandomLong();

        tileHash.blackQueenHash = GenerateRandomLong();
        tileHash.whiteQueenHash = GenerateRandomLong();

        tileHash.blackBishopHash = GenerateRandomLong();
        tileHash.whiteBishopHash = GenerateRandomLong();

        tileHash.blackRookHash = GenerateRandomLong();
        tileHash.whiteRookHash = GenerateRandomLong();

        tileHash.blackKnightHash = GenerateRandomLong();
        tileHash.whiteKnightHash = GenerateRandomLong();

        tileHash.blackPawnHash = GenerateRandomLong();
        tileHash.whitePawnHash = GenerateRandomLong();
    }

    private CastlingHash GenerateCastleHash()
    {
        CastlingHash castlingHash = new CastlingHash();

        castlingHash.whiteKingsideCastleHash = GenerateRandomLong();
        castlingHash.whiteQueensideCastleHash = GenerateRandomLong();
        castlingHash.blackQueensideCastleHash = GenerateRandomLong();
        castlingHash.blackKingsideCastleHash = GenerateRandomLong();

        return castlingHash;
    }

    private long GenerateRandomLong()
    {
        System.Random rand = new System.Random(generationCount);
        byte[] buffer = new byte[8];

        rand.NextBytes(buffer);

        long result = BitConverter.ToInt64(buffer, 0);

        generationCount++;
        return result;
    }

    public long GetHashFromPosition(Board board) 
    {
        long hash = 0;
        for(int row = 0; row < board.BoardRowSize; row++) 
        {
            for(int column = 0; column < board.BoardColumnSize; column++) 
            {
                Tile tile = board.tiles[row][column];

                if (tile.IsOccupied) 
                {
                    TileHash tileHash = hashPerTile[board.BoardRowSize * row + column];
                    hash = hash ^ tileHash.GetHashFromPiece(tile.OccupiedBy);
                }
            }
        }

        if (board.ActualTurn == PieceColor.Black)
            hash = hash ^ isBlackMoveHash;

        if (board.rules.enPassantTile != null)
        {
            int column = board.rules.enPassantTile.TilePosition.column;  
            hash = hash ^ enPassantHashes[column];
        }

        if (board.rules.whiteCastleRights.CanCastleKingSide)
            hash = hash ^ castlingHashes.whiteKingsideCastleHash;

        if (board.rules.whiteCastleRights.CanCastleQueenSide)
            hash = hash ^ castlingHashes.whiteQueensideCastleHash;

        if (board.rules.blackCastleRights.CanCastleKingSide)
            hash = hash ^ castlingHashes.blackKingsideCastleHash;

        if (board.rules.blackCastleRights.CanCastleQueenSide)
            hash = hash ^ castlingHashes.blackQueensideCastleHash;

        return hash;
    }

    public long GetNewHashFromMove(long oldHash, Move move, EspecialRules actualRules, EspecialRules oldRules) 
    {
        long newHash = oldHash;
        int fromIndex = 8 * move.from.TilePosition.row + move.from.TilePosition.column;
        int toIndex = 8 * move.to.TilePosition.row + move.to.TilePosition.column;

        long fromHash = hashPerTile[fromIndex].GetHashFromPiece(move.piece);
        newHash = newHash ^ fromHash;

        TileHash toTileHash = hashPerTile[toIndex];
        if (move.capture != null) 
        {
            newHash = newHash ^ toTileHash.GetHashFromPiece(move.capture);
        }
        
        long toHash = (move is PromotionMove promotionMove) ?
            toTileHash.GetHashFromPiece(promotionMove.promoteTo):
            toTileHash.GetHashFromPiece(move.piece);

        newHash = newHash ^ toHash;

        if (actualRules.blackCastleRights.CanCastleKingSide != oldRules.blackCastleRights.CanCastleKingSide)
            newHash = newHash ^ castlingHashes.blackKingsideCastleHash;

        if (actualRules.blackCastleRights.CanCastleQueenSide != oldRules.blackCastleRights.CanCastleQueenSide)
            newHash = newHash ^ castlingHashes.blackQueensideCastleHash;

        if (actualRules.whiteCastleRights.CanCastleKingSide != oldRules.whiteCastleRights.CanCastleKingSide)
            newHash = newHash ^ castlingHashes.whiteKingsideCastleHash;

        if (actualRules.whiteCastleRights.CanCastleQueenSide != oldRules.whiteCastleRights.CanCastleQueenSide)
            newHash = newHash ^ castlingHashes.whiteQueensideCastleHash;

        if (oldRules.enPassantTile != null)
            newHash = newHash ^ enPassantHashes[oldRules.enPassantTile.TilePosition.column];

        if(actualRules.enPassantTile != null)
            newHash = newHash ^ enPassantHashes[actualRules.enPassantTile.TilePosition.column];

        return newHash;
    }
}

internal class CastlingHash
{
    public long blackQueensideCastleHash;
    public long blackKingsideCastleHash;
    public long whiteQueensideCastleHash;
    public long whiteKingsideCastleHash;
}

internal class TileHash
{
    public long blackKingHash;
    public long whiteKingHash;

    public long blackQueenHash;
    public long whiteQueenHash;

    public long blackRookHash;
    public long whiteRookHash;

    public long blackBishopHash;
    public long whiteBishopHash;

    public long blackKnightHash;
    public long whiteKnightHash;

    public long blackPawnHash;
    public long whitePawnHash;


    public bool HasHash(long hash) 
    {
        return blackKingHash == hash || whiteKingHash == hash || blackQueenHash == hash || whiteQueenHash == hash || blackRookHash == hash || whiteRookHash == hash ||
            blackBishopHash == hash || whiteBishopHash == hash || blackKnightHash == hash || whiteKnightHash == hash || blackPawnHash == hash || whitePawnHash == hash;
    }

    public long GetHashFromPiece(Piece piece) 
    {
        bool isWhite = piece.pieceColor == PieceColor.White;
        if(piece is King)
            return (isWhite) ? whiteKingHash : blackKingHash;

        if (piece is Queen)
            return (isWhite) ? whiteQueenHash : blackQueenHash;

        if (piece is Rook)
            return (isWhite) ? whiteRookHash : blackRookHash;

        if (piece is Bishop)
            return (isWhite) ? whiteBishopHash : blackBishopHash;

        if (piece is Knight)
            return (isWhite) ? whiteKnightHash : blackKnightHash;

        if (piece is Pawn)
            return (isWhite) ? whitePawnHash : blackPawnHash;

        return 0;
    }
}