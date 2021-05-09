// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace BurstLines
{
    public static partial class Extensions
    {
        public const float EPSILON = 0.00001f;

        public static T ValidateReference<T>(this MonoBehaviour sender, T component) where T : Component
        {
            if (component == null)
            {
                component = sender.GetComponentInChildren<T>();
                if (component == null)
                {
                    component = sender.gameObject.AddComponent<T>();
                }
            }
            return component;
        }

        public static void DestroySafe(UnityEngine.Object @object)
        {
            if (@object != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(@object);
                }
                else
                {
                    GameObject.DestroyImmediate(@object);
                }
            }
        }
    }
}