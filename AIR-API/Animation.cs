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

            public class Rect
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int Width { get; set; }
                public int Height { get; set; }
                public Rect(int x, int y, int width, int height)
                {
                    X = x;
                    Y = y;
                    Width = width;
                    Height = height;
                }

                public Rect()
                {

                }
            }

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
                    Rect _rect = new Rect();
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
                                    _rect = new Rect(Rect[0], Rect[1], Rect[2], Rect[3]);
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
                        if (_center_x != null && _center_y != null) FrameList.Add(new Frame(_name, _file, (int)_rect.X, (int)_rect.Y, (int)_rect.Width, (int)_rect.Height, (int)_center_x, (int)_center_y, Directory));
                        else FrameList.Add(new Frame(_name, _file, (int)_rect.X, (int)_rect.Y, (int)_rect.Width, (int)_rect.Height, 0, 0, Directory));
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
                output += "{";
                int count = FrameList.Count() - 1;
                foreach (Frame frame in FrameList)
                {
                    int index = FrameList.IndexOf(frame);
                    output += nL;
                    output += $"\t{q}{frame.Name}{q}:  {bo} {q}File{q}: {q}{frame.File}{q}, {q}Rect{q}: {q}{frame.X},{frame.Y},{frame.Width},{frame.Height}{q}, {q}Center{q}: {q}{frame.CenterX},{frame.CenterY}{q} {bc}";
                    if (index != count) output += ",";
                }
                output += nL;
                output += "}";
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

                public Frame(string _directory)
                {
                    Name = "New Frame";
                    File = "";
                    Directory = _directory;
                    X = 0;
                    Y = 0;
                    Width = 0;
                    Height = 0;
                    CenterX = 0;
                    CenterY = 0;

                }


            }
        }
    }



}
