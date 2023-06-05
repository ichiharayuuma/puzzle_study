using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LogialInput
{
    [Flags]
    public enum Key//キー用のenum
    {
        Right = 1 << 0,
        Left = 1 << 1,
        RotR = 1 << 2,
        RotL = 1 << 3,
        QuickDrop = 1 << 4,
        Down = 1 << 5,

        Max = 6,
    }


    //Key InputDev;//ハードウェア的なオンオフの保持
    Key InputRaw;//現在の値
    Key InputTrg;//押したとき
    Key InputRel;//離したとき
    Key InputRep;//連続入力
    int[] _trgWaitTime = new int[(int)Key.Max];

    const int KEY_REPEAT_START_TIME = 12;//押しっぱなしでリピートに入るフレーム数
    const int KEY_REPEAT_ITERATION_TIME = 1;//リピート後の更新フレーム数

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
        //入力が押された/離された
        InputTrg = (inputDev ^ InputRaw) & inputDev;
        InputRel = (inputDev ^ InputRaw) & InputRaw;

        //rawデータの生成
        InputRaw = inputDev;

        //キーリピートの作成
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
