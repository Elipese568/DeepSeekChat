using DeepSeekChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class FileViewModel : WrapperViewModelBase<FileModel>
{
    public FileViewModel(FileModel wrapped) : base(wrapped)
    {
    }

    public string Name
    {
        get { return _innerObject.Name; }
        set
        {
            _innerObject.Name = value;
            OnPropertyChanged();
        }
    }

    public ulong Size
    {
        get { return _innerObject.Size; }
        set
        {
            _innerObject.Size = value;
            OnPropertyChanged();
        }
    }

    public string Content
    {
        get { return _innerObject.Content; }
        set
        {
            _innerObject.Content = value;
            OnPropertyChanged();
        }
    }

    public bool FileIsActive
    {
        get { return _innerObject.IsActive; }
        set
        {
            _innerObject.IsActive = value;
            OnPropertyChanged();
        }
    }

    public AnalyzeStatus Status
    {
        get { return _innerObject.Status; }
        set
        {
            _innerObject.Status = value;
            OnPropertyChanged();
        }
    }

    public FileType Type
    {
        get { return _innerObject.Type; }
        set
        {
            _innerObject.Type = value;
            OnPropertyChanged();
        }
    }
}

public class FilesViewModel : WrapperViewModelBase<List<FileModel>>
{
    public FilesViewModel(List<FileModel> wrapped) : base(wrapped)
    {
        FileViewModels = new ObservableCollection<FileViewModel>(wrapped.Select(x => new FileViewModel(x)));
    }
    private ObservableCollection<FileViewModel> _wrapedViewModels;

    public ObservableCollection<FileViewModel> FileViewModels
    {
        get { return _wrapedViewModels; }
        set
        {
            _wrapedViewModels = value;
        }
    }
    public FileViewModel Add(FileModel file)
    {
        _innerObject.Add(file);
        var vm = new FileViewModel(file);
        _wrapedViewModels.Add(vm);
        return vm;
    }
    public void Remove(FileModel file)
    {
        _innerObject.Remove(file);
        _wrapedViewModels.Remove(_wrapedViewModels.FirstOrDefault(f => f.Name == file.Name && f.Size == file.Size && f.Type == file.Type));
    }
    public int Count => _innerObject.Count;
}