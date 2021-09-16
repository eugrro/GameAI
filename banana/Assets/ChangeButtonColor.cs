using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ChangeButtonColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button btn;

    public void OnPointerEnter(PointerEventData eventData)
    {
        btn.GetComponent<Image>().color = Color.red;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        btn.GetComponent<Image>().color = Color.red;
    }
    public void buttonColorChange()
    {
        btn.GetComponent<Image>().color = Color.red;
    }
}
