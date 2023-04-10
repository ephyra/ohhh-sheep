using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField]
    List<Transform> tileStores;
    int tileCount = 0;
    // Start is called before the first frame update

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
                prevChild.SetInteractable(false);
                prevChild.CheckAndAddCoveredQuadrants(tiles[i]);
            }
        }
    }

    public void ReduceTileCount()
    {
        tileCount--;
    }

    public void IncreaseTileCount()
    {
        tileCount++;
    }

    public int GetTileCount()
    {
        return tileCount;
    }
}
