using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ws;

class Singletons
{
    public static List<Config> configs =
        File.Exists("./config.json")
            ? JsonSerializer.Deserialize<List<Config>>(File.ReadAllText("./config.json"),
                new JsonSerializerOptions { IncludeFields = true })!
            : new List<Config>()
            {
                new Config()
                {
                    Header = "default",
                    Host = "127.0.0.1",
                    Port = 25566, Password = String.Empty,
                }
            };
}

public class Config
{
    public string? Header;
    public string? Host;
    public int? Port;
    public string? Password;
}