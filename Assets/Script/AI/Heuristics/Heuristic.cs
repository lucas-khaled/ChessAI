public abstract class Heuristic
{
    protected float weight;
    public Heuristic(float weight) 
    {
        this.weight = weight;
    }
    public abstract float GetHeuristic(Environment environment);
}
