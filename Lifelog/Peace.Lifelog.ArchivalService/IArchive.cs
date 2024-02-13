using System.Net;
using DomainModels;

namespace Peace.Lifelog.ArchivalService;

public interface IArchive
{
    public Response Archive(DateTime dateTime);
}
