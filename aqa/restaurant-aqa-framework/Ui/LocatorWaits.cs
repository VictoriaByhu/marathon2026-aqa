using System.Diagnostics;
using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Configuration;

namespace Restaurant_Aqa_Framework.Ui;

public static class LocatorWaits
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);

    public static Task<bool> SettlesOnCountAsync(this ILocator locator, int expected) =>
        UntilAsync(async () => await locator.CountAsync() == expected);

    public static Task<bool> BecomesVisibleAsync(this ILocator locator) =>
        UntilAsync(() => locator.IsVisibleAsync());

    private static async Task<bool> UntilAsync(Func<Task<bool>> condition)
    {
        var deadline = Stopwatch.StartNew();
        var timeout = TimeSpan.FromMilliseconds(Settings.Current.DefaultTimeoutMs);

        while (true)
        {
            if (await condition())
                return true;
            if (deadline.Elapsed > timeout)
                return false;

            await Task.Delay(PollInterval);
        }
    }
}
