using UnityEngine;

public static class ServerConfig
{
#if UNITY_EDITOR
    public const string BaseUrl = "http://localhost/KarigurashinoDaniel_Unity/";
#else
    public const string BaseUrl = "https://10.219.32.66/KarigurashinoDaniel_Unity/";
#endif
}