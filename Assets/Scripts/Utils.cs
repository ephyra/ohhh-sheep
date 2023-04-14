using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float TileWidth = 80f;
    public static float TileHeight = 120f;
    public static Vector2 TileSize = new Vector2(TileWidth, TileHeight);
    public static readonly Dictionary<TileTypes, Color> ColorMap =
    new Dictionary<TileTypes, Color>
    {
        {TileTypes.Red, Color.red},
        {TileTypes.Blue, Color.blue},
        {TileTypes.Yellow, Color.yellow},
        {TileTypes.Green, Color.green},
        {TileTypes.Orange, new Color(0.96f, 0.51f, 0.19f, 1f)},
        {TileTypes.Purple, new Color(0.57f, 0.12f, 0.71f, 1f)},
        {TileTypes.Brown, new Color(0.67f, 0.43f, 0.16f, 1f)},
        {TileTypes.Cyan, new Color(0.27f, 0.94f, 0.94f, 1f)},
        {TileTypes.Magenta, new Color(0.94f, 0.20f, 0.90f, 1f)},
        {TileTypes.Pink, new Color(0.98f, 0.75f, 0.83f, 1f)},
        {TileTypes.Black, Color.black},
        {TileTypes.White, Color.white},
    };

    public static void UpdateTileSize(Vector2 size)
    {
        TileSize = size;
        TileWidth = size.x;
        TileHeight = size.y;
    }
    public static bool Overlapping (float firstXMin, float firstXMax, float firstYMin, float firstYMax, 
            float secondXMin, float secondXMax, float secondYMin, float secondYMax) 
    {
        return !(firstXMax <= secondXMin || firstXMin >= secondXMax || firstYMax <= secondYMin || firstYMin >= secondYMax);
    }
}
