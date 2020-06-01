using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Lobby
{
    public class LobbyMenu : MonoBehaviour, IMenu
    {
        public interface IMenuData
        {
            void Leave();
            void Start();
        }

        public event Action<IMenu> OnFinishedAnimation;

        [SerializeField]
        private CanvasGroup menuCanvasGroup = null;
        [SerializeField]
        private float animationDuration = 0.3f;
        [SerializeField]
        private Button startButton = null;
        [SerializeField]
        private Button leaveButton = null;
        [SerializeField]
        private VerticalLayoutGroup playerListParent = null;
        [SerializeField]
        private PlayerEntry playerEntryPrefab = null;

        private List<PlayerEntry> playerEntries = new List<PlayerEntry>();
        private Coroutine animationCoroutine;

        public MenuType MenuType => MenuType.LobbyMenu;
        public IMenuData MenuData { get; set; }


        public void OnInitialize()
        {
            menuCanvasGroup.alpha = 0;
            leaveButton.onClick.AddListener(LeaveLobby);
        }

        public void OnUpdate()
        {
            List<IPanel> entries = new List<IPanel>(playerEntries);
            foreach (IPanel panel in entries)
            {
                panel.OnUpdate();
            }
        }
        public void OnShow()
        {
            foreach (Player player in NetworkManager.Instance.Players)
            {
                AddPlayerEntry(player);
            }
            this.TryStopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimationCoroutine(1));
        }
        public void OnHide()
        {
            while (playerEntries.Count > 0)
            {
                RemovePlayerEntry(playerEntries[playerEntries.Count - 1].Player);
            }
            this.TryStopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimationCoroutine(0));
        }

        private IEnumerator AnimationCoroutine(float targetAlpha)
        {
            menuCanvasGroup.blocksRaycasts = targetAlpha == 1;
            while (menuCanvasGroup.alpha != targetAlpha)
            {
                menuCanvasGroup.alpha = Mathf.MoveTowards(menuCanvasGroup.alpha, targetAlpha, Time.deltaTime / animationDuration);
                yield return null;
            }
            OnFinishedAnimation?.Invoke(this);
        }


        private PlayerEntry AddPlayerEntry(Player player)
        {
            PlayerEntry entry = Instantiate(playerEntryPrefab, playerListParent.transform, false);
            playerEntries.Add(entry);
            entry.Player = player;
            entry.OnInitialize();
            entry.OnShow();
            return entry;
        }

        private bool RemovePlayerEntry(Player player)
        {
            int index = playerEntries.FindIndex(x => x.Player == player);
            if (index != -1)
            {
                PlayerEntry playerEntry = playerEntries[index];
                playerEntry.transform.SetParent(null, true);
                playerEntry.OnHide();
                playerEntry.onAnimationFinished += () => Destroy(playerEntry.gameObject);
                playerEntries.RemoveAt(index);
                return true;
            }
            return false;
        }

        private void LeaveLobby()
        {
            if (MenuData != null)
            {
                Debug.Log($"Leaving room.");
                MenuData.Leave();
            }
            else
            {
                Debug.LogError($"MenuData not set.");
            }
        }


    }
}


//using Photon.Realtime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;

//public class LobbyMenu : Menu
//{
//    [SerializeField]
//    private Button disconnectButton = null;

//    [Space]
//    [SerializeField]
//    private CanvasGroup canvasGroup = null;
//    [SerializeField]
//    private float showDuration = 0.2f;
//    [SerializeField]
//    private float hideDuration = 0.2f;

//    public override MenuType MenuType => MenuType.LobbyMenu;
//    public override float ShowDuration => showDuration;
//    public override float HideDuration => hideDuration;


//    private void AddPlayer(Player player)
//    {
//        Debug.Log($"Player {player.NickName} joined");
//    }
//    private void ReturnToMainMenu()
//    {
//        MenuManager.Instance.HideTopMenu();
//    }


//    public override void OnInitalize()
//    {
//        disconnectButton.onClick.AddListener(NetworkManager.Disconnect);
//    }

//    public override void OnShowBegin()
//    {
//        NetworkManager.Connection.OnPlayerJoined += AddPlayer;
//        NetworkManager.Connection.OnDisconnectedFromServer += ReturnToMainMenu;
//        foreach (Player player in NetworkManager.Connection.Players)
//        {
//            AddPlayer(player);
//        }
//        canvasGroup.blocksRaycasts = true;
//        canvasGroup.alpha = 0;
//    }

//    public override void OnShowPerform(float time, float normalizedTime)
//    {
//        canvasGroup.alpha = normalizedTime;
//    }

//    public override void OnShowEnd()
//    {
//        canvasGroup.alpha = 1;
//    }

//    public override void OnHideBegin()
//    {
//        canvasGroup.blocksRaycasts = false;
//        canvasGroup.alpha = 1;
//    }

//    public override void OnHidePerform(float time, float normalizedTime)
//    {
//        canvasGroup.alpha = 1 - normalizedTime;
//    }

//    public override void OnHideEnd()
//    {
//        canvasGroup.alpha = 0;
//    }

//}
