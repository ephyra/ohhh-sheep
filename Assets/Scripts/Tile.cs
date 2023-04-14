using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    RectTransform myRectTransform;
    [SerializeField]
    Button myButton;
    [SerializeField]
    Image tileImage;
    [SerializeField]
    GameObject blocker;
    TileTypes type;
    [SerializeField]
    Dictionary<Quadrants, Tile> coveredQuadrants;
    HashSet<Tile> coveredTiles;

    void Awake()
    {
        coveredQuadrants = new Dictionary<Quadrants, Tile>();
        coveredTiles = new HashSet<Tile>();
    }


    public void SetInteractable(bool canInteract)
    {
        myButton.interactable = canInteract;
        Color tempColor = tileImage.color;
        tempColor.a = (canInteract) ? 1f : 0.5f;
        tileImage.color = tempColor;
    }

    public void CheckAndAddCoveredQuadrants(Tile other)
    {
        RectTransform otherRect = other.GetComponent<RectTransform>();
        Vector3 myMin = myRectTransform.position;
        Vector3 myMax = myMin + new Vector3(Utils.TileWidth, Utils.TileHeight);
        Vector3 otherMin = otherRect.position;
        Vector3 otherMax = otherMin + new Vector3(Utils.TileWidth, Utils.TileHeight);
        bool hasCovered = false;
        if (!coveredQuadrants.ContainsKey(Quadrants.TopLeft))
        {
            if (Utils.Overlapping(myMin.x, myMin.x + Utils.TileWidth/2, myMax.y - Utils.TileHeight/2, myMax.y, otherMin.x, otherMax.x, otherMin.y, otherMax.y))
            {
                hasCovered = true;
                coveredQuadrants.Add(Quadrants.TopLeft, other);
            }
        }
        if (!coveredQuadrants.ContainsKey(Quadrants.TopRight))
        {
            if (Utils.Overlapping(myMax.x - Utils.TileWidth/2, myMax.x, myMax.y - Utils.TileHeight / 2, myMax.y, otherMin.x, otherMax.x, otherMin.y, otherMax.y))
            {
                hasCovered = true;
                coveredQuadrants.Add(Quadrants.TopRight, other);
            }
        }
        if (!coveredQuadrants.ContainsKey(Quadrants.BottomRight))
        {
            if (Utils.Overlapping(myMax.x - Utils.TileWidth / 2, myMax.x, myMin.y, myMin.y + Utils.TileHeight/2, otherMin.x, otherMax.x, otherMin.y, otherMax.y))
            {
                hasCovered = true;
                coveredQuadrants.Add(Quadrants.BottomRight, other);
            }
        }
        if (!coveredQuadrants.ContainsKey(Quadrants.BottomLeft))
        {
            if (Utils.Overlapping(myMin.x, myMin.x + Utils.TileWidth / 2, myMin.y, myMin.y + Utils.TileHeight / 2, otherMin.x, otherMax.x, otherMin.y, otherMax.y))
            {
                hasCovered = true;
                coveredQuadrants.Add(Quadrants.BottomLeft, other);
            }
        }
        if (hasCovered) other.AddCoveredTile(this);
    }

    public void AddCoveredTile(Tile other)
    {
        coveredTiles.Add(other);
    }

    public void RemoveSelf()
    {
        foreach(Tile other in coveredTiles)
        {
            other.CoveringRemoved(this);
        }
        coveredTiles.Clear();
    }

    public void CoveringRemoved(Tile other)
    {
        HashSet<Quadrants> ToRemove = new HashSet<Quadrants>();
        foreach(KeyValuePair<Quadrants, Tile> kvp in coveredQuadrants)
        {
            if (kvp.Value == other) ToRemove.Add(kvp.Key);
        }
        foreach (Quadrants quadrant in ToRemove) coveredQuadrants.Remove(quadrant);
        CheckAndActivateIfUncovered();
    }

    public void CheckAndActivateIfUncovered()
    {
        if (coveredQuadrants.Count < 4)
        {
            gameObject.SetActive(true);
            if (coveredQuadrants.Count == 0) SetInteractable(true);
        }
    }

    public void CheckAndDeactivateIfCovered()
    {
        if (coveredQuadrants.Count == 4)
        {

            gameObject.SetActive(false);
        }
    }
    public bool IsOverlappingWith(Tile other)
    {
        RectTransform otherRect = other.GetComponent<RectTransform>();
        return Utils.Overlapping(myRectTransform.position.x, myRectTransform.position.x + Utils.TileWidth, myRectTransform.position.y, 
            myRectTransform.position.y + Utils.TileHeight, otherRect.position.x, otherRect.position.x + Utils.TileWidth, 
                otherRect.position.y, otherRect.position.y + Utils.TileHeight);
    }

    public void CheckOverlapAgainst(List<Tile> tiles, bool isStore)
    {
        foreach (Tile tile in tiles)
        {
            if (!tile.gameObject.activeSelf) continue;
            if (IsOverlappingWith(tile))
            {
                tile.SetInteractable(false);
                tile.CheckAndAddCoveredQuadrants(this);
                if (!isStore) tile.CheckAndDeactivateIfCovered();
            }
        }
    }

    public TileTypes GetTileTypes ()
    {
        return type;
    }
    public void SetTileType(TileTypes type)
    {
        this.type = type;
        tileImage.color = Utils.ColorMap[type];
    }

    public void Clicked()
    {
        GameManager.Instance.AttemptInsertTile(this);
    }
    public void SetBlocker(bool active)
    {
        blocker.SetActive(active);
    }
}
