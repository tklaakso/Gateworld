
public class ItemIdentifier
{

    public Item.Type Type;
    public int Id;

    public ItemIdentifier(Item.Type type, int id)
    {
        Type = type;
        Id = id;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is ItemIdentifier)) return false;
        ItemIdentifier other = (ItemIdentifier)obj;
        return other.Type == Type && other.Id == Id;
    }

    public override int GetHashCode()
    {
        return 17 * (int)Type + 31 * Id;
    }

}
