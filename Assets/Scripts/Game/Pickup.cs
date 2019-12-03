using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{

    public Item item;

    [ShowIf(nameof(IsStackable))]
    public int count;

    public UnityEvent triggered;

    bool IsStackable()
    {
        return item && item.isStackable;
    }

    //TODO: Do we need conditions? (for shops, for example, if the game has them?)

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player.Current.gameObject)
        {
            Inventory.Add(item, count);
            triggered.Invoke();
            Destroy(gameObject);
        }
    }

}
