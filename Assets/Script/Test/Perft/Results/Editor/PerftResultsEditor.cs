using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PerftResults))]
public class PerftResultsEditor : Editor
{
    PerftResults perftResults;
    bool useAllDepths = true;
    string fen = "";
    int depth = 1;

    private void OnEnable()
    {
       perftResults = (PerftResults)target;
    }
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.Add(new IMGUIContainer(() => DrawDefaultInspector()));

        var aditionalBox = new Box();
        aditionalBox.Add(GetTextBox());
        aditionalBox.Add(GetCMDBox());
        aditionalBox.style.marginTop = 30;

        root.Add(aditionalBox);
        return root;
    }

    private Box GetCMDBox()
    {
        var cmdBox = new Box();

        var fenField = new TextField();
        fenField.label = "Enter your FEN string here";
        fenField.RegisterCallback<ChangeEvent<string>>(evt => fen = evt.newValue);

        cmdBox.Add(fenField);
        cmdBox.Add(GetDepthContainer());

        var button = new Button(CallCMD);
        button.text = "Get Results from Stockfish";
        cmdBox.Add(button);

        cmdBox.style.marginTop = 20;
        return cmdBox;
    }

    private void CallCMD()
    {
        Process process = new Process();
        process.StartInfo.FileName = "C:\\Users\\lucas\\Downloads\\stockfish\\stockfish.exe"; // Altere para o .exe que deseja executar
        process.StartInfo.RedirectStandardInput = true; // Permite enviar comandos
        process.StartInfo.RedirectStandardOutput = true; // Captura a saída
        process.StartInfo.RedirectStandardError = true;  // Captura erros
        process.StartInfo.UseShellExecute = false;       // Necessário para redirecionamento
        process.StartInfo.CreateNoWindow = true;         // Oculta a janela do console

        process.Start();

        // Obtemos o stream de entrada para enviar comandos
        using (var writer = process.StandardInput)
        {
            if (writer.BaseStream.CanWrite)
            {
                writer.WriteLine($"position fen {fen}");
                if(useAllDepths)
                {
                    for(int i = 1; i <= depth; i++) 
                    {
                        writer.WriteLine($"echo start depth|{i}|");
                        writer.WriteLine($"go perft {i}");
                        writer.WriteLine($"echo end depth");
                    }
                }
                else 
                {
                    writer.WriteLine($"echo start depth|{depth}|");
                    writer.WriteLine($"go perft {depth}");
                    writer.WriteLine($"echo end depth");
                }
                writer.WriteLine("quit");
            }
        }

        ResultData resultData = new ResultData();
        resultData.fenPosition = fen;
        resultData.depthData = new List<DepthData>();
        using (var reader = process.StandardOutput)
        {
            string line;
            DepthData depthData = new DepthData();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line.Trim()) || line.StartsWith("info") || line.StartsWith("Stockfish")) continue;

                if(line.Contains("echo start depth")) 
                {
                    string[] split = line.Split("|");
                    depth = Convert.ToInt32(split[1]);
                    depthData = new DepthData();
                    depthData.data = PerftData.Empty;
                    depthData.depth = depth;

                    continue;
                }

                if(line.Contains("echo end depth")) 
                {
                    resultData.depthData.Add(depthData);
                    continue;
                }

                if(line.StartsWith("Nodes searched")) 
                {
                    string removedSpaces = line.Replace(" ", "");
                    string[] splited = removedSpaces.Split(":");

                    depthData.data.nodes = Convert.ToInt64(splited[1]);
                    continue;
                }


                string removedSpaces2 = line.Replace(" ", "");
                string[] splited2 = removedSpaces2.Split(":");

                string move = splited2[0];
                long nodeCount = Convert.ToInt64(splited2[1]);

                depthData.data.divideDict.Add(new PerftDivide(move, nodeCount));
            }
        }
        
        process.WaitForExit();

        var correspondingResults = perftResults.results.Find(r => r.fenPosition == fen);
        if(correspondingResults == null) 
        {
            perftResults.results.Add(resultData);
            return;
        }

        perftResults.results[perftResults.results.IndexOf(correspondingResults)] = resultData;
    }

    private VisualElement GetDepthContainer()
    {
        var depthContainer = new VisualElement();
        var depthField = new IntegerField("Depth");
        depthField.value = depth;
        depthField.RegisterCallback<ChangeEvent<int>>(evt => depth = evt.newValue);
        depthField.style.paddingRight = 40;

        var allDepthToggle = new Toggle("Include All Depths");
        allDepthToggle.value = useAllDepths;
        allDepthToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            useAllDepths = evt.newValue;
        }
        );

        depthContainer.Add(depthField);
        depthContainer.Add(allDepthToggle);

        depthContainer.style.flexDirection = FlexDirection.Row;
        depthContainer.style.justifyContent = Justify.FlexStart;
        depthContainer.style.alignContent = Align.FlexStart;
        depthContainer.style.paddingTop = depthContainer.style.paddingBottom = 5;
        return depthContainer;
    }

    private Box GetTextBox()
    {
        var textBox = new Box();

        var objectField = new ObjectField();
        objectField.objectType = typeof(TextAsset);
        objectField.label = "Base Text";

        objectField.value = perftResults.baseText;
        objectField.RegisterCallback<ChangeEvent<TextAsset>>((evt) =>
        {
            perftResults.baseText = evt.newValue;
        }
        );

        var readTextbutton = new Button(ReadText);
        readTextbutton.text = "Read results from text";

        var writeTextbutton = new Button(WriteText);
        writeTextbutton.text = "Write results to text";

        textBox.Add(objectField);
        textBox.Add(readTextbutton);
        textBox.Add(writeTextbutton);

        return textBox;
    }

    public void ReadText()
    {
        if (perftResults.baseText == null) return;

        perftResults.results = JsonConvert.DeserializeObject<List<ResultData>>(perftResults.baseText.text);
    }

    public void WriteText()
    {
        if (perftResults.baseText == null) return;

        string json = JsonConvert.SerializeObject(perftResults.results, Formatting.Indented);
        string path = AssetDatabase.GetAssetPath(perftResults.baseText);

        File.WriteAllText(path, json);
    }
}
