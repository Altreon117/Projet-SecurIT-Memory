using System;

namespace Memory
{
    public class Carte
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string State { get; set; }
        
        public Carte(int id, string imagePath)
        {
            Id = id;
            ImagePath = imagePath;
            State = "Cachée";
        }

        public void Reveal()
        {
            State = "Révélée";
        }

        public void Hide()
        {
            State = "Cachée";
        }

        public void Paired()
        {
            State = "Trouvée";
        }
    }
}