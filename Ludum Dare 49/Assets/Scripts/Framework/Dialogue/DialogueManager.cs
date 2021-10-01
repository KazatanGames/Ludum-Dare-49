using System;
using System.Collections.Generic;
using UnityEngine;
using KazatanGames.Framework;

/**
 * © Kazatan Games, 2020
 */
namespace KazatanGames.Game
{
    public class DialogueManager : SingletonMonoBehaviour<DialogueManager>
    {
        [SerializeField]
        protected float defaultTimeUntilNextLine = 4f;
        
        public static string DIALOGUE_RESOURCES_SUBPATH = "Dialogue/";

        protected HashSet<string> loadedLineDataFiles;

        protected Dictionary<string, DialogueSetData> dialogueSets;
        protected Dictionary<string, DialogueLineData> dialogueLines;

        protected DialogueSetData currentDialogueSet;
        protected Queue<DialogueLoadedLine> currentLines;
        protected float timeUntilNextLine = 0f;

        public event Action<DialogueSetData, int> OnDialogueStart;
        public event Action<DialogueLoadedLine, int> OnDialogueLine;
        public event Action<int> OnDialogueSetEnd;

        public DialogueSetData CurrentDialogueSet => currentDialogueSet;

        public int setPlayId = 1; // start at one so default int doesn't match (0)
        public bool playing = false;

        public int StartDialogueSet(string setId)
        {
            Debug.Log($"[DialogueManager].StartDialogueSet({setId})");

            // end previous
            if (playing) StopDialogueSet();

            // increment setPlayId
            setPlayId++;
            playing = true;

            Debug.Log($"[DialogueManager].StartDialogueSet({setId}) setPlayId = {setPlayId}");

            if (!dialogueSets.ContainsKey(setId))
            {
                Debug.LogWarning("[DialogueManager].StartDialogueSet() - Set not loaded: " + setId);
                return setPlayId;
            }
            currentDialogueSet = dialogueSets[setId];

            currentLines = new Queue<DialogueLoadedLine>(GetLinesForSet(currentDialogueSet));

            if (currentLines.Count == 0)
            {
                Debug.LogWarning("[DialogueManager].StartDialogueSet() - Set has no lines: " + setId);
                return setPlayId;
            }

            Debug.Log($"[DialogueManager] Event: OnDialogueStart({setId})");
            OnDialogueStart?.Invoke(currentDialogueSet, setPlayId);
            timeUntilNextLine = currentLines.Peek().prePause;

            return setPlayId;
        }

        public void StopDialogueSet()
        {
            if (currentDialogueSet == null) return;
            StopDialogueSet(currentDialogueSet.setPlayId);
        }
        public void StopDialogueSet(int setPlayId)
        {
            Debug.Log($"[DialogueManager].StopDialogueSet({setPlayId})");
            playing = false;
            if (currentDialogueSet == null || currentDialogueSet.setPlayId != setPlayId)
            {
                Debug.LogWarning($"[DialogueManager] No Set to Stop.");
                return;
            }
            Debug.Log($"[DialogueManager].StopDialogueSet() - End of Dialogue [STOPPED]");
            currentDialogueSet = null;
            currentLines = new Queue<DialogueLoadedLine>();
            OnDialogueSetEnd?.Invoke(setPlayId);
        }

        public void LoadSet(string setId)
        {
            Debug.Log($"[DialogueManager].LoadSet({setId})");

            if (!dialogueSets.ContainsKey(setId))
            {
                LoadDialogueSetData(setId);
                if (!dialogueSets.ContainsKey(setId)) return;
            }
            DialogueSetData dsd = dialogueSets[setId];
            foreach (string lfPath in dsd.lineFiles)
            {
                LoadDialogueLineFileData(lfPath);
            }

            foreach (DialogueSetContentsData dscd in dsd.contents)
            {
                if (dscd.isSet)
                {
                    LoadSet(dscd.setId);
                }
            }
        }

        public void SkipLine()
        {
            timeUntilNextLine = 0;
        }

        protected override void Initialise()
        {
            base.Initialise();

            loadedLineDataFiles = new HashSet<string>();

            dialogueSets = new Dictionary<string, DialogueSetData>();
            dialogueLines = new Dictionary<string, DialogueLineData>();

            currentLines = new Queue<DialogueLoadedLine>();
        }

        protected void LoadDialogueSetData(string filePath)
        {
            Debug.Log($"[DialogueManager].LoadDialogueSetData({filePath})");

            TextAsset setText = Resources.Load<TextAsset>(DIALOGUE_RESOURCES_SUBPATH + filePath);
            if (setText == null)
            {
                Debug.LogWarning("[DialogueManager].LoadDialogueSetData() - File not found: " + DIALOGUE_RESOURCES_SUBPATH + filePath);
                return;
            }
            try
            {
                DialogueSetData dsd = JsonUtility.FromJson<DialogueSetData>(setText.text);
                dialogueSets.Add(dsd.setId, dsd);
            }
            catch (Exception)
            {
                Debug.LogError("[DialogueManager].LoadDialogueSetData() - Invalid json: " + DIALOGUE_RESOURCES_SUBPATH + filePath);
            }
        }

        protected void LoadDialogueLineFileData(string filePath)
        {
            if (loadedLineDataFiles.Contains(filePath)) return;
            Debug.Log($"[DialogueManager].LoadDialogueLineFileData({filePath})");

            loadedLineDataFiles.Add(filePath);

            TextAsset lineFileText = Resources.Load<TextAsset>(DIALOGUE_RESOURCES_SUBPATH + filePath);
            if (lineFileText == null)
            {
                Debug.LogWarning("[DialogueManager].LoadDialogueLineFileData() - File not found: " + DIALOGUE_RESOURCES_SUBPATH + filePath);
                return;
            }

            try
            {
                DialogueLineFileData dlfd = JsonUtility.FromJson<DialogueLineFileData>(lineFileText.text);
                foreach (DialogueLineData dld in dlfd.lines)
                {
                    dialogueLines.Add(dld.lineId, dld);
                }
            }
            catch (Exception)
            {
                Debug.LogError("[DialogueManager].LoadDialogueLineFileData() - Invalid json: " + DIALOGUE_RESOURCES_SUBPATH + filePath);
            }
        }

        protected List<DialogueLoadedLine> GetLinesForSet(DialogueSetData setData)
        {
            List<DialogueLoadedLine> lines = new List<DialogueLoadedLine>();
            for (int i = 0; i < setData.contents.Length; i++)
            {
                DialogueSetContentsData dscd = currentDialogueSet.contents[i];
                if (dscd.isSet)
                {
                    if (!dialogueSets.ContainsKey(dscd.setId))
                    {
                        Debug.LogWarning("[DialogueManager].GetLinesForSet() - Set not loaded: " + dscd.setId);
                        continue;
                    }
                    lines.AddRange(GetLinesForSet(dialogueSets[dscd.setId]));
                }
                else
                {
                    if (!dialogueLines.ContainsKey(dscd.lineId))
                    {
                        Debug.LogWarning("[DialogueManager].GetLinesForSet() - Line not loaded: " + dscd.lineId);
                        continue;
                    }
                    lines.Add(new DialogueLoadedLine(dialogueLines[dscd.lineId], dscd));
                }
            }
            return lines;
        }

        protected void ShowLine()
        {
            if (currentLines.Count == 0)
            {
                // the end of the set
                playing = false;
                currentDialogueSet = null;
                Debug.Log($"[DialogueManager] Event: OnDialogueEnd({setPlayId})");
                OnDialogueSetEnd?.Invoke(setPlayId);
                MusicManager2.INSTANCE.RemoveDucker(this);
                return;
            }
            DialogueLoadedLine dll = currentLines.Dequeue();

            // duck audio
            MusicManager2.INSTANCE.AddDucker(this);

            timeUntilNextLine = dll.audioClip != null ? dll.audioClip.length : defaultTimeUntilNextLine;
            if (currentLines.Count > 0) timeUntilNextLine += currentLines.Peek().prePause;

            Debug.Log($"[DialogueManager] Event: OnDialogueLine({dll.lineId})");
            OnDialogueLine?.Invoke(dll, setPlayId);
        }

        protected void Update()
        {
            if (playing)
            {
                if (currentDialogueSet != null)
                {
                    timeUntilNextLine -= Time.deltaTime;
                    if (timeUntilNextLine < 0)
                    {
                        ShowLine();
                    }
                }
                else
                {
                    playing = false;
                    // no currentDialogueSet indicates there was a problem playing so now end
                    OnDialogueSetEnd?.Invoke(setPlayId);
                }
            }
        }
    }
}