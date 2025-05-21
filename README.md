# 🗂️ EasySave - Version 2.0

EasySave V1 est une application **console** développée avec **.NET Core** permettant de gérer des travaux de sauvegarde.
EasySave V2 constitue la continuité du dévelopmement et implémente une interface graphique ainsi que de multiples ajustements et features.

---

## ✅ Fonctionnalités principales

- Un **travail de sauvegarde** est défini par :
  - Un nom de sauvegarde
  - Un répertoire source
  - Un répertoire cible
  - Un type de sauvegarde :
    - Sauvegarde **complète**
    - Sauvegarde **différentielle**
- Utilisation possible par des **utilisateurs francophones et anglophones**
- Exécution d’un ou plusieurs travaux

---

## 📂 Emplacements des répertoires supportés

- Disques locaux
- Disques externes
- Lecteurs réseau

Tous les fichiers et sous-dossiers doivent être sauvegardés.

---

## 📝 Fichier log journalier

- Écrit en **temps réel** toutes les actions (transferts, créations de répertoire…)
- Format : **JSON** ou **XML**
- Informations requises :
  - Horodatage
  - Nom de la sauvegarde
  - Chemin complet du fichier source (format UNC)
  - Chemin complet du fichier de destination (format UNC)
  - Taille du fichier
  - Temps de transfert (ms) ou valeur négative en cas d’erreur

> ⚠️ Fonctionnalité implémentée dans une **DLL** pour réutilisation dans d'autres projets. Elle doit rester **compatible avec la version 1.0**.

---

## 📊 Fichier d’état en temps réel

- Stocke l’avancement des sauvegardes en **temps réel**
- Fichier unique au **format JSON** ou **XML**
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
 
---
