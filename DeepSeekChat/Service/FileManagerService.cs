using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DeepSeekChat.Service;

[JsonStorageFile(FileName = "fileIndex.json")]
public class FileManagerService : JsonSeriailizingServiceBase<Dictionary<string, string>>
{
    private StorageFolder _storageFolder;
    public FileManagerService()
    {
        Initialize();
    }

    private void Initialize()
    {
        _storageFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync("RequiredFiles", CreationCollisionOption.OpenIfExists).AsTask().GetAwaiter().GetResult();
        
    }

    private string GenerateFileKey(string discussionID, string fileName)
    {
        return $"{discussionID}_{fileName}";
    }

    public async Task<FileModel> CreateFileReferenceAsync(StorageFile origin, string discussionID, FileType type)
    {
        var copiedFile = await origin.CopyAsync(_storageFolder, discussionID + origin.Path.GetHashCode().ToString() + origin.Name);
        string resultFilename = origin.Name;
        string key = GenerateFileKey(discussionID, resultFilename);
        

        if (_data.ContainsKey(key))
        {
            int retryCount = 1;
            do
            {
                resultFilename = $"{origin.Name} ({retryCount})";
                key = GenerateFileKey(discussionID, resultFilename);
            }
            while(_data.ContainsKey(key));
        }

        _data.Add(key, copiedFile.Path);

        return new FileModel()
        {
            Name = resultFilename,
            Size = (await copiedFile.GetBasicPropertiesAsync()).Size,
            Status = AnalyzeStatus.Analyzing,
            Content = string.Empty,
            Type = type,
            CopiedFileUri = new(copiedFile.Path)
        };
    }

    public async Task<StorageFile> GetStorageFileAsync(string discussionID, string fileName)
    {
        string key = GenerateFileKey(discussionID, fileName);
        if (_data.TryGetValue(key, out var filePath))
        {
            return await StorageFile.GetFileFromPathAsync(filePath);
        }
        throw new KeyNotFoundException($"File with key {key} not found.");
    }

    public async Task RemoveFileReferenceAsync(string discussionID, string fileName)
    {
        string key = GenerateFileKey(discussionID, fileName);
        if (_data.ContainsKey(key))
        {
            var file = await GetStorageFileAsync(discussionID, fileName);
            _data.Remove(key);
            await file.DeleteAsync();
            return;
        }
        
        throw new KeyNotFoundException($"File with key {key} not found.");
    }
}
