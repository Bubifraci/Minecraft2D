using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

    public GameObject gras;
    public GameObject dirt;
    public GameObject stone;
    public GameObject water;

    public Dictionary<int, GameObject> tileset;

    // Start is called before the first frame update
    void Start()
    {
        CreateTileset();
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, gras);
        tileset.Add(1, dirt);
        tileset.Add(2, stone);
        tileset.Add(3, water);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
