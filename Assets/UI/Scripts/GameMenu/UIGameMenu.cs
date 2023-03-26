using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class UIGameMenu : MonoBehaviour
{
    [SerializeField] public UnityAction OnHideSetting;
    [SerializeField] public UnityAction OnSave;

    [SerializeField] private UIDocument _menuDoc;

    private VisualElement _box;

    private const string _nameBox = "GameMenu";
    private const string _nameButtonClose = "ButtonClose";
    private const string _nameButtonSave = "ButtonSave";
    private const string _nameButtonMenuApp = "ButtonMenuApp";

    private DataResultGameMenu _dataResultGameMenu;
    private TaskCompletionSource<DataResultGameMenu> _processCompletionSource;

    public void Init()
    {
        _box = _menuDoc.rootVisualElement.Q<VisualElement>(_nameBox);

        var btnMenuApp = _box.Q<Button>(_nameButtonMenuApp);
        btnMenuApp.clickable.clicked += () =>
        {
            OnClickOk();
            // GameManager.Instance.ChangeState(GameState.StartApp);
        };

        var btnClose = _box.Q<Button>(_nameButtonClose);
        btnClose.clickable.clicked += OnClickClose;

        var btnSave = _box.Q<Button>(_nameButtonSave);
        btnSave.clickable.clicked += () =>
        {
            OnSave?.Invoke();
        };

        // Hide();
    }

    // public void Show()
    // {
    //     _box.style.display = DisplayStyle.Flex;
    // }

    // public void Hide()
    // {
    //     _box.style.display = DisplayStyle.None;
    // }

    private void OnClickOk()
    {
        Debug.Log("Click ok");
        _dataResultGameMenu.isOk = true;
        _processCompletionSource.SetResult(_dataResultGameMenu);

    }
    private void OnClickClose()
    {
        Debug.Log("Click close");
        _dataResultGameMenu.isOk = false;
        _processCompletionSource.SetResult(_dataResultGameMenu);

    }


    public async Task<DataResultGameMenu> ProcessAction()
    {
        _dataResultGameMenu = new DataResultGameMenu();

        _processCompletionSource = new TaskCompletionSource<DataResultGameMenu>();

        return await _processCompletionSource.Task;
    }

}

