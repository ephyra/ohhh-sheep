using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    [SerializeField]
    GameObject tilePrefab;
    int size;
    [SerializeField]
    RectTransform myRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(float localPosX, float localPosY, int size, int amountTiles, List<Tile> lowerLevelTiles)
    {
        this.size = size;
        float totalPossible = size * size;
        int currCount = lowerLevelTiles.Count;
        myRectTransform.localPosition = new Vector3(localPosX, localPosY, 0);
        myRectTransform.sizeDelta = new Vector2(size * Utils.TileWidth, size * Utils.TileHeight);
        List<Tile> generatedTiles = new List<Tile>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (size <= 2 || Random.value < (amountTiles/totalPossible))
                {
                    GameObject currGO = Instantiate(tilePrefab, gameObject.transform);
                    currGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * Utils.TileWidth, j * Utils.TileHeight);
                    //currGO.transform.localPosition = new Vector3(i * Utils.TileWidth, j * Utils.TileHeight, 0);
                    currGO.name = "Tile" + currCount;
                    Tile curr = currGO.GetComponent<Tile>();
                    foreach (Tile tile in lowerLevelTiles)
                    {
                        if (!tile.gameObject.activeSelf) continue;
                        if (curr.IsOverlappingWith(tile))
                        {
                            tile.SetInteractable(false);
                            tile.CheckAndAddCoveredQuadrants(curr);
                            tile.CheckAndDeactivateIfCovered();
                        }
                    }
                    generatedTiles.Add(curr);
                    amountTiles--;
                    currCount++;
                }

                totalPossible--;
                if (amountTiles <= 0) break;
            }
            if (amountTiles <= 0) break;
        }
        lowerLevelTiles.AddRange(generatedTiles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
