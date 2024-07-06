using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public GameObject tilePrefab;

    public static GameObject CreateTile(int x, int y, Tile.Type type)
    {
        GameObject tile = Instantiate(instance.tilePrefab, new Vector2(x * Tile.WIDTH, y * Tile.HEIGHT), Quaternion.identity);
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.type = type;
        tileScript.Initialize();
        return tile;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
