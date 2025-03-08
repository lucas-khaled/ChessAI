using System;

[Serializable]
public struct Lookup
{
    public Bitboard relevantBits;
    public Bitboard[] occupancyMap;
    public Bitboard[] occupancies;

    public Bitboard GetBaseOnOccupancy(Bitboard occupancy) 
    {
        var maskedOccupancy = occupancy & relevantBits;
        var extractedOccupancy = LookupGenerator.ParallelBitExtract(maskedOccupancy, relevantBits);
        return occupancyMap[extractedOccupancy.value];
    }

    public Lookup(Bitboard relevantBits, Bitboard[] occupancyMap, Bitboard[] occupancies)
    {
        this.relevantBits = relevantBits;
        this.occupancyMap = occupancyMap;
        this.occupancies = occupancies;
    }
}
