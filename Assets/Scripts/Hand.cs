using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    Tile lastDrawnTile;
    Transform lastParentTrans;
    Vector3 lastPos;
    [SerializeField]
    Storage storage;
    Dictionary<TileTypes, int> typesCount;
    List<Tile> currHand;
    Dictionary<TileTypes, List<Tile>> typeCount;
    // Start is called before the first frame update
    void Start()
    {
        typesCount = new Dictionary<TileTypes, int>();
        foreach (TileTypes types in System.Enum.GetValues(typeof(TileTypes)))
        {
            typesCount.Add(types, 0);
        }
        currHand = new List<Tile>();
        typeCount = new Dictionary<TileTypes, List<Tile>>();
    }
    
    public bool IsHandFull()
    {
        return currHand.Count >= 7;
    }

    public int GetHandCount()
    {
        return currHand.Count;
    }

    public List<Tile> InsertTile(Tile tile)
    {
        TileTypes currType = tile.GetTileTypes();
        //alr in hand, need to find sibling index to slot
        Transform tileTransform = tile.transform;
        lastParentTrans = tileTransform.parent;
        lastPos = tileTransform.position;
        tileTransform.SetParent(this.transform);
        int currCount = typesCount[currType];
        int currTypeStartIdx = 0;
        if (currCount != 0)
        {
            for(int i = 0; i < currHand.Count; i++)
            {
                if (currHand[i].GetTileTypes() == currType)
                {
                    currTypeStartIdx = i;
                    tileTransform.SetSiblingIndex(i + currCount);
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)this.transform);
                    break;
                }
            }
        }
        typesCount[currType]++;
        currHand.Insert(tileTransform.GetSiblingIndex(), tile);
        if (typesCount[currType] == 3)
        {
            lastDrawnTile = null;
            typesCount[currType] = 0;
            //completed 3, remove from List and return to game manager to destroy
            List<Tile> set = currHand.GetRange(currTypeStartIdx, 3);
            currHand.RemoveRange(currTypeStartIdx, 3);
            return set;
        }
        else
        {
            lastDrawnTile = tile;
        }

        return null;  
    }

    public bool Store()
    {
        if (currHand.Count == 0) return false;
        List<Tile> toStore = new List<Tile>();
        for (int i = 0; i < 3; i++)
        {
            if (currHand.Count == 0) break;
            Tile currTile = currHand[0];
            typesCount[currTile.GetTileTypes()]--;
            currHand.RemoveAt(0);
            toStore.Add(currTile);
        }
        storage.Store(toStore);
        return true;
    }

    public bool Undo()
    {
        if (lastDrawnTile == null) return false;
        TileTypes type = lastDrawnTile.GetTileTypes();
        typesCount[type]--;
        lastDrawnTile.SetBlocker(false);
        lastDrawnTile.transform.SetParent(lastParentTrans);
        lastDrawnTile.transform.position = lastPos;
        currHand.Remove(lastDrawnTile);
        if (lastParentTrans.gameObject.name.Contains("Layer"))
        {
            GameManager.Instance.ReAddTile(lastDrawnTile);
        } else
        {
            storage.ReAddTile(lastDrawnTile);
        }
        lastDrawnTile = null;
        return true;
    }
}
