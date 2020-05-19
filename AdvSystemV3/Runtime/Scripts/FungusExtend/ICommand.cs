using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public interface ICommand
    {
        int CSVLine { get; set; }
        string CSVCommandKey { get; set; }
        void InitializeByParams(object[] param);
    }
}