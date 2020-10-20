using System;
using System.IO;

namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public static class DirectoryHelper
    {
        public static string GetDirectorySeparatorChar(PlatformID platformID)
        {
            switch (platformID)
            {
                case PlatformID.Win32NT:
                    return "\\";
                case PlatformID.Unix:
                    return "/";
                default:
                    return Path.DirectorySeparatorChar.ToString();
            }
        }
    }
}