using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover_TextTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text Text;
    public Color HighlightedColor = Color.yellow;

    private Color initColor;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        initColor = Text.color;
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        Text.color = HighlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text.color = initColor;
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate()
    {
        if(transform.childCount > 0)
        {
            Text = transform.GetChild(0).GetComponent<Text>();
        }
    }
}