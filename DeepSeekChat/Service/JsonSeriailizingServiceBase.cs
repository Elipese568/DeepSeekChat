using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace DeepSeekChat.Service;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class JsonStorageFileAttribute : Attribute
{
    public string FileName { get; set; }
    public JsonStorageFileAttribute()
    {
    }
}

public class JsonSeriailizingServiceBase<TData> : ImplementationLifetimeServiceBase
{
    private StorageFile _storage;
    protected TData _data;

    protected override void OnInitialize()
    {
        _storage = ApplicationData.Current.LocalFolder.CreateFileAsync(GetType().GetCustomAttribute<JsonStorageFileAttribute>().FileName, CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
        using var readStream = _storage.OpenStreamForReadAsync().GetAwaiter().GetResult();
        if (readStream.Length == 0)
        {
            _data = Activator.CreateInstance<TData>();
        }
        else
        {
            try
            {
                _data = JsonSerializer.Deserialize<TData>(readStream);
            }
            catch
            {
                _data = Activator.CreateInstance<TData>();
            }
        }
        base.OnInitialize();
    }

    protected override void OnDispose()
    {
        var serializedStream = _storage.OpenStreamForWriteAsync().GetAwaiter().GetResult();
        try
        {
            JsonSerializer.Serialize(serializedStream, _data);
        }
        catch (Exception ex)
        {
            // Handle serialization exception if needed
            System.Diagnostics.Debug.WriteLine($"Serialization error: {ex.Message}");
        }
        finally
        {
            serializedStream.Flush();
            serializedStream.Close();
        }
        base.OnDispose();
    }
}
