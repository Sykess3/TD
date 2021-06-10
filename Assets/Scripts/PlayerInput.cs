using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameTileContentFactory _tileContentFactory;
    

    private TileContentChanger _contentChanger;
    
    private void Start()
    {
        _contentChanger = new TileContentChanger(_board, _mainCamera, _tileContentFactory);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _contentChanger.ToggleDestination();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _contentChanger.ToggleWall();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _contentChanger.ToggleSpawnPoint();
        }
    }
}
