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
    [HideInInspector] public TextAsset baseText;
    [SerializeField] public List<ResultData> results;

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
