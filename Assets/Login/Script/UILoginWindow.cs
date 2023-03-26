using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using AppInfo;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Text;

public struct DataLogin
{
    public string login;
    public string password;
}
public struct DataResultLogin
{
    public string access_token;
    public string refresh_token;
}

public class UILoginWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument MenuApp => _uiDoc;

    private readonly string _nameFieldLogin = "Login";
    private readonly string _nameFieldPassword = "Password";
    private readonly string _nameButtonLogin = "ButtonLogin";

    private readonly string _nameUseDevice = "ButtonNameDevice";

    private const int MIN_LENGTH_NAME = 3;
    private readonly string _urlLogin = "https://storydata.ru/api/v1/auth";

    private Button _buttonUseDevice;
    private TextField _fieldName;
    private TextField _fieldPassword;
    private Button _buttonLogin;

    private TaskCompletionSource<UserInfoContainer> _loginCompletionSource;


    public UnityEvent loginAction;

    private void Awake()
    {
        _fieldName = MenuApp.rootVisualElement.Q<TextField>(_nameFieldLogin);
        _fieldName.RegisterCallback<InputEvent>(e =>
        {
            OnValidFormField();
        });

        _buttonUseDevice = MenuApp.rootVisualElement.Q<Button>(_nameUseDevice);
        _buttonUseDevice.clickable.clicked += () =>
        {
            LoginAsDeviceId();
        };
        _fieldPassword = MenuApp.rootVisualElement.Q<TextField>(_nameFieldPassword);
        _buttonLogin = MenuApp.rootVisualElement.Q<Button>(_nameButtonLogin);
        _buttonLogin.clickable.clicked += async () =>
        {
            _buttonLogin.SetEnabled(false);
            await OnSimpleLoginClicked();
            _buttonLogin.SetEnabled(true);
        };

        OnValidFormField();
    }

    private void OnValidFormField()
    {
        if (_fieldName.text.Length < MIN_LENGTH_NAME)
        {
            _buttonLogin.SetEnabled(false);
        }
        else
        {
            _buttonLogin.SetEnabled(true);
        }
    }

    public async Task<UserInfoContainer> ProcessLogin()
    {

        _loginCompletionSource = new TaskCompletionSource<UserInfoContainer>();
        return await _loginCompletionSource.Task;
    }

    private void LoginAsDeviceId()
    {
        string deviceId = DeviceInfo.GetDeviceId();
        _loginCompletionSource.SetResult(new UserInfoContainer()
        {
            DeviceId = deviceId
        });

        loginAction?.Invoke();
    }

    private async Task<string> GetUserInfo(string token)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(_urlLogin + "/iam");
        webRequest.SetRequestHeader("Authorization", "Basic " + token);
        webRequest.SetRequestHeader("Content-Type", "application/json");

        webRequest.SendWebRequest();

        while (!webRequest.isDone)
        {
            await Task.Yield();
        }

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending" + webRequest.error);
            return webRequest.error;
        }
        else
        {
            return webRequest.downloadHandler.text;
        }
    }
    private async Task<string> AsyncLogin()
    {
        DataLogin data = new DataLogin()
        {
            login = _fieldName.text,
            password = _fieldPassword.text
        };

        string jsonBody = JsonUtility.ToJson(data);

        byte[] rawBody = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(_urlLogin + "/sign-in", "POST");
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        // request.SetRequestHeader("Authorization", "Basic " + GetAuthenticationKey());
        request.SetRequestHeader("Content-Type", "application/json");

        request.SendWebRequest();

        while (!request.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending" + request.error);
            return request.error;
        }
        else
        {
            return request.downloadHandler.text;
        }
    }


    private async Task OnSimpleLoginClicked()
    {

        if (_fieldName.text.Length < MIN_LENGTH_NAME || _fieldPassword.text == "")
        {
            return;
        }

        UserInfoContainer userInfo = new UserInfoContainer();

        var res = await AsyncLogin();
        var resultObject = JsonUtility.FromJson<DataResultLogin>(res);
        userInfo.UserInfoAuth.RefreshToken = resultObject.refresh_token;
        userInfo.UserInfoAuth.AccessToken = resultObject.access_token;

        if (userInfo.UserInfoAuth.AccessToken == "")
            return;

        var infoUser = await GetUserInfo(resultObject.access_token);
        var infoUserObject = JsonUtility.FromJson<UserInfo>(infoUser);

        _loginCompletionSource.SetResult(userInfo);
        loginAction?.Invoke();
    }

    private void OnFacebookLoginClicked()
    {
        //TODO implement later
    }
}

