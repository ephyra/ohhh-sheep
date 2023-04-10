using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public const float TileWidth = 80f;
    public const float TileHeight = 120f;
    public static readonly Vector2 TileSize = new Vector2(TileWidth, TileHeight);

    public static bool Overlapping (float firstXMin, float firstXMax, float firstYMin, float firstYMax, 
            float secondXMin, float secondXMax, float secondYMin, float secondYMax) 
    {
        return !(firstXMax <= secondXMin || firstXMin >= secondXMax || firstYMax <= secondYMin || firstYMin >= secondYMax);
    }
}
