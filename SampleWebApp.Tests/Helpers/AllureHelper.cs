using Allure.Net.Commons;
using Microsoft.Playwright;

namespace SampleWebApp.Tests.Helpers;

public static class AllureHelper
{
    public static void Step(string name, Action action)
    {
        AllureApi.Step(name, action);
    }

    public static async Task StepAsync(string name, Func<Task> action)
    {
        await AllureApi.Step(name, action);
    }

    public static T Step<T>(string name, Func<T> action)
    {
        return AllureApi.Step(name, action);
    }

    public static async Task<T> StepAsync<T>(string name, Func<Task<T>> action)
    {
        return await AllureApi.Step(name, action);
    }

    public static async Task AttachScreenshot(IPage page, string name = "Screenshot")
    {
        try
        {
            var screenshot = await page.ScreenshotAsync();
            AllureApi.AddAttachment(name, "image/png", screenshot, ".png");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to capture screenshot: {ex.Message}");
        }
    }

    public static void AttachText(string name, string content, string type = "text/plain")
    {
        AllureApi.AddAttachment(name, type, System.Text.Encoding.UTF8.GetBytes(content), ".txt");
    }

    public static void AttachJson(string name, object obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        AllureApi.AddAttachment(name, "application/json", System.Text.Encoding.UTF8.GetBytes(json), ".json");
    }

    public static async Task AttachVideo(string videoPath, string name = "Test Video")
    {
        try
        {
            if (File.Exists(videoPath))
            {
                var videoBytes = await File.ReadAllBytesAsync(videoPath);
                AllureApi.AddAttachment(name, "video/webm", videoBytes, ".webm");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to attach video: {ex.Message}");
        }
    }
}
