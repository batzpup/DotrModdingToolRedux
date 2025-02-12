﻿using System.Collections;
namespace DotrModdingTool2IMGUI;

public class DataAccess
{
    public const int DeckLeaderRankThresholdsByteOffset = 0x2A0952;
    public const int DeckLeaderRankThresholdByteLength = 24;

    public const int FusionListByteOffset = 0x26E930;
    public const int FusionListByteLength = 26540 * 4;

    public const int CardConstantsByteOffset = 0x28F180;
    public const int CardConstantByteLength = 20;
    public const int CardConstantCount = Card.TotalCardCount;

    public const int EnemyAiByteOffset = 0x28AFB0;
    public const int EnemyAiByteLength = 4;
    public const int EnemyAiCount = 32;

    public const int TreasureCardByteOffset = 0x2A09D0;
    public const int TreasureCardByteSize = 4;
    public const int TreasureCardCount = 22;

    public const int CardLeaderAbilitiesOffset = 0x293438;
    public const int CardLeaderAbilityCount = 683;
    public const int CardLeaderAbilityTypeCount = 20;
    public const int CardLeaderAbilityByteSize = 2;

    public const int MonsterEquipCardCompatabilityOffset = 0x26D680;
    public const int MonsterEquipCardCompabilityCardCount = 683;
    public const int MonsterEquipCardCompabilityByteSize = 7;

    public const int DeckByteOffset = 0x2A0A70;
    public const int DeckCount = 51;
    public const int DeckCardCount = 41;
    public const int DeckCardByteCount = 2;

    public const int EffectByteCount = 8;
    public const int MonsterEffectsOffset = 0x299EF0;
    public const int MonsterEffectsCount = 256;
    public const int MonsterEffectsTypeCount = 5;

    public const int MagicEffectsOffset = 0x29C6F0;
    public const int MagicEffectsCount = 171;

    public DotrMap[] maps = new DotrMap[46];
    private static readonly object FileStreamLock = new object();
    public static FileStream fileStream;
    public string filePath = null;
    public bool IsIsoLoaded = false;

    static DataAccess instance;

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
                int mapOffset = 0x29EF5C;
                mapOffset += i * 49;
                DotrMap map = maps[i];

                int x;
                int y;
                for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
                {
                    x = tileIndex % 7;
                    y = tileIndex / 7;
                    fileStream.Seek(mapOffset + tileIndex, SeekOrigin.Begin);
                    fileStream.Write(new byte[] { (byte)map.tiles[x, y] }, 0, 1);
                }
            }
            fileStream.Flush();
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

            int mapOffset = 0x29EF5C;
            mapOffset += mapIndex * 49;
            DotrMap map = maps[mapIndex];

            int x;
            int y;
            for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
            {
                x = tileIndex % 7;
                y = tileIndex / 7;
                fileStream.Seek(mapOffset + tileIndex, SeekOrigin.Begin);
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
                int mapOffset = 0x29EF5C;
                mapOffset += i * 0x31;

                byte[] slusMap = new byte[49];
                fileStream.Seek(mapOffset, SeekOrigin.Begin);

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
        if (fileStream != null)
        {

            fileStream.Dispose();
        }
        this.filePath = filePath;
        fileStream = new FileStream(filePath, FileMode.Open);
        IsIsoLoaded = true;

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
        Effects.MagicEffectsList.Clear();
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

            Effects.MagicEffectsList.Add(new Effect(effectByteSpan.ToArray()));
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

    public void ApplyPatches()
    {

    }
}