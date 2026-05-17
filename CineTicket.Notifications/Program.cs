using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

// Args: <destinatarios_separados_por_coma>
if (args.Length < 1)
{
    Console.Error.WriteLine("Uso: CineTicket.Notifications <comma_emails>");
    Environment.Exit(1);
}

var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.Error.WriteLine("RESEND_API_KEY no está definido en el entorno");
    Environment.Exit(2);
}

// Leer plantilla desde el directorio de trabajo (igual que el script Node)
var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "scripts", "mail-template.html");
if (!File.Exists(templatePath))
{
    Console.Error.WriteLine($"Plantilla no encontrada en: {templatePath}");
    Environment.Exit(5);
}

var html = File.ReadAllText(templatePath);

// Variables de entorno del pipeline
var buildStatus = Environment.GetEnvironmentVariable("BUILD_STATUS") ?? "UNKNOWN";
var isSuccess   = buildStatus.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
var jobName     = Environment.GetEnvironmentVariable("JOB_NAME")     ?? "N/A";
var stageName   = Environment.GetEnvironmentVariable("STAGE_NAME")   ?? "N/A";
var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "N/A";
var buildUrl    = Environment.GetEnvironmentVariable("BUILD_URL")    ?? "#";

var placeholders = new Dictionary<string, string>
{
    ["{{project}}"]   = jobName,
    ["{{status}}"]    = isSuccess ? "Despliegue Exitoso" : "Fallo en el Pipeline",
    ["{{stage}}"]     = stageName,
    ["{{buildId}}"]   = buildNumber,
    ["{{color}}"]     = isSuccess ? "#2ecc71" : "#e74c3c",
    ["{{url}}"]       = buildUrl,
    ["{{commitMsg}}"] = "Revisar logs para detalles del commit"
};

foreach (var (key, value) in placeholders)
    html = html.Replace(key, value);

var to = args[0]
    .Split(',')
    .Select(s => s.Trim())
    .Where(s => !string.IsNullOrEmpty(s))
    .ToArray();

var payload = new
{
    from    = "alertas@angdev.tech",
    to,
    subject = $"[{jobName}] Status: {buildStatus}",
    html
};

using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

var body     = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
var response = await client.PostAsync("https://api.resend.com/emails", body);
var text     = await response.Content.ReadAsStringAsync();

if (!response.IsSuccessStatusCode)
{
    Console.Error.WriteLine($"Error enviando correo: {(int)response.StatusCode} {text}");
    Environment.Exit(3);
}

Console.WriteLine($"Correo enviado: {text}");
