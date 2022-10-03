using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Wheel[] turningWheels;
    public Wheel[] drivingWheels;
    public Rigidbody rb;
    public float TopSpeed = 10;
    
    public AnimationCurve PowerCurve;
    public float EnginePower = 5;

    void Awake(){
        foreach(Wheel wheel in turningWheels){
            wheel.myCar= this;
        }

        foreach(Wheel wheel in drivingWheels){
            wheel.myCar= this;
        }
        

    }

    void FixedUpdate()
    {
            //up or down
            foreach(Wheel wheel in drivingWheels){
                wheel.Rotate(Input.GetAxis("Vertical"));
            }
        

        foreach(Wheel wheel in turningWheels){
            wheel.Steer(Input.GetAxis("Horizontal"));
        }
    }
}
