using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{

    [Header("Values")]
    public float max;
    public float value;

    [Header("Effects")]
    [Tooltip("The duration for which this character cannot be damaged again after being damaged (Iframes).")]
    public float recoverTime;
    float currentRecoverTime;

    public bool isInvincible;
    [Label] public bool isRecovering;

    [Header("Events")]
    public Event onDamage;
    public Event onHeal;
    public Event onDeath;

    public float AsPercentage => value / max;
    [Auto] public Knockback knockback;

    public bool Damage(float by)
    {

        if (value <= 0)
            return false;

        if (currentRecoverTime > 0)
            return false;
        currentRecoverTime = recoverTime;

        value = Mathf.Clamp(value - by, 0, max);

        if (value > 0)
            onDamage.trigger.Invoke(this);
        else
            onDeath.trigger.Invoke(this);

        return true;

    }

    public void Heal(float by)
    {

        if (value == 0)
            return;

        value = Mathf.Clamp(value + by, 0, max);
        onDamage.trigger.Invoke(this);

    }

    private void Update()
    {
        if (currentRecoverTime > 0)
            currentRecoverTime -= Time.deltaTime;
    }

    [System.Serializable]
    public struct Event
    {
        public HealthEvent trigger;
        [System.Serializable]
        public class HealthEvent : UnityEvent<Health>
        { }
    }

}
