using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseMap : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] int mapWidth = 16;
    [SerializeField] int mapHeight = 9;
    [SerializeField] float magnification = 7f;

    [SerializeField] GameObject gras;
    [SerializeField] GameObject dirt;
    [SerializeField] GameObject stone;
    [SerializeField] GameObject water;

    Dictionary<int, GameObject> tileGroups;
    Dictionary<int, GameObject> tileset;

    [SerializeField] Transform player;

    [SerializeField] int renderDistance = 3;

    List<List<Chunk>> chunks = new List<List<Chunk>>();
    [SerializeField] float seed = 0f;

    void Start()
    {
        if(seed == 0f)
        {
            seed = Random.Range(-10000f, 10000f);
        } else
        {
            seed *= 10000f;
        }
        CreateTileset();
        CreateTileGroups();

        chunks.Add(new List<Chunk>());
        Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, 0, 0, tileset, tileGroups, seed, true);
        chunks[0].Add(newChunk);
        generateChunk(newChunk, 0f, 0f);
    }

    private void Update()
    {
        Vector2 playerPos = GetChunkFromPlayerPos();
        CheckUpperBorder(playerPos);
        CheckLowerBorder(playerPos);
        CheckRightBorder(playerPos);
        CheckLeftBorder(playerPos);
        UnloadFarChunks();
    }

    private void CheckUpperBorder(Vector2 playerPos)
    {
        Chunk currentChunk = chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x)];
        Vector3 topBorder = GetTopBorderChunk(currentChunk);

        //Vertical border value at 5,5
        if (Mathf.Abs(topBorder.y - cam.transform.position.y) <= 7f)
        {
            if(playerPos.y == 0)
            {
                List<Chunk> upperChunkList = new List<Chunk>();
                for (int i = 0; i < chunks[0].Count; i++)
                {
                    if (i == playerPos.x)
                    {
                        Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(topBorder.x - mapHeight / 2 + 1), Mathf.FloorToInt(topBorder.y + mapHeight / 2), tileset, tileGroups, seed, true);
                        upperChunkList.Add(newChunk);
                        generateChunk(newChunk, topBorder.x - mapHeight / 2 + 1, topBorder.y + mapHeight / 2 + 1);
                    }
                    else
                    {
                        Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, 0, 0, tileset, tileGroups, seed, false);
                        upperChunkList.Add(newChunk);
                    }
                }
                chunks.Insert(0, upperChunkList);
            } else
            {
                Chunk upperChunk = chunks[Mathf.FloorToInt(playerPos.y - 1)][Mathf.FloorToInt(playerPos.x)];
                if(!upperChunk.isActive)
                {
                    Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(topBorder.x - mapHeight / 2 + 1), Mathf.FloorToInt(topBorder.y + mapHeight / 2), tileset, tileGroups, seed, true);
                    generateChunk(newChunk, topBorder.x - mapHeight / 2 + 1, topBorder.y + mapHeight / 2 + 1);
                    chunks[Mathf.FloorToInt(playerPos.y - 1)][Mathf.FloorToInt(playerPos.x)] = newChunk;
                } else if(!upperChunk.isLoaded())
                {
                    upperChunk.LoadChunk();
                }
            }
        }
    }

    private void CheckLowerBorder(Vector2 playerPos)
    {
        int verticalLength = chunks.Count - 1;
        Chunk currentChunk = chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x)];
        Vector3 bottomBorder = GetBottomBorderChunk(currentChunk);

        //Vertical border value at 5,5
        if (Mathf.Abs(bottomBorder.y - cam.transform.position.y) <= 7f)
        {
            if (playerPos.y == verticalLength)
            {
                List<Chunk> bottomChunkList = new List<Chunk>();
                for (int i = 0; i < chunks[verticalLength].Count; i++)
                {
                    if (i == playerPos.x)
                    {
                        Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(bottomBorder.x + mapHeight / 2), Mathf.FloorToInt(bottomBorder.y - mapHeight / 2), tileset, tileGroups, seed, true);
                        bottomChunkList.Add(newChunk);
                        generateChunk(newChunk, bottomBorder.x + mapHeight / 2, bottomBorder.y - mapHeight / 2);
                    }
                    else
                    {
                        Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, 0, 0, tileset, tileGroups, seed, false);
                        bottomChunkList.Add(newChunk);
                    }
                }
                chunks.Add(bottomChunkList);
            }
            else
            {
                Chunk bottomChunk = chunks[Mathf.FloorToInt(playerPos.y + 1)][Mathf.FloorToInt(playerPos.x)];
                if (!bottomChunk.isActive)
                {
                    Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(bottomBorder.x + mapHeight / 2), Mathf.FloorToInt(bottomBorder.y - mapHeight / 2), tileset, tileGroups, seed, true);
                    generateChunk(newChunk, bottomBorder.x + mapHeight / 2, bottomBorder.y - mapHeight / 2);
                    chunks[Mathf.FloorToInt(playerPos.y + 1)][Mathf.FloorToInt(playerPos.x)] = newChunk;
                } else if (!bottomChunk.isLoaded())
                {
                    bottomChunk.LoadChunk();
                }
            }
        }
    }

    private void CheckRightBorder(Vector2 playerPos)
    {
        int maxHorizontalLength = chunks[0].Count - 1;
        Chunk currentChunk = chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x)];
        Vector3 topBorder = GetTopBorderChunk(currentChunk);

        //Horizontal border value at 8.5
        if (Mathf.Abs(topBorder.x - cam.transform.position.x) <= 10f)
        {
            if(playerPos.x + 1 > maxHorizontalLength)
            {
                Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(topBorder.x + mapWidth / 2 + 1), Mathf.FloorToInt(topBorder.y - mapHeight / 2 + 1), tileset, tileGroups, seed, true);
                generateChunk(newChunk, Mathf.FloorToInt(topBorder.x + mapWidth / 2 + 1), Mathf.FloorToInt(topBorder.y - mapHeight / 2 + 1));
                for(int i = 0; i < chunks.Count; i++)
                {
                    if(i == Mathf.FloorToInt(playerPos.y))
                    {
                        chunks[i].Add(newChunk);
                    } else
                    {
                        Chunk emptyChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, 0, 0, tileset, tileGroups, seed, false);
                        chunks[i].Add(emptyChunk);
                    }
                }

            } else
            {
                Chunk nextChunk = chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x) + 1];
                if (!nextChunk.isActive)
                {
                    Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(topBorder.x + mapWidth / 2 + 1), Mathf.FloorToInt(topBorder.y - mapHeight / 2 + 1), tileset, tileGroups, seed, true);
                    generateChunk(newChunk, Mathf.FloorToInt(topBorder.x + mapWidth / 2 + 1), Mathf.FloorToInt(topBorder.y - mapHeight / 2 + 1));
                    chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x) + 1] = newChunk;
                } else if (!nextChunk.isLoaded())
                {
                    nextChunk.LoadChunk();
                }
            }
        }
    }

    private void CheckLeftBorder(Vector2 playerPos)
    {
        Chunk currentChunk = chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x)];
        Vector3 bottomBorder = GetBottomBorderChunk(currentChunk);

        //Horizontal border value at 8.5
        if (Mathf.Abs(bottomBorder.x - cam.transform.position.x) <= 10f)
        {
            if (playerPos.x == 0)
            {
                Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(bottomBorder.x - mapWidth / 2), Mathf.FloorToInt(bottomBorder.y + mapHeight / 2), tileset, tileGroups, seed, true);
                generateChunk(newChunk, Mathf.FloorToInt(bottomBorder.x - mapWidth / 2), Mathf.FloorToInt(bottomBorder.y + mapHeight / 2));
                for (int i = 0; i < chunks.Count; i++)
                {
                    if (i == Mathf.FloorToInt(playerPos.y))
                    {
                        chunks[i].Insert(0, newChunk);
                    }
                    else
                    {
                        Chunk emptyChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, 0, 0, tileset, tileGroups, seed, false);
                        chunks[i].Insert(0, emptyChunk);
                    }
                }

            }
            else
            {
                Chunk nextChunk = chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x) - 1];
                if (!nextChunk.isActive)
                {
                    Chunk newChunk = new Chunk(gras, dirt, stone, water, mapWidth, mapHeight, magnification, Mathf.FloorToInt(bottomBorder.x - mapWidth / 2), Mathf.FloorToInt(bottomBorder.y + mapHeight / 2), tileset, tileGroups, seed, true);
                    generateChunk(newChunk, Mathf.FloorToInt(bottomBorder.x - mapWidth / 2), Mathf.FloorToInt(bottomBorder.y + mapHeight / 2));
                    chunks[Mathf.FloorToInt(playerPos.y)][Mathf.FloorToInt(playerPos.x) - 1] = newChunk;
                } else if (!nextChunk.isLoaded())
                {
                    nextChunk.LoadChunk();
                }
            }
        }
    }

    void UnloadFarChunks()
    {
        List<Chunk> farChunks = getFarChunks();

        foreach(Chunk chunk in farChunks)
        {
            chunk.UnloadChunk();
        }
    }

    List<Chunk> getFarChunks()
    {
        List<Chunk> farChunks = new List<Chunk>();

        Vector2 currentPos = GetChunkFromPlayerPos();
        int currentX = Mathf.FloorToInt(currentPos.x);
        int currentY = Mathf.FloorToInt(currentPos.y);
        for(int y = 0; y<chunks.Count; y++)
        {
            for(int x = 0; x<chunks[y].Count; x++)
            {
                if(Mathf.Abs(currentX - x) + Mathf.Abs(currentY - y) > renderDistance)
                {
                    farChunks.Add(chunks[y][x]);
                }
            }
        }
        return farChunks;
    }

    Vector2 GetChunkFromPlayerPos()
    {
        Vector2 playerPos = player.position;
        Vector3 topBorder = GetTopMostBorder();
        Vector3 leftBorder = GetLeftMostBorder();

        int yPos = Mathf.FloorToInt(Mathf.Abs(playerPos.y - topBorder.y) / mapHeight);
        int xPos = Mathf.FloorToInt(Mathf.Abs(leftBorder.x - playerPos.x) / mapWidth);

        return new Vector2(xPos, yPos);
    }

    void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> tile in tileset)
        {
            GameObject tileGroup = new GameObject(tile.Value.name);
            tileGroups.Add(tile.Key, tileGroup);
        }
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, gras);
        tileset.Add(1, dirt);
        tileset.Add(2, stone);
        tileset.Add(3, water);
    }

    void generateChunk(Chunk chunk, float xOffset, float yOffset)
    {
        List<List<GameObject>> tileInstances = new List<List<GameObject>>();

        List<List<Tile>> tiles = chunk.getTileGrid();
        for (int y = 0; y < tiles.Count; y++)
        {
            tileInstances.Add(new List<GameObject>());
            foreach (Tile tile in tiles[y])
            {
                GameObject tileInstance = Instantiate(tile.tilePrefab, tile.tileGroup.transform);
                tileInstance.name = tile.name;
                tileInstance.transform.localPosition = new Vector3(tile.x + xOffset - mapWidth / 2, tile.y + yOffset - mapHeight / 2, 0f);
                tileInstances[y].Add(tileInstance);
            }
        }
        chunk.setTileInstances(tileInstances);
    }

    Vector3 GetLeftMostBorder()
    {
        foreach (List<Chunk> chunkList in chunks)
        {
            if (chunkList[0].isActive)
            {
                return chunkList[0].tileInstances[0][0].transform.position;
            }
        }
        return new Vector3(0, 0, 0);
    }

    Vector3 GetBottomMostBorder()
    {
        int bottomY = chunks[0].Count - 1;
        for(int i = 0; i<chunks[bottomY].Count; i++)
        {
            if(chunks[bottomY][i].isActive)
            {
                Chunk bottomChunk = chunks[bottomY][i];
                return bottomChunk.tileInstances[0][0].transform.position;
            }
        }
        return new Vector3(0, 0, 0);
    }

    Vector3 GetTopMostBorder()
    {
        for (int i = 0; i < chunks[0].Count; i++)
        {
            if (chunks[0][i].isActive)
            {
                Chunk topChunk = chunks[0][i];
                return topChunk.tileInstances[topChunk.tileInstances.Count - 1][topChunk.tileInstances[topChunk.tileInstances.Count - 1].Count - 1].transform.position;
            }
        }
        return new Vector3(0, 0, 0);
    }

    Vector3 GetBottomBorderChunk(Chunk bottomChunk)
    {
        return bottomChunk.tileInstances[0][0].transform.position;
    }

    Vector3 GetTopBorderChunk(Chunk topChunk)
    {
        return topChunk.tileInstances[topChunk.tileInstances.Count - 1][topChunk.tileInstances[topChunk.tileInstances.Count - 1].Count - 1].transform.position;
    }
}
