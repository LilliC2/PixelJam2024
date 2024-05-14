using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LC
{
    public class Behaviour : MonoBehaviour
    {
        /// <summary>
        /// Generates a random colour from all possible colours with an alpha of 1
        /// </summary>
        /// <returns>A random colour</returns>
        public Color GetRandomColour()
        {
            return new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        }

        /// <summary>
        /// Scales Game Object to zero  
        /// </summary>
        /// <param name="gameObject">The Game Object we want to scale</param>
        public void ScaleToZero(GameObject gameObject)
        {
            gameObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Get random float between two numbers
        /// </summary>
        /// <param name="num1">The min</param>
        /// <param name="num2">The max</param>
        /// <returns>Returns random float</returns>
        public float RandomFloatBetweenTwoFloats(float num1, float num2)
        {
            return UnityEngine.Random.Range(num1, num2);

        }

        /// <summary>
        /// Get random float between two numbers
        /// </summary>
        /// <param name="num1">The min</param>
        /// <param name="num2">The max</param>
        /// <returns>Returns random float</returns>
        public int RandomIntBetweenTwoInt(int num1, int num2)
        {
            return UnityEngine.Random.Range(num1, num2);

        }

        #region Coroutine Helpers

        /// <summary>
        /// Executes the Action block as a Coroutine on the next frame.
        /// </summary>
        /// <param name="func">The Action block</param>
        protected void ExecuteNextFrame(Action func)
        {
            StartCoroutine(ExecuteAfterFramesCoroutine(1, func));
        }
        /// <summary>
        /// Executes the Action block as a Coroutine after X frames.
        /// </summary>
        /// <param name="func">The Action block</param>
        protected void ExecuteAfterFrames(int frames, Action func)
        {
            StartCoroutine(ExecuteAfterFramesCoroutine(frames, func));
        }
        private IEnumerator ExecuteAfterFramesCoroutine(int frames, Action func)
        {
            for (int f = 0; f < frames; f++)
                yield return new WaitForEndOfFrame();
            func();
        }
        /// <summary>
        /// Executes the Action block as a Coroutine after X seconds
        /// </summary>
        /// <param name="seconds">Seconds.</param>
        protected void ExecuteAfterSeconds(float seconds, Action func)
        {
            if (seconds <= 0f)
                func();
            else
                StartCoroutine(ExecuteAfterSecondsCoroutine(seconds, func));
        }
        private IEnumerator ExecuteAfterSecondsCoroutine(float seconds, Action func)
        {
            yield return new WaitForSeconds(seconds);
            func();
        }
        /// <summary>
        /// Executes the Action block as a Coroutine whern a condition is met
        /// </summary>
        /// <param name="condition">The Condition block</param>
        /// <param name="func">The Action block</param>
        protected void ExecuteWhenTrue(Func<bool> condition, Action func)
        {
            StartCoroutine(ExecuteWhenTrueCoroutine(condition, func));
        }
        private IEnumerator ExecuteWhenTrueCoroutine(Func<bool> condition, Action func)
        {
            while (condition() == false)
                yield return new WaitForEndOfFrame();
            func();
        }
    }
}


        #endregion
