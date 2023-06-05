using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationController
{
    int _time = 0;
    float _inv_time_max = 1.0f;

    public void Set(int max_time)//���Ԃ̃Z�b�g�B���͕s��
    {
        Debug.Assert(0.0f < max_time); 
        
        _time = max_time;
        _inv_time_max = 1.0f / (float)max_time;//�t���������Ă���
    }

    public bool Update()//�A�j���[�V�������Ȃ�true��Ԃ�
    {
        _time = Math.Max(--_time, 0);
        return (0 < _time);
    }

    public float GetNormalized()
    {
        return (float)_time * _inv_time_max;//0~1�͈̔͂łǂꂭ�炢�i�񂾂�
    }
}
