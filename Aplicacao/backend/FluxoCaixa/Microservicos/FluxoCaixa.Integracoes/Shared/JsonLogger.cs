using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FluxoCaixa.Integracoes.Shared;

public static class JsonLogger
{
    // 🔹 Criamos uma única instância estática de JsonSerializerOptions para melhorar a performance
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 🔹 Evita caracteres Unicode escapados
    };

    private static string GetMicroserviceName()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name ?? "microservico-desconhecido";
    }

    private static string GetLogFilePath()
    {
        string logDirectory = "logs";
        string microserviceName = GetMicroserviceName().Replace(".", "-").ToLower(); // 🔹 Padroniza o nome
        string logFileName = $"{microserviceName}_{DateTime.UtcNow:yyyy-MM-dd}.log"; // 🔹 Arquivo por microserviço e data
        return Path.Combine(logDirectory, logFileName);
    }

    public static void Log(string nivel, string mensagem, object? detalhes = null)
    {
        var logEntry = new
        {
            timestamp = DateTime.UtcNow.ToString("o"),
            microservico = GetMicroserviceName(),
            nivel,
            mensagem,
            detalhes
        };

        string logJson = JsonSerializer.Serialize(logEntry, _jsonOptions);

        try
        {
            string logFilePath = GetLogFilePath();
            Directory.CreateDirectory("logs"); // Garante que o diretório existe
            File.AppendAllText(logFilePath, logJson + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Erro de Log] Falha ao escrever log: {ex.Message}");
        }

        Console.WriteLine(logJson);
    }
}

