using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using AppInfo;
using UnityEngine.Events;

public class UILoginWindow : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDoc;
    public UIDocument MenuApp => _uiDoc;

    private readonly string _nameFieldLogin = "Login";
    private readonly string _nameFieldPassword = "Password";
    private readonly string _nameButtonLogin = "ButtonLogin";

    private TextField _fieldName;
    private TextField _fieldPassword;
    private Button _buttonLogin;

    private TaskCompletionSource<UserInfoContainer> _loginCompletionSource;

    private const int MIN_LENGTH_NAME = 3;

    public UnityEvent loginAction;

    private void Awake()
    {
        _fieldName = MenuApp.rootVisualElement.Q<TextField>(_nameFieldLogin);
        _fieldName.RegisterCallback<InputEvent>(e =>
        {
            OnValidFormField();
        });

        _fieldPassword = MenuApp.rootVisualElement.Q<TextField>(_nameFieldPassword);
        _buttonLogin = MenuApp.rootVisualElement.Q<Button>(_nameButtonLogin);
        _buttonLogin.clickable.clicked += OnSimpleLoginClicked;

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

    private void OnSimpleLoginClicked()
    {
        if (_fieldName.text.Length < MIN_LENGTH_NAME)
            return;
        _loginCompletionSource.SetResult(new UserInfoContainer()
        {
            Name = _fieldName.text
        });

        loginAction?.Invoke();
    }

    private void OnFacebookLoginClicked()
    {
        //TODO implement later
    }
}

