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
        protected Button addYButton;
        [SerializeField]
        protected Button addRButton;
        [SerializeField]
        protected Slider heatSlider;

        [SerializeField]
        protected MoleculeTypeSO moleculesToAddY;
        [SerializeField]
        protected MoleculeTypeSO moleculesToAddR;

        protected bool uiEnabled = true;

        public void ChangeHeat(float value)
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.SetHeatLevel(value);
        }

        public void AddMoleculesY()
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.AddMolecules(moleculesToAddY, Random.Range(4, 8));
            GameModel.Current.KnownReactionsInvalidated = true;
        }
        public void AddMoleculesR()
        {
            if (GameModel.Current.GlassCracked) return;

            GameModel.Current.AddMolecules(moleculesToAddR, Random.Range(4, 8));
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
                tr.SetType(t.type);
            }
        }

        private void LateUpdate()
        {
            if (GameModel.Current.GlassCracked == uiEnabled)
            {
                uiEnabled = !GameModel.Current.GlassCracked;

                heatSlider.value = GameModel.Current.CurrentHeatLevel;
                addYButton.interactable = uiEnabled;
                addRButton.interactable = uiEnabled;
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
            foreach (ReactionStruct rs in GameModel.Current.KnownReactions)
            {
                KnownReactionRow krr = Instantiate(knownReactionRowPrefab, reactionsUIContainer);
                Sprite[] sprites = new Sprite[rs.results.Length + 4];

                sprites[0] = rs.input1.sprite;
                sprites[1] = plusSprite;
                sprites[2] = rs.input2.sprite;
                sprites[3] = equalsSprite;
                int i = 0;
                foreach (MoleculeTypeSO mt in rs.results)
                {
                    sprites[4 + i] = mt.sprite;
                    i++;
                }

                krr.CreateImages(sprites);
            }
        }
    }
}