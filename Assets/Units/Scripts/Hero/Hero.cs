using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class DataHero
{
    public int idPlayer;
    public Vector3Int nextPosition;
    public float speed;
    public float hit;
    public float mana;
    public string name;

}

//[System.Serializable]
public class Hero : UnitBase, IDataPlay
{

    [SerializeField] public DataHero Data = new DataHero();

    private bool _canMove = false;
    private Player _player;

    [NonSerialized] public Property<float> hit;
    [NonSerialized] public Property<float> mana;

    [NonSerialized] private Animator _animator;
    [NonSerialized] private Transform _model;
    [NonSerialized] public List<GridTileNode> path;

    public bool CanMove { get { return path.Count > 0 && Data.hit > 0; } private set { } }

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

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {

        base.InitUnit(data, pos);
        path = new List<GridTileNode>();

        hit = new Property<float>(100f);
        mana = new Property<float>(100f);
        hit.Value = 100f;
        Data.hit = 100f;
        Data.speed = 100;
        Data.name = data.name;
    }


    public void SetPlayer(PlayerData playerData)
    {
        //Debug.Log($"SetPlayer::: id{playerData.id}-idArea{playerData.idArea}");
        Data.idPlayer = playerData.id;

        _player = LevelManager.Instance.GetPlayer(Data.idPlayer);
        Transform flag = transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = _player.DataPlayer.color;
    }



    //private Action<GameState> OnChangeGameState()
    //{
    //    throw new NotImplementedException();
    //}


    private void OnChangeGameState(GameState newState)
    {
        _canMove = newState == GameState.StartMoveHero && this ==
            LevelManager.Instance.ActivePlayer.DataPlayer.PlayerDataReferences.ActiveHero;
        if (_canMove && CanMove)
        {
            path.RemoveAt(0);
            StartCoroutine(MoveHero());
        }

        switch (newState)
        {
            case GameState.StepNextPlayer:
                Data.hit = 100f;
                hit.Value = 100f;
                mana.Value = UnityEngine.Random.value * 100f;
                break;
        }
    }

    public void SetPathHero(List<GridTileNode> _path)
    {
        path = _path;
        //for (int i = 1; i < path.Count; i++)
        //{
        //    HeroData.path.Add(path[i]._position);

        //}
        Data.nextPosition = path[path.Count - 1].position;
    }

    public void SetHeroAsActive()
    {
        LevelManager.Instance.ActivePlayer.ActiveHero = this;
    }

    IEnumerator MoveHero()
    {
        while (path.Count > 0 && _canMove && Data.hit >= 1)
        {
            //transform.position = HeroData.path[0];
            Vector3 moveKoof = path[0].OccupiedUnit?.typeInput == TypeInput.Down ? new Vector3(.5f, .0f) : new Vector3(.5f, .5f);

            UpdateAnimate(Position, path[0].position);
            _animator.SetBool("isWalking", true);

            yield return StartCoroutine(SmoothLerp((Vector3)Position + moveKoof, (Vector3)path[0].position + moveKoof));
            //ChangeHit(HeroData.path[0], - 1);
            Data.hit -= CalculateHitByNode(path[0]);
            hit.Value -= CalculateHitByNode(path[0]);
            Position = path[0].position;
            //Position = path[0]._position;
            GameManager.Instance.MapManager.DrawCursor(path, this);

            List<GridTileNode> noskyNode = GameManager.Instance.MapManager.DrawSky(path[0], 4);
            _player.SetNosky(noskyNode);

            GameManager.Instance.MapManager.SetColorForTile(path[0].position, Color.cyan);
            if (path[0].Protected)
            {
                path[0].ProtectedUnit.OnGoHero(LevelManager.Instance.GetPlayer(Data.idPlayer));
                // path[0].SetProtectedNeigbours(null);
                GameManager.Instance.ChangeState(GameState.StopMoveHero);
            }
            if (path[0].OccupiedUnit)
            {
                path[0].OccupiedUnit.OnGoHero(LevelManager.Instance.GetPlayer(Data.idPlayer));
            }
            path.RemoveAt(0);


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

    public override void UpdateAnimate(Vector3Int startPosition, Vector3Int endPosition)
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
        Data.hit += _hit * Data.speed;
        hit.Value += Data.hit;
    }
    public float CalculateHitByNode(GridTileNode node)
    {
        var dataNode = ResourceSystem.Instance.GetLandscape(node.TypeGround);
        float val = (100 - dataNode.dataNode.speed + (100 - Data.speed + 10));
        //Debug.Log($"CalculateHitByNode::: {val}");
        return val;
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

        if (LevelManager.Instance.ActivePlayer.DataPlayer.PlayerDataReferences.ListHero.Contains(this))
        {
            //LevelManager.Instance.GetActivePlayer().SetActiveHero(this);
            SetHeroAsActive();
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

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.Units.heroes.Add(sdata);
    }

    public override void OnLoadUnit<T>(SaveDataUnit<T> Data)
    {
        base.OnLoadUnit(Data);
        DataHero dh = Data.data as DataHero;
        this.Data = dh;
    }

}