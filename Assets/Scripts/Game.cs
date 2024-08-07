using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private Game instance;

    public GameObject playerPrefab;

    public static World World { get; private set; }
    public static Player Player { get; private set; }
    public static SpriteManager SpriteManager { get; private set; }
    public static InventoryManager InventoryManager { get; private set; }
    public static EntityManager EntityManager { get; private set; }
    public static CraftingManager CraftingManager { get; private set; }
    public static BuildManager BuildManager { get; private set; }
    public static Properties Properties { get; private set; }

    void Awake()
    {
        instance = this;
        World = GetComponent<World>();
        SpriteManager = GetComponent<SpriteManager>();
        InventoryManager = GetComponent<InventoryManager>();
        EntityManager = GetComponent<EntityManager>();
        CraftingManager = GetComponent<CraftingManager>();
        BuildManager = GetComponent<BuildManager>();
        Properties = GetComponent<Properties>();
        SpriteManager.Initialize();
        CraftingManager.Initialize();
        InventoryManager.Initialize();
        EntityManager.Initialize();
        BuildManager.Initialize();
        World.Initialize();
        Properties.Initialize();
        Player = Instantiate(playerPrefab, EntityManager.FindPlayerSpawn(), Quaternion.identity).GetComponent<Player>();
    }

}
