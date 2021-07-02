using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower : GameTileContent, IUpdatableGameTileContent
{
    private const int ENEMY_LAYER_MASK = 1 << 6;

    [Range(1.5f, 10.5f)] 
    [SerializeField] private float _targetingRange = 1.5f;

    [Range(1f, 100f)] [SerializeField] private float _damagePerSecond = 10f;
    
    [SerializeField] private Transform _turret;
    [SerializeField] private Transform _laserBeam;

    private Vector3 _laserBeamScale;

    private TargetPoint _target;

    private void Awake()
    {
        _laserBeamScale = _laserBeam.localScale;
    }

    public void UpdateContent()
    {
        if (IsAcquireTarget() || IsTargetTracked())
            Shoot();
        else
            _laserBeam.localScale = Vector3.zero;
    }

    private bool IsAcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(
            transform.localPosition, _targetingRange, ENEMY_LAYER_MASK
        );
        if (targets.Length > 0) {
            _target = targets[0].GetComponent<TargetPoint>();
            return true;
        }
        _target = null;
        return false;
    }

    private bool IsTargetTracked()
    {
        if (_target == null)
        {
            return false;
        }

        float distanceBetweenEnemyAndTower = 
            Vector3.Distance(transform.position, _target.transform.position);
        if (distanceBetweenEnemyAndTower > _targetingRange + _target.ColliderSize * _target.Enemy.Scale)
        {
            _target = null;
            return false;
        }

        return true;
    }

    private void Shoot()
    {
        var point = _target.Position;
        _turret.LookAt(point);

        _laserBeam.localRotation = _turret.localRotation;
        float distance = Vector3.Distance(point, _turret.position);
        _laserBeamScale.z = distance;
        _laserBeam.localScale = _laserBeamScale;
        _laserBeam.localPosition = _turret.localPosition + 0.5f * distance * _laserBeam.forward;
        
        _target.Enemy.TakeDamage(_damagePerSecond * Time.deltaTime);
    }
    
    [ExecuteAlways]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, _targetingRange);
    }
}

