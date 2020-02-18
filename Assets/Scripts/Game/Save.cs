using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveManager
{

    public static Data Current { get; private set; }

    static string Folder => Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "save")).FullName;

    public static Task<Data[]> Load()
    {
        var tasks = new List<Task<Data>>();
        foreach (var file in Directory.GetFiles(Folder, "*.json"))
            tasks.Add(Load(file));
        return Task.WhenAll(tasks).ContinueWith(t => t.Result);
    }

    public static Task<Data> Load(int slot) =>
        Load(slot.ToString());

    public static Task<Data> Load(string file) =>
    Task.Run(() =>
        JsonUtility.FromJson<Data>(File.ReadAllText(EnsureExtension(file))).WithSource(EnsureExtension(file)));

    public static Task Save(Data data, string file) =>
    Task.Run(() =>
        File.WriteAllText(EnsureExtension(file), JsonUtility.ToJson(data, true)));

    public static Task Save() =>
        Save(Current, Current.source);

    public static async Task Create(string name) =>
        await Save(new Data(name), Path.Combine(Folder, await FindAvailableName() + ".json"));

    public static Task Remove(string name) =>
        Task.Run(() => File.Delete(Path.Combine(EnsureExtension(name))));

    static async Task<int> FindAvailableName() =>
        Enumerable.Range(0, int.MaxValue).
        Except((await Load()).Select(s => int.Parse(Path.GetFileNameWithoutExtension(s.source)))).
        FirstOrDefault();

    static string EnsureExtension(string name) =>
        name.EndsWith(".json") ? name : name + ".json";

    public static void Activate(Data data) =>
        Current = data;

    public class Data
    {

        [NonSerialized]
        public string source;

        public string name;
        public int hearts;
        public int rupees;
        public int triforcePieces;
        public Item[] items;

        public Data()
        { }

        public Data(string name) =>
            this.name = name;

        public Data WithSource(string source)
        {
            this.source = source;
            return this;
        }

    }

    public class Item
    {

        public int count;
        public global::Item item;
        
        public static Item[] FromInventory() =>
            Player.Current.inventory.Items.Select(kvp => new Item() { item = kvp.Key, count = kvp.Value }).ToArray();

        public static void OverwriteInventory(Item[] items) =>
            Player.Current.inventory.Set(items.ToDictionary(i => i.item, i => i.count));

    }

}
