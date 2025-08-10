using DeepSeekChat.Helper;
using DeepSeekChat.Service;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls;

public enum MessageSource
{
    Assistant,
    User
}

public sealed partial class MessageItem : ContentControl
{
    private ContentPresenter _contentHost;
    private Grid _root;
    private PersonPicture _avatar;

    public MessageItem()
    {
        DefaultStyleKey = typeof(MessageItem);
    }

    public HorizontalAlignment Side
    {
        get { return (HorizontalAlignment)GetValue(SideProperty); }
        set { SetValue(SideProperty, value); }
    }

    public static readonly DependencyProperty SideProperty =
        DependencyProperty.Register("Side", typeof(HorizontalAlignment), typeof(MessageItem), new PropertyMetadata(HorizontalAlignment.Left));

    public ImageSource AvatarSource
    {
        get { return (ImageSource)GetValue(AvatarSourceProperty); }
        set { SetValue(AvatarSourceProperty, value); }
    }

    public static readonly DependencyProperty AvatarSourceProperty =
        DependencyProperty.Register("AvatarSource", typeof(ImageSource), typeof(MessageItem), new PropertyMetadata(null));

    public MessageSource MessageSource
    {
        get { return (MessageSource)GetValue(MessageSourceProperty); }
        set { SetValue(MessageSourceProperty, value); }
    }

    public static readonly DependencyProperty MessageSourceProperty =
        DependencyProperty.Register("MessageSource", typeof(MessageSource), typeof(MessageItem), new PropertyMetadata(MessageSource.Assistant));

    protected override void OnApplyTemplate()
    {
        HorizontalAlignment = Side;
        _contentHost = GetTemplateChild("PART_Content") as ContentPresenter;
        _avatar = GetTemplateChild("PART_Avatar") as PersonPicture;
        _root = GetTemplateChild("PART_Root") as Grid;
        UpdateContentVisual();
        base.OnApplyTemplate();
    }
    private void UpdateContentVisual()
    {
        if (_contentHost == null) return;
        _contentHost.Content = null;

        if (Content is string str)
        {
            var textContent = "TextMessageTemplate".GetResource<DataTemplate>(rd: Application.Current.Resources).LoadContent() as FrameworkElement;

            textContent.DataContext = str;
            _contentHost.Content = textContent;
        }
        else
        {
            _contentHost.Content = Content;
        }

        switch(Side)
        {
            case HorizontalAlignment.Left:
                _root.ColumnDefinitions[0].Width = new(32);
                _root.ColumnDefinitions[1].Width = new(1, GridUnitType.Star);
                Grid.SetColumn(_avatar, 0);
                Grid.SetColumn(_contentHost, 1);
                break;
            case HorizontalAlignment.Right:
                _root.ColumnDefinitions[0].Width = new(1, GridUnitType.Star);
                _root.ColumnDefinitions[1].Width = new(32);
                Grid.SetColumn(_avatar, 1);
                Grid.SetColumn(_contentHost, 0);
                break;
        }

        if(AvatarSource == null)
        {
            switch(MessageSource)
            {
                case MessageSource.Assistant:
                    _avatar.ProfilePicture = App.Current.GetService<AvatarManagerService>().GetSelectedAiAvatarViewModel().ImageSource;
                    break;
                case MessageSource.User:
                    _avatar.ProfilePicture = App.Current.GetService<AvatarManagerService>().GetSelectedUserAvatarViewModel().ImageSource;
                    break;
            }
        }
    }
}
