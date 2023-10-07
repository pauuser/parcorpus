using System.Text;
using Microsoft.AspNetCore.Http;

namespace Parcorpus.Services.Helpers;

/// <summary>
/// Reader for text in IFormFile
/// </summary>
public static class FormStringReader
{
    /// <summary>
    /// Method to read file to string in IFormFile
    /// </summary>
    /// <param name="file">form file</param>
    /// <returns></returns>
    public static string ReadFormFileToString(IFormFile file)
    {
        var result = new StringBuilder();

        using var reader = new StreamReader(file.OpenReadStream());
        while (reader.Peek() >= 0)
            result.AppendLine(reader.ReadLine());
        
        return result.ToString();
    }
}