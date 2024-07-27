using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject tilePrefab;

    public void Initialize()
    {
        
    }

    public GameObject CreateTile(int x, int y, Tile.Type type)
    {
        GameObject tile = Instantiate(tilePrefab, new Vector2(x * Tile.WIDTH, y * Tile.HEIGHT), Quaternion.identity);
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.type = type;
        tileScript.Initialize();
        return tile;
    }

}
