using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Utils
{

    public static class AnimationUtils
    {
        public static float GetAnimationLength(Animation anim, string name, bool basedOnSpeed = false)
        {
            try
            {
                if (basedOnSpeed)
                {
                    return anim[name].length / anim[name].speed;
                }
                return anim[name].length;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                return 0f;
            }
        }

        //public static float GetRelativeAnimationLength(Animation anim, string name, bool debug = false)
        //{
        //    try
        //    {
        //        if (debug)
        //            Debug.Log(anim.name + " - " +
        //                anim[name].length + " , " +
        //                anim[name].speed + " , " +
        //                anim[name].length / anim[name].speed + " , "
        //                );
        //        return anim[name].length / anim[name].speed;
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.Log(e);
        //        return 0f;
        //    }
        //}

        public static void PlayAnimation(string animationType,
        Animation avatar,
        bool singleVariation = true, int? attackPhase = null, bool playImediatly = false,
        Animation accessory = null,
        Animation accessory2 = null,
        string accessoryName = null,
        string accessory2Name = null,
        Animation accessory3 = null,
        Animation accessory4 = null,
        string accessory3Name = null,
        string accessory4Name = null)
        {
            string variation = string.Empty;
            if (singleVariation == false)
            {
                variation = "_" + attackPhase.Value;
            }

            if (playImediatly)
            {
                avatar.Play(animationType + "_Body" + variation);
                if (accessory != null)
                    accessory.Play(animationType + "_" + accessoryName + variation);
                if (accessory2 != null)
                    accessory2.Play(animationType + "_" + accessory2Name + variation);
                if (accessory3 != null)
                    accessory3.Play(animationType + "_" + accessory3Name + variation);
                if (accessory4 != null)
                    accessory4.Play(animationType + "_" + accessory4Name + variation);
            }
            else
            {
                avatar.CrossFade(animationType + "_Body" + variation);
                if (accessory != null)
                    accessory.CrossFade(animationType + "_" + accessoryName + variation);
                if (accessory2 != null)
                    accessory2.CrossFade(animationType + "_" + accessory2Name + variation);
                if (accessory3 != null)
                    accessory3.CrossFade(animationType + "_" + accessory3Name + variation);
                if (accessory4 != null)
                    accessory4.CrossFade(animationType + "_" + accessory4Name + variation);
            }
        }

        public static void SetAnimationOptions(
            ref Animation anim, string name
            , float speed
            , float weight

            )
        {
            anim[name].speed = speed;
            anim[name].weight = weight;
        }
    }
}
