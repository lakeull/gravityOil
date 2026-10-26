using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKILL;
using System.ComponentModel;

namespace gravityOil
{
    public class OilGravityVolume : MonoSingleton<OilGravityVolume>
    {
        private NewMovement nm;
        private Vector3 upSim;
        //public bool modEnabled;
        public OilGravityVolume()
        {
            nm = NewMovement.Instance;
        }

        private void Awake()
        {
            Debug.Log("Oil Gravity Volume Mod Loaded!");
            GetCanChange(true);
        }

        // updates the player's gravity
        public void UpdateGravity()
        {
            upSim = new Vector3(Mathf.Round(gameObject.transform.forward.x * 10.0f)*4f, Mathf.Round(gameObject.transform.forward.y * 10.0f) * 4.0f, Mathf.Round(gameObject.transform.forward.z * 10.0f) * 4.0f);
            if (upSim.Equals(nm.rb.GetGravityVector())) { return; }
            nm.SwitchGravity(upSim, false, false);
            //Debug.LogError(upSim);
        }
        

        //!!!!!! test this
        
        // chhanges gravity if the player is in front of the object and is close enough. 
        public void GetCanChange(bool onCreation)
        {
            //if(modEnabled == false) { return; }
            Vector3 dir = transform.position - nm.transform.position;
            //Debug.LogError(dir);
            float dotProduct = Vector3.Dot(nm.transform.forward, dir);
            bool inFront = dotProduct > 0.0f;
            bool close = 3.5f >= Vector3.Distance(nm.transform.position, transform.position);
            //Debug.LogError(inFront + " " + close);
            if (inFront && (close || !onCreation))
            {
                Invoke(nameof(UpdateGravity), 0.1f);
            }
        }
    }
}
