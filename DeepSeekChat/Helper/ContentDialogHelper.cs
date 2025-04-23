using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace DeepSeekChat.Helper;

public class ContentDialogHelper
{
    private static ContentDialog CreateDialogInternal(
        string title,
        object content,
        string? primaryButtonText,
        string closeButtonText,
        string? secondaryButtonText,
        ContentDialogButton? defaultButton,
        XamlRoot xamlRoot,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? primaryButtonClickHandler = default,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? secondaryButtonClickHandler = default,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? closeButtonClickHandler = default)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = content,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            CloseButtonText = closeButtonText,
            DefaultButton = defaultButton??ContentDialogButton.None,
            XamlRoot = xamlRoot
        };

        dialog.PrimaryButtonClick += primaryButtonClickHandler;
        dialog.SecondaryButtonClick += secondaryButtonClickHandler;
        dialog.CloseButtonClick += closeButtonClickHandler;
        return dialog;
    }

    public static ContentDialog CreateMessageDialog(
        string title, 
        string message, 
        string closeButtonText, 
        string primaryButtonText,
        string secondaryButtonText,
        ContentDialogButton? defaultButton,
        XamlRoot xamlRoot)
    {
        return CreateDialogInternal(
            title,
            message,
            primaryButtonText,
            closeButtonText,
            secondaryButtonText,
            defaultButton,
            xamlRoot);
    }

    public static ContentDialog CreateContentDialog(
        string title,
        object content,
        string primaryButtonText,
        string closeButtonText,
        string secondaryButtonText,
        ContentDialogButton? defaultButton,
        XamlRoot xamlRoot,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? primaryButtonClickHandler = default,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? secondaryButtonClickHandler = default,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? closeButtonClickHandler = default)
    {
        return CreateDialogInternal(
            title,
            content,
            primaryButtonText,
            closeButtonText,
            secondaryButtonText,
            defaultButton,
            xamlRoot,
            primaryButtonClickHandler,
            secondaryButtonClickHandler,
            closeButtonClickHandler);
    }

    public static async Task<ContentDialogResult> ShowContentDialog(
        string title,
        object content,
        string primaryButtonText,
        string closeButtonText,
        string secondaryButtonText,
        ContentDialogButton? defaultButton,
        XamlRoot xamlRoot,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? primaryButtonClickHandler = default,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? secondaryButtonClickHandler = default,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? closeButtonClickHandler = default)
    {
        return await ShowContentDialog(CreateContentDialog(
            title,
            content,
            primaryButtonText,
            closeButtonText,
            secondaryButtonText,
            defaultButton,
            xamlRoot,
            primaryButtonClickHandler,
            secondaryButtonClickHandler,
            closeButtonClickHandler));
    }

    public static async Task<ContentDialogResult> ShowContentDialog(ContentDialog dialog)
    {
        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> ShowMessageDialog(
        string title,
        string message,
        string primaryButtonText,
        string closeButtonText,
        string secondaryButtonText,
        ContentDialogButton? defaultButton,
        XamlRoot xamlRoot)
    {
        return await ShowContentDialog(CreateMessageDialog(
            title,
            message,
            closeButtonText,
            primaryButtonText,
            secondaryButtonText,
            defaultButton,
            xamlRoot));
    }
}
