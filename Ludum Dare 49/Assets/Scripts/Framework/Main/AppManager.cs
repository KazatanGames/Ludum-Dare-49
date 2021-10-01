namespace KazatanGames.Framework
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;
    /**
     * App Manager
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * The App Manager is the main entry point.
     * An instance of which should exist on the Load Scene (which is the entrypoint scene).
     * It performs any system specific configuration, then creates any prefabs (typically
     * singleton managers), then instantiates an AppModel and loads the intiial scene.
     */
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        [SerializeField]
        protected List<GameObject> prefabsToCreate;
        [SerializeField]
        protected List<GameObject> introElements;
        [SerializeField]
        protected AppConfigSO config;

        public AppModel AppModel { get; protected set; }
        public AppConfigSO AppConfig => config;

        protected bool introComplete;

        protected override void Initialise()
        {
            introComplete = false;

            // 1. System specific configuration

            // 2. Create AppModel
            CreateAppModel();

            // 3. Create any Mono Behaviours
            foreach (GameObject p in prefabsToCreate)
            {
                GameObject.Instantiate(p);
            }
        }

        protected void CreateAppModel()
        {
            AppModel = new AppModel
            {
                debugMode = config.debugMode
            };
        }

        protected bool IntrosComplete
        {
            get
            {
                if (config.skipIntros) return true;
                foreach (GameObject go in introElements)
                {
                    IIntroElement iie = go.GetComponent<IIntroElement>();
                    if (!iie.IsComplete) return false;
                }
                return true;
            }
        }

        private void Update()
        {
            if (!introComplete && IntrosComplete)
            {
                introComplete = true;

                foreach (GameObject go in introElements)
                {
                    GameObject.Destroy(go);
                }

                // 4. Switch to initial scene
                SceneManager.LoadScene(config.initialScene, LoadSceneMode.Single);
            }

            if (SystemInfo.deviceType == DeviceType.Desktop && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F4)))
            {
                Application.Quit();
            }
        }
    }
}