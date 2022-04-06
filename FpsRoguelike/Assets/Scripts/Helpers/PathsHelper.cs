using System.IO;
using UnityEngine;

public static class PathsHelper
{
    // Input actions json path
    public static readonly string REBINDS_PATH = Path.Combine(Application.persistentDataPath, "rebindsSave.json");
}
