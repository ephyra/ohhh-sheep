using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storage : MonoBehaviour
{
    [SerializeField]
    List<Transform> tileStores;
    int tileCount = 0;
    
    public void Store(List<Tile> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].SetBlocker(false);
            tileCount++;
            Tile prevChild = null;
            if (tileStores[i].childCount != 0)
            {
                prevChild = tileStores[i].GetChild(0).GetComponent<Tile>();
            }
            tiles[i].transform.SetParent(tileStores[i]);
            if (prevChild != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)tileStores[i]);
                prevChild.SetInteractable(false);
                prevChild.CheckAndAddCoveredQuadrants(tiles[i]);
            }
        }
    }

    public void ReduceTileCount()
    {
        tileCount--;
    }

    public void ReAddTile(Tile tile)
    {
        IncreaseTileCount();
        tile.CheckOverlapAgainst(GetFirstLayerTiles(), true);
    }

    public void IncreaseTileCount()
    {
        tileCount++;
    }

    public int GetTileCount()
    {
        return tileCount;
    }

    public List<Tile> GetFirstLayerTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach(Transform store in tileStores)
        {
            if (store.childCount == 2)
            {
                tiles.Add(store.GetChild(0).GetComponent<Tile>());
            }
        }
        return tiles;
    }
}
