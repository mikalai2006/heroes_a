// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UIElements;

// /// <summary>
// /// Game view with events for buttons and showing data.
// /// </summary>
// public class UIGameView : UIView
// {
//     [SerializeField] private UIGameAsideView aside;
//     public UIGameAsideView Aside => aside;

//     [SerializeField] private UIGameSettingMenuView settingMenu;
//     public UIGameSettingMenuView SettingMenu => settingMenu;

//     [SerializeField] private UIGameTownView town;
//     public UIGameTownView Town => town;

//     public void Init()
//     {
//         // Setting menu.
//         SettingMenu.Init();

//         // Aside.
//         Aside.Init();

//         // Town.
//         Town.Init();
//     }




//     //// Reference to time label.
//     //[SerializeField]
//     ////private TextMeshProUGUI timeLabel;

//     //// Event called when Finish Button is clicked.
//     //public UnityAction OnFinishClicked;

//     ///// <summary>
//     ///// Method called by Finish Button.
//     ///// </summary>
//     //public void FinishClick()
//     //{
//     //    OnFinishClicked?.Invoke();
//     //}

//     //// Event called when Menu Button is clicked.
//     //public UnityAction OnMenuClicked;

//     ///// <summary>
//     ///// Method called by Menu Button.
//     ///// </summary>
//     //public void MenuClicked()
//     //{
//     //    OnMenuClicked?.Invoke();
//     //}

//     ///// <summary>
//     ///// Method used to update time label.
//     ///// </summary>
//     ///// <param name="time">Game time.</param>
//     //public void UpdateTime(float time)
//     //{
//     //    //timeLabel.text = string.Format("{0:#00}:{1:00.000}", (int)(time / 60), (time % 60));
//     //}
// }
