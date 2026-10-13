namespace TankSurvival.Core
{
    public interface IPoolable
    {
        void OnGetFromPool();
        void OnReleaseFromPool();
    }
}
