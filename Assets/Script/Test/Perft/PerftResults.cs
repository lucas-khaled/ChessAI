using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Perft/Reuslts")]
public class PerftResults : ScriptableObject
{
    [SerializeField] private TextAsset baseText;
    [SerializeField] private List<ResultData> results;

    [ContextMenu("Fill from Text")]
    public void ReadText() 
    {
        if (baseText == null) return;

        results = JsonConvert.DeserializeObject<List<ResultData>>(baseText.text);
    }

    [ContextMenu("Fill Text from object")]
    public void WriteText() 
    {
        if (baseText == null) return;

        string json = JsonConvert.SerializeObject(results, Formatting.Indented);
        string path = AssetDatabase.GetAssetPath(baseText);

        File.WriteAllText(path, json);
    }

    public ResultData GetByFEN(string fen) 
    {
        return results.Find(r => r.fenPosition == fen);
    }
}

[Serializable]
public class ResultData 
{
    public string fenPosition;
    public List<DepthData> depthData;

    public DepthData GetByDepth(int depth) 
    {
        return depthData.Find(d => d.depth == depth);
    }
}

[Serializable]
public class DepthData 
{
    public int depth;
    public PerftData data;
}
