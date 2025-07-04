﻿using System.Text.Json.Serialization;

namespace Library.WebUI.Dtos;
public class ApiResponse<T> where T : class
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
}

public class ApiResponse
{
    public string ErrorMessage { get; set; }
}
