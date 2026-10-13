using UnityEngine;

namespace TankSurvival.Combat
{
    public abstract class DeliveryHandler : MonoBehaviour
    {
        public abstract void Fire(Vector3 direction);
    }
}
