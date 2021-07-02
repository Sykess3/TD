using System;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private Transform _arrow;
    
    private GameTile _west, _north, _south, _east;
    private int _distance;
    
    private readonly Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
    private readonly Quaternion _eastRotation = Quaternion.Euler(90, 90, 0);
    private readonly Quaternion _southRotation = Quaternion.Euler(90, 180, 0);
    private readonly Quaternion _westRotation = Quaternion.Euler(90, 270, 0);
    
    private GameTileContent _content;
    
    public GameTile NextTileOnPath { get; private set; }
    public Direction PathDirection { get; private set; }
    public bool HasPath => _distance != int.MaxValue;
    public bool IsAlternative { get; set; }
    
    public Vector3 ExitPoint { get; private set; }
    
    public GameTileContent Content
    {
        set
        {
            if(_content != null)
                _content.Recycle();

            _content = value;
            _content.transform.localPosition = transform.localPosition;
        }
        get { return _content; }
    }
    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        east._west = west;
        west._east = east;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        north._south = south;
        south._north = north;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;
        NextTileOnPath = null;
    }

    public void BecameDestination()
    {
        _distance = 0;
        NextTileOnPath = null;
    }

    public GameTile GrowPathToNorth() => GrowPathTo(_north, Direction.South);
    public GameTile GrowPathToWest() => GrowPathTo(_west, Direction.East);
    public GameTile GrowPathToSouth() => GrowPathTo(_south, Direction.North);
    public GameTile GrowPathToEast() => GrowPathTo(_east, Direction.West);

    public void ShowPath()
    {
        if (_distance == 0)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }
        
        _arrow.gameObject.SetActive(true);
        _arrow.localRotation =
            NextTileOnPath == _north ? _northRotation :
            NextTileOnPath == _east ? _eastRotation :
            NextTileOnPath == _south ? _southRotation :
            _westRotation;
    }
    
    private GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
            return null;

        neighbor.PathDirection = direction;
        neighbor._distance = _distance + 1;
        neighbor.NextTileOnPath = this;

        neighbor.ExitPoint =
            (neighbor.transform.localPosition + transform.localPosition) * 0.5f;
        
        return neighbor.Content.IsBlockingPath ? null : neighbor;
    }
}
