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

    public void Set(Tile tile, int index, Color color) 
    {
        this.tile = tile;

        Vector3 pos = tile.visualTile.transform.position;
        pos.y += tileOffset;
        transform.position = pos;
        transform.parent = tile.visualTile.transform;

        number.text = index.ToString();
        bitNumber.text = ConvertToBinaryString(tile.Bitboard.value);

        SetColor(color);
    }

    public void SetColor(Color color) 
    {
        number.color = bitNumber.color = color;
    }

    private string ConvertToBinaryString(long value, bool pad = false) 
    {
        string binary = Convert.ToString(value, 2);

        return (pad) ? binary.PadLeft(64, '0') : binary;
    }
}
