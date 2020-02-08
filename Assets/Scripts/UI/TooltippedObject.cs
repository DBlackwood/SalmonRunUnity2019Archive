using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Script for objects that will generate tooltips when the mouse is over them
 */
public class TooltippedObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // content that the tooltip will have
    public string content;

    /**
     * Handle mouse entering an object
     */
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltipInstance(content);
    }

    /**
     * Handle mouse exiting the object
     */
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltipInstance();
    }
}
