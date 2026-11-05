using TankSurvival.Core;
using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public static class Combatants
    {
        private static Transform _playerTransform;
        private static IDamageable _playerDamageable;

        public static void RegisterPlayer(Transform player, IDamageable damageable)
        {
            _playerTransform = player;
            _playerDamageable = damageable;
        }

        public static void UnregisterPlayer()
        {
            _playerTransform = null;
            _playerDamageable = null;
        }

        public static bool TryGetPlayer(out Vector3 position, out IDamageable damageable)
        {
            if (_playerTransform == null || _playerDamageable == null)
            {
                position = Vector3.zero;
                damageable = null;
                return false;
            }

            position = _playerTransform.position;
            damageable = _playerDamageable;
            return true;
        }
    }
}