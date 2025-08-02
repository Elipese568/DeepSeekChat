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
    [Obsolete]public bool IsActive { get; set; } = false; // Select to upload with current message or not, if not, agent will ask to get content
    // Notice: It will not be used in streaming mode, because streaming output with tool calls is
    // I'll give up because deepseek does not support tool calls yet. ()

    public Uri CopiedFileUri { get; init; }
}
