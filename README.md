# 🛡️ Memory SecurIT

**Memory SecurIT** est une application de jeu de mémoire développée en C# avec Windows Forms. Ce projet a été conçu pour le stand de la start-up **SecurIT** lors d'un salon technologique, afin de sensibiliser le public aux enjeux de la cybersécurité de manière ludique.

## 📋 Contexte du Projet
Dans le cadre de cet événement, le jeu permet aux visiteurs de tester leur mémoire tout en se familiarisant avec les icônes et les concepts clés de la sécurité informatique (Pare-feu, Phishing, Malware, etc.).

## 🚀 Fonctionnalités
L'application intègre les fonctionnalités suivantes :
* **Système de jeu complet** : Gestion des paires, comptage des essais et détection automatique de la victoire.
* **Interface Responsive** : La grille de jeu ainsi que la barre d'informations s'adaptent dynamiquement à la taille de la fenêtre.
* **Menu Principal & Options** : Une navigation structurée entre l'accueil, les paramètres et la zone de jeu.
* **Difficulté ajustable** : Possibilité de choisir entre une grille **Facile (4x4)** ou **Difficile (6x6)**.
* **Immersion Sonore** : Intégration d'effets sonores pour le retournement des cartes, la validation des paires et la victoire finale.
* **Thématique Cybersécurité** : Utilisation d'un jeu d'icônes spécifiques représentant les menaces et protections numériques courantes.

## 🛠️ Structure Technique
Le projet repose sur une architecture orientée objet respectant les consignes du barème :
* **`Carte.cs`** : Classe gérant l'état (Cachée, Révélée, Trouvée) et l'identifiant de chaque carte.
* **`Liste.cs`** : Classe contenant la logique métier, notamment le mélange des cartes et la vérification des paires.
* **`Form1.cs`** : Gère l'interface de jeu, l'affichage dynamique des cartes et les interactions utilisateurs.
* **Navigation propre** : Utilisation de `ShowDialog()` pour les options afin de garantir une gestion fluide des fenêtres sans doublons.

## 📦 Installation et Lancement
### Prérequis
* **Visual Studio** (2022 ou version ultérieure recommandée).
* SDK **.NET 10.0** ou supérieur.

### Étapes
1. **Clonez le dépôt** : `git clone [URL_DU_DEPOT]`.
2. **Ouvrez le projet** : Double-cliquez sur le fichier `.sln` dans Visual Studio.
3. **Compilation** : Générez la solution via le menu `Générer > Générer la solution`.
4. **Exécution** : Appuyez sur `F5` pour lancer le Menu Principal.

---
> Ce projet a été réalisé dans le respect des contraintes d'encapsulation et de qualité logicielle définies par SecurIT.