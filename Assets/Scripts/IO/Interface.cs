using UnityEditor;

namespace IO
{
    public static class PlatformUtil
    {
        public static string GetPlatformName()
        {
            string platformSubDir = string.Empty;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            platformSubDir = "Windows";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		platformSubDir = "Mac";
#elif UNITY_STANDALONE_LINUX
		platformSubDir = "Linux";
#elif UNITY_XBOXONE
		platformSubDir = "XBoxOne";
#elif UNITY_IOS || UNITY_TVOS
		platformSubDir = "iOS";
#elif UNITY_ANDROID
		platformSubDir = "Android";
#elif PLATFORM_LUMIN
		platformSubDir = "Lumin";
#elif UNITY_PS4
		platformSubDir = "PS4";
#elif UNITY_WP_8_1
		platformSubDir = "WindowsPhone";
#elif UNITY_SWITCH
		platformSubDir = "Switch";
#elif UNITY_PSP2
#if AK_ARCH_VITA_SW || !AK_ARCH_VITA_HW
		platformSubDir = "VitaSW";
#else
		platformSubDir = "VitaHW";
#endif
#else
            platformSubDir = "Undefined platform sub-folder";
#endif
            return platformSubDir;
        }
        public static string GetPlatformNameByBuildTarget(BuildTarget target)
        {
            var platformSubDir = string.Empty;
            //        GetCustomPlatformName(ref platformSubDir, target);
            if (!string.IsNullOrEmpty(platformSubDir))
                return platformSubDir;

            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";

                case BuildTarget.iOS:
                case BuildTarget.tvOS:
                    return "iOS";

                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "Linux";

#if UNITY_2017_3_OR_NEWER
			case BuildTarget.StandaloneOSX:
#else
                    //case BuildTarget.StandaloneOSXIntel:
                    //case BuildTarget.StandaloneOSXIntel64:
                    //case BuildTarget.StandaloneOSXUniversal:
#endif
                    return "Mac";

                case (BuildTarget)39: // BuildTarget.Lumin
                    return "Lumin";

                case BuildTarget.PS4:
                    return "PS4";

                case BuildTarget.PSP2:
                    return "Vita";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.WSAPlayer:
                    return "Windows";

                case BuildTarget.XboxOne:
                    return "XboxOne";

                case BuildTarget.Switch:
                    return "Switch";
            }

            return target.ToString();
        }
    }
}