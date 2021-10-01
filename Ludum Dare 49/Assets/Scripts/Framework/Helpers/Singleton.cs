namespace KazatanGames.Framework
{
    using UnityEngine;
    using System;

    /**
     * Singleton
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * Simple abstract singleton base class for other App wide singletons to extend.
     */
    public abstract class Singleton<T> where T : Singleton<T>
    {
        public static T Current { get; protected set; } = (T)Activator.CreateInstance(typeof(T));

        // Use this for initialization
        public Singleton()
        {
            if (Current == (T)this) return;

            if (Current != null)
            {
                Current.Destroy();
            }

            Current = (T)this;

            Initialise();
        }

        protected virtual void Initialise() { }

        public virtual void Destroy() { }
    }
}