using UnityEngine;

[CreateAssetMenu(menuName = "Tank Survival/Chassis Data", fileName = "ChassisData")]
public class ChassisData : ScriptableObject
{
    public float health;
    public float speed;
    public float turn;
    public string abilityId;
}
