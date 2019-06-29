using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour{
    public RawImage playerTexture;
    public GameObject model;
    GameObject ModelCamera;
    GameObject Model;
    static ModelShow playerModel = null;

    Button startBtn = null;

    public static void OnDrag(PointerEventData eventData) {
        //print("OnDrag");
        if (playerModel != null) {
            playerModel.OnDrag(eventData);
        }
    }

    void Start() {
        ModelCamera = GameObject.Find("ModelCamera");
        playerTexture = transform.Find("PlayerTexture").GetComponent<RawImage>();
        Rect rect = playerTexture.gameObject.GetComponent<RectTransform>().rect;
        playerModel = new ModelShow(model, (int)(rect.width), (int)(rect.height));
        playerModel.Show(playerTexture, true);
        //playerModel.Clear();

        Text startBtnText = transform.Find("Start/Text").gameObject.GetComponent<Text>();
        startBtn = transform.Find("Start").GetComponent<Button>();
        startBtn.onClick.AddListener(() => {
            if (startBtnText.text == "Loading") return;
            startBtnText.text = "Loading";
            SceneManager.LoadScene("Game");
        });
    }
}