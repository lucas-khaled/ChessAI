public class PerftData 
{
    public long nodes;
    public long captures;
    public long castles;
    public long promotions;
    public long checks;
    public long checkmates;
    public long doubleChecks;

    public static PerftData Empty => new PerftData() { nodes = 1};

    public static PerftData operator +(PerftData a, PerftData b)
    {
        return new PerftData()
        {
            nodes = a.nodes + b.nodes,
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
