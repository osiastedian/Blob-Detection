using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blob_Detection
{
    class CCL : IConnectedComponentLabeling
    {
        IDictionary<int, Bitmap> blobs;
        
        public CCL()
        {
            blobs = new Dictionary<int, Bitmap>();
        }

        public IDictionary<int, Bitmap> Process(Bitmap input)
        {
            
            return null;
        }
        public Bitmap coloredBlobs(Bitmap input) {
            Dictionary<Point, int> labelTable = new Dictionary<Point, int>();
            Dictionary<int, List<Point>> blobs = new Dictionary<int, List<Point>>();
            FirstPass(input, ref labelTable);
            blobs = SecondPass(input, ref labelTable);
            Bitmap bitmap = (Bitmap)input.Clone();
            Random rng = new Random();
            foreach(KeyValuePair<int, List<Point>> blob in blobs)
            {
                Color currentColor = Color.FromArgb(rng.Next(0, 255), rng.Next(0, 255), rng.Next(0, 255));
                foreach (Point p in blob.Value)
                {
                    bitmap.SetPixel(p.X, p.Y, currentColor);
                }
            }
            return bitmap;
        }

        private void FirstPass(Bitmap bmp, ref Dictionary<Point, int> labelTable)
        {
            Point north = new Point();
            Point west = new Point();
            int componentCount = 0;
            for (int row = 0; row < bmp.Width; row++)
                for (int col = 0; col < bmp.Height; col++)
                {
                    Color currentPixel = bmp.GetPixel(row, col);
                    if (CheckIsBackGround(currentPixel))
                        continue;
                    north.X = row;
                    north.Y = col - 1;
                    west.X = row -1;
                    west.Y = col;
                    int assignLabel = -1;
                    // CHECK NORTH
                    if (!OutOfBounds(north, bmp.Size))
                    {
                        int northLabel = -1;
                        if (labelTable.TryGetValue(north, out northLabel))
                        {
                            assignLabel = northLabel;
                        }                        
                    }
                    // CHECK WEST
                    if (!OutOfBounds(west, bmp.Size))
                    {
                        int westLabel = -1;
                        if (labelTable.TryGetValue(west, out westLabel))
                        {
                            assignLabel = westLabel;
                        }
                    }
                    // NO NEIGHBOR LABEL
                    if (assignLabel == -1)
                    {
                        assignLabel = ++componentCount;
                    }
                    labelTable.Add(new Point(row, col), assignLabel);
                }
        }

        bool OutOfBounds(Point point, Size size)
        {
            if (point.X < 0 || point.X > size.Width)
                return true;
            if (point.Y < 0 || point.Y > size.Height)
                return true;
            return false;
        }

        private Dictionary<int, List<Point>> SecondPass(Bitmap bmp, ref Dictionary<Point, int> labelTable)
        {
            Point north = new Point();
            Point west = new Point();
            Dictionary<int, List<Point>> blobs = new Dictionary<int, List<Point>>();
            for (int row = 0; row < bmp.Width; row++)
                for (int col = 0; col < bmp.Height; col++)
                {
                    Color currentPixel = bmp.GetPixel(row, col);
                    if (CheckIsBackGround(currentPixel))
                        continue;
                    north.X = row;
                    north.Y = col - 1;
                    west.X = row - 1;
                    west.Y = col;
                    int assignLabel;
                    // CHECK IF LABELED
                    if (!labelTable.TryGetValue(new Point(row, col), out assignLabel))
                    {
                        assignLabel = -1;
                    }
                    // CHECK NORTH
                    int northLabel = -1;
                    if (!OutOfBounds(north, bmp.Size))
                    {
                        if (!labelTable.TryGetValue(north, out northLabel))
                        {
                            northLabel = -1;
                        }
                    }
                    // CHECK WEST
                    int westLabel = -1;
                    if (!OutOfBounds(west, bmp.Size))
                    {
                        if (!labelTable.TryGetValue(west, out westLabel))
                        {
                            westLabel = -1;
                        }
                    }
                    List<Point> currentBlob;
                    if(westLabel != -1 && northLabel!=-1)
                    {
                        if (westLabel != northLabel){
                            List<Point> northBlob;
                            if (!blobs.TryGetValue(northLabel, out northBlob))
                            {
                                northBlob = null;
                            }
                            List<Point> westBlob;
                            if (!blobs.TryGetValue(westLabel, out westBlob))
                            {
                                northBlob = null;
                            }
                            foreach (Point point in northBlob)
                            {
                                westBlob.Add(point);
                                labelTable[point] = westLabel;
                            }
                            blobs.Remove(northLabel);
                            
                        }
                        /*
                        if (westLabel < northLabel)
                        {
                            if (blobs.TryGetValue(northLabel, out currentBlob))
                            {
                                currentBlob.Remove(north);
                            }

                            if (blobs.TryGetValue(westLabel, out currentBlob))
                            {
                                currentBlob.Add(north);
                            }
                            labelTable[north] = westLabel;                            
                            assignLabel = westLabel;
                        }
                        else if (westLabel > northLabel)
                        {
                            if (blobs.TryGetValue(westLabel, out currentBlob))
                            {
                                currentBlob.Remove(west);
                            }

                            if (blobs.TryGetValue(northLabel, out currentBlob))
                            {
                                currentBlob.Add(west);
                            }
                            labelTable[west] = northLabel;
                            assignLabel = northLabel;
                        }*/
                    }

                    if (blobs.TryGetValue(assignLabel, out currentBlob))
                    {
                        currentBlob.Add(new Point(row, col));
                    }
                    else
                    {
                        currentBlob = new List<Point>();
                        currentBlob.Add(new Point(row, col));
                        blobs.Add(assignLabel, currentBlob);
                    }
                }
            return blobs;
        }

        bool CheckIsBackGround(Color currentPixel)
        {
            return currentPixel.A == 255 && currentPixel.R == 255 &&
                     currentPixel.G == 255 && currentPixel.B == 255;
        }
    }
}
