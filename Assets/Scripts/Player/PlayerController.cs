using TankSurvival.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankSurvival.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private StatBlock statBlock;
        [SerializeField] private InputActionReference moveAction;

        private void Update()
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();

            transform.Rotate(0f, input.x * Mathf.Sign(input.y) * statBlock.turn * Time.deltaTime, 0f, Space.Self);
            transform.Translate(Vector3.forward * (input.y * statBlock.moveSpeed * Time.deltaTime), Space.Self);
        }
    }
}
