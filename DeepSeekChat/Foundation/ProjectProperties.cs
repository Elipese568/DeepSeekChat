using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Foundation;

public static class ProjectProperties
{
    public const string AuthorName = "Elipese568";
    public const string GithubUrl = "https://github.com";
    public const string ProjectName = "DeepSeekChat";
    public const string AuthorGithubUrl = $"{GithubUrl}/{AuthorName}";
    public const string ProjectUrl = $"{AuthorGithubUrl}/{ProjectName}";
    public const string IssuesPageUrl = $"{ProjectUrl}/issues";
    public const string GitClonePath = $"{AuthorGithubUrl}/{ProjectName}.git";
    public const string GitCloneCommand = "git clone";
    public const string ProjectGitCloneCommand = $"{GitCloneCommand} {GitClonePath}";
}
