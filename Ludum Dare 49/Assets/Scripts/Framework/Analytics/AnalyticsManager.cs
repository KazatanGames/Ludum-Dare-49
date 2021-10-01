using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace KazatanGames.Framework
{
    public class AnalyticsManager : SingletonMonoBehaviour<AnalyticsManager>
    {
        [SerializeField]
        protected bool analyticsEnabledProduction = true;
        [SerializeField]
        protected bool analyticsEnabledNonProduction = false;

        public bool AnalyticsEnabled { get; protected set; }

        public void SetEnabled(bool enabled)
        {
            AnalyticsEnabled = enabled;
            AnalyticsEvent.debugMode = Debug.isDebugBuild;
            PerformanceReporting.enabled = AnalyticsEnabled;
            Analytics.enabled = AnalyticsEnabled;
        }

        protected override void Initialise()
        {
            SetEnabled(Debug.isDebugBuild ? analyticsEnabledNonProduction : analyticsEnabledProduction);
        }
    }
}