using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using KazatanGames.Framework;
using TMPro;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class SimpleDialogueViewer : BaseDialogueViewer
    {
        [SerializeField]
        protected TextMeshPro textField;
        [SerializeField]
        protected AudioSource audioSource;
        [SerializeField]
        protected float distanceFromCamera = 0.1f;

        protected float baseAudioSourceVolume;
        protected Vector3 startPosition;

        private void Awake()
        {
            baseAudioSourceVolume = audioSource.volume;
            startPosition = transform.position;
            Reposition();
        }

        public override void ShowLine(DialogueLoadedLine line)
        {
            base.ShowLine(line);
            textField.text = line.text;
            audioSource.clip = line.audioClip;
            audioSource.volume = baseAudioSourceVolume * line.volumeMulti;
            audioSource.Play();

            StartCoroutine(DestroyAfter(line.audioClip.length));
        }

        protected void Reposition()
        {
            //transform.position = startPosition;
            //transform.LookAt(Camera.main.transform);
            //float d = Vector3.Distance(startPosition, Camera.main.transform.position) - distanceFromCamera;
            //transform.Translate(Vector3.forward * d, Space.Self);

            //Vector3 dPos = Camera.main.transform.position - startPosition;
            //transform.position = dPos.normalized * distanceFromCamera;
            //transform.LookAt(Camera.main.transform);
        }

        private IEnumerator DestroyAfter(float time)
        {
            yield return new WaitForSeconds(time);

            Destroy(gameObject);
        }

        private void LateUpdate()
        {
            Reposition();
        }
    }
}