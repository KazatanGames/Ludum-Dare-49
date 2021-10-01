namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;
    /**
     * Base Scene Manager
     * 
     * Kazatan Games Framework
     * 
     * The Base Scene manager is the base class for all scene specific managers.
     * It adds some common methods and plays music from this scenes Catalog.
     */
    public abstract class BaseSceneManager : MonoBehaviour
    {
        protected void Awake()
        {
            Initialise();
        }

        protected abstract void Initialise();
    }
}