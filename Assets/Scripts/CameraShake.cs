using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake cameraInstance;    

    [SerializeField]
    private Transform _camTransform;
    private Vector3 _camStartPos;    
    private float _shakeTime = 0f;
    private float _shakeIntensity = 0.3f;
  
    // Start is called before the first frame update
    private void Awake()
    {
        if (cameraInstance == null)
        {
            cameraInstance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this);
        }    
    }

    void Start()
    {
        _camStartPos = _camTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
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
