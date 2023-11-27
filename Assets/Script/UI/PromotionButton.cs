using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PromotionButton : MonoBehaviour
{
    [SerializeField] private PieceIcon[] piecesMap;
    [SerializeField] private Image pieceImage;
    
    private Button button;
    private Action<PromotionMove> onClicked;
    private PromotionMove move;

    public PromotionPiece Piece { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetIcon(PromotionPiece piece) 
    {
        var foundIcon = piecesMap.FirstOrDefault(x => piece == x.piece);
        if (foundIcon.icon == null) return;
        
        Piece = foundIcon.piece;
        pieceImage.sprite = foundIcon.icon;
    }

    public void SetMove(PromotionMove move, Action<PromotionMove> onClicked) 
    {
        this.move = move;
        this.onClicked = onClicked;
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked() 
    {
        onClicked?.Invoke(move);
        onClicked = null;
    }

    public struct PieceIcon 
    {
        public Sprite icon;
        public PromotionPiece piece;
    }

    public enum PromotionPiece
    {
        Queen,
        Rook,
        Knight,
        Bishop
    }

}
