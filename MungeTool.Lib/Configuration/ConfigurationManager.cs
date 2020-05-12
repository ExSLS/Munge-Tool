using System;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;

namespace MungeTool.Lib.Configuration
{
    public static class ConfigurationManager
    {
        private static readonly string DefaultCodeRootFolder = @"C:\Code";

        public static Config Config { get; set; }

        private static string MasterConfigFileName => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "", @"Config.yaml");
        private static string UserConfigFileName => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MungeTool", "Config.yaml");

        public static void Load()
        {
            LoadMasterConfig();
            LoadUserConfig();
        }

        private static void LoadMasterConfig() =>
            Config = new DeserializerBuilder()
                .Build()
                .Deserialize<Config>(File.ReadAllText(MasterConfigFileName));

        private static void LoadUserConfig() =>
            Config.UserConfig = File.Exists(UserConfigFileName)
                ? new DeserializerBuilder()
                    .Build()
                    .Deserialize<UserConfig>(File.ReadAllText(UserConfigFileName))
                : new UserConfig {CodeRootFolder = DefaultCodeRootFolder};

        public static void FixupSettings() =>
            // Ensure paths end with backslash
            Config.CodeRootFolders = Config.CodeRootFolders
                .Select(x => x.EndsWith(@"\") ? x : x + @"\")
                .ToArray();

        public static void SaveUserConfig()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(UserConfigFileName));

            File.WriteAllText(UserConfigFileName,
                new SerializerBuilder()
                    .Build()
                    .Serialize(Config.UserConfig));
        }
    }
}
