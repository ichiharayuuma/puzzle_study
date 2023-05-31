using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardContoroller : MonoBehaviour
{
    public const int BOARD_WIDTH = 6;
    public const int BOARD_HEIGHT = 14;

    [SerializeField] GameObject prefabPuyo = default!;

    int[,] _board = new int[BOARD_HEIGHT, BOARD_WIDTH];
    GameObject[,] _Puyos = new GameObject[BOARD_HEIGHT, BOARD_WIDTH];

    private void ClearAll()//全削除
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

    public static bool IsValidated(Vector2Int pos)//不正な場所に置こうとしてないか
    {
        return 0 <= pos.x && pos.x < BOARD_WIDTH
            && 0 <= pos.y && pos.y < BOARD_HEIGHT;
    }

    public bool CanSettle(Vector2Int pos)//置けるか
    {
        if (!IsValidated(pos)) return false;

        return 0 == _board[pos.y, pos.x];
    }

    public bool Settle(Vector2Int pos, int val)//置く
    {
        if (!CanSettle(pos)) return false;

        _board[pos.y, pos.x] = val;

        Debug.Assert(_Puyos[pos.y, pos.x] == null);
        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _Puyos[pos.y, pos.x] = Instantiate(prefabPuyo, world_position, Quaternion.identity, transform);
        _Puyos[pos.y, pos.x].GetComponent<PuyoScript>().SetPuyoType((PuyoType)val);

        return true;
    }
}
