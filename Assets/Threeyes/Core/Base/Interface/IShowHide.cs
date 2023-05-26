using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShowHide
{
    bool IsShowing { get; set; }
    void Show(bool isShow);
    void Show();
    void Hide();
    void ToggleShow();
}

public interface IShowHideEx : IShowHide
{
    void ShowAtOnce(bool isShow);//如UI开始动画前的重置
}


//模板，直接复制粘贴
//#region Override IShowHideInterface

//public bool IsShowing { get { return isShowing; } set { isShowing = value; } }
//public bool isShowing = false;

//public void Show()
//{
//    Show(true);
//}
//public void Hide()
//{
//    Show(false);
//}
//public void ToggleShow()
//{
//    Show(!IsShowing);
//}
//public void Show(bool isShow)
//{
//IsShowing = isShow;
//    ShowFunc(isShow);
//}
//protected virtual void ShowFunc(bool isShow)
//{
//    gameObject.SetActive(isShow);
//}

//#endregion
