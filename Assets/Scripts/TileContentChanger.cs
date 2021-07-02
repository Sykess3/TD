using UnityEngine;

public class TileContentChanger
{
    private readonly GameBoard _gameBoard;
    private readonly Camera _mainCamera;
    private readonly GameTileContentFactory _tileContentFactory;

    private Ray TouchRay =>  _mainCamera.ScreenPointToRay(Input.mousePosition);
    
    
    public TileContentChanger(GameBoard gameBoard, Camera mainCamera, GameTileContentFactory tileContentFactory)
    {
        _gameBoard = gameBoard;
        _mainCamera = mainCamera;
        _tileContentFactory = tileContentFactory;
    }

    public void ToggleDestination()
    {
        GameTile tile = _gameBoard.GetTile(TouchRay);
        if (tile != null)
            _gameBoard.ToggleDestination(tile);
    }

    public void ToggleTower()
    {
        GameTile tile = _gameBoard.GetTile(TouchRay);
        if (tile != null)
            _gameBoard.ToggleTower(tile);
    }
    
    public void ToggleWall()
    {
        GameTile tile = _gameBoard.GetTile(TouchRay);
        if (tile != null)
            _gameBoard.ToggleWall(tile);
    }

    public void ToggleSpawnPoint()
    {
        GameTile tile = _gameBoard.GetTile(TouchRay);
        if (tile != null)
            _gameBoard.ToggleSpawnPoint(tile);
    }
    
}
