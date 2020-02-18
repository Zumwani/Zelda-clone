using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Inventory
{

    Dictionary<Item, int> items = new Dictionary<Item, int>();

    public IReadOnlyDictionary<Item, int> Items { get; }

    public Inventory() =>
        Items = new ReadOnlyDictionary<Item, int>(items);

    public void Add(Item item) =>
        items[item] = 1;

    public void Remove(Item item) =>
        items[item] = 0;

    public void Set(Dictionary<Item, int> items) =>
        this.items = items;

    public void Add(Item item, int count = 1)
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

    public void Remove(Item item, int count = 1)
    {
        Add(item, -count);
        Debug.Log("Item removed: " + item.name + ": " + count);
    }

    public bool HasItem(Item item) =>
        items[item] > 0;

}

public static class ItemExtensions
{

    public static void Use(this Item item)
    {

        if (item && item.Count() > 0 && item.CanUse())
        {
            Player.Current.inventory.Remove(item);
            item.OnUse();
        }

    }

    public static int Count(this Item item) =>
        Player.Current.inventory.Items[item];

    public static int MaxCount(this Item item)
    {

        //if (!item)
            return 0;

        //return new[] { item.maxCount }.
        //    Concat(item.upgrades.Select(u => Inventory.HasItem(u.tiedTo) ? u.count : 0)).
        //    Max();

    }

    public static bool HasInInventory(this Item item) =>
        item ? item.Count() > 0 : false;

}
