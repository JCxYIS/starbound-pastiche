using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject _Floorprefab;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            CreateFloor( new Vector2(Random.Range(-2f, 2f), i+0.5f) );
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }

    void CreateFloor(Vector2 pos)
    {
        GameObject g = Instantiate(_Floorprefab);
        g.transform.position = pos;
    }
}