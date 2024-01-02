public class Environment
{
    public Board board;
    public BoardManager boardManager;
    public MoveMaker moveMaker;
    public TurnManager turnManager;
    public MoveChecker moveChecker;
    public EnvironmentEvents events;
    public EspecialRules rules;

    public bool isVirtual;

    public Environment Copy()
    {
        var env = new Environment();

        env.events = new();

        env.board = board.Copy(env) as Board;
        env.boardManager = boardManager.Copy(env) as BoardManager;
        env.moveMaker = moveMaker.Copy(env) as MoveMaker;
        env.moveChecker = moveChecker.Copy(env) as MoveChecker;
        env.turnManager = turnManager.Copy(env) as TurnManager;
        env.rules = rules.Copy(env) as EspecialRules;

        env.isVirtual = true;
        return env;
    }

    public void StartRealEnvironment(Board board) 
    {
        isVirtual = false;
        events = new();

        this.board = board;
        boardManager = new BoardManager(this);
        moveMaker = new MoveMaker(this);
        turnManager = new TurnManager(this);
        moveChecker = new MoveChecker(this);
        rules = new EspecialRules(this);
    }
}
