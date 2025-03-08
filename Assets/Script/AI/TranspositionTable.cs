using System.Collections.Generic;

public class TranspositionTable 
{
    private Dictionary<string, float> tableDictionary = new Dictionary<string, float>();

    public void AddScore(string hash, float score, bool overrideIfExists = false)
    {
        if (HasScore(hash)) 
        {
            if(overrideIfExists)
                tableDictionary[hash] = score;

            return;
        }

        tableDictionary.Add(hash, score);
    }

    public bool HasScore(string hash) 
    {
        return tableDictionary.ContainsKey(hash);
    }

    public float GetScore(string hash) 
    {
        return tableDictionary[hash];
    }
}
