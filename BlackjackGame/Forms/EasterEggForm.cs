using BlackjackGame.Properties;
using DarkModeForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackjackGame.Forms
{
    public partial class EasterEggForm : Form
    {
        // I'm gonna be straight up honest, more than 80% of this Easter Egg logic was gathered from ChatGPT. Bro is the GOAT
        private DarkModeCS dm;
        private int widthInt = 186;
        private int heightInt = 269;

        public EasterEggForm()
        {
            InitializeComponent();
            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.DarkMode
            };

            LoadImages();
            LoadSounds();
        }

        private void AddPictureBox(Image img, string key, ref int x, ref int y, ref int count, Control container, int imagesPerRow, int padding)
        {
            PictureBox pb = new PictureBox
            {
                Image = img,
                Width = widthInt,
                Height = heightInt,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(x, y),
                BorderStyle = BorderStyle.None,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = key
            };

            pb.MouseEnter += (s, e) => HoverCardEffect(pb, true);
            pb.MouseLeave += (s, e) => HoverCardEffect(pb, false);
            pb.Click += (s, e) => PictureBox_Click(s, e);

            container.Controls.Add(pb);
            pb.BringToFront();

            count++;
            if (count % imagesPerRow == 0)
            {
                x = 10;
                y += heightInt + padding;
            }
            else
            {
                x += widthInt + padding;
            }
        }


        private void LoadImages()
        {
            // This type of logic is definitely beyond my knowledge. It doesn't just display all Image assets from my app resources, but it makes sure the images are in order.
            // The order of cards displayed is the same exact order as opening a brand new deck of cards: Spades -> Diamonds -> Clubs -> Hearts
            ResourceManager resourceManager = Resources.ResourceManager;
            ResourceSet resourceSet = resourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

            var imageTab = tabControl1.TabPages[0];
            
            // Clear previous PictureBoxes if reloading
            imageTab.Controls.Clear();

            int x = 10;
            int y = 10;
            int padding = 15;
            int imagesPerRow = 5;
            int count = 0;

            // Spades, Diamonds, Clubs, Hearts
            string[] suitsOrder = { "S", "D", "C", "H" };
            string[] ranksOrder = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            // Collect all image resources
            Dictionary<string, Image> allImages = new();
            foreach (DictionaryEntry entry in resourceSet)
            {
                if (entry.Value is Image img)
                {
                    allImages[entry.Key.ToString()] = img;
                }
            }

            // Add light mode cards in order
            foreach (var suit in suitsOrder)
            {
                foreach (var rank in ranksOrder)
                {
                    string key = rank + suit;
                    if (allImages.ContainsKey(key))
                    {
                        AddPictureBox(allImages[key], key, ref x, ref y, ref count, imageTab, imagesPerRow, padding);
                        allImages.Remove(key);
                    }
                }
            }

            // Add dark mode cards in order
            foreach (var suit in suitsOrder)
            {
                foreach (var rank in ranksOrder)
                {
                    string key = rank + suit + "_Dark";
                    if (allImages.ContainsKey(key))
                    {
                        AddPictureBox(allImages[key], key, ref x, ref y, ref count, imageTab, imagesPerRow, padding);
                        allImages.Remove(key);
                    }
                }
            }

            // Add remaining uncategorized images
            foreach (var kvp in allImages)
            {
                AddPictureBox(kvp.Value, kvp.Key, ref x, ref y, ref count, imageTab, imagesPerRow, padding);
            }
        }




        private void LoadSounds()
        {
            var resources = Resources.ResourceManager;
            var resourceSet = resources.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);

            int buttonWidth = 75;
            int buttonHeight = 75;
            int spacing = 25;
            int maxPerRow = 6;

            int x = spacing;
            int y = spacing;
            int column = 0;

            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key.ToString();

                if (entry.Value is Stream)
                {
                    Button soundButton = new Button
                    {
                        Text = key,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        Width = buttonWidth,
                        Height = buttonHeight,
                        Location = new Point(x, y),
                        Tag = key
                    };

                    // Click to play sound
                    soundButton.Click += (s, e) => SoundButton_Click(s, e);

                    soundTab.Controls.Add(soundButton);

                    column++;

                    if (column >= maxPerRow)
                    {
                        column = 0;
                        x = spacing;
                        y += buttonHeight + spacing;
                    }

                    else
                    {
                        x += buttonWidth + spacing;
                    }
                }
            }
        }

        private async void PictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox && pictureBox.Tag is string resourceName)
            {
                string imageName = (sender as PictureBox)?.Tag.ToString();
                int imageCount = tabControl1.TabPages[0].Controls.OfType<PictureBox>().Count();

                DialogResult result = MessageBox.Show($"Total Images: {imageCount}\r\n\r\nCurrent Resource Name: {imageName}\r\n\r\nWould you like to open this image in your default photo viewing program?", "Resource Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    object resource = Resources.ResourceManager.GetObject(resourceName);

                    if (resource is Bitmap fullQualityImage)
                    {
                        try
                        {
                            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
                            fullQualityImage.Save(tempPath, ImageFormat.Png);

                            // Launch default viewer and wait for exit
                            using (Process viewerProcess = new Process())
                            {
                                viewerProcess.StartInfo = new ProcessStartInfo
                                {
                                    FileName = tempPath,
                                    UseShellExecute = true
                                };

                                viewerProcess.EnableRaisingEvents = true;
                                viewerProcess.Start();

                                // Polling to wait for file to be unlocked (view closed)
                                await Task.Run(() =>
                                {
                                    viewerProcess.WaitForExit();

                                    // Extra safety wait for any lingering file locks
                                    Thread.Sleep(500);
                                });
                            }

                            // Try deleting after exit
                            if (File.Exists(tempPath))
                            {
                                File.Delete(tempPath);
                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error viewing image:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    else
                    {
                        MessageBox.Show("Could not retrieve image from resources.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void SoundButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string resourceName)
            {
                string soundName = (sender as Button)?.Tag.ToString();
                bool isCtrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;

                if (isCtrlPressed)
                {
                    DialogResult result = MessageBox.Show($"Total Sound Effects: {soundTab.Controls.Count}\r\n\r\nCurrent Resource Name: {soundName}\r\n\r\nWould you like to open this sound file in your default media player?", "Resource Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            object resource = Resources.ResourceManager.GetObject(resourceName);

                            if (resource is Stream soundStream)
                            {
                                string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");

                                using (FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                                {
                                    soundStream.CopyTo(fileStream);
                                }

                                using (Process soundPlayer = new Process())
                                {
                                    soundPlayer.StartInfo = new ProcessStartInfo
                                    {
                                        FileName = tempPath,
                                        UseShellExecute = true
                                    };

                                    soundPlayer.EnableRaisingEvents = true;
                                    soundPlayer.Start();

                                    await Task.Run(() =>
                                    {
                                        soundPlayer.WaitForExit();

                                        // Extra safety wait for any lingering file locks
                                        Thread.Sleep(500);
                                    });
                                }

                                if (File.Exists(tempPath))
                                {
                                    File.Delete(tempPath);
                                }
                            }

                            else
                            {
                                MessageBox.Show("Sound resource could not be found or is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to export or play sound:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    else
                    {

                    }
                }

                else
                {
                    object resource = Resources.ResourceManager.GetObject(resourceName);

                    if (resource is Stream stream)
                    {
                        SoundPlayer player = new SoundPlayer(stream);
                        player.Play();
                    }
                }
            }
        }

        private void HoverCardEffect(PictureBox pb, bool isHovered)
        {
            if (isHovered)
            {
                // Make the card appear slightly bigger
                pb.Size = new Size(widthInt + 5, heightInt + 5);

                // Slight lift
                pb.Location = new Point(pb.Location.X - 5, pb.Location.Y - 5);
            }

            else
            {
                // Return to original size
                pb.Size = new Size(widthInt, heightInt);
                
                // Return to original position
                pb.Location = new Point(pb.Location.X + 5, pb.Location.Y + 5);
            }
        }

        private void imageTab_MouseEnter(object sender, EventArgs e)
        {
            imageTab.Focus();
        }

        private void EasterEggForm_Load(object sender, EventArgs e)
        {
            // Empty code. Might add something later, who knows. Was this load event even necessary to add? Of course not, but I did it anyway. Deal with it
        }
    }
}
