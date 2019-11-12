﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AIR_API
{
    public class Recording
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string FormalName { get; set; }
        public string AIRVersion { get; set; } = "N/A";

        public override string ToString()
        {
            return string.Format("{0} -- [A.I.R. {1}]", this.Name, this.AIRVersion);
        }
        public string GetRAW()
        {
            var binData = File.ReadAllBytes(FilePath);
            var sb = new StringBuilder();
            foreach (var b in binData)
                sb.Append(" " + b.ToString("X2"));
            return sb.ToString();
        }

        public Recording(FileInfo file)
        {
            FilePath = file.FullName;

            byte[] byteArray = File.ReadAllBytes(FilePath).Skip(4).Take(10).ToArray();
            AIRVersion = System.Text.Encoding.UTF8.GetString(byteArray);
            

            string baseString = file.Name.Replace("gamerecording_", "");

            string month = baseString.Substring(2, 2);
            string day = baseString.Substring(4, 2);
            string year = baseString.Substring(0, 2);

            string hour = baseString.Substring(7, 2);
            string minute = baseString.Substring(9, 2);
            string second = baseString.Substring(11, 2);

            string recordingFormat = $"{month}/{day}/{year} - {hour}.{minute}.{second}";

            Name = recordingFormat;

            FormalName = $"Sonic 3 AIR Recording [{recordingFormat}] ";
        }



    }
}