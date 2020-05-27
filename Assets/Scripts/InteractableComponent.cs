using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableComponent : MonoBehaviour, IInteractableComponent
{
    public void HandleHover(bool isHovered)
    {
        if (isHovered)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0.1f);
        }
        if (!isHovered)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
        }
    }

    public void HandleClick()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
