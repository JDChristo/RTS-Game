using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Print
{
    public static void Log(string msg, Color? color = null)
    {
        color = color == null ? Color.black : color;
        Debug.Log($"<color={color}>{msg}</color>");
    }
}
