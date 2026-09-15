using SufiChain.SufiBlazor.Contracts.Data;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Contracts.Data;

public class SbDataRequestTests
{
    [Fact]
    public void GetFilterValue_ReturnsMatchingField()
    {
        var request = new SbDataRequest
        {
            Filters =
            [
                new SbFilter("Subject", SbFilterOperator.Contains, "outage")
            ]
        };

        Assert.Equal("outage", request.GetFilterValue("subject"));
        Assert.Null(request.GetFilterValue("Number"));
        Assert.Equal("outage", request.GetFilterValue("Number", "Subject"));
    }
}
