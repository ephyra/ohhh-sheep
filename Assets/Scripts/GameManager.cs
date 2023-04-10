using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    [SerializeField]
    GameObject tilePrefab;
    [SerializeField]
    GameObject layerPrefab;
    [SerializeField]
    GameObject playingCanvas;
    [SerializeField]
    Hand hand;
    [SerializeField]
    Vector2 initialStart;
    [SerializeField]
    int minSets;
    [SerializeField]
    int maxSets;
    [SerializeField]
    int initialSize;
    [SerializeField]
    Storage storage;
    List<Tile> tiles;
    int totalSets;
    Dictionary<TileTypes, int> typeCount;
    [SerializeField]
    GameObject restartOverlay;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        typeCount = new Dictionary<TileTypes, int>();
        tiles = new List<Tile>();
        Init();
    }

    public static GameManager Instance
    {
        get
        {
            if (_instance is null) Debug.LogError("game manager instance not found");
            return _instance;
        }
    }

    void Init()
    {
        totalSets = Random.Range(minSets, maxSets + 1);
        int totalType = System.Enum.GetNames(typeof(TileTypes)).Length;
        int tilesToAdd = totalSets * 3;
        int typesToAdd = tilesToAdd;
        for (int i = 0; i < totalType; i++)
        {
            typeCount.Add((TileTypes)i, 3);
            typesToAdd -= 3;
        }

        while (typesToAdd > 0)
        {
            typeCount[(TileTypes)Random.Range(0, totalType)] += 3;
            typesToAdd -= 3;
        }
        int layerCount = 0;
        while (tilesToAdd > 0)
        {
            int sizeSquare = initialSize * initialSize;
            int tilesToGenerate = Mathf.Min(tilesToAdd, (initialSize <= 3) ? sizeSquare : Random.Range(sizeSquare / 2, sizeSquare + 1));
            GameObject currLayer = Instantiate(layerPrefab, playingCanvas.transform);
            currLayer.name = "Layer" + layerCount++;
            currLayer.GetComponent<Layer>().Init(initialStart.x, initialStart.y, initialSize, tilesToGenerate, tiles);
            tilesToAdd -= tilesToGenerate;
            bool positiveDir = Random.value > 0.5;
            if (initialSize == 1)
            {
                initialSize++;
            } else
            {
                bool gridIncrease = Random.value > 0.3;
                initialSize += (gridIncrease) ? 1 : -1;
            }
            int heightChgMultiplier = Random.Range(-1, 2), widthChgMultiplier = Random.Range(-1, 2);
            initialStart += new Vector2(widthChgMultiplier * Utils.TileWidth, heightChgMultiplier * Utils.TileHeight);
        }

        Shuffle();

        //for(int i = 0; i < 3; i++)
        //{
        //    GameObject currGO = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity, playingCanvas.transform);
        //    currGO.name = "Tile" + i;
        //    Tile curr = currGO.GetComponent<Tile>();
        //    curr.SetType(TileTypes.Blue);
        //    foreach(Tile tile in tiles)
        //    {
        //        if (!tile.gameObject.activeSelf) continue;
        //        if(curr.IsOverlappingWith(tile))
        //        {
        //            tile.SetInteractable(false);
        //            tile.CheckAndAddCoveredQuadrants(curr);
        //            tile.CheckAndDeactivateIfCovered();
        //        }
        //    }
        //    tiles.Add(curr);
        //}
    }

    public void AttemptInsertTile(Tile tile)
    {
        tile.RemoveSelf();
        tile.SetBlocker(true);
        List<Tile> tilesToRemove = hand.InsertTile(tile);
        if(tiles.Remove(tile))
        {
            if (--typeCount[tile.GetTileTypes()] == 0) typeCount.Remove(tile.GetTileTypes());
        } else
        {
            storage.ReduceTileCount();
        }
        
        if (tilesToRemove != null)
        {
            foreach(Tile tileToRemove in tilesToRemove)
            {
                tiles.Remove(tileToRemove);
                Destroy(tileToRemove.gameObject);
            }
        }

        if (tiles.Count + storage.GetTileCount() + hand.GetHandCount() == 0)
        {
            //insert victory
        }
        if (hand.IsHandFull())
        {
            //game over
            restartOverlay.SetActive(true);
        }
    }

    public void ReAddTile(Tile tile)
    {
        tiles.Add(tile);
        TileTypes tileType = tile.GetTileTypes();
        if (!typeCount.ContainsKey(tileType))
        {
            typeCount.Add(tileType, 0);
        }
        typeCount[tileType]++;
    }

    public void Shuffle()
    {
        Dictionary<TileTypes, int> typesCopy = new Dictionary<TileTypes, int>(typeCount);
        List<TileTypes> types = new List<TileTypes>(typeCount.Keys);

        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            Tile tile = tiles[i];
            TileTypes currType = types[Random.Range(0, types.Count)];
            if (--typesCopy[currType] == 0)
            {
                typesCopy.Remove(currType);
                types.Remove(currType);
            }
            tile.SetTileType(currType);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
