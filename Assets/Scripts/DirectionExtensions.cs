﻿using UnityEngine;

public static class DirectionExtensions
{
    private static Quaternion[] _rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0),
        Quaternion.Euler(0f, 180f, 0),
        Quaternion.Euler(0f, 270f, 0)
    };

    private static Vector3[] _halfVectors =
    {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

    public static Quaternion GetRotation(this Direction direction) => _rotations[(int)direction];

    public static float GetAngle(this Direction direction) => 90f * (int) direction;

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
    {
        if (current == next)
            return DirectionChange.None;
        
        if (current + 1 == next || current - 3 == next)
            return DirectionChange.TurnRight;
        
        if (current - 1 == next || current + 3 == next)
            return DirectionChange.TurnLeft;
        
        return DirectionChange.TurnAround;
    }

    public static Vector3 GetHalfVector(this Direction direction) => _halfVectors[(int) direction];
}

public enum Direction
{
    North,
    East,
    South,
    West
}

public enum DirectionChange
{
    None,
    TurnRight,
    TurnLeft,
    TurnAround
}