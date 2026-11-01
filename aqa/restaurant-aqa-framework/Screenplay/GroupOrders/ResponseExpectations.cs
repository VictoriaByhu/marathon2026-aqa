using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Contexts;

namespace Restaurant_Aqa_Framework.Screenplay.GroupOrders;

public static class ResponseExpectations
{
    public static async Task<T> ExpectSuccessAsync<T>(this ApiResponseContext context, IAPIResponse response, string what)
    {
        await context.ExpectSuccessAsync(response, what);
        return context.Json<T>();
    }

    public static async Task ExpectSuccessAsync(this ApiResponseContext context, IAPIResponse response, string what)
    {
        await context.CaptureAsync(response);
        if (!context.Succeeded)
            throw new InvalidOperationException($"{what} failed with {context.Status}: {context.Body}");
    }
}
