using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardSize
{
    public int width;
    public int height;
    public int mines;

    public BoardSize(int width, int height, int mines)
    {
        this.width = width;
        this.height = height;
        this.mines = mines;
    }
}
