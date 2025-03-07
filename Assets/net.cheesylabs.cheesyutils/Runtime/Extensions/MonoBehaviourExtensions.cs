using System;
using System.Collections;
using UnityEngine;

namespace CheesyUtils
{
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Invokes a method on a MonoBehaviour after a specified delay.
        /// </summary>
        /// <param name="behavior">The MonoBehaviour to invoke the method on.</param>
        /// <param name="method">Method to invoke.</param>
        /// <param name="delayInSeconds">Delay in seconds.</param>
        public static void InvokeSafe(this MonoBehaviour behavior, Action method, float delayInSeconds)
        {
            behavior.StartCoroutine(InvokeSafeRoutine(method, delayInSeconds));
        }
        
        /// <summary>
        /// Invokes a method on a MonoBehaviour repeatedly after a specified delay.
        /// </summary>
        /// <param name="behavior">The MonoBehaviour to invoke the method on.</param>
        /// <param name="method">Method to invoke.</param>
        /// <param name="delayInSeconds">Delay in seconds.</param>
        /// <param name="repeatRateInSeconds">Repeat rate in seconds.</param>
        public static void InvokeRepeatingSafe(this MonoBehaviour behavior, Action method, float delayInSeconds, float repeatRateInSeconds)
        {
            behavior.StartCoroutine(InvokeSafeRepeatingRoutine(method, delayInSeconds, repeatRateInSeconds));
        }
        
        /// <summary>
        /// Invokes a method on a MonoBehaviour repeatedly after a specified delay.
        /// </summary>
        /// <param name="method">Method to invoke.</param>
        /// <param name="delayInSeconds">Delay in seconds.</param>
        /// <param name="repeatRateInSeconds">Repeat rate in seconds.</param>
        private static IEnumerator InvokeSafeRepeatingRoutine(Action method, float delayInSeconds, float repeatRateInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);

            while (true)
            {
                if (method != null) method.Invoke();
                yield return new WaitForSeconds(repeatRateInSeconds);
            }
        }
        
        /// <summary>
        /// Invokes a method on a MonoBehaviour after a specified delay.
        /// </summary>
        /// <param name="method">Method to invoke.</param>
        /// <param name="delayInSeconds">Delay in seconds.</param>
        private static IEnumerator InvokeSafeRoutine(Action method, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            if (method != null) method.Invoke();
        }
    }
}
