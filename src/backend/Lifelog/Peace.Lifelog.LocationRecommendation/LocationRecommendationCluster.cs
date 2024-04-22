namespace Peace.Lifelog.LocationRecommendation;

using System.Collections.Generic;
using DomainModels;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Security;

public class LocationRecommendationCluster : IClusterRequest
{
    public Response Cluster(Response response)
    {
        double[][] data = ExtractDataFromResponse(response.Output[4], response.Output[5], response);
        int numberOfClusters = DetermineNumberOfClusters(data);  // This should be adjusted based on data.

        var clusterResults = ClusterAlgorithm(data, numberOfClusters);
        var topClusters = SelectTopClusters(clusterResults.Clusters, 3);
        clusterResults.Clusters = topClusters;

        response.Data = clusterResults;  // Assuming Response has a Data property to store results
        return response;
    }

    private static Cluster ClusterAlgorithm(double[][] data, int numberOfClusters)
    {
        Random random = new Random();
        List<double[]> centers = InitializeCenters(data, numberOfClusters, random);
        bool changed;
        int[] assignments = new int[data.Length];

        do
        {
            changed = AssignDataPointsToCenters(data, centers, assignments);
            UpdateCenters(data, centers, assignments);
        } while (changed);

        var (clusters, radii) = FinalizeClusters(data, centers, assignments);
        return new Cluster { Clusters = clusters, Centers = centers, Radii = radii };
    }

    private static List<List<double[]>> SelectTopClusters(List<List<double[]>> clusters, int count)
    {
        return clusters.OrderByDescending(c => c.Count).Take(count).ToList();
    }

    private static List<double[]> InitializeCenters(double[][] data, int numberOfClusters, Random random)
    {
        return Enumerable.Range(0, numberOfClusters).Select(_ => data[random.Next(data.Length)]).ToList();
    }

    private static bool AssignDataPointsToCenters(double[][] data, List<double[]> centers, int[] assignments)
    {
        bool changed = false;
        for (int i = 0; i < data.Length; i++)
        {
            int nearestCenterIndex = FindNearestCenter(data[i], centers);
            if (assignments[i] != nearestCenterIndex)
            {
                assignments[i] = nearestCenterIndex;
                changed = true;
            }
        }
        return changed;
    }

    private static void UpdateCenters(double[][] data, List<double[]> centers, int[] assignments)
    {
        for (int i = 0; i < centers.Count; i++)
        {
            var points = data.Where((_, index) => assignments[index] == i).ToArray();
            if (points.Any())
                centers[i] = CalculateMean(points);
        }
    }

    private static (List<List<double[]>>, List<double>) FinalizeClusters(double[][] data, List<double[]> centers, int[] assignments)
    {
        List<List<double[]>> clusters = new List<List<double[]>>();
        List<double> radii = new List<double>();

        for (int i = 0; i < centers.Count; i++)
        {
            var clusterPoints = data.Where((_, index) => assignments[index] == i).ToList();
            clusters.Add(clusterPoints);
            double radius = clusterPoints.Select(point => Distance(point, centers[i])).Max();
            radii.Add(radius);
        }

        return (clusters, radii);
    }

    private static int FindNearestCenter(double[] point, List<double[]> centers)
    {
        double minDistance = double.MaxValue;
        int nearestIndex = -1;

        for (int i = 0; i < centers.Count; i++)
        {
            double distance = Distance(point, centers[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    private static double[] CalculateMean(double[][] points)
    {
        int dimensions = points[0].Length;
        double[] mean = new double[dimensions];
        for (int dim = 0; dim < dimensions; dim++)
        {
            mean[dim] = points.Average(point => point[dim]);
        }
        return mean;
    }

    private static double Distance(double[] point1, double[] point2)
    {
        double sum = 0;
        for (int i = 0; i < point1.Length; i++)
        {
            sum += Math.Pow(point1[i] - point2[i], 2);
        }
        return Math.Sqrt(sum);
    }

    private double[][] ExtractDataFromResponse(List<object> lat, List<object> lng, Response response)
    {
        // Assuming response.Data is in a suitable format
        /*
        List<double[]> list1 = new List<double[]>();
        List<double[]> list2 = new List<double[]>();
        if(response.Output != null)
        {
            foreach(List<object> output in lat)
            {
                list1.Add(new double[] {double.Parse(output[])});
            }
        }
        return response.Data as double[][];
        */
        double[][] result = new double[lat.Count][];

        for (int i = 0; i < lat.Count; i++)
        {
            result[i] = new double[] { (double)lat[i], (double)lng[i] };
        }

        return result;
    }

    private int DetermineNumberOfClusters(double[][] data)
    {
        // Placeholder logic, potentially use a method to calculate optimal cluster number
        return Math.Min(3, data.Length);  // Example to determine based on data
    }
}