using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct FallData
{
    public readonly int X { get; }
    public readonly int Y { get; }
    public readonly int Dest { get; }
    public FallData(int x, int y, int dest)
    {
        X = x; 
        Y = y; 
        Dest = dest;
    }
}

public class BoardContoroller : MonoBehaviour
{
    public const int FALL_FRAME_PER_CELL = 5; //�P�ʃZ��������̗����t���[����
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    [SerializeField] GameObject prefabPuyo = default!;

    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    GameObject[,] _Puyos = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    //������ۂ̈ꎞ�I�ϐ�
    List<FallData> _falls = new();
    int _fallFrames = 0;

    private void ClearAll()//�S�폜
    {
        for(int i = 0; i < BOARD_HEIGHT; i++)
        {
            for(int j = 0; j < BOARD_WIDTH; j++)
            {
                _board[i, j] = 0;

                if (_Puyos[i, j] != null) Destroy(_Puyos[i, j]);
                _Puyos[i, j] = null;
            }
        }
    }

    void Start()
    {
        ClearAll();
    }

    public static bool IsValidated(Vector2Int pos)//�s���ȏꏊ�ɒu�����Ƃ��ĂȂ���
    {
        return 0 <= pos.x && pos.x < BOARD_WIDTH
            && 0 <= pos.y && pos.y < BOARD_HEIGHT;
    }

    public bool CanSettle(Vector2Int pos)//�u���邩
    {
        if (!IsValidated(pos)) return false;

        return 0 == _board[pos.y, pos.x];
    }

    public bool Settle(Vector2Int pos, int val)//�u��
    {
        if (!CanSettle(pos)) return false;

        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyos[pos.y, pos.x] == null);
        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _Puyos[pos.y, pos.x] = Instantiate(prefabPuyo, world_position, Quaternion.identity, transform);
        _Puyos[pos.y, pos.x].GetComponent<PuyoScript>().SetPuyoType((PuyoType)val);

        return true;
    }

    public bool CheckFall()
    {
        _falls.Clear();
        _fallFrames = 0;

        //�����鍂���̋L�^�p
        int[] dsts = new int[BOARD_WIDTH];
        for (int x = 0; x < BOARD_WIDTH; x++)
        {
            dsts[x] = 0;
        }

        int max_check_line = BOARD_HEIGHT - 1;
        for(int y = 0; y < max_check_line; y++)
        {
            for(int x = 0; x < BOARD_WIDTH; x++)
            {
                if (_board[y, x] == 0) continue;

                int dst = dsts[x];
                dsts[x] = y + 1;

                if (y == 0) continue;//��ԉ��Ȃ痎���Ȃ�

                if (_board[y - 1, x] != 0) continue;//���ɂ���Η����Ȃ�

                _falls.Add(new FallData(x,y,dst));

                //�f�[�^��ύX���Ă���
                _board[dst, x] = _board[y, x];
                _board[y, x] = 0;
                _Puyos[dst, x] = _Puyos[y, x];
                _Puyos[y, x] = null;

                dsts[x] = dst + 1;
            }
        }

        return _falls.Count != 0;
    }

    public bool Fall()
    {
        _fallFrames++;

        float dy = _fallFrames / (float)FALL_FRAME_PER_CELL;
        int di = (int)dy;

        for(int i = _falls.Count - 1; 0 <= i; i--)//���[�v���ŏ����Ă����v�Ȃ悤�Ɍ�납��
        {
            FallData f = _falls[i];

            Vector3 pos = _Puyos[f.Dest, f.X].transform.localPosition;
            pos.y = f.Y - dy;

            if(f.Y <= f.Dest + di)
            {
                pos.y = f.Dest;
                _falls.RemoveAt(i);
            }
            _Puyos[f.Dest, f.X].transform.localPosition = pos;//�\���ʒu�̍X�V
        }

        return _falls.Count != 0;
    }
}
