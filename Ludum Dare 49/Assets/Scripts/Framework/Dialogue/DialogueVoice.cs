using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class DialogueVoice : MonoBehaviour
    {
        [SerializeField]
        protected string actorId;
        [SerializeField]
        protected AudioSource audioSource;

        protected int setPlayId;

        private void Awake()
        {
            if (actorId != "")
            {
                DialogueManager.INSTANCE.OnDialogueStart += ManageDialogueStart;
                DialogueManager.INSTANCE.OnDialogueLine += ManageDialogueLine;
                DialogueManager.INSTANCE.OnDialogueSetEnd += ManageDialogueEnd;
            }
        }

        private void OnDestroy()
        {
            if (actorId != "")
            {
                DialogueManager.INSTANCE.OnDialogueStart -= ManageDialogueStart;
                DialogueManager.INSTANCE.OnDialogueLine -= ManageDialogueLine;
                DialogueManager.INSTANCE.OnDialogueSetEnd -= ManageDialogueEnd;
            }
        }

        protected void ManageDialogueLine(DialogueLoadedLine dll, int setPlayId)
        {
            if (dll.actor == actorId)
            {
                if (setPlayId != this.setPlayId)
                {
                    audioSource.Stop();
                }

                this.setPlayId = setPlayId;
                audioSource.clip = dll.audioClip;
                audioSource.volume = AppManager.INSTANCE.AppModel.audioPreferences.Data.globalVolume * dll.volumeMulti;
                audioSource.Play();
                Debug.Log($"{this}.ManageDialogueLine({setPlayId})");
            }
        }

        protected void ManageDialogueStart(DialogueSetData dsd, int setPlayId)
        {

        }

        protected void ManageDialogueEnd(int setPlayId)
        {
            Debug.Log($"{this}.ManageDialogueEnd({setPlayId})");
            if (setPlayId == this.setPlayId)
            {
                audioSource.Stop();
            }
        }
    }
}