using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;

public class ArenaHeroMonoBehavior : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected ArenaHeroEntity _arenaHeroEntity;
    public ArenaHeroEntity ArenaEntityHero => _arenaHeroEntity;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;

    private CancellationTokenSource cancelTokenSource;

    #region Unity methods
    protected void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _model = transform.Find("Model");
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        await AudioManager.Instance.Click();

        // Debug.Log($"Click {ArenaEntityHero.ConfigData.name}");

        var dialogHeroInfo = new DialogHeroInfoOperation((EntityHero)ArenaEntityHero.Entity);
        var result = await dialogHeroInfo.ShowAndHide();
        if (result.isOk)
        {

        }
        await UniTask.Delay(1);
    }

    // public async void OnMouseDown()
    // {
    //     Debug.Log($"OnMouseDown {EntityHero.ConfigData.name}");
    //     await UniTask.Delay(1);
    // }
    #endregion

    public void Init(ArenaHeroEntity entityHero)
    {
        _arenaHeroEntity = entityHero;

        if (gameObject.transform.position.x > 10f)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    protected void OnDestroy()
    {
        // ArenaEntity.DestroyGameObject();
    }

    public void UpdateAnimate(Vector3 startPosition, Vector3 endPosition)
    {
        // if (_animator != null && startPosition != Vector3Int.zero && endPosition != Vector3Int.zero)
        // {
        //     if (startPosition.x > endPosition.x)
        //     {
        //         _model.transform.localScale = new Vector3(-1, 1, 1);
        //     }
        //     else
        //     {
        //         _model.transform.localScale = new Vector3(1, 1, 1);
        //     }

        //     Vector3 direction = endPosition - startPosition;
        //     //Debug.Log($"Animator change::: {direction}");
        //     _animator.SetFloat("X", (float)direction.x);
        //     _animator.SetFloat("Y", (float)direction.y);

        //     _animator.SetBool("isWalking", true);
        // }
    }

}
