using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class itemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 开始拖动
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.SetParent(transform.parent.parent.parent,false);
    }

    // 拖动中
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    // 停止拖动
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parent,false);

    }
}
