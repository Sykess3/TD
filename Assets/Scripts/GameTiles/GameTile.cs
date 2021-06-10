using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;

public  class GameTile : MonoBehaviour
{
    [SerializeField] private Transform _arrow;
    private GameTile _west, _north, _south, _east;
    private GameTile _nextOnPath;

    private int _distance;
    
    private Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion _eastRotation = Quaternion.Euler(90, 90, 0);
    private Quaternion _southRotation = Quaternion.Euler(90, 180, 0);
    private Quaternion _westRotation = Quaternion.Euler(90, 270, 0);

    private GameTileContent _content;
    
    public bool HasPath => _distance != int.MaxValue;
    public bool IsAlternative { get; set; }
    
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
        _nextOnPath = null;
    }

    public void BecameDestination()
    {
        _distance = 0;
        _nextOnPath = null;
    }

    public GameTile GrowPathToNorth() => GrowPathTo(_north);
    public GameTile GrowPathToWest() => GrowPathTo(_west);
    public GameTile GrowPathToSouth() => GrowPathTo(_south);
    public GameTile GrowPathToEast() => GrowPathTo(_east);

    public void ShowPath()
    {
        if (_distance == 0)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }
        
        _arrow.gameObject.SetActive(true);
        _arrow.localRotation =
            _nextOnPath == _north ? _northRotation :
            _nextOnPath == _east ? _eastRotation :
            _nextOnPath == _south ? _southRotation :
            _westRotation;
    }
    
    private GameTile GrowPathTo(GameTile neighbor)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
            return null;

        neighbor._distance = _distance + 1;
        neighbor._nextOnPath = this;

        return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
    }
}
