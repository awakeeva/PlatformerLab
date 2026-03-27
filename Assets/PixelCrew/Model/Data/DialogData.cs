using System;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public struct DialogData
    {
        [SerializeField] private string _localizeKey;
        [SerializeField] private Sentence[] _sentences;
        [SerializeField] private DialogType _type;

        public DialogType Type => _type;

        public string LocalizeKey
        {
            get { return _localizeKey; }
          //  set { _localizeKey = value; }
        }

        public Sentence[] Sentences
        {
            get { return _sentences; }
            set { _sentences = value; }
        }
        
    }

    [Serializable]
    public struct Sentence
    {
        [SerializeField] private string _value;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Side _side;

        public string Value => _value;
        public Sprite Icon => _icon;
        public Side Side => _side;
    }

    public enum Side
    {
        Left,
        Right
    }

    public enum DialogType
    {
        Simple,
        Personalized
    }
}

