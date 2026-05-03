using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Media;
using System.IO;
using Timer = System.Windows.Forms.Timer; // LA LIGNE MAGIQUE EST DE RETOUR !

namespace Memory
{
    public partial class Form1 : Form
    {
        private Liste game;
        private PictureBox[,] cardBoxes;
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

        // --- GESTIONNAIRE DE SONS ---
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

        // --- GÉNÉRATION DE L'INTERFACE ---
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
            Panel infoPanel = new Panel();
            infoPanel.Location = new Point(0, 0);
            infoPanel.Width = this.Width;
            infoPanel.Height = 60;
            infoPanel.BackColor = Color.FromArgb(30, 30, 60);
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
            quitButton.Location = new Point(880, 15);
            quitButton.Width = 80;
            quitButton.Height = 30;
            quitButton.BackColor = Color.Red;
            quitButton.ForeColor = Color.White;
            quitButton.FlatStyle = FlatStyle.Flat;
            quitButton.Click += QuitButton_Click;
            infoPanel.Controls.Add(quitButton);

            gamePanel = new Panel();
            gamePanel.Location = new Point(50, 80);
            gamePanel.Width = 900;
            gamePanel.Height = 750;
            gamePanel.BackColor = Color.DarkGray;
            this.Controls.Add(gamePanel);

            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            seconds++;
            int minutes = seconds / 60;
            int secs = seconds % 60;
            timerLabel.Text = $"Temps: {minutes:D2}:{secs:D2}";
        }

        // --- INITIALISATION DU JEU VIA LA CLASSE LISTE ---
        private void StartNewGame()
        {
            soundPlayer?.Stop();

            // On prépare les valeurs uniques (ta classe Liste va les doubler)
            List<string> imagePaths = new List<string>();
            int pairCount = (gridSize == 4) ? 8 : 18;

            for (int i = 1; i <= pairCount; i++)
            {
                imagePaths.Add(i.ToString());
            }

            // Instanciation via ta logique métier
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

            gamePanel.Controls.Clear();
            CreateCardBoxes(gamePanel);
            UpdateScore();
        }

        private void CreateCardBoxes(Panel panel)
        {
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

                    // Le Tag correspond parfaitement au format attendu par tes méthodes (ex: "0:1")
                    box.Tag = $"{i}:{j}";
                    box.Click += CardBox_Click;
                    box.Cursor = Cursors.Hand;

                    cardBoxes[i, j] = box;
                    panel.Controls.Add(box);
                }
            }
        }

        // --- LOGIQUE DE CLIC UTILISANT TES MÉTHODES ---
        private void CardBox_Click(object sender, EventArgs e)
        {
            if (isCheckingPair || game == null) return;

            PictureBox box = sender as PictureBox;
            if (box == null) return;

            string coordinates = box.Tag.ToString();
            string[] parts = coordinates.Split(':');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);

            // On accède à ta classe Carte pour vérifier son statut
            Carte card = game.Cards[row][col];
            if (card.Status != Carte.State.Cachée) return;

            // Utilisation de TA méthode pour révéler
            game.RevealCard(coordinates);
            JouerSon("Flipped_Card.wav");

            // Mise à jour visuelle
            Bitmap cardImage = new Bitmap(box.Width, box.Height);
            Graphics g = Graphics.FromImage(cardImage);
            g.Clear(Color.LimeGreen);
            g.DrawString(card.ImagePath, new Font("Arial", 20, FontStyle.Bold), Brushes.Black, 15, 20);
            g.Dispose();
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

                // Utilisation de TA méthode pour vérifier la paire
                if (game.IsPair(firstCoordinate, secondCoordinate))
                {
                    // Utilisation de TA méthode pour valider la paire
                    game.PairedCard(firstCoordinate);
                    game.PairedCard(secondCoordinate);

                    // Utilisation de TA méthode pour vérifier la victoire finale
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
                        // Utilisation de TA méthode pour recacher les cartes
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