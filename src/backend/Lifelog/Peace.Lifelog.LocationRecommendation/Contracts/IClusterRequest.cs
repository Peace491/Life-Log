using DomainModels;

namespace Peace.Lifelog.LocationRecommendation;

public interface IClusterRequest
{
    public Response Cluster(Response response);
}
