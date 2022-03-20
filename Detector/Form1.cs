using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Detector
{
    /// <summary>
    /// Main class
    /// </summary>
    public partial class Form1 : Form
    {
        Color[] colors;
        RadioButton[] radioButtons;
        public Form1()
        {
            InitializeComponent();

            colors = new Color[]
            {
                Color.Black,
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.White
            };

            radioButtons = new RadioButton[] {
                radioButton1,
                radioButton2,
                radioButton3,
                radioButton4,
                radioButton6
            };
        }

        /// <summary>
        /// Send image to app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadImage(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\users\\PC\\Images";

                // only image
                ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
                pbInputImage.SizeMode = PictureBoxSizeMode.StretchImage;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pbInputImage.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        /// <summary>
        /// B/W filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlackAndWhiteButton(object sender, EventArgs e)
        {
            Bitmap img, convertedBMP;
            try
            {
                img = new Bitmap(pbInputImage.Image);   // exception provocation. Can be a null
                convertedBMP = new Bitmap(img.Width, img.Height);

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // sizemod of picturebox

                if (!(tbBwColorDeep.Text is null) && !tbBwColorDeep.Text.Equals(""))
                {
                    int deep = (Int32.Parse(tbBwColorDeep.Text));   // difference between colors of one channel

                    for (int i = 1; i < img.Width; i++) // go to the columns
                    {
                        for (int j = 1; j < img.Height; j++)    // go to the rows
                        {
                            var pixel = img.GetPixel(i, j);

                            // if differece colors of one channel neighboring pixels is more of than normal
                            if ((Math.Abs(pixel.R - img.GetPixel(i - 1, j).R) > deep) ||
                                (Math.Abs(pixel.G - img.GetPixel(i - 1, j).G) > deep) ||
                                (Math.Abs(pixel.B - img.GetPixel(i - 1, j).B) > deep) ||
                                (Math.Abs(pixel.R - img.GetPixel(i, j - 1).R) > deep) ||
                                (Math.Abs(pixel.G - img.GetPixel(i, j - 1).G) > deep) ||
                                (Math.Abs(pixel.B - img.GetPixel(i, j - 1).B) > deep))
                            {
                                for (int k = 0; k < radioButtons.Length; k++)
                                {
                                    //checking of selected radiobox
                                    if (radioButtons[k].Checked)
                                    {
                                        convertedBMP.SetPixel(i, j, colors[k]); // paint the pixel with the selected color
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // if inversion is "on"
                                if (radioButton6.Checked)
                                {
                                    convertedBMP.SetPixel(i, j, Color.Black);   // paint the pixel in the black color
                                }
                                else
                                {
                                    convertedBMP.SetPixel(i, j, Color.White);   // else paint with the white color
                                }
                            }
                        }
                        // update the picturebox every 5 columns (like loading)
                        if (i % 10 == 0)
                        {
                            pictureBox1.Image = convertedBMP;
                            Update();
                        }
                    }
                }
                pictureBox1.Image = convertedBMP;   // finnaly load image
            }
            // if we are havent image
            catch (NullReferenceException ex)
            {
                UploadImage(sender, e);
            }
        }

        /// <summary>
        /// Mix of 2 image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Mix_Click(object sender, EventArgs e)
        {
            if (pbInputImage.Image != null && MixedImage.Image != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                Bitmap img = new Bitmap(pbInputImage.Image);
                Bitmap mixImg = new Bitmap(MixedImage.Image);

                int minWidth = Math.Min(img.Width, mixImg.Width);
                int minHeight = Math.Min(img.Height, mixImg.Height);

                Bitmap convertedBMP = new Bitmap(minWidth, minHeight);

                int minLight = Int32.Parse(textBox1.Text);
                int maxLight = Int32.Parse(textBox2.Text);

                for (int i = 0; i < minWidth; i++)
                {
                    for (int j = 0; j < minHeight; j++)
                    {
                        int red = mixImg.GetPixel(i, j).R;
                        int green = mixImg.GetPixel(i, j).G;
                        int blue = mixImg.GetPixel(i, j).B;

                        if ((red + green + blue) / 3 >= minLight && (red + green + blue) / 3 < maxLight)
                        {
                            convertedBMP.SetPixel(i, j, img.GetPixel(i, j));
                        }
                        else
                        {
                            convertedBMP.SetPixel(i, j, mixImg.GetPixel(i, j));
                        }
                    }
                    if (i % 5 == 0)
                    {
                        pictureBox1.Image = convertedBMP;
                        Refresh();
                    }
                }
                pictureBox1.Image = convertedBMP;
            }
            else
            {
                MessageBox.Show("Добавьте изображения");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\users\\PC\\Images";
                MixedImage.SizeMode = PictureBoxSizeMode.StretchImage;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    MixedImage.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void GTAFilter_Click(object sender, EventArgs e)
        {
            if (pbInputImage.Image != null)
            {
                Bitmap convertedBMP = new Bitmap(pbInputImage.Image.Width, pbInputImage.Image.Height);
                Bitmap img = new Bitmap(pbInputImage.Image);

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                for (int i = 1; i < img.Width; i++)
                {
                    for (int j = 1; j < img.Height; j++)
                    {
                        var pixel = img.GetPixel(i, j);
                        var red = pixel.R > 32 ? (pixel.R > 64 ? (pixel.R > 128 ? (pixel.R > 160 ? 230 : 128) : 64) : 32) : 0;
                        var green = pixel.G > 32 ? (pixel.G > 64 ? (pixel.G > 128 ? (pixel.G > 160 ? 230 : 128) : 64) : 32) : 0;
                        var blue = pixel.B > 32 ? (pixel.B > 64 ? (pixel.B > 128 ? (pixel.B > 160 ? 230 : 128) : 64) : 32) : 0;

                        convertedBMP.SetPixel(i, j, Color.FromArgb(red, green, blue));
                    }
                    if (i % 5 == 0)
                    {
                        pictureBox1.Image = convertedBMP;
                        Refresh();
                    }
                }
                pictureBox1.Image = convertedBMP;
            }
            else
            {
                MessageBox.Show("Добавьте изображениe");
            }
        }

        private void LineFilter_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            Bitmap img, convertedBMP;
            try
            {
                img = new Bitmap(pbInputImage.Image);
                convertedBMP = new Bitmap(img.Width, img.Height);
            }
            catch (Exception ex)
            {
                return;
            }
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height - 8; j += 8)
                {
                    Color mainColor = GetMainColor(new Color[]
                    {
                        img.GetPixel(i,j),
                        img.GetPixel(i,j+1),
                        img.GetPixel(i,j+2),
                        img.GetPixel(i,j+3),
                        img.GetPixel(i,j+4)
                    });
                    convertedBMP.SetPixel(i, j, mainColor);
                    convertedBMP.SetPixel(i, j + 1, mainColor);
                    convertedBMP.SetPixel(i, j + 2, mainColor);
                    convertedBMP.SetPixel(i, j + 3, mainColor);
                    convertedBMP.SetPixel(i, j + 4, mainColor);
                    convertedBMP.SetPixel(i, j + 5, mainColor);
                    convertedBMP.SetPixel(i, j + 6, mainColor);
                    convertedBMP.SetPixel(i, j + 7, mainColor);
                    convertedBMP.SetPixel(i, j + 8, mainColor);
                }
                if (i % 10 == 0)
                {
                    pictureBox1.Image = convertedBMP;
                    Refresh();
                }
            }
            pictureBox1.Image = convertedBMP;
        }

        private Color GetMainColor(Color[] lineColors)
        {
            int r, g, b;
            r = b = g = 0;
            foreach (var color in lineColors)
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }
            return Color.FromArgb(r / lineColors.Length, g / lineColors.Length, b / lineColors.Length);
        }
    }
}
