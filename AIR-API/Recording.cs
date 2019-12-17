using System;
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


            byte[] header = File.ReadAllBytes(FilePath).Take(3).ToArray();
            if (header[0] == 0x47 && header[1] == 0x52 && header[2] == 0x43)
            {
                byte[] byteArray = File.ReadAllBytes(FilePath).Skip(4).Take(10).ToArray();
                AIRVersion = System.Text.Encoding.UTF8.GetString(byteArray);


                string baseString = file.Name.Replace("gamerecording_", "");

                string month = (int.TryParse(baseString.Substring(2, 2), out int m) ? m.ToString() : "?");
                string day = (int.TryParse(baseString.Substring(4, 2), out int d) ? d.ToString() : "?");
                string year = (int.TryParse(baseString.Substring(0, 2), out int y) ? y.ToString() : "?");

                string hour = (int.TryParse(baseString.Substring(7, 2), out int h) ? d.ToString() : "?");
                string minute = (int.TryParse(baseString.Substring(9, 2), out int M) ? M.ToString() : "?");
                string second = (int.TryParse(baseString.Substring(11, 2), out int s) ? s.ToString() : "?");

                string recordingFormat = $"{month}/{day}/{year} - {hour}.{minute}.{second}";

                if (recordingFormat.Contains("?"))
                {
                    recordingFormat = $"?/?/? - ?.?.?";
                }

                Name = recordingFormat;

                FormalName = $"Sonic 3 AIR Recording [{recordingFormat}] ";
            }
            else
            {
                throw new Exception();
            }

        }



    }
}
