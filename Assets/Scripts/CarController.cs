using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Wheel[] turningWheels;
    public Wheel[] drivingWheels;
    public List<Wheel> allWheels;
    public Rigidbody rb;
    public float TopSpeed = 10;
    public float Deceleration;
    public float BrakePower = 3;
    
    public AnimationCurve PowerCurve;
    public float EnginePower = 5;
    public float curSpeed;

    [Header("Steering")]
    public float maxSteerAngle = 25;
    public float minSteerAngle = 10;

    void Awake(){
        foreach(Wheel wheel in turningWheels){
            wheel.myCar= this;
            if(!allWheels.Contains(wheel)){allWheels.Add(wheel);}
        }

        foreach(Wheel wheel in drivingWheels){
            wheel.myCar= this;
            if(!allWheels.Contains(wheel)){allWheels.Add(wheel);}
        }
    }

    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.R)){transform.eulerAngles = new Vector3(0,0,0);}
        curSpeed = Vector3.Dot(transform.forward, rb.velocity);
        float verticalInput = Input.GetAxis("Vertical");
        
        if((verticalInput >0 && curSpeed < 0) || (verticalInput <0 && curSpeed > 0)){
            foreach(Wheel wheel in allWheels){
                wheel.Brake();
            }
        }

        foreach(Wheel wheel in drivingWheels){
            wheel.Rotate(verticalInput);
        }        

        foreach(Wheel wheel in turningWheels){
            wheel.Steer(Input.GetAxis("Horizontal"));
        }
    }
}
