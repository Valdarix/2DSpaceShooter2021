using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake CameraInstance;    

    [SerializeField] private Transform _camTransform;
    private Vector3 _camStartPos;    
    private float _shakeTime = 0f;
    private readonly float _shakeIntensity = 0.3f;
  
    // Start is called before the first frame update
    private void Awake()
    {
        if (CameraInstance == null)
        {
            CameraInstance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this);
        }    
    }

    private void Start()
    {
        _camStartPos = _camTransform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_shakeTime > 0)
        {
            _camTransform.localPosition  = _camStartPos + (Random.insideUnitSphere * _shakeIntensity);
            _shakeTime -= Time.deltaTime;
        }
        else
        {         
            _shakeTime = 0;
            _camTransform.position = _camStartPos;
        }
    }
    public void ShakeCamera()
    {   
        _shakeTime = 0.5f;
    }
}
