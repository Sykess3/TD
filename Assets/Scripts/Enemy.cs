using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _model;

    private GameTile _tileFrom, _tileTo;
    private Vector3 _positionFrom, _positionTo;
    private float _directionAngleFrom, _directionAngleTo;

    private float _progress, _progressFactor;
    private Direction _direction;
    private DirectionChange _directionChange;
    private float _speed;

    private EnemyFactory _originFactory;
    private float _pathOffset;

    public float Scale { get; private set; }
    public float Health { get; private set; }
    
    public void Init(float scale, float speed, EnemyFactory originFactory, float offset)
    {
        _model.localScale = new Vector3(scale, scale, scale);
        _originFactory = originFactory;
        _pathOffset = offset;
        _speed = speed;
        Scale = scale;
        Health = 100f * scale;
    }

    public void Recycle()
    {
        _originFactory.Reclaim(this);
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
    }
    public void SpawnOn(GameTile tile)
    {
        _tileFrom = tile;
        _tileTo = tile.NextTileOnPath;
        _progress = 0f;
        PrepareIntro();
    }

    public bool TryMove()
    {
        if (Health <= 0f)
        {
            _originFactory.Reclaim(this);
            return false;
        }
        _progress += Time.deltaTime * _progressFactor;
        while (_progress >= 1)
        {
            if (_tileTo == null)
            {
                _originFactory.Reclaim(this);
                return false;
            }
            _progress = (_progress - 1) / _progressFactor;
            PrepareNextState();
            _progress *= _progressFactor;
        }
        
        
        if (_directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
        
        return true;
    }

    private void PrepareIntro()
    {
        _positionFrom = _tileFrom.transform.position;
        _positionTo = _tileFrom.ExitPoint;

        _direction = _tileFrom.PathDirection;
        _directionAngleFrom = _direction.GetAngle();
        _directionAngleTo = _direction.GetAngle();
        
        _directionChange = DirectionChange.None;
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localRotation = _direction.GetRotation();
        transform.localPosition = _positionFrom;

        _progressFactor = 2f * _speed;
    }

    private void PrepareOutro()
    {
        _positionTo = _tileFrom.transform.localPosition;
        _directionChange = DirectionChange.None;
        _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f, 0f);
        transform.localRotation = _direction.GetRotation();
        _progressFactor = 2f * _speed;
    }
    
    private void PrepareNextState()
    {
        _tileFrom = _tileTo;
        _tileTo = _tileFrom.NextTileOnPath;
        _positionFrom = _positionTo;
        if (_tileTo == null)
        {
            PrepareOutro();
            return;
        }
        _positionTo = _tileFrom.ExitPoint;
        
        _directionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
        _direction = _tileFrom.PathDirection;
        _directionAngleFrom = _directionAngleTo;

        switch (_directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            default: PrepareTurnAround(); break;
        }
    }

    private void PrepareForward()
    {
        transform.localRotation = _direction.GetRotation();
        _directionAngleTo = _direction.GetAngle();
        _model.localPosition = new Vector3(_pathOffset, 0f);
        _progressFactor = _speed;
    }

    private void PrepareTurnLeft()
    {
        _directionAngleTo = _directionAngleFrom - 90f;
        _model.localPosition = new Vector3(_pathOffset + 0.5f, 0f);
        transform.localPosition = _positionFrom + _direction.GetHalfVector();
        _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f + _pathOffset));
    }

    private void PrepareTurnRight()
    {
        _directionAngleTo = _directionAngleFrom + 90f;
        _model.localPosition = new Vector3(_pathOffset - 0.5f, 0f);
        transform.localPosition = _positionFrom + _direction.GetHalfVector();
        _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
    }

    private void PrepareTurnAround()
    {
        _directionAngleTo = _directionAngleFrom + (_pathOffset < 0 ? 180 : -180);
        _model.localPosition = new Vector3(_pathOffset, 0f);
        transform.localPosition = _positionFrom;
        _progressFactor = _speed / (Mathf.PI * Mathf.Max(Mathf.Abs(_pathOffset), 0.2f));
    }
    
}
