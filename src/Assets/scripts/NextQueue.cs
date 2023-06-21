using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextQueue
{
    private enum Constants
    {
        PUYO_TYPE_MAX = 4,
        PUYO_NEXT_HISTORIES = 2,
    };

    Queue<Vector2Int> _nexts = new();

    Vector2Int CreateNext()
    {
        return new Vector2Int(
            UnityEngine.Random.Range(0, (int)Constants.PUYO_TYPE_MAX) + 1,
            UnityEngine.Random.Range(0, (int)Constants.PUYO_TYPE_MAX) + 1);
    }

    public void Initialize()
    {
        //ÉLÉÖÅ[ÇóêêîÇ≈ñûÇΩÇ∑
        for(int i = 0; i < (int)Constants.PUYO_NEXT_HISTORIES; i++)
        {
            _nexts.Enqueue(CreateNext());
        }
    }

    public void Each(System.Action<int, Vector2Int> cb)
    {
        int idx = 0;
        foreach(Vector2Int n in _nexts)
        {
            cb(idx++, n);
        }
    }

    public Vector2Int Update()
    {
        Vector2Int next = _nexts.Dequeue();
        _nexts.Enqueue(CreateNext());

        return next;
    }
}
