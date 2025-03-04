﻿using System;
using System.Collections.Generic;

namespace Assets.Data.Models
{
    [Serializable]
    public class SavedGame
    {
        public string label;
        public DateTime saveTime;
        public int score;
        public bool hasHighScore;
        public int livesRemaining;
        public List<SerializableVector2> destroyedBrickPositions;
    }
}

