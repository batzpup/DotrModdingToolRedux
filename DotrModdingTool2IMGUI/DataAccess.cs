using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography;
using DiscUtils.Iso9660;
namespace DotrModdingTool2IMGUI;

public class DataAccess
{
    public const int IsoSlusRamOffset = 0x2ff00;

    //Ram addresses plus offset
    public const int DeckLeaderRankThresholdsByteOffset = 0x2D0852 - IsoSlusRamOffset;
    public const int DeckLeaderRankThresholdByteLength = 24;

    public const int FusionListByteOffset = 0x29E830 - IsoSlusRamOffset;
    public const int FusionListByteLength = 26540 * 4;

    public const int CardConstantsByteOffset = 0x2BF080 - IsoSlusRamOffset;
    public const int CardConstantByteLength = 20;
    public const int CardConstantCount = Card.TotalCardCount;

    public const int EnemyAiByteOffset = 0x2BAEB0 - IsoSlusRamOffset;
    public const int EnemyAiByteLength = 4;
    public const int EnemyAiCount = 32;

    public const int TreasureCardByteOffset = 0x2D08D0 - IsoSlusRamOffset;
    public const int TreasureCardByteSize = 4;
    public const int TreasureCardCount = 22;

    public const int CardLeaderAbilitiesOffset = 0x2C3338 - IsoSlusRamOffset;
    public const int CardLeaderAbilityCount = 683;
    public const int CardLeaderAbilityTypeCount = 20;
    public const int CardLeaderAbilityByteSize = 2;

    public const int MonsterEquipCardCompatabilityOffset = 0x29D580 - IsoSlusRamOffset;
    public const int MonsterEquipCardCompabilityCardCount = 683;
    public const int MonsterEquipCardCompabilityByteSize = 7;

    public const int DeckByteOffset = 0x2D0970 - IsoSlusRamOffset;
    public const int DeckCount = 51;
    public const int DeckCardCount = 41;
    public const int DeckCardByteCount = 2;

    public const int EffectByteCount = 8;
    public const int MonsterEffectsOffset = 0x2C9DF0 - IsoSlusRamOffset;
    public const int MonsterEffectsCount = 256;
    public const int MonsterEffectsTypeCount = 5;

    public const int MagicEffectsOffset = 0x2CC5F0 - IsoSlusRamOffset;
    public const int MagicEffectsCount = 171;

    public const int EnchantIdsOffset = 0x29D4E0 - IsoSlusRamOffset;
    public const int EnchantIdSize = 2;
    public const int EnchantDataCount = 50;
    public const int EnchantScoresOffset = 0x29D512 - IsoSlusRamOffset;
    public const int EnchantScoresSize = 2;

    public const int DestinyCardsOffset = 0x002b8706 - IsoSlusRamOffset;
    public const int DestinyPoolCount = 7;
    public const int DestinyPoolSize = 6;

    
    public const int PictureIsoOffset = 0x1250800;
    public const int PictureSize = 0x4800;
    public const int PictureCount = 871;
    
    public const int PicPackIsoOffset = 0xe9b800;
    public const int PicPackSize = 0x4410;
    public const int PicPackCount = 223;
    
    
    
    public const int PicMiniOffset = 0xBE0800;
    public const int PicMiniSize = 0x1000;
    public const int PicMiniCount = 699;

    public const IntPtr picPackArtsSLUSArray = 0x2CE9FC - IsoSlusRamOffset;


    public static int OffsetTable = 0x2D19D0 - IsoSlusRamOffset;
    public static int TextDataTable = 0x2D49D4 - IsoSlusRamOffset;

    public static int EnglishOffsetStart = 0x2D1A48 - IsoSlusRamOffset;
    public static int EnglishTextStart = 0x2D4E0C - IsoSlusRamOffset;

    public static int TotalStringCount = 3073;
    public static int TotalTextLength = 74252 * 2;


    public DotrMap[] maps = new DotrMap[46];
    public static int MapOffset = 0x2CEE5C - IsoSlusRamOffset;

    static readonly object FileStreamLock = new object();
    public static FileStream fileStream;
    public bool IsIsoLoaded = false;

    static DataAccess instance;

    public string currentIsoPath;
    public string CurrentHash = String.Empty;
    public const string VanillaHash = "ca838dae26ff750936c0814d1c2d33f1";

    public static int ToRamOffset(int offset) => offset + 0x2FF00;
    public static int ToIsoOffset(int ramAddress) => ramAddress - 0x2FF00;

    public static DataAccess Instance
    {
        get
        {

            if (instance == null)
            {
                instance = new DataAccess();
            }

            return instance;
        }
    }

    public DataAccess()
    {
    }

    public void SaveMaps()
    {

        if (maps[0] == null)
        {
            return;
        }
        lock (FileStreamLock)
        {
            for (int i = 0; i < maps.Length; i++)
            {
                DotrMap map = maps[i];
                int x;
                int y;
                for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
                {
                    x = tileIndex % 7;
                    y = tileIndex / 7;
                    fileStream.Seek(MapOffset + (i * 49) + tileIndex, SeekOrigin.Begin);
                    fileStream.Write(new byte[] { (byte)map.tiles[x, y] }, 0, 1);
                }
            }
            fileStream.Flush();
        }
    }

    public async Task GetMd5HashAsync()
    {
        CurrentHash = string.Empty;

        lock (fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
        }
        using var md5 = MD5.Create();
        var sw = Stopwatch.StartNew();
        byte[] hash = md5.ComputeHash(fileStream);
        sw.Stop();
        Console.WriteLine($"Elapsed: {sw.Elapsed.TotalSeconds:F2}s");
        CurrentHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public void GetMd5Hash()
    {
        CurrentHash = string.Empty;

        lock (fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            var sw = Stopwatch.StartNew();
            using var md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(fileStream);
            sw.Stop();
            Console.WriteLine($"Elapsed: {sw.Elapsed.TotalSeconds:F2}s");
            CurrentHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

    }

    public void SaveMap(int mapIndex)
    {
        if (maps[mapIndex] == null)
        {
            return;
        }
        lock (FileStreamLock)
        {

            DotrMap map = maps[mapIndex];
            int x;
            int y;
            for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
            {
                x = tileIndex % 7;
                y = tileIndex / 7;
                fileStream.Seek(MapOffset + (mapIndex * 49) + tileIndex, SeekOrigin.Begin);
                fileStream.Write(new byte[] { (byte)map.tiles[x, y] }, 0, 1);
            }
            fileStream.Flush();
        }
    }


    public void LoadMapsFromIso()
    {
        lock (FileStreamLock)
        {

            for (int i = 0; i < maps.Length; i++)
            {
                byte[] slusMap = new byte[49];
                fileStream.Seek(MapOffset + (i * 49), SeekOrigin.Begin);

                for (int j = 0; j < slusMap.Length; j++)
                {
                    slusMap[j] = Convert.ToByte(fileStream.ReadByte());
                }

                maps[i] = new DotrMap(slusMap);
            }
            fileStream.Flush();
        }
    }

    public void SaveDeck(int deckIndex, byte[] bytes)
    {
        int deckBytesLocation = (DeckByteOffset) + deckIndex * DeckCardCount * DeckCardByteCount;

        lock (FileStreamLock)
        {
            fileStream.Seek(deckBytesLocation, SeekOrigin.Begin);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Flush();

        }
    }

    public byte[][][] LoadDecks()
    {
        byte[][][] allDeckBytes = new byte[DeckCount][][];
        byte[] buffer = new byte[DeckCount * DeckCardCount * DeckCardByteCount];

        lock (FileStreamLock)
        {
            fileStream.Seek(DeckByteOffset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }

        for (int deckIndex = 0; deckIndex < DeckCount; deckIndex++)
        {
            allDeckBytes[deckIndex] = new byte[DeckCardCount][];

            for (int cardIndex = 0; cardIndex < DeckCardCount; cardIndex++)
            {
                int cardByteStartLocation = (deckIndex * (DeckCardCount * DeckCardByteCount)) + (cardIndex * DeckCardByteCount);
                allDeckBytes[deckIndex][cardIndex] = new byte[] { buffer[cardByteStartLocation], buffer[cardByteStartLocation + 1] };
            }
        }

        return allDeckBytes;
    }

    public void LoadDecksData()
    {
        Deck.DeckList.Clear();
        Deck.DeckList = Deck.LoadDeckListFromBytes(LoadDecks());
    }

    public byte[][] LoadMonsterEquipCardData()
    {
        byte[][] monsterEquipData = new byte[MonsterEquipCardCompabilityCardCount][];

        lock (FileStreamLock)
        {
            MonsterEnchantData.MonsterEnchantDataList.Clear();
            for (int cardIndex = 0; cardIndex < MonsterEquipCardCompabilityCardCount; cardIndex++)
            {
                byte[] buffer = new byte[MonsterEquipCardCompabilityByteSize];
                int offset = MonsterEquipCardCompatabilityOffset + (cardIndex * MonsterEquipCardCompabilityByteSize);
                fileStream.Seek(offset, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, buffer.Length);
                monsterEquipData[cardIndex] = buffer;
                MonsterEnchantData.MonsterEnchantDataList.Add(new MonsterEnchantData(new BitArray(monsterEquipData[cardIndex])));

            }
        }
        return monsterEquipData;
    }

    public void SaveMonsterEnchantData(byte[] byteData)
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(DataAccess.MonsterEquipCardCompatabilityOffset, SeekOrigin.Begin);
            fileStream.Write(byteData, 0, byteData.Length);
            fileStream.Flush();
        }
    }

    public void SaveCardDeckLeaderAbilities(byte[] byteData)
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(DataAccess.CardLeaderAbilitiesOffset, SeekOrigin.Begin);
            fileStream.Write(byteData, 0, byteData.Length);
            fileStream.Flush();
        }
    }

    public void LoadCardDeckLeaderAbilities()
    {

        lock (FileStreamLock)
        {
            CardDeckLeaderAbilities.MonsterAbilities.Clear();
            for (int cardIndex = 0; cardIndex < CardLeaderAbilityCount; cardIndex++)
            {

                DeckLeaderAbility[] leaderAbilitiesValues = new DeckLeaderAbility[CardLeaderAbilityTypeCount];
                for (int abilityTypeIndex = 0; abilityTypeIndex < CardLeaderAbilityTypeCount; abilityTypeIndex++)
                {
                    byte[] buffer = new byte[CardLeaderAbilityByteSize];
                    int cardIndexOffset = (cardIndex * (CardLeaderAbilityTypeCount * CardLeaderAbilityByteSize)) +
                                          (abilityTypeIndex * CardLeaderAbilityByteSize);
                    fileStream.Seek(DataAccess.CardLeaderAbilitiesOffset + cardIndexOffset, SeekOrigin.Begin);
                    fileStream.Read(buffer, 0, buffer.Length);
                    leaderAbilitiesValues[abilityTypeIndex] = new DeckLeaderAbility(abilityTypeIndex, buffer);
                }
                CardDeckLeaderAbilities.MonsterAbilities.Add(new DeckLeaderAbilityInstance(cardIndex, leaderAbilitiesValues));
            }
        }

    }

    public void OpenIso(string filePath)
    {

        currentIsoPath = filePath;

        lock (FileStreamLock)
        {
            fileStream?.Dispose();
            fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.ReadWrite);
        }

        IsIsoLoaded = true;
    }

    public void LoadEnchantData()
    {
        EnchantData.EnchantIds.Clear();
        EnchantData.EnchantScores.Clear();
        ;
        lock (FileStreamLock)
        {
            for (int enchantId = 0; enchantId < EnchantDataCount; enchantId++)
            {
                byte[] buffer = new byte[1];
                fileStream.Seek(DataAccess.EnchantIdsOffset + enchantId, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, buffer.Length);
                EnchantData.EnchantIds.Add(buffer[0]);
            }
            for (int enchantScoreIndex = 0; enchantScoreIndex < EnchantDataCount; enchantScoreIndex++)
            {
                byte[] buffer = new byte[EnchantScoresSize];
                int cardIndexOffset = EnchantScoresSize * enchantScoreIndex;
                fileStream.Seek(DataAccess.EnchantScoresOffset + cardIndexOffset, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, buffer.Length);
                EnchantData.EnchantScores.Add(BitConverter.ToUInt16(buffer));
            }

        }
    }

    public void SaveEnchantData()
    {

        lock (FileStreamLock)
        {
            for (int enchantId = 0; enchantId < EnchantDataCount; enchantId++)
            {
                byte[] idBytes = EnchantData.EnchantIds.ToArray();
                fileStream.Seek(EnchantIdsOffset, SeekOrigin.Begin);
                fileStream.Write(idBytes, 0, idBytes.Length);
                fileStream.Flush();
            }
            for (int enchantScoreIndex = 0; enchantScoreIndex < EnchantDataCount; enchantScoreIndex++)
            {
                byte[] scoreBytes = EnchantData.EnchantIds.ToArray();
                fileStream.Seek(EnchantScoresOffset, SeekOrigin.Begin);
                fileStream.Write(EnchantData.EquipScoreBytes, 0, EnchantData.EquipScoreBytes.Length);
                fileStream.Flush();
            }
        }
    }

    public byte[][] LoadCardConstantData()
    {
        byte[][] cardConstantsBytes = new byte[CardConstantCount][];

        lock (FileStreamLock)
        {
            for (int cardIndex = 0; cardIndex < CardConstantCount; cardIndex++)
            {
                byte[] buffer = new byte[CardConstantByteLength];
                int cardIndexOffset = CardConstantByteLength * cardIndex;
                fileStream.Seek(DataAccess.CardConstantsByteOffset + cardIndexOffset, SeekOrigin.Begin);
                fileStream.Read(buffer, 0, buffer.Length);
                cardConstantsBytes[cardIndex] = buffer;
            }
        }

        return cardConstantsBytes;
    }

    public byte[][] WriteCardConstantData(byte[] byteData)
    {

        lock (FileStreamLock)
        {
            fileStream.Seek(DataAccess.CardConstantsByteOffset, SeekOrigin.Begin);
            fileStream.Write(byteData, 0, byteData.Length);
            fileStream.Flush();
        }

        return this.LoadCardConstantData();
    }

    public void LoadLeaderThresholdData()
    {
        byte[] buffer = new byte[24];

        lock (FileStreamLock)
        {
            fileStream.Seek(DeckLeaderRankThresholdsByteOffset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }
        for (int i = 0; i < 12; i++)
        {
            GameplayPatchesWindow.rankExp[i] = BitConverter.ToUInt16(buffer, i * 2);
        }

    }

    public void SaveDeckLeaderThresholds()
    {
        byte[] buffer = new byte[24];
        buffer = GameplayPatchesWindow.rankExp.SelectMany(a => BitConverter.GetBytes((ushort)a)).ToArray();
        lock (FileStreamLock)
        {
            fileStream.Seek(DeckLeaderRankThresholdsByteOffset, SeekOrigin.Begin);
            fileStream.Write(buffer, 0, DataAccess.DeckLeaderRankThresholdByteLength);
            fileStream.Flush();
        }


    }

    public void LoadFusionData()
    {
        FusionData.FusionTableData.Clear();
        byte[] buffer = new byte[FusionListByteLength];
        lock (FileStreamLock)
        {
            fileStream.Seek(FusionListByteOffset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }

        for (int i = 0; i < buffer.Length; i += 4)
        {
            byte[] fusionByteArray = new byte[] { buffer[i], buffer[i + 1], buffer[i + 2], buffer[i + 3] };
            FusionData.FusionTableData.Add(i / 4, new FusionData(fusionByteArray));
        }
    }


    public void SaveFusionData(byte[] byteData)
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(DataAccess.FusionListByteOffset, SeekOrigin.Begin);
            fileStream.Write(byteData, 0, byteData.Length);
            fileStream.Flush();
        }
    }

    public void LoadEffectData()
    {
        byte[] monsterEffectsBytes = new byte[EffectByteCount * MonsterEffectsCount * MonsterEffectsTypeCount];
        byte[] magicEffectsBytes = new byte[EffectByteCount * MagicEffectsCount];
        Effects.MonsterEffectsList.Clear();
        Effects.NonMonsterEffectsList.Clear();
        lock (FileStreamLock)
        {
            fileStream.Seek(MonsterEffectsOffset, SeekOrigin.Begin);
            fileStream.Read(monsterEffectsBytes, 0, monsterEffectsBytes.Length);
            fileStream.Seek(MagicEffectsOffset, SeekOrigin.Begin);
            fileStream.Read(magicEffectsBytes, 0, magicEffectsBytes.Length);
        }

        for (int i = 0; i < MonsterEffectsCount; i++)
        {
            MonsterEffects monsterEffect = new MonsterEffects();
            for (int j = 0; j < 5; j++)
            {
                int startIndex = i * 40 + j * 8;
                Span<byte> effectByteSpan = new Span<byte>(monsterEffectsBytes, startIndex, 8);
                monsterEffect.Effects[j] = new Effect(effectByteSpan.ToArray());
            }
            Effects.MonsterEffectsList.Add(monsterEffect);
        }

        for (int i = 0; i < MagicEffectsCount; i++)
        {
            Span<byte> effectByteSpan = new Span<byte>(magicEffectsBytes, i * 8, 8);

            Effects.NonMonsterEffectsList.Add(new Effect(effectByteSpan.ToArray()));
        }

        Effects.ReloadStrings();
    }

    public void SaveStringData()
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(EnglishOffsetStart, SeekOrigin.Begin);
            fileStream.Write(StringEditor.OffsetBytes.ToArray(), 0, StringEditor.OffsetBytes.Count);
            fileStream.Flush();

            fileStream.Seek(EnglishTextStart, SeekOrigin.Begin);
            fileStream.Write(StringEditor.StringBytes.ToArray(), 0, StringEditor.StringBytes.Count);
            fileStream.Flush();
        }
    }

    public void SaveEffectData(byte[] monsterEffectBytes, byte[] magicEffectsBytes)
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(MonsterEffectsOffset, SeekOrigin.Begin);
            fileStream.Write(monsterEffectBytes, 0, monsterEffectBytes.Length);
            fileStream.Flush();

            fileStream.Seek(MagicEffectsOffset, SeekOrigin.Begin);
            fileStream.Write(magicEffectsBytes, 0, magicEffectsBytes.Length);
            fileStream.Flush();
        }
    }

    public void LoadEnemyAIData()
    {
        Enemies.EnemyList.Clear();
        byte[] buffer = new byte[EnemyAiByteLength * EnemyAiCount];

        lock (FileStreamLock)
        {
            fileStream.Seek(DataAccess.EnemyAiByteOffset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }
        Enemies.LoadEnemies(buffer);

    }

    public void SaveEnemyAiData(byte[] byteData)
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(DataAccess.EnemyAiByteOffset, SeekOrigin.Begin);
            fileStream.Write(byteData, 0, byteData.Length);
            fileStream.Flush();
        }
    }

    public byte[] GetTreasureCardData()
    {
        byte[] buffer = new byte[DataAccess.TreasureCardByteSize * DataAccess.TreasureCardCount];

        lock (FileStreamLock)
        {

            fileStream.Seek(DataAccess.TreasureCardByteOffset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }


        return buffer;
    }

    public void SaveTreasureCard(int index, byte[] treasureCardBytes)
    {
        int writeOffset = DataAccess.TreasureCardByteOffset + (index * DataAccess.TreasureCardByteSize);

        lock (FileStreamLock)
        {

            fileStream.Seek(writeOffset, SeekOrigin.Begin);
            fileStream.Write(treasureCardBytes, 0, DataAccess.TreasureCardByteSize);
            fileStream.Flush();
        }
    }

    public void SaveAllTreasureCards()
    {
        lock (FileStreamLock)
        {
            for (int i = 0; i < TreasureCardCount; i++)
            {
                int writeOffset = TreasureCardByteOffset + (i * TreasureCardByteSize);
                fileStream.Seek(writeOffset, SeekOrigin.Begin);
                fileStream.Write(TreasureCards.Instance.Treasures[i].Bytes, 0, TreasureCardByteSize);
            }
            fileStream.Flush();
        }
    }

    public void LoadDestinyDrawCards()
    {

        byte[] buffer = new byte[DestinyPoolSize * DestinyPoolCount];

        lock (FileStreamLock)
        {
            fileStream.Seek(DestinyCardsOffset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }
        for (int i = 0; i < DestinyPoolCount; i++)
        {
            int byteOffset = i * DestinyPoolSize;
            ushort[] ushorts = new ushort[DestinyPoolSize / 2];
            Buffer.BlockCopy(buffer, byteOffset, ushorts, 0, DestinyPoolSize);
            DestinyDrawData.DestinyCardPools[i] = Array.ConvertAll(ushorts, x => (int)x);
        }
    }

    public void SaveDestinyDrawCards()
    {
        for (var i = 0; i < DestinyDrawData.DestinyCardPools.Length; i++)
        {
            lock (FileStreamLock)
            {
                int writeOffset = DestinyCardsOffset + (i * DestinyPoolSize);
                byte[] buffer = new byte[DestinyPoolSize];
                ushort[] ushorts = Array.ConvertAll(DestinyDrawData.DestinyCardPools[i], x => (ushort)x);
                Buffer.BlockCopy(ushorts, 0, buffer, 0, buffer.Length);
                fileStream.Seek(writeOffset, SeekOrigin.Begin);
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }
    }

    public bool CheckIfPatchApplied(int offset, byte[] patch)
    {
        byte[] buffer = new byte[patch.Length];

        lock (FileStreamLock)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, buffer.Length);
        }

        return buffer.SequenceEqual(patch);
    }


    public void ApplyPatch(int offset, byte[] patch)
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            fileStream.Write(patch, 0, patch.Length);
            fileStream.Flush();
        }
    }

    public byte[] ReadBytes(int offset, int length)
    {

        byte[] bytes = new byte[length];
        lock (FileStreamLock)
        {

            fileStream.Seek(offset, SeekOrigin.Begin);
            fileStream.Read(bytes, 0, bytes.Length);
        }
        return bytes;
    }


    public void NopInstructions(int offset, int instructions)
    {
        int length = instructions * 4;
        lock (FileStreamLock)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            for (int i = 0; i < length; i++)
            {
                fileStream.WriteByte(0x00);
            }
            fileStream.Flush();
        }
    }


    public void WritePicPackImage(int CardArtIndex, int PicPackIndex)
    {
        byte[] picture = GameImageManager.PictureBytes[CardArtIndex];
        if (picture.Length == PictureSize)
        {
            lock (FileStreamLock)
            {
                fileStream.Seek(PicPackIsoOffset + PicPackIndex * PicPackSize, SeekOrigin.Begin);
                fileStream.Write(GameImageManager.ConvertPictureToPicPack(picture), 0, PicPackSize);
                fileStream.Flush();
            }
        }
    }

    public void LoadImageData()
    {
        lock (FileStreamLock)
        {
            fileStream.Seek(PictureIsoOffset, SeekOrigin.Begin);
            for (int i = 0; i < PictureCount; i++)
            {
                byte[] picture = new byte[PictureSize];
                fileStream.ReadExactly(picture, 0, PictureSize);
                GameImageManager.PictureBytes[i] = picture;
            }

            fileStream.Seek(PicPackIsoOffset, SeekOrigin.Begin);
            for (int i = 0; i < PicPackCount; i++)
            {
                byte[] picture = new byte[PicPackSize];
                fileStream.ReadExactly(picture, 0, PicPackSize);
                GameImageManager.PicPackBytes[i] = picture;
                GameImageManager.PicPackImages.TryAdd(i, GameImageManager.GetPicNumber(GameImageManager.PicPackBytes[i]));
            }
            fileStream.Seek(PicMiniOffset, SeekOrigin.Begin);
            for (int i = 0; i < PicMiniCount; i++)
            {
                byte[] picture = new byte[PicMiniSize];
                fileStream.ReadExactly(picture, 0, PicMiniSize);
                GameImageManager.PicMiniBytes[i] = picture;
            }
            ImageCreator.CreateImageFromBytes(GameImageManager.PictureBytes[0], ImageMrgFile.Picture, Path.Combine(Directory.GetCurrentDirectory(), ""), false);


        }
    }


    public void SaveImageData()
    {
        if (fileStream != null)
        {
            lock (FileStreamLock)
            {
                
                for (int i = 0; i < PictureCount; i++)
                {
                    fileStream.Seek(PictureIsoOffset + i * PictureSize, SeekOrigin.Begin);
                    byte[] picture = GameImageManager.PictureBytes[i];
                    fileStream.Write(picture, 0, PictureSize);
                    fileStream.Flush();
                }

                for (int i = 0; i < GameImageManager.PicPackImages.Count; i++)
                {
                    if (GameImageManager.PicPackImages[i] == -1)
                    {
                        continue;
                    }
                    WritePicPackImage(GameImageManager.PicPackImages[i], i);
                    byte[] bytes = BitConverter.GetBytes((ushort)GameImageManager.PicPackImages[i]);
                    lock (FileStreamLock)
                    {
                        fileStream.Seek(picPackArtsSLUSArray + i * 2, SeekOrigin.Begin);
                        fileStream.Write(bytes, 0, 2);
                        fileStream.Flush();
                    }
                }
                
                for (int i = 0; i < PicMiniCount; i++)
                {
                    fileStream.Seek(PicMiniOffset + i * PicMiniSize, SeekOrigin.Begin);
                    byte[] picture = GameImageManager.PicMiniBytes[i];
                    fileStream.Write(picture, 0, PicMiniSize);
                    fileStream.Flush();
                }
            }
        }
    }
}