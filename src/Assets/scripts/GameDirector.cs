using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject prefabMessage = default!;
    [SerializeField] GameObject gameObjectCanvas = default!;
    [SerializeField] PlayDirector playDirector = default!;
    GameObject _message = null;

    void Start()
    {
        StartCoroutine("GameFlow");
    }

    private IEnumerator GameFlow()
    {
        CreateMessage("Ready?");

        yield return new WaitForSeconds(1.0f);
        Destroy(_message); _message = null;

        playDirector.EnableSpawn(true);//�v���C�J�n

        while(!playDirector.IsGameOver())//�I���҂�
        {
            yield return null;
        }

        CreateMessage("GameOver");

        while (!Input.anyKey)//���͑҂�
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("TitleScene");
    }

    void CreateMessage(string message)
    {
        Debug.Assert(_message == null);
        _message = Instantiate(prefabMessage, Vector3.zero, Quaternion.identity, gameObjectCanvas.transform);
        _message.transform.localPosition = new Vector3(0, 0, 0);//���S�ɔz�u

        _message.GetComponent<TextMeshProUGUI>().text = message;
    }
}