using System;
using UnityEngine;
using Newtonsoft.Json;
using Photon.Pun;
using System.Text;
using ExitGames.Client.Photon;

public static class Utils
{
    static Texture2D _whiteTexture;

    private static string buildingDataJSON;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public static Rect GetBoundingBoxOnScreen(Bounds bounds, Camera camera)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;
        Vector3[] vertices = new Vector3[] {
            center + Vector3.right * size.x / 2f + Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center + Vector3.right * size.x / 2f + Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
            center + Vector3.right * size.x / 2f - Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center + Vector3.right * size.x / 2f - Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f + Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f + Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f - Vector3.up * size.y / 2f + Vector3.forward * size.z / 2f,
            center - Vector3.right * size.x / 2f - Vector3.up * size.y / 2f - Vector3.forward * size.z / 2f,
        };
        Rect retVal = Rect.MinMaxRect(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = camera.WorldToScreenPoint(vertices[i]);
            if (v.x < retVal.xMin)
                retVal.xMin = v.x;
            if (v.y < retVal.yMin)
                retVal.yMin = v.y;
            if (v.x > retVal.xMax)
                retVal.xMax = v.x;
            if (v.y > retVal.yMax)
                retVal.yMax = v.y;
        }

        return retVal;
    }

    public static int GetAlphaKeyValue(string inputString)
    {
        if (inputString == "1" || inputString == "&") return 1;
        if (inputString == "2" || inputString == "?") return 2;
        if (inputString == "3" || inputString == "\"") return 3;
        if (inputString == "4" || inputString == "'") return 4;
        if (inputString == "5" || inputString == "(") return 5;
        if (inputString == "6" || inputString == "&") return 6;
        if (inputString == "7" || inputString == "?") return 7;
        if (inputString == "8" || inputString == "!") return 8;
        if (inputString == "9" || inputString == "?") return 9;
        return -1;
    }

    public static byte[] Serialize(object obj)
    {
        buildingDataJSON = JsonConvert.SerializeObject((Building)obj);
        Debug.Log(buildingDataJSON);
        byte[] dataByteArray = Encoding.ASCII.GetBytes(buildingDataJSON);
        return dataByteArray;
    }

    public static object Deserialize(byte[] bytes)
    {
        byte[] dataBytes = new byte[bytes.Length - 4];
        if (buildingDataJSON.Length > 0)
        {
            Array.Copy(bytes, 4, dataBytes, 0, dataBytes.Length);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(dataBytes);
            buildingDataJSON = Encoding.UTF8.GetString(dataBytes);
        }
        else
        {
            buildingDataJSON = string.Empty;
        }
        Building building = (Building)JsonConvert.DeserializeObject(buildingDataJSON);
        return building;
    }
}
