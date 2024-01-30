using System;
using EasyCraft.Abstraction.Instance.Constants;

namespace EasyCraft.Abstraction.Instance
{
    public class InstanceBase
    {
        public required Guid Id { get; init; }
        public string Name { get; set; } = "<Unnamed Instance>";
        public InstanceStatus Status { get; set; } = InstanceStatus.Stopped;
        public required Guid EndpointId { get; set; }
    }
}