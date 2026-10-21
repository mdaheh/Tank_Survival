using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/OnHit Module Data")]
    public class OnHitModuleData : ScriptableObject
    {
        public enum OnHitKind
        {
            Explosion,
            Burn,
            Slow,
            Freeze,
            Knockback,
            Ground
        }

        public OnHitKind kind;
        public int priority;

        public float radius;
        public float damage;
        public GameObject explosionVfx;

        public float duration;
        public float tickDamage;
        public float tickRate;

        public float slowAmount;
        public int freezeThreshold;
        public float force;
        public float damageScaling;
        public GameObject groundVfx;
        public int maxDepth = 5;
    }
}