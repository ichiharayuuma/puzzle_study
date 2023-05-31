using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController
{
    const float DELTA_TIME_MAX = 1.0f;
    float _time = 0.0f;
    float _inv_time_max = 1.0f;

    public void Set(float max_time)//���Ԃ̃Z�b�g�B���͕s��
    {
        Debug.Assert(0.0f < max_time); 
        
        _time = max_time;
        _inv_time_max = 1.0f / max_time;//�t���������Ă���
    }

    public bool Update(float delta_time)//�A�j���[�V�������Ȃ�true��Ԃ�
    {
        if(DELTA_TIME_MAX < delta_time)//��������X�V�͂�������
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
        return _time * _inv_time_max;//0~1�͈̔͂łǂꂭ�炢�i�񂾂�
    }
}
