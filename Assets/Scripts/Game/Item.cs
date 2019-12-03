using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{

    [Header("Stackable")]
    public bool isStackable;

    [ShowIf(nameof(isStackable))] public int maxCount;

    public enum UseAction
    {
        None, SpawnObject, Interact
    }

    [Header("Usage")]
    public UseAction useAction;

    [ShowIf(nameof(useAction), UseAction.SpawnObject)] public GameObject spawnObject;
    [ShowIf(nameof(useAction), UseAction.Interact)] public Transform interactable;

    public bool CanUse()
    {
        if (useAction == UseAction.SpawnObject)
            return spawnObject;
        else if (useAction == UseAction.Interact)
            return false; //TODO: Fix interaction (use key on door for example)
                          //return Map.Current.tilemaps.current.GetTile(Player.Current.Position.ToVector3Int());
        return false;
    }

    public void OnUse()
    {
        if (useAction == UseAction.SpawnObject)
            Instantiate(spawnObject, Player.Current.Position.ToVector3() + Player.Current.Direction.ToVector3(), Quaternion.identity);
        else if (useAction == UseAction.Interact)
        { }
    }

}
