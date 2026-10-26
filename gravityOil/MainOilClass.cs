using BepInEx;
using BepInEx.Logging;
using GameConsole;
using GameConsole.pcon;
using HarmonyLib;
using HarmonyLib.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using ULTRAKILL;
using Unity;
using UnityEngine;

namespace gravityOil
{
    [BepInPlugin("capybara7886.ultrakill.gravityOilShot", "Gravity Oil", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        //private ConfigBuilder config;
        //[Configgable("", "Mod Enabled", 0, null)]
        public static bool modEnabled = true;
        public StainVoxel stainAtPlayer;
        public OilGravityVolume oilGravController;
        
        private void Awake()
        {
            var harm = new Harmony("lakeullHarmony");
            harm.PatchAll();
            //config = new ConfigBuilder("Lakeull.gravityOilMod", "Gravity Oil");
            //config.BuildAll();
            //oilGravController = new OilGravityVolume(modEnabled);
        }
        // finds the player, gets pos, gets the stains at pos, and then updates gravity for each
        private void Update()
        {
            if (NewMovement.Instance != null)
            {
                //Vector3 pos = (NewMovement.Instance.Position + NewMovement.Instance.Rigidbody.GetGravityDirection() * 1.8333334f);
                Vector3 pos = new Vector3(NewMovement.Instance.transform.localPosition.x, NewMovement.Instance.transform.localPosition.y, NewMovement.Instance.transform.localPosition.z);
                //pos = new Vector3(pos.x - (NewMovement.Instance.rb.GetGravityVector().normalized.x), pos.y - NewMovement.Instance.rb.GetGravityVector().normalized.y, pos.z - NewMovement.Instance.rb.GetGravityVector().normalized.z);
                //Debug.LogError("new voxel pos: " + pos + " and " + NewMovement.Instance.rb.GetGravityVector().normalized.x);
                // get the stainVoxel at the players position
                stainAtPlayer = StainVoxelManager.Instance.CreateOrGetVoxel(pos, true);
                if (stainAtPlayer == null)
                {
                    //Debug.LogWarning("no stain");
                    return;
                }
                // differentiate between static and nonstatic proxy
                if (stainAtPlayer.staticProxy != null)
                {
                    // get the voxel proxy 
                    VoxelProxy proxyAtPlayer = stainAtPlayer.staticProxy;
                    // list all gas shits
                    List<GasolineStain> gasList = proxyAtPlayer.stains;
                    //Debug.LogError(gasList.Count);
                    for (var i = 0; i < gasList.Count; i++)
                    {
                        OilGravityVolume oilVolumeItem = gasList[i].gameObject.GetOrAddComponent<OilGravityVolume>();
                        oilVolumeItem.GetCanChange(false);
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
    // turns floor always on, making the  walls slippy
    [HarmonyPatch(typeof(GasolineStain), nameof(GasolineStain.IsFloor), MethodType.Getter)]
    class StainPatch
    {
        static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }

    // gets the gasoline stain and assigns the custom components
    [HarmonyPatch(typeof(RocketLauncher), nameof(RocketLauncher.ShootNapalm))]
    class StainGetter
    {
        public static bool addedStain = false;
        static void Postfix(RocketLauncher __instance)
        {
            //Debug.LogError("stain getter success" + addedStain);
            if (addedStain == false)
            {
                GameObject stainObj = __instance.napalmProjectile.GetComponent<GasolineProjectile>().stain.gameObject;
                //Debug.LogError(stainObj.name);
                if (stainObj != null && stainObj.GetComponent<OilGravityVolume>() == null)
                {
                    //Debug.LogError("attempting add component to " + stainObj.name);
                    stainObj.AddComponent<OilGravityVolume>();
                    addedStain = true;
                }
            }
        }
    }
}