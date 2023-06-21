using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class PuyoPair : MonoBehaviour
{
    [SerializeField] PuyoScript[] puyos = {default!, default!};

    public void SetPuyoType(PuyoType axis, PuyoType child)
    {
        puyos[0].SetPuyoType(axis);
        puyos[1].SetPuyoType(child);
    }
}
