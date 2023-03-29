using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UIElements;


public class UITown : MonoBehaviour
{
    [SerializeField] public UnityAction OnHideSetting;
    [SerializeField] public UnityAction OnSave;
    [SerializeField] private UIDocument _uiDoc;
    private VisualElement _box;
    private const string _nameBox = "Town";
    private const string _nameButtonClose = "ButtonClose";
    private SceneInstance _townScene;

    [SerializeField] private Canvas _bgCanvas;

    public void Init(SceneInstance townScene)
    {

        _townScene = townScene;

        _box = _uiDoc.rootVisualElement.Q<VisualElement>(_nameBox);

        var btnClose = _box.Q<Button>(_nameButtonClose);
        btnClose.clickable.clicked += OnClickClose;

        _bgCanvas.worldCamera = Camera.main;
        Camera.main.transform.position = new Vector3(-20, 0, -10);
    }

    private void OnClickClose()
    {
        Debug.Log("Click close");
        GameManager.Instance.AssetProvider.UnloadAdditiveScene(_townScene);
    }
}

