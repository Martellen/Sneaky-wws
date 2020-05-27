using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherInteractableComponent : MonoBehaviour, IInteractableComponent
{
    public void HandleHover(bool isHovered)
    {
        if (isHovered)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0.1f);
        }
        if (!isHovered)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
        }
    }

    public void HandleClick()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.blue;
    }
}
