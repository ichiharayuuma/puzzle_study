using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum RotState
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,

        Invalid = -1,
    }

    [SerializeField] PuyoScript[] _puyocontrollers = new PuyoScript[2] { default!, default! };
    [SerializeField] BoardContoroller boardContoroller = default!;

    AnimationController _animationController = new AnimationController();
    LogialInput logicalInput = new();
    Vector2Int _position;
    RotState _rotate = RotState.Up;
    Vector2Int _last_position;
    RotState _last_rotate = RotState.Up;

    const int TRANS_TIME = 3;
    const int ROT_TIME = 3;
    const int FALL_COUNT_UNIT = 120;
    const int FALL_COUNT_SPD = 10;
    const int FALL_COUNT_FAST_SPD = 20;
    const int GROUND_FRAMES = 50;

    int _fallCount = 0;
    int _groundFrame = GROUND_FRAMES;

    static readonly Vector2Int[] rotate_tbl = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    static readonly KeyCode[] key_code_tbl = new KeyCode[(int)LogialInput.Key.Max] 
    {
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.X,
        KeyCode.Z,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
    };

    void Start()
    {
        _puyocontrollers[0].SetPuyoType(PuyoType.Green);
        _puyocontrollers[1].SetPuyoType(PuyoType.Red);

        _position = new Vector2Int(2, 12);
        _rotate = RotState.Up;

        _puyocontrollers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        Vector2Int posChild = CalcChildPuyoPos(_position,_rotate);
        _puyocontrollers[1].SetPos(new Vector3((float)posChild.x,(float)posChild.y, 0.0f));
    }

    private bool CanMove(Vector2Int pos, RotState rot)
    {
        if(!boardContoroller.CanSettle(pos)) return false;
        if(!boardContoroller.CanSettle(CalcChildPuyoPos(pos,rot))) return false;

        return true;
    }

    void SetTransition(Vector2Int pos, RotState rot, int time)//animationに移動先、時間を設定
    {
        _last_position = _position;
        _last_rotate = _rotate;

        _position = pos;
        _rotate = rot;

        _animationController.Set(time);
    }

    private bool Translate(bool is_right)//平行移動
    {
        //仮想的に移動
        Vector2Int pos = _position + (is_right ? Vector2Int.right: Vector2Int.left);
        if (!CanMove(pos, _rotate)) return false;

        //実際に移動
        SetTransition(pos, _rotate, TRANS_TIME);

        return true;
    }

    private static Vector2Int CalcChildPuyoPos(Vector2Int pos, RotState rot)//子ぷよの回転先
    {
        return pos + rotate_tbl[(int)rot];
    }

    bool Rotate(bool is_right)//回転
    {
        RotState rot = (RotState)(((int)_rotate + (is_right ? +1 : +3)) & 3);

        //仮想的に移動。埋まったらずらす
        Vector2Int pos = _position;
        switch (rot)
        {
            case RotState.Down:
                if(!boardContoroller.CanSettle(pos + Vector2Int.down) ||
                    !boardContoroller.CanSettle(pos + new Vector2Int(is_right ? 1 : -1, -1)))
                {
                    pos += Vector2Int.up;
                }
                break;
            case RotState.Right:
                if(!boardContoroller.CanSettle(pos + Vector2Int.right))
                {
                    Debug.Log("right");
                    Debug.Log(pos);
                    pos += Vector2Int.left;
                    Debug.Log(pos);
                }
                break;
            case RotState.Left:
                if(!boardContoroller.CanSettle(pos + Vector2Int.left))
                {
                    pos += Vector2Int.right;
                }
                break;
            case RotState.Up:
                break;
            default:
                Debug.Assert(false);
                break;
        }

        if(!CanMove(pos, rot)) return false;

        //実際に移動
        SetTransition(pos, rot, ROT_TIME);

        return true;
    }

    void Settle()
    {
        bool is_set0 = boardContoroller.Settle(_position, (int)_puyocontrollers[0].GetPuyoType());
        Debug.Assert(is_set0);

        bool is_set1 = boardContoroller.Settle(CalcChildPuyoPos(_position, _rotate), (int)_puyocontrollers[1].GetPuyoType());
        Debug.Assert(is_set1);

        gameObject.SetActive(false);
    }

    void QuickDrop()
    {
        Vector2Int pos = _position;
        do
        {
            pos += Vector2Int.down;
        }while(CanMove(pos,_rotate));
        pos -= Vector2Int.down;

        _position = pos;

        Settle();
    }

    bool Fall(bool is_fast)
    {
        _fallCount -= is_fast ? FALL_COUNT_FAST_SPD : FALL_COUNT_SPD;

        while(_fallCount < 0)
        {
            if(!CanMove(_position + Vector2Int.down, _rotate))
            {
                //落ちれないなら
                _fallCount = 0;//動きを止める
                if (0 < --_groundFrame) return true;

                //時間切れになったら固定
                Settle();
                return false;
            }

            //落ちれるなら先に進む
            _position += Vector2Int.down;
            _last_position += Vector2Int.down;
            _fallCount += FALL_COUNT_UNIT;
        }

        return true;
    }

    void Control()
    {
        if(!Fall(logicalInput.isRaw(LogialInput.Key.Down))) return;

        if (_animationController.Update()) return;


        if (logicalInput.isRep(LogialInput.Key.Right))
        {
            if(Translate(true))     return;
        }
        if (logicalInput.isRep(LogialInput.Key.Left))
        {
            if(Translate(false))    return;
        }

        if (logicalInput.isTrg(LogialInput.Key.RotR))
        {
            if(Rotate(true))        return;
        }
        if (logicalInput.isTrg(LogialInput.Key.RotL))
        {
            if(Rotate(false))       return;
        }

        if (logicalInput.isRel(LogialInput.Key.QuickDrop))
        {
            QuickDrop();
        }
    }

    static Vector3 Interpolate(Vector2Int pos, RotState rot, Vector2Int pos_last, RotState rot_last, float rate)
    {
        //平行移動
        Vector3 p = Vector3.Lerp(
            new Vector3((float)pos.x, (float)pos.y, 0.0f),
            new Vector3((float)pos_last.x, (float)pos_last.y, 0.0f), rate);

        if (rot == RotState.Invalid) return p;

        //回転
        float theta0 = 0.5f * Mathf.PI * (float)(int)rot;
        float theta1 = 0.5f * Mathf.PI * (float)(int)rot_last;
        float theta = theta1 - theta0;

        //近いほうへ
        if(+Mathf.PI < theta) theta = theta - 2.0f * Mathf.PI;
        if(theta < -Mathf.PI) theta = theta + 2.0f * Mathf.PI;

        theta = theta0 + rate * theta;

        return p + new Vector3(Mathf.Sin(theta), Mathf.Cos(theta), 0.0f);
    }

    void UpdateInput()//入力を取り込む
    {
        LogialInput.Key inputDev = 0;//デバイス値

        //キー取得
        for(int i = 0; i< (int)LogialInput.Key.Max; i++)
        {
            if (Input.GetKey(key_code_tbl[i]))
            {
                inputDev |= (LogialInput.Key)(1 << i);
                Console.WriteLine(inputDev.ToString());
            }
        }

        logicalInput.Update(inputDev);
    }

    void FixedUpdate()
    {
        //入力を取り込む
        UpdateInput();

        //動かす
        Control();

        //表示
        Vector3 dy = Vector3.up * (float)_fallCount / (float)FALL_COUNT_UNIT;
        float anim_rate = _animationController.GetNormalized();
        _puyocontrollers[0].SetPos(dy + Interpolate(_position, RotState.Invalid, _last_position, RotState.Invalid, anim_rate));//軸ぷよにはInvalidを設定
        _puyocontrollers[1].SetPos(dy + Interpolate(_position, _rotate, _last_position, _last_rotate, anim_rate));
    }
}
