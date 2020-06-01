using Menus.Lobby;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    private enum NavigationState
    {
        MainMenu,
        Lobby,
    }

    private class MainMenuData : MainMenu.IMenuData
    {
        public event Action<bool> OnAllowJoin;
        private MenuNavigator navigator;

        public MainMenuData(MenuNavigator navigator)
        {
            this.navigator = navigator;
        }

        public void Join(string roomCode)
        {
            OnAllowJoin?.Invoke(false);
            NetworkManager.Instance.Connect(roomCode, new NetworkManager.ConnectionModel()
            {
                onSuccess = () => navigator.state = NavigationState.Lobby,
                onError = (result, message) =>
                {
                    Debug.Log($"An error has occured while connecting to Network with result code: {result}, message: {message}");
                    OnAllowJoin?.Invoke(true);
                }
            });
        }
    }

    private class LobbyMenuData : LobbyMenu.IMenuData
    {
        private MenuNavigator navigator;

        public LobbyMenuData(MenuNavigator navigator)
        {
            this.navigator = navigator;
        }

        public void Leave()
        {
            NetworkManager.Instance.Disconnect(new NetworkManager.ConnectionModel()
            {
                onSuccess = () => navigator.state = NavigationState.MainMenu,
            });
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }

    [SerializeField]
    private NavigationState state = NavigationState.MainMenu;

    private void Awake()
    {
        MenuManager.Instance.OnInitialize();
        OnInitialize();
    }
    private void Update()
    {
        MenuManager.Instance.OnUpdate();
    }

    public void OnInitialize()
    {
        StartCoroutine(NavigationCoroutine());
    }

    private IEnumerator NavigationCoroutine()
    {
        while (true)
        {

            switch (state)
            {
                case NavigationState.MainMenu:
                    MainMenu mainMenu = MenuManager.Instance.CreateMenu(MenuType.MainMenu) as MainMenu;
                    mainMenu.MenuData = new MainMenuData(this);
                    MenuManager.Instance.ShowMenu(mainMenu);
                    yield return new WaitWhile(() => state == NavigationState.MainMenu);
                    MenuManager.Instance.HideMenu(mainMenu);
                    break;

                case NavigationState.Lobby:
                    LobbyMenu lobbyMenu = MenuManager.Instance.CreateMenu(MenuType.LobbyMenu) as LobbyMenu;
                    lobbyMenu.MenuData = new LobbyMenuData(this);
                    MenuManager.Instance.ShowMenu(lobbyMenu);
                    yield return new WaitWhile(() => state == NavigationState.Lobby);
                    MenuManager.Instance.HideMenu(lobbyMenu);
                    break;

                default:
                    break;
            }
        }
    }

}