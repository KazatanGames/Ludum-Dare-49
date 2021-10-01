namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;

    public class RotatingGameItem : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 axis = Vector3.up;
        [SerializeField]
        protected float speed = 180f;

        //protected float rotationIncrement;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(axis, speed * Time.deltaTime, Space.World);
        }
    }
}