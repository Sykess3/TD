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
            if (Input.GetKey(KeyCode.LeftShift))
                _contentChanger.ToggleSpawnPoint();
            else
                _contentChanger.ToggleDestination();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _contentChanger.ToggleTower();
            else
                _contentChanger.ToggleWall();
        }
        
    }
}
