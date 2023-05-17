using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PuyoScript[] _puyocontrollers = new PuyoScript[2] { default!, default! };
    [SerializeField] BoardContoroller boardContoroller = default!;

    Vector2Int _position;

    void Start()
    {
        _puyocontrollers[0].SetPuyoType(PuyoType.Green);
        _puyocontrollers[1].SetPuyoType(PuyoType.Red);

        _position = new Vector2Int(2, 12);

        _puyocontrollers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        _puyocontrollers[1].SetPos(new Vector3((float)_position.x,(float)_position.y + 1.0f, 0.0f));
    }

    private bool CanMove(Vector2Int pos)
    {
        if(!boardContoroller.CanSettle(pos)) return false;
        if(!boardContoroller.CanSettle(pos+Vector2Int.up)) return false;

        return true;
    }

    private bool Translate(bool is_right)
    {
        Vector2Int pos = _position + (is_right ? Vector2Int.right: Vector2Int.left);
        if (!CanMove(pos)) return false;

        _position= pos;

        _puyocontrollers[0].SetPos(new Vector3((float)_position.x, (float)_position.y, 0.0f));
        _puyocontrollers[1].SetPos(new Vector3((float)_position.x, (float)_position.y + 1.0f, 0.0f));

        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Translate(true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Translate(false);
        }
    }
}
