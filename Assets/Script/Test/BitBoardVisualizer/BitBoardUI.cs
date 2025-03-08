using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BitBoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text number;
    [SerializeField] private TMP_Text bitNumber;
    [SerializeField] private float tileOffset;

    public Tile tile { get; private set; }

    public void Set(Tile tile, Color color) 
    {
        this.tile = tile;

        Vector3 pos = tile.visualTile.transform.position;
        pos.y += tileOffset;
        transform.position = pos;
        transform.parent = tile.visualTile.transform;

        number.text = tile.Index.ToString();
        bitNumber.text = tile.Bitboard.value.ConvertToBinaryString();

        SetColor(color);
    }

    public void SetColor(Color color) 
    {
        number.color = bitNumber.color = color;
    }
}
