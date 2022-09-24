using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Wheel[] wheels;
    public Rigidbody rb;

    void Awake(){
        foreach(Wheel wheel in wheels){
            wheel.myCar= this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
