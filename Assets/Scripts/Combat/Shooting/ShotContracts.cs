namespace TankSurvival.Combat.Shooting
{
    public interface IOnSpawn
    {
        void OnSpawn(ShotContext ctx);
    }

    public interface IOnFlight
    {
        void OnFlight(ShotContext ctx, float dt);
    }

    public interface IOnHit
    {
        void OnHit(ShotContext ctx, Target target);
    }

    public interface IOnPostHit
    {
        void OnPostHit(ShotContext ctx);
    }

    public interface IOnDestroy
    {
        void OnDestroy(ShotContext ctx);
    }

    public interface IResettable
    {
        void ResetState();
    }
}
