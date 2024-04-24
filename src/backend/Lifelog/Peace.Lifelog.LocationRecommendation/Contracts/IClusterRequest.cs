using DomainModels;

namespace Peace.Lifelog.LocationRecommendation;

public interface IClusterRequest
{
    public Response ClusterRecommendation(Response response);
    public Response ClusterMarkerCoordinates(Response response);
    public Cluster ClusterRequest(Response response);
}
