using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Media;
using System.IO;
using Timer = System.Windows.Forms.Timer;

namespace Memory
{
    public partial class Form1 : Form
    {
        private Liste game;
        private PictureBox[,] cardBoxes;

        private Panel infoPanel;
        private Label scoreLabel;
        private Label timerLabel;
        private Label statusLabel;
        private Button quitButton;

        private int moves = 0;
        private int selectedCount = 0;
        private string firstCoordinate = "";
        private string secondCoordinate = "";
        private PictureBox firstBox = null;
        private PictureBox secondBox = null;
        private bool isCheckingPair = false;

        private Panel gamePanel;
        private Panel gridPanel;
        private Timer gameTimer;
        private int seconds = 0;
        private int gridSize = 4;
        private Bitmap backCardImage;

        private SoundPlayer soundPlayer;

        public Form1(int size = 4)
        {
            gridSize = size;
            this.Text = $"Memory SecurIT - Grille {size}×{size}";
            this.Width = 1000;
            this.Height = 900;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 40);

            CreateBackCardImage();
            CreateUI();
            StartNewGame();
        }

        private void JouerSon(string nomFichier, bool enBoucle = false)
        {
            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "static", "sounds", nomFichier);

                if (File.Exists(soundPath))
                {
                    if (soundPlayer == null) soundPlayer = new SoundPlayer();
                    soundPlayer.SoundLocation = soundPath;

                    if (enBoucle) soundPlayer.PlayLooping();
                    else soundPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERREUR SON] : {ex.Message}");
            }
        }

        private void CreateBackCardImage()
        {
            backCardImage = new Bitmap(250, 250);
            using (Graphics g = Graphics.FromImage(backCardImage))
            {
                string backImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "static", "images", "Card_Back.png");
                bool imageChargee = false;

                if (File.Exists(backImagePath))
                {
                    try
                    {
                        using (Image img = Image.FromFile(backImagePath))
                        {
                            g.DrawImage(img, 0, 0, 250, 250);
                            imageChargee = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ERREUR IMAGE DOS] : {ex.Message}");
                    }
                }

                if (!imageChargee)
                {
                    g.Clear(Color.SteelBlue);
                }
            }
        }

        private void CreateUI()
        {
            infoPanel = new Panel();
            infoPanel.Location = new Point(0, 0);
            infoPanel.Size = new Size(this.ClientSize.Width, 60);
            infoPanel.BackColor = Color.FromArgb(30, 30, 60);
            infoPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(infoPanel);

            scoreLabel = new Label();
            scoreLabel.Text = "Essais: 0";
            scoreLabel.Location = new Point(20, 15);
            scoreLabel.AutoSize = true;
            scoreLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            scoreLabel.ForeColor = Color.Cyan;
            infoPanel.Controls.Add(scoreLabel);

            timerLabel = new Label();
            timerLabel.Text = "Temps: 00:00";
            timerLabel.Location = new Point(400, 15);
            timerLabel.Width = 150;
            timerLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            timerLabel.ForeColor = Color.Lime;
            infoPanel.Controls.Add(timerLabel);

            statusLabel = new Label();
            statusLabel.Text = "Sélectionnez deux cartes";
            statusLabel.Location = new Point(600, 15);
            statusLabel.Width = 250;
            statusLabel.Font = new Font("Arial", 10);
            statusLabel.ForeColor = Color.White;
            infoPanel.Controls.Add(statusLabel);

            quitButton = new Button();
            quitButton.Text = "Quitter";
            quitButton.Width = 80;
            quitButton.Height = 30;
            quitButton.Location = new Point(infoPanel.Width - quitButton.Width - 20, 15);
            quitButton.BackColor = Color.Red;
            quitButton.ForeColor = Color.White;
            quitButton.FlatStyle = FlatStyle.Flat;
            quitButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            quitButton.Click += QuitButton_Click;
            infoPanel.Controls.Add(quitButton);

            gamePanel = new Panel();
            gamePanel.Location = new Point(0, 60);
            gamePanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 60);
            gamePanel.BackColor = Color.ForestGreen;
            gamePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(gamePanel);

            gridPanel = new Panel();
            gridPanel.BackColor = Color.Transparent;
            gamePanel.Controls.Add(gridPanel);

            this.Resize += Form1_Resize;

            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            AjusterTailleCartes();
            AjusterBarreHaut();
        }

        private void AjusterBarreHaut()
        {
            if (infoPanel != null && timerLabel != null && statusLabel != null)
            {
                timerLabel.Left = (infoPanel.Width / 3) - (timerLabel.Width / 2);
                statusLabel.Left = (infoPanel.Width / 3) * 2 - (statusLabel.Width / 2);
            }
        }

        private void AjusterTailleCartes()
        {
            if (game == null || cardBoxes == null || gridPanel == null || gamePanel == null) return;

            int spacing = 15;
            int availableWidth = gamePanel.Width - (spacing * 2);
            int availableHeight = gamePanel.Height - (spacing * 2);

            int maxWidth = availableWidth / game.Columns;
            int maxHeight = availableHeight / game.Rows;
            int boxSize = Math.Min(maxWidth, maxHeight) - spacing;

            boxSize = Math.Max(boxSize, 50);
            boxSize = Math.Min(boxSize, 250);

            gridPanel.Width = game.Columns * (boxSize + spacing) - spacing;
            gridPanel.Height = game.Rows * (boxSize + spacing) - spacing;

            int x = (gamePanel.Width - gridPanel.Width) / 2;
            int y = (gamePanel.Height - gridPanel.Height) / 2;
            gridPanel.Location = new Point(Math.Max(0, x), Math.Max(0, y));

            for (int i = 0; i < game.Rows; i++)
            {
                for (int j = 0; j < game.Columns; j++)
                {
                    PictureBox box = cardBoxes[i, j];
                    if (box != null)
                    {
                        box.Width = boxSize;
                        box.Height = boxSize;
                        box.Left = j * (boxSize + spacing);
                        box.Top = i * (boxSize + spacing);
                    }
                }
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            seconds++;
            int minutes = seconds / 60;
            int secs = seconds % 60;
            timerLabel.Text = $"Temps: {minutes:D2}:{secs:D2}";
        }

        private void StartNewGame()
        {
            soundPlayer?.Stop();

            int pairCount = (gridSize == 6) ? 18 : 8;

            // --- NOUVEAU : Ta liste complète des 18 images ---
            List<string> allImages = new List<string>
            {
                "FireWall.png", "Password.png", "Hacker.png",
                "Server.png", "AntiVirus.png", "Phishing.png",
                "Ransomware.png", "Fingerprint.png", "DoubleSecurity.png",
                "UsbKey.png", "CyberSecurity.png", "IdenttityTheft.png",
                "VPN.png", "SQLInjection.png", "Encryption.png",
                "Malware.png", "Trojan.png", "SecureConnection.png"
            };

            List<string> imagePaths = new List<string>();

            // On ajoute exactement le nombre d'images dont on a besoin
            for (int i = 0; i < pairCount; i++)
            {
                imagePaths.Add(allImages[i]);
            }

            game = new Liste(imagePaths);
            cardBoxes = new PictureBox[game.Rows, game.Columns];

            moves = 0;
            selectedCount = 0;
            firstCoordinate = "";
            secondCoordinate = "";
            firstBox = null;
            secondBox = null;
            isCheckingPair = false;
            seconds = 0;

            gridPanel.Controls.Clear();
            CreateCardBoxes(gridPanel);
            UpdateScore();

            AjusterTailleCartes();
            AjusterBarreHaut();
        }

        private void CreateCardBoxes(Panel panel)
        {
            for (int i = 0; i < game.Rows; i++)
            {
                for (int j = 0; j < game.Columns; j++)
                {
                    PictureBox box = new PictureBox();
                    box.Image = backCardImage;
                    box.SizeMode = PictureBoxSizeMode.StretchImage;
                    box.BorderStyle = BorderStyle.None;

                    box.Tag = $"{i}:{j}";
                    box.Click += CardBox_Click;
                    box.Cursor = Cursors.Hand;

                    cardBoxes[i, j] = box;
                    panel.Controls.Add(box);
                }
            }
        }

        private void CardBox_Click(object sender, EventArgs e)
        {
            if (isCheckingPair || game == null) return;

            PictureBox box = sender as PictureBox;
            if (box == null) return;

            string coordinates = box.Tag.ToString();
            string[] parts = coordinates.Split(':');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);

            Carte card = game.Cards[row][col];
            if (card.Status != Carte.State.Cachée) return;

            game.RevealCard(coordinates);
            JouerSon("Flipped_Card.wav");

            Bitmap cardImage = new Bitmap(250, 250);
            using (Graphics g = Graphics.FromImage(cardImage))
            {
                string frontImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "static", "images", "Card_Front.png");
                bool fondCharge = false;

                if (File.Exists(frontImagePath))
                {
                    try
                    {
                        using (Image imgFront = Image.FromFile(frontImagePath))
                        {
                            g.DrawImage(imgFront, 0, 0, 250, 250);
                            fondCharge = true;
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine($"[ERREUR FOND] : {ex.Message}"); }
                }

                if (!fondCharge) g.Clear(Color.White);

                // Plus besoin de nettoyer le nom de l'image, on l'utilise directement !
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "static", "images", card.ImagePath);

                if (File.Exists(iconPath))
                {
                    try
                    {
                        using (Image icon = Image.FromFile(iconPath))
                        {
                            int iconSize = 150;
                            int x = (250 - iconSize) / 2;
                            int y = (250 - iconSize) / 2;

                            g.DrawImage(icon, x, y, iconSize, iconSize);
                        }
                    }
                    catch (Exception ex) { Debug.WriteLine($"[ERREUR ICÔNE] : {ex.Message}"); }
                }
                else
                {
                    string nomSansExtension = card.ImagePath.Replace(".png", "");
                    g.DrawString(nomSansExtension, new Font("Arial", 14, FontStyle.Bold), Brushes.Black, 20, 100);
                }
            }
            box.Image = cardImage;

            selectedCount++;

            if (selectedCount == 1)
            {
                firstCoordinate = coordinates;
                firstBox = box;
                statusLabel.Text = "Sélectionnez la deuxième carte";
            }
            else if (selectedCount == 2)
            {
                moves++;
                isCheckingPair = true;
                secondCoordinate = coordinates;
                secondBox = box;

                if (game.IsPair(firstCoordinate, secondCoordinate))
                {
                    game.PairedCard(firstCoordinate);
                    game.PairedCard(secondCoordinate);

                    if (game.AllFound())
                    {
                        statusLabel.Text = "Niveau terminé ! Félicitations !";
                        gameTimer.Stop();
                        JouerSon("Success.wav", true);
                    }
                    else
                    {
                        statusLabel.Text = "Paire trouvée !";
                        JouerSon("Paired.wav");
                    }

                    selectedCount = 0;
                    firstBox = null;
                    secondBox = null;
                    isCheckingPair = false;
                }
                else
                {
                    statusLabel.Text = "Ce n'est pas une paire";
                    JouerSon("Wrong.wav");

                    Timer hideTimer = new Timer();
                    hideTimer.Interval = 1500;

                    PictureBox fb = firstBox;
                    PictureBox sb = secondBox;
                    string coord1 = firstCoordinate;
                    string coord2 = secondCoordinate;

                    hideTimer.Tick += (s, args) =>
                    {
                        game.HideCard(coord1);
                        game.HideCard(coord2);

                        if (fb != null) fb.Image = backCardImage;
                        if (sb != null) sb.Image = backCardImage;

                        hideTimer.Stop();
                        hideTimer.Dispose();

                        selectedCount = 0;
                        firstBox = null;
                        secondBox = null;
                        isCheckingPair = false;

                        statusLabel.Text = "Sélectionnez deux cartes";
                    };

                    hideTimer.Start();
                }

                UpdateScore();
            }
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            gameTimer?.Stop();
            Application.Exit();
        }

        private void UpdateScore()
        {
            if (scoreLabel != null)
            {
                scoreLabel.Text = $"Essais: {moves}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}