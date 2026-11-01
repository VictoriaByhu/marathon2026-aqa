using System.Text;
using System.Text.Json;

namespace Restaurant_Aqa_Framework.Screenplay.GroupOrders;

public static class JwtClaims
{
    public static string SubjectOf(string idToken)
    {
        var parts = idToken.Split('.');
        if (parts.Length < 2)
            throw new InvalidOperationException("The id token is not a JWT.");

        var payload = parts[1].Replace('-', '+').Replace('_', '/');
        payload = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');

        using var document = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
        return document.RootElement.GetProperty("sub").GetString()
            ?? throw new InvalidOperationException("The id token has no subject.");
    }
}
