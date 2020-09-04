using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystick : Singleton<VirtualJoystick>
{
    [SerializeField]
    float virtualJoystickSize = 100.0f;

    [Header("Virtual Joystick Outer Circle")]
    [SerializeField]
    private Sprite outerCircleSprite = null;
    [SerializeField]
    private Color outerCircleColor = Color.white;

    [Header("Virtual Joystick Inner Circle")]
    [SerializeField]
    private Sprite innerCircleSprite = null;
    [SerializeField]
    private Color innerCircleColor = Color.white;
    [SerializeField]
    [Range(0.1f, 0.9f)]
    private float innerCircleScale = 0.25f;

    [Header("Virtual Joystick Deadzone Circle")]
    [SerializeField]
    private Sprite deadZoneCircleSprite = null;
    [SerializeField]
    private Color deadZoneCircleColor = Color.white;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float deadZoneCircleScale = 0.0f;

    private GameObject outerCircle;
    private GameObject innerCircle;
    private GameObject deadZoneCircle;

    private Vector3 inputStartPosition;
    private float outerCircleRadius;

    public Vector3 NormalizedDir { get; private set; }

    public float NormalizedMagnitude { get; private set; }

    public bool IsActivated { get; private set; }

    public bool CanBeActivated { private get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        if(virtualJoystickSize == 0.0f)
        {
            virtualJoystickSize = 100.0f;
            Debug.LogWarning("Virtual Joystick Size cannot be 0! Defaulting to size 100.");
        }

        //Outer Circle
        outerCircle = transform.GetChild(0).gameObject;
        if(!outerCircle)
        {
            Debug.LogError("Outer virtual joystick circle not found!");
        }
        Image outerCircleImage = outerCircle.GetComponent<Image>();
        outerCircleImage.rectTransform.sizeDelta = new Vector2(virtualJoystickSize, virtualJoystickSize);
        outerCircleRadius = virtualJoystickSize * 0.5f;
        if (outerCircleSprite)
        {
            outerCircleImage.sprite = outerCircleSprite;
        }
        outerCircleImage.color = outerCircleColor;
        outerCircle.SetActive(false);

        //Inner Circle
        innerCircle = outerCircle.transform.GetChild(0).gameObject;
        if(!innerCircle)
        {
            Debug.LogError("Inner virtual joystick circle not found!");
        }
        innerCircle.transform.localScale = new Vector3(innerCircleScale, innerCircleScale, innerCircleScale);

        Image innerCircleImage = innerCircle.GetComponent<Image>();
        innerCircleImage.rectTransform.sizeDelta = new Vector2(virtualJoystickSize, virtualJoystickSize);
        if (innerCircleSprite)
        {
            innerCircleImage.sprite = innerCircleSprite;
        }
        innerCircleImage.color = innerCircleColor;

        //Deadzone Circle
        deadZoneCircle = outerCircle.transform.GetChild(1).gameObject;
        if (!deadZoneCircle)
        {
            Debug.LogError("Deadzone virtual joystick circle not found!");
        }
        deadZoneCircle.transform.localScale = new Vector3(deadZoneCircleScale, deadZoneCircleScale, deadZoneCircleScale);
        Image deadZoneCircleImage = deadZoneCircle.GetComponent<Image>();
        deadZoneCircleImage.rectTransform.sizeDelta = new Vector2(virtualJoystickSize, virtualJoystickSize);
        if (deadZoneCircleSprite)
        {
            deadZoneCircleImage.sprite = deadZoneCircleSprite;
        }
        deadZoneCircleImage.color = deadZoneCircleColor;

        NormalizedMagnitude = 0.0f;

        CanBeActivated = true;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN
        if(CanBeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                inputStartPosition = Input.mousePosition;
                SetOuterCircle(true, inputStartPosition);
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                VirtualJoystickBehaviour(Input.mousePosition);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                SetInnerCircle(inputStartPosition);
                SetOuterCircle(false, inputStartPosition);
            }
#endif
#if UNITY_ANDROID
            if(Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    inputStartPosition = Input.touches[0].position;
                    SetOuterCircle(true, inputStartPosition);
                }
                if(touch.phase == TouchPhase.Moved)
                {
                    VirtualJoystickBehaviour(Input.touches[0].position);
                
                }
                if(touch.phase == TouchPhase.Ended)
                {
                    SetInnerCircle(inputStartPosition);
                    SetOuterCircle(false, inputStartPosition);
                }
            }
#endif
        }
    }

    public void ResetVirtualJoystick()
    {
        NormalizedDir = Vector3.zero;
        NormalizedMagnitude = 0.0f;
    }

    private void SetOuterCircle(bool active, Vector3 position)
    {
        IsActivated = active;
        outerCircle.SetActive(active);
        outerCircle.transform.position = position;
    }

    private void SetInnerCircle(Vector3 position)
    {
        innerCircle.transform.position = position;
    }

    private void VirtualJoystickBehaviour(Vector3 inputCurrPosition)
    {
        NormalizedDir = (inputCurrPosition - inputStartPosition).normalized;
        float sqrMagnitude = (inputCurrPosition - inputStartPosition).sqrMagnitude;
        float circleSqrRadius = outerCircleRadius * outerCircleRadius;

        float radiusOutsideDZ = outerCircleRadius * deadZoneCircleScale;
        float sqrRadiusOutsideDZ = radiusOutsideDZ * radiusOutsideDZ;

        if (sqrMagnitude < circleSqrRadius)
        {
            NormalizedMagnitude = Mathf.InverseLerp(sqrRadiusOutsideDZ, circleSqrRadius, sqrMagnitude);
            SetInnerCircle(inputCurrPosition);
        }
        else
        {
            NormalizedMagnitude = 1.0f;
            SetInnerCircle(inputStartPosition + (NormalizedDir * outerCircleRadius));
        }
    }
}
