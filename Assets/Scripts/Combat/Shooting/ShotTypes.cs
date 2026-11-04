namespace TankSurvival.Combat.Shooting
{
    public enum ShotState
    {
        Spawn,
        Flight,
        Hit,
        PostHit,
        Destroy,
        Dead
    }

    public enum DestroyReason
    {
        Hit,
        Expired,
        External
    }

    public enum DeliveryType
    {
        Projectile,
        Hitscan,
        Cone,
        AoE
    }
}
