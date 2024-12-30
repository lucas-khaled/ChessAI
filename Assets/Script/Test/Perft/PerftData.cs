using System;
using System.Collections.Generic;

[Serializable]
public struct PerftData 
{
    public long nodes;
    public long captures;
    public long castles;
    public long promotions;
    public long checks;
    public long checkmates;
    public long doubleChecks;
    public List<PerftDivide> divideDict;

    public static PerftData Empty => new PerftData(0);
    public static PerftData Single => new PerftData(1);
    public static PerftData NotValid => new PerftData(-1);

    public bool IsValid() => nodes >= 0;

    public PerftData(long nodes) : this()
    {
        divideDict = new();
        this.nodes = nodes;
    }

    public static PerftData operator +(PerftData a, PerftData b)
    {
        return new PerftData(a.nodes + b.nodes)
        {
            captures = a.captures + b.captures,
            castles = a.castles + b.castles,
            promotions = a.promotions + b.promotions,
            checks = a.checks + b.checks,
            checkmates = a.checkmates + b.checkmates,
            doubleChecks = a.doubleChecks + b.doubleChecks
        };
    }

    public override string ToString()
    {
        return $"Nodes: {nodes}\n" +
            $"Captures: {captures}\n" +
            $"Castles: {castles}\n" +
            $"Promotions: {promotions}\n" +
            $"Checks: {checks}\n" +
            $"Double Checks: {doubleChecks}\n" +
            $"Checkmates: {checkmates}\n";
    }
}

[Serializable]
public struct PerftDivide 
{
    public string move;
    public long nodeCount;

    public PerftDivide(string move, long nodeCount)
    {
        this.move = move;
        this.nodeCount = nodeCount;
    }
}
