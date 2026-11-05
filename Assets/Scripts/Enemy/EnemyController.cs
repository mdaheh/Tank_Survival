using System.Collections.Generic;
using TankSurvival.Combat.Shooting;
using TankSurvival.Core;
using TankSurvival.Data;
using UnityEngine;

namespace TankSurvival.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public static readonly List<EnemyController> Active = new List<EnemyController>();
        private static readonly Dictionary<IDamageable, EnemyController> ByDamageable = new Dictionary<IDamageable, EnemyController>();

        public EnemyData data;
        public Transform target;
        public EnemyHealth health;
        public float speedMultiplier = 1f;
        public float controlSpeedMultiplier = 1f;

        public IDamageable Damageable => health;

        private const float AttackRange = 1.5f;
        private const float AttackInterval = 1f;

        private readonly ShotFactory _shotFactory = new ShotFactory();
        private ShotSystem _shotSystem;
        private IDamageable _targetDamageable;
        private float _attackCooldown;
        private float _shootCooldown;

        private Vector3 _knockbackVelocity;
        private float _knockbackDuration;
        private float _knockbackTimer;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            Active.Add(this);
            if (health != null)
            {
                ByDamageable[health] = this;
            }
        }

        private void OnDisable()
        {
            Active.Remove(this);
            if (health != null)
            {
                ByDamageable.Remove(health);
            }
        }

        public static bool TryGetByDamageable(IDamageable damageable, out EnemyController enemy)
        {
            if (damageable == null)
            {
                enemy = null;
                return false;
            }

            return ByDamageable.TryGetValue(damageable, out enemy);
        }

        public void Init(EnemyData data, Transform target, EnemyKillSystem killSystem)
        {
            this.data = data;
            this.target = target;
            _targetDamageable = target.GetComponent<IDamageable>();
            if (_targetDamageable == null)
            {
                Debug.LogError("EnemyController: target has no IDamageable.", this);
            }

            health.Init(data);
            health.OnDeath += () => killSystem.Enqueue(gameObject);

            controlSpeedMultiplier = 1f;
            _knockbackVelocity = Vector3.zero;
            _knockbackTimer = 0f;
            _knockbackDuration = 0f;
        }

        public void ApplyKnockback(Vector3 velocity, float duration)
        {
            _knockbackVelocity = velocity;
            _knockbackDuration = Mathf.Max(0.0001f, duration);
            _knockbackTimer = duration;
        }

        private ShotSystem ShotSystem
        {
            get
            {
                if (_shotSystem == null)
                {
                    _shotSystem = GameServices.Get<ShotSystem>();
                }

                return _shotSystem;
            }
        }

        private void Update()
        {
            if (data == null || target == null)
            {
                return;
            }

            if (_knockbackTimer > 0f)
            {
                transform.position += _knockbackVelocity * (_knockbackTimer / _knockbackDuration) * Time.deltaTime;
                _knockbackTimer -= Time.deltaTime;
            }

            _attackCooldown -= Time.deltaTime;
            _shootCooldown -= Time.deltaTime;

            float dx = target.position.x - transform.position.x;
            float dz = target.position.z - transform.position.z;
            float distanceSqr = dx * dx + dz * dz;

            if (data.behavior == "ranged" && distanceSqr > AttackRange * AttackRange)
            {
                if (distanceSqr > data.shootRange * data.shootRange)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position, target.position,
                        data.speed * speedMultiplier * controlSpeedMultiplier * Time.deltaTime);
                    return;
                }

                if (_shootCooldown <= 0f && ShotSystem != null)
                {
                    Vector3 dir = (target.position - transform.position).normalized;
                    ShotSystem.Fire(_shotFactory.BuildProjectile(transform.position, dir,
                        data.damage, data.projectileSpeed, data.shootRange * 2f, Faction.Enemy));
                    _shootCooldown = data.shootInterval;
                }
                return;
            }

            if (distanceSqr > AttackRange * AttackRange)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, target.position,
                    data.speed * speedMultiplier * controlSpeedMultiplier * Time.deltaTime);
                return;
            }

            if (_attackCooldown <= 0f && _targetDamageable != null)
            {
                _targetDamageable.TakeDamage(data.damage);
                _attackCooldown = AttackInterval;
            }
        }

#if UNITY_EDITOR || DEBUG
        public float KnockbackRemaining => _knockbackTimer;
#endif
    }
}
