using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Menus.Lobby
{
    public class PlayerEntry : MonoBehaviour, IPanel
    {
        public event Action onAnimationFinished;

        [SerializeField]
        private CanvasGroup canvasGroup = null;
        [SerializeField]
        private TMP_Text playerName = null;

        [Space]
        [SerializeField]
        private AnimationCurve showCurve = null;
        [SerializeField]
        private RectTransform movedTransform = null;
        [SerializeField]
        private RectTransform hiddenPosition = null;
        [SerializeField]
        private RectTransform shownPosition = null;


        private Coroutine animationCoroutine = null;
        private Player player;

        public Player Player
        {
            get => player;
            set
            {
                if (value != null)
                {
                    player = value;
                    playerName.text = value.NickName;
                }
            }
        }

        public void OnInitialize()
        {
            canvasGroup.alpha = 0;
        }
        public void OnUpdate()
        {

        }
        public void OnShow()
        {
            this.TryStopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimationCoroutine(1));
        }
        public void OnHide()
        {
            this.TryStopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimationCoroutine(0));
        }

        private IEnumerator AnimationCoroutine(float alpha)
        {
            while (canvasGroup.alpha != alpha)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alpha, Time.deltaTime);
                movedTransform.localPosition = Vector3.Lerp(shownPosition.localPosition, hiddenPosition.localPosition, canvasGroup.alpha);
                yield return null;
            }
            onAnimationFinished?.Invoke();
        }

    }
}
