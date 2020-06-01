using System;

public interface IMenu : IPanel
{
    event Action<IMenu> OnFinishedAnimation;

    MenuType MenuType { get; }

}

//using UnityEngine;

//public abstract class Menu : MonoBehaviour
//{
//    public abstract MenuType MenuType { get; }
//    public abstract float ShowDuration { get; }
//    public abstract float HideDuration { get; }


//    public virtual void OnInitalize() { }
//    public virtual void OnShowBegin() { }
//    public virtual void OnShowPerform(float time, float normalizedTime) { }
//    public virtual void OnShowEnd() { }
//    public virtual void OnHideBegin() { }
//    public virtual void OnHidePerform(float time, float normalizedTime) { }
//    public virtual void OnHideEnd() { }
//    public virtual void OnUpdate() { }

//}