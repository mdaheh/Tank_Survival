using UnityEngine;

public class SessionState : MonoBehaviour
{
    [SerializeField] private ChassisData selectedChassis;

    public ChassisData SelectedChassis => selectedChassis;
}
