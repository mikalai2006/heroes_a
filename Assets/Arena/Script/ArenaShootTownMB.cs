using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class ArenaShootTownMB : MonoBehaviour
{
    [SerializeField] protected ArenaShootTown _arenaEntity;
    public ArenaShootTown ArenaEntity => _arenaEntity;
    [NonSerialized] private Animator _animator;
    [SerializeField] public GameObject ShootPrefab;
    private GameObject _shoot;

    private string _nameCreature;

    private CancellationTokenSource cancelTokenSource;
    private Vector2 _pos;

    #region Unity methods
    public void Awake()
    {
        if (ShootPrefab != null)
        {
            _shoot = GameObject.Instantiate(ShootPrefab, gameObject.transform.position, Quaternion.identity, transform);
            _shoot.SetActive(false);
        }

        _animator = GetComponentInChildren<Animator>();
        transform.Find("Quantity").gameObject.SetActive(false);
    }

    protected void OnDestroy()
    {
        ((ScriptableAttributeCreature)ArenaEntity.Entity.ScriptableDataAttribute).ArenaModel.ReleaseInstance(gameObject);
        if (_shoot != null) Destroy(_shoot);
    }
    #endregion

    public void Init(ArenaShootTown arenaEntity)
    {
        _arenaEntity = arenaEntity;
        // _collider.layerOverridePriority = 11 - _arenaEntity.OccupiedNode.position.y;
        // gameObject.transform.localPosition = new Vector3(
        //     gameObject.transform.position.x,
        //     gameObject.transform.position.y,
        //     0);

        var splitName = ArenaEntity.Entity.ScriptableDataAttribute.name.Split('_');
        _nameCreature = splitName.Length > 1 ? splitName[1] : splitName[0];
        gameObject.transform.localScale = new Vector3(-1, 1, 1);
        // gameObject.transform.localPosition = Vector3.zero;
    }

    internal async UniTask RunAttackShoot(GridArenaNode nodeForAttack, Transform positionGameObejct = null)
    {
        string nameAnimAttack = "ShootStraight";
        Vector3 positionToAttack = nodeForAttack != null ? nodeForAttack.center : positionGameObejct.position;
        Vector3 difPos = _arenaEntity.OccupiedNode.center - positionToAttack;

        if (difPos.x > 0 && difPos.y == 0)
        {
            nameAnimAttack = "ShootStraight";
        }
        else if (difPos.x < 0 && difPos.y == 0)
        {
            nameAnimAttack = "ShootStraight";
        }
        else if (difPos.x != 0 && difPos.y > 0)
        {
            nameAnimAttack = "ShootDown";
        }
        else if (difPos.x != 0 && difPos.y < 0)
        {
            nameAnimAttack = "ShootUp";
        }

        string nameAnimationAttack = string.Format("{0}{1}", _nameCreature, nameAnimAttack);
        Debug.Log($"Shoot {nameAnimationAttack}");

        _animator.Play(nameAnimationAttack, 0, 0f);

        _shoot.SetActive(true);
        await SmoothLerpShoot(transform.position, positionToAttack, LevelManager.Instance.ConfigGameSettings.speedArenaAnimation * 2);
        _shoot.SetActive(false);

        await UniTask.Delay(200);

        if (nodeForAttack != null) await nodeForAttack.OccupiedUnit.RunGettingHit(_arenaEntity.OccupiedNode);

        _animator.Play(string.Format("{0}{1}", _nameCreature, "Idle"), 0, 0f);
    }

    private async UniTask SmoothLerpShoot(Vector3 startPosition, Vector3 endPosition, float time)
    {

        Vector3 difPos = startPosition - endPosition;

        if (difPos.x > 0)
        {
            _shoot.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (difPos.x < 0)
        {
            _shoot.transform.localScale = new Vector3(1, 1, 1);
        }

        startPosition = startPosition + new Vector3(0, 1, 0);
        endPosition = endPosition + new Vector3(0, 1, 0);
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            _shoot.transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
    }
}
