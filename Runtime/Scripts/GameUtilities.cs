using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Core
{
    public class GameUtilities : MonoBehaviour
    {
        public static class WaitTimers
        {
            public static WaitForEndOfFrame waitForEndFrame = new WaitForEndOfFrame();

            public static WaitForSeconds waitForOneSecond = new WaitForSeconds(1.0F);

            public static WaitForSeconds waitForPointFive = new WaitForSeconds(0.5F);
        }
    }
}
