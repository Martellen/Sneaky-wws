using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField]
    private RectTransform menuParent = null;
    [SerializeField]
    private List<MonoBehaviour> menuPrefabs = new List<MonoBehaviour>();

    private Dictionary<MenuType, IMenu> menuPrefabLookup = new Dictionary<MenuType, IMenu>();
    private List<IMenu> openedMenus = new List<IMenu>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        List<MenuType> existingTypes = new List<MenuType>();
        for (int i = 0; i < menuPrefabs.Count; i++)
        {
            IMenu menu = menuPrefabs[i] as IMenu;
            if (menu == null)
            {
                Debug.LogError($"Supplied component {menuPrefabs[i].name} does not implement IMenu interface.");
                menuPrefabs.RemoveAt(i);
                i -= 1;
            }
            else if (menu.MenuType == MenuType.None)
            {
                Debug.LogError($"Supplied IMenu {menuPrefabs[i].name} has MenuType {menu.MenuType} which is not allowed.");
                menuPrefabs.RemoveAt(i);
                i -= 1;
            }
            else if (existingTypes.Contains(menu.MenuType) == true)
            {
                Debug.LogError($"Supplied IMenu {menuPrefabs[i].name} with MenuType {menu.MenuType} already exists on the list.");
                menuPrefabs.RemoveAt(i);
                i -= 1;
            }
            else
            {
                existingTypes.Add(menu.MenuType);
            }
        }
    }
#endif

    public void OnInitialize()
    {
        foreach (MonoBehaviour prefabs in menuPrefabs)
        {
            IMenu menu = prefabs as IMenu;
            this.menuPrefabLookup.Add(menu.MenuType, menu);
        }
    }

    public IMenu CreateMenu(MenuType menuType)
    {
        IMenu menu = Instantiate(menuPrefabLookup[menuType] as MonoBehaviour, menuParent, false) as IMenu;
        menu.OnInitialize();
        return menu;
    }
    public void ShowMenu(IMenu menu)
    {
        if (openedMenus.Contains(menu) == false)
        {
            openedMenus.Add(menu);
            menu.OnShow();
        }
        else
        {
            Debug.Log($"Menu already visible.");
        }
    }

    public void HideMenu(IMenu menu)
    {
        if (openedMenus.Remove(menu) == true)
        {
            menu.OnHide();
        }
        else
        {
            Debug.Log($"Menu already hidden.");
        }
    }

    public bool IsMenuShown(IMenu menu)
    {
        return openedMenus.Contains(menu);
    }

    public void OnUpdate()
    {
        List<IMenu> menus = new List<IMenu>(openedMenus);
        foreach (IMenu menu in menus)
        {
            menu.OnUpdate();
        }
    }


}


//using Photon.Realtime;
//using Sirenix.OdinInspector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public class MenuManager : Singleton<MenuManager>
//{
//    [Serializable]
//    private class MenuDictionary : SerializedDictionary<MenuType, Menu> { }

//    private class VisibilityUpdateModel
//    {
//        public Menu menu;
//        public float time;
//        public bool isHiding;
//    }

//    [SerializeField]
//    private MenuType initialMenuType = MenuType.MainMenu;
//    [SerializeField]
//    [InfoBox("One or more menus have a different key in dictionary than the Menu.MenuType property.", "IsBadMenuInDictionart_Impl", InfoMessageType = InfoMessageType.Error)]
//    private MenuDictionary menus = null;

//    private Stack<Menu> menuStack = new Stack<Menu>();
//    private Dictionary<MenuType, VisibilityUpdateModel> visibilityDictionary = new Dictionary<MenuType, VisibilityUpdateModel>();

//#if UNITY_EDITOR
//    private bool IsBadMenuInDictionart_Impl
//    {
//        get
//        {
//            foreach (KeyValuePair<MenuType, Menu> model in menus)
//            {
//                if (model.Key != model.Value.MenuType)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//    }
//#endif
//    private void Awake()
//    {
//        Initialize();
//    }
//    private void Update()
//    {
//        OnUpdate(Time.deltaTime);
//    }

//    public void Initialize()
//    {
//        foreach (KeyValuePair<MenuType, Menu> menu in menus)
//        {
//            menu.Value.OnInitalize();
//            menu.Value.gameObject.SetActive(false);
//        }
//        ShowMenu(initialMenuType);
//    }

//    public void ShowMenu(MenuType menuType)
//    {
//        if (menus.TryGetValue(menuType, out Menu currentMenu) == false)
//        {
//            Debug.Log($"Menu with type {menuType} not found.");
//            return;
//        }

//        if (menuStack.Count > 0)
//        {
//            Menu previousMenu = menuStack.Peek();
//            HideMenu(previousMenu);
//        }

//        menuStack.Push(currentMenu);
//        ShowMenu(currentMenu);

//    }

//    public void HideTopMenu()
//    {
//        if (menuStack.Count > 0)
//        {
//            Menu previousMenu = menuStack.Pop();
//            HideMenu(previousMenu);

//            if (menuStack.Count > 0)
//            {
//                Menu currentMenu = menuStack.Peek();
//                ShowMenu(currentMenu);
//            }
//        }
//        else
//        {
//            Debug.Log($"Trying to hide the top menu without any menu being shown.");
//        }
//    }

//    private void HideMenu(Menu menu)
//    {
//        if (visibilityDictionary.TryGetValue(menu.MenuType, out VisibilityUpdateModel model) == false)
//        {
//            visibilityDictionary.Add(menu.MenuType, new VisibilityUpdateModel()
//            {
//                isHiding = true,
//                menu = menu,
//            });
//        }
//        else
//        {
//            if (model.isHiding == false)
//            {
//                menu.OnShowEnd();
//                menu.OnHideBegin();
//            }
//            model.isHiding = true;
//        }
//    }

//    private void ShowMenu(Menu menu)
//    {
//        menu.gameObject.SetActive(true);
//        if (visibilityDictionary.TryGetValue(menu.MenuType, out VisibilityUpdateModel model) == false)
//        {
//            visibilityDictionary[menu.MenuType] = new VisibilityUpdateModel()
//            {
//                isHiding = false,
//                menu = menu,
//            };
//        }
//        else
//        {
//            if (model.isHiding == true)
//            {
//                menu.OnHideEnd();
//                menu.OnShowBegin();
//            }
//            model.isHiding = false;
//        }
//    }

//    public void OnUpdate(float time)
//    {
//        List<MenuType> keysToRemove = null;
//        foreach (KeyValuePair<MenuType, VisibilityUpdateModel> model in visibilityDictionary)
//        {
//            if (model.Value.time == 0)
//            {
//                if (model.Value.isHiding == true)
//                {
//                    model.Value.menu.OnHideBegin();
//                }
//                else
//                {
//                    model.Value.menu.OnShowBegin();
//                }
//            }

//            model.Value.time += time;
//            float targetTime = model.Value.isHiding == true ? model.Value.menu.HideDuration : model.Value.menu.ShowDuration;

//            if (model.Value.time >= targetTime)
//            {
//                model.Value.time = targetTime;
//                if (model.Value.isHiding == true)
//                {
//                    model.Value.menu.OnHidePerform(model.Value.time, 1);
//                    model.Value.menu.OnHideEnd();
//                    model.Value.menu.gameObject.SetActive(false);
//                }
//                else
//                {
//                    model.Value.menu.OnShowPerform(model.Value.time, 1);
//                    model.Value.menu.OnShowEnd();
//                }
//                keysToRemove = keysToRemove ?? new List<MenuType>();
//                keysToRemove.Add(model.Key);
//            }
//            else
//            {
//                if (model.Value.isHiding == true)
//                {
//                    model.Value.menu.OnHidePerform(model.Value.time, model.Value.time / targetTime);
//                }
//                else
//                {
//                    model.Value.menu.OnShowPerform(model.Value.time, model.Value.time / targetTime);
//                }
//            }
//        }

//        if (keysToRemove != null)
//        {
//            foreach (MenuType type in keysToRemove)
//            {
//                visibilityDictionary.Remove(type);
//            }
//        }

//        if (menuStack.Count > 0)
//        {
//            Menu menu = menuStack.Peek();
//            menu.OnUpdate();
//        }
//    }


//}