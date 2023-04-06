using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

//[System.Serializable]
public class MapEntityHero : BaseMapEntity
{
    private bool _canMove = false;
    public static event Action<EntityHero> onChangeParamsActiveHero;

    [NonSerialized] public Property<float> hit;
    [NonSerialized] public Property<float> mana;

    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;


    #region Unity methods
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
        _model = transform.Find("Model");

        //GameManager.OnBeforeStateChanged += OnChangeGameState;

    }
    //private void OnDestroy() => GameManager.OnBeforeStateChanged -= OnChangeGameState;

    public override void OnBeforeStateChanged(GameState newState)
    {
        base.OnBeforeStateChanged(newState);
        OnChangeGameState(newState);
    }
    public override void OnAfterStateChanged(GameState newState)
    {
        base.OnAfterStateChanged(newState);
    }

    //protected override void Start()
    //{
    //    base.Start();
    //    // Example usage of a static system
    //    // AudioSystem.Instance.PlaySound(_someSound);
    //}

    #endregion

    public override void InitUnit(BaseEntity mapObject)
    {

        base.InitUnit(mapObject);
        MapObjectClass = (EntityHero)mapObject;

        hit = new Property<float>(100f);
        mana = new Property<float>(100f);
        hit.Value = 100f;
        // Data.hit = 100f;
        // Data.speed = 100;
        // Data.name = MapObjectClass.ScriptableData.name;

        if (mapObject.Player != null)
        {
            SetPlayer(mapObject.Player);
        }
    }


    public void SetPlayer(Player player)
    {
        Transform flag = transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = player.DataPlayer.color;
    }



    //private Action<GameState> OnChangeGameState()
    //{
    //    throw new NotImplementedException();
    //}


    private void OnChangeGameState(GameState newState)
    {
        var entityHero = (EntityHero)MapObjectClass;
        _canMove = newState == GameState.StartMoveHero && this.MapObjectClass ==
            LevelManager.Instance.ActivePlayer.DataPlayer.PlayerDataReferences.ActiveHero;
        if (_canMove && entityHero.CanMove)
        {
            entityHero.Data.path.RemoveAt(0);
            StartCoroutine(MoveHero());
        }

        switch (newState)
        {
            case GameState.StepNextPlayer:
                entityHero.Data.hit = 100f;
                hit.Value = 100f;
                mana.Value = UnityEngine.Random.value * 100f;
                break;
        }
    }

    IEnumerator MoveHero()
    {
        var heroEntity = (EntityHero)MapObjectClass;
        while (heroEntity.Data.path.Count > 0 && _canMove && heroEntity.Data.hit >= 1)
        {
            //transform.position = HeroData.path[0];
            Vector3 moveKoof = heroEntity.Data.path[0].OccupiedUnit?.ScriptableData.typeInput == TypeInput.Down ? new Vector3(.5f, .0f) : new Vector3(.5f, .5f);

            UpdateAnimate(MapObjectClass.Position, heroEntity.Data.path[0].position);
            _animator.SetBool("isWalking", true);

            yield return StartCoroutine(
                SmoothLerp((Vector3)MapObjectClass.Position + moveKoof, (Vector3)heroEntity.Data.path[0].position + moveKoof));
            //ChangeHit(HeroData.path[0], - 1);
            heroEntity.Data.hit -= heroEntity.CalculateHitByNode(heroEntity.Data.path[0]);
            hit.Value -= heroEntity.CalculateHitByNode(heroEntity.Data.path[0]);
            MapObjectClass.Position = heroEntity.Data.path[0].position;
            //Position = path[0]._position;
            GameManager.Instance.MapManager.DrawCursor(heroEntity.Data.path, heroEntity);

            List<GridTileNode> noskyNode = GameManager.Instance.MapManager.DrawSky(heroEntity.Data.path[0], 4);
            heroEntity.Player.SetNosky(noskyNode);

            GameManager.Instance.MapManager.SetColorForTile(heroEntity.Data.path[0].position, Color.cyan);
            if (heroEntity.Data.path[0].Protected && heroEntity.Data.path[0].ProtectedUnit != null)
            {
                heroEntity.Data.path[0].ProtectedUnit.MapObjectGameObject.OnGoHero(LevelManager.Instance.GetPlayer(heroEntity.Data.idPlayer));
                // path[0].SetProtectedNeigbours(null);
                GameManager.Instance.ChangeState(GameState.StopMoveHero);
            }
            if (heroEntity.Data.path[0].OccupiedUnit != null)
            {
                heroEntity.Data.path[0].OccupiedUnit.MapObjectGameObject.OnGoHero(LevelManager.Instance.GetPlayer(heroEntity.Data.idPlayer));
            }

            onChangeParamsActiveHero?.Invoke(heroEntity);
            // GameManager.Instance.ChangeState(GameState.ChangeHeroParams);
            heroEntity.Data.path.RemoveAt(0);


            //yield return new WaitForSeconds(.1f);
        }
        GameManager.Instance.ChangeState(GameState.StopMoveHero);
        _animator.SetBool("isWalking", false);
    }

    IEnumerator SmoothLerp(Vector3 startPosition, Vector3 endPosition)
    {
        float time = .4f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            Camera.main.transform.position = Vector3.Lerp(
                startPosition + new Vector3(0, 0, -10),
                endPosition + new Vector3(0, 0, -10),
                (elapsedTime / time));
            //new Vector3((float)startPosition.x, (float)endPosition.y, -10f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateAnimate(Vector3Int startPosition, Vector3Int endPosition)
    {
        if (_animator != null && startPosition != Vector3Int.zero && endPosition != Vector3Int.zero)
        {
            if (startPosition.x > endPosition.x)
            {
                _model.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                _model.transform.localScale = new Vector3(1, 1, 1);
            }

            Vector3Int direction = endPosition - startPosition;
            //Debug.Log($"Animator change::: {direction}");
            _animator.SetFloat("X", (float)direction.x);
            _animator.SetFloat("Y", (float)direction.y);

            _animator.SetBool("isWalking", true);
        }
    }

    public void ChangeHit(GridTileNode node, float _hit)
    {
        var data = (EntityHero)MapObjectClass;
        data.Data.hit += _hit * data.Data.speed;
        hit.Value += data.Data.hit;
    }
    private void OnMouseDown()
    {
        // Only allow interaction when it's the hero turn
        //if (GameManager.Instance.State != GameState.HeroTurn) return;

        //// Don't move if we've already moved
        //if (!_canMove) return;

        // Show movement/attack options

        // Eventually either deselect or ExecuteMove(). You could split ExecuteMove into multiple functions
        // like Move() / Attack() / Dance()

        //Debug.Log("Unit clicked");

        if (LevelManager.Instance.ActivePlayer.DataPlayer.PlayerDataReferences.ListHero.Contains((EntityHero)this.GetMapObjectClass))
        {
            //LevelManager.Instance.GetActivePlayer().SetActiveHero(this);
            EntityHero entityHero = (EntityHero)MapObjectClass;
            entityHero.SetHeroAsActive();
            Debug.Log($"Check My hero {name}");
        }
        else
        {
            Debug.Log($"Check Enemy hero {name}");
        }
    }

    //public virtual void ExecuteMove() {
    //    // Override this to do some hero-specific logic, then call this base method to clean up the turn

    //    //_canMove = false;
    //}

    //public override void OnSaveUnit()
    //{
    //    SaveUnit(Data);
    //}
    //public override void OnLoadUnit(SaveDataUnit<DataHero> saveData)
    //{
    //    Data = saveData.data;
    //    LoadUnit(saveData);
    //}

    // public void LoadDataPlay(DataPlay data)
    // {
    //     //throw new System.NotImplementedException();
    // }

    // public void SaveDataPlay(ref DataPlay data)
    // {
    //     var sdata = SaveUnit(Data);
    //     data.Units.heroes.Add(sdata);
    // }

    // public override void OnLoadUnit<T>(SaveDataUnit<T> Data)
    // {
    //     base.OnLoadUnit(Data);
    //     DataHero dh = Data.data as DataHero;
    //     this.Data = dh;
    // }

}