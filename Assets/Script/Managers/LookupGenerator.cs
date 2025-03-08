using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LookupGenerator
{
    private Bitboard borderMask = new Bitboard(0b1111111110000001100000011000000110000001100000011000000111111111);

    private LookupSave currentSave;
    private object threadLock = new object();

    public void GenerateLookups(int rows, int columns, LookupSave lookupSave) 
    {
        Debug.Log("Creating");
        currentSave = lookupSave;
        int amount = rows * columns;

        lookupSave.rookLookups = new Lookup[amount];
        lookupSave.bishopLookups = new Lookup[amount];

        for(int i = 0; i < 4; i++) 
        {
            Debug.Log($"Creating in index {i}");
            int index = i;
            Generate(index, amount);
        }

        Debug.Log("Created");
    }

    private void Generate(int index, int amount) 
    {
        Bitboard bitboard = new Bitboard(index);

        var rookLookup = GenerateRookLookup(bitboard, amount);
        var bishopLookup = GenerateBishopLookup(bitboard, amount);

        lock (threadLock)
        {
            currentSave.rookLookups[index] = rookLookup;
            currentSave.bishopLookups[index] = bishopLookup;
        }
    }

    private Lookup GenerateRookLookup(Bitboard bitboard, int boardSize)
    {
        int[] shifts = new int[] { 8, -8, 1, -1 };

        Bitboard[] masks = new Bitboard[] { 
        new Bitboard(18374686479671623680UL),
        new Bitboard(255UL),
        new Bitboard(0b1000000110000001100000011000000110000001100000011000000110000001),
        new Bitboard(0b1000000110000001100000011000000110000001100000011000000110000001)};

        var relevantBits = GenerateRelevantBits(shifts, masks, boardSize, bitboard);
        
        Bitboard[] occupancies;
        var occupancyMap = GenerateAttackingMap(bitboard, relevantBits, shifts, out occupancies);

        Lookup lookup = new Lookup(relevantBits, occupancyMap, occupancies);
        return lookup;
    }

    private Lookup GenerateBishopLookup(Bitboard bitboard, int boardSize)
    {
        int[] shifts = new int[] { 9, -9, 7, -7 };

        Bitboard[] masks = new Bitboard[] { new Bitboard(0b1111111110000001100000011000000110000001100000011000000111111111),
        new Bitboard(0b1111111110000001100000011000000110000001100000011000000111111111),
        new Bitboard(0b1111111110000001100000011000000110000001100000011000000111111111),
        new Bitboard(0b1111111110000001100000011000000110000001100000011000000111111111)};

        var relevantBits = GenerateRelevantBits(shifts, masks, boardSize, bitboard);

        Bitboard[] occupancies;
        var occupancyMap = GenerateAttackingMap(bitboard, relevantBits, shifts, out occupancies);

        Lookup lookup = new Lookup(relevantBits, occupancyMap, occupancies);
        return lookup;
    }

    private ulong GenerateMagicNumber(Bitboard bitboard)
    {
        var rand = new System.Random();
        int randomNum = rand.Next();

        return (ulong)randomNum * bitboard.value % 7919UL;
    }

    private Bitboard[] GenerateAttackingMap(Bitboard square, Bitboard relevantBits, int[] shifts, out Bitboard[] occupancies)
    {
        int relevantBitsCount = relevantBits.ConvertToIndexes().Count;
        int possibilities = 1 << relevantBitsCount;
        Bitboard[] map = new Bitboard[possibilities];
        occupancies = new Bitboard[possibilities];

        for (int i = 0; i < possibilities; i++)
        {
            Bitboard occupancy = GenerateOccupancy(i, relevantBitsCount);
            occupancy.Remove(square);

            occupancies[i] = occupancy;
            Bitboard depositedOccupancy = ParallelBitDeposit(occupancy, relevantBits); 
            map[i] = GetSimulatedAttack(square, depositedOccupancy, relevantBits, shifts);
        }

        return map;
    }

    private Bitboard GetSimulatedAttack(Bitboard square, Bitboard occupancy, Bitboard relevantBits, int[] shifts)
    {
        Bitboard validSquares = new Bitboard();
        for(int i = 0; i<shifts.Length; i++) 
        {
            int shiftIteration = 1;
            int shift = shifts[i];

            while(true) 
            {
                var shiftedSquare = square << shift * shiftIteration;
                if((shiftedSquare & relevantBits) <= 0) 
                    break;

                validSquares.Add(shiftedSquare);
                if ((shiftedSquare & occupancy) > 0) break;

                shiftIteration++;
            }
        }

        return validSquares;
    }

    private Bitboard GenerateOccupancy(int index, int relevantBitsCount)
    {
        Bitboard occupancy = new Bitboard();
        int bitPosition = 0;

        while(relevantBitsCount > 0) 
        {
            if((index & (1<<bitPosition)) != 0) 
            {
                occupancy.Add(1UL << bitPosition);
            }
            bitPosition++;
            relevantBitsCount--;
        }

        return occupancy;
    }

    private Bitboard GenerateRelevantBits(int[] lookupShifts, Bitboard[] masks, int boardSize, Bitboard actualSquare) 
    {
        int[] numShiftsDone = new int[lookupShifts.Length];
        bool[] shiftsDone = { false, false, false, false };

        Bitboard relevantBits = new Bitboard();
        while (shiftsDone.Any(x => !x))
        {
            for (int i = 0; i < lookupShifts.Length; i++)
            {
                if (shiftsDone[i]) continue;

                int shift = lookupShifts[i] * (numShiftsDone[i] + 1);
                var shifted = (Math.Sign(shift) > 0) ? actualSquare << shift : actualSquare >> (shift*-1);

                bool minusZero = shifted.value <= 0;
                //bool bypass = shifted > maxIndex;
                bool inMask = (shifted & masks[i]) > 0;

                if (minusZero || inMask)
                {
                    shiftsDone[i] = true;
                    continue;
                }

                relevantBits.Add(shifted);
                numShiftsDone[i]++;
            }
        }

        return relevantBits;
    }

    /*public static Bitboard ParallelBitExtract(Bitboard value, Bitboard mask)
    {
        Bitboard result = new Bitboard();
        for (Bitboard bit = new Bitboard(1UL), pos = new Bitboard(1UL); bit != 0; bit <<= 1)
        {
            if ((mask & bit) != 0)
            {
                if ((value & bit) != 0)
                    result |= pos;
                pos <<= 1;
            }
        }
        return result;
    }*/

    public static Bitboard ParallelBitExtract(Bitboard value, Bitboard mask)
    {
        Bitboard result = new Bitboard();
        int bitPosition = 0;

        for (int i = 0; i < 64; i++)
        {
            if ((mask & (new Bitboard(1UL) << i)) != 0)
            {
                if ((value & (new Bitboard(1UL) << i)) != 0)
                {
                    result |= new Bitboard(1UL << bitPosition);
                }
                bitPosition++;
            }
        }

        return result;
    }

    public static Bitboard ParallelBitDeposit(Bitboard value, Bitboard mask)
    {
        Bitboard result = new Bitboard();
        for (Bitboard bit = new Bitboard(1UL), pos = new Bitboard(1UL); bit != 0; bit <<= 1)
        {
            if ((mask & bit) != 0)
            {
                if ((value & pos) != 0)
                    result |= bit;
                pos <<= 1;
            }
        }
        return result;
    }
}
