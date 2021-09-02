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
    public BigInteger score = 0;
    public uint lv = 0;
    public uint maxCombo = 0;
    public uint steppedFloor = 0;
    public int randomSeed = 0;
    [SerializeField] Queue<Floor> floors = new Queue<Floor>();

    AudioSource _bgmPlayer;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // init random
        if(randomSeed == 0)
            randomSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(randomSeed);

        // init floors
        for(int i = 0; i < 10; i++)
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
            score += ((BigInteger)combo * combo * combo + 1) * lv * lv;
        }

        // lv up
        if(_bgmPlayer.time > nextLvUpTime)
        {
            lv++;
            nextLvUpTime = _bgm.length / 30f * lv;
            _lvUpChannel.RaiseEvent(lv);
        }
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
        if(floors.Count <= 10)
        {
            f = Instantiate(_floorprefab).GetComponent<Floor>();
        }    
        else
        {
            f = floors.Dequeue();
        }

        floors.Enqueue(f);
        createdFloor++;
        f.Reallocate(createdFloor, Random.Range(-2f, 2f) ); // TODO randomize floor spawn

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