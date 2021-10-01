using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class DialogueAnimator : MonoBehaviour
    {
        [SerializeField]
        protected string actorId;
        [SerializeField]
        protected Animator anim;

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
            if (dll.actor == actorId && dll.animationTriggerName != "")
            {
                anim.SetTrigger(dll.animationTriggerName);
            }
        }

        protected void ManageDialogueStart(DialogueSetData dsd, int setPlayId)
        {

        }

        protected void ManageDialogueEnd(int setPlayId)
        {
            
        }
    }
}