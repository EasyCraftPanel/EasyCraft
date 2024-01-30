using System;
using EasyCraft.Abstraction.Endpoint.Constants;

namespace EasyCraft.Abstraction.Endpoint;

public abstract class EndpointBase
{
    public required Guid Id { get; init; }
    public string Name { get; set; } = "<Unnamed Endpoint>";
    public required EndpointSystemType SystemType { get; init; }
}