using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace picshow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(@"F:\IotLab\Color_Recognition\imgData\imgData321_2.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);//imgData224_10.txt镜头扭紧一点会更清晰
            string str1 = sr.ReadToEnd();
            byte[] imgData = Str2Byte(str1);
            List<byte> list = imgData.ToList();
            list.RemoveAt(0);
            list.RemoveAt(list.Count-1);
            //rgb565转到rgb888呈现图片
            //byte[] newImgData = list.ToArray();//byte[]数组的长度应该是153600
            //byte[] rgbData = RGB565ToRGB888(newImgData);
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            //pictureBox1.Image = GetBitmap1(rgbData);
            //直接呈现rgb888图像
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = FromListGetBitmap(list, 80, 200);
        }

        /// <summary>
        /// 将十六进制的字符串转换成byte[]
        /// </summary>
        /// <param name="hexString">十六进制的字符串</param>
        /// <returns>byte[]数组</returns>
        public static byte[] Str2Byte(string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                hexString = hexString.ToUpper();
                if ((hexString.Length % 2) != 0)
                    hexString = "0" + hexString;
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                return returnBytes;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将RGB565的16byte数据转换成RGB888，未进行量化补偿
        /// </summary>
        /// <param name="imgData"></param>
        /// <returns></returns>
        public byte[] RGB565ToRGB888(byte[] imgData)
        {
            byte[] rgbByte = new byte[imgData.Length/2*3];
            int j = 0;
            for (int i = 0; i < imgData.Length-1; i=i+2)
            {
                string rgb1 = System.Convert.ToString(imgData[i], 2).PadLeft(8, '0');
                string rgb2 = System.Convert.ToString(imgData[i+1], 2).PadLeft(8, '0');
                string rgb = rgb1 + rgb2;
                string r = rgb.Substring(0, 5).PadRight(8,'0');
                string g = rgb.Substring(5, 6).PadRight(8, '0');
                string b = rgb.Substring(11, 5).PadRight(8, '0');
                //rgbByte[j] = Convert.ToByte(r);
                string rr = string.Format("{0:X}", System.Convert.ToInt32(r, 2)).PadLeft(2, '0');
                string gg = string.Format("{0:X}", System.Convert.ToInt32(g, 2)).PadLeft(2, '0');
                string bb = string.Format("{0:X}", System.Convert.ToInt32(b, 2)).PadLeft(2, '0');
                rgbByte[j] = Convert.ToByte(rr, 16);
                rgbByte[j+1] = Convert.ToByte(gg, 16);
                rgbByte[j+2] = Convert.ToByte(bb, 16);
                j = j + 3;
            }
            return rgbByte;
        }
        
        /// <summary>
        /// 将2进制流转换成图像
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            ms.Position = 0;
            try
            {
                Image image = System.Drawing.Image.FromStream(ms);
                return image;
            }
            catch (Exception e)
            {
                MessageBox.Show("二进制流转图像失败");
                return null;
            }
        }

        /// <summary>
        /// 转化bytes成16进制的字符
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static string Bytes2HexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        
        /// <summary>
        /// 将16进制转换成二进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string Hex2Bin(string hex)
        {
            int hexLength = hex.Length;
            int binLength = hexLength * 4;
            int tem = Convert.ToInt32(hex, 16);
            string bin = Convert.ToString(tem, 2);
            int binlen = bin.Length;
            for (int i = 0; i < binLength - binlen; i++)
                bin = "0" + bin;
            return bin;
        }

        public byte[] SaveImage()
        {
            FileStream fs = new FileStream(@"F:\IotLab\Color_Recognition\testImg.bmp", FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return imgBytesIn;
        }

        //图像尺寸为320*240
        public Bitmap GetBitmap(byte[] imgData,int height,int width)
        {
            Bitmap img = new Bitmap(width, height);
            int k = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color c=Color.FromArgb(imgData[k],imgData[k+1],imgData[k+2]);
                    img.SetPixel(j,i,c);
                    k = k + 3;
                }
            }
            return img;
        }

        //图像尺寸为200*80
        public Bitmap GetBitmap1(byte[] imgData)
        {
            Bitmap img = new Bitmap(200, 80);
            int k = 0;
            for (int i = 0; i < 80; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    Color c = Color.FromArgb(imgData[k], imgData[k + 1], imgData[k + 2]);
                    img.SetPixel(j, i, c);
                    k = k + 3;
                }
            }
            return img;
        }
        /// <summary>
        /// 产生200*80的图片
        /// </summary>
        /// <param name="colorList"></param>
        /// <returns></returns>
        //public Bitmap FromListGetBitmap1(List<byte> colorList)
        //{
        //    Bitmap img = new Bitmap(200, 80);
        //    int k = 0;
        //    for (int i = 0; i < 80; i++)
        //    {
        //        for (int j = 0; j < 200; j++)
        //        {
        //            Color c = Color.FromArgb(colorList[k], colorList[k + 1], colorList[k + 2]);
        //            img.SetPixel(j, i, c);
        //            k = k + 3;
        //        }
        //    }
        //    return img;
        //}
        public Bitmap FromListGetBitmap(List<byte> colorList,int height,int width)
        {
            Bitmap img = new Bitmap(width, height);
            int k = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color c = Color.FromArgb(colorList[k], colorList[k + 1], colorList[k + 2]);
                    img.SetPixel(j, i, c);
                    k = k + 3;
                }
            }
            return img;
        }
        /// <summary>
        /// 产生25*80的图片
        /// </summary>
        /// <param name="colorList"></param>
        /// <returns></returns>
        //public Bitmap FromListGetBitmap2(List<byte> colorList)
        //{
        //    Bitmap img = new Bitmap(25, 80);
        //    int k = 0;
        //    for (int i = 0; i < 80; i++)
        //    {
        //        for (int j = 0; j < 25; j++)
        //        {
        //            Color c = Color.FromArgb(colorList[k], colorList[k + 1], colorList[k + 2]);
        //            img.SetPixel(j, i, c);
        //            k = k + 3;
        //        }
        //    }
        //    return img;
        //}

    }
}
