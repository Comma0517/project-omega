using System;

namespace EnvironmentSettings.Interface
{
    public interface IEnvSettings
    {
        void AddSettings<TEnum>() where TEnum : Enum;
        string GetString(Enum e);
        string GetString(Enum e, string defaultValue);
        int GetInt(Enum e);
        int? GetInt(Enum e, int? defaultValue);
        bool GetBool(Enum e);
        bool GetBool(Enum e, bool defaultValue);
        string GetString(string name);
        string GetString(string name, string defaultValue);
        int GetInt(string name);
        int? GetInt(string name, int? defaultValue);
        bool GetBool(string name);
        bool GetBool(string name, bool defaultValue);
        string GetAllAsSafeLogString();
        bool IsLocal();
    }
}
