# 🗂️ EasySave - Version 1.0

## 📄 Description du livrable

EasySave est une application **console** développée avec **.NET Core** permettant de gérer des travaux de sauvegarde. Il s'agit de la **version 1.0**.

---

## ✅ Fonctionnalités principales

- Création jusqu’à **5 travaux de sauvegarde**
- Un **travail de sauvegarde** est défini par :
  - Un nom de sauvegarde
  - Un répertoire source
  - Un répertoire cible
  - Un type de sauvegarde :
    - Sauvegarde **complète**
    - Sauvegarde **différentielle**
- Utilisation possible par des **utilisateurs francophones et anglophones**
- Exécution d’un ou plusieurs travaux :
  - Exemple 1 : `1-3` ➜ exécute les sauvegardes 1 à 3
  - Exemple 2 : `1;3` ➜ exécute les sauvegardes 1 et 3

---

## 📂 Emplacements des répertoires supportés

- Disques locaux
- Disques externes
- Lecteurs réseau

Tous les fichiers et sous-dossiers doivent être sauvegardés.

---

## 📝 Fichier log journalier

- Écrit en **temps réel** toutes les actions (transferts, créations de répertoire…)
- Format : **JSON**
- Informations requises :
  - Horodatage
  - Nom de la sauvegarde
  - Chemin complet du fichier source (format UNC)
  - Chemin complet du fichier de destination (format UNC)
  - Taille du fichier
  - Temps de transfert (ms) ou valeur négative en cas d’erreur
- Exemple : `2020-12-17.json`

> ⚠️ Fonctionnalité implémentée dans une **DLL** pour réutilisation dans d'autres projets. Elle doit rester **compatible avec la version 1.0**.

---

## 📊 Fichier d’état en temps réel

- Stocke l’avancement des sauvegardes en **temps réel**
- Fichier unique au **format JSON**
- Informations à enregistrer :
  - Nom du travail
  - Horodatage de la dernière action
  - État du travail (Actif / Non Actif)

### Si le travail est actif :
  - Nombre total de fichiers à sauvegarder
  - Taille totale à transférer
  - Pourcentage de progression
  - Nombre de fichiers restants
  - Taille restante
  - Fichier source en cours
  - Fichier de destination en cours

- Exemple : `state.json`

> ❌ Emplacements comme `C:\temp\` interdits (non compatibles avec les serveurs clients)  
> ✅ Fichiers JSON avec retours à la ligne pour lisibilité dans Notepad  
> ➕ Pagination appréciée

---

## 🚀 Perspective d’évolution

> Si la version 1.0 donne satisfaction, une **version 2.0** avec **interface graphique (architecture MVVM)** sera développée.

