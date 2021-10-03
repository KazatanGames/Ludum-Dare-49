using KazatanGames.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * © Kazatan Games, 2021
 */
namespace KazatanGames.Game
{
    public class UserInterface : MonoBehaviour
    {
        [SerializeField]
        protected Sprite plusSprite;
        [SerializeField]
        protected Sprite equalsSprite;
        [SerializeField]
        protected KnownReactionRow knownReactionRowPrefab;
        [SerializeField]
        protected TargetsRow targetRowPrefab;
        [SerializeField]
        protected RectTransform reactionsUIContainer;
        [SerializeField]
        protected RectTransform targetsUIContainer;

        [SerializeField]
        protected Button addRButton;
        [SerializeField]
        protected Button addGButton;
        [SerializeField]
        protected Button addBButton;
        [SerializeField]
        protected Slider heatSlider;
        [SerializeField]
        protected TextMeshProUGUI reactionTitleTxt;

        [SerializeField]
        protected MoleculeTypeSO moleculesToAddR;
        [SerializeField]
        protected MoleculeTypeSO moleculesToAddG;
        [SerializeField]
        protected MoleculeTypeSO moleculesToAddB;

        [SerializeField]
        protected AudioSource waterAudio;

        protected bool uiEnabled = true;

        public void ChangeHeat(float value)
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.SetHeatLevel(value);
        }

        public void AddMoleculesR()
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.AddMolecules(moleculesToAddR, 2);

            waterAudio.Play();
        }

        public void AddMoleculesG()
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.AddMolecules(moleculesToAddG, 2);

            waterAudio.Play();
        }

        public void AddMoleculesB()
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.AddMolecules(moleculesToAddB, 2);

            waterAudio.Play();
        }

        public void ResetFlask()
        {
            GameModel.Current.Reset();
            heatSlider.value = GameModel.Current.CurrentHeatLevel;
        }

        private void Start()
        {
            // create targets
            foreach(TargetStruct t in GameModel.Current.Config.targets)
            {
                TargetsRow tr = Instantiate(targetRowPrefab, targetsUIContainer);
                tr.SetTarget(t);
            }
        }

        private void LateUpdate()
        {
            if (GameModel.Current.GlassCracked == uiEnabled)
            {
                uiEnabled = !GameModel.Current.GlassCracked;

                heatSlider.value = GameModel.Current.CurrentHeatLevel;
                addRButton.interactable = uiEnabled;
                addGButton.interactable = uiEnabled;
                addBButton.interactable = uiEnabled;
                heatSlider.interactable = uiEnabled;
            }

            if (!uiEnabled)
            {
                heatSlider.value = GameModel.Current.CurrentHeatLevel;
            }

            if (GameModel.Current.KnownReactionsInvalidated) DrawKnownReactions();
        }

        protected void DrawKnownReactions()
        {
            GameModel.Current.KnownReactionsInvalidated = false;
            foreach (RectTransform child in reactionsUIContainer)
            {
                Destroy(child.gameObject);
            }

            List<ReactionStruct> sorted = new List<ReactionStruct>(GameModel.Current.Config.reactions);
            sorted.Sort((r1, r2) => float.Parse(r1.displayEnergy).CompareTo(float.Parse(r2.displayEnergy)));

            foreach (ReactionStruct rs in sorted)
            {
                KnownReactionRow krr = Instantiate(knownReactionRowPrefab, reactionsUIContainer);
                krr.SetData(rs);
                krr.SetSeen(GameModel.Current.KnownReactions.Contains(rs));
            }

            reactionTitleTxt.text = $"Reactions (Seen {GameModel.Current.KnownReactions.Count}/{GameModel.Current.Config.reactions.Count})";
        }
    }
}