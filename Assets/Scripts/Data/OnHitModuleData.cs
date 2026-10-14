using UnityEngine;

namespace TankSurvival.Data
{
    [CreateAssetMenu(menuName = "Tank Survival/OnHit Module Data")]
    public class OnHitModuleData : ScriptableObject
    {
        public enum OnHitKind
        {
            Explosion,
            Burn
        }

        public OnHitKind kind;
        public int priority;

        public float radius;
        public float damage;
        public GameObject explosionVfx;

        public float duration;
        public float tickDamage;
        public float tickRate;
    }
}