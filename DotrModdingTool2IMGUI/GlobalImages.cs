using Raylib_cs;
namespace DotrModdingTool2IMGUI;

public class GlobalImages
{
    static GlobalImages instance;
    public Dictionary<ETerrainImages, IntPtr> Terrain = new Dictionary<ETerrainImages, IntPtr>();
    public Dictionary<EEnemyImages, IntPtr> Enemies = new Dictionary<EEnemyImages, IntPtr>();
    public Dictionary<string, IntPtr> Cards = new Dictionary<string, IntPtr>();
    public Dictionary<CardColourType, IntPtr> CardFrames = new Dictionary<CardColourType, IntPtr>();
    public Dictionary<AttributeVisual, IntPtr> CardElements = new Dictionary<AttributeVisual, IntPtr>();
    public Dictionary<DeckLeaderRank,IntPtr> LeaderRanks = new Dictionary<DeckLeaderRank,IntPtr>();
    
    public static GlobalImages Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GlobalImages();
            }
            return instance;
        }
    }

    public void LoadAllImages()
    {
        LoadTerrainImages();
        LoadEnemyImages();
        LoadCardImages();
        LoadCardFrames();
        LoadCardElements();
        LoadLeaderRanks();
    }

    void LoadLeaderRanks()
    {
        LeaderRanks.Add(DeckLeaderRank.NCO,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.NCO.png"));
        LeaderRanks.Add(DeckLeaderRank.LT2,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.1LT.png"));
        LeaderRanks.Add(DeckLeaderRank.LT1,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.2LT.png"));
        LeaderRanks.Add(DeckLeaderRank.CPT,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.CPT.png"));
        LeaderRanks.Add(DeckLeaderRank.MAJ,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.MAJ.png"));
        LeaderRanks.Add(DeckLeaderRank.LTC,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.LTC.png"));
        LeaderRanks.Add(DeckLeaderRank.COL,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.COL.png"));
        LeaderRanks.Add(DeckLeaderRank.BG,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.BG.png"));
        LeaderRanks.Add(DeckLeaderRank.RADM,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.RADM.png"));
        LeaderRanks.Add(DeckLeaderRank.VADM,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.VADM.png"));
        LeaderRanks.Add(DeckLeaderRank.ADM,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.ADM.png"));
        LeaderRanks.Add(DeckLeaderRank.SADM,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.SADM.png"));
        LeaderRanks.Add(DeckLeaderRank.SD,ImageHelper.LoadImageImgui($"Images.deckLeaderRanks.SD.png"));
        
    }

    void LoadCardElements()
    {
        CardElements.Add(AttributeVisual.Light, ImageHelper.LoadImageImgui($"Images.cardExtras.light.png"));
        CardElements.Add(AttributeVisual.Dark, ImageHelper.LoadImageImgui($"Images.cardExtras.dark.png"));
        CardElements.Add(AttributeVisual.Fire, ImageHelper.LoadImageImgui($"Images.cardExtras.fire.png"));
        CardElements.Add(AttributeVisual.Earth, ImageHelper.LoadImageImgui($"Images.cardExtras.earth.png"));
        CardElements.Add(AttributeVisual.Wind, ImageHelper.LoadImageImgui($"Images.cardExtras.wind.png"));
        CardElements.Add(AttributeVisual.Water, ImageHelper.LoadImageImgui($"Images.cardExtras.water.png"));
        CardElements.Add(AttributeVisual.Magic, ImageHelper.LoadImageImgui($"Images.cardExtras.magic.png"));
        CardElements.Add(AttributeVisual.Trap, ImageHelper.LoadImageImgui($"Images.cardExtras.trap.png"));
        
    }

    void LoadCardImages()
    {
        foreach (var name in Card.cardNameList)
        {
            //Always use default names for images
            Cards.Add(name.Default, ImageHelper.LoadImageImgui($"Images.MonsterImages.{name}.png"));
        }
    }

    void LoadCardFrames()
    {

        CardFrames.Add(CardColourType.NormalMonster, ImageHelper.LoadImageImgui($"Images.cardFrames.normalMonsterCardBorder.png"));
        CardFrames.Add(CardColourType.EffectMonster, ImageHelper.LoadImageImgui($"Images.cardFrames.effectMonsterCardBorder.png"));
        CardFrames.Add(CardColourType.Magic, ImageHelper.LoadImageImgui($"Images.cardFrames.magicCardBorder.png"));
        CardFrames.Add(CardColourType.Trap, ImageHelper.LoadImageImgui($"Images.cardFrames.trapCardBorder.png"));
        CardFrames.Add(CardColourType.Ritual, ImageHelper.LoadImageImgui($"Images.cardFrames.ritualCardBorder.png"));
        
    }

    void LoadTerrainImages()
    {
        Terrain.Add(ETerrainImages.RedRose, ImageHelper.LoadImageImgui("Images.RedRose.png"));
        Terrain.Add(ETerrainImages.WhiteRose, ImageHelper.LoadImageImgui("Images.WhiteRose.png"));
        Terrain.Add(ETerrainImages.Forest, ImageHelper.LoadImageImgui("Images.TerrainTiles.FOREST.PNG"));
        Terrain.Add(ETerrainImages.Wasteland, ImageHelper.LoadImageImgui("Images.TerrainTiles.WASTELAND.PNG"));
        Terrain.Add(ETerrainImages.Mountain, ImageHelper.LoadImageImgui("Images.TerrainTiles.MOUNTAIN.PNG"));
        Terrain.Add(ETerrainImages.Meadow, ImageHelper.LoadImageImgui("Images.TerrainTiles.MEADOW.PNG"));
        Terrain.Add(ETerrainImages.Sea, ImageHelper.LoadImageImgui("Images.TerrainTiles.SEA.PNG"));
        Terrain.Add(ETerrainImages.Dark, ImageHelper.LoadImageImgui("Images.TerrainTiles.DARK.PNG"));
        Terrain.Add(ETerrainImages.Toon, ImageHelper.LoadImageImgui("Images.TerrainTiles.TOON.PNG"));
        Terrain.Add(ETerrainImages.Normal, ImageHelper.LoadImageImgui("Images.TerrainTiles.NORMAL.PNG"));
        Terrain.Add(ETerrainImages.Labyrinth, ImageHelper.LoadImageImgui("Images.TerrainTiles.LABYRINTH.PNG"));
        Terrain.Add(ETerrainImages.Crush, ImageHelper.LoadImageImgui("Images.TerrainTiles.CRUSH.PNG"));
    }

    void LoadEnemyImages()
    {
        Enemies.Add(EEnemyImages.Simon, ImageHelper.LoadImageImgui("Images.EnemyFaces.Simon.png"));
        Enemies.Add(EEnemyImages.Seto, ImageHelper.LoadImageImgui("Images.EnemyFaces.Seto.png"));
        Enemies.Add(EEnemyImages.Weevil, ImageHelper.LoadImageImgui("Images.EnemyFaces.Weevil.png"));
        Enemies.Add(EEnemyImages.Rex, ImageHelper.LoadImageImgui("Images.EnemyFaces.Rex.png"));
        Enemies.Add(EEnemyImages.Keith, ImageHelper.LoadImageImgui("Images.EnemyFaces.Keith.png"));
        Enemies.Add(EEnemyImages.Ishtar, ImageHelper.LoadImageImgui("Images.EnemyFaces.Ishtar.png"));
        Enemies.Add(EEnemyImages.Necro, ImageHelper.LoadImageImgui("Images.EnemyFaces.Necromancer.png"));
        Enemies.Add(EEnemyImages.Darkness_Ruler, ImageHelper.LoadImageImgui("Images.EnemyFaces.Panic.png"));
        Enemies.Add(EEnemyImages.Lab_Ruler, ImageHelper.LoadImageImgui("Images.EnemyFaces.LabRuler.png"));
        Enemies.Add(EEnemyImages.Pegasus, ImageHelper.LoadImageImgui("Images.EnemyFaces.Pegasus.png"));
        Enemies.Add(EEnemyImages.Richard, ImageHelper.LoadImageImgui("Images.EnemyFaces.Richard.png"));
        Enemies.Add(EEnemyImages.Tea, ImageHelper.LoadImageImgui("Images.EnemyFaces.Tea.png"));
        Enemies.Add(EEnemyImages.Tristan, ImageHelper.LoadImageImgui("Images.EnemyFaces.Tristan.png"));
        Enemies.Add(EEnemyImages.Mai, ImageHelper.LoadImageImgui("Images.EnemyFaces.Mai.png"));
        Enemies.Add(EEnemyImages.Mako, ImageHelper.LoadImageImgui("Images.EnemyFaces.Mako.png"));
        Enemies.Add(EEnemyImages.Joey, ImageHelper.LoadImageImgui("Images.EnemyFaces.Joey.png"));
        Enemies.Add(EEnemyImages.Shadi, ImageHelper.LoadImageImgui("Images.EnemyFaces.Shadi.png"));
        Enemies.Add(EEnemyImages.Grandpa, ImageHelper.LoadImageImgui("Images.EnemyFaces.Grandpa.png"));
        Enemies.Add(EEnemyImages.Bakura, ImageHelper.LoadImageImgui("Images.EnemyFaces.Bakura.png"));
        Enemies.Add(EEnemyImages.Yugi, ImageHelper.LoadImageImgui("Images.EnemyFaces.Yugi.png"));
        Enemies.Add(EEnemyImages.MFL_SK, ImageHelper.LoadImageImgui("Images.EnemyFaces.MFL.png"));
        Enemies.Add(EEnemyImages.MFL_Chakra, ImageHelper.LoadImageImgui("Images.EnemyFaces.MFL.png"));
        //Probs some solution to only use the one image but im tired.

    }
}

