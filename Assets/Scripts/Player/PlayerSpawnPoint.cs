using UnityEngine;

namespace TankSurvival.Player
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
