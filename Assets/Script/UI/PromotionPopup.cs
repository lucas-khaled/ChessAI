using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PromotionPopup : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private PromotionButton buttonPrefab;

    private PromotionButton[] buttons;

    private bool isPopulated = false;
    private Action<PromotionMove> onPromotionCallback;

    private void Awake()
    {
        Populate();    
    }

    private void Populate() 
    {
        Clear();
        var size = Enum.GetValues(typeof(PromotionButton.PromotionPiece)).Length;

        buttons = new PromotionButton[size];
        for(int i = 0; i < size; i++) 
        {
            buttons[i] = Instantiate(buttonPrefab, content);
            buttons[i].SetIcon((PromotionButton.PromotionPiece)i);
        }

        isPopulated = true;
    }

    private void Clear() 
    {
        if (buttons.Length <= 0) return;

        foreach (var button in buttons)
            Destroy(button);

        isPopulated = false;
    }

    public void Show(List<PromotionMove> moves, Action<PromotionMove> onPromotionChoosen) 
    {
        onPromotionCallback = onPromotionChoosen;

        foreach (var move in moves) 
        {
            if (isPopulated is false)
                Populate();

            var button = GetCorrespondingButton(move.piece);
            if (button != null)
                button.SetMove(move, onPromotionChoosen);
        }

        gameObject.SetActive(true);
    }

    private PromotionButton GetCorrespondingButton(Piece piece) 
    {
        PromotionButton.PromotionPiece pieceEnum;

        if (Enum.TryParse(piece.GetType().Name, out pieceEnum) is false)
        {
            Debug.LogError($"Did not found piece of type {piece.GetType().Name} in promotion pieces enum");
            return null;
        }

        var button = buttons.FirstOrDefault(x => x.Piece == pieceEnum);

        if(button == null) 
        {
            Debug.LogError($"Did not found promotion button of type {pieceEnum}");
            return null;
        }

        return button;
    }

    private void OnPromotionChoosen(PromotionMove move) 
    {
        gameObject.SetActive(false);
        onPromotionCallback?.Invoke(move);
    }
}
