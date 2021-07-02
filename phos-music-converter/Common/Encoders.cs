// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Encoders
    {
        private static readonly string AdpcmPath = $@"{Directory.GetCurrentDirectory()}\dependencies\AdpcmEncode.exe";

        public static string GetAdpcmPath()
        {
            if (!File.Exists(AdpcmPath))
            {
                throw new FileNotFoundException("AdpcmEncode.exe could not be found!", AdpcmPath);
            }
            else
            {
                return AdpcmPath;
            }
        }
    }
}