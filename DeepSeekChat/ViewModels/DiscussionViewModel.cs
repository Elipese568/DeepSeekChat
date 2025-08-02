using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeepSeekChat.Command;
using DeepSeekChat.Helper;
using DeepSeekChat.Helper.Converters;
using DeepSeekChat.Models;
using DeepSeekChat.Service;
using DeepSeekChat.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace DeepSeekChat.ViewModels;

public partial class DiscussionViewModel : ObservableRecipient
{
    private readonly ExecuteAICommand _sendCommand;
    private readonly SettingService _settingService;
    private readonly AvatarManagerService _avatarManagerService;
    private readonly FileManagerService _fileManagerService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputingPrompt;

    [ObservableProperty]
    private AvatarDataViewModel _userAvatarDataViewModel;
    [ObservableProperty]
    private AvatarDataViewModel _aiAvatarDataViewModel;

    [ObservableProperty]
    private FileViewModel? _previewFileViewModel = null;

    public Visibility FileListVisibility => SelectedDiscussItemViewModel.FilesViewModel.FileViewModels.Any() ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ContentVisibility => SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].AiChatCompletion.ContentViewModels.Any() ? Visibility.Visible : Visibility.Collapsed;

    public DiscussionItemViewModel SelectedDiscussItemViewModel { get; set; }

    public DiscussionViewModel(DiscussionItemViewModel item)
    {
        SelectedDiscussItemViewModel = item;
        _settingService = App.Current.GetService<SettingService>();
        _avatarManagerService = App.Current.GetService<AvatarManagerService>();
        _fileManagerService = App.Current.GetService<FileManagerService>();

        _sendCommand = new ExecuteAICommand(item.InnerObject);
        _sendCommand.StreamResponseReceived += OnStreamResponseReceived;
        _sendCommand.StreamCompleted += OnStreamCompleted;
        _sendCommand.FunctionCallingResponseReceived += OnFunctionCalling;
        _sendCommand.CompletionMetadataReceived += OnCompletionMetadataReceived;

        UserAvatarDataViewModel = _avatarManagerService.GetSelectedUserAvatarViewModel();
        AiAvatarDataViewModel = _avatarManagerService.GetSelectedAiAvatarViewModel();

        _avatarManagerService.SelectedUserAvatarChanged += (s, e) =>
        {
            if (e.Type.HasFlag(AvatarType.User))
                UserAvatarDataViewModel = e.ViewModel;
            else
                AiAvatarDataViewModel = e.ViewModel;
        };

        SelectedDiscussItemViewModel.FilesViewModel.FileViewModels.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FileListVisibility));
    }

    private void OnCompletionMetadataReceived(object? sender, ChatCompletionMetadata e)
    {
        SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].Metadata = e;
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(string prompt)
    {
        if (string.IsNullOrWhiteSpace(InputingPrompt)) return;

        InputingPrompt = string.Empty;
        SelectedDiscussItemViewModel.MessagesViewModel.Add(new ApplicationChatMessage
        {
            UserPrompt = prompt,
            AiChatCompletion = new()
            {
                ReasoningContent = "",
                Content = []
            },
            TokenUsage = new(),
            ProgressStatus = ProgressStatus.InProgress
        });

        if (SelectedDiscussItemViewModel.ChatOptionsViewModel.SeedAutoRefresh)
            RandomSeed();

        _sendCommand.Execute(prompt);
    }

    [RelayCommand]
    public async Task DetailEditSystemPrompt()
    {
        TextBox textBox = new()
        {
            MaxLength = int.MaxValue,
            AcceptsReturn = true,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
            TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap
        };
        ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Visible);
        textBox.Text = SelectedDiscussItemViewModel.ChatOptionsViewModel.SystemPrompt;
        var contentDialog = ContentDialogHelper.CreateContentDialog(
            "EditSystemPromptDialogHeader".GetLocalized("DiscussionPage"),
            textBox,
            "ConfirmText".GetLocalized(),
            "CancelText".GetLocalized(),
            null,
            ContentDialogButton.Primary,
            MainPage.Current.Content.XamlRoot);
        if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
        {
            SelectedDiscussItemViewModel.ChatOptionsViewModel.SystemPrompt = textBox.Text;
        }
    }


    private bool CanSend() => !string.IsNullOrWhiteSpace(InputingPrompt) && _sendCommand.CanExecute(null);

    [RelayCommand]
    public void RandomSeed()
    {
        SelectedDiscussItemViewModel.ChatOptionsViewModel.Seed = Random.Shared.Next();
    }

    [RelayCommand]
    public async Task AddFile(int nType)
    {
        FileType type = (FileType)nType;
        var filePicker = new FileOpenPicker();
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Current));

        filePicker.FileTypeFilter.Add("*");
        filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        filePicker.ViewMode = type switch
        {
            FileType.Document => PickerViewMode.List,
            FileType.Media => PickerViewMode.Thumbnail
        };

        var pickedFile = await filePicker.PickSingleFileAsync();
        if(pickedFile == null)
            return;

        var fileModel = await _fileManagerService.CreateFileReferenceAsync(pickedFile, SelectedDiscussItemViewModel.Id.ToString(), type);
        var realType = FileTypeChecker.GetFileType(pickedFile.Path);
        var expectedType = type;

        if (realType == CheckFileType.Unknown || type switch { FileType.Document => realType is not CheckFileType.Text and not CheckFileType.Unknown, FileType.Media => realType is not CheckFileType.Image})
        {
            var result = await ContentDialogHelper.ShowMessageDialog("未知的文件类型", $"当前正在添加{(type == FileType.Document ? ("文档/文本") : ("图片"))}文件，但当前文件非此格式，是否以{(type == FileType.Document ? ("图片") : ("文档/文本"))}格式上传？", "是", "取消", "我还是想这样上传", ContentDialogButton.Close, MainPage.Current.XamlRoot);
            if(result == ContentDialogResult.Primary) 
            {
                expectedType = (FileType)(((int)expectedType + 1) % 2); // Toggle between Text and Image
                fileModel.Type = expectedType;
            }
            else if(result != ContentDialogResult.Secondary)
            {
                return; // User chose to cancel
            }
        }

        var fileVm = SelectedDiscussItemViewModel.FilesViewModel.Add(fileModel);
        AnalyzingFileContent(fileVm, realType == CheckFileType.Unknown? expectedType switch { FileType.Document => CheckFileType.Text, FileType.Media => CheckFileType.Image } : realType);
    }

    private async Task AnalyzingFileContent(FileViewModel fileVm, CheckFileType fileType)
    {
        var fileInstance = await _fileManagerService.GetStorageFileAsync(SelectedDiscussItemViewModel.Id.ToString(), fileVm.Name);

        if (fileType == CheckFileType.Text)
        {
            fileVm.Content = await FileIO.ReadTextAsync(fileInstance);
            fileVm.Status = AnalyzeStatus.Already;
            return;
        }

        if (fileType == CheckFileType.Document)
        {
            string mimeTypeString;
            var mimeType = FileTypeChecker.GetFileMimeType(fileInstance.Path);
            if (mimeType == null)
            {
                mimeTypeString = FileTypeChecker.GetMimeTypeByExtension(fileInstance.FileType);
            }
            else
            {
                mimeTypeString = mimeType.MimeType;
            }

            fileVm.Content = DocumentHelper.ExtractText(mimeTypeString, fileInstance.Path);
            fileVm.Status = AnalyzeStatus.Already;
            return;
        }

        if (fileType == CheckFileType.Image)
        {
            fileVm.Content = await App.Current.GetService<OcrService>().DelectTextAsync(fileInstance);
            fileVm.Status = AnalyzeStatus.Already;
            return;
        }
    }


    public void RemoveFile(FileViewModel fileVm)
    {
        if (fileVm == null) return;
        if (SelectedDiscussItemViewModel.FilesViewModel.FileViewModels.Contains(fileVm))
        {
            SelectedDiscussItemViewModel.FilesViewModel.Remove(fileVm.InnerObject);
            _fileManagerService.RemoveFileReferenceAsync(SelectedDiscussItemViewModel.Id.ToString(),fileVm.InnerObject.Name);
        }
    }

    public void StopGenerating()
    {
        _sendCommand.Cancel();
        SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].ProgressStatus = ProgressStatus.Stoped;
    }

    private void OnFunctionCalling(object? sender, ChatResponseFunctionCallingReceivedEventArgs e) // as known, deepseek doesn't call tools in reasoning, so we can safely assume that the last message "content" part is the one we are working on
    {
        if(e.Data is ToolCallingItem item)
        {
            SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].AiChatCompletion.AddContentViewModel(new ToolCallingContentPartViewModel(new ToolCallingContentPart()
            {
                Arguments = item.Function.Arguments.Select(x => KeyValuePair.Create(x.Key, x.Value.ToString())).ToDictionary(),
                Name = item.Function.Name,
                Id = item.Id
            }));
        }
        else
        {
            ((ToolCallingContentPartViewModel)SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].AiChatCompletion.ContentViewModels[^1]).Result = e.Data.ToString() ?? "null";
        }
    }

    private void OnStreamResponseReceived(object sender, ChatResponseReceivedEventArgs e)
    {
        var messageVm = SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1];
        if (e.Type == UpdateType.Reasoning) // as known, deepseek doesn't call tools in reasoning
        {
            messageVm.AiChatCompletion.ReasoningContent += e.ContentUpdate;
        }
        else if (e.Type == UpdateType.Content)
        {
            if (messageVm.AiChatCompletion.ContentViewModels.Any() && messageVm.AiChatCompletion.ContentViewModels[^1] is TextContentPartViewModel textContentPartVm)
            {
                textContentPartVm.Text += e.ContentUpdate;
            }
            else
            {
                messageVm.AiChatCompletion.AddContentViewModel(new TextContentPartViewModel(new TextContentPart() { Text = e.ContentUpdate }));
            }
        }
        SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].TokenUsage = e.TokenUsage;
    }

    private void OnStreamCompleted(object sender, ChatResponseCompletedEventArgs e)
    {
        if(SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].ProgressStatus != ProgressStatus.Stoped)
            SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels[^1].ProgressStatus = e.Status;

        SelectedDiscussItemViewModel.LeastStatus = e.Status;

        if (MainPage.Current.ViewModel.SelectedDiscussItem.Id != SelectedDiscussItemViewModel.Id)
            SelectedDiscussItemViewModel.IsViewed = false;
        else
            SelectedDiscussItemViewModel.IsViewed = true;
    }

    public event EventHandler ScrollToBottomRequested;
}
