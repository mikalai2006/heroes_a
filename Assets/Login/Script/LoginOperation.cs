using System;
using System.Threading.Tasks;
using AppInfo;
//using Common;
using Cysharp.Threading.Tasks;
//using Extensions;
using UnityEngine;
//using UnityEngine.SceneManagement;
using Loader;

namespace Login
{
    public class LoginOperation : ILoadingOperation
    {
        private readonly AppInfoContainer _appInfoContainer;

        private Action<float> _onProgress;
        private Action<string> _onSetNotify;

        public LoginOperation(AppInfoContainer appInfoContainer)
        {
            _appInfoContainer = appInfoContainer;
        }

        public async UniTask Load(Action<float> onProgress, Action<string> onSetNotify)
        {
            _onProgress = onProgress;
            _onSetNotify = onSetNotify;

            _onSetNotify?.Invoke("Login to server...");
            _onProgress?.Invoke(0.1f);
            _appInfoContainer.UserInfo = await GetUserInfo(DeviceInfo.GetDeviceId());

            _onProgress?.Invoke(.2f);
        }

        private async UniTask<UserInfoContainer> GetUserInfo(string deviceId)
        {
            UserInfoContainer result = null;

            //Fake login
            if (PlayerPrefs.HasKey(deviceId))
            {
                result = JsonUtility.FromJson<UserInfoContainer>(PlayerPrefs.GetString(deviceId));
            }
            // await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            _onProgress?.Invoke(0.3f);
            //Fake login

            if (result == null || result.Id == null)
            {
                result = await GameManager.Instance.LoginWindowProvider.ShowAndHide();
            }

            _appInfoContainer.UserInfo = result;

            PlayerPrefs.SetString(deviceId, JsonUtility.ToJson(result));

            return result;
        }
    }
}