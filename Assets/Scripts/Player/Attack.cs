using System.Collections;
using System.Linq;
using UnityEngine;

public class Attack : MonoBehaviour
{

    [Required] public LinkAnimationController animator;
    [Required] public Health health;
    public float duration;

    private void Update()
    {
    
        if (Input.GetMouseButtonDown(0) && canAttack)
            StartCoroutine(DoAttack());

        if (canAttack)
            return;

        var enemies = EnemiesHit();
        foreach (var enemy in enemies)
            if (enemy.Damage(1))
                enemy.knockback.InDirection(animator.direction);

    }

    bool canAttack = true;
    IEnumerator DoAttack()
    {

        canAttack = false;

        Player.Current.Disable();
        yield return Player.Current.animator.SetState(LinkAnimationController.AnimationState.Attacking, duration);
        yield return new WaitForSeconds(0.25f);
        Player.Current.Enable();

        canAttack = true;

    }

    (Vector3 origin, Vector3 destination) line;
    Health[] EnemiesHit()
    {

        var origin = transform.position + (0.5f, -0.5f, 0f).ToVector() + (animator.direction.ToVector3() * 0.6f);
        var destination = origin + (animator.direction.ToVector3() * 0.5f);
        line = (origin, destination);
        return Physics2D.LinecastAll(origin, destination).Select(c => c.collider.GetComponent<Health>()).Where(h => h && h != health).ToArray();

    }

    private void OnDrawGizmos()
    {
        
        if (canAttack)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(line.origin, line.destination);

    }

}
