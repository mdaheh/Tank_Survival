using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private StatBlock statBlock;
    [SerializeField] private InputActionReference moveAction;

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        transform.Translate(
            input.x * statBlock.moveSpeed * Time.deltaTime,
            0f,
            input.y * statBlock.moveSpeed * Time.deltaTime,
            Space.World);
    }
}
