using System;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public RectTransform rectTransform;

    private Transform _target;
    private Vector3 _lastTargetPosition;
    private Vector2 _pos;

    private Transform camera;
    private Vector3 lastCameraPosition;
    private float lastOrthographicSize;

    private float _yOffset;

    private void Awake()
    {
        camera = Camera.main.transform;
    }

    private void Update()
    {
        if (lastCameraPosition == camera.position 
            && lastOrthographicSize == camera.GetComponent<Camera>().orthographicSize 
            && _target && _lastTargetPosition == _target.position) return;
        SetPosition();
    }

    public void Initialize(Transform target, float yOffSet)
    {
        _target = target;
        _yOffset = yOffSet;
    }

    public void SetPosition()
    {
        if (!_target) return;
        _pos = Camera.main.WorldToScreenPoint(_target.position);
        _pos.y += _yOffset;
        rectTransform.anchoredPosition = _pos;
        _lastTargetPosition = _target.position;
        
        lastCameraPosition = camera.position;
        lastOrthographicSize = camera.GetComponent<Camera>().orthographicSize;
    }
}
