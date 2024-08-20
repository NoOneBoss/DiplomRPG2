using System;
using System.Collections;
using UnityEngine;

namespace Other
{
    public static class Schedulers
    {
        public static IEnumerator ExecuteAfterTime(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }
        
        public static IEnumerator RepeatWithInterval(float interval, Action action)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                action();
            }
        }
    }
}