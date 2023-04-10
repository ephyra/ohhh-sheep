using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    int handCount = 0;
    Tile lastDrawnTile;
    Transform lastParentTrans;
    Vector3 lastPos;
    [SerializeField]
    Storage storage;

    Dictionary<TileTypes, List<Tile>> typeCount;
    // Start is called before the first frame update
    void Start()
    {
        typeCount = new Dictionary<TileTypes, List<Tile>>();
    }
    
    public bool IsHandFull()
    {
        return handCount >= 7;
    }

    public int GetHandCount()
    {
        return handCount;
    }

    public List<Tile> InsertTile(Tile tile)
    {
        handCount++;
        TileTypes currType = tile.GetTileTypes();
        if (!typeCount.ContainsKey(currType))
        {
            typeCount.Add(currType, new List<Tile>());
        }

        int pos = 0;
        foreach(KeyValuePair<TileTypes, List<Tile>> kvp in typeCount)
        {
            
            pos += kvp.Value.Count;
            if (kvp.Key == currType)
            {
                Transform tileTransform = tile.transform;
                lastParentTrans = tileTransform.parent;
                lastPos = tileTransform.position;
                tileTransform.SetParent(this.transform);
                tileTransform.SetSiblingIndex(pos);
                kvp.Value.Add(tile);
                if (kvp.Value.Count == 3)
                {
                    lastDrawnTile = null;
                    handCount -= 3;
                    //completed 3, remove from dictionary and return to game manager to remove from list
                    List<Tile> set = typeCount[currType];
                    typeCount.Remove(currType);
                    return set;
                } else
                {
                    lastDrawnTile = tile;
                }
                break;
            }
        }
        return null;  
    }

    public bool Store()
    {
        if (handCount == 0) return false;
        List<Tile> toStore = new List<Tile>();
        List<TileTypes> emptiedTypes = new List<TileTypes>();
        foreach (KeyValuePair<TileTypes, List<Tile>> kvp in typeCount)
        {
            while (toStore.Count < 3 && kvp.Value.Count > 0)
            {
                toStore.Add(kvp.Value[0]);
                kvp.Value.RemoveAt(0);
            }
            if (kvp.Value.Count == 0) emptiedTypes.Add(kvp.Key);
        }

        foreach(TileTypes type in emptiedTypes)
        {
            typeCount.Remove(type);
        }
        handCount -= toStore.Count;
        storage.Store(toStore);
        return true;
    }

    public bool Undo()
    {
        if (lastDrawnTile == null) return false;
        if (lastParentTrans.gameObject.name.Contains("Layer"))
        {
            GameManager.Instance.ReAddTile(lastDrawnTile);
        } else
        {
            storage.IncreaseTileCount();
        }
        handCount--;
        TileTypes type = lastDrawnTile.GetTileTypes();
        foreach(KeyValuePair<TileTypes, List<Tile>> kvp in typeCount)
        {
            if (kvp.Key == type)
            {
                kvp.Value.Remove(lastDrawnTile);
                break;
            }
        }
        if (typeCount[type].Count == 0) typeCount.Remove(type);
        lastDrawnTile.SetBlocker(false);
        lastDrawnTile.transform.SetParent(lastParentTrans);
        lastDrawnTile.transform.position = lastPos;
        return true;
    }
}
