using System.Xml.Serialization;
using Refresh.GameServer.Authentication;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Endpoints.Game.DataTypes.Response;

[XmlRoot("user")]
public class GameUserResponse : IDataConvertableFrom<GameUserResponse, GameUser>
{
    [XmlAttribute("type")] public string Type { get; set; } = "user";
    [XmlIgnore] public required string IconHash { get; set; }
    [XmlElement("biography")] public required string Description { get; set; }
    [XmlElement("location")] public required GameLocation Location { get; set; }
    [XmlElement("planets")] public required string PlanetsHash { get; set; }
    
    [XmlElement("npHandle")] public SerializedUserHandle Handle { get; set; }
    [XmlElement("commentCount")] public int CommentCount { get; set; }
    [XmlElement("commentsEnabled")] public bool CommentsEnabled { get; set; }
    [XmlElement("favouriteSlotCount")] public int FavouriteLevelCount { get; set; }
    [XmlElement("favouriteUserCount")] public int FavouriteUserCount { get; set; }
    [XmlElement("lolcatftwCount")] public int QueuedLevelCount { get; set; }
    [XmlElement("heartCount")] public int HeartCount { get; set; }
    [XmlElement("photosByMeCount")] public int PhotosByMeCount { get; set; }
    [XmlElement("photosWithMeCount")] public int PhotosWithMeCount { get; set; }
    
    [XmlElement("freeSlots")] public int FreeSlots { get; set; }
    [XmlElement("lbp2FreeSlots")] public int FreeSlotsLBP2 { get; set; }
    [XmlElement("lbp3FreeSlots")] public int FreeSlotsLBP3 { get; set; }
    [XmlElement("entitledSlots")] public int EntitledSlots { get; set; }
    [XmlElement("lbp2EntitledSlots")] public int EntitledSlotsLBP2 { get; set; }
    [XmlElement("lbp3EntitledSlots")] public int EntitledSlotsLBP3 { get; set; }
    [XmlElement("lbp1UsedSlots")] public int UsedSlots { get; set; }
    [XmlElement("lbp2UsedSlots")] public int UsedSlotsLBP2 { get; set; }
    [XmlElement("lbp3UsedSlots")] public int UsedSlotsLBP3 { get; set; }
    [XmlElement("lbp2PurchasedSlots")] public int PurchasedSlotsLBP2 { get; set; }
    [XmlElement("lbp3PurchasedSlots")] public int PurchasedSlotsLBP3 { get; set; }
    
    public static GameUserResponse? FromOldWithExtraData(GameUser? old, TokenGame gameVersion)
    {
        if (old == null) return null;

        GameUserResponse response = FromOld(old)!;
        response.FillInExtraData(old, gameVersion);

        return response;
    }
    
    public static GameUserResponse? FromOld(GameUser? old)
    {
        if (old == null) return null;

        GameUserResponse response = new()
        {
            IconHash = old.IconHash,
            Description = old.Description,
            Location = old.Location,
            PlanetsHash = "0",
            
            Handle = SerializedUserHandle.FromUser(old),
            CommentCount = old.ProfileComments.Count,
            CommentsEnabled = true,
            FavouriteLevelCount = old.FavouriteLevelRelations.Count(),
            FavouriteUserCount = old.UsersFavourited.Count(),
            QueuedLevelCount = old.QueueLevelRelations.Count(),
            HeartCount = old.UsersFavouritingMe.Count(),
            PhotosByMeCount = old.PhotosByMe.Count(),
            PhotosWithMeCount = old.PhotosWithMe.Count(),
            
            EntitledSlots = 100,
            EntitledSlotsLBP2 = 100,
            EntitledSlotsLBP3 = 100,
            UsedSlots = 0,
            UsedSlotsLBP2 = 0,
            UsedSlotsLBP3 = 0,
            PurchasedSlotsLBP2 = 0,
            PurchasedSlotsLBP3 = 0,
        };

        return response;
    }

    public static IEnumerable<GameUserResponse> FromOldList(IEnumerable<GameUser> oldList) => oldList.Select(FromOld)!;
    
    public static IEnumerable<GameUserResponse> FromOldListWithExtraData(IEnumerable<GameUser> oldList, TokenGame gameVersion) 
        => oldList.Select(old => FromOldWithExtraData(old, gameVersion))!;

    private void FillInExtraData(GameUser old, TokenGame gameVersion)
    {
        this.PlanetsHash = gameVersion switch
        {
            TokenGame.LittleBigPlanet1 => "0",
            TokenGame.LittleBigPlanet2 => old.Lbp2PlanetsHash,
            TokenGame.LittleBigPlanet3 => old.Lbp3PlanetsHash,
            TokenGame.LittleBigPlanetVita => old.VitaPlanetsHash,
            TokenGame.LittleBigPlanetPSP => "0",
            TokenGame.Website => "0",
            _ => throw new ArgumentOutOfRangeException(nameof(gameVersion), gameVersion, null),
        };

        //Fill out the used slots
        switch (gameVersion)
        {
            case TokenGame.LittleBigPlanet3: {
                //Match all LBP3 levels
                this.UsedSlotsLBP3 = old.PublishedLevels.Count(x => x._GameVersion == (int)TokenGame.LittleBigPlanet3);
                this.FreeSlotsLBP3 = 100 - this.UsedSlotsLBP3;
                //Fill out LBP2/LBP1 levels
                goto case TokenGame.LittleBigPlanet2;
            }
            case TokenGame.LittleBigPlanet2: {
                //Match all LBP2 levels
                this.UsedSlotsLBP2 = old.PublishedLevels.Count(x => x._GameVersion == (int)TokenGame.LittleBigPlanet2);
                this.FreeSlotsLBP2 = 100 - this.UsedSlotsLBP2;
                //Fill out LBP1 levels
                goto case TokenGame.LittleBigPlanet1;
            }
            case TokenGame.LittleBigPlanetVita: { 
                //Match all LBP Vita levels
                this.UsedSlotsLBP2 = old.PublishedLevels.Count(x => x._GameVersion == (int)TokenGame.LittleBigPlanetVita);
                this.FreeSlotsLBP2 = 100 - this.UsedSlotsLBP2;
                break;
            }
            case TokenGame.LittleBigPlanet1: {
                //Match all LBP1 levels
                this.UsedSlots = old.PublishedLevels.Count(x => x._GameVersion == (int)TokenGame.LittleBigPlanet1);
                this.FreeSlots = 100 - this.UsedSlots;
                break;
            }
            case TokenGame.LittleBigPlanetPSP: {
                //Match all LBP PSP levels
                this.UsedSlots = old.PublishedLevels.Count(x => x._GameVersion == (int)TokenGame.LittleBigPlanetPSP);
                this.FreeSlots = 100 - this.UsedSlots;
                break;
            }
            case TokenGame.Website: break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameVersion), gameVersion, null);
        }
    }
}