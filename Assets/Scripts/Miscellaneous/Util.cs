
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;

public class Util
{
    
    public static void ApplyItemEntityDispersalForce(GameObject itemEntity)
    {
        System.Random rand = new System.Random();
        float force = Game.Properties.ItemEntityDispersalForce;
        itemEntity.GetComponent<Rigidbody2D>().AddForce(new Vector2((float)(rand.NextDouble() * 2 - 1) * force, (float)(rand.NextDouble() * 2 - 1) * force));
    }

}
