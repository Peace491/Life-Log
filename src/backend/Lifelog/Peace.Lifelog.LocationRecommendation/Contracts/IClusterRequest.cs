using DomainModels;

namespace Peace.Lifelog.LocationRecommendation;

public interface IClusterRequest
{
    public Cluster ClusterRequest(Response response);
}
