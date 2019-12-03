using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public static class Inventory
{

    static readonly Dictionary<Item, int> items = new Dictionary<Item, int>();

    public static IReadOnlyDictionary<Item, int> Items { get; }

    static Inventory()
    {
        Items = new ReadOnlyDictionary<Item, int>(items);
    }

    public static void Add(Item item)
    {
        items[item] = 1;
    }

    public static void Remove(Item item)
    {
        items[item] = 0;
    }

    public static void Add(Item item, int count = 1)
    {

        if (!item.isStackable)
        { 
            Add(item);
            return;
        }

        items[item] += count;
        items.EnsureItemCap(item, item.MaxCount());
        Debug.Log("Item added: " + item.name + ": " + count);

    }

    public static void Remove(Item item, int count = 1)
    {
        Add(item, -count);
        Debug.Log("Item removed: " + item.name + ": " + count);
    }

    public static bool HasItem(Item item)
    {
        return items[item] > 0;
    }

}

public static class ItemExtensions
{

    public static void Use(this Item item)
    {

        if (item && item.Count() > 0 && item.CanUse())
        {
            Inventory.Remove(item);
            item.OnUse();
        }

    }

    public static int Count(this Item item)
    {
        return Inventory.Items[item];
    }

    public static int MaxCount(this Item item)
    {

        //if (!item)
            return 0;

        //return new[] { item.maxCount }.
        //    Concat(item.upgrades.Select(u => Inventory.HasItem(u.tiedTo) ? u.count : 0)).
        //    Max();

    }

    public static bool HasInInventory(this Item item)
    {
        if (!item) return false;
        return item.Count() > 0;
    }

}
