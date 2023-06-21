using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDirector : MonoBehaviour
{
    [SerializeField] GameObject player = default!;
    PlayerController _playerController = null;
    LogialInput _logicalInput = new();

    NextQueue _nextQueue = new();
    [SerializeField] PuyoPair[] nextPuyoPairs = { default!, default! };

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
        _playerController = player.GetComponent<PlayerController>();
        _logicalInput.Clear();
        _playerController.SetLogicalInput(_logicalInput);

        _nextQueue.Initialize();
        Spawn(_nextQueue.Update());
        UpdateNextView();
    }

    void UpdateNextView()
    {
        _nextQueue.Each( (int idx, Vector2Int n) =>
        {
            nextPuyoPairs[idx++].SetPuyoType((PuyoType)n.x, (PuyoType)n.y);
        });
    }

    void UpdateInput()//入力を取り込む
    {
        LogialInput.Key inputDev = 0;//デバイス値

        //キー取得
        for (int i = 0; i < (int)LogialInput.Key.Max; i++)
        {
            if (Input.GetKey(key_code_tbl[i]))
            {
                inputDev |= (LogialInput.Key)(1 << i);
                Console.WriteLine(inputDev.ToString());
            }
        }

        _logicalInput.Update(inputDev);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateInput();

        if(!player.activeSelf)
        {
            Spawn(_nextQueue.Update());
            UpdateNextView();
        }
    }

    bool Spawn(Vector2Int next) => _playerController.Spawn((PuyoType)next[0], (PuyoType)next[1]);
}
