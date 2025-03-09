using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Players Configuration/Player Config", fileName = "PlayersConfig")]
public class PlayersConfig : ScriptableObject
{
    [SerializeReference] public PlayerSelectionInfo firstPlayerInfo;
    [SerializeReference] public PlayerSelectionInfo secondPlayerInfo;

    public virtual IPlayer GetFirstPlayer(GameManager manager) 
    {
        return firstPlayerInfo.GetPlayer(manager);
    }

    public virtual IPlayer GetSecondPlayer(GameManager manager) 
    {
        return secondPlayerInfo.GetPlayer(manager);
    }
}

[System.Serializable]
public abstract class PlayerSelectionInfo 
{
    [HideInInspector]
    public PlayerSelectionType selected;
    public abstract IPlayer GetPlayer(GameManager manager);

    public PlayerSelectionInfo(PlayerSelectionType selectionType) 
    {
        selected = selectionType;
    }
}

[System.Serializable]
public class HumanPlayerInfo : PlayerSelectionInfo
{
    public HumanPlayerInfo() : base(PlayerSelectionType.Human) { }
    public HumanPlayerInfo(PlayerSelectionType selectionType) : base(selectionType)
    {
    }

    public override IPlayer GetPlayer(GameManager manager)
    {
        return new HumanPlayer(manager);
    }
}

[System.Serializable]
public class RandomPlayerInfo : PlayerSelectionInfo
{
    public RandomPlayerInfo(): base(PlayerSelectionType.Random) { }
    public RandomPlayerInfo(PlayerSelectionType selectionType) : base(selectionType)
    {
    }

    public override IPlayer GetPlayer(GameManager manager)
    {
        return new RandomAI(manager);
    }
}

[System.Serializable]
public class MinimaxPlayerInfo : PlayerSelectionInfo
{
    [SerializeField] public int depth = 3;
    [SerializeField] public int maxTimePerChoice = 60000;

    public MinimaxPlayerInfo() : base(PlayerSelectionType.Minimax) { }

    public MinimaxPlayerInfo(PlayerSelectionType selectionType) : base(selectionType)
    {
    }

    public override IPlayer GetPlayer(GameManager manager)
    {
        return new MinimaxAI(manager, depth, maxTimePerChoice);
    }
}

public enum PlayerSelectionType
{
    Minimax,
    Human,
    Random
}
