using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float radius =0.35f;
    public float defaultPosition;
    public float springConstant =1f;
    public ForceMode springForceMode;
    public float displacementRange = 0.1f;
    public float dampningSpeed =0.1f;
    public float hitPoint=0;
    public float curDisplacement => defaultPosition- transform.localPosition.y;
    public float minDisplacement => defaultPosition + displacementRange;
    public float maxDisplacement => defaultPosition - displacementRange; 
    LayerMask groundMask;

    public CarController myCar;
    
    void Awake()
    {
        defaultPosition=transform.localPosition.y;
        groundMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(myCar==null){return;}

        RaycastHit hit = new RaycastHit();
        float newDisplacement = transform.localPosition.y;
        float bottomEnd = transform.position.y - radius;
        // hitPoint=0;
        bool grounded=false;

        // if(Physics.Linecast(transform.position+new Vector3(0,0,-0.1f), new Vector3(transform.position.x,bottomEnd,transform.position.z-0.1f), out hit, groundMask)){
        //     Debug.DrawLine(transform.position, hit.point, Color.red);
            
        //     hitPoint=hit.point.y;
        //     grounded=true;
        // }
        if(Physics.Linecast(transform.position, new Vector3(transform.position.x,bottomEnd,transform.position.z), out hit, groundMask)){
            Debug.DrawLine(transform.position, hit.point, Color.red);
            // if(hit.point.y > hitPoint){
                hitPoint=hit.point.y;
            // }
            
            grounded=true;
        }
        // if(Physics.Linecast(transform.position+new Vector3(0,0,0.1f), new Vector3(transform.position.x,bottomEnd,transform.position.z+0.1f), out hit, groundMask)){
        //     Debug.DrawLine(transform.position, hit.point, Color.red);
        //     if(hit.point.y > hitPoint){
        //         hitPoint=hit.point.y;
        //     }
            
        //     grounded=true;
        // }
        
        if(!grounded){
            newDisplacement = maxDisplacement;
            Debug.Log("Not Touching ground ");
        }else{
            newDisplacement=hitPoint+radius;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,newDisplacement, transform.position.z),dampningSpeed);
        if(curDisplacement < 0){
            //Apply upforce
        }
            myCar.rb.AddForceAtPosition(-Physics.gravity*springConstant,transform.position,springForceMode);

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        
        int segmants = 12;

        Vector3 lastPoint= Vector3.zero;
        for(int i=0; i< segmants;i++){
            float theta = ((Mathf.PI*2)/segmants) * i;
            float z= transform.position.z + (radius * Mathf.Sin(theta));
            float y= transform.position.y +(radius * Mathf.Cos(theta));
            Vector3 thisPoint = new Vector3(transform.position.x,y,z);
            
            
            if(i>0){Gizmos.DrawLine(lastPoint,thisPoint);}
            lastPoint = thisPoint;
        }

        Gizmos.DrawWireSphere(new Vector3(transform.position.x,hitPoint,transform.position.z),0.2f);
    }
}
