namespace KazatanGames.Framework
{
    using UnityEngine;
    using System;

    public class EventChannel<T> : Singleton<T> where T : EventChannel<T>
    {

        /***
         * ChannelClosing event
         ***/
        public event Action OnChannelClosing;
        protected void DispatchChannelClosing()
        {
            OnChannelClosing?.Invoke();
        }

        public override void Destroy()
        {
            DispatchChannelClosing();
        }

        protected override void Initialise()
        {

        }
    }
}