using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseState
{
    SHOW,
    HIDE
}

public class MouseController : MonoBehaviour
{
    public static Action<MouseState> OnMouseUpdate;

    private void Start()
    {
        MouseController.OnMouseUpdate(MouseState.HIDE);
    }

    private void OnEnable()
    {
        OnMouseUpdate += UpdateMouse;
    }

    private void OnDisable()
    {
        OnMouseUpdate -= UpdateMouse;
    }

    private static void UpdateMouse(MouseState state)
    {
        if (state == MouseState.SHOW)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (state == MouseState.HIDE)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
