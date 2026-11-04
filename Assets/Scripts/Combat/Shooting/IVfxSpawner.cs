using UnityEngine;

namespace TankSurvival.Combat.Shooting
{
    public interface IVfxSpawner
    {
        void Spawn(GameObject prefab, Vector3 position);
    }
}
