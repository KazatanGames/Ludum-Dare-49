using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public abstract class OptimizedMonobehaviourGroup<Self, T> : MonoBehaviour where Self : OptimizedMonobehaviourGroup<Self, T> where T : IOptimizedMonobehaviour
    {
        private static OptimizedMonobehaviourGroup<Self, T> instance;

        protected List<T> registered;

        public static OptimizedMonobehaviourGroup<Self, T> Group
        {
            get
            {
                if (instance == null)
                {
                    GameObject container = new GameObject(typeof(Self).Name);
                    instance = container.AddComponent<Self>();
                }
                return instance;
            }
        }

        public OptimizedMonobehaviourGroup() {
            registered = new List<T>();
        }

        public void Register(T registration)
        {
            if (!registered.Contains(registration)) registered.Add(registration);
        }
        public void Unregister(T deregistration)
        {
            registered.Remove(deregistration);
        }

        private void Update()
        {
            foreach(T registree in registered)
            {
                registree.OptimizedUpdate();
            }
        }
    }
}