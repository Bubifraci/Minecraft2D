using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    float seed;
    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;

    GameObject gras;
    GameObject dirt;
    GameObject stone;
    GameObject water;

    int chunkWidth = 16;
    int chunkHeight = 9;
    float magnification = 7f;

    int xOffset = 0;
    int yOffset = 0;

    List<List<int>> noiseGrid = new List<List<int>>();
    List<List<Tile>> tileGrid = new List<List<Tile>>();

    public List<List<GameObject>> tileInstances = new List<List<GameObject>>();

    public bool isActive;

    public Chunk(GameObject gras, GameObject dirt, GameObject stone, GameObject water, int chunkWidth, int chunkHeight, float magnification, int xOffset, int yOffset, Dictionary<int, GameObject> tileset, Dictionary<int, GameObject> tileGroups, float seed, bool isActive)
    {
        this.gras = gras;
        this.dirt = dirt;
        this.stone = stone;
        this.water = water;
        this.chunkWidth = chunkWidth;
        this.chunkHeight = chunkHeight;
        this.magnification = magnification;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.tileset = tileset;
        this.tileGroups = tileGroups;
        this.isActive = isActive;
        this.seed = seed;
        if(isActive)
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            noiseGrid.Add(new List<int>());
            tileGrid.Add(new List<Tile>());

            for (int x = 0; x < chunkWidth; x++)
            {
                int tileId = GetIdUsingPerlin(x, y);
                noiseGrid[y].Add(tileId);
                CreateTile(tileId, x, y);
            }
        }
    }

    public void UnloadChunk()
    {
        for(int y = 0; y<tileInstances.Count; y++)
        {
            for(int x = 0; x<tileInstances[y].Count; x++)
            {
                if(tileInstances[y][x] != null)
                {
                    tileInstances[y][x].SetActive(false);
                }
            }
        }
    }

    public void LoadChunk()
    {
        for (int y = 0; y < tileInstances.Count; y++)
        {
            for (int x = 0; x < tileInstances[y].Count; x++)
            {
                if(tileInstances[y][x] != null)
                {
                    tileInstances[y][x].SetActive(true);
                } else
                {
                    tileGrid[y][x] = null;
                }
            }
        }
    }

    public bool isLoaded()
    {
        return tileInstances[0][0].activeSelf;
    }

    void CreateTile(int tileId, int x, int y)
    {
        GameObject tilePrefab = tileset[tileId];
        GameObject tileGroup = tileGroups[tileId];
        //GameObject tile = Instantiate(tilePrefab, tileGroup.transform);

        string name = string.Format("{0}: {1}_{2}", tilePrefab.name, x, y);
        Tile tile = new Tile(x, y, tilePrefab, tileGroup, name);

        tileGrid[y].Add(tile);
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float rawPerlin = Mathf.PerlinNoise((x + xOffset + seed) / magnification, (y + yOffset + seed) / magnification);
        float normalizedPerlin = Mathf.Clamp(rawPerlin, 0f, 1f);
        float scaledPerlin = normalizedPerlin * tileset.Count;
        if (scaledPerlin == 4)
        {
            scaledPerlin = 3;
        }
        return Mathf.FloorToInt(scaledPerlin);
    }

    public List<List<Tile>> getTileGrid()
    {
        return tileGrid;
    }

    Vector3 GetTopBorder()
    {
        Tile tile = tileGrid[0][0];
        return new Vector3(tile.x, tile.y, 0);
    }

    Vector3 GetBottomBorder()
    {
        Tile tile =  tileGrid[tileGrid.Count - 1][tileGrid[tileGrid.Count - 1].Count - 1];
        return new Vector3(tile.x, tile.y, 0);
    }

    public void setTileInstances(List<List<GameObject>> instances)
    {
        tileInstances = instances;
    }
}

public class Tile
{
    public float x;
    public float y;
    public GameObject tilePrefab;
    public GameObject tileGroup;
    public string name;
    public bool isActive;

    public Tile(float x, float y, GameObject tilePrefab, GameObject tileGroup, string name)
    {
        this.x = x;
        this.y = y;
        this.tilePrefab = tilePrefab;
        this.tileGroup = tileGroup;
        this.name = name;
    }
}
