namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections.Generic;

    public class RandomGameObject : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField]
        protected List<GameObject> gameObjects;
        [Header("Settings")]
        [SerializeField]
        protected bool randomYRotation = false;
        [SerializeField]
        protected bool alignToWorldSpace = false;
        [Header("Scale")]
        [SerializeField]
        protected float minScale = 1f;
        [SerializeField]
        protected float maxScale = 1f;
        [Header("Chance")]
        [SerializeField]
        protected float chance = 1f;

        protected void Awake()
        {
            if (gameObjects.Count > 0 && Random.value <= chance)
            {
                Quaternion rot;
                if (randomYRotation)
                {
                    rot = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                }
                else
                {
                    if (alignToWorldSpace)
                    {
                        rot = Quaternion.identity;
                    }
                    else
                    {
                        rot = transform.rotation;
                    }
                }

                GameObject prefab = gameObjects[Random.Range(0, gameObjects.Count)];
                GameObject go = GameObject.Instantiate<GameObject>(prefab, transform.position + prefab.transform.position, rot, transform.parent);
                float scale = Random.Range(minScale, maxScale);
                go.transform.localScale = go.transform.localScale * scale;
            }

            Destroy(gameObject);
        }
    }
}