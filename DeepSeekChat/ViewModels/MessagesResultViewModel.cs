using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class MessagesResultViewModel
{
    public DiscussionItemViewModel DiscussionItemViewModel { get; set; }
    public MessagesViewModel MessagesViewModel { get; set; }
    public string QueryString { get; set; }
}
