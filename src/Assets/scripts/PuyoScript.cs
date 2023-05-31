using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuyoType
{
    Blank = 0,

    Green = 1,
    Red = 2,
    Yellow = 3,
    Blue = 4,
    Purple = 5,
    Cyan = 6,

    Invalid = 7
};

public class PuyoScript : MonoBehaviour
{
    static readonly Color[] color_table = new Color[]
    {
        Color.black,

        Color.green,
        Color.red,
        Color.yellow,
        Color.blue,
        Color.magenta,
        Color.cyan,

        Color.gray
    };

    [SerializeField] Renderer my_renderer = default!;
    PuyoType _type = PuyoType.Blank;

    public void SetPuyoType(PuyoType type)//色をセット
    {
        _type = type;

        my_renderer.material.color = color_table[(int)_type];
    }

    public PuyoType GetPuyoType()//色を取得
    {
        return _type;
    }

    public void SetPos(Vector3 pos)//場所を設定
    {
        this.transform.localPosition = pos;
    }
}
