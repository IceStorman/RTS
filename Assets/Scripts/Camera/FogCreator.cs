using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class FogCreator : MonoBehaviour
{
    [SerializeField] private GameObject fogOfWarPlane;
    [SerializeField] private LayerMask fogOfWarMask;
    
    private Mesh _mesh;
    private Vector3[] _verticles;
    private Color[] _colors;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        _verticles = _mesh.vertices;
        _colors = new Color[_verticles.Length];

        for (int i = 0; i < _colors.Length; i++)
        {
            _colors[i] = Color.black;
        }
        
        UpdateColor();
    }

    private void UpdateColor()
    {
        _mesh.colors = _colors;
    }

    private void Update()
    {
        foreach (var entity in Globals.SUMMONED_ENTITIES)
        {
            var position = transform.position;
            Ray ray = new(position, entity.Transform.position - position);

            if (Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    1000f,
                    fogOfWarMask,
                    QueryTriggerInteraction.Collide))
            {
                for (int i = 0; i < _verticles.Length; i++)
                {
                    Vector3 v = fogOfWarPlane.transform.TransformPoint(_verticles[i]);
                    float dist = Vector3.SqrMagnitude(v - hit.point);

                    if (dist < entity.Data.fieldOfView * entity.Data.fieldOfView)
                    {
                        //Debug.Log($"Alpha: {_colors[i].a}, Dist: {dist}, viewSqr: {entity.Data.fieldOfView * entity.Data.fieldOfView}");
                        //Debug.Log(_colors[i].a + ", " + dist / entity.Data.fieldOfView * entity.Data.fieldOfView);
                        float alpha = Mathf.Min(_colors[i].a, dist / (entity.Data.fieldOfView * entity.Data.fieldOfView));
                        _colors[i].a = alpha;
                    }
                }
                UpdateColor();
            }
        }
    }
}
