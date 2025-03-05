namespace CheesyUtils
{
    public class PersistentObjects : Singleton<PersistentObjects>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}