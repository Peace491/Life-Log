namespace Peace.Lifelog.LifetreeService.Contracts;

using DomainModels;
using Peace.Lifelog.PersonalNote;
using System.Threading.Tasks;



    public interface ILifetreeService
    {

        public Task<Response> getAllCompletedLLI(string userHash);

        public Task<Response> GetOnePNWithLifetree(string userHash, PN pn);

        public Task<Response> CreatePNWithLifetree(string userHash, PN pn);

    }

