using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModelShow {

    private static int num = 0;
    GameObject cameraGo = null;
    GameObject model;
    int width;
    int height;
    bool isContinue = false;
    RawImage image;

    public ModelShow(GameObject model, int width, int height) {
        this.model = model;
        this.width = width;
        this.height = height;
    }

    public void Show(RawImage image, bool isContinue) {
        this.image = image;
        if (cameraGo == null) {
            cameraGo = new GameObject(string.Format("ModelShow_{0:D2}", num));
            num++;
            model = GameObject.Instantiate(model, cameraGo.transform);
            cameraGo.transform.localPosition = new Vector3(10000 + num * 2000, 10000);
        }
        Camera camera3d = cameraGo.GetComponent<Camera>();
        if (camera3d == null) {
            camera3d = cameraGo.AddComponent<Camera>();
        }
        camera3d.clearFlags = CameraClearFlags.Color;
        model.transform.localPosition = new Vector3(0, 0, 3);
        model.transform.localRotation = Quaternion.Euler(0, 180, 0);

        string name = "Texture2D_"+Time.time;
        camera3d.targetTexture = new RenderTexture(width, height, 10);
        camera3d.targetTexture.name = name;
        RenderTexture rt = camera3d.targetTexture;
        image.texture = camera3d.targetTexture;
    }

    public void OnDrag(PointerEventData eventData) {
        model.transform.Rotate(new Vector3(0, -eventData.delta.x, 0));
    }

    public void Clear() {
        isContinue = false;
        GameObject.Destroy(cameraGo);
        GameObject.Destroy(model);
    }
}
