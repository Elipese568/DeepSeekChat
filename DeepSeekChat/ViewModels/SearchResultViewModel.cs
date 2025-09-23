using DeepSeekChat.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.ViewModels;

public class SearchResultViewModel
{
    public string QueryString { get; set; }
    public List<DiscussionsResultGroup> DiscussionResults { get; set; }
    public List<MessagesResultGroup> MessageResults { get; set; }
}
