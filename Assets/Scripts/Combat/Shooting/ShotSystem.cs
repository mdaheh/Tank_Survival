using System.Collections.Generic;
using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public class ShotSystem : MonoBehaviour
    {
        [SerializeField] private ShotView shotViewPrefab;
        [SerializeField] private int maxDepth = 10;

        private readonly Stack<ShotContext> _pool = new Stack<ShotContext>();
        private readonly List<ActiveShot> _active = new List<ActiveShot>();

        private ProjectileController _projectile;
        private InstantHitscanController _instantHitscan;
        private ContinuousHitscanController _continuousHitscan;
        private ConeController _cone;
        private AoEController _aoe;
        private ZoneController _zone;

        private PoolManager _poolManager;

        private int _nextId;

        private PoolManager PoolService
        {
            get
            {
                if (_poolManager == null)
                {
                    _poolManager = GameServices.Get<PoolManager>();
                }

                return _poolManager;
            }
        }

        private void Awake()
        {
            GameServices.Register(this);
            _projectile = new ProjectileController();
            _instantHitscan = new InstantHitscanController();
            _continuousHitscan = new ContinuousHitscanController();
            _cone = new ConeController();
            _aoe = new AoEController();
            _zone = new ZoneController();
        }

        public int Fire(SpawnRequest request)
        {
            ShotContext ctx = _pool.Count > 0 ? _pool.Pop() : new ShotContext();
            ctx.Reset();

            ctx.position = request.position;
            ctx.direction = request.direction;
            ctx.buildSnapshot = request.buildSnapshot;
            ctx.damage = request.buildSnapshot.damage;
            ctx.speed = request.buildSnapshot.speed;
            ctx.config = request.config;
            ctx.depth = request.depth;
            ctx.faction = request.faction;
            ctx.state = ShotState.Spawn;

            IOnSpawn[] spawnModules = ctx.buildSnapshot.spawnModules;
            for (int i = 0; i < spawnModules.Length; i++)
            {
                spawnModules[i].OnSpawn(ctx);
            }

            FlushSpawnQueue(ctx);

            ShotView view = null;

            switch (request.type)
            {
                case DeliveryType.Projectile:
                    GameObject instance = PoolService.Get(shotViewPrefab.gameObject);
                    view = instance.GetComponent<ShotView>();
                    view.Bind(ctx);
                    break;
            }

            int id = _nextId++;
            _active.Add(new ActiveShot { id = id, ctx = ctx, view = view, type = request.type });

            return id;
        }

        public bool IsAlive(int id)
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].id == id)
                {
                    return true;
                }
            }

            return false;
        }

        public bool SetOrigin(int id, Vector3 position)
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].id != id)
                {
                    continue;
                }

                _active[i].ctx.position = position;
                return true;
            }

            return false;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                ActiveShot shot = _active[i];

                TickShot(shot, dt);
                FlushSpawnQueue(shot.ctx);

                if (shot.ctx.state != ShotState.Dead)
                {
                    continue;
                }

                if (shot.view != null)
                {
                    PoolService.Release(shot.view.gameObject);
                }

                _pool.Push(shot.ctx);
                _active.RemoveAt(i);
            }
        }

        private void TickShot(ActiveShot shot, float dt)
        {
            switch (shot.type)
            {
                case DeliveryType.Projectile:
                    _projectile.Tick(shot.ctx, dt);
                    break;
                case DeliveryType.Hitscan:
                    _instantHitscan.Tick(shot.ctx, dt);
                    break;
                case DeliveryType.ContinuousHitscan:
                    _continuousHitscan.Tick(shot.ctx, dt);
                    break;
                case DeliveryType.Cone:
                    _cone.Tick(shot.ctx, dt);
                    break;
                case DeliveryType.AoE:
                    _aoe.Tick(shot.ctx, dt);
                    break;
                case DeliveryType.Zone:
                    _zone.Tick(shot.ctx, dt);
                    break;
            }
        }

        private void FlushSpawnQueue(ShotContext ctx)
        {
            while (ctx.spawnQueue.Count > 0)
            {
                int last = ctx.spawnQueue.Count - 1;
                SpawnRequest child = ctx.spawnQueue[last];
                ctx.spawnQueue.RemoveAt(last);

                if (child.depth > maxDepth)
                {
                    continue;
                }

                Fire(child);
            }
        }

        private class ActiveShot
        {
            public int id;
            public ShotContext ctx;
            public ShotView view;
            public DeliveryType type;
        }
    }
}
