using System;

namespace RaceSharp.Application.WorkFlowCore
{
    public class NewDefinitionCommand
    {
        public Guid Originator { get; set; }
        public string DefinitionId { get; set; }
        public int Version { get; set; }
    }
}
