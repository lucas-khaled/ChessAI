using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PlayersConfig))]
public class PlayersConfigEditor : Editor
{
    private SerializedProperty whiteInfo;
    private SerializedProperty blackInfo;

    private PlayersConfig config;

    private Dictionary<PlayerSelectionType, Type> typeMap = new Dictionary<PlayerSelectionType, Type>()
    {
        { PlayerSelectionType.Human, typeof(HumanPlayerInfo)},
        { PlayerSelectionType.Random, typeof(RandomPlayerInfo)},
        { PlayerSelectionType.Minimax, typeof(MinimaxPlayerInfo)}
    };


    private void OnEnable()
    {
        config = (PlayersConfig)target;
        whiteInfo = serializedObject.FindProperty("firstPlayerInfo");

        if(whiteInfo.managedReferenceValue == null) 
        {
            whiteInfo.managedReferenceValue = new HumanPlayerInfo();
        }

        blackInfo = serializedObject.FindProperty("secondPlayerInfo");
        if (blackInfo.managedReferenceValue == null)
        {
            blackInfo.managedReferenceValue = new HumanPlayerInfo();
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement inspector = new VisualElement();

        var label = new Label("Players Config");
        label.style.fontSize = 18;
        inspector.Add(label);

        inspector.Add(GetWhiteContainer());
        inspector.Add(GetBlackContainer());

        return inspector;
    }

    private VisualElement GetWhiteContainer() 
    {
        var type = (config.firstPlayerInfo == null) ? PlayerSelectionType.Human : config.firstPlayerInfo.selected;
        return GetContainer("White", whiteInfo, type);
    }

    private VisualElement GetBlackContainer()
    {
        var type = (config.secondPlayerInfo == null) ? PlayerSelectionType.Human : config.secondPlayerInfo.selected;
        return GetContainer("Black", blackInfo, type);
    }

    private VisualElement GetContainer(string containerName, SerializedProperty property, PlayerSelectionType defaultType) 
    {
        UnityEngine.UIElements.PopupWindow container = new UnityEngine.UIElements.PopupWindow();
        container.text = containerName;

        var enumField = new EnumField(defaultType);
        enumField.RegisterCallback<ChangeEvent<Enum>>((evt) => ChangedEnum(evt, property));
        container.Add(enumField);

        var popup = new UnityEngine.UIElements.PopupWindow();
        popup.text = property.managedReferenceValue.GetType().Name;

        var propertyField = new PropertyField(property);
        popup.Add(propertyField);

        container.Add(popup);

        return container;
    }

    private void ChangedEnum(ChangeEvent<Enum> evt, SerializedProperty property) 
    {
        var seletionType = (PlayerSelectionType)evt.newValue;
        var type = typeMap[seletionType];

        property.managedReferenceValue = Activator.CreateInstance(type, seletionType);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        CreateInspectorGUI();
    }
}
