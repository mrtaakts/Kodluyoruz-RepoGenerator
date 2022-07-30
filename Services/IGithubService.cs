using Kodluyoruz_RepoGenerator.Models;

namespace Kodluyoruz_RepoGenerator.Services
{
    public interface IGithubService
    {
        ResponseModel<Dictionary<string, string>> GetTemplateRepoList();
        ResponseModel<List<string>> GetMemberList();
        string CloneTemplateRepositoryBulk(List<string> memberList, Dictionary<string, string> templateList);
    }
}
