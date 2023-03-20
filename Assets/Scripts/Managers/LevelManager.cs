using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelManager : Singleton<LevelManager>, IDataPlay
{
    [SerializeField] private Transform _camera;
        
    [SerializeField] public DataLevel level;

    [Header("General")]
    [Space(10)]
    public bool isWater = false;
    [SerializeField] private int countArea;
    public int CountArea => countArea;

    [SerializeField] public DataGameMode gameModeData;

    //public int width;
    //public int height;
    //[Range(0.5f, 1.0f)] public float koofSizeArea;

    //[Header("Mountains")]
    //[Range(0f, 0.2f)] public float noiseScaleMontain;
    //[Range(0f, 0.6f)] public float koofMountains;


    //[Header("Nature")]
    //[Range(0f, 0.2f)] public float koofNature;

    //[Header("Mines")]
    ////[Range(0f, 0.2f)] public float noiseScaleMines;
    //[Range(0f, 0.8f)] public float koofMines;


    //[Range(.01f, .2f)] public float koofMinTown;
    

    //[Header("Resource")]
    //[Range(0f, 0.01f)] public float koofResource;
    //[Range(0f, 0.05f)] public float koofFreeResource;


    //[Header("Explore")]
    //[Range(0f, 0.01f)] public float koofExplore;

    //[Header("Skills")]
    //[Range(0f, 0.1f)] public float koofSchoolSkills;

    //[Header("Artifacts")]
    //[Range(0f, 0.1f)] public float koofArtifacts;

    [Header("Setting level")]
    [Space(10)]
    public int countPlayer;
    public int countBot;
    public int maxPlayer;
    //public List<Player> listPlayer;

    public Color[] colors = new Color[4] {
        Color.red,
        Color.green,
        Color.yellow,
        Color.cyan
    };

    GridTileHelper mapHelper;

    public Player ActivePlayer
    {
        get { return level.listPlayer[level.activePlayer]; }
        set {
            level.listPlayer[level.activePlayer] = value;
        }
    }

    public LevelManager()
    {
        gameModeData = new DataGameMode();
    }

    //protected override void Awake()
    //{
    //    base.Awake();
    //    GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
    //    GameManager.OnAfterStateChanged += OnAfterStateChanged;
    //}

    //private void OnDestroy()
    //{
    //    GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
    //    GameManager.OnAfterStateChanged -= OnAfterStateChanged;
    //}

    //public void OnBeforeStateChanged(GameState newState)
    //{
    //    switch (newState)
    //    {
    //        case GameState.SaveGame:
    //            SaveSystem.SaveLevel(level);
    //            //UnitManager.Instance.SaveUnits();
    //            break;
    //    }
    //}
    //public void OnAfterStateChanged(GameState newState)
    //{
    //    switch (newState)
    //    {
    //        case GameState.SaveGame:

    //            break;
    //    }
    //}
    
    public void NewLevel()
    {

        level = new DataLevel();
        level.countPlayer = countPlayer;
        countArea = Mathf.CeilToInt((gameModeData.width * gameModeData.height) / (((gameModeData.width * gameModeData.height)  / countPlayer) * gameModeData.koofSizeArea));
        level.activePlayer = -1;

        for (int i = 0; i < countPlayer; i++)
        {
            var dataPlayer = new PlayerData();
            dataPlayer.id = i;
            dataPlayer.color = colors[i];
            dataPlayer.playerType = PlayerType.User;

            var player = new Player(dataPlayer);
            level.listPlayer.Add(player);
        }

        for (int i = countPlayer; i < (countPlayer + countBot); i++)
        {
            var dataPlayer = new PlayerData();
            dataPlayer.id = i;
            dataPlayer.color = colors[i];
            dataPlayer.playerType = PlayerType.Bot;
            var player = new Player(dataPlayer);

            level.listPlayer.Add(player);
        }

    }

    public void StepNextPlayer()
    {
        if (level.activePlayer < countPlayer - 1)
        {
            level.activePlayer++;
        }
        else
        {
            level.activePlayer = 0;
        }

        //level.activePlayer = level.activePlayer < (countPlayer + countEnemies + 1) ? level.activePlayer++ : 0;

        if (ActivePlayer.ActiveHero == null)
        {
            //GetActivePlayer().SetActiveHero(GetActivePlayer().GetActiveHero());
            ActivePlayer.ActiveHero = ActivePlayer.DataPlayer.ListHero[0];
        }

        SetPositionCamera(new Vector3(ActivePlayer.ActiveHero.Position.x, ActivePlayer.ActiveHero.Position.y, -10f));

        GameManager.Instance.mapManager.ResetSky(ActivePlayer.DataPlayer.nosky);

        if (ActivePlayer.DataPlayer.nosky.Count == 0)
        {
            List<GridTileNode> listNoskyNode = GameManager.Instance.mapManager.DrawSky(ActivePlayer.ActiveHero.OccupiedNode, 5);
            ActivePlayer.SetNosky(listNoskyNode);
        }

        //Debug.Log($" Active Hero {level.activePlayer}");
    }

    public void AddArea(int id, TypeGround typeGround)
    {
        //Debug.Log($"Add area {id}");
        Area area = new Area();
        area.id = id;
        area.typeGround = typeGround;
        TileLandscape landscape = ResourceSystem.Instance.GetLandscape(typeGround);
        area.isFraction = landscape.isFraction;
        level.listArea.Add(area);
    }
    public void RemoveArea(Area area)
    {
        level.listArea.Remove(area);
    }
    public Area GetArea(int id)
    {
        //Debug.Log($"Get area {id}");
        List<Area> listArea = level.listArea.Where(t => t.id == id).ToList();
        return listArea.Count > 0 ? listArea[0] : null;
    }

    //public void AddPlayer(Player player)
    //{
    //    listPlayer.Add(player.data.id, player);
    //}

    public Player GetPlayer(int id)
    {
        //Debug.Log($"GetPlayer {id}");
        if (id >= level.listPlayer.Count) return null;

        return level.listPlayer[id];
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        string text = string.Format("Level::: \r\n {0}", level.ToString());
        //foreach (Player player in listPlayer.Values)
        //{
        //    text += string.Format("\r\nPlayer::: id:[{0}] color:[{1}] type:[{2}]",
        //        player.data.id,
        //        player.data.color,
        //        player.data.playerType
        //        );
        //}
        foreach (Area area in level.listArea)
        {
            text += string.Format("\r\nArea::: id:[{0}] countNode:[{1}] startPosition:[{2}] \n {3}",
                area.id,
                area.countNode,
                area.startPosition,
                area.Stat.ToString()
                );
        }
        return text;
    }

#endif
    public void SetPositionCamera(Vector3 pos)
    {
        _camera.transform.position = pos;
    }

    public void LoadDataPlay(DataPlay data)
    {
        level = data.Level;
        //foreach (Player player in level.listPlayer)
        //{
        //    player.DataPlayer = new PlayerData()
        //    {
        //        ListTown = new List<UnitBase>(),
        //        ListHero = new List<Hero>(),
        //        ListMines = new List<UnitBase>(),
        //    };
        //}
        //level.listArea = new List<Area>();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        data.Level = level;
    }
}
