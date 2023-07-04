using Refresh.GameServer.Types.Levels;

namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameLevelResponse : IApiResponse, IDataConvertableFrom<ApiGameLevelResponse, GameLevel>
{
    [JsonProperty] public required int LevelId { get; set; }
    [JsonProperty] public required ApiGameUserResponse? Publisher { get; set; }

    [JsonProperty] public required string Title { get; set; }
    [JsonProperty] public required string IconHash { get; set; }
    [JsonProperty] public required string Description { get; set; }
    [JsonProperty] public required ApiGameLocationResponse Location { get; set; }
    
    [JsonProperty] public required DateTimeOffset PublishDate { get; set; }
    [JsonProperty] public required DateTimeOffset UpdateDate { get; set; }

    public static ApiGameLevelResponse? FromOld(GameLevel? level)
    {
        if (level == null) return null;
        
        return new ApiGameLevelResponse
        {
            Title = level.Title,
            Publisher = ApiGameUserResponse.FromOld(level.Publisher),
            LevelId = level.LevelId,
            IconHash = level.IconHash,
            Description = level.Description,
            Location = ApiGameLocationResponse.FromGameLocation(level.Location)!,
            PublishDate = DateTimeOffset.FromUnixTimeMilliseconds(level.PublishDate),
            UpdateDate = DateTimeOffset.FromUnixTimeMilliseconds(level.UpdateDate),
        };
    }

    public static IEnumerable<ApiGameLevelResponse> FromOldList(IEnumerable<GameLevel> oldList) => oldList.Select(FromOld)!;
}