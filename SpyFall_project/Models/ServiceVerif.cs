using System;
using System.Collections.Generic;

namespace SpyFall_project.Models;

/// <summary>
/// A class that contains all information about the service test.
///  - send message
///  - what the response should contain
///  - with what the response should start
///  - what length the response should have
///  - the size of the sent message
/// </summary>
public partial class ServiceVerif
{
    public int ServId { get; set; }

    public string Name { get; set; } = null!;

    public byte[]? SendMessage { get; set; }

    public byte[]? ContainResponse { get; set; }

    public byte[]? StartResponse { get; set; }

    public int MinLengthResponse { get; set; }

    public int? SendSize { get; set; } 
}
