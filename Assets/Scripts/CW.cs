using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CW
{

    [Serializable]
    public class Crossword
    {
        public Size size;
        public string[] grid;
        public int[] gridnums;
        public Answers answers;
        public Clues clues;
        public Indices[] gridIndices;
    }

    [Serializable]
    public class Size
    {
        public int cols;
        public int rows;
    }

    [Serializable]
    public class Answers
    {
        public string[] across;
        public string[] down;
    }

    [Serializable]
    public class Clues
    {
        public string[] across;
        public string[] down;

    }

    [Serializable]
    public class Indices
    {
        public int across;
        public int down;
    }



}
