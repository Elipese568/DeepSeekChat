using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Models;

public enum AnalyzeStatus
{
    Analyzing,
    Already
}

public enum FileType
{
    Document,
    Media
}

public class FileModel
{
    public string Name { get; set; }
    public ulong Size { get; set; }
    public string Content { get; set; }

    public AnalyzeStatus Status { get; set; }
    public FileType Type { get; set; }
    public bool IsActive { get; set; } = false; // Select to upload with current message or not, if not, agent will ask to get content

    public Uri CopiedFileUri { get; init; }
}
