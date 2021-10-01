namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;

    public class KGAnimation : MonoBehaviour, IIntroElement
    {

        [SerializeField]
        protected GameObject movingLight;
        [SerializeField]
        protected float lightDistance = 5f;
        [SerializeField]
        protected float startDelay = 1f;
        [SerializeField]
        protected float animTime = 3f;

        protected float halfAnimTime;

        private void Awake()
        {
            halfAnimTime = animTime / 2f;
            movingLight.transform.position = new Vector3(-lightDistance, movingLight.transform.position.y, movingLight.transform.position.z);
        }

        private void Update()
        {
            if (startDelay > 0f)
            {
                startDelay -= Time.deltaTime;
                return;
            }
            if (animTime > 0f)
            {
                if (animTime >= halfAnimTime)
                {
                    // first half
                    float ratio = (animTime - halfAnimTime) / halfAnimTime;
                    movingLight.transform.position = new Vector3(
                        Easing.Circular.InOut(ratio) * -lightDistance,
                        movingLight.transform.position.y,
                        movingLight.transform.position.z
                    );
                }
                else
                {
                    // second half
                    float ratio = 1f - (animTime / halfAnimTime);
                    movingLight.transform.position = new Vector3(
                        Easing.Circular.InOut(ratio) * lightDistance,
                        movingLight.transform.position.y,
                        movingLight.transform.position.z
                    );
                }
            }

            animTime -= Time.deltaTime;
        }

        public bool IsComplete
        {
            get
            {
                return animTime <= 0f;
            }
        }
    }
}