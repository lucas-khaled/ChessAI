public class Environment
{
    public Board board;
    public BoardManager boardManager;
    public MoveMaker moveMaker;
    public TurnManager turnManager;
    public MoveChecker moveChecker;
    public EnvironmentEvents events;

    public bool isVirtual;

    public Environment Copy()
    {
        var env = new Environment();

        env.board = board.Copy(env) as Board;
        env.boardManager = boardManager.Copy(env) as BoardManager;
        env.moveMaker = moveMaker.Copy(env) as MoveMaker;
        env.moveChecker = moveChecker.Copy(env) as MoveChecker;
        env.turnManager = turnManager.Copy(env) as TurnManager;

        env.isVirtual = true;
        return env;
    }

    public void StartRealEnvironment(Board board) 
    {
        isVirtual = false;

        this.board = board;
        boardManager = new BoardManager(this);
        moveMaker = new MoveMaker(this);
        turnManager = new TurnManager(this);
        moveChecker = new MoveChecker(this);

        events = new();
    }
}
