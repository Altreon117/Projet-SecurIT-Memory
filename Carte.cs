using System;

namespace Memory
{
    public class Carte
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public enum State { Cachée, Révélée, Trouvée }
        public State Status { get; set; }

        public Carte(int id, string imagePath)
        {
            Id = id;
            ImagePath = imagePath;
            Status = State.Cachée;
        }

        public void Reveal()
        {
            Status = State.Révélée;
        }

        public void Hide()
        {
            Status = State.Cachée;
        }

        public void Paired()
        {
            Status = State.Trouvée;
        }
    }
}