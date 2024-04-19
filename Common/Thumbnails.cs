using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiginBS.Common
{
    public class Thumbnails
    {
        public Thumbnails() { }
        public string  GeneralThumb(string _fullpath, string _thumbDir, int w2, double h2)
        {
            string fname = Path.GetFileName(_fullpath);
            string _dirpath = Path.GetDirectoryName(_fullpath); // .Substring(0, _fullpath.Length - fname.Length);
            string _destPath = System.IO.Path.Combine(_dirpath, _thumbDir);

            try
            {
                if (!Directory.Exists(_destPath))
                {
                    Directory.CreateDirectory(_destPath);
                }
                //get a source Image to System.Drawing.Image
                System.Drawing.Image objImage = System.Drawing.Image.FromFile(_fullpath);
                //Create a System.Drawing.Bimap with the disert width and height of the thumbnail
                int w = objImage.Width;
                int h = objImage.Height;
                w2 = (w2 > 0) ? w2 : 100;
                //double h2 = ((double)w2 / w) * h;
                if (h2 == 0) h2 = ((double)w2 / w) * h;

                System.Drawing.Bitmap objBMP = new Bitmap(w2, (Int32)h2);
                //Create System.Drawing.Graphics object from Bitmap which we will use to draw the high quality scaled image
                System.Drawing.Graphics objGraph = System.Drawing.Graphics.FromImage(objBMP);
                //Setting
                objGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                objGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                objGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //Draw the original image into the target Graphics Object scaling to the disert width and heigh
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, w2, (Int32)h2);
                objGraph.DrawImage(objImage, rectDestination, 0, 0, w, h, GraphicsUnit.Pixel);
                objBMP.Save(_destPath + @"\Thumb_" + fname);
                objBMP.Dispose();
                objImage.Dispose();
                return _destPath + @"\Thumb_" + fname;
                //img.ImageUrl = "Images/Thumbnails/" + "Thumb_" + fname;
            }
            catch
            {
                return "error";
            }


        }

        public int GeneralThumb(string _fullpath, string _thumbDir, int w2, double h2, int fileNo)
        {
            string fname = Path.GetFileName(_fullpath);
            string _dirpath = _fullpath.Substring(0, _fullpath.Length - fname.Length);
            string _destPath = _dirpath + _thumbDir;
            try
            {
                if (!Directory.Exists(_dirpath))
                {
                    Directory.CreateDirectory(_dirpath);
                }
                //get a source Image to System.Drawing.Image
                System.Drawing.Image objImage = System.Drawing.Image.FromFile(_fullpath);
                //Create a System.Drawing.Bimap with the disert width and height of the thumbnail
                int w = objImage.Width;
                int h = objImage.Height;
                w2 = (w2 > 0) ? w2 : 100;

                if (h2 == 0) h2 = ((double)w2 / w) * h;

                // double h2 = ((double)w2 / w) * h;
                System.Drawing.Bitmap objBMP = new Bitmap(w2, (Int32)h2);
                //Create System.Drawing.Graphics object from Bitmap which we will use to draw the high quality scaled image
                System.Drawing.Graphics objGraph = System.Drawing.Graphics.FromImage(objBMP);
                //Setting
                objGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                objGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                objGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //Draw the original image into the target Graphics Object scaling to the disert width and heigh
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, w2, (Int32)h2);
                objGraph.DrawImage(objImage, rectDestination, 0, 0, w, h, GraphicsUnit.Pixel);
                objBMP.Save(_destPath + @"\(" + fileNo + ")Thumb_" + fname);
                objBMP.Dispose();
                objImage.Dispose();
                return 1;
                //img.ImageUrl = "Images/Thumbnails/" + "Thumb_" + fname;
            }
            catch
            {
                return 0;
            }


        }

        public int GeneralThumb_root(string _fullpath, string _thumbdir, int w2, double h2)
        {
            string fname = Path.GetFileName(_fullpath);
            try
            {
                //get a source Image to System.Drawing.Image
                System.Drawing.Image objImage = System.Drawing.Image.FromFile(_fullpath);
                //Create a System.Drawing.Bimap with the disert width and height of the thumbnail
                int w = objImage.Width;
                int h = objImage.Height;
                w2 = (w2 > 0) ? w2 : 100;
                if (h2 == 0) h2 = ((double)w2 / w) * h;
                System.Drawing.Bitmap objBMP = new Bitmap(w2, (Int32)h2);
                //Create System.Drawing.Graphics object from Bitmap which we will use to draw the high quality scaled image
                System.Drawing.Graphics objGraph = System.Drawing.Graphics.FromImage(objBMP);
                //Setting
                objGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                objGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                objGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //Draw the original image into the target Graphics Object scaling to the disert width and heigh
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, w2, (Int32)h2);
                objGraph.DrawImage(objImage, rectDestination, 0, 0, w, h, GraphicsUnit.Pixel);
                objBMP.Save(_thumbdir + @"\Thumb_" + fname);
                objBMP.Dispose();
                objImage.Dispose();
                return 1;
                //img.ImageUrl = "Images/Thumbnails/" + "Thumb_" + fname;
            }
            catch
            {
                return 0;
            }
        }


    }
}
