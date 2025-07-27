using MimeDetective;
using MimeDetective.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

public enum CheckFileType { Text, Document, Image, Unknown }

public class FileTypeChecker
{
    private static IContentInspector _inspector;
    private static FileExtensionToMimeTypeLookup _fileExtToMimeTypeLookup;

    static FileTypeChecker()
    {
        _inspector = new ContentInspectorBuilder()
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All(),
        }.Build();

        _fileExtToMimeTypeLookup = new FileExtensionToMimeTypeLookupBuilder()
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        }.Build();
    }

    public static CheckFileType GetFileType(string filePath)
    {
        MimeTypeMatch? result = GetFileMimeType(filePath);

        if (result != null)
        {
            var type = GetFileTypeByMime(result.MimeType);
            if (type != CheckFileType.Unknown)
                return type;
        }

        // 如果内容检测无法确定，则回退到扩展名检测
        return GetFileTypeByExtension(filePath);
    }

    public static MimeTypeMatch? GetFileMimeType(string filePath)
    {
        var results = _inspector.Inspect(File.OpenRead(filePath));
        var result = results.ByMimeType().FirstOrDefault();
        return result;
    }

    private static CheckFileType GetFileTypeByMime(string mimeType)
    {
        if (string.IsNullOrEmpty(mimeType))
            return CheckFileType.Unknown;

        if (mimeType.StartsWith("text/") ||
            mimeType == "application/xml" ||
            mimeType == "application/json" ||
            mimeType == "application/javascript")
            return CheckFileType.Text;

        var documentMimes = new HashSet<string> {
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/rtf",
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.oasis.opendocument.text",
            "application/vnd.oasis.opendocument.spreadsheet",
            "application/vnd.oasis.opendocument.presentation"
        };

        if (documentMimes.Contains(mimeType))
            return CheckFileType.Document;

        if (mimeType.StartsWith("image/"))
            return CheckFileType.Image;

        return CheckFileType.Unknown;
    }

    public static string GetMimeTypeByExtension(string ext)
    {
        return _fileExtToMimeTypeLookup.TryGetValue(ext);
    }

    private static CheckFileType GetFileTypeByExtension(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension))
            return CheckFileType.Unknown;

        var textExtensions = new HashSet<string> {
            ".txt", ".cs", ".js", ".html", ".css", ".json", ".xml",
            ".log", ".md", ".config", ".yaml", ".ini", ".csv", ".bat", ".sh",
            ".sql", ".php", ".py", ".rb", ".java", ".yaml", ".r", ".tpl",
            ".h", ".cpp", ".log", ".properties", ".toml", ".jade", ".scss",
            ".ts", ".yml", ".xaml", ".vim", ".zsh", ".bash", ".cmd"
        };


        var documentExtensions = new HashSet<string> {
            ".docx", ".rtf", ".odt", ".pdf", ".xlsx",
            ".pptx", ".odp", ".ods"
        };

        var imageExtensions = new HashSet<string> {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp",
            ".svg", ".ico", ".psd", ".raw"
        };

        if (textExtensions.Contains(extension)) return CheckFileType.Text;
        if (documentExtensions.Contains(extension)) return CheckFileType.Document;
        if (imageExtensions.Contains(extension)) return CheckFileType.Image;

        return CheckFileType.Unknown;
    }
}