using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Memory;
using Timer = System.Windows.Forms.Timer;

namespace Memory
{
    public partial class Form1 : Form
    {
        private Liste? game;
        private PictureBox[,]? cardBoxes;
        private Label? scoreLabel;
        private Label? timerLabel;
        private Label? statusLabel;
        private Button? menuButton;
        private int moves = 0;
        private int pairs = 0;
        private int selectedCount = 0;
        private string firstCoordinate = "";
        private PictureBox? firstBox = null;
        private PictureBox? secondBox = null;
        private bool isCheckingPair = false;
        private Panel? gamePanel;
        private Timer? gameTimer;
        private int seconds = 0;
        private int gridSize = 4;
        private Bitmap? backCardImage;

        public Form1(int size = 4)
        {
            gridSize = size;
            this.Text = $"Memory - Grille {size}×{size}";
            this.Width = 1000;
            this.Height = 900;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 40);

            CreateBackCardImage();
            CreateUI();
            StartNewGame();
        }

        private void CreateBackCardImage()
        {
            backCardImage = new Bitmap(80, 80);
            Graphics g = Graphics.FromImage(backCardImage);
            g.Clear(Color.SteelBlue);
            g.DrawString("?", new Font("Arial", 30, FontStyle.Bold), Brushes.White, 25, 20);
            g.Dispose();
        }

        private void CreateUI()
        {
            // Panel supérieur avec infos
            Panel infoPanel = new Panel();
            infoPanel.Location = new Point(0, 0);
            infoPanel.Width = this.Width;
            infoPanel.Height = 60;
            infoPanel.BackColor = Color.FromArgb(30, 30, 60);
            this.Controls.Add(infoPanel);

            // Label Score
            scoreLabel = new Label();
            scoreLabel.Text = "Essais: 0 | Paires: 0/0";
            scoreLabel.Location = new Point(20, 15);
            scoreLabel.AutoSize = true;
            scoreLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            scoreLabel.ForeColor = Color.Cyan;
            infoPanel.Controls.Add(scoreLabel);

            // Label Chronomètre
            timerLabel = new Label();
            timerLabel.Text = "Temps: 00:00";
            timerLabel.Location = new Point(400, 15);
            timerLabel.Width = 150;
            timerLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            timerLabel.ForeColor = Color.Lime;
            infoPanel.Controls.Add(timerLabel);

            // Label Statut
            statusLabel = new Label();
            statusLabel.Text = "Sélectionnez deux cartes";
            statusLabel.Location = new Point(600, 15);
            statusLabel.Width = 250;
            statusLabel.Font = new Font("Arial", 10);
            statusLabel.ForeColor = Color.White;
            infoPanel.Controls.Add(statusLabel);

            // Bouton Menu
            menuButton = new Button();
            menuButton.Text = "↶ Menu";
            menuButton.Location = new Point(880, 15);
            menuButton.Width = 80;
            menuButton.Height = 30;
            menuButton.BackColor = Color.Orange;
            menuButton.FlatStyle = FlatStyle.Flat;
            menuButton.Click += MenuButton_Click;
            infoPanel.Controls.Add(menuButton);

            // Panel pour les cartes
            gamePanel = new Panel();
            gamePanel.Location = new Point(50, 80);
            gamePanel.Width = 900;
            gamePanel.Height = 750;
            gamePanel.BackColor = Color.DarkGray;
            this.Controls.Add(gamePanel);

            // le chronomètre
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            seconds++;
            int minutes = seconds / 60;
            int secs = seconds % 60;
            timerLabel!.Text = $"Temps: {minutes:D2}:{secs:D2}";
        }

        private void StartNewGame()
        {
            if (game != null)
            {
                game = null;
            }

            List<string> imagePaths = new List<string>();
            int pairCount = (gridSize == 4) ? 8 : 18;
            for (int i = 1; i <= pairCount; i++)
            {
                imagePaths.Add(i.ToString());
            }

            game = new Liste(imagePaths);
            cardBoxes = new PictureBox[game.Rows, game.Columns];
            moves = 0;
            pairs = 0;
            selectedCount = 0;
            firstCoordinate = "";
            firstBox = null;
            secondBox = null;
            isCheckingPair = false;
            seconds = 0;

            gamePanel?.Controls.Clear();
            CreateCardBoxes(gamePanel);
            UpdateScore();
        }

        private void CreateCardBoxes(Panel? panel)
        {
            if (panel == null || game == null) return;

            int boxSize = 80;
            int spacing = 10;

            for (int i = 0; i < game.Rows; i++)
            {
                for (int j = 0; j < game.Columns; j++)
                {
                    PictureBox box = new PictureBox();
                    box.Width = boxSize;
                    box.Height = boxSize;
                    box.Left = j * (boxSize + spacing) + spacing;
                    box.Top = i * (boxSize + spacing) + spacing;
                    box.Image = backCardImage;
                    box.SizeMode = PictureBoxSizeMode.StretchImage;
                    box.BorderStyle = BorderStyle.FixedSingle;
                    box.Tag = $"{i}:{j}";
                    box.Click += CardBox_Click;
                    box.Cursor = Cursors.Hand;

                    if (cardBoxes != null)
                    {
                        cardBoxes[i, j] = box;
                    }
                    panel.Controls.Add(box);
                }
            }
        }

        private void CardBox_Click(object? sender, EventArgs e)
        {
            if (isCheckingPair || game == null || cardBoxes == null) return;

            PictureBox? box = sender as PictureBox;
            if (box == null) return;

            string coordinates = (string?)box.Tag ?? "";
            string[] parts = coordinates.Split(':');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);

            Carte card = game.Cards[row][col];

            if (card.Status == Carte.State.Trouvée) return;
            if (card.Status == Carte.State.Révélée) return;

            card.Reveal();

            // Affiche le numéro sur la PictureBox
            Bitmap cardImage = new Bitmap(box.Width, box.Height);
            Graphics g = Graphics.FromImage(cardImage);
            g.Clear(Color.LimeGreen);
            g.DrawString(card.ImagePath, new Font("Arial", 20, FontStyle.Bold), Brushes.Black, 15, 20);
            g.Dispose();

            box.Image = cardImage;
            box.SizeMode = PictureBoxSizeMode.StretchImage;

            selectedCount++;

            if (selectedCount == 1)
            {
                firstCoordinate = coordinates;
                firstBox = box;
                statusLabel!.Text = "Sélectionnez la deuxième carte";
            }
            else if (selectedCount == 2)
            {
                moves++;
                isCheckingPair = true;

                string secondCoordinate = coordinates;
                secondBox = box;

                if (game.IsPair(firstCoordinate, secondCoordinate))
                {
                    game.PairedCard(firstCoordinate);
                    game.PairedCard(secondCoordinate);
                    pairs++;

                    statusLabel!.Text = " Paire trouvée!";

                    if (pairs == game.AllCards.Count / 2)
                    {
                        statusLabel!.Text = $"Gagné en {moves} essais et {timerLabel!.Text.Substring(7)}!";
                        gameTimer!.Stop();
                        menuButton!.Enabled = true;
                    }

                    selectedCount = 0;
                    firstCoordinate = "";
                    firstBox = null;
                    secondBox = null;
                    isCheckingPair = false;
                    UpdateScore();
                }
                else
                {
                    statusLabel!.Text = " Ce n'est pas une paire";

                    Timer hideTimer = new Timer();
                    hideTimer.Interval = 1500;
                    PictureBox? fb = firstBox;
                    PictureBox? sb = secondBox;
                    hideTimer.Tick += (s, args) =>
                    {
                        game.HideCard(firstCoordinate);
                        game.HideCard(secondCoordinate);

                        if (fb != null)
                        {
                            fb.Image = backCardImage;
                            fb.SizeMode = PictureBoxSizeMode.StretchImage;
                        }

                        if (sb != null)
                        {
                            sb.Image = backCardImage;
                            sb.SizeMode = PictureBoxSizeMode.StretchImage;
                        }

                        hideTimer.Stop();
                        hideTimer.Dispose();

                        selectedCount = 0;
                        firstCoordinate = "";
                        firstBox = null;
                        secondBox = null;
                        isCheckingPair = false;

                        statusLabel!.Text = "Sélectionnez deux cartes";
                    };
                    hideTimer.Start();
                }

                UpdateScore();
            }
        }

        private void MenuButton_Click(object? sender, EventArgs e)
        {
            gameTimer!.Stop();
            this.Close();
        }

        private void UpdateScore()
        {
            if (scoreLabel != null && game != null)
            {
                int totalPairs = game.AllCards.Count / 2;
                scoreLabel.Text = $"Essais: {moves} | Paires: {pairs}/{totalPairs}";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}
