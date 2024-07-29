
using System.Collections.Generic;

public class CraftingRecipe
{

    public ItemIdentifier Identifier;
    public List<Item> Ingredients;

    public CraftingRecipe(ItemIdentifier identifier, List<Item> ingredients)
    {
        Identifier = identifier;
        Ingredients = ingredients;
    }

}
