using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;
    [SerializeField] private Vector2Int _size;
    [SerializeField] private GameTileContentFactory _tileContentFactory;

    private GameTileContentSwitch _gameTileContentSwitch;
    private GameTile[] _tiles;
    private readonly Queue<GameTile> _searchFrontier = new Queue<GameTile>();
    private readonly List<GameTile> _spawnPoints = new List<GameTile>();

    private readonly List<IUpdatableGameTileContent> _contentToUpdate =
        new List<IUpdatableGameTileContent>();

    public IReadOnlyList<GameTile> SpawnPoints { get; private set; }
    
    private void Start()
    {
        transform.position = Vector3.zero;
        _ground.localScale = new Vector3(_size.x, _size.y, 1);
        _tiles = new GameTile[_size.x * _size.y];

        Vector2 offset = new Vector2((_size.x - 1) * 0.5f,  (_size.y - 1) * 0.5f);
        for (int y = 0, i = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++, i++)
            {
                _tiles[i] = Instantiate(_tilePrefab, transform, false);
                _tiles[i].Content = _tileContentFactory.Get(GameTileContentType.Empty);
                _tiles[i].transform.localPosition = new Vector3(x - offset.x,0, y - offset.y);

                if (x > 0)
                    GameTile.MakeEastWestNeighbors(_tiles[i], _tiles[i - 1]);
                if (y > 0)
                    GameTile.MakeNorthSouthNeighbors(_tiles[i], _tiles[i - _size.x]);

                _tiles[i].IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                    _tiles[i].IsAlternative = !_tiles[i].IsAlternative;

            }
        }

        SpawnPoints = _spawnPoints.AsReadOnly();

        _gameTileContentSwitch = new GameTileContentSwitch(_tileContentFactory, TryFindPathes);
        
        ToggleDestination(_tiles[(int)(_size.x * _size.y * 0.5)]);
        ToggleSpawnPoint(_tiles[0]);
    }

    private void Update()
    {
        for (int i = 0; i < _contentToUpdate.Count; i++)
        {
            _contentToUpdate[i].UpdateContent();
        }
    }


    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + _size.x * 0.5);
            int y = (int)(hit.point.z + _size.y * 0.5);
            
            if ((x < 0 || x > _size.x) && (y < 0 || y > _size.y))
                return null;

            return _tiles[x + y * _size.x];
        }

        return null;
    }

    public void ToggleDestination(GameTile tile) => _gameTileContentSwitch.ToggleDestination(tile);

    public void ToggleTower(GameTile tile) => _gameTileContentSwitch.ToggleTower(tile, _contentToUpdate);

    public void ToggleWall(GameTile tile) => _gameTileContentSwitch.ToggleWall(tile);

    public void ToggleSpawnPoint(GameTile tile) => _gameTileContentSwitch.ToggleSpawnPoint(tile, _spawnPoints);

    private bool TryFindPathes()
    {
        foreach (var tile in _tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecameDestination();
                _searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if (_searchFrontier.Count == 0)
            return false;

        while (_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchFrontier.Enqueue(tile.GrowPathToWest());
                    _searchFrontier.Enqueue(tile.GrowPathToEast());
                    _searchFrontier.Enqueue(tile.GrowPathToSouth());
                    _searchFrontier.Enqueue(tile.GrowPathToNorth());
                }
                else
                {
                    _searchFrontier.Enqueue(tile.GrowPathToNorth());
                    _searchFrontier.Enqueue(tile.GrowPathToSouth());
                    _searchFrontier.Enqueue(tile.GrowPathToEast());
                    _searchFrontier.Enqueue(tile.GrowPathToWest());
                }
                
            }
        }

        foreach (var tile in _tiles)
            if (!tile.HasPath)
                return false;

        foreach (var tile in _tiles)
            tile.ShowPath();
        
        return true;
    }

}

class GameTileContentSwitch
{
    private Func<bool> ContentChanged;

    private GameTileContentFactory _factory;
    public GameTileContentSwitch(GameTileContentFactory factory, Func<bool> contentChangedCallback)
    {
        _factory = factory;
        if (contentChangedCallback == null)
            throw new ArgumentException("Set correct contentChangedCallback, it can`t be null");
        ContentChanged = contentChangedCallback;
    }
    
    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Empty)
        {
            SetContentTo(GameTileContentType.Destination, tile);
        }
        else if(tile.Content.Type == GameTileContentType.Destination)
        {
            if (!TrySetContentTo(GameTileContentType.Empty, tile))
            {
                SetContentTo(GameTileContentType.Destination, tile);
            }

        }
    }
    

    public void ToggleTower(GameTile tile, List<IUpdatableGameTileContent> contentToUpdate)
    {
        if (tile.Content.Type == GameTileContentType.Tower)
        {
            contentToUpdate.Remove(tile.Content as IUpdatableGameTileContent);
            SetContentTo(GameTileContentType.Empty, tile);
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            if (TrySetContentTo(GameTileContentType.Tower, tile))
            {
                contentToUpdate.Add(tile.Content as IUpdatableGameTileContent);
            }
            else
            {
                SetContentTo(GameTileContentType.Empty, tile);
            }
                
        }
        else
        {
            tile.Content = _factory.Get(GameTileContentType.Tower);
            contentToUpdate.Add(tile.Content as IUpdatableGameTileContent);
        }
    }
    
    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
            SetContentTo(GameTileContentType.Empty, tile);
        else if (tile.Content.Type == GameTileContentType.Empty)
            if (!TrySetContentTo(GameTileContentType.Wall, tile))
                SetContentTo(GameTileContentType.Empty, tile);
    }
    public void ToggleSpawnPoint(GameTile tile, List<GameTile> spawnPoints)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                tile.Content = _factory.Get(GameTileContentType.Empty);
                spawnPoints.Remove(tile);
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = _factory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }
    private bool TrySetContentTo(GameTileContentType type, GameTile tile)
    {
        tile.Content = _factory.Get(type);
        return ContentChanged();
    }

    private void SetContentTo(GameTileContentType type, GameTile tile)
    {
        TrySetContentTo(type, tile);
    }
}
