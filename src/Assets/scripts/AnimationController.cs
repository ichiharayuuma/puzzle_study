using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationController
{
    int _time = 0;
    float _inv_time_max = 1.0f;

    public void Set(int max_time)//時間のセット。負は不正
    {
        Debug.Assert(0.0f < max_time); 
        
        _time = max_time;
        _inv_time_max = 1.0f / (float)max_time;//逆数を持っておく
    }

    public bool Update()//アニメーション中ならtrueを返す
    {
        _time = Math.Max(--_time, 0);
        return (0 < _time);
    }

    public float GetNormalized()
    {
        return (float)_time * _inv_time_max;//0~1の範囲でどれくらい進んだか
    }
}
