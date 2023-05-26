namespace Threeyes.Pool
{
    public interface IPoolableHandler
    {
        void OnSpawn();
        void OnDespawn();
    }
}