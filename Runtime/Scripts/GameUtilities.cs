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

        public static class PlayerPrefs
        {
            public static string isSFXMuted = "isSFXMuted";
            public static string isMusicMuted = "isMusicMuted";
            public static string SFXVolume = "sfxVolume";
            public static string MusicVolume = "musicVolume";
        }
    }
}
