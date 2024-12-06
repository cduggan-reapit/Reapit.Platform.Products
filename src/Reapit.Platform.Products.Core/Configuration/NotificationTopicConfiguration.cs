﻿using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Core.Configuration;

/// <summary>Options model containing notification topic configuration.</summary>
public class NotificationTopicConfiguration
{
    /// <summary>The topic ARN.</summary>
    [property: JsonPropertyName("Arn")] 
    public string? Arn { get; init; }
}