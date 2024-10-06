using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Players Configuration", fileName = "PlayersConfig")]
public class PlayersConfig : ScriptableObject
{
    [SerializeReference] public PlayerSelectionInfo whitePlayerInfo;
    [SerializeReference] public PlayerSelectionInfo blackPlayerInfo;

    public IPlayer GetWhitePlayer(GameManager manager) 
    {
        return whitePlayerInfo.GetPlayer(manager);
    }

    public IPlayer GetBlackPlayer(GameManager manager) 
    {
        return blackPlayerInfo.GetPlayer(manager);
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

    public MinimaxPlayerInfo() : base(PlayerSelectionType.Minimax) { }

    public MinimaxPlayerInfo(PlayerSelectionType selectionType) : base(selectionType)
    {
    }

    public override IPlayer GetPlayer(GameManager manager)
    {
        return new MinimaxAI(manager, depth);
    }
}

public enum PlayerSelectionType
{
    Minimax,
    Human,
    Random
}
