using System;

namespace TankSurvival.Combat.Shooting
{
    public class BuildSnapshot
    {
        public IOnSpawn[] spawnModules = Array.Empty<IOnSpawn>();
        public IOnFlight[] flightModules = Array.Empty<IOnFlight>();
        public IOnHit[] hitModules = Array.Empty<IOnHit>();
        public IOnPostHit[] postHitModules = Array.Empty<IOnPostHit>();
        public IOnDestroy[] destroyModules = Array.Empty<IOnDestroy>();

        public float damage;
        public float speed;

        public static readonly BuildSnapshot Empty = new BuildSnapshot();

        public BuildSnapshot Without<T>()
        {
            BuildSnapshot copy = new BuildSnapshot();
            copy.spawnModules = WithoutType<T, IOnSpawn>(spawnModules);
            copy.flightModules = WithoutType<T, IOnFlight>(flightModules);
            copy.hitModules = WithoutType<T, IOnHit>(hitModules);
            copy.postHitModules = WithoutType<T, IOnPostHit>(postHitModules);
            copy.destroyModules = WithoutType<T, IOnDestroy>(destroyModules);
            copy.damage = damage;
            copy.speed = speed;
            return copy;
        }

        private static TModule[] WithoutType<T, TModule>(TModule[] source) where TModule : class
        {
            if (source == null || source.Length == 0)
            {
                return Array.Empty<TModule>();
            }

            int kept = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (!(source[i] is T))
                {
                    kept++;
                }
            }

            if (kept == source.Length)
            {
                return source;
            }

            if (kept == 0)
            {
                return Array.Empty<TModule>();
            }

            TModule[] result = new TModule[kept];
            int index = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (!(source[i] is T))
                {
                    result[index] = source[i];
                    index++;
                }
            }

            return result;
        }
    }
}
