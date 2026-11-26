using UnityEngine;

namespace CSVS.GodMode.Destruction
{
    public enum DamageType
    {
        Kinetic,
        Thermal,
        Explosive,
        Crush
    }

    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDestroyed { get; }
        void TakeDamage(float amount, DamageType damageType, Vector3 hitPoint, Vector3 hitNormal);
    }
}
