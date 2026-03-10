using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public class DialogData
    {
        [SerializeField] private string _localizeKey;
        [SerializeField] private string[] _sentences;

        public string LocalizeKey
        {
            get { return _localizeKey; }
          //  set { _localizeKey = value; }
        }
        public string[] Sentences
        {
            get { return _sentences; }
            set { _sentences = value; }
        }
    }
}

