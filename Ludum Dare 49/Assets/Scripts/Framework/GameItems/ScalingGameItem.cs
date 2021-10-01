namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;

    public class ScalingGameItem : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 axis = Vector3.one;
        [SerializeField]
        protected float time = 5f;
        [SerializeField]
        protected float factor = 1.25f;

        //protected float rotationIncrement;

        protected Vector3 referenceScale;
        protected Vector3 maxScale;
        protected Vector3 minScale;
        protected float t;
        protected bool enlarging = true;

        // Use this for initialization
        void Start()
        {
            t = time / 2f;
            referenceScale = transform.localScale;
            minScale = referenceScale / factor;
            maxScale = referenceScale * factor;
        }

        // Update is called once per frame
        void Update()
        {
            // debug
            minScale = referenceScale / factor;
            maxScale = referenceScale * factor;

            if (enlarging)
            {
                t += Time.deltaTime;
                if (t > time)
                {
                    t = (2f * time) - t;
                    enlarging = false;
                }
            } else
            {
                t -= Time.deltaTime;
                if (t < 0f)
                {
                    t = -t;
                    enlarging = true;
                }
            }

            Vector3 newScale = Vector3.Lerp(minScale, maxScale, Easing.Quadratic.InOut(t / time));
            transform.localScale = newScale;
        }
    }
}