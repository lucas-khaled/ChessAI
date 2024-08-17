public abstract class Heuristic
{
    protected float weight;
    protected GameManager manager;
    public Heuristic(GameManager manager, float weight) 
    {
        this.weight = weight;
        this.manager = manager;
    }
    public abstract float GetHeuristic(Board board);
}
