using System;
using System.Windows.Forms;
using System.Drawing;

namespace Memory
{
    public class MenuPrincipal : Form
    {
        private Button? playButton;
        private Button? optionsButton;
        private Button? quitButton;
        private Label? titleLabel;

        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Memory - Menu Principal";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 40);
            this.DoubleBuffered = true;

            // Titre
            titleLabel = new Label();
            titleLabel.Text = " MEMORY CYBERSÉCURITÉ ";
            titleLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.Cyan;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(50, 50);
            titleLabel.Width = 500;
            titleLabel.Height = 80;
            this.Controls.Add(titleLabel);

            // Bouton Jouer
            playButton = new Button();
            playButton.Text = "▶ JOUER";
            playButton.Font = new Font("Arial", 14, FontStyle.Bold);
            playButton.Width = 300;
            playButton.Height = 60;
            playButton.Left = (this.Width - playButton.Width) / 2;
            playButton.Top = 170;
            playButton.BackColor = Color.LimeGreen;
            playButton.ForeColor = Color.Black;
            playButton.FlatStyle = FlatStyle.Flat;
            playButton.Click += PlayButton_Click;
            this.Controls.Add(playButton);

            // Bouton Options
            optionsButton = new Button();
            optionsButton.Text = "⚙ OPTIONS";
            optionsButton.Font = new Font("Arial", 14, FontStyle.Bold);
            optionsButton.Width = 300;
            optionsButton.Height = 60;
            optionsButton.Left = (this.Width - optionsButton.Width) / 2;
            optionsButton.Top = 250;
            optionsButton.BackColor = Color.Gold;
            optionsButton.ForeColor = Color.Black;
            optionsButton.FlatStyle = FlatStyle.Flat;
            optionsButton.Click += OptionsButton_Click;
            this.Controls.Add(optionsButton);

            // Bouton Quitter
            quitButton = new Button();
            quitButton.Text = "✕ QUITTER";
            quitButton.Font = new Font("Arial", 14, FontStyle.Bold);
            quitButton.Width = 300;
            quitButton.Height = 60;
            quitButton.Left = (this.Width - quitButton.Width) / 2;
            quitButton.Top = 330;
            quitButton.BackColor = Color.IndianRed;
            quitButton.ForeColor = Color.White;
            quitButton.FlatStyle = FlatStyle.Flat;
            quitButton.Click += QuitButton_Click;
            this.Controls.Add(quitButton);
        }

        private void PlayButton_Click(object? sender, EventArgs e)
        {
            Form1 gameForm = new Form1(4);
            gameForm.Show();
            this.Hide();
        }

        private void OptionsButton_Click(object? sender, EventArgs e)
        {
            OptionsForm optionsForm = new OptionsForm();
            optionsForm.ShowDialog();
        }

        private void QuitButton_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
