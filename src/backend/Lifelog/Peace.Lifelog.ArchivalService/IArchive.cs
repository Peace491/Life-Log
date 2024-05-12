using System.Net;
using System.Runtime.CompilerServices;
using DomainModels;

namespace Peace.Lifelog.ArchivalService;

public interface IArchive
{
    public Task<Response> ArchiveFileToS3(string tableName);
}
