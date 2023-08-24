using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AIR_API
{
    public class Animation
    {
        private Anim Instance;

        public List<Anim.Frame> FrameList { get => Instance.FrameList; set => Instance.FrameList = value; }
        public string Directory { get => Instance.Directory; set => Instance.Directory = value; }
        public string FileLocation { get => Instance.FileLocation; set => Instance.FileLocation = value; }

        public Animation(FileInfo file)
        {
            try
            {
                Instance = new Anim(file);
            }
            catch (Exception ex)
            {
                Instance = null;
            }
        }

        public Animation(string _directory, string _fileLocation)
        {
            try
            {
                Instance = new Anim(_directory, _fileLocation);
            }
            catch (Exception ex)
            {
                Instance = null;
            }
        }

        public void Save(string SaveAsLocation = "")
        {
            try
            {
                Instance.Save(SaveAsLocation);
            }
            catch (Exception ex)
            {
            }
        }

        public class Anim
        {
            private string nL = Environment.NewLine;
            public List<Frame> FrameList = new List<Frame>();
            public string Directory;
            public string FileLocation;

            public Anim(FileInfo file)
            {
                Directory = file.Directory.FullName;
                FileLocation = file.FullName;
                string data = File.ReadAllText(file.FullName);
                JToken stuff = JRaw.Parse(data);

                foreach (var child in stuff.Children())
                {
                    string _name = "";
                    string _file = "";
                    int? _x = null;
                    int? _y = null;
                    int? _width = null;
                    int? _height = null;
                    int? _center_x = null;
                    int? _center_y = null;

                    if (child.HasValues)
                    {
                        _name = child.Path;
                        foreach (JProperty content in child.Children().Children())
                        {
                            if (content.HasValues)
                            {
                                if (content.Name == "File")
                                {
                                    _file = content.Value.ToString();
                                }
                                else if (content.Name == "Rect")
                                {
                                    List<int> Rect = new List<int>();
                                    foreach (string item in content.Value.ToString().Split(',').ToList())
                                    {
                                        Rect.Add(int.Parse(item));
                                    }
                                    _x = Rect[0];
                                    _y = Rect[1];
                                    _width = Rect[2];
                                    _height = Rect[3];
                                }
                                else if (content.Name == "Center")
                                {
                                    List<int> Center = new List<int>();
                                    foreach (string item in content.Value.ToString().Split(',').ToList())
                                    {
                                        Center.Add(int.Parse(item));
                                    }
                                    _center_x = Center[0];
                                    _center_y = Center[1];
                                }
                            }
                        }
                        if (_center_x == null) _center_x = 0;
                        if (_center_y == null) _center_y = 0;
                        FrameList.Add(new Frame(_name, _file, (int)_x, (int)_y, (int)_width, (int)_height, (int)_center_x, (int)_center_y, Directory));
                    }
                }
            }

            public Anim(string _directory, string _fileLocation)
            {
                Directory = _directory;
                FileLocation = _fileLocation;
                FrameList.Add(new Frame(Directory));
            }

            public void Save(string SaveAsLocation = "")
            {
                string bc = "}";
                string bo = "{";
                string q = "\"";

                string output = "";
                output += $"{bo}";
                int count = FrameList.Count() - 1;
                foreach (Frame frame in FrameList)
                {
                    int index = FrameList.IndexOf(frame);
                    output += nL;
                    output += $"\t{q}{frame.Name}{q}: {bo} {q}File{q}: {q}{frame.File}{q}";
                    output += $", {q}Rect{q}: {q}{frame.X}, {frame.Y}, {frame.Width}, {frame.Height}{q}";
                    output += $", {q}Center{q}: {q}{frame.CenterX}, {frame.CenterY}{q}";
                    output += $"{bc}";
                    if (index != count) output += ",";
                }
                output += nL;
                output += $"{bc}";
                if (SaveAsLocation != "")
                {
                    File.WriteAllText(SaveAsLocation, output);
                    FileLocation = SaveAsLocation;
                }
                else File.WriteAllText(FileLocation, output);
            }

            public class Frame
            {
                public string Name;
                public string File;
                public string Directory;
                public int X;
                public int Y;
                public int Width;
                public int Height;
                public int CenterX;
                public int CenterY;

                public Frame(string _name, string _file, int _x, int _y, int _width, int _height, int _centerX, int _centerY, string _directory)
                {
                    Name = _name;
                    File = _file;
                    Directory = _directory;
                    X = _x;
                    Y = _y;
                    Width = _width;
                    Height = _height;
                    CenterX = _centerX;
                    CenterY = _centerY;
                }

                public Frame(string _directory) : this("New Frame", "", 0, 0, 0, 0, 0, 0, _directory) { }

                public override string ToString()
                {
                    return Name;
                }

                public ImageSource FrameImage { get { return GetImage(); } }

                public ImageSource GetImage()
                {
                    BitmapSource bitmap = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgr24, null, new byte[3] { 0, 0, 0 }, 3);
                    BitmapImage img = new BitmapImage();
                    string path = $"{Directory}\\{File}";

                    if (System.IO.File.Exists(path))
                    {
                        img.BeginInit();
                        img.UriSource = new Uri(path);
                        img.EndInit();

                        if (Width > 0 && Height > 0 && img != null)
                        {
                            try
                            {
                                bitmap = new CroppedBitmap(img,
                                new System.Windows.Int32Rect()
                                {
                                    X = X,
                                    Y = Y,
                                    Width = Width,
                                    Height = Height
                                });
                            }
                            catch (ArgumentException)
                            {
                            }
                        }
                    }
                    ImageSource result = bitmap;
                    return result;
                }
            }
        }
    }
}
