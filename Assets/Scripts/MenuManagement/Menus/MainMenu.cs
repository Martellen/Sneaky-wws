using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IMenu
{
    public interface IMenuData
    {
        event Action<bool> OnAllowJoin;
        void Join(string roomCode);
    }

    public event Action<IMenu> OnFinishedAnimation;

    [SerializeField]
    private CanvasGroup menuCanvasGroup = null;
    [SerializeField]
    private float animationDuration = 0.3f;
    [Space]
    [SerializeField]
    private Button joinRoomButton = null;
    [SerializeField]
    private CanvasGroup joinRoomButtonCanvasGroup = null;
    [SerializeField]
    private TMP_InputField joinRoomName = null;
    [SerializeField]
    private string allowedSymbols = null;

    private IMenuData menuData;
    private Coroutine animationCoroutine;

    public MenuType MenuType => MenuType.MainMenu;
    public IMenuData MenuData
    {
        get => menuData;
        set
        {
            if (menuData != null)
            {
                menuData.OnAllowJoin -= ConnectionData_OnAllowJoin;
            }
            menuData = value;
            if (menuData != null)
            {
                menuData.OnAllowJoin += ConnectionData_OnAllowJoin;
            }
        }
    }

    public void OnInitialize()
    {
        menuCanvasGroup.alpha = 0;
        joinRoomButton.onClick.AddListener(Join);
        joinRoomName.onValueChanged.AddListener(OnValueChanged);
        joinRoomName.onValidateInput = OnValidateInput;
        OnValueChanged(joinRoomName.text);
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

    private void ConnectionData_OnAllowJoin(bool allow)
    {
        joinRoomButton.interactable = allow;
        joinRoomName.interactable = allow;
    }
    private void Join()
    {
        if (MenuData != null && joinRoomName.text.Length == joinRoomName.characterLimit)
        {
            MenuData.Join(joinRoomName.text);
        }
    }

    private char OnValidateInput(string text, int charIndex, char addedChar)
    {
        if (allowedSymbols.Contains($"{addedChar}") == true)
        {
            return char.ToUpper(addedChar);
        }
        return '\0';
    }
    private void OnValueChanged(string arg0)
    {
        if (joinRoomName.text.Length == joinRoomName.characterLimit)
        {
            joinRoomButtonCanvasGroup.interactable = true;
        }
        else
        {
            joinRoomButtonCanvasGroup.interactable = false;
        }
    }

}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class MainMenu : Menu
//{
//    [SerializeField]
//    private Button joinRoomButton = null;
//    [SerializeField]
//    private CanvasGroup joinRoomButtonCanvasGroup = null;
//    [SerializeField]
//    private TMP_InputField joinRoomName = null;
//    [SerializeField]
//    private string allowedSymbols = null;

//    [Space]
//    [SerializeField]
//    private CanvasGroup canvasGroup = null;
//    [SerializeField]
//    private float showDuration = 0.2f;
//    [SerializeField]
//    private float hideDuration = 0.2f;


//    public override MenuType MenuType => MenuType.MainMenu;
//    public override float ShowDuration => showDuration;
//    public override float HideDuration => hideDuration;

//    private void JoinRoom()
//    {
//        joinRoomButton.interactable = false;
//        joinRoomName.interactable = false;
//        NetworkManager.Initialize();
//        NetworkManager.Connection.OnConnectedToServer += NetworkConnection_OnConnectedToServer;
//    }

//    private void NetworkConnection_OnConnectedToServer()
//    {
//        NetworkManager.Connection.OnConnectedToServer -= NetworkConnection_OnConnectedToServer;
//        NetworkManager.Connection.OnConnectedToRoom += OnJoinedRoom;
//        NetworkManager.Connection.OnConnectedToRoomFailed += OnJoinRoomFailed;
//        string roomName = joinRoomName.text;
//        NetworkManager.Connect(roomName);
//    }

//    public void OnJoinedRoom()
//    {
//        NetworkManager.Connection.OnConnectedToRoom -= OnJoinedRoom;
//        NetworkManager.Connection.OnConnectedToRoomFailed -= OnJoinRoomFailed;
//        Debug.Log($"Connected");
//        MenuManager.Instance.ShowMenu(MenuType.LobbyMenu);
//    }

//    public void OnJoinRoomFailed()
//    {
//        NetworkManager.Connection.OnConnectedToRoom -= OnJoinedRoom;
//        NetworkManager.Connection.OnConnectedToRoomFailed -= OnJoinRoomFailed;
//        Debug.Log($"Connection Failed");
//        joinRoomButton.interactable = true;
//        joinRoomName.interactable = true;
//    }

//    private char OnValidateInput(string text, int charIndex, char addedChar)
//    {
//        if (allowedSymbols.Contains($"{addedChar}") == true)
//        {
//            return char.ToUpper(addedChar);
//        }
//        return '\0';
//    }
//    private void OnValueChanged(string arg0)
//    {
//        if (joinRoomName.text.Length == joinRoomName.characterLimit)
//        {
//            joinRoomButtonCanvasGroup.interactable = true;
//        }
//        else
//        {
//            joinRoomButtonCanvasGroup.interactable = false;
//        }
//    }


//    public override void OnInitalize()
//    {
//        joinRoomButton.onClick.AddListener(JoinRoom);
//        joinRoomName.onValidateInput = OnValidateInput;
//        joinRoomName.onValueChanged.AddListener(OnValueChanged);
//    }

//    public override void OnShowBegin()
//    {
//        joinRoomButton.interactable = true;
//        joinRoomName.interactable = true;
//        joinRoomButtonCanvasGroup.interactable = false;
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