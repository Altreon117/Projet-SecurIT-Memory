using System;
using System.Collections.Generic;
using Memory;

namespace projet_memory
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> listeCartes = new List<string> { "A", "B"};
            Liste liste = new Liste(listeCartes);

            liste.DisplayBoard();
                Console.WriteLine("Entrez les coordonnées de la première carte à retourner (ex: 0:1) :");
                string coordinates01 = Console.ReadLine();
                liste.RevealCard(coordinates01);
                liste.DisplayBoard();
                Console.WriteLine("Entrez les coordonnées de la deuxième carte à retourner (ex: 1:0) :");
                string coordinates02 = Console.ReadLine();
                while (coordinates01 == coordinates02)
                {
                    Console.WriteLine("Vous avez entré les mêmes coordonnées. Veuillez entrer des coordonnées différentes pour la deuxième carte :");
                    coordinates02 = Console.ReadLine();
                }
                liste.RevealCard(coordinates02);
                liste.DisplayBoard();
                if (liste.IsPair(coordinates01, coordinates02))
                {
                    liste.PairedCard(coordinates01);
                    liste.PairedCard(coordinates02);
                }
                else
                {
                    liste.HideCard(coordinates01);
                    liste.HideCard(coordinates02);
                }

            while (liste.AllFound() == false)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Tour suivant");
                liste.DisplayBoard();
                Console.WriteLine("Entrez les coordonnées de la première carte à retourner (ex: 0:1) :");
                string coordinates1 = Console.ReadLine();
                liste.RevealCard(coordinates1);
                liste.DisplayBoard();
                Console.WriteLine("Entrez les coordonnées de la deuxième carte à retourner (ex: 1:0) :");
                string coordinates2 = Console.ReadLine();
                while (coordinates1 == coordinates2)
                {
                    Console.WriteLine("Vous avez entré les mêmes coordonnées. Veuillez entrer des coordonnées différentes pour la deuxième carte :");
                    coordinates2 = Console.ReadLine();
                }
                liste.RevealCard(coordinates2);
                liste.DisplayBoard();
                if (liste.IsPair(coordinates1, coordinates2))
                {
                    liste.PairedCard(coordinates1);
                    liste.PairedCard(coordinates2);
                }
                else
                {
                    liste.HideCard(coordinates1);
                    liste.HideCard(coordinates2);
                }
            }
            Console.WriteLine("Félicitations, vous avez trouvé toutes les paires !");
        }
    }
}