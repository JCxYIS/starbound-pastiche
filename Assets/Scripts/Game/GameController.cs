using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject _floorprefab;

    [Header("Channels")]
    [SerializeField] FloorCanReallocateChannel _floorCanReallocateChannel;
    [SerializeField] FloorFirstSteppedChannel _floorFirstSteppedChannel;
    [SerializeField] GameStartChannel _gameStartChannel;
    
    [Header("Variables")]
    [SerializeField] int steppedFloor = 0;
    [SerializeField] int createdFloor = 0;
    [SerializeField] Queue<Floor> floors = new Queue<Floor>();

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            AllocateFloor(); // new Vector2(Random.Range(-2f, 2f), i+0.5f) 
        }        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        _floorCanReallocateChannel.OnEventRaised += OnFloorCanReallocate;
        _floorFirstSteppedChannel.OnEventRaised += OnfloorStepped;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        _floorCanReallocateChannel.OnEventRaised -= OnFloorCanReallocate;
        _floorFirstSteppedChannel.OnEventRaised -= OnfloorStepped;
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
        _gameStartChannel.RaiseEvent();
    }

    /// <summary>
    /// 
    /// </summary>
    void GameOver()
    {

    }

    private void OnfloorStepped(Floor floor)
    {
        steppedFloor++;
        if(steppedFloor == 1)
        {
            GameStart();
        }
    }
    private void OnFloorCanReallocate(Floor floor)
    {
        Floor newFloor = AllocateFloor();
    }

}