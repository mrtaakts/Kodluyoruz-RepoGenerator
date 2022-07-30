using Kodluyoruz_RepoGenerator.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Kodluyoruz_RepoGenerator.Services
{
    public class GithubService : IGithubService
    {
        protected readonly IConfiguration _configuration;

        public GithubService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CloneTemplateRepositoryBulk(List<string> memberList, Dictionary<string, string> templateList)
        {
            int counter = 0;

            foreach (var item in templateList)
            {
                string template_owner = item.Value;
                string template_repo = item.Key;
                foreach (var member in memberList)
                {
                    var client = new RestClient($"https://api.github.com/repos/{template_owner}/{template_repo}/generate");
                    var request = new RestRequest();
                    var token = _configuration.GetValue<string>("GithubToken");
                    request.Method = Method.Post;
                    request.AddHeader("Accept", "application/vnd.github+json");
                    request.AddHeader("Authorization", $"token {token}");
                    request.AddBody(new { name = $"{template_repo}-{member}", owner = template_owner });
                    var response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        counter++;
                    }
                    else if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        continue;
                    }
                    else
                    {
                        continue;
                    }


                }
            }
            return $"Başarıyla eklenmiş repo sayısı : {counter}";
        }

        public ResponseModel<List<string>> GetMemberList()
        {
            try
            {
                var url = _configuration.GetValue<string>("GithubUrl:Members");
                var client = new RestClient($"https://api.github.com/orgs/{url}");
                var request = new RestRequest();
                var token = _configuration.GetValue<string>("GithubToken");
                request.Method = Method.Get;
                request.AddHeader("Accept", "application/vnd.github+json");
                request.AddHeader("Authorization", $"token {token}");
                request.AddParameter("role", "member");

                var response = client.Execute(request);
                var members = JsonConvert.DeserializeObject<List<MembersModel>>(response.Content);
                List<string> membersnames = new List<string>(members.Select(x => x.login.ToString()));

                return new ResponseModel<List<string>> { Data = membersnames, counter = membersnames.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<string>> { Data = new List<string> { ex.Message.ToString() },counter=0 };
            }


        }

        public ResponseModel<Dictionary<string, string>> GetTemplateRepoList()
        {
            try
            {
                var url = _configuration.GetValue<string>("GithubUrl:Repos");
                var client = new RestClient($"https://api.github.com/orgs/{url}");
                var request = new RestRequest();
                var token = _configuration.GetValue<string>("GithubToken");
                request.Method = Method.Get;
                request.AddHeader("Accept", "application/vnd.github+json");
                request.AddHeader("Authorization", $"token {token}");
                request.AddParameter("type", "private");

                var response = client.Execute(request);
                var repoList = JsonConvert.DeserializeObject<List<TemplateRepoModel>>(response.Content);

                Dictionary<string, string> templateRepositories = repoList.Where(x => x.is_template == true)
                .Select(x => new KeyValuePair<string, string>(x.name, x.owner.login))
                .ToDictionary(x => x.Key, x => x.Value);
                return new ResponseModel<Dictionary<string, string>> { Data = templateRepositories, counter = templateRepositories.Count };
            }
            catch(Exception ex)
            {
                return new ResponseModel<Dictionary<string, string>> { Data = new Dictionary<string, string>() {{ "1", ex.Message.ToString()}},counter=0 };
            }
           

            
        }
    }
}
