using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(LookupSave))]
public class LookupSaveEditor : Editor
{
    private LookupSave lookupSave;
    private enum LookupTypeSelected 
    {
        None,
        Rook,
        Bishop
    }

    private LookupTypeSelected lookupTypeSelected;

    private VisualElement lookupContainer;
    private VisualElement lookupTypeTab;
    private VisualElement root;
    
    private LookupGenerator generator = new LookupGenerator();
    private int lookupIndex = -1;
    private int occupancyIndex = -1;

    private ScrollView lookupsScroll;
    private ScrollView occupancyScroll;

    private void OnEnable()
    {
        lookupSave = (LookupSave)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        CreateAll();
        DrawAll();

        root = new VisualElement();
        root.Add(lookupTypeTab);
        root.Add(lookupContainer);

        Button generateButton = DrawGenerateButton();
        root.Add(generateButton);

        return root;
    }

    private void CreateAll() 
    {
        lookupTypeTab = new VisualElement();
        lookupTypeTab.style.flexDirection = FlexDirection.Row;

        lookupContainer = new VisualElement();
        lookupContainer.style.flexGrow = 1;
        lookupContainer.style.flexShrink = 0;
    }

    private void DrawAll()
    {
        DrawTab(); 
        DrawLookupContainer();
    }

    private void DrawLookupContainer()
    {
        lookupContainer.Clear();
        if (lookupTypeSelected == LookupTypeSelected.None) return;

        VisualElement lookupTab = DrawLookupTab();
        lookupContainer.Add(lookupTab);

        if (lookupIndex == -1) return;

        VisualElement currentLookup = DrawCurrentLookup();
        lookupContainer.Add(currentLookup);

    }

    private VisualElement DrawCurrentLookup()
    {
        Box content = new Box();

        Lookup[] lookups = GetCurrentLookups();
        var lookup = lookups[lookupIndex];

        var label = new Label("Relevant Bits:");
        label.style.marginBottom = label.style.marginTop = 3;
        label.style.fontSize = 18;
        content.Add(label);

        content.Add(DrawBoard(lookup.relevantBits, new Bitboard()));
        content.Add(DrawOccupancy(lookup.occupancyMap, lookup.occupancies));

        label.style.marginBottom = label.style.marginTop = 5;
        label.style.fontSize = 14;

        return content;
    }

    private VisualElement DrawOccupancy(Bitboard[] occupancyMap, Bitboard[] occupancies)
    {
        VisualElement rootOccupancy = new VisualElement();

        if (occupancyMap == null)
        {
            Debug.Log("Occupancy is null");
            return rootOccupancy;
        }

        occupancyScroll = CreateOrCleanScroll(occupancyScroll);
        rootOccupancy.Add(occupancyScroll);

        var label = new Label("Occupancy Map:");
        label.style.marginBottom = label.style.marginTop = 3;
        label.style.fontSize = 18;
        rootOccupancy.Add(label);

        for(int i = 0; i < occupancyMap.Length; i++) 
        {
            var occupancy = occupancyMap[i];
            Button button = new Button();
            button.text = i.ToString();

            if (i == occupancyIndex)
                button.style.backgroundColor = new StyleColor(Color.gray);

            int index = i;
            button.clicked += () =>
            {
                occupancyIndex = index;
                DrawLookupContainer();
            };

            occupancyScroll.Add(button);
        }

        if(occupancyIndex > -1)
            rootOccupancy.Add(DrawBoard(occupancyMap[occupancyIndex], occupancies[occupancyIndex]));

        return rootOccupancy;
    }

    private VisualElement DrawBoard(Bitboard relevantBits, Bitboard secondaryBits)
    {
        VisualElement board = new VisualElement();
        board.name = "Board";
        board.style.flexDirection = FlexDirection.Column;

        for(int row = 7; row >= 0; row--) 
        {
            VisualElement rowElement = new VisualElement();
            rowElement.style.flexDirection = FlexDirection.Row;

            for (int column = 0; column < 8; column++)
            {
                Box square = new Box();
                square.style.width = square.style.height = 30;
                square.style.marginBottom = square.style.marginLeft = square.style.marginRight = square.style.marginTop = 1;

                int index = (row * 8 + column);
                Bitboard squareBitboard = new Bitboard(index);

                Color color;
                if (index == lookupIndex)
                    color = Color.green;
                else
                    color =
                        ((squareBitboard & secondaryBits) > 0)
                        ? Color.magenta
                        : ((squareBitboard & relevantBits) > 0) 
                            ? Color.red 
                            : Color.gray;

                square.style.color = square.style.backgroundColor = new StyleColor(color);
                rowElement.Add(square);
            }

            board.Add(rowElement);
        }

        return board;
    }

    private VisualElement DrawLookupTab()
    {
        ScrollView scroll = CreateOrCleanScroll(lookupsScroll);

        var currentLookups = GetCurrentLookups();

        for (int i = 0; i < currentLookups.Length; i++)
        {
            Button button = new Button();
            button.text = i.ToString();

            if (i == lookupIndex)
                button.style.backgroundColor = new StyleColor(Color.gray);

            int index = i;
            button.clicked += () =>
            {
                lookupIndex = index;
                DrawLookupContainer();
            };
            scroll.Add(button);
        }

        return scroll;
    }

    private ScrollView CreateOrCleanScroll(ScrollView scroll)
    {
        if (scroll == null)
        {
            scroll = new ScrollView(ScrollViewMode.Horizontal);
            scroll.style.height = 50;
            scroll.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            scroll.style.flexGrow = 1;
            scroll.style.flexShrink = 0;
            scroll.style.marginTop = scroll.style.marginBottom = 10;
        }
        else
            scroll.Clear();

        return scroll;
    }

    private Button DrawGenerateButton()
    {
        var generateButton = new Button();
        generateButton.text = "Create Lookups";
        generateButton.clicked += GenerateLookups;
        return generateButton;
    }

    private void DrawTab()
    {
        lookupTypeTab.Clear();
        var rookButton = new Button();
        rookButton.text = "Rook Lookups";
        rookButton.clicked += RookClicked;

        var bishopButton = new Button();
        bishopButton.text = "Bishop Lookups";
        bishopButton.clicked += BishopClicked;

        if(lookupTypeSelected == LookupTypeSelected.Rook) 
            rookButton.style.backgroundColor = new StyleColor(Color.gray);
        else if(lookupTypeSelected == LookupTypeSelected.Bishop)
            bishopButton.style.backgroundColor = new StyleColor(Color.gray);

        lookupTypeTab.Add(bishopButton);
        lookupTypeTab.Add(rookButton);
    }

    private void BishopClicked()
    {
        lookupTypeSelected = LookupTypeSelected.Bishop;
        lookupIndex = -1;
        DrawAll();
        root.MarkDirtyRepaint();
    }

    private void RookClicked()
    {
        lookupTypeSelected = LookupTypeSelected.Rook;
        lookupIndex = -1;
        DrawAll();
        root.MarkDirtyRepaint();
    }

    private Lookup[] GetCurrentLookups() 
    {
        switch (lookupTypeSelected) 
        {
            case LookupTypeSelected.Rook:
                return lookupSave.rookLookups;
            case LookupTypeSelected.Bishop:
                return lookupSave.bishopLookups;
            default:
                return null;
        }
    }

    private void GenerateLookups() 
    {
        generator.GenerateLookups(8, 8, lookupSave);
        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }
}
