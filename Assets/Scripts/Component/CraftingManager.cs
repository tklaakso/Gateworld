using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{

    private Dictionary<ItemIdentifier, CraftingRecipe> recipes;

    public void Initialize()
    {
        recipes = new Dictionary<ItemIdentifier, CraftingRecipe>();
        recipes[new ItemIdentifier(Item.Type.PICKAXE, 0)] = new CraftingRecipe(new ItemIdentifier(Item.Type.PICKAXE, 0), new List<Item> { Item.Create(Tile.Type.STONE, 3), Item.Create(Item.Type.WOOD, 2) });
    }

    public bool CraftSingleItem(ItemIdentifier identifier)
    {
        if (!recipes.ContainsKey(identifier))
            return false;
        CraftingRecipe recipe = recipes[identifier];
        if (!Game.InventoryManager.ConsumeItems(recipe.Ingredients))
            return false;
        Game.InventoryManager.AddItem(Item.Create(identifier));
        return true;
    }

    public List<ItemIdentifier> GetCraftableItems()
    {
        Dictionary<ItemIdentifier, int> itemAmounts = Game.InventoryManager.GetAvailableItemAmounts();
        List<ItemIdentifier> craftable = new List<ItemIdentifier>();
        foreach (KeyValuePair<ItemIdentifier, CraftingRecipe> recipe in recipes)
        {
            bool isCraftable = true;
            foreach (Item item in recipe.Value.Ingredients)
            {
                if (!itemAmounts.ContainsKey(item.Identifier) || itemAmounts[item.Identifier] < item.quantity)
                {
                    isCraftable = false;
                    break;
                }
            }
            if (isCraftable)
                craftable.Add(recipe.Key);
        }
        return craftable;
    }

}
