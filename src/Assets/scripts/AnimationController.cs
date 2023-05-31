using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController
{
    const float DELTA_TIME_MAX = 1.0f;
    float _time = 0.0f;
    float _inv_time_max = 1.0f;

    public void Set(float max_time)//時間のセット。負は不正
    {
        Debug.Assert(0.0f < max_time); 
        
        _time = max_time;
        _inv_time_max = 1.0f / max_time;//逆数を持っておく
    }

    public bool Update(float delta_time)//アニメーション中ならtrueを返す
    {
        if(DELTA_TIME_MAX < delta_time)//長すぎる更新はおかしい
        {
            delta_time = DELTA_TIME_MAX;
        }

        _time -= delta_time;

        if(_time <= 0.0f)
        {
            _time = 0.0f;
            return false;
        }

        return true;
    }

    public float GetNormalized()
    {
        return _time * _inv_time_max;//0~1の範囲でどれくらい進んだか
    }
}
