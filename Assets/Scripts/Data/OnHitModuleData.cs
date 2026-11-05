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
            Ground,
            Pierce,
            Homing,
            BeamSplit,
            LaserFocus
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

        public int maxPierceCount = 3;
        public float damageFalloff = 0.8f;

        public float homingStrength = 3f;
        public float homingRadius = 3f;

        public int splitCount = 3;
        public float splitAngle = 45f;

        public float damageGrowth = 0.1f;
        public int maxStacks = 5;
        public bool stackResetOnSwitch = true;

        public int maxDepth = 5;
    }
}