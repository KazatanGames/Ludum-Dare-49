namespace KazatanGames.Framework
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Analytics;

    /**
     * Base Panel UI
     * 
     * Kazatan Games Framework - should not require customization per game.
     * 
     * Panel UIs are UI elements that can be shown and hidden. They play an animation and sfx 
     * when showing and hiding.
     */
    public abstract class UIPanel : UIElement
    {
        public static bool PanelShowing = false;
        protected static UIPanel CurrentPanel;

        protected bool isShowing;

        public bool IsShowing { get { return isShowing; } }

        public virtual void Show()
        {
            if (isShowing) return;

            if (PanelShowing)
            {
                if (CurrentPanel == null)
                {
                    PanelShowing = false;
                }
                else
                {
                    CurrentPanel.Hide();
                }
            }

            gameObject.SetActive(true);

            GetComponent<Animator>().SetBool("Show", true);
            isShowing = true;
            PanelShowing = true;
            CurrentPanel = this;

            SfxPlayUIPanelIn();
        }

        public virtual void Hide()
        {
            if (!isShowing) return;
            GetComponent<Animator>().SetBool("Show", false);
            isShowing = false;
            PanelShowing = false;
            CurrentPanel = null;

            SfxPlayUIPanelOut();
        }

        public void Toggle()
        {
            if (isShowing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected abstract bool ShowingAtStart { get; }

        protected virtual string ScreenStringForAnalytics { get { return ""; } }

        protected virtual void Awake()
        {
            if (ShowingAtStart) Show();
        }

        protected void OnDestroy()
        {
            if (!isShowing) return;
            PanelShowing = false;
            CurrentPanel = null;
        }
    }
}