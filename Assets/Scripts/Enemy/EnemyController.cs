using System.Collections.Generic;
using TankSurvival.Core;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public static readonly List<EnemyController> Active = new List<EnemyController>();

        public EnemyData data;
        public Transform target;
        public EnemyHealth health;
        public float speedMultiplier = 1f;

        public IDamageable Damageable => health;

        private const float AttackRange = 1.5f;
        private const float AttackInterval = 1f;

        private IDamageable _targetDamageable;
        private float _attackCooldown;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            Active.Add(this);
        }

        private void OnDisable()
        {
            Active.Remove(this);
        }

        public void Init(EnemyData data, Transform target, PoolManager pool)
        {
            this.data = data;
            this.target = target;
            _targetDamageable = target.GetComponent<IDamageable>();
            if (_targetDamageable == null)
            {
                Debug.LogError("EnemyController: target has no IDamageable.", this);
            }

            health.Init(data);
            health.OnDeath += () => pool.Release(gameObject);
        }

        private void Update()
        {
            if (data == null || target == null)
            {
                return;
            }

            _attackCooldown -= Time.deltaTime;

            float dx = target.position.x - transform.position.x;
            float dz = target.position.z - transform.position.z;
            float distanceSqr = dx * dx + dz * dz;
            if (distanceSqr > AttackRange * AttackRange)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, target.position, data.speed * speedMultiplier * Time.deltaTime);
                return;
            }

            if (_attackCooldown <= 0f && _targetDamageable != null)
            {
                _targetDamageable.TakeDamage(data.damage);
                _attackCooldown = AttackInterval;
            }
        }
    }
}
