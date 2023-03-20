using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ClickableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    //[SerializeField] private Image _img;
    [SerializeField] private Sprite _default;
    [SerializeField] private Color _colorDefault, _colorPress;

    private void Awake()
    {
        //_img = GetComponent<Image>();
        //_img.color = _colorDefault;
    }

    private void Start()
    {
        GameManager.OnBeforeStateChanged += OnChangeGameState;
    }
    private void OnDestroy() => GameManager.OnBeforeStateChanged -= OnChangeGameState;

    public void OnPointerDown(PointerEventData eventData)
    {
        //_img.color = _colorPress;
        //if (GameState == GameState.ChooseTown)
    }

    private void OnChangeGameState(GameState newState)
    {
        if (newState == GameState.ChooseTown)
        {
            //UnitManager.Instance._activeTown;

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //_img.color = _colorDefault;
    }
}
