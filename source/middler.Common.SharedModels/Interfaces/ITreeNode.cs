using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace middler.Common.SharedModels.Interfaces
{
    public interface ITreeNode
    {
        Guid Id { get; set; }
        string Parent { get; set; }
        string Name { get; set; }
        string Path { get; }
        bool IsFolder { get; set; }
        string Extension { get; set; }
        JToken Content { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        List<ITreeNode> Children { get; set; }
    }
}