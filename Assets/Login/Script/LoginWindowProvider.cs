using System.Threading.Tasks;
using AppInfo;
using Assets;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Login
{
    public class LoginWindowProvider : LocalAssetLoader
    {
        public async UniTask<UserInfoContainer> ShowAndHide()
        {
            var loginWindow = await Load();
            var result = await loginWindow.ProcessLogin();
            Unload();
            return result;
        }

        public UniTask<UILoginWindow> Load()
        {
            return LoadInternal<UILoginWindow>("UILogin");
        }

        public void Unload()
        {
            UnloadInternal();
        }
    }
}