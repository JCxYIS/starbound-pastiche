using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class GameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject _floorprefab;
    [SerializeField] GameObject _resultCanvasPrefab;

    [Header("Predefine")]
    [SerializeField] AudioClip _bgm;

    [Header("Channels")]
    [SerializeField] FloorCanReallocateChannel _floorCanReallocateChannel;
    [SerializeField] FloorFirstSteppedChannel _floorFirstSteppedChannel;
    [SerializeField] GameStartChannel _gameStartChannel;
    [SerializeField] GameOverChannel _gameOverChannel;
    [SerializeField] ComboChangeChannel _comboChangeChannel;
    [SerializeField] ComboAddChannel _comboAddChannel;
    [SerializeField] LvUpChannel _lvUpChannel;

    
    [Header("Variables")]
    [SerializeField] uint createdFloor = 0;
    [SerializeField] uint combo = 0;
    [SerializeField] bool isGameRunning = false;
    [SerializeField] float comboResetClock = 0f;
    [SerializeField] float nextLvUpTime = 0f;
    [SerializeField] float lastPosX = 0f;
    public BigInteger score = 0;
    public uint lv = 0;
    public uint maxCombo = 0;
    public uint steppedFloor = 0;
    public int randomSeed = 0;
    public float lvProgress;
    [SerializeField] Queue<Floor> floors = new Queue<Floor>();

    public int Section => (int)(lv-1) / 10;              // lv.0-10 => 0, lv.11-19 => 1, lv.30 => 2
    public float LvAtCurrentSection => (lv-1) % 10;      // lv.X1 => 0, lv.X3 => 2, lv.X0 => 9

    AudioSource _bgmPlayer;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public void Init(GameData gameData)
    {
        randomSeed = gameData.RandomSeed;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // init random
        // if(randomSeed == 0)
        //     randomSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(randomSeed);

        // init floors
        for(int i = 0; i < 5; i++)
        {
            AllocateFloor(); // new Vector2(Random.Range(-2f, 2f), i+0.5f) 
        }      

        // init bgm
        _bgmPlayer = gameObject.AddComponent<AudioSource>();
        _bgmPlayer.clip = _bgm;  
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {   
        // combo reset
        if(comboResetClock > 0)
        {
            comboResetClock -= Time.deltaTime;
            if(comboResetClock <= 0)
            {
                combo = 0;
                _comboChangeChannel.RaiseEvent(combo);
            }
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        // add score
        if(isGameRunning)
        {
            score += ((BigInteger)combo * combo + 1) * lv * lv;
        }

        // lv up
        if(_bgmPlayer.time > nextLvUpTime)
        {
            lv++;
            nextLvUpTime = _bgm.length / 30f * lv;
            _lvUpChannel.RaiseEvent(lv);
        }
        lvProgress = 1 - (nextLvUpTime - _bgmPlayer.time) / (_bgm.length / 30f);
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        _floorCanReallocateChannel.OnEventRaised += OnFloorCanReallocate;
        _floorFirstSteppedChannel.OnEventRaised += OnfloorStepped;
        _comboAddChannel.OnEventRaised += DoComboAdd;
        _gameOverChannel.OnEventRaised += GameOver;
    }    

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        _floorCanReallocateChannel.OnEventRaised -= OnFloorCanReallocate;
        _floorFirstSteppedChannel.OnEventRaised -= OnfloorStepped;
        _gameOverChannel.OnEventRaised -= GameOver;
    }


    Floor AllocateFloor()
    {
        Floor f;
        if(floors.Count <= 5)
        {
            f = Instantiate(_floorprefab).GetComponent<Floor>();
        }    
        else
        {
            f = floors.Dequeue();
        }

        floors.Enqueue(f);
        createdFloor++;

        // detemine floor width (1~3) and pos
        float floorWidth = 1;
        float minDistance = 1;
        float maxDistance = 1;
        uint mlv = lv < 1 ? 1 : lv;

        if(mlv <= 10)
        {
            floorWidth = Mathf.Lerp(3.0f, 2.2f, (mlv-1)/9f);
            maxDistance = Mathf.Lerp(0.8f, 2.2f, (mlv-1)/9f);
            minDistance = Mathf.Lerp(0.1f, 1.3f, (mlv-1)/9f);
        }
        else if(mlv <= 20)
        {
            floorWidth = Mathf.Lerp(2.2f, 2.0f, (mlv-11)/9f);
            maxDistance = Mathf.Lerp(0.9f, 1.9f, (mlv-11)/9f);
            minDistance = Mathf.Lerp(0.2f, 1.0f, (mlv-11)/9f);
        }
        else if(mlv <= 28)
        {
            floorWidth = 2f;
            maxDistance = Mathf.Lerp(1.0f, 1.7f, (mlv-21)/7f);
            minDistance = Mathf.Lerp(0.3f, 0.8f, (mlv-21)/7f);
        }
        else
        {
            floorWidth = 1f;
            maxDistance = 1.2f;
            minDistance = 0.3f;
        }

                
        float floorPosX = 9999;
        float distance = Mathf.Abs(floorPosX - lastPosX);
        int attempt = 0;
        while(distance > maxDistance || distance < minDistance)
        {
            if(attempt > 30)
            {
                print($"<color=red>[FLOOR] DISTANCE={minDistance} < {distance}</color>");
                break;
            }            
            floorPosX = Random.Range(-2f, 2f);
            distance = Mathf.Abs(floorPosX - lastPosX);
            attempt++;
        }
        lastPosX = floorPosX;

        print($"[FLOOR] WIDTH=<color=green>{floorWidth}</color> DISTANCE={minDistance} < <color=green>{distance}</color> < {maxDistance} @ {floorPosX}");
        f.Reallocate(createdFloor, Section, floorPosX, floorWidth); 

        return f;
    }

    

    /// <summary>
    /// When player steps on the first floor, the game hajimari yo!
    /// </summary>
    void GameStart()
    {
        print("Game Start!");
        _gameStartChannel.RaiseEvent(this);
        _bgmPlayer.Play();
        isGameRunning = true;
    }

    /// <summary>
    /// 
    /// </summary>
    void GameOver(GameOverReason reason)
    {
        if(!isGameRunning)
            return;
        print(reason);
        ResultCanvas resultCanvas = Instantiate(_resultCanvasPrefab).GetComponent<ResultCanvas>();
        resultCanvas.Init(reason, this);
        _bgmPlayer.Pause();
        isGameRunning = false;
    }   

    private void OnfloorStepped(Floor floor)
    {
        steppedFloor++;
        if(steppedFloor == 1)
        {
            GameStart();
        }

        if(lv >= 30 && !_bgmPlayer.isPlaying) // TODO end condition
        {
            GameOver(GameOverReason.Finished);
        }
    }

    private void DoComboAdd()
    {
        combo++;
        if(combo > maxCombo)
            maxCombo = combo;
        comboResetClock = 2.020f;
        _comboChangeChannel.RaiseEvent(combo); 
    }

    private void OnFloorCanReallocate(Floor floor)
    {
        Floor newFloor = AllocateFloor();
    }

    
}