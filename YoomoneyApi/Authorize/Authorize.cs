using YoomoneyApi.Exeption;
using static System.Console;
namespace YoomoneyApi.Authorize;

public class Authorize
{
    public string? TokenUrl { get; set; }
    public string AuthorizeUrl { get; set; }

    // Метод авторизации
    public Authorize(string clientId, string redirectUri, string[] scope)
    {
        // Формируем URL-адрес для запроса авторизации
        AuthorizeUrl = $"https://yoomoney.ru/oauth/authorize?client_id={clientId}&response_type=code" +
                       $"&redirect_uri={redirectUri}&scope={string.Join(" ", scope).Replace(" ", "%20")}";
        
        // Выводим сообщение для пользователя с URL-адресом авторизации
        WriteLine("\nVisit this website and confirm the application authorization request:");
        WriteLine($"{AuthorizeUrl}");
    }
    
    /// <summary>
    /// Метод для получения токена доступа
    /// </summary>
    /// <param name="code"></param>
    /// <param name="clientId"></param>
    /// <param name="redirectUri"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    public async Task<string?> GetAccessToken(string? code,string clientId,string redirectUri, string? clientSecret = null)
    {
        // Проверяем, что введенный код не является пустым и содержит "code="
        if (!string.IsNullOrEmpty(code))
        {
            // Извлекаем код авторизации из строки и удаляем пробелы
            code = code.Substring(code.IndexOf("code=", StringComparison.Ordinal) + 5).Replace(" ", "");
        }
        else
        {
            throw new AppException(message: "EmptyRedirectUrlCode");
        }
        
        // Формируем URL-адрес для запроса токена
        var tokenUrl = $"https://yoomoney.ru/oauth/token?code={code}&client_id={clientId}" +
                       $"&grant_type=authorization_code&redirect_uri={redirectUri}" + (clientSecret != null ? $"&client_secret={clientSecret}" : "");
        
        using HttpClient httpClient = new();
        // Выполняем асинхронный POST-запрос к указанному URL (tokenUrl) без тела запроса
        var response = await httpClient.PostAsync(tokenUrl, null);
        // Проверяем, что ответ от сервера имеет успешный статус (HTTP 200 OK)
        if (response.IsSuccessStatusCode)
        {
            // Считываем содержимое ответа в виде строки
            var content = await response.Content.ReadAsStringAsync();
            // Пытаемся десериализовать JSON-строку в словарь (Dictionary) с ключами и значениями строк
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            // Проверяем, что json не является пустым и содержит ключ "access_token"
            if (json is not null && json.TryGetValue("access_token", out var accessToken))
            {
                // Если ключ "access_token" найден, возвращаем значение токена доступа
                TokenUrl = accessToken;
                return TokenUrl;
            }
        }
        else
        {
            // Если ответ не успешный, проверяем наличие ошибки в содержимом ответа
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(errorContent))
            {
                // Пытаемся десериализовать JSON-строку с ошибкой
                var errorJson = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(errorContent);
                if (errorJson is not null && errorJson.TryGetValue("error", out var errorMessage))
                {
                    // Если ошибка найдена, выбрасываем исключение с сообщением об ошибке
                    throw new AppException(message: errorMessage);
                }
            }
        }
        return null;
    }
}