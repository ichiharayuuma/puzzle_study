using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LogialInput
{
    [Flags]
    public enum Key//�L�[�p��enum
    {
        Right = 1 << 0,
        Left = 1 << 1,
        RotR = 1 << 2,
        RotL = 1 << 3,
        QuickDrop = 1 << 4,
        Down = 1 << 5,

        Max = 6,
    }


    //Key InputDev;//�n�[�h�E�F�A�I�ȃI���I�t�̕ێ�
    Key InputRaw;//���݂̒l
    Key InputTrg;//�������Ƃ�
    Key InputRel;//�������Ƃ�
    Key InputRep;//�A������
    int[] _trgWaitTime = new int[(int)Key.Max];

    const int KEY_REPEAT_START_TIME = 12;//�������ςȂ��Ń��s�[�g�ɓ���t���[����
    const int KEY_REPEAT_ITERATION_TIME = 1;//���s�[�g��̍X�V�t���[����

    public bool isRaw(Key key)
    {
        return InputRaw.HasFlag(key);
    }

    public bool isTrg(Key key)
    {
        return InputTrg.HasFlag(key);
    }

    public bool isRel(Key key)
    {
        return InputRel.HasFlag(key);
    }

    public bool isRep(Key key)
    {
        return InputRep.HasFlag(key);
    }

    public void Clear()
    {
        InputRaw = 0;
        InputTrg = 0;
        InputRel = 0;
        InputRep = 0;
        for(int i = 0; i < (int)Key.Max; i++)
        {
            _trgWaitTime[i] = 0;
        }
    }

    public void Update(Key inputDev)
    {
        //���͂������ꂽ/�����ꂽ
        InputTrg = (inputDev ^ InputRaw) & inputDev;
        InputRel = (inputDev ^ InputRaw) & InputRaw;

        //raw�f�[�^�̐���
        InputRaw = inputDev;

        //�L�[���s�[�g�̍쐬
        InputRep = 0;
        for(int i = 0; i < (int)Key.Max; i++)
        {
            if(InputTrg.HasFlag((Key)(1 << i)))
            {
                InputRep |= (Key)(1 << i);
                _trgWaitTime[i] = KEY_REPEAT_START_TIME;
            }else
            if(InputRaw.HasFlag((Key)(1 << i)))
            {
                if (--_trgWaitTime[i] <= 0)
                {
                    InputRep|= (Key)(1 << i);
                    _trgWaitTime[i] = KEY_REPEAT_ITERATION_TIME;
                }
            }
        }
    }
}
