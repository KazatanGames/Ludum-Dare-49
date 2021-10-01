namespace KazatanGames.Framework
{
    using UnityEngine;
    /**
     * Singleton MonoBehaviour
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * Simple abstract singleton base class for other App wide singletons to extend.
     */
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T INSTANCE { get; protected set; }

        // Use this for initialization
        protected void Awake()
        {
            if (INSTANCE != null && INSTANCE != this)
            {
                Destroy(this);
                Debug.LogWarning("An instance of a SingletonMonoBehaviour already existed: " + typeof(T));
            }
            else
            {
                INSTANCE = (T)this;
            }

            if (PersistAcrossScenes) DontDestroyOnLoad(gameObject);

            Initialise();
        }

        protected virtual void Initialise() { }

        protected virtual bool PersistAcrossScenes => true;
    }
}