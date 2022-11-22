using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public float translationSpeed = 60f;
    public float altitude = 40f;
    public float zoomSpeed = 30f;
    
    private Camera camera;
    private RaycastHit hit;
    private Ray ray;
    
    private Vector3 forwardDir;
    private int mouseOnScreenBorder;
    
    private Coroutine mouseOnScreenCoroutine;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        forwardDir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        mouseOnScreenBorder = -1;
        mouseOnScreenCoroutine = null;
    }
    
    private void Update()
    {
        if (mouseOnScreenBorder >= 0)
        {
            _TranslateCamera(mouseOnScreenBorder);
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
                _TranslateCamera(0);
            else if (Input.GetKey(KeyCode.RightArrow))
                _TranslateCamera(1);
            else if (Input.GetKey(KeyCode.DownArrow))
                _TranslateCamera(2);
            else if (Input.GetKey(KeyCode.LeftArrow))
                _TranslateCamera(3);
        }
        
        if (Math.Abs(Input.mouseScrollDelta.y) > 0f)
            _Zoom(Input.mouseScrollDelta.y > 0f ? -1 : 1);
    }
    
    private void _TranslateCamera(int dir)
    {
        switch (dir)
        {
            case 0:
                transform.Translate(forwardDir * (Time.deltaTime * translationSpeed), Space.World);
                break;
            case 1:
                transform.Translate(transform.right * (Time.deltaTime * translationSpeed));
                break;
            case 2:
                transform.Translate(-forwardDir * (Time.deltaTime * translationSpeed), Space.World);
                break;
            case 3:
                transform.Translate(-transform.right * (Time.deltaTime * translationSpeed));
                break;
        }
        
        ray = new Ray(transform.position, Vector3.up * -1000f);
        
        if (Physics.Raycast(ray, out hit, 1000f, Globals.TERRAIN_LAYER_MASK))
        {
            transform.position = hit.point + Vector3.up * altitude;
        }
    }
    
    private void _Zoom(int zoomDir)
    {
        camera.orthographicSize += zoomDir * Time.deltaTime * zoomSpeed;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 18f, 36f);
    }
    
    public void OnMouseEnterScreenBorder(int borderIndex)
    {
        mouseOnScreenCoroutine = StartCoroutine(_SetMouseOnScreenBorder(borderIndex));
    }

    public void OnMouseExitScreenBorder()
    {
        StopCoroutine(mouseOnScreenCoroutine);
        mouseOnScreenBorder = -1;
    }
    
    private IEnumerator _SetMouseOnScreenBorder(int borderIndex)
    {
        yield return new WaitForSeconds(0.3f);
        mouseOnScreenBorder = borderIndex;
    }
}
