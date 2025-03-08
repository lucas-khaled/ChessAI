public interface IGameManager
{
    public Board GameBoard { get; }
    public Board TestBoard { get; }

    public TurnManager TurnManager { get; }
    public EndGameChecker EndGameChecker { get; }
    public ZobristHashManager HashManager { get; }
}
